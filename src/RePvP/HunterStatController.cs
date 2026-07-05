namespace RePvP;

public sealed class HunterStatController
{
    private readonly RePvPConfig _config;
    private readonly RoleManager _roleManager;

    public HunterStatController(RePvPConfig config, RoleManager roleManager)
    {
        _config = config;
        _roleManager = roleManager;
    }

    public void ApplyHunterStats()
    {
        // TODO: Hook into R.E.P.O.'s actual movement/player stat component.
        // Current placeholder only logs the intended effect.
        if (_roleManager.Hunter == null)
        {
            return;
        }

        Plugin.Log.LogInfo($"TODO Apply Hunter speed multiplier x{_config.HunterSpeedMultiplier.Value:0.00} to {_roleManager.Hunter.DisplayName}.");
    }

    public void ResetHunterStats()
    {
        // TODO: Reset modified movement/player stats once real hooks exist.
        Plugin.Log.LogInfo("TODO Reset Hunter stat modifiers.");
    }
}
