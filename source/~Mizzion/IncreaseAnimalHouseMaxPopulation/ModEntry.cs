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
using Netcode;
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

        public ITranslationHelper i18n;

        public Building CurrentHoveredBuilding = null;

        public Building CurrentHoveredBuildingDummy = null;

        public List<string> AnimalHouseBuildings = new List<string>
        {
            "Deluxe Barn",
            "Deluxe Coop"
        };

        public int Cost = 0;

        public bool IsTestBuild = false;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            this.i18n = helper.Translation;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.Saving += Saving;
            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.Display.RenderingHud += RenderingHud;
            helper.ConsoleCommands.Add("pop_reset", "Deletes the save data for Increased Animal House Population.",
                ResetSave);
            this.DoSanityCheck();
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

            if (e.IsDown(this.RefreshConfig))
            {
                this.Config = base.Helper.ReadConfig<ModConfig>();
                this.DoSanityCheck();
                base.Monitor.Log("Reloaded the configuration file.", LogLevel.Debug);
            }

            if (e.IsDown(SButton.F6) && this.IsTestBuild)
            {
                this.DoPopChange(this.Config.MaxBarnPopulation, this.Config.MaxCoopPopulation);
            }

            if (!e.IsDown(SButton.MouseLeft) || this.CurrentHoveredBuilding == null ||
                this.CurrentHoveredBuilding.buildingType.Value.Contains("Cabin") ||
                this.CurrentHoveredBuilding.buildingType.Value.Contains("Silo") ||
                this.CurrentHoveredBuilding.buildingType.Value.Contains("Mill") ||
                this._data.buildings.ContainsKey(this.CurrentHoveredBuilding.indoors.Value.uniqueName.Value) ||
                Game1.activeClickableMenu != null)
            {
                return;
            }

            Vector2 tLocation = this.GetCursorLocation();
            if (!this.AnimalHouseBuildings.Any((string ab) =>
                this.CurrentHoveredBuilding.buildingType.Contains(ab) &&
                this.CurrentHoveredBuilding.indoors.Value != null) || this.CurrentHoveredBuilding == null)
            {
                return;
            }

            int freeOrNot = ((!this.Config.Cheats.EnableFree) ? this.Config.CostPerPopulationIncrease : 0);
            //Lets calculate the difference between max and current max population
            int currentMaxOccupants = ((AnimalHouse) this.CurrentHoveredBuilding.indoors.Value).animalLimit.Value; //(AnimalHouse)this.CurrentHoveredBuilding.indoors
            this.Cost = (this.CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn")
                ? ((this.Config.MaxBarnPopulation - currentMaxOccupants)* freeOrNot)
                : ((this.Config.MaxCoopPopulation - currentMaxOccupants) * freeOrNot));
            
            this.CurrentHoveredBuildingDummy = this.CurrentHoveredBuilding;
            string question = this.i18n.Get("upgrade_question", new
            {
                current_building = this.CurrentHoveredBuilding.buildingType.Value,
                next_cost = this.Cost,
                current_max_occupants = currentMaxOccupants,
                config_max_occupants = this.CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn") ? this.Config.MaxBarnPopulation : this.Config.MaxCoopPopulation
            });
            Game1.getFarm().createQuestionDialogue(question, Game1.getFarm().createYesNoResponses(),
                delegate(Farmer _, string answer)
                {
                    if (answer == "Yes")
                    {
                        if (Game1.player.Money >= this.Cost)
                        {
                            Game1.player.Money -= this.Cost;
                            this.DoPopChange(this.CurrentHoveredBuildingDummy);
                        }
                        else
                        {
                            Game1.showRedMessage($"You don't have {this.Cost} gold.");
                        }
                    }
                });
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            base.Helper.Data.WriteSaveData(base.Helper.ModRegistry.ModID, this._data);
        }

        private void RenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (this.CurrentHoveredBuilding != null && Game1.activeClickableMenu == null &&
                (this.Config.EnableDebugMode || this.Config.EnableHoverTip) && this.AnimalHouseBuildings.Any(
                    (string ab) => this.CurrentHoveredBuilding.buildingType.Contains(ab) &&
                                   this.CurrentHoveredBuilding.indoors.Value != null))
            {
                Translation tipText = this.i18n.Get("upgrade_tooltip_text", new
                {
                    current_building = this.CurrentHoveredBuilding.buildingType.Value,
                    max_animals = ((AnimalHouse) this.CurrentHoveredBuilding.indoors.Value).animalLimit
                });
                IClickableMenu.drawHoverText(Game1.spriteBatch, tipText, Game1.smallFont);
            }

            if (this.CurrentHoveredBuilding != null)
            {
                int p = (this.CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe Barn")
                    ? this.Config.MaxBarnPopulation
                    : this.Config.MaxCoopPopulation);
                AnimalHouse obj = this.CurrentHoveredBuilding.indoors.Value as AnimalHouse;
                if ((obj == null || obj.animalLimit.Value != p) &&
                    this.CurrentHoveredBuilding.buildingType.Value.Contains("Deluxe") &&
                    !this.CurrentHoveredBuilding.buildingType.Value.Contains("Cabin") &&
                    !this.CurrentHoveredBuilding.buildingType.Value.Contains("Silo") &&
                    !this.CurrentHoveredBuilding.buildingType.Value.Contains("Mill") &&
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
                this.CurrentHoveredBuilding =
                    (Game1.currentLocation as BuildableGameLocation)?.getBuildingAt(Game1.currentCursorTile);
            }
        }

        private void OneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (this._data != null && this._data.buildings.Count > 0)
            {
                foreach (KeyValuePair<string, bool> b in this._data.buildings)
                {
                    IEnumerable<Building> bb = Game1.getFarm().buildings
                        .Where((Building bbb) => bbb.indoors.Value.uniqueName.Value.Equals(b.Key));
                    foreach (Building build in bb)
                    {
                        int pop = (build.buildingType.Value.Contains("Deluxe Barn")
                            ? this.Config.MaxBarnPopulation
                            : this.Config.MaxCoopPopulation);
                        if (((AnimalHouse) build.indoors.Value).animalLimit.Value != pop)
                        {
                            this.DoPopChange(build, showLog: true, doRestore: true);
                        }
                    }
                }
            }

            if (this.Config.Cheats.EnableFree)
            {
                this.DoPopChange(this.Config.MaxBarnPopulation, this.Config.MaxCoopPopulation);
            }
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (this.Config.AutoFeedExtraAnimals)
            {
                this.DoFeeding(Game1.getFarm());
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            this._data = base.Helper.Data.ReadSaveData<PlayerData>(base.Helper.ModRegistry.ModID) ?? new PlayerData();
            if (!this._data.buildings.Any())
            {
                return;
            }

            IEnumerator<Building> enumerator = Game1.getFarm().buildings.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Building b = enumerator.Current;
                    AnimalHouse bb = b.indoors.Value as AnimalHouse;
                    if (bb == null)
                    {
                        continue;
                    }

                    foreach (KeyValuePair<string, bool> building in this._data.buildings)
                    {
                        if (building.Key.Equals(bb.uniqueName.Value))
                        {
                            this.DoPopChange(bb.getBuilding(), showLog: true, doRestore: true);
                        }
                    }
                }
            }
            finally
            {
                ((IDisposable) enumerator).Dispose();
            }
        }

        private bool CheckClick()
        {
            if (this.CurrentHoveredBuilding == null)
            {
                return false;
            }

            Vector2 curTile = this.GetCursorLocation();
            if (this.AnimalHouseBuildings.Any((string ahb) => this.CurrentHoveredBuilding.buildingType.Contains(ahb)) &&
                this.CurrentHoveredBuilding.indoors.Value != null &&
                Utility.tileWithinRadiusOfPlayer((int) curTile.X, (int) curTile.Y, 4, Game1.player))
            {
                return true;
            }

            return false;
        }

        private void DoPopChange(int maxBarnPop, int maxCoopPop, bool showLog = true)
        {
            foreach (Building b2 in Game1.getFarm().buildings.Where((Building b) =>
                b.buildingType.Value.Contains("Deluxe Barn") || b.buildingType.Value.Contains("Deluxe Coop")))
            {
                int pop = (b2.buildingType.Value.Contains("Deluxe Barn") ? maxBarnPop : maxCoopPop);
                if (this.Config.AutoFeedExtraAnimals)
                {
                    this.DoFeeding((AnimalHouse) b2.indoors.Value);
                }

                if (((AnimalHouse) b2.indoors.Value).animalLimit.Value != pop)
                {
                    ((AnimalHouse) b2.indoors.Value).animalLimit.Value = pop;
                    b2.maxOccupants.Value = pop;
                    this._data.buildings.Add(b2.indoors.Value.uniqueName.Value, value: true);
                    if (this.Config.EnableDebugMode)
                    {
                        Game1.showGlobalMessage($"Set {b2.buildingType.Value} to {b2.maxOccupants.Value}");
                    }

                    base.Monitor.Log($"Set {b2.buildingType.Value} to {b2.maxOccupants.Value}",
                        showLog ? LogLevel.Debug : LogLevel.Trace);
                }
            }
        }

        private void DoPopChange(Building b, bool showLog = true, bool doRestore = false)
        {
            int pop = (b.buildingType.Value.Contains("Deluxe Barn")
                ? this.Config.MaxBarnPopulation
                : this.Config.MaxCoopPopulation);
            if (this.Config.AutoFeedExtraAnimals)
            {
                this.DoFeeding((AnimalHouse) b.indoors.Value);
            }

            if (((AnimalHouse) b.indoors.Value).animalLimit.Value != pop)
            {
                ((AnimalHouse) b.indoors.Value).animalLimit.Value = pop;
                b.maxOccupants.Value = pop;
                if (!doRestore)
                {
                    this._data.buildings.Add(b.indoors.Value.uniqueName.Value, value: true);
                }

                if (this.Config.EnableDebugMode)
                {
                    Game1.showGlobalMessage($"Set {b.buildingType.Value} to {b.maxOccupants.Value}");
                }

                base.Monitor.Log($"Set {b.buildingType.Value} to {b.maxOccupants.Value}",
                    showLog ? LogLevel.Debug : LogLevel.Trace);
            }
        }

        private void DoFeeding(AnimalHouse ah)
        {
            if (ah == null)
            {
                base.Monitor.Log("There was an error while trying to load the animal house. Code:1");
                return;
            }

            foreach (KeyValuePair<long, FarmAnimal> a in ah.animals.Pairs)
            {
                a.Value.fullness.Value = byte.MaxValue;
                a.Value.daysSinceLastFed.Value = 1;
                if (this.Config.EnableDebugMode)
                {
                    base.Monitor.Log($"Fed: {a.Value.Name}, new Fullnes: {a.Value.fullness.Value}");
                }
            }
        }

        private void DoFeeding(Farm loc)
        {
            if (loc == null)
            {
                return;
            }

            foreach (Building b2 in loc.buildings.Where((Building b) =>
                b.buildingType.Value.Contains("Deluxe Barn") || b.buildingType.Value.Contains("Deluxe Coop")))
            {
                AnimalHouse ah = b2.indoors.Value as AnimalHouse;
                if (ah == null)
                {
                    break;
                }

                foreach (KeyValuePair<long, FarmAnimal> a in ah.animals.Pairs)
                {
                    if (a.Value.fullness.Value != byte.MaxValue)
                    {
                        a.Value.fullness.Value = byte.MaxValue;
                        a.Value.daysSinceLastFed.Value = 1;
                        if (this.Config.EnableDebugMode)
                        {
                            base.Monitor.Log($"Fed: {a.Value.Name}, new Fullnes: {a.Value.fullness.Value}");
                        }
                    }
                }
            }
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
            if (this.Config.MaxBarnPopulation <= 0)
            {
                this.Config.MaxBarnPopulation = 1;
                base.Helper.WriteConfig(this.Config);
                base.Monitor.Log("The configured MaxBarnPopulation wasn't higher than Zero, it has been set to 1",
                    LogLevel.Debug);
            }

            if (this.Config.MaxCoopPopulation <= 0)
            {
                this.Config.MaxCoopPopulation = 1;
                base.Helper.WriteConfig(this.Config);
                base.Monitor.Log("The configured MaxCoopPopulation wasn't higher than Zero, it has been set to 1",
                    LogLevel.Debug);
            }

            if (!Enum.TryParse<SButton>(this.Config.RefreshConfigButton.ToString(), ignoreCase: true,
                out this.RefreshConfig))
            {
                this.RefreshConfig = SButton.F5;
                base.Monitor.Log("There was an error parsing the RefreshConfigButton. It was reset to F5");
            }
        }

        private void ResetSave(string command, string[] args)
        {
            if (this._data != null)
            {
                this._data?.buildings.Clear();
                base.Monitor.Log("Save data was reset.", LogLevel.Debug);
            }
        }
    }
}