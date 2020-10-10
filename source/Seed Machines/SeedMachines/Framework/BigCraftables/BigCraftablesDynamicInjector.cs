/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

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
