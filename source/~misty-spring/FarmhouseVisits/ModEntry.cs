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
using System.Net;
using FarmVisitors.Datamodels;
using FarmVisitors.Models;
using FarmVisitors.Visit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Pathfinding;

// ReSharper disable InconsistentNaming

namespace FarmVisitors;

// ReSharper disable once ClassNeverInstantiated.Global
public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += GameLaunched;
        helper.Events.GameLoop.SaveLoaded += Events.SaveLoaded;

        helper.Events.GameLoop.DayStarted += DayStarted;
        helper.Events.GameLoop.TimeChanged += OnTimeChange;
        helper.Events.GameLoop.DayEnding += Events.DayEnding;

        helper.Events.Player.Warped += Events.PlayerWarp;

        helper.Events.GameLoop.ReturnedToTitle += Events.TitleReturn;
        helper.Events.Content.AssetRequested += Events.AssetRequest;
        helper.Events.Content.AssetsInvalidated += Events.AssetInvalidated;

        Config = Helper.ReadConfig<ModConfig>();

        Help = this.Helper;
        Logger = Monitor.Log;
        TL = Helper.Translation;

        var harmony = new Harmony(ModManifest.UniqueID);
        CharacterPatches.Apply(harmony);

        if (!Config.Debug) return;

        helper.ConsoleCommands.Add("print", "List the values requested.", Debugging.Print);
        helper.ConsoleCommands.Add("vi_reload", "Reload visitor info.", Debugging.Reload);
        helper.ConsoleCommands.Add("vi_force", "Force a visit to happen.", Debugging.ForceVisit);
    }

    private void GameLaunched(object sender, GameLaunchedEventArgs e)
    {
        if (Config.Debug)
        {
            Monitor.Log("Debug has been turned on. This will change configuration for testing purposes.", LogLevel.Warn);

            Monitor.Log("Chance set to 100 (% every 10 min)");
            Config.CustomChance = 100;
            Monitor.Log("Starting hour will be 600.");
            Config.StartingHours = 600;
        }

        var allowedStringVals = new[]
        {
            "VanillaOnly",
            "VanillaAndMod",
            "None"
        };

        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        #region config
        #region main
        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );

        // basic config options
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
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.GiftChance.name"),
            tooltip: () => Helper.Translation.Get("config.GiftChance.description"),
            getValue: () => Config.GiftChance,
            setValue: value => Config.GiftChance = value,
            min: 0,
            max: 100,
            interval: 1
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.MaxVisitsPerDay.name"),
            tooltip: () => Helper.Translation.Get("config.MaxVisitsPerDay.description"),
            getValue: () => Config.MaxVisitsPerDay,
            setValue: value => Config.MaxVisitsPerDay = value,
            min: 0,
            max: 24,
            interval: 1
        );
        configMenu.SetTitleScreenOnlyForNextOptions(
            mod: ModManifest,
            titleScreenOnly: true
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.VisitDuration.name"),
            tooltip: () => Helper.Translation.Get("config.VisitDuration.description"),
            getValue: () => Config.Duration,
            setValue: value => Config.Duration = value,
            min: 1,
            max: 20,
            interval: 1
        );
        configMenu.SetTitleScreenOnlyForNextOptions(
            mod: ModManifest,
            titleScreenOnly: false
        );
        //extra customization
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Extras",
            text: () => TL.Get("config.Extras")
        );

        //extra customization
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Places",
            text: () => TL.Get("config.Places.name")
        );

        //extra customization
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Sleepovers",
            text: () => TL.Get("config.Sleepovers")
        );

        //developer config
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Debug",
            text: () => TL.Get("config.Debug.name")
        );
        #endregion

        #region extras
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Extras",
            pageTitle: () => TL.Get("config.Extras")
        );

        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => TL.Get("config.VisitConfiguration"),
            tooltip: null);

        configMenu.SetTitleScreenOnlyForNextOptions(
            mod: ModManifest,
            titleScreenOnly: true
        );
        configMenu.AddTextOption(
            mod: ModManifest,
            getValue: () => Config.Blacklist,
            setValue: value => Config.Blacklist = value,
            name: () => TL.Get("config.Blacklist.name"),
            tooltip: () => TL.Get("config.Blacklist.description")
        );
        configMenu.SetTitleScreenOnlyForNextOptions(
            mod: ModManifest,
            titleScreenOnly: false
            );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.StartingHours.name"),
            tooltip: () => Helper.Translation.Get("config.StartingHours.description"),
            getValue: () => Config.StartingHours,
            setValue: value => Config.StartingHours = value,
            min: 600,
            max: 2400,
            interval: 100
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.EndingHours.name"),
            tooltip: () => Helper.Translation.Get("config.EndingHours.description"),
            getValue: () => Config.EndingHours,
            setValue: value => Config.EndingHours = value,
            min: 600,
            max: 2400,
            interval: 100
        );

        //from here on, ALL config is title-only
        configMenu.SetTitleScreenOnlyForNextOptions(
            mod: ModManifest,
            titleScreenOnly: true
            );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.UniqueDialogue.name"),
            tooltip: () => Helper.Translation.Get("config.UniqueDialogue.description"),
            getValue: () => Config.UniqueDialogue,
            setValue: value => Config.UniqueDialogue = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.AskForConfirmation.name"),
            tooltip: () => Helper.Translation.Get("config.AskForConfirmation.description"),
            getValue: () => Config.NeedsConfirmation,
            setValue: value => Config.NeedsConfirmation = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.RejectionDialogues.name"),
            tooltip: () => Helper.Translation.Get("config.RejectionDialogues.description"),
            getValue: () => Config.RejectionDialogue,
            setValue: value => Config.RejectionDialogue = value
        );
        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.InLawComments.name"),
            tooltip: () => Helper.Translation.Get("config.InLawComments.description"),
            getValue: () => Config.InLawComments,
            setValue: value => Config.InLawComments = value,
            allowedValues: allowedStringVals,
            formatAllowedValue: value => Helper.Translation.Get($"config.InLawComments.values.{value}")
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.ReplacerCompat.name"),
            tooltip: () => Helper.Translation.Get("config.ReplacerCompat.description"),
            getValue: () => Config.ReplacerCompat,
            setValue: value => Config.ReplacerCompat = value
        );
        #endregion

        #region places
        configMenu.AddPage(
            ModManifest,
            "Places",
            () => TL.Get("config.Places.name")
            );

        configMenu.AddParagraph(
            ModManifest,
            () => Helper.Translation.Get("config.Places.description")
            );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.WalkOnFarm.name"),
            tooltip: () => Helper.Translation.Get("config.WalkOnFarm.description"),
            getValue: () => Config.WalkOnFarm,
            setValue: value => Config.WalkOnFarm = value
        );
        /*
        configMenu.AddBoolOption(
            ModManifest,
            getValue: () => Config.AnimalHomes,
            setValue: value => Config.AnimalHomes = value,
            name: () => Data.AnimalBuildingsTitle()
            );*/

        configMenu.AddBoolOption(
            ModManifest,
            getValue: () => Config.Greenhouse,
            setValue: value => Config.Greenhouse = value,
            name: () => Game1.content.LoadString("Strings/Buildings:Greenhouse_Name")
            );

        configMenu.AddBoolOption(
            ModManifest,
            getValue: () => Config.Shed,
            setValue: value => Config.Shed = value,
            name: () => Game1.content.LoadString("Strings/Buildings:Shed_Name")
            );
        #endregion

        #region debug
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Debug",
            pageTitle: () => TL.Get("config.Debug.name")
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Debug.name"),
            tooltip: () => Helper.Translation.Get("config.Debug.Explanation"),
            getValue: () => Config.Debug,
            setValue: value => Config.Debug = value
        );
        configMenu.SetTitleScreenOnlyForNextOptions(
            mod: ModManifest,
            titleScreenOnly: false
            );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.Verbose.name"),
            tooltip: () => Helper.Translation.Get("config.Verbose.description"),
            getValue: () => Config.Verbose,
            setValue: value => Config.Verbose = value
        );
        #endregion

        #region sleepovers
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Sleepovers",
            pageTitle: () => TL.Get("config.Sleepovers")
            );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.SleepoverEnabled.name"),
            tooltip: () => Helper.Translation.Get("config.SleepoverEnabled.description"),
            getValue: () => Config.Sleepover,
            setValue: value => Config.Sleepover = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.SleepoverChance.name"),
            tooltip: () => Helper.Translation.Get("config.SleepoverChance.Explanation"),
            getValue: () => Config.SleepoverChance,
            setValue: value => Config.SleepoverChance = value,
            min: 0,
            max: 100,
            interval: 1
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.SleepoverMinHearts.name"),
            tooltip: () => Helper.Translation.Get("config.SleepoverMinHearts.Explanation"),
            getValue: () => Config.SleepoverMinHearts,
            setValue: value => Config.SleepoverMinHearts = value,
            min: 0,
            max: 12,
            interval: 1
        );
        #endregion
        #endregion
    }

    private void DayStarted(object sender, DayStartedEventArgs e)
    {
        PlayerHome = Utility.getHomeOfFarmer(Game1.player);
        TodaysVisitors = new();

        //if faulty config, don't do anything + mark as unvisitable
        if (!IsConfigValid)
        {
            Monitor.Log("Configuration isn't valid. Mod will not work.", LogLevel.Warn);
            CanBeVisited = false;
            return;
        }

        //friendship data is reloaded.
        if (!_firstLoadedDay)
        {
            Monitor.Log("Reloading data...");
            NameAndLevel?.Clear();
            RepeatedByLV?.Clear();

            Content.GetAllVisitors();
        }

        #region festival check
        /* if no friendship with anyone OR festival day:
         * make unvisitable & return
         */
        FestivalToday = Utility.isFestivalDay(Game1.dayOfMonth, Game1.season);
        var anyInLv = RepeatedByLV?.Any() ?? false;
        Monitor.Log($"isFestivalToday = {FestivalToday}; anyInLV = {anyInLv}");

        if (!anyInLv || FestivalToday)
        {
            CanBeVisited = false;
            return;
        }
        #endregion

        CanBeVisited = true;

        //on winter, remove crop/animal data
        if (Game1.currentSeason == "winter")
        {
            Crops?.Clear();
            Animals?.Clear();
        }
        else
        {
            //if(Game1.player.mailReceived.Contains("ccPantry"))
            GreenhouseCrops = Values.GetCropsNameOnly(Utility.fuzzyLocationSearch("Greenhouse"));
            Crops = Values.GetCropsNameOnly(Game1.getFarm());
            Animals = Values.GetAnimals();
        }
    }

    private void OnTimeChange(object sender, TimeChangedEventArgs e)
    {
        if (!CanBeVisited)
            return;

        //on 610, fix children data
        if (e.OldTime == 600 || e.NewTime == 610)
        {
            Content.FixChildrenInfo();
            return;
        }

        var visitsOpen = e.NewTime > Config.StartingHours && e.NewTime < Config.EndingHours;
        var hasReachedMax = CounterToday >= Config.MaxVisitsPerDay;

        //if can visit & hasn't reached MAX
        if (visitsOpen && !hasReachedMax && !HasAnyVisitors)
        {
            var choseOne = false;

            //prioritize scheduled
            if (HasCustomSchedules)
            {
                foreach (var pair in SchedulesParsed)
                {
                    var data = pair.Value;

                    //if GSQ and doesn't apply
                    var hasGSQ = !string.IsNullOrWhiteSpace(data.Extras?.GameStateQuery);
                    if (hasGSQ)
                    {
                        var GSQmatch = GameStateQuery.CheckConditions(data.Extras.GameStateQuery);

                        if (!GSQmatch)
                        {
                            Log("GSQ conditions don't match. Character will be skipped", LogLevel.Debug);
                            continue;
                        }
                    }

                    var visit = Game1.getCharacterFromName(pair.Key);

                    //whether npc is free or is forced schedule
                    var canVisit = Values.IsFree(visit, false) || data.Extras.Force;

                    //if must be exact, checks that time matches. if not, checks if you're in time range.
                    var inTimeRange = data.MustBeExact ? e.NewTime.Equals(data.From) : e.NewTime >= data.From && e.NewTime < (data.To - 10);

                    //if it's not starting time OR they're not free
                    if (!inTimeRange || !canVisit)
                        continue;

                    //duplicate NPC, and set visiting
                    VContext = new VisitData(visit, true, data);
                    Visitor = DupeNPC.Duplicate(visit);
                    HasAnyVisitors = true;

                    if (data.Extras.Force)
                    {
                        Monitor.Log($"Adding NPC {Visitor.Name} by force (Force.Enable = {data.Extras.Force})");

                        //add and set forced
                        Actions.AddWhileOutside(Visitor);
                        ForcedSchedule = true; //avoids certain behavior
                        return;
                    }

                    if (Config.NeedsConfirmation)
                    {
                        Confirmation.AskToEnter();
                    }
                    else
                    {
                        //add them to farmhouse (last to avoid issues)
                        Actions.AddCustom(Visitor, PlayerHome, data, false);

                        Monitor.Log($"HasAnyVisitors set to true.\n{Visitor.Name} will begin visiting player.\nTimeOfArrival = {VContext.TimeOfArrival};\nControllerTime = {VContext.ControllerTime};", Level);
                    }

                    //break, since we already found a schedule
                    choseOne = true;
                    break;
                }
            }
            //if none matched, do random
            if (!choseOne)
            {
                var inFarmhouse = Game1.player.currentLocation.Equals(PlayerHome);
                var chanceMatch = Random.Next(1, 101) <= Config.CustomChance;

                if (chanceMatch && inFarmhouse)
                    Content.ChooseRandom();
            }
            return;
        }


        //if they're going to sleep, return
        if (Visitor == null || (bool)VContext?.IsGoingToSleep)
            return;

        //in the future, add dialogue for when characters fall asleep.
        var soonToSleep = Values.IsCloseToSleepTime(VContext);

        //if custom visit and reached max time
        var maxCustomTime = VContext.CustomVisiting && e.NewTime >= VContext.CustomData?.To;

        //if npc has stayed too long, check how to retire
        if (VContext.DurationSoFar >= MaxTimeStay || maxCustomTime || soonToSleep)
        {
            //sleepover bool checks: soon to sleep, enabled, % match, min hearts OK
            var shouldSleepOver = soonToSleep && Config.Sleepover && Game1.random.Next(0, 100) <= Config.SleepoverChance && Config.SleepoverMinHearts <= NameAndLevel[Visitor.Name];

            //log, LV depends on config
            var action = shouldSleepOver ? "resting" : "retiring";
            Log($"{Visitor.Name} is {action} for the day.", Level);

            //update info
            //ModEntry.Visitors?.Remove(Name);
            CounterToday++;
            TodaysVisitors.Add(Visitor.Name);

            //get data before we remove it
            var durationSoFar = VContext.DurationSoFar;
            var controllerTime = VContext.ControllerTime;
            ForcedSchedule = false;

            if (Config.Debug)
            {
                Log($"HasAnyVisitors = false, CounterToday = {CounterToday}, TodaysVisitors= {Data.TurnToString(TodaysVisitors)}, DurationSoFar = {durationSoFar}, ControllerTime = {controllerTime}, VisitorName = {Visitor?.Name}", Level);
            }

            //if custom has dialogue: exit with it. else, normal
            if (VContext.CustomVisiting)
            {
                //if there's a mail string, add for tomorrow
                var mail = VContext.CustomData.Extras.Mail;
                if (!string.IsNullOrWhiteSpace(mail))
                {
                    Game1.player.mailForTomorrow.Add(mail);
                }

                var exitd = VContext.CustomData?.ExitDialogue;
                if (!string.IsNullOrWhiteSpace(exitd))
                    Actions.RetireCustom(Visitor, exitd);
                else
                    Actions.Retire(Visitor);
            }
            else if (shouldSleepOver)
            {
                Actions.GoToSleep(Visitor, VContext);
            }
            else
            {
                Actions.Retire(Visitor);
            }

            return;
        }

        //otherwise, they'll try to move around.
        Log($"{Visitor.Name} will move around.", Level);

        if (Config.Verbose)
        {
            var endPoint = Visitor.controller?.pathToEndPoint;
            if (endPoint != null)
                Log($"Current endpoint: {Data.TurnToString(endPoint)},moving: {Visitor.isMovingOnPathFindPath}", LogLevel.Debug);
        }

        //if they just arrived they won't move.
        if (e.NewTime == VContext.TimeOfArrival)
        {
            Log($"Time of arrival equals current time. NPC won't move around", Level);
        }
        //if they've been moving too long, they'll stop
        else if (VContext.ControllerTime != 0)
        {
            VContext.ControllerTime = 0;
            Visitor.Halt();
            Visitor.temporaryController = null;
            Visitor.controller = null;
            if (Config.Debug)
            {
                Log($"ControllerTime = {VContext.ControllerTime}", Level);
            }
            /*
            var isAnimating = Visitor.Sprite.CurrentAnimation?.Count > 0;

            if (Visitor.isMoving() || Visitor.isMovingOnPathFindPath.Value || Visitor.IsRemoteMoving() || isAnimating)
            {
                Visitor.Halt();
                Visitor.temporaryController = null;
                Visitor.controller = null;
                if (Config.Debug)
                {
                    Log($"ControllerTime = {VisitContext.ControllerTime}", Level);
                }
            }
            else
            {
                var newtile = Data.RandomSpotInSquare(Visitor, 2);
                Visitor.controller = new PathFindController(Visitor, Visitor.currentLocation, newtile, Game1.random.Next(0, 4));
                Visitor.returnToEndPoint();
            }*/
        }
        //otherwise, will try moving.
        else
        {
            Visitor.resetCurrentDialogue();
            //DurationSoFar++;
            VContext.ControllerTime++;
            //temporaryController = null;
            //var isBarnOrCoop = Visitor.currentLocation.Name.Contains("Coop") || Visitor.currentLocation.Name.Contains("Barn");
            var newtile = Point.Zero;

            //walk on farm OR house
            if (Visitor.currentLocation.Name == "Farm")
            {
                if (Config.Verbose)
                    Log("Current is farm.");

                //Visitor.Idle = true;
                newtile = Data.RandomSpotInSquare(Visitor, 10);
                Visitor.controller = new PathFindController(
                    Visitor,
                    Visitor.currentLocation,
                    newtile,
                    Game1.random.Next(0, 4)
                )
                {
                    endPoint = newtile
                };
            }
            else if (Visitor.currentLocation is FarmHouse)
            {
                if (Config.Verbose)
                    Log("Current is farmhouse.");

                newtile = (Visitor.currentLocation as FarmHouse).getRandomOpenPointInHouse(Game1.random);
                Visitor.controller = new PathFindController(
                    Visitor,
                    Visitor.currentLocation,
                    newtile,
                    Game1.random.Next(0, 4)
                );
            }
            else
            {
                if (Config.Verbose)
                    Log("Current is probably a shed or greenhouse.");

                newtile = Data.RandomTile(Visitor.currentLocation, Visitor).ToPoint();

                //stop JIC
                Visitor.Halt();
                Visitor.temporaryController = null;
                Visitor.controller = null;

                Visitor.controller = new PathFindController(
                    Visitor,
                    Visitor.currentLocation,
                    newtile,
                    Game1.random.Next(0, 4)
                )
                {
                    endPoint = newtile
                };
            }

            if (Config.Debug)
                Log($"New position: {newtile}, pathing to {Visitor.controller.endPoint}", LogLevel.Debug);

            if (VContext.CustomVisiting)
            {
                Log("Checking if NPC has any custom dialogue...", Level);

                var hasCustomDialogue = VContext.CustomData?.Dialogues?.Any() ?? false;
                if (hasCustomDialogue)
                {
                    Actions.SetDialogue(Visitor, VContext.CustomData.Dialogues[0]);

                    Log($"Adding custom dialogue for {Visitor.Name}...");

                    if (Config.Debug)
                        Log($"({Visitor.Name}) C. Dialogue: {VContext.CustomData.Dialogues[0]}", Level);

                    //remove this dialogue from the queue
                    VContext.CustomData.Dialogues.RemoveAt(0);
                }
            }
        }

        CheckForDialogue();

        VContext.DurationSoFar++;
        if (Config.Debug)
            Log($"ControllerTime = {VContext.ControllerTime}, DurationSoFar = {VContext.DurationSoFar} ({VContext.DurationSoFar * 10} minutes).", Level);
    }

    private static void CheckForDialogue()
    {
        //if custom, check for dialogue
        if (VContext.CustomVisiting)
        {
            Log("Checking if NPC has any custom dialogue...", Level);

            var hasCustomDialogue = VContext.CustomData?.Dialogues?.Any() ?? false;
            if (hasCustomDialogue)
            {
                Actions.SetDialogue(Visitor, VContext.CustomData.Dialogues[0]);

                Log($"Adding custom dialogue for {Visitor.Name}...");

                if (Config.Debug)
                    Log($"C. Dialogue: {VContext.CustomData.Dialogues[0]}", Level);

                //remove this dialogue from the queue
                VContext.CustomData.Dialogues.RemoveAt(0);
            }
        }

        //otherwise, check % for random dialogue
        else if (Game1.random.Next(0, 11) > 5)
            return;

        var isFarm = Visitor.currentLocation.Name == "Farm";
        var furniture = Visitor.currentLocation.furniture;
        var isFarmhouse = Visitor.currentLocation is FarmHouse;
        var isShed = Visitor.currentLocation is Shed;
        //if in farm
        if (isFarm)
        {
            var anyCrops = Crops.Any();

            if (Game1.currentSeason == "winter")
            {
                Actions.SetDialogue(Visitor, Values.GetDialogueType(Visitor, DialogueType.Winter));
            }
            else if ((Game1.random.Next(0, 2) <= 0 || !anyCrops) && Animals.Any())
            {
                var animal = Game1.random.ChooseFrom(Animals);
                var rawtext = Values.GetDialogueType(Visitor, DialogueType.Animal);
                var formatted = string.Format(rawtext, animal);
                Actions.SetDialogue(Visitor, formatted);
            }
            else if (anyCrops)
            {
                var crop = Game1.random.ChooseFrom(Crops);
                var rawtext = Values.GetDialogueType(Visitor, DialogueType.Crop);
                var formatted = string.Format(rawtext, crop);
                Actions.SetDialogue(Visitor, formatted);
            }
            else
            {
                Actions.SetDialogue(Visitor, Values.GetDialogueType(Visitor, DialogueType.NoneYet));
            }
        }
        //if in shed/house and any furniture
        else if ((isFarmhouse || isShed) && furniture.Any())
        {
            var text = Values.GetDialogueType(Visitor, DialogueType.Furniture);
            var formatted = string.Format(text, Game1.random.ChooseFrom(furniture).DisplayName);
            Actions.SetDialogue(Visitor, formatted);

            if (Config.Debug)
                Log($"Adding dialogue for {Visitor.Name}...", Level);
        }
        else
        {
            var isCoopOrBarn = Visitor.currentLocation.Name.Contains("Coop") || Visitor.currentLocation.Name.Contains("Barn");
            var isGreenHouse = Visitor.currentLocation.Name == "Greenhouse";

            if (isGreenHouse)
            {
                var crops = GreenhouseCrops;
                if (crops == null || crops.Count == 0)
                    return;

                var chosen = Game1.random.ChooseFrom(crops);
                var text = Values.GetDialogueType(Visitor, DialogueType.Crop);
                var formatted = string.Format(text, chosen);
                Actions.SetDialogue(Visitor, formatted);
                if (Config.Debug)
                    Log($"Adding dialogue for {Visitor.Name}...", Level);
            }
            else if (isCoopOrBarn)
            {
                var animals = Visitor.currentLocation.getAllFarmAnimals();

                if (animals == null || animals.Count == 0)
                    return;

                var chosen = Game1.random.ChooseFrom(animals);
                var text = Values.GetDialogueType(Visitor, DialogueType.Animal);
                var formatted = string.Format(text, chosen.displayName);
                Actions.SetDialogue(Visitor, formatted);
                if (Config.Debug)
                    Log($"Adding dialogue for {Visitor.Name}...", Level);
            }
        }
    }

    /// <summary>
    /// Set all used visitor variables to null.
    /// </summary>
    //its here because i'd rather have a single call than repeat through code and forget one of them
    internal static void SetNoVisitor()
    {
        HasAnyVisitors = false;
        Visitor = null;
        VContext = null;
    }

    #region used by visitors
    internal static Dictionary<string, List<string>> RetiringDialogue { get; set; } = new();
    internal static Dictionary<string, List<string>> InlawDialogue { get; set; } = new();
    //internal static List<string> FurnitureList { get; set; } = new();
    internal static List<string> Animals { get; private set; } = new();
    internal static List<string> Crops { get; private set; } = new();
    internal static List<string> GreenhouseCrops { get; private set; } = new();
    //internal static Character Puppet { get; set; }
    #endregion

    #region used by mod
    internal static LogLevel Level => Config.Verbose ? LogLevel.Debug : LogLevel.Trace;
    internal static Action<string, LogLevel> Logger { get; private set; }
    internal static void Log(string data, LogLevel type = LogLevel.Trace) => Logger(data, type);

    internal static ITranslationHelper TL { get; private set; }
    internal static IModHelper Help { get; set; }

    private static Random Random
    {
        get
        {
            random ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed * 36));
            return random;
        }
    }
    private static Random random;
    #endregion

    #region player data
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    internal static Dictionary<string, List<string>> InLaws { get; private set; } = new();
    internal static Dictionary<string, int> NameAndLevel { get; private set; } = new();
    internal static List<string> RepeatedByLV = new();
    internal static List<string> BlacklistParsed { get; set; } = new();
    internal static FarmHouse PlayerHome { get; set; }
    internal static bool _firstLoadedDay;
    private static bool CanBeVisited;
    internal static ModConfig Config;
    #endregion

    #region configurable
    internal static string BlacklistRaw { get; set; }
    internal static bool IsConfigValid { get; set; }
    internal static bool HasAnyVisitors { get; set; }
    internal static bool HasCustomSchedules { get; set; }
    #endregion

    #region visitdata
    internal static NPC Visitor { get; set; }
    internal static VisitData VContext { get; set; }
    internal static int MaxTimeStay { get; set; }
    internal static int CounterToday { get; set; }
    public static bool ForcedSchedule { get; internal set; }
    internal static List<string> TodaysVisitors { get; set; } = new();
    #endregion

    #region game information
    private static bool FestivalToday;
    internal static Dictionary<string, ScheduleData> SchedulesParsed { get; set; } = new();
    internal static List<string> MarriedNPCs { get; private set; } = new();
    #endregion
}