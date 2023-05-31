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
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace FarmVisitors
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;

            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChange;
            helper.Events.GameLoop.DayEnding += this.DayEnding;

            helper.Events.Player.Warped += FarmOutside.PlayerWarp;

            helper.Events.GameLoop.ReturnedToTitle += this.TitleReturn;
            helper.Events.Content.AssetRequested += Extras.AssetRequest;

            ModEntry.Config = this.Helper.ReadConfig<ModConfig>();

            Log = this.Monitor.Log;
            TL = this.Helper.Translation;

            if (ModEntry.Config.Debug)
            {
                helper.ConsoleCommands.Add("print", "List the values requested.", Debugging.Print);
                helper.ConsoleCommands.Add("vi_reload", "Reload visitor info.", this.Reload);
                helper.ConsoleCommands.Add("vi_force", "Force a visit to happen.", Debugging.ForceVisit);
            }
        }

        #region hooks
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (ModEntry.Config.Debug)
            {
                this.Monitor.Log("Debug has been turned on. This will change configuration for testing purposes.", LogLevel.Warn);

                this.Monitor.Log("Chance set to 100 (% every 10 min)");
                ModEntry.Config.CustomChance = 100;
                this.Monitor.Log("Starting hour will be 600.");
                ModEntry.Config.StartingHours = 600;
            }

            var AllowedStringVals = new string[3]
            {
                "VanillaOnly",
                "VanillaAndMod",
                "None"
            };

            //add actions
            ActionList.Add(new Action(Proceed));
            ActionList.Add(new Action(CancelVisit));

            //clear values. better safe than sorry
            ClearValues();

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => ModEntry.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(ModEntry.Config)
            );

            // basic config options
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.CustomChance.name"),
                tooltip: () => this.Helper.Translation.Get("config.CustomChance.description"),
                getValue: () => ModEntry.Config.CustomChance,
                setValue: value => ModEntry.Config.CustomChance = value,
                min: 0,
                max: 100,
                interval: 1
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.GiftChance.name"),
                tooltip: () => this.Helper.Translation.Get("config.GiftChance.description"),
                getValue: () => ModEntry.Config.GiftChance,
                setValue: value => ModEntry.Config.GiftChance = value,
                min: 0,
                max: 100,
                interval: 1
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.MaxVisitsPerDay.name"),
                tooltip: () => this.Helper.Translation.Get("config.MaxVisitsPerDay.description"),
                getValue: () => ModEntry.Config.MaxVisitsPerDay,
                setValue: value => ModEntry.Config.MaxVisitsPerDay = value,
                min: 0,
                max: 24,
                interval: 1
            );
            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: true
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.VisitDuration.name"),
                tooltip: () => this.Helper.Translation.Get("config.VisitDuration.description"),
                getValue: () => ModEntry.Config.Duration,
                setValue: value => ModEntry.Config.Duration = value,
                min: 1,
                max: 20,
                interval: 1
            );
            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: false
            );
            //extra customization
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "Extras",
                text: Extras.ExtrasTL
            );

            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "Extras"
            );

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: Extras.VisitConfiguration,
                tooltip: null);

            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: true
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                getValue: () => ModEntry.Config.Blacklist,
                setValue: value => ModEntry.Config.Blacklist = value,
                name: Extras.BlacklistTL,
                tooltip: Extras.BlacklistTTP
            );
            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: false
                );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.StartingHours.name"),
                tooltip: () => this.Helper.Translation.Get("config.StartingHours.description"),
                getValue: () => ModEntry.Config.StartingHours,
                setValue: value => ModEntry.Config.StartingHours = value,
                min: 600,
                max: 2400,
                interval: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.EndingHours.name"),
                tooltip: () => this.Helper.Translation.Get("config.EndingHours.description"),
                getValue: () => ModEntry.Config.EndingHours,
                setValue: value => ModEntry.Config.EndingHours = value,
                min: 600,
                max: 2400,
                interval: 100
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.WalkOnFarm.name"),
                tooltip: () => this.Helper.Translation.Get("config.WalkOnFarm.description"),
                getValue: () => ModEntry.Config.WalkOnFarm,
                setValue: value => ModEntry.Config.WalkOnFarm = value
            );

            //from here on, ALL config is title-only
            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: true
                );
            
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.UniqueDialogue.name"),
                tooltip: () => this.Helper.Translation.Get("config.UniqueDialogue.description"),
                getValue: () => ModEntry.Config.UniqueDialogue,
                setValue: value => ModEntry.Config.UniqueDialogue = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.AskForConfirmation.name"),
                tooltip: () => this.Helper.Translation.Get("config.AskForConfirmation.description"),
                getValue: () => ModEntry.Config.NeedsConfirmation,
                setValue: value => ModEntry.Config.NeedsConfirmation = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.RejectionDialogues.name"),
                tooltip: () => this.Helper.Translation.Get("config.RejectionDialogues.description"),
                getValue: () => ModEntry.Config.RejectionDialogue,
                setValue: value => ModEntry.Config.RejectionDialogue = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.InLawComments.name"),
                tooltip: () => this.Helper.Translation.Get("config.InLawComments.description"),
                getValue: () => ModEntry.Config.InLawComments,
                setValue: value => ModEntry.Config.InLawComments = value,
                allowedValues: AllowedStringVals,
                formatAllowedValue: value => this.Helper.Translation.Get($"config.InLawComments.values.{value}")
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ReplacerCompat.name"),
                tooltip: () => this.Helper.Translation.Get("config.ReplacerCompat.description"),
                getValue: () => ModEntry.Config.ReplacerCompat,
                setValue: value => ModEntry.Config.ReplacerCompat = value
            );

            //developer config
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: Extras.DebugTL,
                tooltip: null
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.Verbose.name"),
                tooltip: () => this.Helper.Translation.Get("config.Verbose.description"),
                getValue: () => ModEntry.Config.Verbose,
                setValue: value => ModEntry.Config.Verbose = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.Debug.name"),
                tooltip: () => this.Helper.Translation.Get("config.Debug.Explanation"),
                getValue: () => ModEntry.Config.Debug,
                setValue: value => ModEntry.Config.Debug = value
            );

            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: false
                );
        }
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //get translations
            if (ResponseList?.Count is not 2)
            {
                ResponseList.Add(new Response("AllowedEntry", this.Helper.Translation.Get("Visit.Yes")));
                ResponseList.Add(new Response("RejectedEntry", this.Helper.Translation.Get("Visit.No")));
            }

            FirstLoadedDay = true;

            //clear ALL values and temp data on load. this makes sure there's no conflicts with savedata cache (e.g if player had returned to title)
            ClearValues();
            CleanTempData();

            //check config
            if (ModEntry.Config.StartingHours >= ModEntry.Config.EndingHours)
            {
                this.Monitor.Log("Starting hours can't happen after ending hours! To use the mod, fix this and reload savefile.", LogLevel.Error);
                IsConfigValid = false;
                return;
            }
            else
            {
                this.Monitor.Log("User config is valid.");
                IsConfigValid = true;
            }

            //get all possible visitors- which also checks blacklist and married NPCs, etc.
            GetAllVisitors();

            /* if allowed, get all inlaws. 
             * this doesn't need daily reloading. NPC dispositions don't vary 
             * (WON'T add compat for the very small % of mods with conditional disp., its an expensive action)*/
            if (ModEntry.Config.InLawComments is "VanillaAndMod")
            {
                this.Monitor.Log("Getting all in-laws...");
                var tempdict = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
                foreach (string name in NameAndLevel.Keys)
                {
                    var temp = Moddeds.GetInlawOf(tempdict, name);
                    if (temp is not null)
                    {
                        InLaws.Add(name, temp);
                    }
                }
            }
        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            //if faulty config, don't do anything + mark as unvisitable
            if (IsConfigValid == false)
            {
                this.Monitor.Log("Configuration isn't valid. Mod will not work.", LogLevel.Warn);
                CanBeVisited = false;
                return;
            }

            //every sunday, friendship data is reloaded.
            if (Game1.dayOfMonth % 7 == 0 && FirstLoadedDay == false)
            {
                this.Monitor.Log("Day is sunday and not first loaded day. Reloading data...");
                NameAndLevel?.Clear();
                RepeatedByLV?.Clear();

                GetAllVisitors();
            }

            /* if no friendship with anyone OR festival day:
             * make unvisitable
             * return (= dont check custom schedule)
             */
            isFestivalToday = Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
            var anyInLV = RepeatedByLV.Any();
            //log info
            this.Monitor.Log($"isFestivalToday = {isFestivalToday}; anyInLV = {anyInLV}");

            if (anyInLV == false || isFestivalToday)
            {
                CanBeVisited = false;
                return;
            }

            CanBeVisited = true;

            //update animal and crop list
            if (Game1.currentSeason == "winter")
            {
                Crops?.Clear();
                Animals?.Clear();
            }
            else
            {
                Crops = Values.GetCrops();
                Animals = Values.GetAnimals();
            }
        }
        private void OnTimeChange(object sender, TimeChangedEventArgs e)
        {
            if (!CanBeVisited)
            {
                return;
            }

            if (e.NewTime > ModEntry.Config.StartingHours && e.NewTime < ModEntry.Config.EndingHours && CounterToday < ModEntry.Config.MaxVisitsPerDay)
            {
                if (!HasAnyVisitors && HasAnySchedules) //custom
                {
                    foreach (KeyValuePair<string, ScheduleData> pair in SchedulesParsed)
                    {
                        NPC visit = Game1.getCharacterFromName(pair.Key);
                        if (e.NewTime.Equals(pair.Value.From) && Values.IsFree(visit, false))
                        {
                            VisitorData = new TempNPC(visit);

                            currentCustom.Add(pair.Key, pair.Value);

                            VisitorName = visit.Name;

                            DurationSoFar = 0;

                            HasAnyVisitors = true;
                            TimeOfArrival = e.NewTime;
                            ControllerTime = 0;
                            CustomVisiting = true;

                            if(pair.Value.Force.Enable)
                            {
                                this.Monitor.Log($"Adding NPC {VisitorName} by force (scheduled, Force.Enable = {pair.Value.Force.Enable})...");

                                Actions.AddWhileOutside(Game1.getCharacterFromName(VisitorName));

                                ForcedSchedule = true; //set forced to true. (avoids certain behavior)

                                var mail = pair.Value.Force.Mail;
                                if(!(string.IsNullOrWhiteSpace(mail))) //if there's a mail string, add for tomorrow
                                {
                                    Game1.player.mailForTomorrow.Add(mail);
                                }

                                return;

                            }

                            if (ModEntry.Config.NeedsConfirmation)
                            {
                                AskToEnter();
                            }
                            else
                            {
                                //add them to farmhouse (last to avoid issues)
                                Actions.AddCustom(Game1.getCharacterFromName(VisitorName), farmHouse, currentCustom[VisitorName], false);

                                if (ModEntry.Config.Verbose)
                                {
                                    this.Monitor.Log($"\nHasAnyVisitors set to true.\n{VisitorName} will begin visiting player.\nTimeOfArrival = {TimeOfArrival};\nControllerTime = {ControllerTime};", LogLevel.Debug);
                                }
                            }

                            break;
                        }
                    }
                } //custom
                if (!HasAnyVisitors) //random
                {
                    if (Random.Next(1, 101) <= ModEntry.Config.CustomChance && Game1.currentLocation == farmHouseAsLocation)
                    {
                        ChooseRandom();
                    }
                } //random
            }

            if (HasAnyVisitors)
            {
                NPC c = Game1.getCharacterFromName(VisitorName);

                /*if (!(Game1.player.currentLocation.getCharacters().Contains(c))) //this was causing a bug if player left farm
                {
                    if (ModEntry.Config.Debug)
                    {
                        this.Monitor.Log($"Character \"{c.Name}\" ({c.displayName}) is currently not in the map. This time change will not be considered for MaxTimeStay count. (Skipping...)", LogLevel.Debug);
                    }
                    return;
                }*/

                //in the future, add unique dialogue for when characters fall asleep in your house.
                var isVisitSleeping = c.isSleeping.Value;
                if (DurationSoFar >= MaxTimeStay || (CustomVisiting && (bool)(e.NewTime.Equals(currentCustom[VisitorName]?.To))) || isVisitSleeping)
                {
                    this.Monitor.Log($"{c.Name} is retiring for the day.");

                    //if custom AND has custom dialogue: exit with custom. else normal
                    if (CustomVisiting)
                    {
                        var exitd = currentCustom[VisitorName].ExitDialogue;
                        if (!string.IsNullOrWhiteSpace(exitd))
                            Actions.RetireCustom(c, e.NewTime, farmHouse, exitd);
                        else
                            Actions.Retire(c, e.NewTime, farmHouse);

                        CustomVisiting = false;
                        currentCustom.Clear();
                    }
                    else
                    {
                        Actions.Retire(c, e.NewTime, farmHouse);
                    }

                    HasAnyVisitors = false;
                    CounterToday++;
                    TodaysVisitors.Add(VisitorName);
                    DurationSoFar = 0;
                    ControllerTime = 0;
                    VisitorName = null;

                    VisitorData = null;
                    ForcedSchedule = false;

                    if (ModEntry.Config.Verbose)
                    {
                        this.Monitor.Log($"HasAnyVisitors = false, CounterToday = {CounterToday}, TodaysVisitors= {Actions.TurnToString(TodaysVisitors)}, DurationSoFar = {DurationSoFar}, ControllerTime = {ControllerTime}, VisitorName = {VisitorName}", LogLevel.Debug);
                    }
                    return;
                }
                else
                {
                    if (ModEntry.Config.Verbose)
                    {
                        this.Monitor.Log($"{c.Name} will move around.", LogLevel.Debug);
                    }

                    if (e.NewTime.Equals(TimeOfArrival))
                    {
                        this.Monitor.Log($"Time of arrival equals current time. NPC won't move around", LogLevel.Debug);
                    }
                    else if (ControllerTime >= 1)
                    {
                        c.Halt();
                        c.controller = null;
                        ControllerTime = 0;
                        if (ModEntry.Config.Verbose)
                        {
                            this.Monitor.Log($"ControllerTime = {ControllerTime}", LogLevel.Debug);
                        }
                    }
                    else
                    {
                        if (IsOutside)
                        {
                            FarmOutside.WalkAroundFarm(c.Name);
                            ControllerTime++;
                            DurationSoFar++;
                            this.Monitor.Log($"ControllerTime = {ControllerTime}, DurationSoFar = {DurationSoFar} ({DurationSoFar * 10} minutes).", LogLevel.Debug);

                            return;
                        }

                        //old: c.controller = new PathFindController(c, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random), Random.Next(0, 4));
                        var FarmhouseRandomPoint = Actions.RandomPoint_Farmhouse(farmHouse, Game1.random);

                        c.controller = new PathFindController(c, farmHouse, FarmhouseRandomPoint, Random.Next(0, 4));
                        
                        if (CustomVisiting)
                        {
                            if(ModEntry.Config.Verbose)
                            this.Monitor.Log("Checking if NPC has any custom dialogue...");
                        
                            bool hasCustomDialogue = false;
                            try
                            {
                                var AnyDialogue = currentCustom?[VisitorName]?.Dialogues.Any<string>();
                                hasCustomDialogue = (bool)AnyDialogue;
                            }
                            catch (System.Exception)
                            {
                            }

                            if(hasCustomDialogue)
                            {
                                c.setNewDialogue(currentCustom[VisitorName].Dialogues[0], true, false);

                                this.Monitor.Log($"Adding custom dialogue for {c.Name}...");
                                
                                if (ModEntry.Config.Verbose)
                                    this.Monitor.Log($"C. Dialogue: {currentCustom[VisitorName].Dialogues[0]}", LogLevel.Debug);

                                //remove this dialogue from the queue
                                currentCustom[VisitorName].Dialogues.RemoveAt(0);
                            }
                        }
                        
                        else if (Random.Next(0, 11) <= 5 && FurnitureList.Any())
                        {
                            c.setNewDialogue(
                                string.Format(
                                    Values.GetDialogueType(
                                        c,
                                        DialogueType.Furniture),
                                    Values.GetRandomObj
                                        (ItemType.Furniture)),
                                true,
                                false);

                            if (ModEntry.Config.Verbose)
                            {
                                this.Monitor.Log($"Adding dialogue for {c.Name}...", LogLevel.Debug);
                            }
                        }

                        ControllerTime++;
                        if (ModEntry.Config.Verbose)
                        {
                            this.Monitor.Log($"ControllerTime = {ControllerTime}", LogLevel.Debug);
                        }
                    }

                    DurationSoFar++;
                    if (ModEntry.Config.Verbose)
                    {
                        this.Monitor.Log($"DurationSoFar = {DurationSoFar} ({DurationSoFar * 10} minutes).", LogLevel.Debug);
                    }
                }
            }
        }
        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            FirstLoadedDay = false;

            CleanTempData();

            SchedulesParsed?.Clear();

            if (ModEntry.Config.Verbose)
            {
                this.Monitor.Log("Clearing today's visitor list, visitor count, and all other temp info...", LogLevel.Debug);
            }
        }
        private void TitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            ClearValues();

            this.Monitor.Log($"Removing cached information: HasAnyVisitors= {HasAnyVisitors}, TimeOfArrival={TimeOfArrival}, CounterToday={CounterToday}, VisitorName={VisitorName}. TodaysVisitors, NameAndLevel, FurnitureList, and RepeatedByLV cleared.");
        }
#endregion
        
        #region mod methods
        private void ParseBlacklist()
        {
            this.Monitor.Log("Getting raw blacklist.");
            BlacklistRaw = ModEntry.Config.Blacklist;
            if (BlacklistRaw is null)
            {
                this.Monitor.Log("No characters in blacklist.");
            }

            var charsToRemove = new string[] { "-", ",", ".", ";", "\"", "\'", "/" };
            foreach (var c in charsToRemove)
            {
                BlacklistRaw = BlacklistRaw.Replace(c, string.Empty);
            }
            if (ModEntry.Config.Verbose)
            {
                Monitor.Log($"Raw blacklist: \n {BlacklistRaw} \nWill be parsed to list now.", LogLevel.Debug);
            }
            BlacklistParsed = BlacklistRaw.Split(' ').ToList();
        }
        private void CleanTempData()
        {
            CounterToday = 0;

            VisitorData = null;
            VisitorName = null;

            CustomVisiting = false;
            HasAnyVisitors = false;

            currentCustom?.Clear();
            TodaysVisitors?.Clear();

            if (MaxTimeStay != (ModEntry.Config.Duration - 1))
            {
                MaxTimeStay = (ModEntry.Config.Duration - 1);
                Monitor.Log($"MaxTimeStay = {MaxTimeStay}; Config.Duration = {Config.Duration};");
            }

            Animals?.Clear();
            Crops?.Clear();
            FurnitureList?.Clear();
            FurnitureList = Values.UpdateFurniture(Utility.getHomeOfFarmer(Game1.MasterPlayer));

            if (ModEntry.Config.Verbose)
            {
                Monitor.Log($"Furniture list updated. Count: {FurnitureList?.Count ?? 0}", LogLevel.Debug);
            }
        }
        private void ClearValues()
        {
            InLaws.Clear();
            NameAndLevel?.Clear();
            RepeatedByLV?.Clear();
            TodaysVisitors?.Clear();
            FurnitureList?.Clear();
            currentCustom?.Clear();
            SchedulesParsed?.Clear();
            BlacklistRaw = null;
            BlacklistParsed?.Clear();
            
            if (!string.IsNullOrWhiteSpace(ModEntry.Config.Blacklist))
            {
                ParseBlacklist();
            }

            TimeOfArrival = 0;
            CounterToday = 0;
            DurationSoFar = 0;
            ControllerTime = 0;

            HasAnyVisitors = false;
            CustomVisiting = false;

            VisitorName = null;
            VisitorData = null;
        }
        private void ReloadCustomschedules()
        {
            this.Monitor.Log("Began reloading custom schedules.");

            SchedulesParsed?.Clear();

            var schedules = Game1.content.Load<Dictionary<string, ScheduleData>>("mistyspring.farmhousevisits/Schedules");

            if (schedules.Any())
            {
                foreach (KeyValuePair<string, ScheduleData> pair in schedules)
                {
                    this.Monitor.Log($"Checking {pair.Key}'s schedule...");
                    bool isPatchValid = Extras.IsScheduleValid(pair);

                    if (!isPatchValid)
                    {
                        this.Monitor.Log($"{pair.Key} schedule won't be added.", LogLevel.Error);
                    }
                    else
                    {
                        SchedulesParsed.Add(pair.Key, pair.Value); //NRE
                    }
                }
            }

            HasAnySchedules = SchedulesParsed?.Any() ?? false;

            this.Monitor.Log("Finished reloading custom schedules.");
        }

        //below methods: REQUIRED, dont touch UNLESS it's for bug-fixing
        internal static void ChooseRandom()
        {
            ModEntry.Log("Getting random...",LogLevel.Trace);
            var RChoice = Random.Next(0, (RepeatedByLV.Count));

            VisitorName = RepeatedByLV[RChoice];
            ModEntry.Log($"Random: {RChoice}; VisitorName= {VisitorName}",LogLevel.Trace);

            NPC visit = Game1.getCharacterFromName(VisitorName);

            if (Values.IsFree(visit, true))
            {
                //save values
                VisitorData = new TempNPC(visit);

                CustomVisiting = false;

                DurationSoFar = 0;

                HasAnyVisitors = true;
                TimeOfArrival = Game1.timeOfDay;
                ControllerTime = 0;


                if (ModEntry.Config.NeedsConfirmation)
                {
                    AskToEnter();
                }
                else
                {
                    //add them to farmhouse (last to avoid issues)
                    Actions.AddToFarmHouse(Game1.getCharacterFromName(VisitorName), farmHouse, false);

                    if (ModEntry.Config.Verbose)
                    {
                        ModEntry.Log($"\nHasAnyVisitors set to true.\n{VisitorName} will begin visiting player.\nTimeOfArrival = {TimeOfArrival};\nControllerTime = {ControllerTime};", LogLevel.Debug);
                    }
                }
            }
            else
            {
                VisitorName = null;
            }
        }
        private void GetAllVisitors()
        {
            if (!IsConfigValid)
            {
                return;
            }

            this.Monitor.Log("Began obtaining all visitors.");
            if (!string.IsNullOrWhiteSpace(ModEntry.Config.Blacklist))
            {
                ParseBlacklist();
            }
            NPCNames = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions").Keys.ToList<string>();

            MaxTimeStay = (ModEntry.Config.Duration - 1);
            this.Monitor.Log($"MaxTimeStay = {MaxTimeStay}; Config.Duration = {Config.Duration};");

            farmHouseAsLocation = Utility.getHomeOfFarmer(Game1.MasterPlayer);
            farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);

            foreach (string name in Game1.NPCGiftTastes.Keys)
            {
                if (ModEntry.Config.Debug)
                {
                    this.Monitor.Log($"Name: {name}", LogLevel.Trace);
                }
                if (!name.StartsWith("Universal_") && name is not null)
                {
                    int hearts = Game1.MasterPlayer.getFriendshipHeartLevelForNPC(name);
                    NPC npcn = Game1.getCharacterFromName(name);

                    if (npcn is not null)
                    {
                        var _IsDivorced = Values.IsDivorced(npcn);
                        var _IsMarried = Values.IsMarriedToPlayer(npcn);

                        if (_IsMarried == false && hearts is not 0)
                        {
                            if (BlacklistParsed is not null)
                            {
                                if (BlacklistParsed.Contains(npcn.Name))
                                {
                                    this.Monitor.Log($"{npcn.displayName} is in the blacklist.", LogLevel.Info);
                                }
                                else
                                {
                                    NameAndLevel.Add(name, hearts);
                                }
                            }
                            else if (_IsDivorced)
                            {
                                this.Monitor.Log($"{name} is Divorced.");
                            }
                            else
                            {
                                if (npcn.Name.Equals("Dwarf"))
                                {
                                    if (!Game1.MasterPlayer.canUnderstandDwarves)
                                        this.Monitor.Log("Player can't understand dwarves yet!");

                                    else
                                        NameAndLevel.Add(name, hearts);
                                }
                                else
                                    NameAndLevel.Add(name, hearts);
                            }
                        }
                        else
                        {
                            if (_IsMarried)
                            {
                                MarriedNPCs.Add(npcn.Name);
                                this.Monitor.Log($"Adding {npcn.displayName} (internal name {npcn.Name}) to married list...");
                            }

                            if (_IsDivorced)
                            {
                                this.Monitor.Log($"{name} is Divorced. They won't visit player");
                            }
                            else if (ModEntry.Config.Verbose)
                            {
                                this.Monitor.Log($"{name} won't be added to the visitor list.", LogLevel.Debug);
                            }
                        }
                    }
                    else
                    {
                        this.Monitor.Log($"{name} is not an existing NPC!");
                    }

                }
            }
            string call = "\n Name   | Hearts\n--------------------";
            foreach (KeyValuePair<string, int> pair in NameAndLevel)
            {
                call += $"\n   {pair.Key}   {pair.Value}";

                List<string> tempdict = Enumerable.Repeat(pair.Key, pair.Value).ToList();
                RepeatedByLV.AddRange(tempdict);
            }
            this.Monitor.Log(call);

            FurnitureList?.Clear();
            FurnitureList = Values.UpdateFurniture(farmHouse);
            this.Monitor.Log($"Furniture count: {FurnitureList.Count}");

            this.Monitor.Log("Finished obtaining all visitors.");

            ReloadCustomschedules();
        }

        //if user wants confirmation for NPC to come in
        internal static void AskToEnter()
        {
            //knock on door
            /*
            Game1.currentLocation.playSound("stoneStep", SoundContext.Default);
            System.Threading.Thread.Sleep(300);
            Game1.currentLocation.playSound("stoneStep", SoundContext.Default);
            System.Threading.Thread.Sleep(300);*/

            //get name, place in question string
            var displayName = Game1.getCharacterFromName(VisitorName).displayName;
            var formattedQuestion = string.Format(ModEntry.TL.Get("Visit.AllowOrNot"), displayName);

            var EntryQuestion = new EntryQuestion(formattedQuestion, ResponseList, ActionList);

            //put it on the game
            Game1.activeClickableMenu = EntryQuestion;
        }
        internal void CancelVisit()
        {
            VisitorData = null;
            HasAnyVisitors = false;
            CustomVisiting = false;

            var visit = Game1.getCharacterFromName(VisitorName);

            if (ModEntry.Config.RejectionDialogue)
            {
                //Game1.drawDialogue(visit, Values.GetRejectionResponse(visit));
                Game1.drawDialogue(visit, Values.GetDialogueType(visit, DialogueType.Rejected));
            }

            TodaysVisitors.Add(VisitorName);
            VisitorName = null;
        }
        internal void Proceed()
        {
            if (CustomVisiting)
            {
                Actions.AddCustom(Game1.getCharacterFromName(VisitorName), farmHouse, currentCustom[VisitorName], true);
            }
            else
            {
                Actions.AddToFarmHouse(Game1.getCharacterFromName(VisitorName), farmHouse, true);
            }
        }
        /*  console commands  */
        private void Reload(string command, string[] arg2) => GetAllVisitors();
        public static void SetFromCommand(NPC visit)
        {

            VisitorData = new TempNPC(visit);

            ModEntry.DurationSoFar = 0;

            ModEntry.HasAnyVisitors = true;
            ModEntry.TimeOfArrival = Game1.timeOfDay;
            ModEntry.ControllerTime = 0;

            ModEntry.Log($"\nHasAnyVisitors set to true.\n{VisitorName} will begin visiting player.\nTimeOfArrival = {TimeOfArrival};\nControllerTime = {ControllerTime};", LogLevel.Info);
        }
#endregion

        #region used by mod
        internal static List<string> FurnitureList { get; private set; } = new();
        internal static List<string> Animals { get; private set; } = new();
        internal static Dictionary<int, string> Crops { get; private set; } = new();

        internal static Action<string, LogLevel> Log { get; private set; }
        internal static ITranslationHelper TL { get; private set; }
        internal static TempNPC VisitorData { get; set; }

        internal static Random Random
        {
            get
            {
                random ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed * 36));
                return random;
            }
        }

        internal GameLocation farmHouseAsLocation;
        private bool FirstLoadedDay;
        internal static FarmHouse farmHouse { get; private set; }

        internal static List<Response> ResponseList { get; private set; } = new();
        internal static List<Action> ActionList { get; private set; } = new();
        #endregion
        
        #region player data
        internal static ModConfig Config;
        private static List<string> RepeatedByLV = new();
        private static Random random;
        private static bool isFestivalToday;
        private static bool CanBeVisited;
        internal static List<string> BlacklistParsed { get; private set; } = new();
        #endregion
        #region configurable
        internal static string BlacklistRaw { get; private set; }
        internal static bool CustomVisiting { get; private set; }
        internal bool IsConfigValid { get; private set; }
        internal static bool HasAnyVisitors { get; private set; }
        internal static bool HasAnySchedules { get; private set; }
#endregion
        
        #region visitdata
        internal static int TimeOfArrival { get; private set; }
        internal int CounterToday { get; private set; }
        internal static int DurationSoFar { get; private set; }
        internal int MaxTimeStay { get; private set; }
        internal static int ControllerTime { get; private set; }
        public static bool ForcedSchedule { get; private set; } = false;
#endregion
        #region game information
        internal static Dictionary<string, int> NameAndLevel { get; private set; } = new();
        internal static List<string> NPCNames { get; private set; } = new();
        internal static Dictionary<string, ScheduleData> SchedulesParsed { get; private set; } = new();
        internal static Dictionary<string, ScheduleData> currentCustom { get; private set; } = new();
#endregion
        #region public data
        public static Dictionary<string, List<string>> InLaws { get; private set; } = new();
        public static List<string> MarriedNPCs { get; private set; } = new();
        public static List<string> TodaysVisitors { get; internal set; } = new();
        public static readonly char[] slash = new char['/'];
        public static string VisitorName { get; internal set; }
        public static bool IsOutside { get; internal set; }
#endregion
    }
}