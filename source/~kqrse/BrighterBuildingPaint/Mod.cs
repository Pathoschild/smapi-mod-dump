/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace BrighterBuildingPaint;

internal partial class Mod: StardewModdingAPI.Mod {
    internal static Configuration Config;

    public override void Entry(IModHelper helper) {
        Config = Helper.ReadConfig<Configuration>();
        I18n.Init(helper.Translation);
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Constructor(typeof(BuildingPaintMenu.BuildingColorSlider), new []{
                typeof(BuildingPaintMenu),
                typeof(int),
                typeof(Rectangle),
                typeof(int), 
                typeof(int),
                typeof(Action<int>)}),
            postfix: new HarmonyMethod(typeof(BuildingPaintMenu_BuildingColorSlider_Patch),
                nameof(BuildingPaintMenu_BuildingColorSlider_Patch.Postfix))
        );
    }
    
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new Configuration(),
            save: () => Helper.WriteConfig(Config)
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.Enabled,
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.MaxBrightness,
            getValue: () => Config.MaxBrightness,
            setValue: value => Config.MaxBrightness = value,
            min: 0,
            max: 100
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.MinBrightness,
            getValue: () => Config.MinBrightness,
            setValue: value => Config.MinBrightness = value,
            min: -100,
            max: 0
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.MaxSaturation,
            getValue: () => Config.MaxSaturation,
            setValue: value => Config.MaxSaturation = value,
            min: 0,
            max: 100
        );
    }
}