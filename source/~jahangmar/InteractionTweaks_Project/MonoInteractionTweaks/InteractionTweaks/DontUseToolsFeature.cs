//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace InteractionTweaks
{
    public class DontUseToolsFeature : ModFeature
    {
        public static new void Enable()
        {
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        public static new void Disable()
        {
            Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
        }

        private static void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (Game1.currentLocation is Farm farm && e.Button.IsUseToolButton())
            {
                Vector2 toolHitTile = GetToolHitLocation(e.Cursor.Tile);
               
                //Monitor.Log("toolVec = "+toolVec, LogLevel.Trace);
                if (BlockTool(toolHitTile))
                {
                    Monitor.Log("Blocking Hoe use", LogLevel.Trace);
                    Helper.Input.Suppress(e.Button);
                }
            }

        }

        private static bool BlockTool(Vector2 toolHitTile)
        {
            return Game1.player.CurrentTool is StardewValley.Tools.Hoe && Game1.getFarm().isTileHoeDirt(toolHitTile);
        }

        private static Vector2 GetToolHitLocation(Vector2 mapTile)
        {
            Farmer player = Game1.player;
            int plX = player.getTileX();
            int plY = player.getTileY();
            int hitX = (int)mapTile.X;
            int hitY = (int)mapTile.Y;
            switch (player.FacingDirection)
            {
                case 0: //up
                    if (hitY == plY - 1 && (hitX == plX - 1 || hitX == plX || hitX == plX + 1))
                        return mapTile;
                    else
                        return new Vector2(plX, plY - 1);
                case 1: //right
                    if (hitX == plX + 1 && (hitY == plY - 1 || hitY == plY || hitY == plY + 1))
                        return mapTile;
                    else
                        return new Vector2(plX+1, plY);
                case 2: //down
                    if (hitY == plY + 1 && (hitX == plX - 1 || hitX == plX || hitX == plX + 1))
                        return mapTile;
                    else
                        return new Vector2(plX, plY + 1);
                case 3: //left
                    if (hitX == plX - 1 && (hitY == plY - 1 || hitY == plY || hitY == plY + 1))
                        return mapTile;
                    else
                        return new Vector2(plX - 1, plY);
                default:
                    Monitor.Log("player.FacingDirection has unexpected value: " + player.FacingDirection, LogLevel.Error);
                    return mapTile;
            }
        }
    }
}
