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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Characters;

//using StardewValley.Menus;
//using System.Collections.Generic;

namespace WorkingFireplace
{
    public class ModEntry : Mod
    {
        private WorkingFireplaceConfig Config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
           
            Config = helper.ReadConfig<WorkingFireplaceConfig>();
        }

        void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            bool warmth = false;
            if (Game1.currentLocation is FarmHouse farmHouse)
            {
                foreach (Furniture furniture in farmHouse.furniture)
                {
                    if (furniture.furniture_type == Furniture.fireplace && furniture.isOn) {
                        Point tile = VectorToPoint(furniture.tileLocation.Get());
                        SetFireplace(farmHouse, tile.X, tile.Y, false, false);
                        warmth = true;
                    }
                }
            }
            if (Game1.IsWinter && Config.need_fire_in_winter || Game1.wasRainingYesterday && Config.need_fire_on_rainy_day)
            {
                if (warmth)
                {
                    if (Config.showMessageOnStartOfDay)
                        Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("msg.warm"), ""));
                }
                else
                {
                    if (Config.showMessageOnStartOfDay)
                    {
                        if (HasSpouse())
                            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("msg.spousecold"), ""));
                        else
                            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("msg.cold"), ""));
                    }
                    Game1.currentLocation.playSound("coldSpell");

                    if (Config.penalty)
                    {
                        Game1.player.health = CalcAttribute(Game1.player.health, Config.reduce_health, Game1.player.maxHealth);
                        Game1.player.stamina = CalcAttribute(Game1.player.stamina, Config.reduce_stamina, Game1.player.maxStamina);
                        if (HasSpouse())
                            Game1.player.changeFriendship(-Config.reduce_friendship_spouse, GetSpouse());
                        Game1.player.getChildren().ForEach((child) => Game1.player.changeFriendship(-Config.reduce_friendship_children, child));
                    }

                    if (HasSpouse())
                    {
                        string please = Helper.Translation.Get("dia.please");
                        switch (Game1.player.getChildrenCount())
                        {
                            case 1:
                                Child child = Game1.player.getChildren()[0];
                                GetSpouse().setNewDialogue("$2" + Helper.Translation.Get("dia.spousecold_child", new { child1 = child.Name }) + " " + please, true);
                                break;
                            case 2:
                                Child child1 = Game1.player.getChildren()[0];
                                Child child2 = Game1.player.getChildren()[1];
                                GetSpouse().setNewDialogue("$2" + Helper.Translation.Get("dia.spousecold_children", new { child1 = child1.Name, child2 = child2.Name }) + " " + please, true);
                                break;
                            default:
                                GetSpouse().setNewDialogue("$2" + Helper.Translation.Get("dia.spousecold") + " " + please, true);
                                break;
                        }
                    }

                }
            }
        }

        private bool HasSpouse() => GetSpouse() != null;
        private NPC GetSpouse() => Game1.player.getSpouse();

        void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            Point grabtile = VectorToPoint(e.Cursor.GrabTile);

            if (Game1.currentLocation is FarmHouse farmHouse1 &&
                e.Button.IsUseToolButton() && e.IsDown(e.Button))
            {
                //the fireplace is moved. We want to turn it off to avoid floating flames.
                Point tile = VectorToPoint(e.Cursor.Tile);
                SetFireplace(farmHouse1, tile.X, tile.Y, false, true);
            }
            else if (Game1.currentLocation is FarmHouse farmHouse &&
                e.Button.IsActionButton() && e.IsDown(e.Button) &&
                farmHouse.getObjectAtTile(grabtile.X, grabtile.Y) is Furniture furniture &&
                furniture.furniture_type == Furniture.fireplace)
            {
                Helper.Input.Suppress(e.Button);

                if (!furniture.isOn)
                {
                    Item item = Game1.player.CurrentItem;
                    if (item != null && item.Name == "Wood" && item.getStack() >= Config.wood_pieces)
                    {
                        Game1.player.removeItemsFromInventory(item.ParentSheetIndex, Config.wood_pieces);
                        SetFireplace(farmHouse, grabtile.X, grabtile.Y, true);
                        return;
                    }
                    Game1.showRedMessage(Helper.Translation.Get("msg.nowood", new { Config.wood_pieces }));
                }
            }
        }

        private int CalcAttribute(float value, double fac, int max)
        {
            int result = Convert.ToInt32(value - max * fac);

            if (result > max)
                return max;
            else if (result <= 0)
                return 1;
            else
                return result;
        }

        /// <summary>
        /// Checks if the given position matches a fireplace.
        /// Toggles the fireplace on or off if its state differs from <c>on</c>./// 
        /// </summary>
        /// <param name="farmHouse">Farm house.</param>
        /// <param name="X">X tile position of fireplace.</param>
        /// <param name="Y">Y tile position of fireplace.</param>
        /// <param name="on">new state of fireplace.</param>
        /// <param name="playsound">should a sound be played?</param>
        private void SetFireplace(FarmHouse farmHouse, int X, int Y, bool on, bool playsound = true)
        {
            if (farmHouse.getObjectAtTile(X, Y) is Furniture furniture && furniture.furniture_type == Furniture.fireplace)
            {
                //fireplaces are two tiles wide. The "FarmHouse.setFireplace" method needs the left tile so we set it to the left one.
                if (farmHouse.getObjectAtTile(X-1, Y) == furniture)
                {
                    X = X - 1;
                }
                if (furniture.isOn.Get() != on)
                {
                    furniture.isOn.Set(on);
                    farmHouse.setFireplace(on, X, Y, playsound);
                }
            }
        }

        private Point VectorToPoint(Vector2 vec)
        {
            return new Point(Convert.ToInt32(vec.X), Convert.ToInt32(vec.Y));
        }

    }
}
