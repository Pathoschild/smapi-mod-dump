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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using SpousesIsland.GenericModConfigMenu;
using SpousesIsland.ModContent;
using Patches = SpousesIsland.ModContent.Patches;

namespace SpousesIsland
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //adds config and loads assets
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            //changes mod info (and NPCs)
            helper.Events.GameLoop.DayStarted += Changes.DayStart;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.ReturnedToTitle += this.TitleReturn;

            //edits things but doesn't change info, can be safely moved to another file (for readability and briefness)
            helper.Events.GameLoop.TimeChanged += Changes.OnTimeChanged;
            helper.Events.GameLoop.UpdateTicked += Changes.UpdateTicked;

            //gets user data
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;

            Config = Helper.ReadConfig<ModConfig>();

            Mon = Monitor;
            Help = Helper;
            TL = Helper.Translation;

            //commands
            helper.ConsoleCommands.Add("ichance", helper.Translation.Get("CLI.chance"), Debugging.Chance);
            helper.ConsoleCommands.Add("getstat", "", Debugging.GetStatus);
            helper.ConsoleCommands.Add("sgidata", "", Debugging.GeneralInfo);
            helper.ConsoleCommands.Add("sgiprint", "", Debugging.Print);

            this.Monitor.Log($"Applying Harmony patch \"{nameof(Patches)}\": prefixing SDV method \"NPC.tryToReceiveActiveObject(Farmer who)\".");
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.tryToReceiveTicket))
                );
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            LoadedBasicData = true;

            //try to read file from moddata. if empty, check mail
            ReadModData(Game1.player);

            //now get user data
            var boatFix = Game1.player?.mailReceived?.Contains("willyBoatFixed");
            BoatFixed = boatFix ?? false;
            this.Monitor.Log($"BoatFixed = {BoatFixed};", LogLevel.Debug);

            _islandHouse = Game1.player?.mailReceived?.Contains("Island_UpgradeHouse") ?? false;

            var married = Values.GetAllSpouses(Game1.player);
            foreach (var name in married)
            {
                this.Monitor.Log($"Checking NPC {name}...", Config.Debug ? LogLevel.Debug : LogLevel.Trace); //log to debug or trace depending on config

                if (!Values.IntegratedAndEnabled(name)) continue;
                
                MarriedAndAllowed.Add(name);
                this.Monitor.Log($"{name} is married to player.", LogLevel.Debug);
            }
            //e
            Children = Information.PlayerChildren(Game1.player);
            
            PatchPathfind = Information.PlayerSpouses(Game1.player); //add all spouses
            
            if (!InstalledMods["C2N"] && !InstalledMods["LPNCs"])
                return;
            foreach(var kid in Children)
            {
                PatchPathfind.Add(kid.Name);
            }
        }

        //these add and/or depend on config
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ClearValues();

            //check for compatibility, log result
            InstalledMods["SVE"] = Information.HasMod("FlashShifter.StardewValleyExpandedCP");
            InstalledMods["C2N"] = Information.HasMod("Loe2run.ChildToNPC");
            InstalledMods["LNPCs"] = Information.HasMod("Candidus42.LittleNPCs");
            InstalledMods["ExGIM"] = Information.HasMod("mistyspring.extraGImaps");
            InstalledMods["Devan"] = Information.HasMod("mistyspring.NPCDevan");

            Monitor.Log($"\n   Mod info: {InstalledMods}", LogLevel.Debug);

            //choose random
            RandomizedInt = Random.Next(1, 101);
            IslandToday = Config.CustomChance >= RandomizedInt || Config.Debug;

            // get CP's api and register token
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if(api is not null)
            {
                api.RegisterToken(this.ModManifest, "CanVisitIsland", () =>
                {
                    // is island day
                    return IslandToday ? new[] { "true" } : new[] { "false" };
                });

                api.RegisterToken(this.ModManifest, "Invited", () =>
                {
                    // is island day NOT ticket
                    if (IslandToday && !IsFromTicket && LoadedBasicData)
                        return Information.PlayerSpouses(Game1.player);
                    //if ticket island
                    else if (IsFromTicket && LoadedBasicData)
                        return Status.Who.ToArray();
                    else
                        return new[] { "none" };
                });

                api.RegisterToken(this.ModManifest, "Devan", () =>
                {
                    return Config.NPCDevan switch
                    {
                        // on, not seasonal
                        true when Config.SeasonalDevan == false => new[] { "enabled" },
                        // on, seasonal
                        true when Config.SeasonalDevan => new[] { "enabled", "seasonal" },
                        _ => new[] { "false" }
                    };
                });

                api.RegisterToken(this.ModManifest, "AllowChildren", () =>
                {
                    var canGo = Config.UseFurnitureBed == false || (Config.UseFurnitureBed && BedCode.HasAnyKidBeds()) && Context.IsWorldReady;

                    return Config.Allow_Children switch
                    {
                        // on, has bed
                        true when canGo => new[] { "true" },
                        // doesnt
                        true when true => new[] { "false" },
                        _ => new[] { "false" }
                    };
                });

                api.RegisterToken(this.ModManifest, "HasChildren", () =>
                {
                    return Context.IsWorldReady ? new[] { $"{Game1.player.getChildrenCount() != 0}" } : new[] { "false" };
                });

                api.RegisterToken(this.ModManifest, "IslandAtt", () =>
                {
                    return Config.IslandClothes ? new[] {"true"} : new[] {"false"};
                });
            }

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;
            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            #region basic config options
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

            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "Devan",
                text: () => Helper.Translation.Get("config.Devan_Nosit.name")+ "..."
            );
                
            if (InstalledMods["C2N"]||InstalledMods["LPNCs"])
            {
                configMenu.AddPageLink(
                    mod: ModManifest,
                    pageId: "C2Nconfig",
                    text: () => "Child NPC...",
                    tooltip: () => Helper.Translation.Get("config.Child2NPC.description")
                );
            }

            //links to config pages
            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "advancedConfig",
                text: () => Helper.Translation.Get("config.advancedConfig.name"),
                tooltip: () => Helper.Translation.Get("config.advancedConfig.description")
            );
            #endregion

            if (InstalledMods["C2N"]||InstalledMods["LPNCs"])
            {
                configMenu.AddPage(
                    mod: ModManifest,
                    pageId: "C2Nconfig",
                    pageTitle: () => "Child NPC..."
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => Helper.Translation.Get("config.ChildVisitIsland.name"),
                    tooltip: () => Helper.Translation.Get("config.ChildVisitIsland.description"),
                    getValue: () => Config.Allow_Children,
                    setValue: value => Config.Allow_Children = value
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
                        texture: Integrated.KbcSamples,
                        texturePixelArea: null,
                        scale: 1
                    );
                }
            }
                
            #region devan
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "Devan",
                pageTitle: () => Helper.Translation.Get("config.Devan_Nosit.name")
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.Enabled"),
                tooltip: () => Helper.Translation.Get("config.Devan_Nosit.description"),
                getValue: () => Config.NPCDevan,
                setValue: value => Config.NPCDevan = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.SeasonalDevan.name"),
                tooltip: () => Helper.Translation.Get("config.SeasonalDevan.description"),
                getValue: () => Config.SeasonalDevan,
                setValue: value => Config.SeasonalDevan = value
            );
#endregion
            #region adv. config page
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "advancedConfig",
                pageTitle: () => Helper.Translation.Get("config.advancedConfig.name")
            );
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "SVE",
                tooltip: () => ModEntry.Help.Translation.Get("config.Vanillas.description")
            );
            //all spouse bools below
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Abigail",
                tooltip: () => null,
                getValue: () => Config.Allow_Abigail,
                setValue: value => Config.Allow_Abigail = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Alex",
                tooltip: () => null,
                getValue: () => Config.Allow_Alex,
                setValue: value => Config.Allow_Alex = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Elliott",
                tooltip: () => null,
                getValue: () => Config.Allow_Elliott,
                setValue: value => Config.Allow_Elliott = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Emily",
                tooltip: () => null,
                getValue: () => Config.Allow_Emily,
                setValue: value => Config.Allow_Emily = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Haley",
                tooltip: () => null,
                getValue: () => Config.Allow_Haley,
                setValue: value => Config.Allow_Haley = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Harvey",
                tooltip: () => null,
                getValue: () => Config.Allow_Harvey,
                setValue: value => Config.Allow_Harvey = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Krobus",
                tooltip: () => null,
                getValue: () => Config.Allow_Krobus,
                setValue: value => Config.Allow_Krobus = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Leah",
                tooltip: () => null,
                getValue: () => Config.Allow_Leah,
                setValue: value => Config.Allow_Leah = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Maru",
                tooltip: () => null,
                getValue: () => Config.Allow_Maru,
                setValue: value => Config.Allow_Maru = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Penny",
                tooltip: () => null,
                getValue: () => Config.Allow_Penny,
                setValue: value => Config.Allow_Penny = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Sam",
                tooltip: () => null,
                getValue: () => Config.Allow_Sam,
                setValue: value => Config.Allow_Sam = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Sebastian",
                tooltip: () => null,
                getValue: () => Config.Allow_Sebastian,
                setValue: value => Config.Allow_Sebastian = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Shane",
                tooltip: () => null,
                getValue: () => Config.Allow_Shane,
                setValue: value => Config.Allow_Shane = value
            );

            if (InstalledMods["SVE"])
            {
                configMenu.AddSectionTitle(
                    mod: ModManifest,
                    text: () => "SVE",
                    tooltip: null
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Claire",
                    tooltip: () => Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => Config.Allow_Claire,
                    setValue: value => Config.Allow_Claire = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Lance",
                    tooltip: () => Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => Config.Allow_Lance,
                    setValue: value => Config.Allow_Lance = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Magnus",
                    tooltip: () => Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => Config.Allow_Magnus,
                    setValue: value => Config.Allow_Magnus = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Olivia",
                    tooltip: () => Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => Config.Allow_Olivia,
                    setValue: value => Config.Allow_Olivia = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Sophia",
                    tooltip: () => Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => Config.Allow_Sophia,
                    setValue: value => Config.Allow_Sophia = value
                );
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => "Victor",
                    tooltip: () => Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => Config.Allow_Victor,
                    setValue: value => Config.Allow_Victor = value
                );
            }
            #endregion
            #region debugging
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("config.Debug"),
                tooltip: null
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.DebugComm.name"),
                tooltip: () => Helper.Translation.Get("config.DebugComm.description"),
                getValue: () => Config.Debug,
                setValue: value => Config.Debug = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.Verbose.name"),
                tooltip: () => Helper.Translation.Get("config.Verbose.description"),
                getValue: () => Config.Verbose,
                setValue: value => Config.Verbose = value
            );
            #endregion
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            /* Format:
             * 1. Word
             * 2. if partial OK (e.g Word1)
             * 3. if subfolder OK (e.g Word/Sub/file)
             */

            //dialogue is added regardless of conditions
            if (e.Name.StartsWith("Characters/Dialogue/", false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/MarriageDialogueKrobus"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        data.Add("funLeave_Krobus", this.Helper.Translation.Get("Krobus.GoOutside"));
                    });

                Integrated.Dialogues(e);

            }

            //same with devan
            if (Config.NPCDevan && !InstalledMods["Devan"])
            {
                this.Monitor.LogOnce("Adding Devan", LogLevel.Debug);

                if (e.Name.StartsWith("Data/", false))
                {
                    if (e.Name.StartsWith("Data/Festivals/", false, false))
                    {
                        Devan.AppendFestivalData(e);
                    }
                    else
                    {
                        Devan.MainData(e);
                    }
                }
            }

            //and map edits
            if (e.Name.StartsWith("Maps/", false))
            {
                Integrated.IslandMaps(e);
            }

            /* if hasnt unlocked island:
             * returns / doesnt apply these patches
             */
            if (!_islandHouse || !IslandToday)
            {
                return;
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/FishShop"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsMap();
                    var map = editor.Data;
                    map.Properties.Add("NPCWarp", "4 3 IslandSouth 19 43");
                });
            }

            if (e.Name.StartsWith("Characters/schedules/", false))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/Krobus"))
                {
                    e.LoadFrom(
                        () => new Dictionary<string, string>(),
                        AssetLoadPriority.Low);
                } 
                Integrated.KidSchedules(e);
            }
        }
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            //get new %
            PreviousDayRandom = RandomizedInt;
            RandomizedInt = Random.Next(1, 101);
            IslandToday = Config.CustomChance >= RandomizedInt;
            IsFromTicket = false;

            var hadYesterday = Config.CustomChance >= PreviousDayRandom;

            /* reload. just in case.
             * if island yesterday AND not today (or viceversa).*/
            if (IslandToday && !hadYesterday || hadYesterday && RandomizedInt > Config.CustomChance)
            {
                foreach (var spouse in Status.Who)
                {
                    //invalidate schedule, portrait AND dialogue
                    Helper.GameContent.InvalidateCache($"Characters/schedules/{spouse}");

                    if (spouse == "Krobus" || spouse == "Harvey")
                    {
                        Helper.GameContent.InvalidateCache($"Portraits/{spouse}");
                        Helper.GameContent.InvalidateCache($"Characters/{spouse}");
                    }

                    if (spouse != "Krobus") continue;
                    
                    var npc = Game1.getCharacterFromName(spouse,false);
                    npc.Sprite.SpriteHeight = 24; //original size
                    npc.Sprite.UpdateSourceRect();
                    npc.reloadSprite();
                }
            }

            //if flag is set in status
            var ticketday = Status.DayVisit;
            var ticketweek = Status.WeekVisit.Item1;

            //if player used a visit ticket
            if (ticketday || ticketweek)
            {
                RandomizedInt = 0;
                IslandToday = true;
                IsFromTicket = true;

                //remove flags
                Game1.player.RemoveMail("VisitTicket_day");

                Status.DayVisit = ticketday;
                Status.WeekVisit = (ticketweek, Status?.WeekVisit.Item2 ?? 0);


                //if true, check int value. if 7, reset. else, add 1
                // ReSharper disable once PossibleNullReferenceException
                var week = Status.WeekVisit;
                if (week.Item1)
                {
                    if(week.Item2 == 7)
                    {
                        Game1.player.RemoveMail("VisitTicket_week");
                        Status.WeekVisit = (false, 0);
                    }
                    else
                    {
                        Status.WeekVisit = (true, week.Item2 + 1);
                    }
                }
            }
            //if not
            else
            {
                //clear inviteds list
                Status.Who.Clear();

                //if still island
                if (IslandToday)
                {
                    Status.Who = Information.PlayerSpouses(Game1.player);
                }
            }

            /* then we save values. this is done regardless of status

             * check if theres other savedata(s), and include accordingly
             */

            this.Helper.Data.WriteJsonFile(Datapath, Status);

            //remove the values from status
            //(only do for day, week has its own thing).
            Status.DayVisit = false;

            Children = Information.PlayerChildren(Game1.player);
            //get for patching
            PatchPathfind = Information.PlayerSpouses(Game1.player); //add all spouses
            if (!InstalledMods["C2N"] && !InstalledMods["LPNCs"])
                return;
            foreach (var kid in Children)
            {
                PatchPathfind.Add(kid.Name);
            }
        }
        private void TitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            ClearValues();

            //get new %
            PreviousDayRandom = 0;
            RandomizedInt = Random.Next(1, 101);
        }

        //methods to get/clear values
        private void ClearValues()
        {
            Status = new();

            this.Monitor.Log("Clearing Children...");
            Children?.Clear();

            //empty bools and int
            LoadedBasicData = false;
            PreviousDayRandom = 0;
            RandomizedInt = 0;

            this.Monitor.Log("SawDevan4H = false; CCC = false; RandomizedInt = 0;");
        }

        private void ReadModData(Farmer player)
        {
            DevanExists = Config.NPCDevan || InstalledMods["Devan"];

            var file = Helper.Data.ReadJsonFile<ModStatus>(Datapath);
            if(file == null)
            {
                Status = new ModStatus(player, IslandToday); 
            }
            else
            {
                if (file.DayVisit)
                {
                    //set to true n remove
                    IsFromTicket = true;
                    IslandToday = true;
                    RandomizedInt = 0;
                    file.DayVisit = false;
                }
                if (file.WeekVisit.Item1)
                {
                    var wv = file.WeekVisit;
                    //check value
                    if (wv.Item2 == 7)
                    {
                        file.WeekVisit = (false, 0);
                    }
                    else
                    {
                        IsFromTicket = true;
                        IslandToday = true;
                        RandomizedInt = 0;
                        file.WeekVisit = (true, wv.Item2 + 1);
                    }
                }
                else
                {
                    Status = file;
                }
            }
        }

        /* Helpers + things the mod uses */

        internal static ModConfig Config;
        private static Random _random;
        internal static IModHelper Help { get; private set; }
        // ReSharper disable once InconsistentNaming
        internal static ITranslationHelper TL { get; private set; }
        internal static IMonitor Mon { get; private set; }
        internal static Random Random
        {
            get
            {
                _random ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed * 36));
                return _random;
            }
        }

        /* User-related starts here */
        internal static bool IslandToday { get; private set; }
        internal static bool IsFromTicket { get; private set; }
        internal static int RandomizedInt { get; private set; }
        internal static int PreviousDayRandom { get; private set; }
        private static bool LoadedBasicData {get; set;}

        /* children related */
        internal static List<Character> Children { get; private set; } = new();

        /* player data */
        public static List<string> MarriedAndAllowed { get; } = new();
        internal static bool BoatFixed;
        private static bool _islandHouse;
        
        internal static Dictionary<string,bool> InstalledMods = new(){
            {"SVE",false},
            {"C2N",false},
            {"LPNCs",false},
            {"ExGIM",false},
            {"Devan",false}
        };

        private static string Datapath => Context.IsWorldReady ? $"{Constants.CurrentSavePath}/SGI/data.json" : null;

        internal static bool DevanExists;
        internal static List<string> PatchPathfind { get; private set; } = new();

        internal static ModStatus Status { get; private set; }
    }
}