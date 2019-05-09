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

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using StardewValley.Objects;
using Microsoft.Xna.Framework;

namespace InteractionTweaks
{
    public class CarpenterMenuFeature : ModFeature
    {
        private const int moneyButtonID = 108;
        //between forward (left) and move (right)
        private static ClickableTextureComponent moneyButton;

        private static ClickableTextureComponent cancelTexture;

        private static bool moneyButtonEnabled;
        private static bool setupBlueprints;

        private static readonly Item woodObject = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, Object.wood, 1);
        private static readonly Item stoneObject = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, Object.stone, 1);

        private static List<BluePrint> vanillaBlueprints, modBlueprints, saleBlueprints;

        private static int remainingWoodReq;
        private static int remainingStoneReq;

        private static string hoverTextOn, hoverTextOff, ingredInfo;

        public static new void Enable()
        {
            //enabled = true;
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        public static new void Disable()
        {
            //enabled = false;
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        static void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu carpenterMenu)
            {
                if (Helper.ModRegistry.IsLoaded("Pathoschild.TractorMod"))
                {
                    Monitor.Log("TractorMod was detected.", LogLevel.Debug);
                    int stableCount = Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").GetValue().FindAll((BluePrint blueprint) => blueprint.name.Equals("Stable")).Count;
                    if (stableCount == 1)//garage not added
                    {
                        Monitor.Log("CarpenterMenu: TractorMod hasn't changed blueprints yet.", LogLevel.Trace);
                        setupBlueprints = false;
                    }
                    else
                    {
                        Monitor.Log("CarpenterMenu: TractorMod changed blueprints already.", LogLevel.Trace);
                        SetupBlueprints(carpenterMenu);
                    }
                }
                else
                {
                    SetupBlueprints(carpenterMenu);
                }

                SetupButtons(carpenterMenu);

                Helper.Events.Display.RenderingActiveMenu += Display_RenderingActiveMenu;
                Helper.Events.Display.RenderedActiveMenu += Display_RenderedActiveMenu;
                Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            }
            else if (e.OldMenu is CarpenterMenu oldCarpenterMenu)
            {
                Helper.Events.Display.RenderingActiveMenu -= Display_RenderingActiveMenu;
                Helper.Events.Display.RenderedActiveMenu -= Display_RenderedActiveMenu;
                Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
                Monitor.Log("CarpenterMenu: setting vanillaBlueprint", LogLevel.Trace);
                Helper.Reflection.GetField<List<BluePrint>>(oldCarpenterMenu, "blueprints").SetValue(vanillaBlueprints);
                oldCarpenterMenu.setNewActiveBlueprint();
            }
        }

        private static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!(Game1.activeClickableMenu is CarpenterMenu))
                return;
            CarpenterMenu carpenterMenu = (CarpenterMenu)Game1.activeClickableMenu;
            if (!setupBlueprints)
                SetupBlueprints(carpenterMenu);

            if (!e.Button.Equals(SButton.MouseLeft))
                return;

            Helper.Input.Suppress(e.Button);

            int X = Game1.getMouseX();
            int Y = Game1.getMouseY();

            bool onFarm = Helper.Reflection.GetField<bool>(carpenterMenu, "onFarm").GetValue();

            if (!onFarm && moneyButton.containsPoint(X, Y))
            {
                moneyButtonEnabled = !moneyButtonEnabled;
            }

            if (onFarm && moneyButtonEnabled)
            {
                bool demolishing = Helper.Reflection.GetField<bool>(carpenterMenu, "demolishing").GetValue();
                bool moving = Helper.Reflection.GetField<bool>(carpenterMenu, "moving").GetValue();
                if (demolishing || moving)
                {
                    moneyButtonEnabled = false;
                }
                else
                {
                    Monitor.Log("CarpenterMenu: setting saleBlueprint", LogLevel.Trace);
                    Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").SetValue(saleBlueprints);
                    carpenterMenu.setNewActiveBlueprint();
                }
            }

            Monitor.Log("carpenterMenu.receiveLeftClick", LogLevel.Trace);
            carpenterMenu.receiveLeftClick(X, Y);

            if (!onFarm && moneyButtonEnabled)
            {
                Monitor.Log("CarpenterMenu: setting modBlueprint", LogLevel.Trace);
                Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").SetValue(modBlueprints);
                carpenterMenu.setNewActiveBlueprint();
                int currentBlueprintIndex = Helper.Reflection.GetField<int>(carpenterMenu, "currentBlueprintIndex").GetValue();
                remainingWoodReq = GetRequired(vanillaBlueprints[currentBlueprintIndex], woodObject) - InventoryAmount(woodObject.Name);
                remainingStoneReq = GetRequired(vanillaBlueprints[currentBlueprintIndex], stoneObject) - InventoryAmount(stoneObject.Name);
            }
            if (!moneyButtonEnabled && setupBlueprints)
            {
                Monitor.Log("CarpenterMenu: setting vanillaBlueprint", LogLevel.Trace);
                Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").SetValue(vanillaBlueprints);
                carpenterMenu.setNewActiveBlueprint();
            }
        }

        private static void Display_RenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            if (!(Game1.activeClickableMenu is CarpenterMenu))
                return;
            CarpenterMenu carpenterMenu = (CarpenterMenu)Game1.activeClickableMenu;
            bool onFarm = Helper.Reflection.GetField<bool>(carpenterMenu, "onFarm").GetValue();
            if (onFarm)
                return;

            int X = Game1.getMouseX();
            int Y = Game1.getMouseY();

            moneyButton.tryHover(X, Y);
            if (moneyButton.containsPoint(X, Y))
            {
                Helper.Reflection.GetField<string>(carpenterMenu, "hoverText").SetValue(moneyButtonEnabled ? hoverTextOff : hoverTextOn);
            }
        }

        static void Display_RenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!(Game1.activeClickableMenu is CarpenterMenu))
                return;
            CarpenterMenu carpenterMenu = (CarpenterMenu)Game1.activeClickableMenu;
            bool onFarm = Helper.Reflection.GetField<bool>(carpenterMenu, "onFarm").GetValue();
            if (onFarm)
                return;

            moneyButton.draw(e.SpriteBatch);

            if (moneyButtonEnabled && setupBlueprints)
            {
                cancelTexture.draw(e.SpriteBatch, Color.Gray, 0.88f);

                Vector2 vector = new Vector2((float)(carpenterMenu.xPositionOnScreen + carpenterMenu.maxWidthOfDescription + 16 + 64), (float)(carpenterMenu.yPositionOnScreen + 256 + 32));
                vector.X -= 16f;
                vector.Y -= 21f;

                List<Item> ingredients = Helper.Reflection.GetField<List<Item>>(carpenterMenu, "ingredients").GetValue();
                bool magicalConstruction = Helper.Reflection.GetField<bool>(carpenterMenu, "magicalConstruction").GetValue();

                vector.Y += 68f * ingredients.Count;

                int currentBlueprintIndex = Helper.Reflection.GetField<int>(carpenterMenu, "currentBlueprintIndex").GetValue();
                Item ingredient;
                if (GetRequired(vanillaBlueprints[currentBlueprintIndex], woodObject) > 0)
                {
                    vector.Y += 68f;
                    ingredient = woodObject.getOne();
                    ingredient.Stack = remainingWoodReq;
                    ingredient.drawInMenu(e.SpriteBatch, vector, 1f);
                    Utility.drawTextWithShadow(e.SpriteBatch, ingredient.DisplayName + " " + ingredInfo, Game1.dialogueFont, new Vector2(vector.X + 64f + 16f, vector.Y + 20f), magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f, 3);
                    if (ingredient.Stack == 0)
                        Utility.drawTinyDigits(0, e.SpriteBatch, vector + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(0, 3f * 1f)) + 3f * 1f, 64f - 18f * 1f + 2f), 3f * 1f, 1f, Color.White);
                }

                if (GetRequired(vanillaBlueprints[currentBlueprintIndex], stoneObject) > 0)
                {
                    vector.Y += 68f;
                    ingredient = stoneObject.getOne();
                    ingredient.Stack = remainingStoneReq;
                    ingredient.drawInMenu(e.SpriteBatch, vector, 1f);
                    Utility.drawTextWithShadow(e.SpriteBatch, ingredient.DisplayName + " " + ingredInfo, Game1.dialogueFont, new Vector2(vector.X + 64f + 16f, vector.Y + 20f), magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f, 3);
                    if (ingredient.Stack == 0)
                        Utility.drawTinyDigits(0, e.SpriteBatch, vector + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(0, 3f * 1f)) + 3f * 1f, 64f - 18f * 1f + 2f), 3f * 1f, 1f, Color.White);
                }

            }

            //draw mosue and hover text above everything again
            carpenterMenu.drawMouse(e.SpriteBatch);
            string hoverText = Helper.Reflection.GetField<string>(carpenterMenu, "hoverText").GetValue();
            if (hoverText.Length > 0)
            {
                IClickableMenu.drawHoverText(e.SpriteBatch, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
        }

        private static void SetupButtons(CarpenterMenu carpenterMenu)
        {
            Monitor.Log("CarpenterMenu: Setting up buttons.", LogLevel.Trace);
            moneyButtonEnabled = false;
            hoverTextOn = Helper.Translation.Get("menu.carpentermoneybuttonon");
            hoverTextOff = Helper.Translation.Get("menu.carpentermoneybuttonoff");
            ingredInfo = Helper.Translation.Get("menu.carpenteringredinfo");

            cancelTexture = new ClickableTextureComponent("CMON", new Rectangle(carpenterMenu.xPositionOnScreen + carpenterMenu.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 - 20 - 64 - 10 + 5, carpenterMenu.yPositionOnScreen + carpenterMenu.maxHeightOfBuildingViewer + 64 + 5, 64 - 8, 64 - 8), null, hoverTextOn, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(267, 469, 16, 16), 3.0f, false);

            moneyButton = new ClickableTextureComponent("MON", new Rectangle(carpenterMenu.xPositionOnScreen + carpenterMenu.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 - 20 - 64 - 10, carpenterMenu.yPositionOnScreen + carpenterMenu.maxHeightOfBuildingViewer + 64, 64, 64), null, hoverTextOn, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(0, 384, 16, 16), 4f, false)
            {
                myID = moneyButtonID,
                leftNeighborID = CarpenterMenu.region_forwardButton,
                rightNeighborID = CarpenterMenu.region_moveBuitton,
                visible = true
            };
            carpenterMenu.forwardButton.rightNeighborID = moneyButtonID;
            carpenterMenu.moveButton.leftNeighborID = moneyButtonID;
        }

        private static void SetupBlueprints(CarpenterMenu carpenterMenu)
        {
            Monitor.Log("CarpenterMenu: Setting up blueprints.", LogLevel.Trace);
            setupBlueprints = true;

            vanillaBlueprints = new List<BluePrint>(Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").GetValue());
            modBlueprints = new List<BluePrint>(vanillaBlueprints);
            saleBlueprints = new List<BluePrint>(vanillaBlueprints);
            for (int i = 0; i < modBlueprints.Count; i++)
            {
                modBlueprints[i] = NewBlueprint(vanillaBlueprints[i]);
                saleBlueprints[i] = NewBlueprint(vanillaBlueprints[i]);
                modBlueprints[i].itemsRequired.Remove(woodObject.ParentSheetIndex);

                //modBlueprints[i].itemsRequired.Add(woodObject.ParentSheetIndex, 0);
                modBlueprints[i].itemsRequired.Remove(stoneObject.ParentSheetIndex);

                //modBlueprints[i].itemsRequired.Add(stoneObject.ParentSheetIndex, 0);
                modBlueprints[i].moneyRequired = GetPrice(vanillaBlueprints[i]);
                saleBlueprints[i].moneyRequired = GetPrice(vanillaBlueprints[i]);
            }
        }

        private static BluePrint NewBlueprint(BluePrint bluePrint)
        {
            if (bluePrint.name.Equals("Stable"))
            {
                BluePrint newBlueprint = new BluePrint(bluePrint.name)
                {
                    displayName = bluePrint.displayName,
                    description = bluePrint.description,
                    maxOccupants = bluePrint.maxOccupants,
                    moneyRequired = bluePrint.moneyRequired,
                    tilesWidth = bluePrint.tilesWidth,
                    tilesHeight = bluePrint.tilesHeight,
                    sourceRectForMenuView = bluePrint.sourceRectForMenuView,
                    itemsRequired = bluePrint.itemsRequired
                };
                return newBlueprint;
            }
            else
                return new BluePrint(bluePrint.name);
        }

        private static int GetPrice(BluePrint bluePrint)
        {
            int basePrice = bluePrint.moneyRequired;
            int remainingWoodRequired = GetRequired(bluePrint, woodObject) - InventoryAmount(woodObject.Name);
            remainingWoodRequired = remainingWoodRequired >= 0 ? remainingWoodRequired : 0;
            int remainingStoneRequired = GetRequired(bluePrint, stoneObject) - InventoryAmount(stoneObject.Name);
            remainingStoneRequired = remainingStoneRequired >= 0 ? remainingStoneRequired : 0;
            int totalPrice = basePrice + remainingWoodRequired * woodObject.salePrice() + remainingStoneRequired * stoneObject.salePrice();
            return totalPrice;
        }

        private static int GetRequired(BluePrint bluePrint, Item item)
        {
            if (bluePrint.itemsRequired.ContainsKey(item.ParentSheetIndex))
                return bluePrint.itemsRequired[item.ParentSheetIndex];
            else
                return 0;
        }

        private static int InventoryAmount(string name)
        {
            int amount = 0;
            foreach (Item item in Game1.player.Items)
                if (item != null && item.Name.Equals(name))
                    amount += item.Stack;
            return amount;
        }
    }
}
