/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using AudioDescription.CustomZone;
using StardewValley;

namespace AudioDescription;

public class ModEntry : Mod
{
    #region config
    internal static List<string> AllowedCues { get; set; } = new();
    internal static ModConfig Config;
    internal static int Cooldown;
    #endregion

    #region utilities
    internal static IModHelper Help { get; private set; }
    internal static IMonitor Mon { get; private set; }
    public static Texture2D MuteIcon { get; internal set; }
    internal const int NexusId = 16294;
    internal const int MaxMsgs = 5;
    #endregion

    #region variables
    internal static string LastSound { get; set; }
    internal static readonly string[] NotifType = new string[]
    {
        "HUDMessage",
        "Box"
    };
    internal static List<SoundInfo> SoundMessages { get; set; } = new();
    internal static Vector2 SafePositionTop { get; set; } = new Vector2(99f, 99f);
    internal static Vector2 WidthandHeight { get; set; }
    #endregion

    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnGameStart;
        helper.Events.GameLoop.SaveLoaded += ConfigInfo.SaveLoaded;
        helper.Events.GameLoop.OneSecondUpdateTicked += SecondPassed;

        //if custom box
        helper.Events.Display.RenderedHud += SoundBox.RenderedHud;

        Config = Helper.ReadConfig<ModConfig>();
        Help = Helper;
        Mon = Monitor;

        var harmony = new Harmony(ModManifest.UniqueID);

        Monitor.Log($"Applying Harmony patch \"{nameof(SoundPatches)}\": postfixing SDV method \"Game1.playSound\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Game1), nameof(Game1.playSound)),
            postfix: new HarmonyMethod(typeof(SoundPatches), nameof(SoundPatches.PostFix_playSound))
        );

        Monitor.Log($"Applying Harmony patch \"{nameof(SoundPatches)}\": prefixing SDV method \"HUDMessage.draw\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(HUDMessage), nameof(HUDMessage.draw)),
            prefix: new HarmonyMethod(typeof(SoundPatches), nameof(SoundPatches.PrefixHuDdraw))
        );

        Monitor.Log($"Applying Harmony patch \"{nameof(SoundPatches)}\": postfixing SDV method \"FarmAnimal.makeSound()\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.makeSound)),
            postfix: new HarmonyMethod(typeof(SoundPatches), nameof(SoundPatches.PostFix_makeSound))
        );
    }

    private static void SecondPassed(object sender, OneSecondUpdateTickedEventArgs e)
    {
        if (Cooldown != 0)
            Cooldown--;
    }

    private void OnGameStart(object sender, GameLaunchedEventArgs e)
    {
        //HasDailyPlanner = ModEntry.Help.ModRegistry.Get("MevNav.DailyPlanner") != null;

        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );

        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.msg.name"),
            tooltip: () => Helper.Translation.Get("config.msg.description"),
            getValue: () => Config.Type,
            setValue: value => Config.Type = value,
            allowedValues:  NotifType,
            formatAllowedValue: value => Helper.Translation.Get($"config.msg.values.{value}")
        );
            
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Cooldown.name"),
            tooltip: () => Helper.Translation.Get("config.Cooldown.description"),
            getValue: () => Config.CoolDown,
            setValue: value => Config.CoolDown = value
        );
//offset
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.XOffset.name"),
            tooltip: () => Helper.Translation.Get("config.XOffset.description"),
            getValue: () => Config.XOffset,
            setValue: value => Config.XOffset = value,
            min: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Left - 30,
            max: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 30 - (int)Game1.smallFont.MeasureString("Liquid filling container").X,
            interval: 1
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.YOffset.name"),
            tooltip: () => Helper.Translation.Get("config.YOffset.description"),
            getValue: () => Config.YOffset,
            setValue: value => Config.YOffset = value,
            min: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Top - 20,
            max: Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 20 - ((int)Game1.smallFont.MeasureString("Liquid filling container").Y * 5 + 25),
            interval: 1
        );

//types
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => Helper.Translation.Get("config.Sounds.name"),
            tooltip: () => Helper.Translation.Get("config.Sounds.description")
        );
            
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Environment.name"),
            tooltip: () => Helper.Translation.Get("config.Environment.description"),
            getValue: () => Config.Environment,
            setValue: value => Config.Environment = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.NPCs.name"),
            tooltip: () => Helper.Translation.Get("config.NPCs.description"),
            getValue: () => Config.NPCs,
            setValue: value => Config.NPCs = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Player.name"),
            tooltip: () => Helper.Translation.Get("config.Player.description"),
            getValue: () => Config.PlayerSounds,
            setValue: value => Config.PlayerSounds = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Items.name"),
            tooltip: () => Helper.Translation.Get("config.Items.description"),
            getValue: () => Config.ItemSounds,
            setValue: value => Config.ItemSounds = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Fishing.name"),
            tooltip: () => Helper.Translation.Get("config.Fishing.description"),
            getValue: () => Config.FishingCatch,
            setValue: value => Config.FishingCatch = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Minigames.name"),
            tooltip: () => Helper.Translation.Get("config.Minigames.description"),
            getValue: () => Config.Minigames,
            setValue: value => Config.Minigames = value
        );
    }
}