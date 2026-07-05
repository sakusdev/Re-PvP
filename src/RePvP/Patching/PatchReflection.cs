using System;
using System.Linq;
using System.Reflection;

namespace RePvP;

public static class PatchReflection
{
    public static Type? FindTypeByName(params string[] typeNameCandidates)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).Cast<Type>().ToArray();
            }
            catch
            {
                continue;
            }

            foreach (var candidate in typeNameCandidates)
            {
                var exact = types.FirstOrDefault(t => string.Equals(t.FullName, candidate, StringComparison.Ordinal)
                    || string.Equals(t.Name, candidate, StringComparison.Ordinal));
                if (exact != null)
                {
                    return exact;
                }
            }
        }

        return null;
    }

    public static MethodInfo? FindMethod(Type type, params string[] methodNameCandidates)
    {
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var name in methodNameCandidates)
        {
            var method = methods.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.Ordinal));
            if (method != null)
            {
                return method;
            }
        }

        return null;
    }

    public static int TryReadIntMember(object? instance, params string[] memberNameCandidates)
    {
        if (instance == null)
        {
            return 0;
        }

        var type = instance.GetType();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (var name in memberNameCandidates)
        {
            var field = type.GetField(name, flags);
            if (field != null && TryConvertToInt(field.GetValue(instance), out var fieldValue))
            {
                return fieldValue;
            }

            var property = type.GetProperty(name, flags);
            if (property != null && TryConvertToInt(property.GetValue(instance), out var propertyValue))
            {
                return propertyValue;
            }
        }

        return 0;
    }

    public static int TryReadIntArgument(object[]? args, MethodBase? originalMethod, params string[] argumentNameCandidates)
    {
        if (args == null || args.Length == 0)
        {
            return 0;
        }

        var parameters = originalMethod?.GetParameters() ?? Array.Empty<ParameterInfo>();
        for (var i = 0; i < args.Length; i++)
        {
            if (i < parameters.Length && argumentNameCandidates.Any(name => string.Equals(parameters[i].Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                if (TryConvertToInt(args[i], out var namedValue))
                {
                    return namedValue;
                }
            }
        }

        foreach (var arg in args)
        {
            if (TryConvertToInt(arg, out var directValue))
            {
                return directValue;
            }

            var memberValue = TryReadIntMember(arg, argumentNameCandidates);
            if (memberValue > 0)
            {
                return memberValue;
            }
        }

        return 0;
    }

    private static bool TryConvertToInt(object? value, out int result)
    {
        switch (value)
        {
            case int i:
                result = i;
                return true;
            case float f:
                result = (int)f;
                return true;
            case double d:
                result = (int)d;
                return true;
            case long l:
                result = (int)l;
                return true;
            default:
                result = 0;
                return false;
        }
    }
}
