/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using BetterReturnScepter.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace BetterReturnScepter.HarmonyPatches
{
    public class WandPatches
	{
		private static Logger logger;
        private static PreviousPoint previousPoint;
        private static RodCooldown rodCooldown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger">An instance of our main Logger class, used for trace logs of which location/tile was saved.</param>
        /// <param name="point">The main instance of PreviousPoint the rest of the mod will warp the player to. Contains a GameLocation and Vector2 tile location.</param>
		public WandPatches(Logger newLogger, PreviousPoint point, RodCooldown cooldown)
		{
			logger = newLogger;
            previousPoint = point;
            rodCooldown = cooldown;
        }
        
        public static bool Wand_DoFunction_Prefix(Wand __instance, int x, int y, int power, Farmer who)
        {
            // If the player is in an instanceable structure (such as a shed, coop, barn, etc.), we want to warp to the front door.
            if (!who.currentLocation.isStructure)
            {
                previousPoint.Location = who.currentLocation;
                previousPoint.Tile = who.getTileLocation();
            }
            else
            {
                Game1.showRedMessage("Can't warp back to this location. Scepter set to default location.");
                previousPoint.Location = Utility.fuzzyLocationSearch("Farm");
                previousPoint.Tile = Utility.getHomeOfFarmer(who).getFrontDoorSpot().ToVector2();
            }
            
            // A warp has happened, so we reset our countdown.
            rodCooldown.ResetCountdown();

            logger.Log($"[In {nameof(Wand_DoFunction_Prefix)}] Saved previous tile {previousPoint.Tile} and location {previousPoint.Location.Name}", LogLevel.Trace);

            return true;
        }
    }
}