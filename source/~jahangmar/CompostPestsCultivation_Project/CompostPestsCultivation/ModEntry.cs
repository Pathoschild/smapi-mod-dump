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

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System.Linq;
using StardewModdingAPI.Events;
//using System;

namespace CompostPestsCultivation
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            _Monitor = Monitor;
            _Helper = helper;

            Config conf = helper.ReadConfig<Config>();
            Pests.Init(conf);
            Cultivation.Init(conf);

            helper.Events.Display.RenderingHud += Display_RenderingHud;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;


            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;

            helper.Events.Display.MenuChanged += Display_MenuChanged;

            //helper.Events.Player.Warped += Player_Warped;
        }

        void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            Pests.Save();
            Cultivation.Save();
            Composting.Save();
        }

        void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            Composting.Load();
            Cultivation.Load();
            Pests.Load();

            /*
            for (int i = 0; i < 10; i++)
                Helper.ConsoleCommands.Trigger("player_add", new string[2] { "name", "Seed Maker" });
            Helper.ConsoleCommands.Trigger("player_add", new string[3] { "name", "Melon", "100" });

            */           
        }

        void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            Vector2 grabTile = e.Cursor.GrabTile;
            //Monitor.Log($"obj: {Game1.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y)?.Name}");
            //Object obj2 = Game1.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y);
            //Monitor.Log("1" + (obj2 is Object && obj2 != null));
            //Monitor.Log("2" + (e.Button.IsActionButton()));
            //Monitor.Log("3" + obj2.Name.Equals("Seed Maker"));
            //Monitor.Log("4" + (obj2.heldObject?.Value != null));
            //Monitor.Log("5" + obj2.heldObject.Value.CanBeGrabbed);
            if (Game1.currentLocation != null && Game1.activeClickableMenu == null && Game1.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y) is Object obj && obj != null &&
            e.Button.IsActionButton() && obj.Name.Equals("Seed Maker"))
            {
                if (obj.heldObject?.Value != null && obj.heldObject?.Value.Category == Object.SeedsCategory && obj.readyForHarvest && Game1.player.couldInventoryAcceptThisObject(obj.heldObject.Value.ParentSheetIndex, obj.heldObject.Value.Stack))
                {
                    //Monitor.Log("Detected Seeds in Seed Maker", LogLevel.Alert);
                    Cultivation.NewSeeds(obj.heldObject.Value.ParentSheetIndex);
                }
                else if (obj.heldObject?.Value == null && Game1.player.ActiveObject?.Category == Object.SeedsCategory)
                {
                    if (Cultivation.GetCropItemFromSeeds(Game1.player.ActiveObject) is Object crop)
                        Game1.activeClickableMenu = new SeedsInfoMenu(Game1.player.ActiveObject, crop);
                }
            }

            /*if (e.Button.IsActionButton() && Game1.currentLocation is Farm farm && Game1.activeClickableMenu == null && Composting.IsComposter(farm.getBuildingAt(grabTile)))
            {
                Game1.activeClickableMenu = new ComposterMenu();
            }*/
        }

        void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            Cultivation.OnEndDay();
        }


        void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            Composting.OnNewDay();
            Pests.OnNewDay();
            Cultivation.OnNewDay();
        }


        void Display_RenderingHud(object sender, StardewModdingAPI.Events.RenderingHudEventArgs e)
        {
            Pests.DrawPests(e.SpriteBatch);
        }

        void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu menu)
            {
                Helper.Events.Input.ButtonReleased += CarpenterMenu_ButtonReleased;
                Composting.AddBlueprint(menu);
            }
            else if (e.OldMenu is CarpenterMenu oldmenu)
            {
                Helper.Events.Input.ButtonReleased -= CarpenterMenu_ButtonReleased;
                Game1.getFarm().buildings.Set(new List<Building>(Game1.getFarm().buildings).Select((Building building) => building is ShippingBin bin && Composting.IsComposter(bin) ? CompostingBin.FromShippingBin(bin) : building).ToList());
            }

            if (e.NewMenu is ComposterMenu compMenu)
            {

            }
            else if (e.OldMenu is ComposterMenu oldCompMenu)
            {
                oldCompMenu.SaveCompostItems();
            }

        }

        private void CarpenterMenu_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (Game1.activeClickableMenu is CarpenterMenu menu && Composting.IsComposterBlueprint(menu.CurrentBlueprint))
            {
                Helper.Reflection.GetField<Building>(menu, "currentBuilding").SetValue(new CompostingBin(menu.CurrentBlueprint, Vector2.Zero));
            }
        }

        private static IMonitor _Monitor;
        private static IModHelper _Helper;

        public static IMonitor GetMonitor() => _Monitor;
        public static IModHelper GetHelper() => _Helper;

    }


}

//TODO test loading/saving
//TODO: Test Effects of Cultivation system

//TODO: Implement whole Compost menu and applying
    //menu has mode similar to carpentermenu building moving to apply compost
//TODO: Rotten plant item is usefull for compost or ...

//Effects
//0: normal Quality
//1: better Quality
//2: best   Quality

//reduced Speed
//normal  Speed
//inc     Speed
//best    Speed

//no      Pest Res
//better  Pest Res
//full    Pest Res

//dry without water
//normal Water
//staying watered 1
//staying watered 2
