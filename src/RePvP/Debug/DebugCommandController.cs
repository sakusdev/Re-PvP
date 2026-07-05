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
        if (Input.GetKeyDown(KeyCode.F4))
        {
            _roundManager.ExtractNextHeisterForDebug();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SpawnDebugPlayers();
        }

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
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F12))
        {
            DebugPlayerRegistry.Clear();
            _roundManager.ResetToWaitingForDebug();
            return;
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            _roundManager.LogDebugState();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            _roundManager.ResetToWaitingForDebug();
        }
    }

    private static void SpawnDebugPlayers()
    {
        if (DebugPlayerRegistry.HasDebugPlayers)
        {
            Plugin.Log.LogInfo("Debug players already exist. Press Ctrl+F12 to clear them first.");
            return;
        }

        for (var i = 0; i < 4; i++)
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = $"RePvP_DebugPlayer_{i + 1}";
            player.transform.position = new Vector3(i * 2f, 1f, 0f);
            player.AddComponent<Rigidbody>().isKinematic = true;
            DebugPlayerRegistry.Register(player);
        }

        Plugin.Log.LogInfo("Spawned 4 debug players. Press F6 to start a round using them.");
    }
}
