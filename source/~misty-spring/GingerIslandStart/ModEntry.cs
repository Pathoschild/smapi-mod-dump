/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Text;
using GingerIslandStart.Additions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GingerIslandStart;

public sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        Mon = Monitor;
        Help = Helper;
        Id = ModManifest.UniqueID;
        NameInData = $"{ModManifest.UniqueID}_Island";

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Content.LocaleChanged += OnLocaleChange;
        helper.Events.GameLoop.ReturnedToTitle += OnTitleReturn;

        helper.Events.GameLoop.SaveCreated += Events.Save.Created;
        helper.Events.GameLoop.SaveLoaded += Events.Save.Loaded;
        helper.Events.GameLoop.DayStarted += Events.Day.OnStart;
        helper.Events.GameLoop.DayEnding += Events.Day.OnEnd;

        helper.Events.Display.RenderedWorld += Events.Render.DrawOverWorld;
        
        helper.Events.Player.Warped += Events.Location.OnWarp;
        helper.Events.Content.AssetReady += Events.Location.PropertyChanges;
        helper.Events.Content.AssetRequested += Events.Asset.Requested;

        GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_Batteries", TileActions.Batteries);
        GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_Anchor", TileActions.Anchor);
        GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_Hull", TileActions.Hull);
        GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_Builder", TileActions.Builder);
        GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_WeaponShop", TileActions.WeaponShop);
        GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_ToolsNGeodes", TileActions.ToolShop);
        GameLocation.RegisterTouchAction($"{ModManifest.UniqueID}_Sleep", TileActions.Sleep);
        
        var harmony = new Harmony(ModManifest.UniqueID);
        Patches.Game1Patches.Apply(harmony);
        Patches.ItemPatches.Apply(harmony);
        Patches.MenuPatches.Apply(harmony);
        Patches.MonsterPatches.Apply(harmony);

        Config = helper.ReadConfig<ModConfig>();
        
#if DEBUG
        helper.ConsoleCommands.Add("printmenu", "prints current menu.", PrintMenu);
#endif
    }

    private void PrintMenu(string arg1, string[] arg2)
    {
        Monitor.Log(Game1.activeClickableMenu.GetType().ToString(), LogLevel.Info);
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        GetLocalizedContent();
        
        var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
        api?.RegisterToken(ModManifest, "GeneralDifficulty", () =>
        {
            return new[] { $"{Config.Difficulty}" };
        });
        
        api?.RegisterToken(ModManifest, "ShopDifficulty", () =>
        {
            return new[] { $"{Config.Shops}" };
        });
        
        api?.RegisterToken(ModManifest, "EnableRodUpgrade", () =>
        {
            return new[] { $"{Config.RodUpgrades}" };
        });
        
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;
        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
        
        configMenu.AddSectionTitle(
            mod:  ModManifest,
            text: () => Helper.Translation.Get("config.Gameplay.name"),
            tooltip: () =>Helper.Translation.Get("config.Gameplay.description")
        );
        
        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.GeneralDifficulty.name"),
            tooltip: () => Helper.Translation.Get("config.GeneralDifficulty.description"),
            getValue: () => Config.Difficulty,
            setValue: value => Config.Difficulty = value,
            allowedValues: new[]{"easy","normal","hard"},
            formatAllowedValue: value => Helper.Translation.Get($"config.Gameplay.values.{value}")
        );
        
        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.ShopDifficulty.name"),
            tooltip: () => Helper.Translation.Get("config.ShopDifficulty.description"),
            getValue: () => Config.Shops,
            setValue: value => Config.Shops = value,
            allowedValues: new[]{"easy","normal","hard"},
            formatAllowedValue: value => Helper.Translation.Get($"config.Gameplay.values.{value}")
        );
        
        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.MonsterDifficulty.name"),
            tooltip: () => Helper.Translation.Get("config.MonsterDifficulty.description"),
            getValue: () => Config.MonsterDifficulty,
            setValue: value => Config.MonsterDifficulty = value,
            allowedValues: new[]{"easy","normal","hard"},
            formatAllowedValue: value => Helper.Translation.Get($"config.Gameplay.values.{value}")
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.FasterWeaponAccess.name"),
            tooltip: () => Helper.Translation.Get("config.FasterWeaponAccess.description"),
            getValue: () => Config.FasterWeaponAccess,
            setValue: value => Config.FasterWeaponAccess = value
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.RodUpgrades.name"),
            tooltip: () => Helper.Translation.Get("config.RodUpgrades.description"),
            getValue: () => Config.RodUpgrades,
            setValue: value => Config.RodUpgrades = value
        );
    }
    
    private static void OnTitleReturn(object sender, ReturnedToTitleEventArgs e)
    {
        EnabledOption = false;
        NeedsWarp = false;
    }
    
    private static void OnLocaleChange(object sender, LocaleChangedEventArgs e)
    {
        GetLocalizedContent();
    }
    
    private static void GetLocalizedContent()
    {
        //create build question from other strings
        var barn = Game1.content.LoadString("Strings/Buildings:Barn_Name");
        //var build = Game1.content.LoadString("Strings/UI:Carpenter_Build");
        //var prepend = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es ? '¿' : (char?)null;
        
        var woodBuilder = new StringBuilder();
        woodBuilder.AppendFormat(Game1.content.LoadString("Strings/UI:ItemHover_Requirements"), 120, Game1.content.LoadString("Strings/Objects:Wood_Name"));
        woodBuilder.Replace('\n', ' ');
        var comma = LocalizedContentManager.CurrentLanguageCode switch
        {
            LocalizedContentManager.LanguageCode.ja => "、",
            LocalizedContentManager.LanguageCode.zh => "，",
            _ => ", "
        };
        var dot = LocalizedContentManager.CurrentLanguageCode switch
        {
            LocalizedContentManager.LanguageCode.ja => "。",
            LocalizedContentManager.LanguageCode.zh => "。",
            _ => "."
        };
        woodBuilder.Replace(dot, comma);
        var wood = woodBuilder.ToString();
        
        
        var stoneBuilder = new StringBuilder();
        stoneBuilder.AppendFormat(Game1.content.LoadString("Strings/UI:ItemHover_Requirements_Extra"), 75,
            Game1.content.LoadString("Strings/Objects:Stone_Name"));
        stoneBuilder.Append(dot);
        var stone = stoneBuilder.ToString();
        
        
        var question = $"{barn}: {wood}{stone}"; //({barn}) {prepend}{build}?
        BuildQuestion = question;
        
        
        //create tooltip
        var artifactSpot = Game1.content.LoadString("Strings/Objects:ArtifactSpot_Name");
        var stringsFromCs = Help.GameContent.Load<Dictionary<string,string>>("Strings/StringsFromCSFiles");
        var artifact = stringsFromCs["Object.cs.12849"];
        var beginning = stringsFromCs["Home"].ToLower();
        
        var fixedString = artifactSpot.Replace(artifact, beginning, StringComparison.OrdinalIgnoreCase);

        ToolTip = fixedString.ToLower();
    }

    public static ModConfig Config { get; private set; }
    
    internal static IModHelper Help { get; set; }
    internal static IMonitor Mon { get; set; }
    internal static bool EnabledOption { get; set; }
    internal static bool NeedsWarp { get; set; }
    private static string ToolTip { get; set; }
    internal static string NameInData { get; set; }

    internal static string Id { get; set; }

    internal static string RecoveryKey { get; set; } = $"{Id}_ItemRecovery"; //{Constants.CurrentSavePath}/{Id}_

    internal static string GiftWarpId { get; set; } = $"{Id}_gotGiftWarped";
    internal static Point StartingPoint { get; } = new(28,29); //27,24 is where tent patch starts
    internal static string BuildQuestion { get; set; }

    internal static ClickableTextureComponent IslandButton
    {
        get
        {
            var r = new ClickableTextureComponent(
                name: "Ginger Island", 
                bounds: new Rectangle(), 
                label: $"{Game1.content.LoadString("Strings/StringsFromCSFiles:IslandName")}\n({ToolTip})", 
                hoverText: Game1.content.LoadString("Strings/StringsFromCSFiles:IslandName"), 
                texture: Game1.mouseCursors, 
                sourceRect: new Rectangle(227, 425, 9, 9), scale: 4f)
            {
                myID = 405,
                upNeighborID = 506,
                leftNeighborID = 517,
                rightNeighborID = 505
            };
            return r;
        }
    }
    internal static double GeneralDifficultyMultiplier
    {
        get
        {
            if (!Context.IsWorldReady) return 1;
            var multiplier = Config.Difficulty switch
            {
                "easy" => 0.5,
                "hard" => 2,
                _ => 1
            };
            return multiplier;
        }
    }
    internal static double ShopMultiplier
    {
        get
        {
            if (!Context.IsWorldReady) return 1;
            var multiplier = Config.Shops switch
            {
                "easy" => 0.5,
                "hard" => 2,
                _ => 1
            };
            return multiplier;
        }
    }
}