using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugSandBoxAndReferences
{
    public class DebugSandBox
    {
        /*
         * Notes:
         * Game1.player will always target the appropriate player whether it is the FarmHand or the main player.
         * Game1.MainPlayer will always target the host player.
         * 
         */


        public void Main()
        {

        }

        /// <summary>
        /// This is how you will iterate across a new dictionary in stardew valley
        /// </summary>
        public void dictionaryAccess()
        {
            GameLocation location=Game1.getLocationFromName("Farm");
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
            {

            }

        }
    }
}
