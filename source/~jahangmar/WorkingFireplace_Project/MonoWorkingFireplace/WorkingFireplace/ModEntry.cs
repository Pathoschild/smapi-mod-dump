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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Characters;
using StardewValley.BellsAndWhistles;

//using StardewValley.Menus;
//using System.Collections.Generic;

namespace WorkingFireplace
{
    public class ModEntry : Mod
    {
        private WorkingFireplaceConfig Config;

        private const double defaultYesterdayCOFLow = 1000;
        private double yesterdayCOFLow = defaultYesterdayCOFLow;
        private bool tooColdToday = false;
        private double tempToday = defaultYesterdayCOFLow;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<WorkingFireplaceConfig>();

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is StardewValley.Menus.DialogueBox && Game1.player.isInBed && Config.show_temperature_in_bed)
            {
                Helper.Events.Display.RenderedActiveMenu += Display_RenderedActiveMenu;
            }
            else
            {
                Helper.Events.Display.RenderedActiveMenu -= Display_RenderedActiveMenu;
            }
        }

        void Display_RenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            //from Game1.drawHUD
            float num = 0.625f;
            Rectangle rectangle = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            float x = (float)(rectangle.Right - 48 - 8);
            rectangle = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            Vector2 vector = new Vector2(x, (float)(rectangle.Bottom - 224 - 16 - (int)((float)(Game1.player.MaxStamina - 270) * num)));

            bool isCold = !WarmInside(false) && tooColdToday;

            string text = Helper.Translation.Get("msg.temp") + ((isCold ? Helper.Translation.Get("msg.tempcold") : Helper.Translation.Get("msg.tempwarm")));

            int width = SpriteText.getWidthOfString(text);
            SpriteText.drawString(e.SpriteBatch, text, (int)vector.X - width, (int)vector.Y - 100, 999999, -1, 999999, 1f, 0.88f, false, -1, "", isCold ? SpriteText.color_Cyan : SpriteText.color_Orange);
        }


        void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {

            SetTemperatureToday();

            bool warmth = WarmInside(true);


            bool tooColdOutside = TooColdYesterday();

            if (tooColdOutside)
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

        private bool WarmInside(bool changeFireplace)
        {
            bool warmth = false;
            if (Game1.currentLocation is FarmHouse farmHouse)
            {
                foreach (Furniture furniture in farmHouse.furniture)
                {
                    if (furniture.furniture_type == Furniture.fireplace && furniture.isOn)
                    {
                        Point tile = VectorToPoint(furniture.tileLocation.Get());
                        if (changeFireplace)
                            SetFireplace(farmHouse, tile.X, tile.Y, false, false);
                        warmth = true;
                    }
                }
            }
            else
            {
                warmth = true; //if the player sleeps somewhere else (Sleepover mod)
            }
            return warmth;
        }

        private bool TooColdYesterday()
        {
            bool tooColdOutside = false;
            if (Config.COFIntegration && Helper.ModRegistry.IsLoaded("KoihimeNakamura.ClimatesOfFerngill")) //check if ClimatesOfFerngill is loaded and integration is activated
            {
                double todayCOFLow = Helper.Reflection.GetMethod(Helper.ModRegistry.GetApi("KoihimeNakamura.ClimatesOfFerngill"), "GetTodaysLow").Invoke<double>(Array.Empty<object>());
                tooColdOutside = (((int)yesterdayCOFLow == (int)defaultYesterdayCOFLow) ? todayCOFLow : yesterdayCOFLow) - (Game1.wasRainingYesterday ? Config.COFRainImpact : 0) <= Config.COFMinTemp;
                Monitor.Log("Climates of Ferngill integration is active. Temperature is " + (tooColdOutside ? "cold" : "warm"), LogLevel.Trace);
                yesterdayCOFLow = todayCOFLow;
                Monitor.Log("yesterdayCOFLow set to " + yesterdayCOFLow, LogLevel.Trace);

            }
            else
            {
                tooColdOutside = Game1.IsWinter && (Config.need_fire_in_winter || Game1.wasRainingYesterday && Config.need_fire_in_winter_rain) ||
                                 Game1.IsSpring && (Config.need_fire_in_spring || Game1.wasRainingYesterday && Config.need_fire_in_spring_rain) ||
                                 Game1.IsSummer && (Config.need_fire_in_summer || Game1.wasRainingYesterday && Config.need_fire_in_summer_rain) ||
                                 Game1.IsFall && (Config.need_fire_in_fall || Game1.wasRainingYesterday && Config.need_fire_in_fall_rain);

                Monitor.Log("Temperature is " + (tooColdOutside ? "cold" : "warm"), LogLevel.Trace);
            }
            return tooColdOutside;
        }

        private void SetTemperatureToday()
        {
            bool tooColdOutside = false;
            if (Config.COFIntegration && Helper.ModRegistry.IsLoaded("KoihimeNakamura.ClimatesOfFerngill")) //check if ClimatesOfFerngill is loaded and integration is activated
            {
                double todayCOFLow = Helper.Reflection.GetMethod(Helper.ModRegistry.GetApi("KoihimeNakamura.ClimatesOfFerngill"), "GetTodaysLow").Invoke<double>(Array.Empty<object>());
                tooColdOutside = todayCOFLow - (Game1.isRaining ? Config.COFRainImpact : 0) <= Config.COFMinTemp;
                tempToday = todayCOFLow;

            }
            else
            {
                tooColdOutside = Game1.IsWinter && (Config.need_fire_in_winter || Game1.isRaining && Config.need_fire_in_winter_rain) ||
                                 Game1.IsSpring && (Config.need_fire_in_spring || Game1.isRaining && Config.need_fire_in_spring_rain) ||
                                 Game1.IsSummer && (Config.need_fire_in_summer || Game1.isRaining && Config.need_fire_in_summer_rain) ||
                                 Game1.IsFall && (Config.need_fire_in_fall || Game1.isRaining && Config.need_fire_in_fall_rain);
            }
            tooColdToday = tooColdOutside;
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
                    if (!on && furniture.lightSource != null)
                    {
                        farmHouse.removeLightSource(furniture.lightSource.Identifier);
                    }
                }
            }
        }

        private Point VectorToPoint(Vector2 vec)
        {
            return new Point(Convert.ToInt32(vec.X), Convert.ToInt32(vec.Y));
        }

    }
}
