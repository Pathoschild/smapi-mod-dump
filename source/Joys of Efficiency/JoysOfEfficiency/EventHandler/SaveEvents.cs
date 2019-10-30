using System;
using JoysOfEfficiency.Automation;
using JoysOfEfficiency.Core;
using StardewModdingAPI;
using StardewValley;

namespace JoysOfEfficiency.EventHandler
{
    internal class SaveEvents
    {
        private static Config Conf => InstanceHolder.Config;

        public void OnBeforeSave(object sender, EventArgs args)
        {
            if (!Context.IsWorldReady || !Conf.AutoAnimalDoor)
            {
                return;
            }
            AnimalAutomation.LetAnimalsInHome();
            AnimalAutomation.AutoCloseAnimalDoor();
        }

        public void OnDayStarted(object sender, EventArgs args)
        {
            //Reset LastTimeOfDay
            UpdateEvents.LastTimeOfDay = Game1.timeOfDay;

            if (!Context.IsWorldReady || !Conf.AutoAnimalDoor)
            {
                return;
            }
            AnimalAutomation.AutoOpenAnimalDoor();
            UpdateEvents.DayEnded = false;
        }
    }
}
