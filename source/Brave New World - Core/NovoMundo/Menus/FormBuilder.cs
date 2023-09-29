/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using StardewValley.Locations;
using NovoMundo.Managers;

namespace NovoMundo.Menus
{
    public class Form_Builder : IClickableMenu
    {
        public new const int width = 1280;
        public new const int height = 576;
        public const int buttonWidth = 147;
        public const int buttonHeight = 30;
        private readonly Texture2D texture2D;
        private readonly int npc;
        public List<ClickableComponent> checkboxes = new List<ClickableComponent>();
        private string hoverText;
        private bool boughtSomething;
        private int exitTimer = -1;
        private Property_Manager property_Manager = new();
        private int whichForm;
        private bool IsSeller;
        private bool pass = true;
        private bool jumpThis = false;
        private readonly bool lockIt = Game1.getFarm().isThereABuildingUnderConstruction() is true || Property_Manager.QuarryLand().isThereABuildingUnderConstruction() is true || Property_Manager.PlantationLand().isThereABuildingUnderConstruction() is true;

        public Form_Builder(Texture2D texture2D, int npc, int whichForm, bool IsSeller, int size) : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 288, 1280, 576, showUpperRightCloseButton: true)
        {
            Game1.player.forceCanMove();
            this.texture2D = texture2D;
            this.npc = npc;
            this.whichForm = whichForm;
            this.IsSeller = IsSeller;
            int num = xPositionOnScreen + 4;
            int num2 = yPositionOnScreen + 208;
            
            for (int i = 0; i < size; i++)
            {
                checkboxes.Add(new ClickableComponent(new Rectangle(num, num2, 588, 120), i.ToString() ?? "")
                {
                    myID = i,
                    rightNeighborID = ((i % 2 != 0 || i == 4) ? (-1) : (i + 1)),
                    leftNeighborID = ((i % 2 == 0) ? (-1) : (i - 1)),
                    downNeighborID = i + 2,
                    upNeighborID = i - 2
                });
                num += 592;
                if (num > xPositionOnScreen + 1184)
                {
                    num = xPositionOnScreen + 4;
                    num2 += 120;
                }
            }
            if (whichForm == 0)
            {
                if (lockIt || Game1.player.daysUntilHouseUpgrade.Value > 0)
                {
                    checkboxes[0].name = "complete";
                }
                if (lockIt || Game1.player.daysUntilHouseUpgrade.Value > 0)
                {
                    checkboxes[1].name = "complete";
                }
                if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmComReformed") || !Utility.doesAnyFarmerHaveOrWillReceiveMail("nmComReformedStarted"))
                {
                    checkboxes[2].name = "complete";
                }
                if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmDemoCompleted"))
                {
                    checkboxes[3].name = "complete";
                }
            }
            if (whichForm == 1)
            {
                if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry"))
                {
                    checkboxes[0].name = "complete";
                }
                if (!Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry") || Utility.doesAnyFarmerHaveOrWillReceiveMail("nmCinema"))
                {
                    checkboxes[1].name = "complete";
                }
                if (!Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry") || Utility.doesAnyFarmerHaveOrWillReceiveMail("nmFarm"))
                {
                    checkboxes[2].name = "complete";
                }
                if (!Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry") || Utility.doesAnyFarmerHaveOrWillReceiveMail("nmLake"))
                {
                    checkboxes[3].name = "complete";
                }
            }
            if (whichForm == 2)
            {
                return;
            }
            if (whichForm == 3)
            {

                if (lockIt is true || Game1.player.daysUntilHouseUpgrade.Value > 0)
                {
                    checkboxes[0].name = "complete";
                }
                if (lockIt is true || Game1.player.daysUntilHouseUpgrade.Value > 0 || !Utility.doesAnyFarmerHaveOrWillReceiveMail("nmQuarry"))
                {
                    checkboxes[1].name = "complete";
                }
                if (lockIt is true || Game1.player.daysUntilHouseUpgrade.Value > 0 || !Utility.doesAnyFarmerHaveOrWillReceiveMail("nmFarm"))
                {
                    checkboxes[2].name = "complete";
                }
                if (lockIt is true || Game1.player.daysUntilHouseUpgrade.Value > 0 || !Utility.doesAnyFarmerHaveOrWillReceiveMail("nmLake"))
                {
                    checkboxes[3].name = "complete";
                }
            }
            if (whichForm == 4 || whichForm == 5 || whichForm == 6)
            {
                if (Game1.player.HouseUpgradeLevel == 3 || Game1.player.daysUntilHouseUpgrade.Value > 0)
                {
                    checkboxes[0].name = "complete";
                }
                if (Game1.player.HouseUpgradeLevel < 2)
                {
                    checkboxes[1].name = "complete";
                }
                if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmGreenhouse"))
                {
                    checkboxes[2].name = "complete";
                }
                if (Utility.doesAnyFarmerHaveOrWillReceiveMail("nmCaveBridge"))
                {
                    checkboxes[3].name = "complete";
                }
            }
            exitFunction = onExitFunction;
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
                Game1.mouseCursorTransparency = 1f;

            }
        }
        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }
        private void onExitFunction()
        {           
            if (boughtSomething &&!jumpThis)
            {
               if (whichForm == 1)
                {
                    Game1.drawDialogue(Game1.getCharacterFromName("nmATMJoja"), ModEntry.ModHelper.Translation.Get("NPCDataMorrisBuySomethingMessage"));
                }
               if (whichForm == 4 || whichForm == 5 || whichForm == 6)
                {
                    Game1.drawDialogue(Game1.getCharacterFromName("nmATM"), ModEntry.ModHelper.Translation.Get("NPCDataCarpenterBuySomethingMessage"));
                }
            }
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            int priceFromButtonNumber = 0;
            if (exitTimer >= 0)
            {
                return;
            }
            base.receiveLeftClick(x, y);
            foreach (ClickableComponent checkbox in checkboxes)
            {
                if (!checkbox.containsPoint(x, y) || checkbox.name.Equals("complete"))
                {
                    continue;
                }
                int num = Convert.ToInt32(checkbox.name);
                if (IsSeller)
                {
                    switch (whichForm)
                    {
                        case 1:
                            {
                                priceFromButtonNumber = getPriceFromButtonForm1(num);
                                break;
                            }
                        case 4:
                            {
                                priceFromButtonNumber = getPriceFromButtonForm4(num);
                                break;
                            }
                        case 5:
                            {
                                priceFromButtonNumber = getPriceFromButtonForm5(num);
                                break;
                            }
                        case 6:
                            {
                                priceFromButtonNumber = getPriceFromButtonForm6(num);
                                break;
                            }
                    }
                    if (Game1.player.Money >= priceFromButtonNumber)
                    {
                        Game1.player.Money -= priceFromButtonNumber;
                    }
                    else
                    {
                        Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                        pass = false;
                    }
                }
                if (pass)
                {
                    if (npc == 0 || npc == 1)
                    {
                        Game1.playSound("mouseClick");
                    }
                    else
                    {
                        Game1.playSound("swordswipe");
                    }
                    checkbox.name = "complete";
                    if (IsSeller)
                    {
                        boughtSomething = true;
                    }
                    switch (num)
                    {
                        case 0:
                            if (whichForm == 0)
                            {
                                property_Manager.callMenu("CarpenterFormATMSelectType");

                            }
                            else if (whichForm == 1)
                            {
                                Game1.addMailForTomorrow("nmQuarry", noLetter: true, sendToEveryone: true);
                            }
                            else if (whichForm == 2)
                            {
                                property_Manager.callMenu("CarpenterMenuVanilla");

                            }
                            else if (whichForm == 3)
                            {
                                property_Manager.callMenu("CarpenterMenuVanilla");

                            }
                            else if (whichForm == 4 || whichForm == 5 || whichForm == 6)
                            {
                                Game1.player.daysUntilHouseUpgrade.Value = 3;
                            }
                            break;
                        case 1:
                            if (whichForm == 0)
                            {
                                if (Game1.player.HouseUpgradeLevel == 0)
                                {
                                    property_Manager.callMenu("CarpenterFormATMImprovements0");
                                }
                                if (Game1.player.HouseUpgradeLevel == 1)
                                {
                                    property_Manager.callMenu("CarpenterFormATMImprovements1");
                                }
                                if (Game1.player.HouseUpgradeLevel >= 2)
                                {
                                    property_Manager.callMenu("CarpenterFormATMImprovements2");
                                }
                            }
                            else if (whichForm == 1)
                            {
                                Game1.addMailForTomorrow("nmCinema", noLetter: true, sendToEveryone: true);
                            }
                            else if (whichForm == 2)
                            {
                                property_Manager.callMenu("CarpenterFormATMSelectWhere");

                            }
                            else if (whichForm == 3)
                            {
                                property_Manager.callMenu("CarpenterMenuVanilla");

                            }
                            else if (whichForm == 4 || whichForm == 5 || whichForm == 6)
                            {
                                HouseRenovation.ShowRenovationMenu();
                                jumpThis = true;
                            }
                            break;
                        case 2:
                            if (whichForm == 0)
                            {
                                property_Manager.callMenu("CarpenterMenuMainFarm");
                            }
                            else if (whichForm == 1)
                            {
                                Game1.addMailForTomorrow("nmFarm", noLetter: true, sendToEveryone: true);
                            }
                            else if (whichForm == 3)
                            {
                                property_Manager.callMenu("CarpenterMenuVanilla");

                            }
                            else if (whichForm == 4 || whichForm == 5 || whichForm == 6)
                            {
                                Game1.addMailForTomorrow("nmGreenhouse", noLetter: true, sendToEveryone: true);

                            }
                            break;
                        case 3:
                            if (whichForm == 0)
                            {
                                property_Manager.callMenu("CarpenterMenuMainFarm");
                            }
                            else if (whichForm == 1)
                            {
                                Game1.addMailForTomorrow("nmLake", noLetter: true, sendToEveryone: true);
                            }
                            else if (whichForm == 3)
                            {
                                property_Manager.callMenu("CarpenterMenuVanilla");

                            }
                            else if (whichForm == 4 || whichForm == 5 || whichForm == 6)
                            {
                                Game1.addMailForTomorrow("nmCaveBridge", noLetter: true, sendToEveryone: true);
                            }
                            break;
                        case 4:
                            break;
                    }
                    exitTimer = 1000;

                }
            }
        }
        public override bool readyToClose()
        {
            return true;
        }
        public override void update(GameTime time)
        {
            base.update(time);
            if (exitTimer >= 0)
            {
                exitTimer -= time.ElapsedGameTime.Milliseconds;
                if (exitTimer <= 0)
                {
                    exitThisMenu();
                }
            }
            Game1.mouseCursorTransparency = 1f;
        }
        public int getPriceFromButtonForm1(int buttonNumber)
        {
            return buttonNumber switch
            {
                0 => 10,
                1 => 50,
                2 => 25,
                3 => 10,
                _ => -1,
            };
        }
        public int getPriceFromButtonForm4(int buttonNumber)
        {
            return buttonNumber switch
            {
                0 => 15,
                1 => 0,
                2 => 35,
                3 => 10,
                _ => -1,
            };
        }
        public int getPriceFromButtonForm5(int buttonNumber)
        {
            return buttonNumber switch
            {
                0 => 60,
                1 => 0,
                2 => 35,
                3 => 10,
                _ => -1,
            };
        }
        public int getPriceFromButtonForm6(int buttonNumber)
        {
            return buttonNumber switch
            {
                0 => 100,
                1 => 0,
                2 => 35,
                3 => 10,
                _ => -1,
            };
        }
        public string getDescriptionFromButtonNumber(int buttonNumber)
        {
            switch (whichForm)
            {
                case 0:
                    {
                        return ModEntry.ModHelper.Translation.Get("MenusCarpenterMainMenu" + buttonNumber);
                    }
                case 1:
                    {
                        return ModEntry.ModHelper.Translation.Get("MenusNewJojaFormHoverMessage" + buttonNumber);
                    }
                case 2:
                    {
                        return ModEntry.ModHelper.Translation.Get("MenusNewCarpenterATMSelectTypeHoverMessage" + buttonNumber);
                    }
                case 3:
                    {
                        return ModEntry.ModHelper.Translation.Get("MenusNewCarpenterATMBuildWhereHoverMessage" + buttonNumber);
                    }
                case 4:
                    {
                        return ModEntry.ModHelper.Translation.Get("MenusNewCarpenterATMImprovementsHoverMessage" + buttonNumber);
                    }
                case 5:
                    {
                        return ModEntry.ModHelper.Translation.Get("MenusNewCarpenterATMImprovementsHoverMessage" + buttonNumber);
                    }
                case 6:
                    {
                        return ModEntry.ModHelper.Translation.Get("MenusNewCarpenterATMImprovementsHoverMessage" + buttonNumber);
                    }
            }
            return null;
            
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";
            foreach (ClickableComponent checkbox in checkboxes)
            {
                if (checkbox.containsPoint(x, y))
                {
                    hoverText = (checkbox.name.Equals("complete") ? "" : Game1.parseText(getDescriptionFromButtonNumber(Convert.ToInt32(checkbox.name)), Game1.dialogueFont, 384));
                }
            }
        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            xPositionOnScreen = Game1.uiViewport.Width / 2 - 640;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - 288;
            int num = xPositionOnScreen + 4;
            int num2 = yPositionOnScreen + 208;
            checkboxes.Clear();
            for (int i = 0; i < 5; i++)
            {
                checkboxes.Add(new ClickableComponent(new Rectangle(num, num2, 588, 120), i.ToString() ?? ""));
                num += 592;
                if (num > xPositionOnScreen + 1184)
                {
                    num = xPositionOnScreen + 4;
                    num2 += 120;
                }
            }
        }
        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
        }
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            b.Draw(texture2D, Utility.getTopLeftPositionForCenteringOnScreen(1280, 576), new Rectangle(0, 0, 320, 144), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.79f);
            base.draw(b);
            foreach (ClickableComponent checkbox in checkboxes)
            {
                if (checkbox.name.Equals("complete"))
                {
                    if (whichForm == 0 || whichForm == 3)
                    {
                        b.Draw(texture2D, new Vector2(checkbox.bounds.Left + 16, checkbox.bounds.Y + 16), new Rectangle(0, 144, 160, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
                    }
                    else
                    {
                        b.Draw(texture2D, new Vector2(checkbox.bounds.Left + 16, checkbox.bounds.Y + 16), new Rectangle(0, 144, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
                    }                   
                }
            }
            if (IsSeller)
            {
                Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.uiViewport.Width - 300 - spaceToClearSideBorder * 2, 4);
            }           
            Game1.mouseCursorTransparency = 1f;
            if (hoverText != null && !hoverText.Equals(""))
            {
                drawHoverText(b, hoverText, Game1.dialogueFont);
                drawMouse(b, false, 44);
            }
            else
            {
                drawMouse(b);
            }
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}
