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
using ItemExtensions.Additions;
using ItemExtensions.Events;
using ItemExtensions.Models;
using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Items;
using ItemExtensions.Patches;
using ItemExtensions.Patches.Resource_spawning;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Triggers;

namespace ItemExtensions;

public sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += OnLaunch;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        
        helper.Events.GameLoop.DayStarted += Day.Started;
        helper.Events.GameLoop.DayEnding += Day.Ending;
        
        helper.Events.Content.AssetRequested += Assets.OnRequest;
        helper.Events.Content.AssetsInvalidated += Assets.OnInvalidate;
        
        helper.Events.Input.ButtonPressed += ActionButton.Pressed;
        helper.Events.World.ObjectListChanged += World.ObjectListChanged;
        
        helper.Events.Content.LocaleChanged += LocaleChanged;

        Config = Helper.ReadConfig<ModConfig>();
        
        Mon = Monitor;
        Help = Helper;
        Id = ModManifest.UniqueID;
        CropPatches.HasCropsAnytime = helper.ModRegistry.Get("Pathoschild.CropsAnytimeAnywhere") != null;
        
        // patches
        var harmony = new Harmony(ModManifest.UniqueID);

        ItemPatches.Apply(harmony);
        ObjectPatches.Apply(harmony);

        if (Config.MixedSeeds)
        {
            HoeDirtPatches.Apply(harmony);
        }

        InventoryPatches.Apply(harmony);

        if (Config.EatingAnimations)
        {
            FarmerPatches.Apply(harmony);
        }
        
        if (Config.Panning)
        {
            PanPatches.Apply(harmony);
        }

        if (Config.Treasure)
        {
            FishingRodPatches.Apply(harmony);
        }

        if (Config.TrainDrops)
        {
            TrainPatches.Apply(harmony);
        }

        if (Config.FishPond)
        {
            FishPondPatches.Apply(harmony);
        }

        if (Config.Resources)
        {
            GameLocationPatches.Apply(harmony);
            MineShaftPatches.Apply(harmony);
            ResourceClumpPatches.Apply(harmony);

            if (Config.ResourcesMtn)
                MountainPatches.Apply(harmony);

            if (Config.ResourcesVolcano)
                VolcanoPatches.Apply(harmony);
        }

        if (Config.ShopTrades)
        {
            ShopMenuPatches.Apply(harmony);
        }

        if(helper.ModRegistry.Get("Esca.FarmTypeManager") is not null)
            FarmTypeManagerPatches.Apply(harmony);
        
        if(helper.ModRegistry.Get("mistyspring.dynamicdialogues") is not null)
            NpcPatches.Apply(harmony);
        
        if(helper.ModRegistry.Get("Pathoschild.TractorMod") is not null)
            TractorModPatches.Apply(harmony);
        
        //GSQ
        GameStateQuery.Register($"{Id}_ToolUpgrade", Queries.ToolUpgrade);
        GameStateQuery.Register($"{Id}_InInventory", Queries.InInventory);
        
        // trigger actions
        TriggerActionManager.RegisterTrigger($"{Id}_OnBeingHeld");
        TriggerActionManager.RegisterTrigger($"{Id}_OnStopHolding");
        
        TriggerActionManager.RegisterTrigger($"{Id}_OnPurchased");
        TriggerActionManager.RegisterTrigger($"{Id}_OnItemRemoved");
        TriggerActionManager.RegisterTrigger($"{Id}_OnItemDropped");
        TriggerActionManager.RegisterTrigger($"{Id}_OnItemAttached");
        TriggerActionManager.RegisterTrigger($"{Id}_OnItemDetached");

        TriggerActionManager.RegisterTrigger($"{Id}_OnEquip");
        TriggerActionManager.RegisterTrigger($"{Id}_OnUnequip");
        
        TriggerActionManager.RegisterTrigger($"{Id}_AddedToStack");
        
        #if DEBUG
        helper.ConsoleCommands.Add("ie", "Tests ItemExtension's mod capabilities", Debugging.Tester);
        helper.ConsoleCommands.Add("dump", "Exports ItemExtension's internal data", Debugging.Dump);
        helper.ConsoleCommands.Add("tas", "Tests animated sprite", Debugging.DoTas);
        #endif
        helper.ConsoleCommands.Add("fixclumps", "Fixes any missing clumps, like in the case of removed mod packs. (Usually, this won't be needed unless it's an edge-case)", Debugging.Fix);
    }

    public override object GetApi() =>new Api();

    private void OnLaunch(object sender, GameLaunchedEventArgs e)
    {
        if  (Config.Resources)
        {
            Mon.Log("Getting resources for the first time...");
            var oreData = Help.GameContent.Load<Dictionary<string, ResourceData>>($"Mods/{Id}/Resources");
            Parser.Resources(oreData, true);
        }
        
#if DEBUG
        Assets.WriteTemplates();
#endif
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        
        // register mod
        configMenu?.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
        
        configMenu?.AddParagraph(
            mod:  ModManifest,
            text: () => Helper.Translation.Get("config.Description")
        );

        configMenu?.AddPageLink(
            ModManifest, 
            pageId: "Resources", 
            text: () => Helper.Translation.Get("config.Resources.title")
        );

        configMenu?.AddPageLink(
            ModManifest,
            "Drops",
            text: () => Helper.Translation.Get("config.Drops.title")
        );

        //customization
        configMenu?.AddPageLink(
            ModManifest,
            "Custom",
            text: () => Helper.Translation.Get("config.Customization.title")
        );

        configMenu?.AddPageLink(
            ModManifest,
            "Ext",
            () => Helper.Translation.Get("config.VanillaExt.title")
        );

        configMenu?.AddPage(ModManifest, "Custom", () => Helper.Translation.Get("config.Customization.title"));

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.EatingAnimations.name"),
            getValue: () => Config.EatingAnimations,
            setValue: value => Config.EatingAnimations = value
        );
         
        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.MenuActions.name"),
            getValue: () => Config.MenuActions,
            setValue: value => Config.MenuActions = value
        );
         
        //vanilla function extension
        configMenu?.AddPage( ModManifest, "Ext", () => Helper.Translation.Get("config.VanillaExt.title"));

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.QualityChanges.name"),
            getValue: () => Config.QualityChanges,
            setValue: value => Config.QualityChanges = value
        );
        
        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.FishPond.name"),
            getValue: () => Config.FishPond,
            setValue: value => Config.FishPond = value
        );
         
        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.ShopTrades.name"),
            getValue: () => Config.ShopTrades,
            setValue: value => Config.ShopTrades = value
        );

        //extra drops

        configMenu?.AddPage(ModManifest, "Drops", () => Helper.Translation.Get("config.Drops.title"));

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.Treasure.name"),
            getValue: () => Config.Treasure,
            setValue: value => Config.Treasure = value
        );

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.MixedSeeds.name"),
            getValue: () => Config.MixedSeeds,
            setValue: value => Config.MixedSeeds = value
        );

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.Panning.name"),
            getValue: () => Config.Panning,
            setValue: value => Config.Panning = value
        );

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.TrainDrops.name"),
            getValue: () => Config.TrainDrops,
            setValue: value => Config.TrainDrops = value
        );

        //resources

        configMenu?.AddPage(ModManifest, "Resources", () => Helper.Translation.Get("config.Resources.title"));

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.Ores.name"),
            tooltip: () => Help.Translation.Get("config.Ores.tooltip"),
            getValue: () => Config.Resources,
            setValue: value => Config.Resources = value
        );

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.Ores.name") + ' ' + Help.Translation.Get("config.Volcano.name"),
            getValue: () => Config.ResourcesVolcano,
            setValue: value => Config.ResourcesVolcano = value
        );

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.Ores.name") + ' ' + Help.Translation.Get("config.Mountain.name"),
            getValue: () => Config.ResourcesMtn,
            setValue: value => Config.ResourcesMtn = value
        );

        configMenu?.AddBoolOption(
            mod: ModManifest,
            name: () => Help.Translation.Get("config.TerrainFeatures.name"),
            getValue: () => Config.TerrainFeatures,
            setValue: value => Config.TerrainFeatures = value
        );
    }

    /// <summary>
    /// At this point, the mod loads its files and adds contentpacks' changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        //get obj data
        var objData = Help.GameContent.Load<Dictionary<string, ItemData>>($"Mods/{Id}/Data");
        Parser.ObjectData(objData);
        Monitor.Log($"Loaded {Data?.Count ?? 0} item data.", LogLevel.Debug);
        
        if (Config.EatingAnimations)
        {
            //get custom animations
            var animations = Help.GameContent.Load<Dictionary<string, FarmerAnimation>>($"Mods/{Id}/EatingAnimations");
            Parser.EatingAnimations(animations);
            Monitor.Log($"Loaded {EatingAnimations?.Count ?? 0} eating animations.", LogLevel.Debug);
        }
        
        if (Config.Resources)
        {
            //get extra terrain for mineshaft
            var trees = Help.GameContent.Load<Dictionary<string, TerrainSpawnData>>($"Mods/{Id}/Mines/Terrain");
            Parser.Terrain(trees);
            Monitor.Log($"Loaded {MineTerrain?.Count ?? 0} mineshaft terrain features.", LogLevel.Debug);
        }
        
        if (Config.MixedSeeds)
        {
            //get mixed seeds
            var seedData = Help.GameContent.Load<Dictionary<string, List<MixedSeedData>>>($"Mods/{Id}/MixedSeeds");
            Parser.MixedSeeds(seedData);
            Monitor.Log($"Loaded {Seeds?.Count ?? 0} mixed seeds data.", LogLevel.Debug);
        }
        
        if (Config.Panning)
        {
            //get panning
            var panData = Help.GameContent.Load<Dictionary<string, PanningData>>($"Mods/{Id}/Panning");
            Parser.Panning(panData);
            Monitor.Log($"Loaded {Panning?.Count ?? 0} panning data.", LogLevel.Debug);
        }
        
        if(Config.TrainDrops)
        {
            //train stuff
            var trainData = Help.GameContent.Load<Dictionary<string, TrainDropData>>($"Mods/{Id}/Train");
            Parser.Train(trainData);
            Monitor.Log($"Loaded {TrainDrops?.Count ?? 0} custom train drops.", LogLevel.Debug);
        }

        //ACTION BUTTON LIST
        var temp = new List<SButton>();
        foreach (var b in Game1.options.actionButton)
        {
            temp.Add(b.ToSButton());
            Monitor.Log("Button: " + b);
        }
        Monitor.Log($"Total {Game1.options.actionButton?.Length ?? 0}");

        ActionButtons = temp;
    }

    private static void LocaleChanged(object sender, LocaleChangedEventArgs e)
    {
        Comma = e.NewLanguage switch
        {
            LocalizedContentManager.LanguageCode.ja => "、",
            LocalizedContentManager.LanguageCode.zh => "，",
            _ => ", "
        };
    }

    /// <summary>Buttons used for custom item actions</summary>
    internal static List<SButton> ActionButtons { get; private set; } = new();

    public static string Id { get; set; }
    internal static string Comma { get; private set; } = ", ";

    internal static IModHelper Help { get; set; }
    internal static IMonitor Mon { get; set; }
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif

    internal static bool Holding { get; set; }
    internal static ModConfig Config { get; private set; }
    //resources
    public static Dictionary<string, ResourceData> BigClumps { get; internal set; } = new();
    public static Dictionary<string, ResourceData> Ores { get; internal set; } = new();
    public static Dictionary<string, TerrainSpawnData> MineTerrain { get; internal set; } = new();
    //customization
    public static Dictionary<string, ItemData> Data { get; internal set; } = new();
    public static Dictionary<string, FarmerAnimation> EatingAnimations { get; internal set; } = new();
    //extra drops
    public static Dictionary<string, TrainDropData> TrainDrops { get; internal set; } = new();
    public static List<PanningData> Panning { get; internal set; } = new();
    public static Dictionary<string, List<MixedSeedData>> Seeds { get; internal set; } = new();
    public static Dictionary<string, ExtraSpawn> Treasure { get; internal set; } = new();
}
