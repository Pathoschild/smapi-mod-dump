/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System;
using JoysOfEfficiency.Automation;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Misc;
using StardewModdingAPI;

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
            if (!Context.IsWorldReady || !Conf.AutoAnimalDoor)
            {
                return;
            }
            AnimalAutomation.AutoOpenAnimalDoor();
            UpdateEvents.DayEnded = false;
            IdlePause.OnDataLoaded();
        }
    }
}
