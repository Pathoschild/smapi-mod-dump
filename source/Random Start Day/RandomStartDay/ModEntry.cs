/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Objects;
using System;
using System.Linq;

namespace RandomStartDay
{

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal static ModConfig config;
        internal static IMonitor monitor;
        internal static IModHelper modHelper;

        const int COUNT_OF_SEASONS = 4;
        static string errorString;

        public override void Entry(IModHelper helper)
        {
            config = this.Helper.ReadConfig<ModConfig>();
            monitor = this.Monitor;
            modHelper = helper;
            var harmony = new Harmony(this.ModManifest.UniqueID);

            // randomization methods
            helper.Events.Specialized.LoadStageChanged += Specialized_LoadStageChanged;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            helper.Events.Content.AssetRequested += Content_AssetRequested;
            harmony.Patch(
                original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.createdNewCharacter)),
                prefix: new HarmonyMethod(typeof(Randomize), nameof(Randomize.Harmony_SetDateForIntro))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(Intro), nameof(Intro.createBeginningOfLevel)),
                prefix: new HarmonyMethod(typeof(Randomize), nameof(Randomize.Harmony_ChangeIntroSeason))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addQuest)),
                postfix: new HarmonyMethod(typeof(Randomize), nameof(Randomize.Harmony_Quest6ToWheatQuest))
                );
            Randomize.allowedSeasons = GetAllowedSeasonsFromConfig();

            // fix aftermath methods
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            harmony.Patch(
                original: AccessTools.Method(typeof(TV), "getTodaysTip"),
                postfix: new HarmonyMethod(typeof(FixAftermath), nameof(FixAftermath.Harmony_ChangeTodaysTip))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(TV), "getWeeklyRecipe"),
                postfix: new HarmonyMethod(typeof(FixAftermath), nameof(FixAftermath.Harmony_ChangeCookingChannel))
                );

            // console command
            helper.ConsoleCommands.Add("rsd_set_date", "Change date without changing number of days played. SMAPI console commands do with that.\n" +
                    "Usage: rsd_set_date [-d <day>] [-s <season>] [-y <year>] [-nv]\n-nv: No validation for day of month and year. This may cause errors.", ConsoleCommand_SetDate);
        }

        // EVENTS
        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Randomize.Initialize();
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (config.DisableAll)
                Monitor.Log("This mod is disabled on game launch.", LogLevel.Debug);
            if (errorString != "")
            {
                Monitor.Log(errorString, LogLevel.Error);
                return;
            }

            // GMCM
            var gmcm = modHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm != null)
                AddGMCMOptions(gmcm);
        }

        private void Specialized_LoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (config.DisableAll) { return; }

            // randomize: after character created(Harmony)
            if (e.NewStage == LoadStage.CreatedInitialLocations)
                Randomize.Apply();
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            if (config.DisableAll) { return; }
            FixAftermath.SubtractYearWhenWinter28();
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            FixAftermath.SendTodaysMails();
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // sign and roadside
            if (e.NameWithoutLocale.IsEquivalentTo("Minigames/Intro"))
            {
                if (config.DisableAll) { return; }

                if (Randomize.randomizedSeason != Season.Spring)
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsImage();
                        Texture2D sourceImage = this.Helper.ModContent.Load<Texture2D>("assets/intro_" + Randomize.randomizedSeason.ToString().ToLower() + ".png");

                        editor.PatchImage(sourceImage, targetArea: (new Rectangle(0, 176, 48, 80)), sourceArea: (new Rectangle(0, 0, 48, 80)));
                        editor.PatchImage(sourceImage, targetArea: (new Rectangle(48, 224, 64, 16)), sourceArea: (new Rectangle(48, 48, 64, 16)));
                    });
                }
            }
        }

        // CONSOLE COMMANDS
        private static void ConsoleCommand_SetDate(string command, string[] args)
        {
            // method termination if not the intended command
            if (command != "rsd_set_date")
            {
                string commandErrorString = "Something seems wrong.\nAction executed: SetDate(string command, string[] args)\nConsole command entered: " + command + "\nArguments entered: \n";
                for (int i = 0; i < args.Length; i++)
                    commandErrorString += "args[" + i + "]: " + args[i] + "\n";
                monitor.Log(commandErrorString, LogLevel.Error);
                return;
            }

            bool doValidation = true;
            bool changedDay = false;
            bool changedSeason = false;
            bool changedYear = false;

            int d = 1;
            Season s = Season.Spring;
            int y = 1;
            if (args.Length == 0 || (args.Length == 1 && args[0] == "-nv"))
            {
                monitor.Log("This command requires at least one of the following arguments: day, season, or year.\n" +
                    "If you're not sure what to input, try typing \"help rsd_set_date\" into the console.", LogLevel.Info);
                return;
            }

            if (args.Contains("-nv"))
                doValidation = false;

            for (var i = 0; i < args.Length; i++)
            {
                // dayOfMonth set
                if (args[i] == "-d" && !changedDay)
                {
                    if (args.Length == i + 1) // if "-d"is last arg
                    {
                        monitor.Log("There are no arguments for the day of month to be changed.", LogLevel.Info);
                        return;
                    }
                    else if (!int.TryParse(args[i + 1], out d)) // failed parse arg
                    {
                        monitor.Log("The argument could not be parsed.\nArgument: " + args[i + 1], LogLevel.Info);
                        return;
                    }
                    else
                    {
                        if (doValidation)
                        {
                            if (d < 1 || d > 28)
                            {
                                monitor.Log("Validation failed. Proper range for day of month is 1 ~ 28.\nArgument: " + args[i + 1]
                                    + "\nIf you intended that value, disable validation by adding the \"-nv\" argument to the command.", LogLevel.Info);
                                return;
                            }
                        }
                        changedDay = true;
                    }
                }

                // season set
                if (args[i] == "-s" && !changedSeason)
                {
                    if (args.Length == i + 1) // if "-s"is last arg
                    {
                        monitor.Log("There are no arguments for the season to be changed.", LogLevel.Info);
                        return;
                    }
                    else if (!Enum.TryParse(args[i + 1], true, out s)) // failed parse arg
                    {
                        monitor.Log("The argument could not be parsed.\nArgument: " + args[i + 1], LogLevel.Info);
                        return;
                    }
                    else
                    {
                        changedSeason = true;
                    }
                }

                // year set
                if (args[i] == "-y" && !changedYear)
                {
                    if (args.Length == i + 1) // if "-d"is last arg
                    {
                        monitor.Log("There are no arguments for year to be changed.", LogLevel.Info);
                        return;
                    }
                    else if (!int.TryParse(args[i + 1], out y)) // failed parse arg
                    {
                        monitor.Log("The argument could not be parsed.\nArgument: " + args[i + 1], LogLevel.Info);
                        return;
                    }
                    else
                    {
                        if (doValidation)
                        {
                            if (y < 1)
                            {
                                monitor.Log("Validation failed. Proper value is greater than or equal to 1.\nArgument: " + args[i + 1]
                                    + "\nIf you intended that value, disable validation by adding the \"-nv\" argument to the command.", LogLevel.Info);
                                return;
                            }
                        }

                        changedYear = true;
                    }
                }
            }

            if (!changedDay && !changedSeason && !changedYear)
            {
                monitor.Log("None of the day, season or year has changed.", LogLevel.Info);
                return;
            }
            else
            {
                if (changedDay)
                    Game1.dayOfMonth = d;
                if (changedSeason)
                    Game1.season = s;
                if (changedYear)
                    Game1.year = y;
                monitor.Log("Now is Day " + Game1.dayOfMonth + " of " + Game1.season.ToString() + ", Year " + Game1.year, LogLevel.Info);
            }
        }

        // METHODS
        private static bool Verification(out string errorString)
        {
            bool succeeded = true;
            errorString = "";

            // check if content pack is not found
            if (!modHelper.ModRegistry.IsLoaded("idermailer.RandomStartDayContentPatch"))
            {
                errorString += "- Content Pack for this mod is not found.\n" +
                    "You will probably need to check directory structure, or reinstall this mod.\n";
                succeeded = false;
            }

            return succeeded;
        }

        private static Season[] GetAllowedSeasonsFromConfig()
        {
            Season[] seasonsArray = Array.Empty<Season>();
            errorString = "";

            for (var i = 0; i < Enum.GetValues(typeof(Season)).Length; i++)
            {
                if (config.AllowSpringSummerFallWinter[i])
                    seasonsArray = seasonsArray.AddToArray((Season)i);
            }

            if (seasonsArray.Length == 0)
            {
                monitor.Log("There are no seasons allowed. Mod will consider that setting to allow all four seasons.", LogLevel.Warn);
                return new Season[] { Season.Spring, Season.Summer, Season.Fall, Season.Winter };
            }
            else
            {
                return seasonsArray;
            }
        }

        // GMCM support
        private void AddGMCMOptions(IGenericModConfigMenuApi api)
        {
            api.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config),
                titleScreenOnly: true
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.DisableAll.name"),
                getValue: () => config.DisableAll,
                setValue: value => config.DisableAll = value
            );
            api.AddSectionTitle(mod: ModManifest, text: () => modHelper.Translation.Get("section.AllowedSeasons"));
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.Spring"),
                getValue: () => config.AllowSpringSummerFallWinter[0],
                setValue: value =>
                {
                    config.AllowSpringSummerFallWinter[0] = value;
                    Randomize.allowedSeasons = GetAllowedSeasonsFromConfig();
                }
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.Summer"),
                getValue: () => config.AllowSpringSummerFallWinter[1],
                setValue: value =>
                {
                    config.AllowSpringSummerFallWinter[1] = value;
                    Randomize.allowedSeasons = GetAllowedSeasonsFromConfig();
                }
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.Fall"),
                getValue: () => config.AllowSpringSummerFallWinter[2],
                setValue: value =>
                {
                    config.AllowSpringSummerFallWinter[2] = value;
                    Randomize.allowedSeasons = GetAllowedSeasonsFromConfig();
                }
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.Winter"),
                getValue: () => config.AllowSpringSummerFallWinter[3],
                setValue: value =>
                {
                    config.AllowSpringSummerFallWinter[3] = value;
                    Randomize.allowedSeasons = GetAllowedSeasonsFromConfig();
                }
            );
            api.AddSectionTitle(mod: ModManifest, text: () => modHelper.Translation.Get("section.RandomizeOptions"));
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.AlwaysStartAt1st.name"),
                tooltip: () => modHelper.Translation.Get("config.AlwaysStartAt1st.tooltip"),
                getValue: () => config.AlwaysStartAt1st,
                setValue: value => config.AlwaysStartAt1st = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.AvoidFestivalDay.name"),
                tooltip: () => modHelper.Translation.Get("config.AvoidFestivalDay.tooltip"),
                getValue: () => config.AvoidFestivalDay,
                setValue: value => config.AvoidFestivalDay = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.AvoidPassiveFestivalDay.name"),
                tooltip: () => modHelper.Translation.Get("config.AvoidPassiveFestivalDay.tooltip"),
                getValue: () => config.AvoidPassiveFestivalDay,
                setValue: value => config.AvoidPassiveFestivalDay = value
            );
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.UseWheatSeeds.name"),
                tooltip: () => modHelper.Translation.Get("config.UseWheatSeeds.tooltip"),
                getValue: () => config.UseWheatSeeds,
                setValue: value => config.UseWheatSeeds = value
            );
            api.AddSectionTitle(mod: ModManifest, text: () => modHelper.Translation.Get("section.FixingOptions"));
            api.AddBoolOption(
                mod: ModManifest,
                name: () => modHelper.Translation.Get("config.TVRecipeWithSeasonContext.name"),
                tooltip: () => modHelper.Translation.Get("config.TVRecipeWithSeasonContext.tooltip"),
                getValue: () => config.TVRecipeWithSeasonContext,
                setValue: value => config.TVRecipeWithSeasonContext = value
            );
        }
    }
}