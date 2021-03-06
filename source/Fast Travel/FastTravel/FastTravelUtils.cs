/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeathGameDev/SDV-FastTravel
**
*************************************************/

using System.Linq;
using StardewValley;
using StardewValley.Menus;

namespace FastTravel
{
	public class FastTravelUtils
	{
        /// <summary>Checks if a map point exists within the config.</summary>
        /// <param name="point">The map point to check.</param>
        public static bool PointExistsInConfig(ClickableComponent point)
		{
            return ModEntry.Config.FastTravelPoints.Any(t => point.myID == t.pointId);
		}

		/// <summary>Checks if a player contains needed requirements to warp.</summary>
		/// <param name="mails">Array of strings.</param>
		public static ValidationPointResult CheckPointRequiredMails(string[] mails)
		{
            string lastErrorMessageKey = null;
			bool isValidWarp = true;
			foreach (var mail in mails)
			{
				if (!Game1.player.mailReceived.Contains(mail))
				{
                    lastErrorMessageKey = mail;
                    isValidWarp = false;
					break;
				}
			}

            return new ValidationPointResult(isValidWarp, lastErrorMessageKey);
        }

		/// <summary>Gets a location for a corresponding point on the map.</summary>
		/// <param name="point">The map point to check.</param>
		public static GameLocation GetLocationForMapPoint(ClickableComponent point)
		{
			return Game1.locations[ModEntry.Config.FastTravelPoints.First(t => t.pointId == point.myID).GameLocationIndex];
		}

		/// <summary>Gets the fast travel info for a corresponding point on the map.</summary>
		/// <param name="point">The map point to check.</param>
		public static FastTravelPoint GetFastTravelPointForMapPoint(ClickableComponent point)
		{
            FastTravelPoint fastTravelPointResult = ModEntry.Config.FastTravelPoints.First(t => t.pointId == point.myID);
            fastTravelPointResult.MapName = point.name;

            return fastTravelPointResult;
        }
	}
}