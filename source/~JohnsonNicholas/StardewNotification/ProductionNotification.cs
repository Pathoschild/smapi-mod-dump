using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace StardewNotification
{
    public class ProductionNotification
    {
        /// <summary>
        /// Production notification.
        /// Handles notifications for all production machines like
        /// Bee House, Cheese Press, Keg, etc. 
        /// </summary>
        public void CheckProductionAroundFarm(ITranslationHelper Trans)
        {
            CheckFarmProductions(Trans);
            CheckShedProductions(Trans);
            CheckGreenhouseProductions(Trans);
            CheckCellarProductions(Trans);
            CheckBarnProductions(Trans);
        }

        public void CheckBarnProductions(ITranslationHelper Trans)
        {
            if (StardewNotification.Config.NotifyBarn)
            {
                //get barn(s)
                var ags = from loc in Game1.locations
                    where loc is AnimalHouse
                    from ag in loc.objects.Pairs
                    where ag.Value.bigCraftable.Value && ag.Value.ParentSheetIndex == 165
                    let count = (ag.Value.heldObject.Value as Chest)?.items.Count ?? 0
                    where count > 0
                    select ag;

                foreach (var ag in ags)
                {
                    Game1.addHUDMessage(new HUDMessage(Trans.Get("autoGrabber")));
                }
            }
        }

        public void CheckFarmProductions(ITranslationHelper Trans)
        {
            if (StardewNotification.Config.NotifyFarm)
            {
                CheckObjectsInLocation(Trans, Game1.getFarm());
            }
        }

        public void CheckShedProductions(ITranslationHelper Trans)
        {
            if (StardewNotification.Config is null)
                Console.WriteLine("Config is null");

            if (StardewNotification.Config.NotifyShed)
            {
                Farm f = Game1.getFarm();

                if (f is null)
                    Console.WriteLine("Farm is null. Somehow.");

                if (f.buildings is null)
                    Console.WriteLine("Farm Buildings is null. Somehow.");

                foreach (var building in f.buildings)
                {
                    if (building.indoors.Value is Shed)
                    {
                        CheckObjectsInLocation(Trans, building.indoors.Value);
                    }
                }
            }
        }

        public void CheckGreenhouseProductions(ITranslationHelper Trans)
        {
            if (StardewNotification.Config.NotifyGreenhouse)
            {
                CheckObjectsInLocation(Trans, Game1.getLocationFromName("Greenhouse"));
            }
        }

        public void CheckCellarProductions(ITranslationHelper Trans)
        {
            if (StardewNotification.Config.NotifyCellar)
            {
                CheckObjectsInLocation(Trans, Game1.getLocationFromName("Cellar"));
            }
        }

        private void CheckObjectsInLocation(ITranslationHelper Trans, GameLocation location)
        {
            var counter = new Dictionary<string, Pair<StardewValley.Object, int>>();

            foreach (var pair in location.Objects.Pairs)
            {
                if (!pair.Value.readyForHarvest.Value) continue;
                if (counter.ContainsKey(pair.Value.Name)) counter[pair.Value.Name].Second++;
                else counter.Add(pair.Value.Name, new Pair<StardewValley.Object, int>(pair.Value, 1));
            }

            foreach (var pair in counter)
                Util.ShowHarvestableMessage(Trans, pair);
        }
    }
}