/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jacksonkimball/AVerySpecialBlueChicken
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace AVerySpecialBlueChicken
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<Config>();
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }
        Random random = new Random();

        public double PercentageChance { get; set; }
        public int HeartLevel { get; set; }
        internal Config Config { get; set; }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            int[] playerWantedLevel = new[] { 0, 200, 400, 600, 800, 1000 };
            var heartLevel = playerWantedLevel[Config.HeartLevel];
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is Coop coop)
                {
                    foreach (FarmAnimal animal in (building.indoors.Value as AnimalHouse).animals.Values)
                    {
                        if (animal.type.Value == "Blue Chicken" &&
                           animal.friendshipTowardFarmer.Value >= heartLevel &&
                           random.NextDouble() <= Config.PercentageChance)
                        {
                            foreach (KeyValuePair<Vector2, StardewValley.Object> objectAndLocation in building.indoors.Value.Objects.Pairs)
                            {
                                if (objectAndLocation.Value.ParentSheetIndex == 174 || objectAndLocation.Value.ParentSheetIndex == 176)
                                {
                                    building.indoors.Value.Objects[objectAndLocation.Key] = new StardewValley.Object(objectAndLocation.Key, 797, "Pearl", false, true, false, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
