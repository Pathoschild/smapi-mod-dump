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
using StardewValley.Characters;

namespace WheresMyHorse;

internal partial class Mod: StardewModdingAPI.Mod {
    internal static Configuration Config;
    internal static IModHelper ModHelper;
    internal static bool EmoteEnabled;
    internal static int CurrentEmoteInterval;
    internal static int CurrentEmoteFrame;

    public override void Entry(IModHelper helper) {
        Config = helper.ReadConfig<Configuration>();
        ModHelper = helper;
        I18n.Init(helper.Translation);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        helper.Events.Player.Warped += Warped;
        helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
        ApplyHarmonyPatches();
    }

    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(Horse), nameof(Horse.draw), new []{typeof(SpriteBatch)}),
            postfix: new HarmonyMethod(typeof(Horse_draw_Patch), nameof(Horse_draw_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = ModHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }
    
    private void Warped(object sender, WarpedEventArgs e) {
        if (Config.DisableOnMapChange) EmoteEnabled = false;
    }
    
    private void SaveLoaded(object sender, SaveLoadedEventArgs e) {
        EmoteEnabled = false;
    }
    
    private void UpdateTicked(object sender, UpdateTickedEventArgs e) {
        if (!Config.Enabled) return;
        if (Config.DisableOnMount && Game1.player.isAnimatingMount) EmoteEnabled = false;
        AnimateEmote();
    }

    private static void AnimateEmote() {
        CurrentEmoteInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

        if (CurrentEmoteFrame is < 40 or > 43) CurrentEmoteFrame = 40;
        if (CurrentEmoteInterval > Config.EmoteInterval) {
            if (CurrentEmoteFrame < 43) CurrentEmoteFrame++;
            else CurrentEmoteFrame = 40;
            CurrentEmoteInterval = 0;
        }
    }
    
    private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e) {
        if (!Config.Enabled) return;
        if (Game1.player.isRidingHorse()) return;
        if (!Config.DoEmoteKey.JustPressed()) return;
        
        if (IsHorseInLocation()) EmoteEnabled = !EmoteEnabled;
        else Game1.player.doEmote(8);
    }

    private static bool IsHorseInLocation() {
        var horses = Game1.player.currentLocation.characters.OfType<Horse>().ToList();
        return Config.OnlyMyHorse ?
            horses.Any(horse => horse.getOwner() == Game1.player) :
            horses.Any();
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

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: I18n.DoEmoteKey,
            getValue: () => Config.DoEmoteKey,
            setValue: value => Config.DoEmoteKey = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.RenderOnTop,
            getValue: () => Config.RenderOnTop,
            setValue: value => Config.RenderOnTop = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DisableOnMount,
            getValue: () => Config.DisableOnMount,
            setValue: value => Config.DisableOnMount = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.DisableOnMapChange,
            getValue: () => Config.DisableOnMapChange,
            setValue: value => Config.DisableOnMapChange = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.OnlyMyHorse,
            getValue: () => Config.OnlyMyHorse,
            setValue: value => Config.OnlyMyHorse = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: I18n.AlwaysRender,
            getValue: () => Config.AlwaysRender,
            setValue: value => Config.AlwaysRender = value
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
            max: 200
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
    }
}