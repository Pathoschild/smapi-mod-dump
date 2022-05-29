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
    public class BarsPosition
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
                    if (current_location.Contains("UndergroundMine") || current_location.Contains("VolcanoDungeon") || Game1.player.health < Game1.player.maxHealth) barPosition.X = sizeUI.X - 171;
                    else barPosition.X = sizeUI.X - 116;
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
                    if (current_location.Contains("UndergroundMine")) barPosition.Y = 320;
                    else if (current_location.Contains("VolcanoDungeon") && current_location != "VolcanoDungeon0") barPosition.Y = 320;
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
    }
}
