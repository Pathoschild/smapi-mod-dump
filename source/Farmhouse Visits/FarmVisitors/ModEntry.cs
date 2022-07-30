/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/FarmhouseVisits
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

            helper.Events.GameLoop.ReturnedToTitle += this.TitleReturn;
            helper.Events.Content.AssetRequested += Extras.AssetRequest;

            this.Config = this.Helper.ReadConfig<ModConfig>();

            Help = this.Helper;
            Mon = this.Monitor;
            InLawDialogue = Config.InLawComments;
            ReplacerOn = Config.ReplacerCompat;

            if (Config.Debug is true)
            {
                helper.ConsoleCommands.Add("force_visit", "Force a visitor to show up.", this.ForceVisit);
                helper.ConsoleCommands.Add("print_all", "Print all values being used.", this.PrintAll);
                helper.ConsoleCommands.Add("vi_reload", "Reload visitor info.", this.Reload);
                helper.ConsoleCommands.Add("print_inlaws", "Print all in-laws.", this.PrintInLaws);
            }
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var AllowedStringVals = new string[3]
            {
                "VanillaOnly",
                "VanillaAndMod",
                "None"
            };

            //clear values. better safe than sorry
            ClearValues();

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;
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
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.MaxVisitsPerDay.name"),
                tooltip: () => this.Helper.Translation.Get("config.MaxVisitsPerDay.description"),
                getValue: () => this.Config.MaxVisitsPerDay,
                setValue: value => this.Config.MaxVisitsPerDay = value,
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
                getValue: () => this.Config.Duration,
                setValue: value => this.Config.Duration = value,
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
                getValue: () => this.Config.Blacklist,
                setValue: value => this.Config.Blacklist = value,
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
                getValue: () => this.Config.StartingHours,
                setValue: value => this.Config.StartingHours = value,
                min: 600,
                max: 2400,
                interval: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.EndingHours.name"),
                tooltip: () => this.Helper.Translation.Get("config.EndingHours.description"),
                getValue: () => this.Config.EndingHours,
                setValue: value => this.Config.EndingHours = value,
                min: 600,
                max: 2400,
                interval: 100
            );

            //from here on, ALL config is title-only
            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: true
                );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.InLawComments.name"),
                tooltip: () => this.Helper.Translation.Get("config.InLawComments.description"),
                getValue: () => this.Config.InLawComments,
                setValue: value => this.Config.InLawComments = value,
                allowedValues: AllowedStringVals,
                formatAllowedValue: value => this.Helper.Translation.Get($"config.InLawComments.values.{value}")
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ReplacerCompat.name"),
                tooltip: () => this.Helper.Translation.Get("config.ReplacerCompat.description"),
                getValue: () => this.Config.ReplacerCompat,
                setValue: value => this.Config.ReplacerCompat = value
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
                getValue: () => this.Config.Verbose,
                setValue: value => this.Config.Verbose = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.Debug.name"),
                tooltip: () => this.Helper.Translation.Get("config.Debug.Explanation"),
                getValue: () => this.Config.Debug,
                setValue: value => this.Config.Debug = value
            );

            configMenu.SetTitleScreenOnlyForNextOptions(
                mod: this.ModManifest,
                titleScreenOnly: false
                );
        }
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            FirstLoadedDay = true;

            //clear ALL values and temp data on load. this makes sure there's no conflicts with savedata cache (e.g if player had returned to title)
            ClearValues();
            CleanTempData();

            //get all possible visitors- which also checks blacklist and married NPCs, etc.
            GetAllVisitors();

            /* if allowed, get all inlaws. 
             * this doesn't need daily reloading. NPC dispositions don't vary 
             * (WON'T add compat for the very small % of mods with conditional disp., its an expensive action)*/
            if (Config.InLawComments is "VanillaAndMod")
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
            if (!IsConfigValid)
            {
                this.Monitor.Log("Configuration isn't valid. Mod will not work.", LogLevel.Warn);
                CanBeVisited = false;
                return;
            }

            //every sunday, friendship data is reloaded.
            if(Game1.dayOfMonth % 7 == 0 && !FirstLoadedDay)
            {
                NameAndLevel?.Clear();
                RepeatedByLV?.Clear();

                GetAllVisitors();
            }

            /* if no friendship with anyone OR festival day:
             * make unvisitable
             * return (= dont check custom schedule)
             */
            isFestivalToday = Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
            if (!RepeatedByLV.Any() || isFestivalToday)
            {
                CanBeVisited = false;
                return;
            }

            ReloadCustomschedules();
        }
        private void OnTimeChange(object sender, TimeChangedEventArgs e)
        {
            if (!CanBeVisited)
            {
                return;
            }

            if (e.NewTime > Config.StartingHours && e.NewTime < Config.EndingHours && CounterToday < Config.MaxVisitsPerDay)
            {
                if (HasAnyVisitors == false && SchedulesParsed.Any()) //custom
                {
                    foreach (KeyValuePair<string, ScheduleData> pair in SchedulesParsed)
                    {
                        var visitingIsland = Game1.IsVisitingIslandToday(VisitorName);
                        NPC visit = Game1.getCharacterFromName(VisitorName);
                        if (e.NewTime.Equals(pair.Value.From) && visitingIsland == false)
                        {
                            VisitorData = new TempNPC(visit);

                            currentCustom.Add(pair.Key, pair.Value);

                            VisitorName = pair.Key;

                            DurationSoFar = 0;

                            HasAnyVisitors = true;
                            TimeOfArrival = e.NewTime;
                            ControllerTime = 0;
                            CustomVisiting = true;
                            //set last to avoid errors
                            Actions.AddCustom(Game1.getCharacterFromName(VisitorName), farmHouse, currentCustom[VisitorName]);

                            if (Config.Verbose == true)
                            {
                                this.Monitor.Log($"\nHasAnyVisitors set to true.\n{VisitorName} will begin visiting player.\nTimeOfArrival = {e.NewTime};\nControllerTime = {ControllerTime};");
                            }

                            break;
                        }
                    }
                } //custom
                if (HasAnyVisitors == false) //random
                {
                    if (Random.Next(1, 101) <= Config.CustomChance && Game1.currentLocation == farmHouseAsLocation)
                    {
                        ChooseRandom();
                    }
                } //random
            }

            if (HasAnyVisitors == true)
            {
                NPC c = Game1.getCharacterFromName(VisitorName);
                //in the future, add unique dialogue for when characters fall asleep in your house.
                var isVisitSleeping = c.isSleeping.Value;
                if (DurationSoFar >= MaxTimeStay || (CustomVisiting && e.NewTime.Equals(currentCustom[VisitorName].To)) || isVisitSleeping)
                {
                    this.Monitor.Log($"{c.Name} is retiring for the day.");

                    //if custom AND has custom dialogue: exit with custom. else normal
                    if (CustomVisiting == true)
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

                    if (Config.Verbose == true)
                    {
                        this.Monitor.Log($"HasAnyVisitors = false, CounterToday = {CounterToday}, TodaysVisitors= {Actions.TurnToString(TodaysVisitors)}, DurationSoFar = {DurationSoFar}, ControllerTime = {ControllerTime}, VisitorName = {VisitorName}");
                    }
                    return;
                }
                else
                {
                    if (Config.Verbose == true)
                    {
                        this.Monitor.Log($"{c.Name} will move around the house now.");
                    }

                    if(e.NewTime.Equals(TimeOfArrival))
                    {
                        this.Monitor.Log($"Time of arrival equals current time. NPC won't move around");
                    }
                    else if (ControllerTime >= 1)
                    {
                        c.Halt();
                        c.controller = null;
                        ControllerTime = 0;
                        if (Config.Verbose == true)
                        {
                            this.Monitor.Log($"ControllerTime = {ControllerTime}");
                        }
                    }
                    else
                    {
                        c.controller = new PathFindController(c, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random), Random.Next(0, 4));

                        var AnyDialogue = currentCustom?[VisitorName]?.Dialogues.Any<string>();
                        bool hasCustomDialogue = AnyDialogue ?? false;

                        if (CustomVisiting == true && hasCustomDialogue == true)
                        {
                            c.setNewDialogue(currentCustom[VisitorName].Dialogues[0], true, false);

                            this.Monitor.Log($"Adding custom dialogue for {c.Name}...");
                            if (Config.Verbose == true)
                            {
                                this.Monitor.Log($"Custom dialogue: {currentCustom[VisitorName].Dialogues[0]}");
                            }

                            //remove this dialogue from the queue
                            currentCustom[VisitorName].Dialogues.RemoveAt(0);
                        }
                        else if (Random.Next(0, 11) <= 5 && FurnitureList.Any())
                        {
                            c.setNewDialogue(string.Format(Values.TalkAboutFurniture(c), Values.GetRandomFurniture()), true, false);
                            if (Config.Verbose == true)
                            {
                                this.Monitor.Log($"Adding dialogue for {c.Name}...");
                            }
                        }

                        ControllerTime++;
                        if (Config.Verbose == true)
                        {
                            this.Monitor.Log($"ControllerTime = {ControllerTime}");
                        }
                    }

                    DurationSoFar++;
                    if (Config.Verbose == true)
                    {
                        this.Monitor.Log($"DurationSoFar = {DurationSoFar} ({DurationSoFar * 10} minutes).");
                    }
                }
            }
        }
        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            FirstLoadedDay = false;

            CleanTempData();

            SchedulesParsed?.Clear();

            if (Config.Verbose == true)
            {
                this.Monitor.Log("Clearing today's visitor list, visitor count, and all other temp info...");
            }
        }
        private void TitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            ClearValues();

            this.Monitor.Log($"Removing cached information: HasAnyVisitors= {HasAnyVisitors}, TimeOfArrival={TimeOfArrival}, CounterToday={CounterToday}, VisitorName={VisitorName}. TodaysVisitors, NameAndLevel, FurnitureList, and RepeatedByLV cleared.");
        }

        /*  methods used by mod  */
        private void ParseBlacklist()
        {
            this.Monitor.Log("Getting raw blacklist.");
            BlacklistRaw = Config.Blacklist;
            if(BlacklistRaw is null)
            {
                this.Monitor.Log("No characters in blacklist.");
            }

            var charsToRemove = new string[] { "-", ",", ".", ";", "\"", "\'", "/" };
            foreach (var c in charsToRemove)
            {
                BlacklistRaw = BlacklistRaw.Replace(c, string.Empty);
            }
            if (Config.Verbose == true)
            {
                Monitor.Log($"Raw blacklist: \n {BlacklistRaw} \nWill be parsed to list now.");
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

            if (MaxTimeStay != (Config.Duration - 1))
            {
                MaxTimeStay = (Config.Duration - 1);
                Monitor.Log($"MaxTimeStay = {MaxTimeStay}; Config.Duration = {Config.Duration};");
            }

            FurnitureList?.Clear();
            FurnitureList = Values.UpdateFurniture(Utility.getHomeOfFarmer(Game1.MasterPlayer));

            if (Config.Verbose == true)
            {
                Monitor.Log($"Furniture list updated. Count: {FurnitureList?.Count ?? 0}");
            }
        }
        private void ClearValues()
        {
            if (!string.IsNullOrWhiteSpace(Config.Blacklist))
            {
                ParseBlacklist();
            }
            else
            {
                BlacklistRaw = null;
                BlacklistParsed?.Clear();
            }

            TimeOfArrival = 0;
            CounterToday = 0;
            DurationSoFar = 0;
            ControllerTime = 0;

            HasAnyVisitors = false;
            CustomVisiting = false;

            VisitorName = null;
            VisitorData = null;

            InLaws.Clear();
            NameAndLevel?.Clear();
            RepeatedByLV?.Clear();
            TodaysVisitors?.Clear();
            FurnitureList?.Clear();
            currentCustom?.Clear();
            SchedulesParsed?.Clear();
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
                    if (!Extras.IsScheduleValid(pair))
                    {
                        this.Monitor.Log($"{pair.Key} schedule won't be added.", LogLevel.Error);
                    }
                    else
                    {
                        SchedulesParsed.Add(pair.Key, pair.Value);
                    }
                }
            }

            this.Monitor.Log("Finished reloading custom schedules.");
        }
        //below methods: REQUIRED, dont touch UNLESS it's for bug-fixing
        private void ChooseRandom()
        {
            var RChoice = Random.Next(0, (RepeatedByLV.Count));
            VisitorName = RepeatedByLV[RChoice];
            this.Monitor.Log($"Random: {RChoice}; VisitorName= {VisitorName}");
            NPC visit = Game1.getCharacterFromName(VisitorName);

            var isHospitalDay = Utility.IsHospitalVisitDay(VisitorName);
            var visitedToday = TodaysVisitors.Contains(VisitorName);
            var visitingIsland = Game1.IsVisitingIslandToday(VisitorName);
            var isSleeping = visit.isSleeping.Value;

            if (!visitedToday && !isHospitalDay && !visitingIsland && !isSleeping)
            {
                //save values
                VisitorData = new TempNPC(visit);

                CustomVisiting = false;

                DurationSoFar = 0;

                HasAnyVisitors = true;
                TimeOfArrival = Game1.timeOfDay;
                ControllerTime = 0;

                //add them to farmhouse (last to avoid issues)
                Actions.AddToFarmHouse(Game1.getCharacterFromName(VisitorName), farmHouse);

                if (Config.Verbose == true)
                {
                    this.Monitor.Log($"\nHasAnyVisitors set to true.\n{VisitorName} will begin visiting player.\nTimeOfArrival = {TimeOfArrival};\nControllerTime = {ControllerTime};");
                }
            }
            else
            {
                visit = null;
                VisitorName = null;

                if(visitedToday)
                {
                    this.Monitor.Log($"{VisitorName} has already visited the Farm today!");
                }
                if(isHospitalDay)
                {
                    this.Monitor.Log($"{VisitorName} has a hospital visit scheduled today. They won't visit the farmer.");
                }
                if(visitingIsland)
                {
                    this.Monitor.Log($"{VisitorName} is visiting the island today!");
                }
                if(isSleeping)
                {
                    this.Monitor.Log($"{VisitorName} is sleeping right now.");
                }

            }
        }
        private void GetAllVisitors()
        {
            this.Monitor.Log("Began obtaining all visitors.");
            if (!string.IsNullOrWhiteSpace(Config.Blacklist))
            {
                ParseBlacklist();
            }
            NPCNames = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions").Keys.ToList<string>();

            MaxTimeStay = (Config.Duration - 1);
            this.Monitor.Log($"MaxTimeStay = {MaxTimeStay}; Config.Duration = {Config.Duration};");

            farmHouseAsLocation = Utility.getHomeOfFarmer(Game1.MasterPlayer);
            farmHouse = Utility.getHomeOfFarmer(Game1.MasterPlayer);

            if (Config.StartingHours >= Config.EndingHours)
            {
                this.Monitor.Log("Starting hours can't happen after ending hours! To use the mod, fix this and reload savefile.", LogLevel.Error);
                IsConfigValid = false;
                return;
            }
            else
            {
                IsConfigValid = true;
            }

            foreach (string name in Game1.NPCGiftTastes.Keys)
            {
                if (Config.Debug == true)
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
                            else if (_IsDivorced == true)
                            {
                                this.Monitor.Log($"{name} is Divorced.");
                            }
                            else
                            {
                                if (npcn.Name.Equals("Dwarf"))
                                {
                                    if(!Game1.MasterPlayer.canUnderstandDwarves)
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
                            else if (Config.Verbose == true)
                            {
                                this.Monitor.Log($"{name} won't be added to the visitor list.");
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

        /*  console commands  */
        private void Reload(string command, string[] arg2)
        {
            GetAllVisitors();
        }
        private void ForceVisit(string command, string[] arg2)
        {
            if (Context.IsWorldReady)
            {
                if (Game1.MasterPlayer.currentLocation == farmHouse)
                {
                    if (arg2 is null)
                    {
                        ChooseRandom();
                    }
                    else if (NPCNames.Contains(arg2[0]))
                    {
                        VisitorName = arg2[0];
                        this.Monitor.Log($"VisitorName= {VisitorName}");

                        if (!TodaysVisitors.Contains(VisitorName))
                        {
                            //save values
                            NPC visit = Game1.getCharacterFromName(VisitorName);
                            VisitorData = new TempNPC(visit);

                            //add them to farmhouse
                            Actions.AddToFarmHouse(visit, farmHouse);
                            DurationSoFar = 0;

                            HasAnyVisitors = true;
                            TimeOfArrival = Game1.timeOfDay;
                            ControllerTime = 0;

                            this.Monitor.Log($"\nHasAnyVisitors set to true.\n{VisitorName} will begin visiting player.\nTimeOfArrival = {TimeOfArrival};\nControllerTime = {ControllerTime};");
                        }
                        else if (arg2[1] is "force")
                        {
                            //save values
                            NPC visit = Game1.getCharacterFromName(VisitorName);
                            VisitorData = new TempNPC(visit);

                            //add them to farmhouse
                            Actions.AddToFarmHouse(visit, farmHouse);
                            DurationSoFar = 0;

                            HasAnyVisitors = true;
                            TimeOfArrival = Game1.timeOfDay;
                            ControllerTime = 0;

                            this.Monitor.Log($"\nHasAnyVisitors set to true.\n{VisitorName} will begin visiting player.\nTimeOfArrival = {TimeOfArrival};\nControllerTime = {ControllerTime};");
                        }
                        else
                        {
                            VisitorName = null;
                            this.Monitor.Log($"{VisitorName} has already visited the Farm today!");
                        }
                    }
                    else
                    {
                        this.Monitor.Log(Helper.Translation.Get("error.InvalidValue"), LogLevel.Error);
                    }
                }
                else
                {
                    this.Monitor.Log(Helper.Translation.Get("error.NotInFarmhouse"), LogLevel.Error);
                }
            }
            else
            {
                this.Monitor.Log(Helper.Translation.Get("error.WorldNotReady"), LogLevel.Error);
            }
        }
        private void PrintAll(string command, string[] arg2)
        {
            string cc = currentCustom?.Count.ToString() ?? "none";
            string f = VisitorData?.Facing.ToString() ?? "none";
            string pv = VisitorData?.CurrentPreVisit?.Count.ToString() ?? "none";
            string n = VisitorData?.Name ?? "none";
            string am = VisitorData?.AnimationMessage ?? "none";

            this.Monitor.Log($"\n\nVisitorName = {VisitorName ?? "none"}; \nIsConfigValid = {IsConfigValid}; \nHasAnyVisitors = {HasAnyVisitors}; \nTimeOfArrival = {TimeOfArrival}; \nCounterToday = {CounterToday}; \nDurationSoFar = {DurationSoFar}; \nMaxTimeStay = {MaxTimeStay}; \nControllerTime = {ControllerTime},");

            this.Monitor.Log($"\ncurrentCustom count = {cc}; \nVisitorData: \n   Name = {n},\n   Facing = {f}, \n  AnimationMessage = {am}, \n  Dialogues pre-visit: {pv}");
        }
        private void PrintInLaws(string arg1, string[] arg2)
        {
            if(!Context.IsWorldReady)
            {
                this.Monitor.Log(this.Helper.Translation.Get("error.WorldNotReady"),LogLevel.Error);
            }
            else
            {
                string result = "\n";

                foreach (var pair in InLaws)
                {
                    string pairvalue = "";
                    int lastvalue = pair.Value.Count - 1;
                    foreach (string name in pair.Value)
                    {
                        if (pair.Value[lastvalue].Equals(name))
                        {
                            pairvalue += $"{name}.";
                        }
                        else
                        {
                            pairvalue += $"{name}, ";
                        }
                    }

                    result += $"\n{pair.Key}: {pairvalue}";
                }

                if (result.Equals("\n"))
                {
                    this.Monitor.Log("No in-laws found. (Searched all NPCs with friendship)", LogLevel.Warn);
                }
                else
                {
                    this.Monitor.Log(result, LogLevel.Info);
                }
            }
        }

        /*  data  */
        private ModConfig Config;
        private List<string> RepeatedByLV = new();
        private List<string> TodaysVisitors = new();
        private static Random random;
        private bool isFestivalToday;
        private bool CanBeVisited;

        internal static IMonitor Mon { get; private set; }
        internal static IModHelper Help { get; private set; }
        internal static TempNPC VisitorData { get; private set; }

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

        internal FarmHouse farmHouse { get; private set; }

        internal static string BlacklistRaw { get; private set; }
        internal bool CustomVisiting { get; private set; }
        internal bool IsConfigValid { get; private set; }
        internal bool HasAnyVisitors { get; private set; }
        internal int TimeOfArrival { get; private set; }
        internal int CounterToday { get; private set; }
        internal int DurationSoFar { get; private set; }
        internal int MaxTimeStay { get; private set; }
        internal int ControllerTime { get; private set; }
        internal static Dictionary<string, int> NameAndLevel { get; private set; } = new();
        internal static List<string> NPCNames { get; private set; }
        internal static List<string> FurnitureList { get; private set; }
        internal static List<string> BlacklistParsed { get; private set; }
        internal static Dictionary<string, ScheduleData> SchedulesParsed { get; private set; }
        internal static Dictionary<string, ScheduleData> currentCustom { get; private set; }

        //public data
        public static Dictionary<string, List<string>> InLaws { get; private set; } = new();
        public static List<string> MarriedNPCs { get; private set; } = new();
        public static string InLawDialogue { get; private set; }
        public static bool ReplacerOn { get; private set; }
        public static string VisitorName { get; private set; }
    }
}