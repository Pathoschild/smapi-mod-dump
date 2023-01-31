/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using System;
using System.Collections.Generic;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Enums;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Linq;
using HarmonyLib;
using JsonAssets;

namespace SpousesIsland
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //adds config and loads assets
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;

            //changes mod info (and npcs)
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.ReturnedToTitle += this.TitleReturn;

            //gets user data
            helper.Events.Multiplayer.PeerContextReceived += PeerContextReceived;
            helper.Events.Specialized.LoadStageChanged += LoadStageChanged;

            this.Config = this.Helper.ReadConfig<ModConfig>();

            Mon = this.Monitor;
            Help = this.Helper;
            TL = this.Helper.Translation;
            IsDebug = Config.Debug;

            //commands
            if (Config.Debug is true)
            {
                helper.ConsoleCommands.Add("ichance", helper.Translation.Get("CLI.chance"), Debugging.Chance);
                helper.ConsoleCommands.Add("getstat", "", Debugging.GetStatus);
                helper.ConsoleCommands.Add("playerdata", "", Debugging.GeneralInfo);
            }

            this.Monitor.Log($"Applying Harmony patch \"{nameof(Patches)}\": prefixing SDV method \"NPC.tryToReceiveActiveObject(Farmer who)\".");
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.NPC), nameof(StardewValley.NPC.tryToReceiveActiveObject)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.tryToReceiveTicket))
                );
        }

        //these add and/or depend on config
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ClearValues();

            //check for compatibility, log result
            HasSVE = Information.HasMod("FlashShifter.StardewValleyExpandedCP");
            HasC2N = Information.HasMod("Loe2run.ChildToNPC");
            HasExGIM = Information.HasMod("mistyspring.extraGImaps");
            notfurniture = Config.UseFurnitureBed == false;

            if(HasC2N)
            {
                //C2N has a bug with warping in fish shop. Only this version and older ones will allow patching from my mod's side (assuming this gets fixed next update)
                MustPatchC2N = Information.IsVersionOrLower("Loe2run.ChildToNPC", "1.2.1-unofficial.8-candidus42");
            }

            this.Monitor.Log($"\n   HasSVE = {HasSVE}\n   HasC2N = {HasC2N}\n   HasExGIM = {HasExGIM}", LogLevel.Debug);

            //choose random
            RandomizedInt = Random.Next(1, 101);
            IslandToday = Config.CustomChance >= RandomizedInt;
            IslandToday = true;
            /* get all content packs installed - deprecated
            GetContentPacks(); */

            // get CP's api and register token
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if(api is not null)
            {
                api.RegisterToken(this.ModManifest, "CanVisitIsland", () =>
                {
                    // is island day
                    if (IslandToday)
                        return new string[] { "true" }; 

                    else
                        return new string[] { "false" };
                });

                api.RegisterToken(this.ModManifest, "Invited", () =>
                {
                    // is island day NOT ticket
                    if (IslandToday && !IsFromTicket && LoadedBasicData)
                        return Information.PlayerSpouses(Player_MP_ID);
                    //if ticket island
                    else if (IsFromTicket && LoadedBasicData)
                        return Status[Player_MP_ID].Who.ToArray();
                    else
                        return new string[] { "none" };
                });

                api.RegisterToken(this.ModManifest, "Devan", () =>
                {
                    // on, not seasonal
                    if (Config.NPCDevan && Config.SeasonalDevan == false)
                        return new string[] {"enabled"};
                    // on, seasonal
                    else if (Config.NPCDevan && Config.SeasonalDevan)
                        return new string[] {"enabled","seasonal"};
                    // off
                    else
                        return new string[] { "false" };
                });

                api.RegisterToken(this.ModManifest, "AllowChildren", () =>
                {
                    var CanGo = Config.UseFurnitureBed == false || (Config.UseFurnitureBed && BedCode.HasAnyKidBeds()) && Context.IsWorldReady;
                
                    // on, has bed
                    if (Config.Allow_Children && CanGo)
                        return new string[] {"true"};
                    // doesnt
                    else if (Config.Allow_Children && !CanGo)
                        return new string[] {"false"};
                    // off
                    else
                        return new string[] { "false" };
                });
            }

            //InfoChildren = ChildrenData.GetInformation(Config.ChildSchedules);

            jsonAssets = Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is not null)
            {
                // register mod
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config)
                );

                // basic config options
                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.CustomChance.name"),
                    tooltip: () => this.Helper.Translation.Get("config.CustomChance.description"),
                    getValue: () => this.Config.CustomChance,
                    setValue: value => this.Config.CustomChance = value,
                    min: 0,
                    max: 100,
                    interval: 1
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.CustomRoom.name"),
                    tooltip: () => this.Helper.Translation.Get("config.CustomRoom.description"),
                    getValue: () => this.Config.CustomRoom,
                    setValue: value => this.Config.CustomRoom = value
                );

                //devan
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.Devan_Nosit.name"),
                    tooltip: () => this.Helper.Translation.Get("config.Devan_Nosit.description"),
                    getValue: () => this.Config.NPCDevan,
                    setValue: value => this.Config.NPCDevan = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.SeasonalDevan.name"),
                    tooltip: () => this.Helper.Translation.Get("config.SeasonalDevan.description"),
                    getValue: () => this.Config.SeasonalDevan,
                    setValue: value => this.Config.SeasonalDevan = value
                );

                //random place
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.ScheduleRandom.name"),
                    tooltip: () => this.Helper.Translation.Get("config.ScheduleRandom.description"),
                    getValue: () => this.Config.ScheduleRandom,
                    setValue: value => this.Config.ScheduleRandom = value
                );
                //links to config pages
                configMenu.AddPageLink(
                    mod: this.ModManifest,
                    pageId: "advancedConfig",
                    text: () => this.Helper.Translation.Get("config.advancedConfig.name"),
                    tooltip: () => this.Helper.Translation.Get("config.advancedConfig.description")
                );

                if (HasC2N is true)
                {
                    configMenu.AddPageLink(
                        mod: this.ModManifest,
                        pageId: "C2Nconfig",
                        text: () => "Child2NPC...",
                        tooltip: () => this.Helper.Translation.Get("config.Child2NPC.description")
                    );
                    configMenu.AddPage(
                        mod: this.ModManifest,
                        pageId: "C2Nconfig",
                        pageTitle: () => "Child2NPC..."
                    );
                    configMenu.AddBoolOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.ChildVisitIsland.name"),
                        tooltip: () => this.Helper.Translation.Get("config.ChildVisitIsland.description"),
                        getValue: () => this.Config.Allow_Children,
                        setValue: value => this.Config.Allow_Children = value
                    );
                    configMenu.AddBoolOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.UseFurnitureBed.name"),
                        tooltip: () => this.Helper.Translation.Get("config.UseFurnitureBed.description"),
                        getValue: () => this.Config.UseFurnitureBed,
                        setValue: value => this.Config.UseFurnitureBed = value 
                    );
                    if (Config.UseFurnitureBed == false) //if it's not bed furniture: lets you decide the "mod bed" color.
                    {
                        configMenu.AddTextOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.Childbedcolor.name"),
                        tooltip: () => this.Helper.Translation.Get("config.Childbedcolor.description"),
                        getValue: () => this.Config.Childbedcolor,
                        setValue: value => this.Config.Childbedcolor = value,
                        allowedValues: new string[] { "1", "2", "3", "4", "5", "6" }
                    );
                        configMenu.AddImage(
                        mod: this.ModManifest,
                        texture: Integrated.KbcSamples,
                        texturePixelArea: null,
                        scale: 1
                    );
                    }/*
                    configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.UseModSchedule.name"),
                    tooltip: () => this.Helper.Translation.Get("config.UseModSchedule.description"),
                    getValue: () => this.Config.UseModSchedule,
                    setValue: value => this.Config.UseModSchedule = value
                    );*/
                }
                
                //adv. config page
                configMenu.AddPage(
                    mod: this.ModManifest,
                    pageId: "advancedConfig",
                    pageTitle: () => this.Helper.Translation.Get("config.advancedConfig.name")
                );
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: Titles.SpouseT,
                    tooltip: Integrated.SpouseD
                );
                //all spouse bools below
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Abigail",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Abigail,
                    setValue: value => this.Config.Allow_Abigail = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Alex",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Alex,
                    setValue: value => this.Config.Allow_Alex = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Elliott",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Elliott,
                    setValue: value => this.Config.Allow_Elliott = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Emily",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Emily,
                    setValue: value => this.Config.Allow_Emily = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Haley",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Haley,
                    setValue: value => this.Config.Allow_Haley = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Harvey",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Harvey,
                    setValue: value => this.Config.Allow_Harvey = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Krobus",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Krobus,
                    setValue: value => this.Config.Allow_Krobus = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Leah",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Leah,
                    setValue: value => this.Config.Allow_Leah = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Maru",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Maru,
                    setValue: value => this.Config.Allow_Maru = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Penny",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Penny,
                    setValue: value => this.Config.Allow_Penny = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Sam",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Sam,
                    setValue: value => this.Config.Allow_Sam = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Sebastian",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Sebastian,
                    setValue: value => this.Config.Allow_Sebastian = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Shane",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Shane,
                    setValue: value => this.Config.Allow_Shane = value
                );
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: Titles.SVET,
                    tooltip: null
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Claire",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Claire,
                    setValue: value => this.Config.Allow_Claire = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Lance",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Lance,
                    setValue: value => this.Config.Allow_Lance = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Magnus",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Magnus,
                    setValue: value => this.Config.Allow_Magnus = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Olivia",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Olivia,
                    setValue: value => this.Config.Allow_Olivia = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Sophia",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Sophia,
                    setValue: value => this.Config.Allow_Sophia = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Victor",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Victor,
                    setValue: value => this.Config.Allow_Victor = value
                );
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: Titles.Debug,
                    tooltip: null
                );
                //debug options
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.DebugComm.name"),
                    tooltip: () => this.Helper.Translation.Get("config.DebugComm.description"),
                    getValue: () => this.Config.Debug,
                    setValue: value => this.Config.Debug = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.Verbose.name"),
                    tooltip: () => this.Helper.Translation.Get("config.Verbose.description"),
                    getValue: () => this.Config.Verbose,
                    setValue: value => this.Config.Verbose = value
                );
            }
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            /* Format:
             * 1. Word
             * 2. if partial OK (e.g Word1)
             * 3. if subfolder OK (e.g Word/Sub/file)
             */
            if (e.Name.StartsWith("Maps/", false, true))
            {
                Integrated.Maps(e);
                Integrated.IslandMaps(e, Config);
            }
            
            if (Config.NPCDevan == true)
            {
                this.Monitor.LogOnce("Adding Devan", LogLevel.Debug);

                if (e.Name.StartsWith("Data/", false, true))
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

            /* if hasnt unlocked island:
             * returns / doesnt apply these patches
             */
            if (!IslandHouse)
            {
                return;
            }


            if (e.Name.Equals("Data/mail") && IslandHouse)
            {
                string fullmail = "@,^" + this.Helper.Translation.Get("Islandvisit_Qi") + $"%item object {jsonAssets?.GetObjectId("Island ticket (day)")} %%";
                e.Edit(asset => {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Islandvisit_Qi", fullmail);
                });
            }

            if (e.Name.StartsWith("Characters/schedules/", false, true))
                Integrated.Schedules(e);

            if (e.Name.StartsWith("Characters/Dialogue/", false, true))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/MarriageDialogueKrobus"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        data.Add("funLeave_Krobus", this.Helper.Translation.Get("Krobus.GoOutside"));
                    });
                    
                Integrated.Dialogues(e);

            }
        }

        //if SP, loadstagechanged will obtain required data. if MP, peercontext will.
        private void LoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.SaveLoadedBasicInfo)
            {
                GetRequiredData(Game1.MasterPlayer);
            }
            if(e.NewStage == LoadStage.Ready)
            {
                //get kids
                Children = Information.PlayerChildren(Game1.player);
                //get for patching
                MustPatchPF = Information.PlayerSpouses(Player_MP_ID); //add all spouses
                if (!HasC2N)
                    return;
                foreach(var kid in Children)
                {
                    MustPatchPF.Add(kid.Name);
                }
            }
        }
        private void PeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            var PID = e.Peer?.PlayerID ?? 0; //set in a variable just in case

            var newFarmer = Game1.getFarmer(PID);

            if (!newFarmer.IsLocalPlayer)
            {
                return;
            }

            GetRequiredData(newFarmer);
        }

        //these happen regardless of SP/MP
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
                foreach (var spouse in Status[Player_MP_ID].Who)
                {
                    //invalidate schedule, portrait AND dialogue
                    Helper.GameContent.InvalidateCache($"Characters/schedules/{spouse}");

                    if (spouse == "Krobus" || spouse == "Harvey")
                    {
                        Helper.GameContent.InvalidateCache($"Portraits/{spouse}");
                        Helper.GameContent.InvalidateCache($"Characters/{spouse}");
                    }
                }
            }

            //if flag is set in status
            var ticketday = Status[Player_MP_ID].DayVisit;
            var ticketweek = Status[Player_MP_ID].WeekVisit.Item1;

            //if player used a visit ticket
            if (ticketday || ticketweek)
            {
                RandomizedInt = 0;
                IslandToday = true;
                IsFromTicket = true;

                //remove flags
                Game1.player.RemoveMail("VisitTicket_day");

                //get id + set them in mod file. ticketday set to true for future loading
                var playerID = Game1.player.UniqueMultiplayerID.ToString();

                Status[playerID].DayVisit = ticketday;
                Status[playerID].WeekVisit = (ticketweek, Status[playerID]?.WeekVisit.Item2 ?? 0);


                //if true, check int value. if 7, reset. else, add 1
                var week = Status[playerID].WeekVisit;
                if (week.Item1)
                {
                    if(week.Item2 == 7)
                    {
                        Game1.player.RemoveMail("VisitTicket_week");
                        Status[playerID].WeekVisit = (false, 0);
                    }
                    else
                    {
                        Status[playerID].WeekVisit = (true, week.Item2 + 1);
                    }
                }

                //check if theres other values in file
                var file = Helper.Data.ReadJsonFile<Dictionary<string, ModStatus>>("moddata.json");

                //remove user's outdated info + duplicate status (temp var)
                file?.Remove(playerID);
                var allStatuses = Status;

                //if theres data of other savefiles
                if (file != null && file?.Count != 0)
                {
                    //add each one
                    foreach(var data in file)
                    {
                        allStatuses.TryAdd(data.Key, data.Value);
                    }
                }

                this.Helper.Data.WriteJsonFile("moddata.json", allStatuses);

                //remove the values from status
                //(only do for day, week has its own thing).
                Status[playerID].DayVisit = false;
            }
            //if not
            else
            {
                //clear inviteds list
                Status[Player_MP_ID].Who.Clear();

                //if still island
                if (IslandToday)
                {
                    Status[Player_MP_ID].Who = Information.PlayerSpouses(Player_MP_ID);
                }
            }

            //re-check values
            Children = Information.PlayerChildren(Game1.player);
            //get for patching
            MustPatchPF = Information.PlayerSpouses(Player_MP_ID); //add all spouses
            if (!HasC2N)
                return;
            foreach (var kid in Children)
            {
                MustPatchPF.Add(kid.Name);
            }
        }
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            //avoid running unnecessary code
            if(IslandToday == false)
                return;

            //patch schedules manually
            if (e.NewTime >= 2000)
            {
                var farmHouse = Game1.getLocationFromName("IslandFarmHouse");
                //var islandW = Game1.getLocationFromName("IslandWest");

                foreach (var chara in MustPatchPF)
                {
                    var npc = Game1.getCharacterFromName(chara, false, false);

                    //i still don't understand how schedulepath directions work internally. so let's use controller instead
                    //Helper.Data.WriteJsonFile($"{chara}-path.json", npc.Schedule);

                    var inIsland = npc?.currentLocation?.GetLocationContext() == GameLocation.LocationContext.Island;
                    
                    if (!inIsland || !(npc.controller.endPoint == Point.Zero))
                    {
                        continue;
                    }

                    npc.followSchedule = false;
                    //get location from CONTROLLER location
                    npc.Halt();
                    npc.controller = null;
                    npc.controller = new PathFindController
                        (npc, farmHouse,
                        Information.GetReturnPoint(npc), 0
                        );
                    Monitor.Log("Attempting to fix null endpoint in schedule...", LogLevel.Debug);
                }
            }

            //if 10pm or later. code for npcs to sleep
            if (e.NewTime >= 2200)
            {
                var bc = new BedCode();
                var ifh = Game1.getLocationFromName("IslandFarmHouse");

                foreach (NPC c in ifh.characters)
                {
                    if(c.isMarried())
                    {
                        this.Monitor.Log($"Pathing {c.Name} to bed in {ifh.Name}...");
                        try
                        {
                            bc.MakeSpouseGoToBed(c, ifh);
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                        }
                    }
                    
                    else if(!c.isMarried() && !c.isRoommate() && HasC2N == true)
                    {
                        this.Monitor.Log($"Pathing {c.Name} to kid bed in {ifh.Name}...");
                        try
                        {
                            bc.MakeKidGoToBed(c, ifh);
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                        }
                    }
                }
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
            Player_MP_ID = null;

            this.Monitor.Log("Clearing Children...");
            Children?.Clear();

            //empty bools and int
            LoadedBasicData = false;
            PreviousDayRandom = 0;
            RandomizedInt = 0;

            this.Monitor.Log("SawDevan4H = false; CCC = false; RandomizedInt = 0;");
        }
        private void GetRequiredData(Farmer player)
        {
            //set for using in Integrated.cs
            Player_MP_ID = player.UniqueMultiplayerID.ToString();
            LoadedBasicData = true;

            //try to read file from moddata. if empty, check mail
            //read here. 
            ReadModData(player);

            //now get user data
            var boatFix = player?.mailReceived?.Contains("willyBoatFixed");
            BoatFixed = boatFix ?? false;
            this.Monitor.Log($"BoatFixed = {BoatFixed};", LogLevel.Debug);

            IslandHouse = player?.mailReceived?.Contains("Island_UpgradeHouse") ?? false;
            bool QiMail = player?.mailReceived?.Contains("Islandvisit_Qi") ?? false;
            if (!QiMail && IslandHouse)
            {
                player.mailForTomorrow.Add("Islandvisit_Qi");
            }

            var married = Values.GetAllSpouses(player);
            foreach (var name in married)
            {
                this.Monitor.Log($"Checking NPC {name}...", IsDebug ? LogLevel.Debug : LogLevel.Trace); //log to debug or trace depending on config

                if (Values.IntegratedAndEnabled(name, Config))
                {
                    MarriedAndAllowed.Add(name);
                    this.Monitor.Log($"{name} is married to player.", LogLevel.Debug);
                }
            }
        }
        private void ReadModData(Farmer player)
        {
            var userID = player.UniqueMultiplayerID.ToString(); //userID causes a NRE, use MP id. 
            var file = Helper.Data.ReadJsonFile<Dictionary<string, ModStatus>>("moddata.json");
            if(file == null)
            {
                Status.Add(userID, new ModStatus(player, IslandToday)); 
            }
            else if (file.Keys.Any(id => id == userID))
            {
                if (file[userID].DayVisit)
                {
                    //set to true n remove
                    IsFromTicket = true;
                    IslandToday = true;
                    RandomizedInt = 0;
                    file[userID].DayVisit = false;
                }
                if (file[userID].WeekVisit.Item1)
                {
                    var wv = file[userID].WeekVisit;
                    //check value
                    if (wv.Item2 == 7)
                    {
                        file[userID].WeekVisit = (false, 0);
                    }
                    else
                    {
                        IsFromTicket = true;
                        IslandToday = true;
                        RandomizedInt = 0;
                        file[userID].WeekVisit = (true, wv.Item2 + 1);
                    }
                }
                else
                {
                    Status = file;
                }
            }
            else
            {
                Status.Add(userID, new ModStatus(player, IslandToday));
            }
        }

        /* Helpers + things the mod uses */

        private ModConfig Config;
        public static IApi jsonAssets;
        private static Random random;
        internal static IModHelper Help { get; private set; }
        internal static ITranslationHelper TL { get; private set; }
        internal static IMonitor Mon { get; private set; }
        internal static Random Random
        {
            get
            {
                random ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed * 36));
                return random;
            }
        }

        /* User-related starts here */

        internal static bool IsDebug = false;
        internal static bool IslandToday { get; private set; }
        internal static bool IsFromTicket { get; private set; } = false;
        internal static int RandomizedInt { get; private set; }
        internal static int PreviousDayRandom { get; private set; }
        internal static bool LoadedBasicData {get; private set;} = false;

        /* children related */
        internal static List<Character> Children { get; private set; } = new();
        internal static Dictionary<string,string> InfoChildren = new(); //this refers to info in relation to the mod (ie, schedule data for island visit). not actual info
        internal static bool MustPatchC2N = false;

        /* player data */
        internal static string Player_MP_ID;
        public static List<string> MarriedAndAllowed { get; private set; } = new();
        internal static bool BoatFixed;
        internal static bool IslandHouse = false;
        internal static bool HasSVE;
        internal static bool HasC2N;
        internal static bool HasExGIM;
        internal static bool notfurniture;
        internal static List<string> MustPatchPF { get; set; } = new();

        internal static Dictionary<string, ModStatus> Status { get; private set; } = new();
    }
}