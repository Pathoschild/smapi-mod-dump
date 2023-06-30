/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace Survivalistic.Framework.Bars
{
    public static class BarsPosition
    {
        public static Vector2 barPosition;

        private static Vector2 sizeUI;
        private static string current_location;

        public static void SetBarsPosition()
        {
            if (!Context.IsWorldReady) return;

            sizeUI = new Vector2(Game1.uiViewport.Width, Game1.uiViewport.Height);
            current_location = Game1.player.currentLocation.Name;

            switch (ModEntry.config.bars_position)
            {
                case "bottom-right":
                    barPosition.X = GetPositionInRightBottomCorner();
                    barPosition.Y = sizeUI.Y;
                    BarsDatabase.right_side = true;
                    break;

                case "bottom-left":
                    barPosition.X = 70;
                    barPosition.Y = sizeUI.Y;
                    BarsDatabase.right_side = false;
                    break;

                case "middle-right":
                    barPosition.X = sizeUI.X - 56;
                    barPosition.Y = (sizeUI.Y / 2) + 75;
                    BarsDatabase.right_side = true;
                    break;

                case "middle-left":
                    barPosition.X = 70;
                    barPosition.Y = (sizeUI.Y / 2) + 75;
                    BarsDatabase.right_side = false;
                    break;

                case "top-right":
                    barPosition.X = sizeUI.X - 365;
                    if (Game1.player.appliedSpecialBuffs != null) barPosition.Y = 325;
                    else barPosition.Y = 290;
                    BarsDatabase.right_side = true;
                    break;

                case "top-left":
                    barPosition.X = 70;
                    if (CheckCavernLevelIsVisible(current_location)) barPosition.Y = 320;
                    else barPosition.Y = 260;
                    BarsDatabase.right_side = false;
                    break;

                case "custom":
                    barPosition.X = ModEntry.config.bars_custom_x;
                    barPosition.X = ModEntry.config.bars_custom_y;
                    if (barPosition.X >= sizeUI.X / 2) BarsDatabase.right_side = true;
                    else BarsDatabase.right_side = false;
                    break;
            }
        }

        /// <summary>
        /// Cause right bottom corner contains a lot of dynamic bars, so I moved this logic to this function.
        /// </summary>
        /// <returns>Position on 'X' axis.</returns>
        private static float GetPositionInRightBottomCorner()
        {
            #region Used variables.

            float position;

            bool inDangerous = CheckToDangerous();
            bool ultimateIsVisible = false;
            #endregion

            // Player is Safe, Ultimate isn't Visible.
            if (!inDangerous && !ultimateIsVisible)
                position = sizeUI.X - 116;

            // Player is Safe, Ultimate is Visible.
            else if (!inDangerous && ultimateIsVisible)
                position = sizeUI.X - 171;

            // Player isn't Safe, Ultimate isn't Visible.
            else if (inDangerous && !ultimateIsVisible)
                position = sizeUI.X - 171;

            // Player isn't Safe, Ultimate is Visible.
            else
                position = sizeUI.X - 226;

            return position;

        }

        private static bool CheckToDangerous() =>
                            Game1.showingHealth;

        private static bool CheckCavernLevelIsVisible(string locationName) =>
                            current_location.Contains("UndergroundMine") || current_location.Contains("SkullCavern") || 
                            (current_location.Contains("VolcanoDungeon") && current_location != "VolcanoDungeon0");
    }
}
