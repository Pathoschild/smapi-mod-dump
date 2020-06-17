using Microsoft.Xna.Framework;
using SeedMachines.Framework.BigCraftables;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework.BigCraftables
{
    class BigCraftablesDynamicInjector
    {
        public static void injectDynamicsInCurrentLocation()
        {
            OverlaidDictionary objects = Game1.currentLocation.objects;

            foreach (Vector2 key in objects.Keys)
            {
                IBigCraftableWrapper.checkAndInjectDynamicObject(objects, key);
            }
        }

        public static void removeDynamicsInAllLocations()
        {
            foreach (GameLocation location in Game1.locations)
            {
                OverlaidDictionary objects = location.objects;
                foreach (Vector2 key in objects.Keys)
                {
                    IBigCraftableWrapper.checkAndRemoveDynamicObject(objects, key);
                }
            }
        }
    }
}
