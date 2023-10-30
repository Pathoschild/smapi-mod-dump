/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;
using Newtonsoft.Json;

namespace StardewSurvivalProject.source.harmony_patches
{
    class UIDrawPatches
    {
        private static IMonitor Monitor = null;
        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void DrawHoverText_Postfix(IClickableMenu __instance, SpriteBatch b, StringBuilder text, SpriteFont font, int xOffset, int yOffset, int moneyAmountToDisplayAtBottom, string boldTitleText, int healAmountToDisplay, string[] buffIconsToDisplay, ref Item hoveredItem, int currencySymbol, int extraItemToShowIndex, int extraItemToShowAmount, int overrideX, int overrideY, float alpha, CraftingRecipe craftingIngredients, IList<Item> additional_craft_materials)
        {
            if (ModConfig.GetInstance().DisableModItemInfo) return;
            try
            {
                double addThirst = 0;
                double addHunger = 0;
                double heatResistant = 0;
                double coldResistant = 0;
                double coreTemp = -274;
                string deviceType = "general";
                double effectiveRange = 0;
                double ambientCoefficient = 1;
                double operationalRange = 0;

                data.ClothingTempResistantData tempResistData = null;
                data.TempControlObject tempControlData = null;
                if (hoveredItem is SObject)
                {
                    //LogHelper.Debug("bigcraftable");
                    tempControlData = data.TempControlObjectDictionary.GetTempControlData(hoveredItem.Name);

                    //edible and drinkable data
                    addThirst = data.CustomHydrationDictionary.getHydrationValue(hoveredItem.Name);
                    int edibility = ((SObject)hoveredItem).Edibility;
                    addHunger = (edibility >= 0) ? edibility * ModConfig.GetInstance().HungerGainMultiplierFromItemEdibility : 0;

                    //band-aid fix coming, if edibility is 1 and healing value is not 0, dont add hunger
                    //TODO: document this weird anomaly
                    if (data.HealingItemDictionary.getHealingValue(hoveredItem.Name) > 0 && edibility == 1) addHunger = 0;

                    //FIXME: not all object info is in objectInformation dict
                    string[] arrInfo = { };
                    if (Game1.objectInformation.ContainsKey(hoveredItem.ParentSheetIndex))
                    {
                        arrInfo = Game1.objectInformation[hoveredItem.ParentSheetIndex].Split('/');
                        if (arrInfo.Length > 6)
                        {
                            if (arrInfo[6].Equals("drink") && addThirst == 0)
                            {
                                addThirst = ModConfig.GetInstance().DefaultHydrationGainOnDrinkableItems;
                            }
                        }
                    }

                }
                else if (hoveredItem is StardewValley.Objects.Furniture)
                {
                    LogHelper.Debug("furniture");
                    tempControlData = data.TempControlObjectDictionary.GetTempControlData(hoveredItem.DisplayName);
                }
                else if (hoveredItem is StardewValley.Objects.Clothing)
                {
                    //0 for shirt, 1 for pants

                    StardewValley.Objects.Clothing clothingInfo = (StardewValley.Objects.Clothing)hoveredItem;
                    //using DisplayName instead of Name, may break on other languages usage
                    if (clothingInfo.clothesType.Value == 0)
                        tempResistData = getClothingTempResistInfo(hoveredItem.DisplayName, "shirt");
                    else if (clothingInfo.clothesType.Value == 1)
                        tempResistData = getClothingTempResistInfo(hoveredItem.DisplayName, "pants");
                }
                else if (hoveredItem is StardewValley.Objects.Hat)
                {
                    tempResistData = getClothingTempResistInfo(hoveredItem.Name, "hat");

                }
                else if (hoveredItem is StardewValley.Objects.Boots)
                {
                    tempResistData = getClothingTempResistInfo(hoveredItem.Name, "boots");
                }

                if (tempResistData != null)
                {
                    heatResistant = tempResistData.heatInsulationModifier * 100;
                    coldResistant = tempResistData.coldInsulationModifier * 100;
                }
                if (tempControlData != null)
                {
                    coreTemp = tempControlData.coreTemp;
                    deviceType = tempControlData.deviceType;
                    effectiveRange = tempControlData.effectiveRange;
                    ambientCoefficient = tempControlData.ambientCoefficient;
                    operationalRange = tempControlData.operationalRange;
                }

                //draw the UI
                int x = Game1.getOldMouseX() + 32 + xOffset;
                int y4 = Game1.getOldMouseY() + 32 + yOffset;

                //need testing on bigger UI scale
                if (overrideX != -1)
                {
                    x = overrideX;
                }
                if (overrideY != -1)
                {
                    y4 = overrideY;
                }
                if (x + 256 > Utility.getSafeArea().Right)
                {
                    x = Utility.getSafeArea().Right - 256;
                }
                //FIXME: redo this hardcode fest jesus chirst
                int UIHeight = 0;
                int UIWidth = 0;
                if (addThirst > 0 || addHunger > 0) { UIHeight = 64; UIWidth = 256; }
                if (addHunger > 0 && addThirst > 0) { UIHeight += 40; }
                
                //temporary fit
                if (heatResistant != 0 || coldResistant != 0) { UIHeight = 64; UIWidth = 296; }
                if (heatResistant != 0 && coldResistant != 0) { UIHeight += 40; }

                //more temporary fix
                if (coreTemp > -274) { 
                    UIHeight = 104; UIWidth = 168;
                    //even more temporary fix
                    if (operationalRange != 0) { UIHeight = 104; UIWidth = 260; }
                    if (ambientCoefficient != 1) { UIHeight = 104; UIWidth = 300; }
                }
                Rectangle iconTarget = new Rectangle(70, 0, 10, 10);
                if (deviceType.Equals("heating")) iconTarget = new Rectangle(50, 0, 10, 10);
                if (deviceType.Equals("cooling")) iconTarget = new Rectangle(60, 0, 10, 10);

                

                y4 -= UIHeight + 16;
                if (UIHeight > 0)
                {
                    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y4, UIWidth, UIHeight, Color.White * alpha);
                    Utility.drawWithShadow(b, ModEntry.ModIcon, new Vector2(x + UIWidth, y4), new Rectangle(0, 0, 16, 16), Color.White, 0f, new Vector2(7, 7), 3f, flipped: false, 0.95f);
                }
                int startX = x + 18;
                int startY = y4 + 20;
                if (addThirst > 0)
                {
                    string thirstText = $"+{addThirst} Hydration";
                    Utility.drawWithShadow(b, ModEntry.InfoIcon, new Vector2(startX, startY), new Rectangle(0, 0, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                    Utility.drawTextWithShadow(b, thirstText, font, new Vector2(startX + 34, startY), Game1.textColor);
                    startY += 36;
                }
                if (addHunger > 0)
                {
                    string hungerText = $"+{addHunger} Hunger";
                    Utility.drawWithShadow(b, ModEntry.InfoIcon, new Vector2(startX, startY), new Rectangle(10, 0, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                    Utility.drawTextWithShadow(b, hungerText, font, new Vector2(startX + 34, startY), Game1.textColor);
                }

                if (heatResistant != 0)
                {
                    string heatText = $"{((heatResistant > 0)? "+" : "")}{heatResistant}% Heat Resist.";
                    Utility.drawWithShadow(b, ModEntry.InfoIcon, new Vector2(startX, startY), new Rectangle(20, 0, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                    Utility.drawTextWithShadow(b, heatText, font, new Vector2(startX + 34, startY), Game1.textColor);
                    startY += 36;
                }
                if (coldResistant != 0)
                {
                    string coldText = $"{((coldResistant > 0) ? "+" : "")}{coldResistant}% Cold Resist.";
                    Utility.drawWithShadow(b, ModEntry.InfoIcon, new Vector2(startX, startY), new Rectangle(30, 0, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                    Utility.drawTextWithShadow(b, coldText, font, new Vector2(startX + 34, startY), Game1.textColor);
                }
                if (coreTemp > -274)
                {
                    string coreTempText = String.Format("{0}C {1}", coreTemp, (deviceType.Equals("general")) ? $"({coreTemp - operationalRange}C - {coreTemp + operationalRange}C)" : "");
                    Utility.drawWithShadow(b, ModEntry.InfoIcon, new Vector2(startX, startY), iconTarget, Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                    Utility.drawTextWithShadow(b, coreTempText, font, new Vector2(startX + 34, startY), Game1.textColor);
                    startY += 36;
                    string effectiveRangeText = $"{effectiveRange} tile{(effectiveRange >= 1 && effectiveRange <= -1 ? "" : "s")} {(ambientCoefficient != 1 ? $"({Math.Floor(ambientCoefficient * 100)}% Eff.)" : "")}";
                    Utility.drawWithShadow(b, ModEntry.InfoIcon, new Vector2(startX, startY), new Rectangle(40, 0, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                    Utility.drawTextWithShadow(b, effectiveRangeText, font, new Vector2(startX + 34, startY), Game1.textColor);
                }


                return;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(DrawHoverText_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static data.ClothingTempResistantData getClothingTempResistInfo(string clothesName, string type)
        {
            return data.ClothingTempResistantDictionary.GetClothingData(clothesName, type);
        } 
    }
}
