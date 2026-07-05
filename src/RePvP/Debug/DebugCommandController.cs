using UnityEngine;

namespace RePvP;

public sealed class DebugCommandController
{
    private readonly RePvPConfig _config;
    private readonly RoundManager _roundManager;

    private string _cashBuffer = "";

    public DebugCommandController(RePvPConfig config, RoundManager roundManager)
    {
        _config = config;
        _roundManager = roundManager;
    }

    public void Tick()
    {
        if (!_config.DebugKeysEnabled.Value)
        {
            return;
        }

        HandleFunctionKeys();
        HandleCashBufferKeys();
        HandleAdvancedDebugKeys();
    }

    private void HandleFunctionKeys()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            _roundManager.ForceStartRound();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            _roundManager.AddCashForDebug(_config.DebugCashAmount.Value);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            _roundManager.TriggerExtractionForDebug();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            _roundManager.EndRoundForDebug(Team.Heisters);
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            _roundManager.EndRoundForDebug(Team.Hunter);
        }
    }

    private void HandleCashBufferKeys()
    {
        for (var i = 0; i <= 9; i++)
        {
            var keyCode = KeyCode.Alpha0 + i;
            if (Input.GetKeyDown(keyCode))
            {
                _cashBuffer += i.ToString();
                Plugin.Log.LogInfo($"Debug cash buffer: {_cashBuffer}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && _cashBuffer.Length > 0)
        {
            _cashBuffer = _cashBuffer.Substring(0, _cashBuffer.Length - 1);
            Plugin.Log.LogInfo($"Debug cash buffer: {_cashBuffer}");
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (int.TryParse(_cashBuffer, out var amount) && amount > 0)
            {
                _roundManager.AddCashForDebug(amount);
            }

            _cashBuffer = "";
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _cashBuffer = "";
        }
    }

    private void HandleAdvancedDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            _roundManager.LogDebugState();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            _roundManager.ResetToWaitingForDebug();
        }
    }
}
