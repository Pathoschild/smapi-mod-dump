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
            Composting.Init(conf);

            SeedMakerController.Init(helper);
            SeedMakerController.HeldItemRemoved += SeedMakerController_HeldItemRemoved;


            helper.Events.Display.RenderingHud += Display_RenderingHud;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;


            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;

            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;

            helper.Events.Display.MenuChanged += Display_MenuChanged;

            helper.Events.World.BuildingListChanged += World_BuildingListChanged;

            //helper.Events.Player.Warped += Player_Warped;

            Helper.ConsoleCommands.Add("cpc_clearcompost", "", (string arg1, string[] arg2) =>
            {
                Composting.CompostAppliedDays.Clear();
                Composting.ComposterDaysLeft.Clear();
                Composting.ComposterCompostLeft.Clear();
            });
            Helper.ConsoleCommands.Add("cpc_clearcomposterinv", "", (string arg1, string[] arg2) =>
            {
                Composting.ComposterContents.Clear();
            });
            Helper.ConsoleCommands.Add("cpc_clearpests", "", (string arg1, string[] arg2) =>
            {
                Pests.pests.Clear();
            });
            Helper.ConsoleCommands.Add("cpc_cleartraits", "", (string arg1, string[] arg2) =>
            {
                Cultivation.CropSeeds.Clear();
                Cultivation.CropTraits.Clear();
            });
        }

        void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            try
            {
                SaveData data = new SaveData();
                Pests.Save(data);
                Cultivation.Save(data);
                Composting.Save(data);
                ModEntry.GetHelper().Data.WriteSaveData<SaveData>(nameof(SaveData), data);
            }
            finally
            {
                Composting.ResetCompostingBins();
            }
        }

        void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            SaveData data = null;
            try
            {
                data = Helper.Data.ReadSaveData<SaveData>(nameof(SaveData));

            }
            finally
            {
                if (data == null)
                {
                    Monitor.Log($"No save data with key '{nameof(SaveData)}' found");
                    data = new SaveData();
                }
                data.InitNullValues();

                Composting.Load(data);
                Cultivation.Load(data);
                Pests.Load(data);
            }
        }

        void SeedMakerController_HeldItemRemoved(object sender, SeedMakerEventArgs e)
        {
            if (e.HeldItem != null && e.HeldItem.Category == Object.SeedsCategory)
            {
                Cultivation.NewSeeds(e.HeldItem.ParentSheetIndex);
            }
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
            if (Game1.currentLocation != null && Game1.activeClickableMenu == null && e.Button.IsActionButton())
            {
                if (Game1.currentLocation.getObjectAtTile((int)grabTile.X, (int)grabTile.Y) is Object obj && obj != null && obj.Name.Equals("Seed Maker"))
                {
                    if (obj.heldObject?.Value != null && obj.heldObject?.Value.Category == Object.SeedsCategory && obj.readyForHarvest && Game1.player.couldInventoryAcceptThisObject(obj.heldObject.Value.ParentSheetIndex, obj.heldObject.Value.Stack))
                    {
                        //Monitor.Log("Detected Seeds in Seed Maker", LogLevel.Alert);
                        //Cultivation.NewSeeds(obj.heldObject.Value.ParentSheetIndex); //TODO should be unneccessary now
                    }
                    else if (obj.heldObject?.Value == null && Game1.player.ActiveObject?.Category == Object.SeedsCategory)
                    {
                        if (Cultivation.GetCropItemFromSeeds(Game1.player.ActiveObject) is Object crop)
                            Game1.activeClickableMenu = new SeedsInfoMenu(Game1.player.ActiveObject, crop, false);
                    }
                }
                else if (Game1.currentLocation is Farm farm && farm.getBuildingAt(grabTile) is CompostingBin && Game1.player.ActiveObject?.Category == Object.SeedsCategory)
                {
                    if (Cultivation.GetCropItemFromSeeds(Game1.player.ActiveObject) is Object crop)
                        Game1.activeClickableMenu = new SeedsInfoMenu(Game1.player.ActiveObject, crop, true);
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
                Helper.Events.Input.ButtonPressed += CarpenterMenu_ButtonPressed;
                Composting.AddBlueprint(menu);
            }
            else if (e.OldMenu is CarpenterMenu oldmenu)
            {
                Helper.Events.Input.ButtonReleased -= CarpenterMenu_ButtonReleased;
                Helper.Events.Input.ButtonPressed -= CarpenterMenu_ButtonPressed;
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

        void World_BuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            if (Game1.activeClickableMenu is CarpenterMenu menu && Helper.Reflection.GetField<bool>(menu, "onFarm").GetValue() && Helper.Reflection.GetField<bool>(menu, "demolishing").GetValue())
                foreach (Building building in e.Removed)
                {
                    //Monitor.Log("Removed: " + building.GetType(), LogLevel.Error);
                    if (building is CompostingBin bin)
                        Composting.RemoveCompostingBin(bin);
                }
                
        }

        private bool clickedToSelect;
        private bool clickedToPlace;
        private ShippingBin binToMove;
        private Vector2 binOldPos;

        private void CarpenterMenu_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu is CarpenterMenu menu && e.Button.IsUseToolButton() && Helper.Reflection.GetField<bool>(menu, "onFarm").GetValue())
            {
                if (Helper.Reflection.GetField<bool>(menu, "moving").GetValue())
                {
                    if (Helper.Reflection.GetField<Building>(menu, "buildingToMove").GetValue() == null)
                        clickedToSelect = true;
                    else
                        clickedToPlace = true;
                }
            }
        }

        private void CarpenterMenu_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (Game1.activeClickableMenu is CarpenterMenu menu)
            {
                if (Composting.IsComposterBlueprint(menu.CurrentBlueprint))
                {
                    Helper.Reflection.GetField<Building>(menu, "currentBuilding").SetValue(new CompostingBin(menu.CurrentBlueprint, Vector2.Zero));
                }

                if (e.Button.IsUseToolButton() && Helper.Reflection.GetField<bool>(menu, "moving").GetValue() && Helper.Reflection.GetField<bool>(menu, "onFarm").GetValue())
                {
                    Building buildingToMove = Helper.Reflection.GetField<Building>(menu, "buildingToMove").GetValue();

                    if (buildingToMove != null && clickedToSelect)
                    {
                        //Monitor.Log("Building selected?", LogLevel.Error);
                        if (buildingToMove is CompostingBin bin)
                        {
                            binOldPos = new Vector2(bin.tileX, bin.tileY);
                            binToMove = bin;
                        }
                    }
                    else if (buildingToMove == null && clickedToPlace)
                    {
                        //Monitor.Log("Building placed?", LogLevel.Error);
                         if (binToMove != null)
                        {
                            Composting.MoveCompostingBin(binOldPos, new Vector2(binToMove.tileX, binToMove.tileY));
                            binToMove = null;
                        }
                    }
                }
            }

            clickedToSelect = false;
            clickedToPlace = false;
        }

        private static IMonitor _Monitor;
        private static IModHelper _Helper;

        public static IMonitor GetMonitor() => _Monitor;
        public static IModHelper GetHelper() => _Helper;

    }


}

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


//Greenhouse:
//Quality level base is 0
//Speed level base is 1
//Resistance ignored
//Water base is 1

//Wild Seed Crops are can not gain traits but also have no negative effects and are resistant against pests.
//Mixed Seeds can not gain traits
