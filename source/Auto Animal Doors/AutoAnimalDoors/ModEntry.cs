using AutoAnimalDoors.StardewValleyWrapper;
using System.Collections.Generic;
using Buildings = AutoAnimalDoors.StardewValleyWrapper.Buildings;

namespace AutoAnimalDoors
{
    class ModEntry : StardewModdingAPI.Mod
    {
        private ModConfig config;

        public override void Entry(StardewModdingAPI.IModHelper helper)
        {
            Logger.Instance.Initialize(this.Monitor);
            config = helper.ReadConfig<ModConfig>();
            StardewModdingAPI.Events.TimeEvents.AfterDayStarted += SetupAutoDoorCallbacks;
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

        private void OnMenuChanged(object sender, StardewModdingAPI.Events.EventArgsClickableMenuChanged menuChangedEventArgs)
        {
            if (IsGoToSleepDialog(menuChangedEventArgs.NewMenu))
            {
                foreach (Farm farm in Game.Instance.Farms)
                {
                    farm.SendAllAnimalsHome();
                }
            }
        }

        private void SetupAutoDoorCallbacks(object sender, System.EventArgs eventArgs)
        {
            // Disable mod if not the main player (only one player needs to open/close the doors
            if (!StardewModdingAPI.Context.IsMainPlayer)
            {
                StardewModdingAPI.Events.TimeEvents.AfterDayStarted -= SetupAutoDoorCallbacks;
                return;
            }

            Game game = Game.Instance;
            if (game.IsLoaded())
            {
                // Remove the subscriptions before adding them, this ensures we are only ever subscribed once
                StardewModdingAPI.Events.MenuEvents.MenuChanged -= this.OnMenuChanged;
                StardewModdingAPI.Events.TimeEvents.TimeOfDayChanged -= this.OpenAnimalDoors;
                StardewModdingAPI.Events.TimeEvents.TimeOfDayChanged -= this.CloseAnimalDoors;

                bool skipDueToWinter = !config.OpenDoorsDuringWinter && game.Season == Season.WINTER;
                bool skipDueToWeather = !config.OpenDoorsWhenRaining && (game.Weather == Weather.RAINING || game.Weather == Weather.LIGHTNING);
                if (!skipDueToWinter && !skipDueToWeather)
                {
                    if (config.AutoOpenEnabled)
                    {
                        StardewModdingAPI.Events.TimeEvents.TimeOfDayChanged += this.OpenAnimalDoors;
                    }
                    
                    StardewModdingAPI.Events.TimeEvents.TimeOfDayChanged += this.CloseAnimalDoors;
                    StardewModdingAPI.Events.MenuEvents.MenuChanged += this.OnMenuChanged;
                }
            }
        }

        private void SetAllAnimalDoorsState(Buildings.AnimalDoorState state)
        {
            foreach (Farm farm in Game.Instance.Farms)
            {
                farm.SetAnimalDoorsState(state);
            }
        }

        private void CloseAnimalDoors(object sender, StardewModdingAPI.Events.EventArgsIntChanged timeOfDayChanged)
        {
            if (timeOfDayChanged.NewInt >= config.AnimalDoorCloseTime)
            {
                bool allAnimalsInAllFarmsAreHome = true;
                foreach (Farm farm in Game.Instance.Farms)
                {
                    if (farm.AreAllAnimalsHome())
                    {
                        farm.SetAnimalDoorsState(Buildings.AnimalDoorState.CLOSED);
                    }
                    else
                    {
                        allAnimalsInAllFarmsAreHome = false;
                    }
                }
                if (allAnimalsInAllFarmsAreHome)
                {
                    StardewModdingAPI.Events.TimeEvents.TimeOfDayChanged -= this.CloseAnimalDoors;
                }
            }
        }

        private void OpenAnimalDoors(object sender, StardewModdingAPI.Events.EventArgsIntChanged timeOfDayChanged)
        {
            if (timeOfDayChanged.NewInt >= config.AnimalDoorOpenTime && timeOfDayChanged.NewInt < config.AnimalDoorCloseTime)
            {
                StardewModdingAPI.Events.TimeEvents.TimeOfDayChanged -= this.OpenAnimalDoors;
                SetAllAnimalDoorsState(Buildings.AnimalDoorState.OPEN);
            }
        }
    }
}
