using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace BetterQualityMoreSeeds.Framework
{
    class TryToCheckAtPatch
    {
        private static IMonitor Monitor;
       
        //Call Method from Entry Class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        
        public static bool TryToCheckAt_PreFix( Vector2 grabTile, Farmer who, out KeyValuePair<Object, Object> __state)
        {
            __state = new KeyValuePair<Object, Object>(null,null);
            // The Most Important thing is to check if the player has a seedable object in his hand and whether or not the player is close to a seedmaker
            if ( who.CurrentItem == null || !IsSeedableCrop(who.CurrentItem)) return true;

            Object nearestSeedMaker = CheckForNearbySeedMakers(Game1.player.currentLocation, new Location((int)grabTile.X, (int)grabTile.Y), Game1.viewport, who);
            if ( nearestSeedMaker == null) return true;
            if (nearestSeedMaker.heldObject.Value != null ) return true;

            // Now just save the farmer's current item and the seedmaker that will get it and allow the game (or other mods) to perform its magic

            __state = new KeyValuePair<Object, Object>(nearestSeedMaker, (who.CurrentItem as Object) );

            return true;
        }

        internal static void TryToCheckAt_PostFix(KeyValuePair<Object, Object> __state)
        {
            PatchCommon.PostFix(__state, Monitor);
        }

        private static bool IsSeedableCrop(Item currentItem)
        {
            if (currentItem.ParentSheetIndex == 433) return false;

            Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
            foreach (KeyValuePair<int, string> keyValuePair in dictionary)
            {
                if (Convert.ToInt32(keyValuePair.Value.Split('/')[3]) == currentItem.ParentSheetIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private static Object CheckForNearbySeedMakers(GameLocation currentLocation, Location location, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            Vector2 key = new Vector2((float)location.X, (float)location.Y);
            if (currentLocation.objects.ContainsKey(key) && currentLocation.objects[key].Name.Equals("Seed Maker"))
                return currentLocation.objects[key];
            return null;
        }
    }
}
