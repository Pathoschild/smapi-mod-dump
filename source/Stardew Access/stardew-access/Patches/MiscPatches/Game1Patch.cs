/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Microsoft.Xna.Framework;
using stardew_access.Utils;
using StardewModdingAPI;
using StardewValley;

namespace stardew_access.Patches
{
    internal class Game1Patch
    {
        private static Vector2? prevTile = null;

        internal static void ExitActiveMenuPatch()
        {
            try
            {
                Log.Debug($"Closing {Game1.activeClickableMenu.GetType()} menu, performing cleanup...");
                IClickableMenuPatch.Cleanup(Game1.activeClickableMenu);
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void CloseTextEntryPatch()
        {
            TextBoxPatch.activeTextBoxes = "";
        }

        internal static bool PlaySoundPatch(string cueName)
        {
            try
            {
                if (!Context.IsPlayerFree)
                    return true;

                if (!Game1.player.isMoving())
                    return true;

                if (cueName == "grassyStep" || cueName == "sandyStep" || cueName == "snowyStep" || cueName == "stoneStep" || cueName == "thudStep" || cueName == "woodyStep")
                {
                    Vector2 nextTile = CurrentPlayer.FacingTile;
                    if (TileInfo.IsCollidingAtTile(Game1.currentLocation, (int)nextTile.X, (int)nextTile.Y))
                    {
                        if (prevTile != nextTile)
                        {
                            prevTile = nextTile;
                            //Game1.playSound("colliding");
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }
    }
}
