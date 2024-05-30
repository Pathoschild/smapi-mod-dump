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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace BetterTruffles;

internal partial class Mod: StardewModdingAPI.Mod {
    internal static Configuration Config;
    internal static IModHelper ModHelper;

    public override void Entry(IModHelper helper) {
        Config = helper.ReadConfig<Configuration>();
        ModHelper = helper;
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.behaviors)),
            prefix: new HarmonyMethod(typeof(FarmAnimal_behaviors_Patch), 
                nameof(FarmAnimal_behaviors_Patch.Prefix))
        );
        
        harmony.Patch(
            original: AccessTools.Method(typeof(Object), nameof(Object.draw), new [] {
                typeof(SpriteBatch),
                typeof(int),
                typeof(int),
                typeof(float)
            }),
            postfix: new HarmonyMethod(typeof(Object_draw_Patch), nameof(Object_draw_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = ModHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new Configuration(),
            save: () => ModHelper.WriteConfig(Config)
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.Enabled,
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.PigsDigInGrass,
            getValue: () => Config.PigsDigInGrass,
            setValue: value => Config.PigsDigInGrass = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.ShowBubbles,
            getValue: () => Config.ShowBubbles,
            setValue: value => Config.ShowBubbles = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.RenderOnTop,
            getValue: () => Config.RenderOnTop,
            setValue: value => Config.RenderOnTop = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleYOffset,
            getValue: () => Config.OffsetY,
            setValue: value => Config.OffsetY = value,
            min: -128,
            max: 128
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleXOffset,
            getValue: () => Config.OffsetX,
            setValue: value => Config.OffsetX = value,
            min: -128,
            max: 128
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.Opacity,
            getValue: () => Config.OpacityPercent,
            setValue: value => Config.OpacityPercent = value,
            min: 1,
            max: 100
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: I18n.BubbleSize,
            getValue: () => Config.SizePercent,
            setValue: value => Config.SizePercent = value,
            min: 1,
            max: 100
        );
    }
}