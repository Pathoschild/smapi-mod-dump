/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/

using AutoAnimalDoors.Menu;
using AutoAnimalDoors.StardewValleyWrapper;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using Buildings = AutoAnimalDoors.StardewValleyWrapper.Buildings;

namespace AutoAnimalDoors
{
    public class ModEntry : StardewModdingAPI.Mod
    {
        public static bool HasDoorsClosedToday { get; set; } = false;
        public static bool HasDoorsOpenedToday { get; set; } = false;


        private MenuRegistry GenericMenuRegistry { get; set; }

        public override void Entry(StardewModdingAPI.IModHelper helper)
        {
            Logger.Instance.Initialize(this.Monitor);
            GenericMenuRegistry = new MenuRegistry(Helper);
            ModConfig.Instance = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += SetupAutoDoorCallbacks;
            helper.Events.GameLoop.GameLaunched += SetupMenu;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        }

        private void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            HasDoorsOpenedToday = false;
            HasDoorsClosedToday = false;
        }

        private void SetupMenu(object sender, GameLaunchedEventArgs e)
        {
            GenericMenuRegistry.InitializeMenu(ModManifest, ModConfig.Instance);
            GenericMenuRegistry.AutoOpenedEnabledChanged += AutoOpenedEnabledChanged;
        }

        private void AutoOpenedEnabledChanged(object sender, bool autoOpenEnabled)
        {
            SetupAutoDoorCallbacks();
            if (ModConfig.Instance.AutoOpenEnabled && Game.Instance.IsLoaded())
            {
                OpenAnimalDoors(Game.Instance.CurrentTime);
            }
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
                Logger.Instance.Log("Entered Bed, Warping Animals Back Now");
                foreach (Buildings.AnimalBuilding eligibleAnimalBuilding in this.EligibleAnimalBuildings)
                {
                    eligibleAnimalBuilding.SendAllAnimalsHome();
                }
            }
        }

        private void SetupAutoDoorCallbacks(object sender, System.EventArgs eventArg)
        {
            SetupAutoDoorCallbacks();
        }
        private void SetupAutoDoorCallbacks()
        {
            ModEntry.HasDoorsClosedToday = false;
            ModEntry.HasDoorsOpenedToday = false;

            // Remove callback for non host computers, no need to keep calling this
            if (!StardewModdingAPI.Context.IsOnHostComputer)
            {
                Logger.Instance.Log("User is not the host, disabling mod");
                Helper.Events.GameLoop.DayStarted -= SetupAutoDoorCallbacks;
                return;
            }

            // Split screen users don't need to close the doors, but can't remove callback since the 
            // host needs to have this method run for them
            if (!StardewModdingAPI.Context.IsMainPlayer)
            {
                return;
            }

            Game game = Game.Instance;
            if (game.IsLoaded())
            {
                // Remove the subscriptions before adding them, this ensures we are only ever subscribed once
                Helper.Events.Display.MenuChanged -= this.OnMenuChanged;
                Helper.Events.GameLoop.TimeChanged -= this.OpenAnimalDoors;
                Helper.Events.GameLoop.TimeChanged -= this.CloseAnimalDoors;

                bool skipDueToWinter = !ModConfig.Instance.OpenDoorsDuringWinter && game.Season == StardewValleyWrapper.Season.WINTER;
                bool skipDueToWeather = !ModConfig.Instance.OpenDoorsWhenRaining && (game.Weather == Weather.RAINING || game.Weather == Weather.LIGHTNING);

                if (skipDueToWinter)
                {
                    Logger.Instance.Log("Skipping because it is Winter");
                    return;
                }

                else if (skipDueToWeather)
                {
                    Logger.Instance.Log("Skipping due to Weather");
                    return;
                }

                if (ModConfig.Instance.AutoOpenEnabled)
                {
                    Helper.Events.GameLoop.TimeChanged += this.OpenAnimalDoors;
                }

                Helper.Events.GameLoop.TimeChanged += this.CloseAnimalDoors;
                Helper.Events.Display.MenuChanged += this.OnMenuChanged;
            }
        }

        private int GetUpgradeLevelRequirementForBuidlingType(Buildings.AnimalBuildingType type)
        {
            if (type == Buildings.AnimalBuildingType.BARN)
            {
                return ModConfig.Instance.BarnRequiredUpgradeLevel;
            }
            else if (type == Buildings.AnimalBuildingType.COOP)
            {
                return ModConfig.Instance.CoopRequiredUpgradeLevel;
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
                foreach (StardewValleyWrapper.Farm farm in Game.Instance.Farms)
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
            List<Buildings.AnimalBuilding> eligibleAnimalBuildings = this.EligibleAnimalBuildings;
            Logger.Instance.Log(string.Format("Changing state of {0} animal doors to {1}", eligibleAnimalBuildings.Count, state));

            foreach (Buildings.AnimalBuilding animalBuilding in eligibleAnimalBuildings)
            {
                animalBuilding.AnimalDoorState = state;
            }
        }

        private void CloseAnimalDoors(object sender, StardewModdingAPI.Events.TimeChangedEventArgs timeOfDayChanged)
        {
            if (timeOfDayChanged.NewTime >= ModConfig.Instance.AnimalDoorCloseTime)
            {
                List<Buildings.AnimalBuilding> eligibleAnimalBuildings = this.EligibleAnimalBuildings;
                if (ModConfig.Instance.CloseAllBuildingsAtOnce)
                {
                    foreach (Buildings.AnimalBuilding animalBuilding in eligibleAnimalBuildings)
                    {
                        if (!animalBuilding.AreAllAnimalsHome())
                        {
                            return;
                        }
                    }
                    SetAllAnimalDoorsState(Buildings.AnimalDoorState.CLOSED);
                }
                else
                {
                    foreach (Buildings.AnimalBuilding animalBuilding in eligibleAnimalBuildings)
                    {
                        if (animalBuilding.AnimalDoorState == Buildings.AnimalDoorState.OPEN && animalBuilding.AreAllAnimalsHome())
                        {
                            animalBuilding.AnimalDoorState = Buildings.AnimalDoorState.CLOSED;
                        }
                    }
                }

                if (eligibleAnimalBuildings.All(ab => ab.AnimalDoorState == Buildings.AnimalDoorState.CLOSED))
                {
                    if (ModConfig.Instance.DoorEventPopupEnabled)
                    {
                        Game1.chatBox.addInfoMessage("All animal doors have been closed!");
                    }
                    
                    ModEntry.HasDoorsClosedToday = true;
                    Helper.Events.GameLoop.TimeChanged -= this.CloseAnimalDoors;
                }
            }
        }

        private void OpenAnimalDoors(int currentTime)
        {
            if (currentTime >= ModConfig.Instance.AnimalDoorOpenTime && currentTime < ModConfig.Instance.AnimalDoorCloseTime)
            {
                if (ModConfig.Instance.DoorEventPopupEnabled)
                {
                    Game1.chatBox.addInfoMessage("All animal doors have been opened!");
                }
                
                ModEntry.HasDoorsOpenedToday = true;
                Helper.Events.GameLoop.TimeChanged -= this.OpenAnimalDoors;
                SetAllAnimalDoorsState(Buildings.AnimalDoorState.OPEN);
            }
        }

        private void OpenAnimalDoors(object sender, StardewModdingAPI.Events.TimeChangedEventArgs timeOfDayChanged)
        {
            OpenAnimalDoors(timeOfDayChanged.NewTime);
        }
    }
}
