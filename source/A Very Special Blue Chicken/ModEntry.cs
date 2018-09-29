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
            this.Config = this.Helper.ReadConfig<Config>();
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
        }
        Random random = new Random();

        public double PercentageChance { get; set; }
        internal Config Config { get; set; }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building is Coop coop)
                {
                    foreach (FarmAnimal animal in (building.indoors.Value as AnimalHouse).animals.Values)
                    {
                        if (animal.type.Value == "Blue Chicken" &&
                           animal.friendshipTowardFarmer.Value >= 800 &&
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
