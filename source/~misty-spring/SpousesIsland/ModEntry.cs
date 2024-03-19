/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SpousesIsland.Additions;
using SpousesIsland.Patches;
using SpousesIsland.APIs;
using SpousesIsland.Events;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;

namespace SpousesIsland;

public sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        //adds config and loads assets
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        helper.Events.Content.AssetRequested += Asset.Requested;

        //changes mod info (and NPCs)
        helper.Events.GameLoop.DayStarted += Day.OnStart;
        helper.Events.GameLoop.DayEnding += Day.OnEnd;
        helper.Events.GameLoop.TimeChanged += Day.OnTimeChange;

        helper.Events.GameLoop.ReturnedToTitle += TitleReturn;

        //gets user data

        Config = Helper.ReadConfig<ModConfig>();

        Mon = Monitor;
        Help = Helper;
        Id = ModManifest.UniqueID;

        var harmony = new Harmony(ModManifest.UniqueID);
        NPCPatches.Apply(harmony);
        
        #if DEBUG
        helper.ConsoleCommands.Add("printschedule", "Prints current schedule for spouses.", Debugging.Print);
        helper.ConsoleCommands.Add("printpath", "Prints current pathfind for spouses.", Debugging.PrintDetailed);
        helper.ConsoleCommands.Add("printfull", "Prints entire pathfind for spouses.", Debugging.PrintFull);
        helper.ConsoleCommands.Add("getwarps", "Prints all NPC warps in map.", Debugging.GetNpcWarps);
        helper.ConsoleCommands.Add("allwarps", "Prints all warps in map.", Debugging.GetAllWarps);
        helper.ConsoleCommands.Add("ismoving", "Prints all spouse movements.", Debugging.IsMoving);
        #endif
        
        GameStateQuery.Register("mistyspring.spousesisland_IslandVisitDay", Information.IslandVisitDay);
    }

    #region events
    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        ModInfo = new InstalledMods();

        // get CP's api and register token
        var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
        if(api is not null)
        {
            api.RegisterToken(ModManifest, "CanVisitIsland", () =>
            {
                return new[] { $"{IslandToday}" };
            });

            api.RegisterToken(ModManifest, "Invited", () =>
            {
                if(!Context.IsWorldReady)
                    return Array.Empty<string>();

                if (IslandToday)
                {
                    return IsFromTicket ? Status.Keys.ToArray() : Information.PlayerSpouses(Game1.player);
                }
                
                return Array.Empty<string>();
            });

            api.RegisterToken(ModManifest, "Devan", () =>
            {
                return new[] { $"{Config.Devan}" };
            });

            api.RegisterToken(ModManifest, "AllowChildren", () =>
            {
                if (!Context.IsWorldReady || !Config.AllowChildren)
                    return Array.Empty<string>();
                
                var hasChildBed = Config.UseFurnitureBed == false || (Config.UseFurnitureBed && Beds.HasAnyKidBeds());

                return new[] { $"{hasChildBed}" };
            });

            api.RegisterToken(ModManifest, "HasChildren", () =>
            {
                if (!Context.IsWorldReady)
                    return new[] { "false" };
                
                return new[] { $"{Game1.player.getChildrenCount() > 0}" };
            });

            api.RegisterToken(ModManifest, "IslandAttire", () =>
            {
                return new[] {$"{Config.IslandClothes}"};
            });
        }

        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        
        if (configMenu is null) 
            return;
        
        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );

        #region basic config
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.CustomChance.name"),
            tooltip: () => Helper.Translation.Get("config.CustomChance.description"),
            getValue: () => Config.CustomChance,
            setValue: value => Config.CustomChance = value,
            min: 0,
            max: 100,
            interval: 1
        );

        //random place
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.ScheduleRandom.name"),
            tooltip: () => Helper.Translation.Get("config.ScheduleRandom.description"),
            getValue: () => Config.ScheduleRandom,
            setValue: value => Config.ScheduleRandom = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.UseIslandClothes.name"),
            getValue: () => Config.IslandClothes,
            setValue: value => Config.IslandClothes = value
        );
        
        configMenu.AddTextOption(
            mod: ModManifest,
            getValue: () => Config.Blacklist,
            setValue: value => Config.Blacklist = value,
            name: () => Helper.Translation.Get("config.Blacklist.name"),
            tooltip: () => Helper.Translation.Get("config.Blacklist.description")
        );
        #endregion

        if (ModInfo.LittleNpcs)
        {
            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "C2Nconfig",
                text: () => ChildrenTitle,
                tooltip: () => Helper.Translation.Get("config.Child2NPC.description")
            );
            
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "C2Nconfig",
                pageTitle: () => ChildrenTitle
            );
            
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.Enabled"),
                tooltip: () => Helper.Translation.Get("config.Devan_Nosit.description"),
                getValue: () => Config.Devan,
                setValue: value => Config.Devan = value
            );
            
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.ChildVisitIsland.name"),
                tooltip: () => Helper.Translation.Get("config.ChildVisitIsland.description"),
                getValue: () => Config.AllowChildren,
                setValue: value => Config.AllowChildren = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.UseFurnitureBed.name"),
                tooltip: () => Helper.Translation.Get("config.UseFurnitureBed.description"),
                getValue: () => Config.UseFurnitureBed,
                setValue: value => Config.UseFurnitureBed = value 
            );
            if (Config.UseFurnitureBed == false) //if it's not bed furniture: lets you decide the "mod bed" color.
            {
                configMenu.AddTextOption(
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("config.Childbedcolor.name"),
                    tooltip: () => Helper.Translation.Get("config.Childbedcolor.description"),
                    getValue: () => Config.Childbedcolor,
                    setValue: value => Config.Childbedcolor = value,
                    allowedValues: new[] { "1", "2", "3", "4", "5", "6" }
                );
                configMenu.AddImage(
                    mod: ModManifest,
                    texture: () => ModEntry.Help.ModContent.Load<Texture2D>("assets/KidbedSamples.png"),
                    texturePixelArea: null,
                    scale: 1
                );
            }
        }
    }

    private static void SaveLoaded(object sender, SaveLoadedEventArgs e) => CheckPlayer();

    internal static void CheckPlayer()
    {
        Unlocked = Game1.player.mailReceived.Contains("Island_UpgradeHouse");
        
        if (!Unlocked)
            return;

        if (!string.IsNullOrWhiteSpace(Config.Blacklist))
            ParseBlacklist();
        
        //try to read file from moddata. if empty, check mail
        ValidSpouses = Information.AllowedSpouses(Game1.player);
        Children = Information.PlayerChildren(Game1.player);
        
        DevanExists = Config.Devan || ModInfo.Devan;

        //should be a string, dictionary<string,int>
        Status = Help.Data.ReadSaveData<Dictionary<string,(int,bool)>>( $"{Id}_IslandVisit");

        if (Status == null)
        {
            IsFromTicket = false;
            return;
        }

        IsFromTicket = true;
        IslandToday = true;
        
        var toRemove = new List<string>();

        foreach (var character in Status)
        {
            var i = character.Value.Item1 - 1;
            
            if (i <= 0)
                toRemove.Add(character.Key);
        }

        foreach (var name in toRemove)
        {
            Status.Remove(name);
        }
        
        Help.Data.WriteSaveData( $"{Id}_IslandVisit", Status);
    }

    private void TitleReturn(object sender, ReturnedToTitleEventArgs e)
    {
        Status = new();
        ValidSpouses = new();

        Monitor.Log("Clearing Children...");
        Children?.Clear();

        //empty bools and int
        PreviousDayRandom = 0;
        RandomizedInt = 0;
    }
    #endregion
    
    internal static string Id { get; set; }

    #region helper and config
    internal static ModConfig Config;
    internal static IModHelper Help { get; private set; }
    internal static string Translate(string key) => Help.Translation.Get(key);
    internal static IMonitor Mon { get; private set; }
#if DEBUG
    internal static LogLevel Level { get; set; } = LogLevel.Debug;
#else
    internal static LogLevel Level { get; set; } = LogLevel.Trace;
#endif
    #endregion
    
    private static string ChildrenTitle { 
        get
        {
            var plural = Lexicon.makePlural(Game1.content.LoadString("Strings/StringsFromCSFiles:Dialogue.cs.793"));
            var s = Lexicon.capitalize(plural);
            return s; 
        }
    }
    
    private static void ParseBlacklist()
    {
        Blacklist.Clear();
        Mon.Log("Getting raw blacklist.", Level);
        var blacklist = Config.Blacklist;
        if (blacklist is null)
        {
            Mon.Log("No characters in blacklist.", Level);
        }

        var charsToRemove = new[] { "-", ",", ".", ";", "\"", "\'", "/", "\\" };
        foreach (var c in charsToRemove)
        {
            blacklist = blacklist?.Replace(c, string.Empty);
        }
        #if DEBUG
            Mon.Log($"Raw blacklist: \n {blacklist} \nWill be parsed to list now.", LogLevel.Debug);
        #endif
        Blacklist= blacklist?.Split(' ').ToList();
    }

    #region visit info
    internal static bool IslandToday { get; set; }
    internal static bool IsFromTicket { get; set; }
    internal static int RandomizedInt { get; set; }
    internal static int PreviousDayRandom { get; set; }
    #endregion

    #region user data
    internal static List<Child> Children { get; set; } = new();
    public static List<string> ValidSpouses { get; private set; }
    internal static bool Unlocked { get; private set; }

    internal static InstalledMods ModInfo;
    internal static List<string> Blacklist { get; set; } = new();
    internal static bool DevanExists { get; set; }
    internal static Dictionary<string,(int, bool)> Status { get; private set; }
    #endregion
}