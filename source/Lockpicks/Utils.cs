/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdymods/SDV-Lockpicks
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace Lockpicks
{
    static class Utils
    {
        public static Vector2 GetTargetedTile()
        {
            Vector2 vector = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / 64f;
            if (Game1.mouseCursorTransparency == 0f || !Game1.wasMouseVisibleThisFrame || (!Game1.lastCursorMotionWasMouse && (Game1.player.ActiveObject == null || (!Game1.player.ActiveObject.isPlaceable() && Game1.player.ActiveObject.Category != -74))))
            {
                vector = Game1.player.GetGrabTile();
                if (vector.Equals(Game1.player.getTileLocation()))
                {
                    vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 1f);
                }
            }
            if (!Utility.tileWithinRadiusOfPlayer((int)vector.X, (int)vector.Y, 1, Game1.player))
            {
                vector = Game1.player.GetGrabTile();
                if (vector.Equals(Game1.player.getTileLocation()) && Game1.isAnyGamePadButtonBeingPressed())
                {
                    vector = Utility.getTranslatedVector2(vector, Game1.player.FacingDirection, 1f);
                }
            }
            return vector;
        }
        
        public static string GetAction(GameLocation l, Vector2 vec)
        {
            if(l is Forest)
            {
                if(l.getTileIndexAt(Utility.Vector2ToPoint(vec), "Buildings") == 1394)
                {
                    return "SewerGrate";
                }
            }
            string value = l.doesTileHaveProperty((int)vec.X, (int)vec.Y, "Action", "Buildings");
            if (string.IsNullOrWhiteSpace(value)) return null;
            return value;
        }

        public static void WarpFarmer(string location, int x, int y)
        {
            GameLocation destination = Game1.getLocationFromName(location);
            Game1.warpFarmer(new LocationRequest(destination.NameOrUniqueName, destination.uniqueName.Value != null, destination), x, y, 2);
        }

        public static bool IsWizardHouseUnlocked()
        {
            if (Game1.player.mailReceived.Contains("wizardJunimoNote") || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
                return true;
            int num1 = Game1.MasterPlayer.mailReceived.Contains("ccFishTank") ? 1 : 0;
            bool flag1 = Game1.MasterPlayer.mailReceived.Contains("ccBulletin");
            bool flag2 = Game1.MasterPlayer.mailReceived.Contains("ccPantry");
            bool flag3 = Game1.MasterPlayer.mailReceived.Contains("ccVault");
            bool flag4 = Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom");
            bool flag5 = Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom");
            int num2 = flag1 ? 1 : 0;
            return (num1 & num2 & (flag2 ? 1 : 0) & (flag3 ? 1 : 0) & (flag4 ? 1 : 0) & (flag5 ? 1 : 0)) != 0;
        }
    }
}
