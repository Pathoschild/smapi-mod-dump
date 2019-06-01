using AutoAnimalDoors.StardewValleyWrapper;
using System.Collections.Generic;
using Buildings = AutoAnimalDoors.StardewValleyWrapper.Buildings;

namespace AutoAnimalDoors
{
    class ModEntry : StardewModdingAPI.Mod
    {
        private ModConfig config;
        private StardewModdingAPI.IModHelper helper;

        public override void Entry(StardewModdingAPI.IModHelper helper)
        {
            Logger.Instance.Initialize(this.Monitor);
            config = helper.ReadConfig<ModConfig>();
            this.helper = helper;
            helper.Events.GameLoop.DayStarted += SetupAutoDoorCallbacks;
        }

        private bool IsGoToSleepDialog(StardewValley.Menus.IClickableMenu menu)
        {
            StardewValley.Menus.DialogueBox dialogBox = menu as StardewValley.Menus.DialogueBox;
            if (dialogBox != null)
            {
                List<string> dialogs = this.Helper.Reflection.GetField<List<string>>(dialogBox, "dialogues").GetValue();
                if (dialogs != null && dialogs.Count >= 1)
                {
                    return dialogs[0].Equals(StardewValley.Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"));
                }
            }

            return false;
        }

        private void OnMenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs menuChangedEventArgs)
        {
            if (IsGoToSleepDialog(menuChangedEventArgs.NewMenu))
            {
                foreach (Buildings.AnimalBuilding eligibleAnimalBuilding in this.EligibleAnimalBuildings)
                {
                    eligibleAnimalBuilding.SendAllAnimalsHome();
                }
            }
        }

        private void SetupAutoDoorCallbacks(object sender, System.EventArgs eventArgs)
        {
            // Disable mod if not the main player (only one player needs to open/close the doors
            if (!StardewModdingAPI.Context.IsMainPlayer)
            {
                helper.Events.GameLoop.DayStarted -= SetupAutoDoorCallbacks;
                return;
            }

            Game game = Game.Instance;
            if (game.IsLoaded())
            {
                // Remove the subscriptions before adding them, this ensures we are only ever subscribed once
                helper.Events.Display.MenuChanged -= this.OnMenuChanged;
                helper.Events.GameLoop.TimeChanged -= this.OpenAnimalDoors;
                helper.Events.GameLoop.TimeChanged -= this.CloseAnimalDoors;

                bool skipDueToWinter = !config.OpenDoorsDuringWinter && game.Season == Season.WINTER;
                bool skipDueToWeather = !config.OpenDoorsWhenRaining && (game.Weather == Weather.RAINING || game.Weather == Weather.LIGHTNING);
                if (!skipDueToWinter && !skipDueToWeather)
                {
                    if (config.AutoOpenEnabled)
                    {
                        helper.Events.GameLoop.TimeChanged += this.OpenAnimalDoors;
                    }

                    helper.Events.GameLoop.TimeChanged += this.CloseAnimalDoors;
                    helper.Events.Display.MenuChanged += this.OnMenuChanged;
                }
            }
        }

        private int GetUpgradeLevelRequirementForBuidlingType(Buildings.AnimalBuildingType type)
        {
            if (type == Buildings.AnimalBuildingType.BARN)
            {
                return config.BarnRequiredUpgradeLevel;
            } else if (type == Buildings.AnimalBuildingType.COOP)
            {
                return config.CoopRequiredUpgradeLevel;
            }
            return 0;
        }

        /// <summary>This method gets only the animal buildings that are eligible for 
        ///    auto opening/closing based off the config settings.
        /// <example>For example:
        ///    If the CoopRequiredUpgradeLevel was set to 2, the Coops that are upgrade 
        ///    level 2 or higher would be returned ("Big Coop"s and "Deluxe Coop"s) while the coops
        ///    below that upgrade level (Normal "Coop"s) would not be returned.
        /// </example>
        /// </summary>
        private List<Buildings.AnimalBuilding> EligibleAnimalBuildings
        {
            get
            {
                List<Buildings.AnimalBuilding> eligibleAnimalBuildings = new List<Buildings.AnimalBuilding>(); ;
                foreach (Farm farm in Game.Instance.Farms)
                {
                    foreach (Buildings.AnimalBuilding animalBuilding in farm.AnimalBuildings)
                    {
                        if (animalBuilding.UpgradeLevel >= GetUpgradeLevelRequirementForBuidlingType(animalBuilding.Type))
                        {
                            eligibleAnimalBuildings.Add(animalBuilding);
                        }
                    }
                }
                return eligibleAnimalBuildings;
            }
        }

        private void SetAllAnimalDoorsState(Buildings.AnimalDoorState state)
        {
            foreach (Buildings.AnimalBuilding animalBuilding in this.EligibleAnimalBuildings)
            {
                animalBuilding.AnimalDoorState = state;
            }
        }

        private void CloseAnimalDoors(object sender, StardewModdingAPI.Events.TimeChangedEventArgs timeOfDayChanged)
        {
            if (timeOfDayChanged.NewTime >= config.AnimalDoorCloseTime)
            {
                List<Buildings.AnimalBuilding> eligibleAnimalBuildings = this.EligibleAnimalBuildings;
                foreach (Buildings.AnimalBuilding animalBuilding in eligibleAnimalBuildings)
                {
                    if (!animalBuilding.AreAllAnimalsHome())
                    {
                        return;
                    }
                }

                SetAllAnimalDoorsState(Buildings.AnimalDoorState.CLOSED);
                helper.Events.GameLoop.TimeChanged -= this.CloseAnimalDoors;
            }
        }

        private void OpenAnimalDoors(object sender, StardewModdingAPI.Events.TimeChangedEventArgs timeOfDayChanged)
        {
            if (timeOfDayChanged.NewTime >= config.AnimalDoorOpenTime && timeOfDayChanged.NewTime < config.AnimalDoorCloseTime)
            {
                helper.Events.GameLoop.TimeChanged -= this.OpenAnimalDoors;
                SetAllAnimalDoorsState(Buildings.AnimalDoorState.OPEN);
            }
        }
    }
}
