/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using IncreaseAnimalHouseMaxPopulation.Framework;
using IncreaseAnimalHouseMaxPopulation.Framework.Configs;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;

namespace IncreaseAnimalHouseMaxPopulation
{
    public class ModEntry : Mod
    {
        public ModConfig Config;

        private PlayerData _data;

        public SButton RefreshConfig;

        public ITranslationHelper I18N;

        public Building CurrentHoveredBuilding;

        public Building CurrentHoveredBuildingDummy;

        public List<string> AnimalHouseBuildings = new List<string>
        {
            "Deluxe Barn",
            "Deluxe Coop"
        };

        public int Cost;

        public bool IsTestBuild = false;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            I18N = helper.Translation;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.Display.RenderingHud += RenderingHud;
            helper.ConsoleCommands.Add("pop_reset", "Deletes the save data for Increased Animal House Population.",
                ResetSave);
            DoSanityCheck();
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {

        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (e.IsDown(RefreshConfig))
            {
                Config = Helper.ReadConfig<ModConfig>();
                DoSanityCheck();
                Monitor.Log("Reloaded the configuration file.", LogLevel.Debug);
            }

            if (e.IsDown(SButton.F6) && IsTestBuild)
            {
                DoPopChange(Config.MaxBarnPopulation, Config.MaxCoopPopulation);
            }

            if (!e.IsDown(SButton.MouseLeft) || CurrentHoveredBuilding == null ||
                !AnimalHouseBuildings.Contains(CurrentHoveredBuilding.buildingType.Value) ||
                /*
                this.CurrentHoveredBuilding.buildingType.Value.Contains("Shipping Bin") ||
                this.CurrentHoveredBuilding.buildingType.Value.Contains("Cabin") ||
                this.CurrentHoveredBuilding.buildingType.Value.Contains("Silo") ||
                this.CurrentHoveredBuilding.buildingType.Value.Contains("Mill") ||*/
                _data.Buildings.ContainsKey(CurrentHoveredBuilding.indoors.Value.uniqueName.Value) && _data.Buildings != null ||
                Game1.activeClickableMenu != null)
            {
                return;
            }

            Vector2 tLocation = GetCursorLocation();
            if (!AnimalHouseBuildings.Any(ab =>
                CurrentHoveredBuilding.buildingType.Contains(ab) &&
                CurrentHoveredBuilding.indoors.Value != null) || CurrentHoveredBuilding == null)
            {
                return;
            }

            int freeOrNot = ((!Config.Cheats.EnableFree) ? Config.CostPerPopulationIncrease : 0);
            //Lets calculate the difference between max and current max population
            int currentMaxOccupants = ((AnimalHouse) CurrentHoveredBuilding.indoors.Value).animalLimit.Value; //(AnimalHouse)this.CurrentHoveredBuilding.indoors
            Cost = (CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn")
                ? ((Config.MaxBarnPopulation - currentMaxOccupants)* freeOrNot)
                : ((Config.MaxCoopPopulation - currentMaxOccupants) * freeOrNot));
            
            CurrentHoveredBuildingDummy = CurrentHoveredBuilding;
            string question = I18N.Get("upgrade_question", new
            {
                current_building = CurrentHoveredBuilding.buildingType.Value,
                next_cost = Cost,
                current_max_occupants = currentMaxOccupants,
                config_max_occupants = CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn") ? Config.MaxBarnPopulation : Config.MaxCoopPopulation
            });
            Game1.getFarm().createQuestionDialogue(question, Game1.getFarm().createYesNoResponses(),
                delegate(Farmer _, string answer)
                {
                    if (answer == "Yes")
                    {
                        if (Game1.player.Money >= Cost)
                        {
                            Game1.player.Money -= Cost;
                            DoPopChange(CurrentHoveredBuildingDummy);
                        }
                        else
                        {
                            Game1.showRedMessage($"You don't have {Cost} gold.");
                        }
                    }
                });
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            Helper.Data.WriteSaveData(Helper.ModRegistry.ModID, _data);
        }

        private void RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (CurrentHoveredBuilding != null && Game1.activeClickableMenu == null &&
                (Config.EnableDebugMode || Config.EnableHoverTip) && AnimalHouseBuildings.Any(
                    ab => CurrentHoveredBuilding.buildingType.Contains(ab) &&
                            CurrentHoveredBuilding.indoors.Value != null))
            {
                Translation tipText = I18N.Get("upgrade_tooltip_text", new
                {
                    current_building = CurrentHoveredBuilding.buildingType.Value,
                    max_animals = ((AnimalHouse) CurrentHoveredBuilding.indoors.Value).animalLimit
                });
                IClickableMenu.drawHoverText(Game1.spriteBatch, tipText, Game1.smallFont);
            }

            if (CurrentHoveredBuilding != null)
            {
                int p = (CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn")
                    ? Config.MaxBarnPopulation
                    : Config.MaxCoopPopulation);
                AnimalHouse obj = CurrentHoveredBuilding.indoors.Value as AnimalHouse;
                if ((obj == null || obj.animalLimit.Value != p) &&
                    CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe") &&
                    !CurrentHoveredBuilding.buildingType.Value.Contains("Cabin") &&
                    !CurrentHoveredBuilding.buildingType.Value.Contains("Silo") &&
                    !CurrentHoveredBuilding.buildingType.Value.Contains("Mill") &&
                    Game1.activeClickableMenu == null)
                {
                    Game1.mouseCursor = 4;
                }
            }
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady && e.IsMultipleOf(4u))
            {
                CurrentHoveredBuilding =
                    (Game1.currentLocation as BuildableGameLocation)?.getBuildingAt(Game1.currentCursorTile);
            }
        }

        private void OneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (_data != null && _data.Buildings.Count > 0)
            {
                foreach (KeyValuePair<string, bool> b in _data.Buildings)
                {
                    IEnumerable<Building> bb = Game1.getFarm().buildings
                        .Where(bbb => bbb.indoors.Value.uniqueName.Value.Equals(b.Key));
                    foreach (Building build in bb)
                    {
                        int pop = (build.buildingType.Value.Contains("Deluxe Barn")
                            ? Config.MaxBarnPopulation
                            : Config.MaxCoopPopulation);
                        if (((AnimalHouse) build.indoors.Value).animalLimit.Value != pop)
                        {
                            DoPopChange(build, showLog: true, doRestore: true);
                        }
                    }
                }
            }

            if (Config.Cheats.EnableFree)
            {
                DoPopChange(Config.MaxBarnPopulation, Config.MaxCoopPopulation);
            }
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (Config.AutoFeedExtraAnimals)
            {
                DoFeeding(Game1.getFarm());
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            _data = Helper.Data.ReadSaveData<PlayerData>(Helper.ModRegistry.ModID) ?? new PlayerData();
            if (!_data.Buildings.Any())
            {
                return;
            }

            IEnumerator<Building> enumerator = Game1.getFarm().buildings.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Building b = enumerator.Current;
                    if (b != null)
                    {
                        AnimalHouse bb = b.indoors.Value as AnimalHouse;
                        if (bb == null)
                        {
                            continue;
                        }

                        foreach (KeyValuePair<string, bool> building in _data.Buildings)
                        {
                            if (building.Key.Equals(bb.uniqueName.Value))
                            {
                                DoPopChange(bb.getBuilding(), showLog: true, doRestore: true);
                            }
                        }
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private bool CheckClick()
        {
            if (CurrentHoveredBuilding == null)
            {
                return false;
            }

            Vector2 curTile = GetCursorLocation();
            if (AnimalHouseBuildings.Any(ahb => CurrentHoveredBuilding.buildingType.Contains(ahb)) &&
                CurrentHoveredBuilding.indoors.Value != null &&
                Utility.tileWithinRadiusOfPlayer((int) curTile.X, (int) curTile.Y, 4, Game1.player))
            {
                return true;
            }

            return false;
        }

        private void DoPopChange(int maxBarnPop, int maxCoopPop, bool showLog = true)
        {
            foreach (Building b2 in Game1.getFarm().buildings.Where(b =>
                b.buildingType.Value.Contains("Deluxe Barn") || b.buildingType.Value.Contains("Deluxe Coop")))
            {
                int pop = (b2.buildingType.Value.Contains("Deluxe Barn") ? maxBarnPop : maxCoopPop);
                if (Config.AutoFeedExtraAnimals)
                {
                    DoFeeding((AnimalHouse) b2.indoors.Value);
                }

                if (((AnimalHouse) b2.indoors.Value).animalLimit.Value != pop)
                {
                    ((AnimalHouse) b2.indoors.Value).animalLimit.Value = pop;
                    b2.maxOccupants.Value = pop;
                    _data.Buildings.Add(b2.indoors.Value.uniqueName.Value, value: true);
                    if (Config.EnableDebugMode)
                    {
                        Game1.showGlobalMessage($"Set {b2.buildingType.Value} to {b2.maxOccupants.Value}");
                    }

                    Monitor.Log($"Set {b2.buildingType.Value} to {b2.maxOccupants.Value}",
                        showLog ? LogLevel.Debug : LogLevel.Trace);
                }
            }
        }

        private void DoPopChange(Building b, bool showLog = true, bool doRestore = false)
        {
            int pop = (b.buildingType.Value.Contains("Deluxe Barn")
                ? Config.MaxBarnPopulation
                : Config.MaxCoopPopulation);
            if (Config.AutoFeedExtraAnimals)
            {
                DoFeeding((AnimalHouse) b.indoors.Value);
            }

            if (((AnimalHouse) b.indoors.Value).animalLimit.Value != pop)
            {
                ((AnimalHouse) b.indoors.Value).animalLimit.Value = pop;
                b.maxOccupants.Value = pop;
                if (!doRestore)
                {
                    _data.Buildings.Add(b.indoors.Value.uniqueName.Value, value: true);
                }

                if (Config.EnableDebugMode)
                {
                    Game1.showGlobalMessage($"Set {b.buildingType.Value} to {b.maxOccupants.Value}");
                }

                Monitor.Log($"Set {b.buildingType.Value} to {b.maxOccupants.Value}",
                    showLog ? LogLevel.Debug : LogLevel.Trace);
            }
        }

        private void DoFeeding(AnimalHouse ah)
        {
            if (ah == null)
            {
                Monitor.Log("There was an error while trying to load the animal house. Code:1");
                return;
            }

            foreach (KeyValuePair<long, FarmAnimal> a in ah.animals.Pairs)
            {
                if (a.Value.fullness.Value != byte.MaxValue && Game1.getFarm().piecesOfHay.Value >= 1)
                {
                    a.Value.fullness.Value = byte.MaxValue;
                    a.Value.daysSinceLastFed.Value = 1;
                    Game1.getFarm().piecesOfHay.Value--;
                    if (Config.EnableDebugMode)
                    {
                        Monitor.Log($"Fed: {a.Value.Name}, new Fullness: {a.Value.fullness.Value}");
                    }
                }
            }
        }

        private void DoFeeding(Farm loc)
        {
            if (loc == null)
            {
                return;
            }

            foreach (Building b2 in loc.buildings.Where(b =>
                b.buildingType.Value.Contains("Deluxe Barn") || b.buildingType.Value.Contains("Deluxe Coop")))
            {
                AnimalHouse ah = b2.indoors.Value as AnimalHouse;
                if (ah == null)
                {
                    break;
                }

                foreach (KeyValuePair<long, FarmAnimal> a in ah.animals.Pairs)
                {
                    if (a.Value.fullness.Value != byte.MaxValue && Game1.getFarm().piecesOfHay.Value >= 1)
                    {
                        a.Value.fullness.Value = byte.MaxValue;
                        a.Value.daysSinceLastFed.Value = 1;
                        Game1.getFarm().piecesOfHay.Value--;
                        if (Config.EnableDebugMode)
                        {
                            Monitor.Log($"Fed: {a.Value.Name}, new Fullness: {a.Value.fullness.Value}");
                        }
                    }
                }
            }
        }

        private bool doHayRemoval(AnimalHouse ah)
        {

            return false;
        }
        private Vector2 GetCursorLocation()
        {
            return new Vector2((Game1.getOldMouseX() + Game1.viewport.X) / 64,
                (Game1.getOldMouseY() + Game1.viewport.Y) / 64);
        }

        private void DoRobinMenu()
        {
        }

        private void DoSanityCheck()
        {
            if (Config.MaxBarnPopulation <= 0)
            {
                Config.MaxBarnPopulation = 1;
                Helper.WriteConfig(Config);
                Monitor.Log("The configured MaxBarnPopulation wasn't higher than Zero, it has been set to 1",
                    LogLevel.Debug);
            }

            if (Config.MaxCoopPopulation <= 0)
            {
                Config.MaxCoopPopulation = 1;
                Helper.WriteConfig(Config);
                Monitor.Log("The configured MaxCoopPopulation wasn't higher than Zero, it has been set to 1",
                    LogLevel.Debug);
            }

            if (!Enum.TryParse(Config.RefreshConfigButton.ToString(), ignoreCase: true,
                out RefreshConfig))
            {
                RefreshConfig = SButton.F5;
                Monitor.Log("There was an error parsing the RefreshConfigButton. It was reset to F5");
            }
        }

        private void ResetSave(string command, string[] args)
        {
            if (_data != null)
            {
                _data?.Buildings.Clear();
                Monitor.Log("Save data was reset.", LogLevel.Debug);
            }
        }
    }
}