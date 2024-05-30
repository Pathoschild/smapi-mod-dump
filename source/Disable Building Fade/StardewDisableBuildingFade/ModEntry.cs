/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewDisableBuildingFade
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Buildings;

namespace StardewDisableBuildingFade;

internal class ModConfig
{
    internal bool FadeDisabled { get; set; } = true;
}

internal sealed class ModEntry : Mod
{
    private static ModConfig Config { get; set; } = new();
    
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        
        harmony.Patch(
            original: AccessTools.Method(typeof(Building), "UpdateTransparency"),
            prefix: new HarmonyMethod(typeof(ModEntry), nameof(DisableFadePrefix))
        );

        helper.Events.GameLoop.GameLaunched += (sender, e) =>
        {
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;
            
            Config = helper.ReadConfig<ModConfig>();

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => helper.WriteConfig(Config)
            );
            
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Disable building fade",
                tooltip: () => "Disables buildings fading when the player walks behind",
                getValue: () => Config.FadeDisabled,
                setValue: value => Config.FadeDisabled = value
            );
        };
    }

    private static bool DisableFadePrefix()
    {
        return !Config.FadeDisabled;
    }
}