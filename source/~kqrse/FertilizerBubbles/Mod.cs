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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FertilizerBubbles;

internal partial class Mod: StardewModdingAPI.Mod {
    internal static Configuration Config;
    internal static IModHelper ModHelper;
    internal static int CurrentEmoteInterval;
    internal static int CurrentEmoteFrame;
    internal static bool ToggleEmoteEnabled;

    public override void Entry(IModHelper helper) {
        Config = helper.ReadConfig<Configuration>();
        ModHelper = helper;
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        Helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);
        
        harmony.Patch(
            original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.draw)),
            postfix: new HarmonyMethod(typeof(HoeDirt_draw_Patch), nameof(HoeDirt_draw_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = ModHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }
    
    private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e) {
        if (!Config.Enabled) return;
        if (Config.ToggleEmoteKey.JustPressed()) ToggleEmoteEnabled = !ToggleEmoteEnabled;
    }
    
    private void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!Config.Enabled) return;
        AnimateEmote();
    }

    private static void AnimateEmote() {
        CurrentEmoteInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

        if (CurrentEmoteFrame is < 16 or > 19) CurrentEmoteFrame = 16;
        if (CurrentEmoteInterval > Config.EmoteInterval) {
            if (CurrentEmoteFrame < 19) CurrentEmoteFrame++;
            else CurrentEmoteFrame = 16;
            CurrentEmoteInterval = 0;
        }
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
            name: I18n.DisplayWhenHeld,
            getValue: () => Config.DisplayWhenHeld,
            setValue: value => Config.DisplayWhenHeld = value
        );
        
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: I18n.ToggleEmoteKey,
            tooltip: I18n.ToggleEmoteKeyTooltip,
            getValue: () => Config.ToggleEmoteKey,
            setValue: value => Config.ToggleEmoteKey = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.HideWhenUnusable,
            getValue: () => Config.HideWhenUnusable,
            setValue: value => Config.HideWhenUnusable = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.HideWhenNoCrop,
            getValue: () => Config.HideWhenNoCrop,
            setValue: value => Config.HideWhenNoCrop = value
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
            name: I18n.EmoteInterval,
            getValue: () => Config.EmoteInterval,
            setValue: value => Config.EmoteInterval = value,
            min: 0,
            max: 1000
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