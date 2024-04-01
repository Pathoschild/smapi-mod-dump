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
using ItemExtensions.Patches;
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
        
        Mon = Monitor;
        Help = Helper;
        Id = ModManifest.UniqueID;
        CropPatches.HasCropsAnytime = helper.ModRegistry.Get("Pathoschild.CropsAnytimeAnywhere") != null;
        
        // patches
        var harmony = new Harmony(ModManifest.UniqueID);
        CropPatches.Apply(harmony);
        FarmerPatches.Apply(harmony);
        GameLocationPatches.Apply(harmony);
        HoeDirtPatches.Apply(harmony);
        InventoryPatches.Apply(harmony);
        ItemPatches.Apply(harmony);
        MineShaftPatches.Apply(harmony);
        ObjectPatches.Apply(harmony);
        ResourceClumpPatches.Apply(harmony);
        ShopMenuPatches.Apply(harmony);
        
        if(helper.ModRegistry.Get("mistyspring.dynamicdialogues") is null)
            NpcPatches.Apply(harmony);
        
        //GSQ
        GameStateQuery.Register($"{Id}_ToolUpgrade", Queries.ToolUpgrade);
        GameStateQuery.Register($"{Id}_InInventory", Queries.InInventory);
        
        // trigger actions
        TriggerActionManager.RegisterTrigger($"{Id}_OnBeingHeld");
        TriggerActionManager.RegisterTrigger($"{Id}_OnStopHolding");
        
        TriggerActionManager.RegisterTrigger($"{Id}_OnPurchased");
        TriggerActionManager.RegisterTrigger($"{Id}_OnItemRemoved");
        TriggerActionManager.RegisterTrigger($"{Id}_OnItemDropped");
        
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

    private static void OnLaunch(object sender, GameLaunchedEventArgs e)
    {
        Mon.Log("Getting resources for the first time...", Level);
        var oreData = Help.GameContent.Load<Dictionary<string, ResourceData>>($"Mods/{Id}/Resources");
        Parser.Resources(oreData, true);
#if DEBUG
        Assets.WriteTemplates();
#endif
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
        var dc = Data?.Count ?? 0;
        Monitor.Log($"Loaded {dc} item data.", LogLevel.Debug);
        
        //get custom animations
        var animations = Help.GameContent.Load<Dictionary<string, FarmerAnimation>>($"Mods/{Id}/EatingAnimations");
        Parser.EatingAnimations(animations);
        var ac = EatingAnimations?.Count ?? 0;
        Monitor.Log($"Loaded {ac} eating animations.", LogLevel.Debug);
        
        //get item actions
        var menuActions = Help.GameContent.Load<Dictionary<string, List<MenuBehavior>>>($"Mods/{Id}/MenuActions");
        Parser.ItemActions(menuActions);
        var ic = MenuActions?.Count ?? 0;
        Monitor.Log($"Loaded {ic} menu actions.", LogLevel.Debug);
        
        //get mixed seeds
        var seedData = Help.GameContent.Load<Dictionary<string, List<MixedSeedData>>>($"Mods/{Id}/MixedSeeds");
        Parser.MixedSeeds(seedData);
        var msc = Seeds?.Count ?? 0;
        Monitor.Log($"Loaded {msc} mixed seeds data.", LogLevel.Debug);
        
        var temp = new List<SButton>();
        foreach (var b in Game1.options.actionButton)
        {
            temp.Add(b.ToSButton());
            Monitor.Log("Button: " + b);
        }
        Monitor.Log($"Total {Game1.options.actionButton.Length}");

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
    public static Dictionary<string, ResourceData> BigClumps { get; internal set; } = new();
    public static Dictionary<string, ItemData> Data { get; internal set; } = new();
    internal static Dictionary<string, FarmerAnimation> EatingAnimations { get; set; } = new();
    internal static Dictionary<string, List<MenuBehavior>> MenuActions { get; set; } = new();
    public static Dictionary<string, ResourceData> Ores { get; internal set; } = new();
    internal static Dictionary<string, List<MixedSeedData>> Seeds { get; set; } = new();
}