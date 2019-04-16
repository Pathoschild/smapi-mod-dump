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
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

namespace InteractionTweaks
{
    public class ModCarpenterMenu : CarpenterMenu
    {
        private IMonitor Monitor;
        private IModHelper Helper;

        private const int moneyButtonID = 108;
        //between forward (left) and move (right)
        private ClickableTextureComponent moneyButton;

        private ClickableTextureComponent cancelTexture;

        private bool moneyButtonEnabled;

        private readonly Item woodObject = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, Object.wood, 1);
        private readonly Item stoneObject = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, Object.stone, 1);

        private List<BluePrint> vanillaBlueprints, modBlueprints, saleBlueprints;

        private int remainingWoodReq;
        private int remainingStoneReq;

        private readonly string hoverTextOn, hoverTextOff, ingredInfo;

        public ModCarpenterMenu(IMonitor monitor, IModHelper helper, bool magicalConstruction) : base(magicalConstruction)
        {
            Monitor = monitor;
            Helper = helper;

            hoverTextOn = Helper.Translation.Get("menu.carpentermoneybuttonon");
            hoverTextOff = Helper.Translation.Get("menu.carpentermoneybuttonoff");
            ingredInfo = Helper.Translation.Get("menu.carpenteringredinfo");

            cancelTexture = new ClickableTextureComponent("CMON", new Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 - 20 - 64 - 10 + 5, yPositionOnScreen + maxHeightOfBuildingViewer + 64 + 5, 64 - 8, 64 - 8), null, hoverTextOn, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(267, 469, 16, 16), 3.0f, false);

            moneyButton = new ClickableTextureComponent("MON", new Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 256 - 20 - 64 - 10, yPositionOnScreen + maxHeightOfBuildingViewer + 64, 64, 64), null, hoverTextOn, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(0, 384, 16, 16), 4f, false)
            {
                myID = moneyButtonID,
                leftNeighborID = region_forwardButton,
                rightNeighborID = region_moveBuitton,
                visible = true
            };
            forwardButton.rightNeighborID = moneyButtonID;
            moveButton.leftNeighborID = moneyButtonID;

            vanillaBlueprints = new List<BluePrint>(Helper.Reflection.GetField<List<BluePrint>>(this, "blueprints").GetValue());
            modBlueprints = new List<BluePrint>(vanillaBlueprints);
            saleBlueprints = new List<BluePrint>(vanillaBlueprints);
            for (int i = 0; i < modBlueprints.Count; i++)
            {
                modBlueprints[i] = new BluePrint(vanillaBlueprints[i].name);
                saleBlueprints[i] = new BluePrint(vanillaBlueprints[i].name);
                modBlueprints[i].itemsRequired.Remove(woodObject.ParentSheetIndex);

                //modBlueprints[i].itemsRequired.Add(woodObject.ParentSheetIndex, 0);
                modBlueprints[i].itemsRequired.Remove(stoneObject.ParentSheetIndex);

                //modBlueprints[i].itemsRequired.Add(stoneObject.ParentSheetIndex, 0);
                modBlueprints[i].moneyRequired = GetPrice(vanillaBlueprints[i]);
                saleBlueprints[i].moneyRequired = GetPrice(vanillaBlueprints[i]);
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            bool onFarm = Helper.Reflection.GetField<bool>(this, "onFarm").GetValue();
            if (onFarm)
            {
                return;
            }

            moneyButton.draw(b);


            if (moneyButtonEnabled)
            {
                cancelTexture.draw(b, Color.Gray, 0.88f);

                Vector2 vector = new Vector2((float)(xPositionOnScreen + maxWidthOfDescription + 16 + 64), (float)(yPositionOnScreen + 256 + 32));
                vector.X -= 16f;
                vector.Y -= 21f;

                List<Item> ingredients = Helper.Reflection.GetField<List<Item>>(this, "ingredients").GetValue();
                bool magicalConstruction = Helper.Reflection.GetField<bool>(this, "magicalConstruction").GetValue();
                //ingredients = CurrentBlueprint.

                vector.Y += 68f * ingredients.Count;

                int currentBlueprintIndex = Helper.Reflection.GetField<int>(this, "currentBlueprintIndex").GetValue();
                Item ingredient;
                if (GetRequired(vanillaBlueprints[currentBlueprintIndex], woodObject) > 0)
                {
                    vector.Y += 68f;
                    ingredient = woodObject.getOne();
                    ingredient.Stack = remainingWoodReq;
                    ingredient.drawInMenu(b, vector, 1f);
                    Utility.drawTextWithShadow(b, ingredient.DisplayName + " " + ingredInfo, Game1.dialogueFont, new Vector2(vector.X + 64f + 16f, vector.Y + 20f), magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f, 3);
                    if (ingredient.Stack == 0)
                        Utility.drawTinyDigits(0, b, vector + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(0, 3f * 1f)) + 3f * 1f, 64f - 18f * 1f + 2f), 3f * 1f, 1f, Color.White);
                }

                if (GetRequired(vanillaBlueprints[currentBlueprintIndex], stoneObject) > 0)
                {
                    vector.Y += 68f;
                    ingredient = stoneObject.getOne();
                    ingredient.Stack = remainingStoneReq;
                    ingredient.drawInMenu(b, vector, 1f);
                    Utility.drawTextWithShadow(b, ingredient.DisplayName + " " + ingredInfo, Game1.dialogueFont, new Vector2(vector.X + 64f + 16f, vector.Y + 20f), magicalConstruction ? Color.PaleGoldenrod : Game1.textColor, 1f, -1f, -1, -1, magicalConstruction ? 0f : 0.25f, 3);
                    if (ingredient.Stack == 0)
                        Utility.drawTinyDigits(0, b, vector + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(0, 3f * 1f)) + 3f * 1f, 64f - 18f * 1f + 2f), 3f * 1f, 1f, Color.White);
                }

            }

            drawMouse(b);
            string hoverText = Helper.Reflection.GetField<string>(this, "hoverText").GetValue();
            if (hoverText.Length > 0)
            {
                IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }

        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            bool onFarm = Helper.Reflection.GetField<bool>(this, "onFarm").GetValue();
            if (onFarm)
                return;

            moneyButton.tryHover(x, y);
            if (moneyButton.containsPoint(x, y))
            {
                Helper.Reflection.GetField<string>(this, "hoverText").SetValue(moneyButtonEnabled ? hoverTextOff : hoverTextOn);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            bool onFarm = Helper.Reflection.GetField<bool>(this, "onFarm").GetValue();

            if (!onFarm && moneyButton.containsPoint(x, y))
                moneyButtonEnabled = !moneyButtonEnabled;

            int currentBlueprintIndex = Helper.Reflection.GetField<int>(this, "currentBlueprintIndex").GetValue();

            if (onFarm && moneyButtonEnabled)
            {
                bool demolishing = Helper.Reflection.GetField<bool>(this, "demolishing").GetValue();
                bool moving = Helper.Reflection.GetField<bool>(this, "moving").GetValue();
                if (demolishing || moving)
                {
                    moneyButtonEnabled = false;
                }
                else
                {
                    //set blueprints
                    Monitor.Log("setting saleBlueprint", LogLevel.Trace);
                    Helper.Reflection.GetField<List<BluePrint>>(this, "blueprints").SetValue(saleBlueprints);
                    setNewActiveBlueprint();
                    //Monitor.Log("Money required: " + CurrentBlueprint.moneyRequired, LogLevel.Trace);
                    //Monitor.Log("Wood required: " + GetRequired(CurrentBlueprint, woodObject), LogLevel.Trace);
                    //moneyButtonEnabled = false;
                }
            }


            //Monitor.Log("I.  Money required: " + CurrentBlueprint.moneyRequired, LogLevel.Trace);
            //Monitor.Log("I.  Wood required: " + GetRequired(CurrentBlueprint, woodObject), LogLevel.Trace);
            Monitor.Log("base.receiveLeftClick", LogLevel.Trace);
            base.receiveLeftClick(x, y, playSound);
            //Monitor.Log("II. Money required: " + CurrentBlueprint.moneyRequired, LogLevel.Trace);
            //Monitor.Log("II. Wood required: " + GetRequired(CurrentBlueprint, woodObject), LogLevel.Trace);

            if (!onFarm && moneyButtonEnabled)
            {
                Monitor.Log("setting modBlueprint", LogLevel.Trace);
                Helper.Reflection.GetField<List<BluePrint>>(this, "blueprints").SetValue(modBlueprints);
                setNewActiveBlueprint();
                remainingWoodReq = GetRequired(vanillaBlueprints[currentBlueprintIndex], woodObject) - InventoryAmount(woodObject.Name);
                remainingStoneReq = GetRequired(vanillaBlueprints[currentBlueprintIndex], stoneObject) - InventoryAmount(stoneObject.Name);
            }
            if (!moneyButtonEnabled)
            {
                Monitor.Log("setting vanillaBlueprint", LogLevel.Trace);
                Helper.Reflection.GetField<List<BluePrint>>(this, "blueprints").SetValue(vanillaBlueprints);
                setNewActiveBlueprint();
            }
        }

        private int GetPrice(BluePrint bluePrint)
        {
            int basePrice = bluePrint.moneyRequired;
            int remainingWoodRequired = GetRequired(bluePrint, woodObject) - InventoryAmount(woodObject.Name);
            remainingWoodRequired = remainingWoodRequired >= 0 ? remainingWoodRequired : 0;
            int remainingStoneRequired = GetRequired(bluePrint, stoneObject) - InventoryAmount(stoneObject.Name);
            remainingStoneRequired = remainingStoneRequired >= 0 ? remainingStoneRequired : 0;
            int totalPrice = basePrice + remainingWoodRequired * woodObject.salePrice() + remainingStoneRequired * stoneObject.salePrice();
            return totalPrice;
        }

        private int GetRequired(BluePrint bluePrint, Item item)
        {
            if (bluePrint.itemsRequired.ContainsKey(item.ParentSheetIndex))
                return bluePrint.itemsRequired[item.ParentSheetIndex];
            else
                return 0;
        }

        private int InventoryAmount(string name)
        {
            int amount = 0;
            foreach (Item item in Game1.player.Items)
                if (item != null && item.Name.Equals(name))
                    amount += item.Stack;
            return amount;
        }
    }
}
