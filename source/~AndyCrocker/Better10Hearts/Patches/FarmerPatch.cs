/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;

namespace Better10Hearts.Patches
{
    /// <summary>Contains patches for patching game code in the StardewValley.Farmer class.</summary>
    internal class FarmerPatch
    {
        /// <summary>This is code that will replace some game code, this is ran whenever the player is about to passout.</summary>
        /// <returns>This will always return false as this contains the game code as well as the patch.</returns>
        internal static bool PassOutFromTiredPrefix(Farmer who)
        {
            if (!who.IsLocalPlayer)
            {
                return false;
            }

            if (who.isRidingHorse())
            {
                who.mount.dismount();
            }

            if (Game1.activeClickableMenu != null)
            {
                Game1.activeClickableMenu.emergencyShutDown();
                Game1.exitActiveMenu();
            }

            who.completelyStopAnimatingOrDoingAction();
            if (who.bathingClothes.Value)
            {
                who.changeOutOfSwimSuit();
            }

            who.swimming.Value = false;
            who.CanMove = false;
            GameLocation passOutLocation = Game1.currentLocation;
            Vector2 bed = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).getBedSpot()) * 64f;
            bed.X -= 64f;

            LocationRequest.Callback callback = (LocationRequest.Callback)(() =>
            {
                who.Position = bed;
                who.currentLocation.lastTouchActionLocation = bed;
                if (!Game1.IsMultiplayer || Game1.timeOfDay >= 2600)
                {
                    Game1.PassOutNewDay();
                }

                Game1.changeMusicTrack("none");

                if (passOutLocation is FarmHouse || passOutLocation is Cellar)
                {
                    return;
                }

                // check if an NPC has picked up the player or if the game should handle the passout
                if (!ModEntry.HasPassoutBeenHandled)
                {
                    int num = Math.Min(1000, who.Money / 10);
                    who.Money -= num;
                    who.mailForTomorrow.Add("passedOut " + (object)num);
                }
            });
            
            if (!who.isInBed.Value)
            {
                LocationRequest locationRequest = Game1.getLocationRequest(who.homeLocation.Value, false);
                Game1.warpFarmer(locationRequest, (int)bed.X / 64, (int)bed.Y / 64, 2);
                locationRequest.OnWarp += callback;
                who.FarmerSprite.setCurrentSingleFrame(5, (short)3000, false, false);
                who.FarmerSprite.PauseForSingleAnimation = true;
            }
            else
            {
                callback();
            }

            return false;
        }
    }
}
