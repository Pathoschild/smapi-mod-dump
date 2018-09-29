using FarmExpansion.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace FarmExpansion.Menus
{
    public class FEPurchaseAnimalsMenu : IClickableMenu
    {
        FEFramework framework;

        GameLocation previousLocation = Game1.currentLocation;

        Farm currentFarm = Game1.getFarm();

        public const int region_okButton = 101;

        public const int region_doneNamingButton = 102;

        public const int region_randomButton = 103;

        public const int region_namingBox = 104;

        public static int menuHeight = Game1.tileSize * 4;

        public static int menuWidth = Game1.tileSize * 6;

        public List<ClickableTextureComponent> animalsToPurchase = new List<ClickableTextureComponent>();

        private ClickableTextureComponent previousFarmButton;

        private ClickableTextureComponent nextFarmButton;

        public ClickableTextureComponent okButton;

        public ClickableTextureComponent doneNamingButton;

        public ClickableTextureComponent randomButton;

        public ClickableTextureComponent hovered;

        public ClickableComponent textBoxCC;

        private bool onFarm;

        private bool namingAnimal;

        private bool freeze;

        private FarmAnimal animalBeingPurchased;

        private TextBox textBox;

        private TextBoxEvent e;

        private Building newAnimalHome;

        private string[] farmName = new string[2] { "  Farm  ", "Expansion" };

        private int priceOfAnimal;

        public FEPurchaseAnimalsMenu(FEFramework framework)
        {
            this.framework = framework;
            this.resetBounds();
            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        private void resetBounds()
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - menuWidth / 2 - IClickableMenu.borderWidth * 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - menuHeight - IClickableMenu.borderWidth * 2; 
            this.width = menuWidth + IClickableMenu.borderWidth * 2;
            this.height = menuHeight + IClickableMenu.borderWidth + Game1.tileSize * 2;
            this.populateAnimalStock();
            this.previousFarmButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + Game1.tileSize / 4, yPositionOnScreen, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), Game1.pixelZoom);
            this.nextFarmButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(xPositionOnScreen + SpriteText.getWidthOfString("Expansion") * 2 - Game1.tileSize / 2, yPositionOnScreen, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), Game1.pixelZoom);
            this.okButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - Game1.tileSize - IClickableMenu.borderWidth, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false)
            {
                myID = 101,
                upNeighborID = 103,
                leftNeighborID = 103
            };
            this.randomButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize * 4 / 5 + Game1.tileSize, Game1.viewport.Height / 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(381, 361, 10, 10), (float)Game1.pixelZoom, false)
            {
                myID = 103,
                downNeighborID = 101,
                rightNeighborID = 101
            };
            this.textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor);
            this.textBox.X = Game1.viewport.Width / 2 - Game1.tileSize * 3;
            this.textBox.Y = Game1.viewport.Height / 2;
            this.textBox.Width = Game1.tileSize * 4;
            this.textBox.Height = Game1.tileSize * 3;
            this.e = new TextBoxEvent(this.textBoxEnter);
            this.textBoxCC = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X, this.textBox.Y, 192, 48), "")
            {
                myID = 104,
                rightNeighborID = 102,
                downNeighborID = 101
            };
            this.randomButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X + this.textBox.Width + Game1.tileSize + Game1.tileSize * 3 / 4 - Game1.pixelZoom * 2, Game1.viewport.Height / 2 + Game1.pixelZoom, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(381, 361, 10, 10), (float)Game1.pixelZoom, false)
            {
                myID = 103,
                leftNeighborID = 102,
                downNeighborID = 101,
                rightNeighborID = 101
            };
            this.doneNamingButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X + this.textBox.Width + Game1.tileSize / 2 + Game1.pixelZoom, Game1.viewport.Height / 2 - Game1.pixelZoom * 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false)
            {
                myID = 102,
                rightNeighborID = 103,
                leftNeighborID = 104,
                downNeighborID = 101
            };
        }

        private void populateAnimalStock()
        {
            this.animalsToPurchase.Clear();
            List<Object> stock = this.getPurchaseAnimalStock();
            for (int i = 0; i < stock.Count; i++)
            {
                this.animalsToPurchase.Add(new ClickableTextureComponent(string.Concat(stock[i].salePrice()), new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + i % 3 * Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + i / 3 * (Game1.tileSize + Game1.tileSize / 3), Game1.tileSize * 2, Game1.tileSize), null, stock[i].Name, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(i % 3 * 16 * 2, 448 + i / 3 * 16, 32, 16), 4f, stock[i].type == null)
                {
                    item = stock[i],
                    myID = i,
                    rightNeighborID = ((i % 3 == 2) ? -1 : (i + 1)),
                    leftNeighborID = ((i % 3 == 0) ? -1 : (i - 1)),
                    downNeighborID = i + 3,
                    upNeighborID = i - 3
                });
            }
        }

        private List<Object> getPurchaseAnimalStock()
        {
            List<Object> list = new List<Object>();
            Object item = new Object(100, 1, false, 400, 0)
            {
                name = "Chicken",
                type = ((currentFarm.isBuildingConstructed("Coop") || currentFarm.isBuildingConstructed("Deluxe Coop") || currentFarm.isBuildingConstructed("Big Coop")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5926", new object[0])),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5922", new object[0])
            };
            list.Add(item);
            item = new Object(100, 1, false, 750, 0)
            {
                name = "Dairy Cow",
                type = ((currentFarm.isBuildingConstructed("Barn") || currentFarm.isBuildingConstructed("Deluxe Barn") || currentFarm.isBuildingConstructed("Big Barn")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5931", new object[0])),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5927", new object[0])
            };
            list.Add(item);
            item = new Object(100, 1, false, 2000, 0)
            {
                name = "Goat",
                type = ((currentFarm.isBuildingConstructed("Big Barn") || currentFarm.isBuildingConstructed("Deluxe Barn")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5936", new object[0])),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5933", new object[0])
            };
            list.Add(item);
            item = new Object(100, 1, false, 2000, 0)
            {
                name = "Duck",
                type = ((currentFarm.isBuildingConstructed("Big Coop") || currentFarm.isBuildingConstructed("Deluxe Coop")) ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5940", new object[0])),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5937", new object[0])
            };
            list.Add(item);
            item = new Object(100, 1, false, 4000, 0)
            {
                name = "Sheep",
                type = (currentFarm.isBuildingConstructed("Deluxe Barn") ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5944", new object[0])),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5942", new object[0])
            };
            list.Add(item);
            item = new Object(100, 1, false, 4000, 0)
            {
                name = "Rabbit",
                type = (currentFarm.isBuildingConstructed("Deluxe Coop") ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5947", new object[0])),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5945", new object[0])
            };
            list.Add(item);
            item = new Object(100, 1, false, 8000, 0)
            {
                name = "Pig",
                type = (currentFarm.isBuildingConstructed("Deluxe Barn") ? null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5950", new object[0])),
                displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5948", new object[0])
            };
            list.Add(item);
            return list;
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = base.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        public void textBoxEnter(TextBox sender)
        {
            if (!this.namingAnimal)
            {
                return;
            }
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is FEPurchaseAnimalsMenu))
            {
                this.textBox.OnEnterPressed -= this.e;
                return;
            }
            if (sender.Text.Length >= 1)
            {
                if (Utility.areThereAnyOtherAnimalsWithThisName(sender.Text))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11308", new object[0]));
                    return;
                }
                this.textBox.OnEnterPressed -= this.e;
                this.animalBeingPurchased.name = sender.Text;
                this.animalBeingPurchased.displayName = sender.Text;
                this.animalBeingPurchased.home = this.newAnimalHome;
                this.animalBeingPurchased.homeLocation = new Vector2((float)this.newAnimalHome.tileX, (float)this.newAnimalHome.tileY);
                this.animalBeingPurchased.setRandomPosition(this.animalBeingPurchased.home.indoors);
                (this.newAnimalHome.indoors as AnimalHouse).animals.Add(this.animalBeingPurchased.myID, this.animalBeingPurchased);
                (this.newAnimalHome.indoors as AnimalHouse).animalsThatLiveHere.Add(this.animalBeingPurchased.myID);
                this.newAnimalHome = null;
                this.namingAnimal = false;
                Game1.player.money -= this.priceOfAnimal;
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForReturnAfterPurchasingAnimal), 0.02f);
            }
        }

        public void setUpForReturnAfterPurchasingAnimal()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = previousLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            this.onFarm = false;
            this.okButton.bounds.X = this.xPositionOnScreen + this.width + 4;
            Game1.displayHUD = true;
            Game1.displayFarmer = true;
            this.freeze = false;
            this.textBox.OnEnterPressed -= this.e;
            this.textBox.Selected = false;
            Game1.viewportFreeze = false;
            Game1.globalFadeToClear(new Game1.afterFadeFunction(this.marnieAnimalPurchaseMessage), 0.02f);
        }

        public void marnieAnimalPurchaseMessage()
        {
            base.exitThisMenu(true);
            Game1.player.forceCanMove();
            this.freeze = false;
            Game1.drawDialogue(Game1.getCharacterFromName("Marnie", false), this.animalBeingPurchased.isMale() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11311", new object[]
            {
                this.animalBeingPurchased.displayName
            }) : Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11314", new object[]
            {
                this.animalBeingPurchased.displayName
            }));
        }

        public void setUpForAnimalPlacement()
        {
            Game1.displayFarmer = false;
            Game1.currentLocation = currentFarm;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            this.onFarm = true;
            this.freeze = false;
            this.okButton.bounds.X = Game1.viewport.Width - Game1.tileSize * 2;
            this.okButton.bounds.Y = Game1.viewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
        }

        public void setUpForReturnToShopMenu()
        {
            this.freeze = false;
            Game1.displayFarmer = true;
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = previousLocation;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear(null, 0.02f);
            this.onFarm = false;
            this.resetBounds();
            this.okButton.bounds.X = this.xPositionOnScreen + this.width + 4;
            this.okButton.bounds.Y = this.yPositionOnScreen + this.height - Game1.tileSize - IClickableMenu.borderWidth;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            this.namingAnimal = false;
            this.textBox.OnEnterPressed -= this.e;
            this.textBox.Selected = false;
            if (Game1.options.SnappyMenus)
            {
                this.snapToDefaultClickableComponent();
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade || this.freeze)
            {
                return;
            }
            if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
            {
                if (this.onFarm)
                {
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForReturnToShopMenu), 0.02f);
                    Game1.playSound("smallSelect");
                }
                else
                {
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                }
            }
            if (this.onFarm)
            {
                Vector2 tile = new Vector2((float)((x + Game1.viewport.X) / Game1.tileSize), (float)((y + Game1.viewport.Y) / Game1.tileSize));
                Building buildingAt = currentFarm.getBuildingAt(tile);
                if (buildingAt != null && !this.namingAnimal)
                {
                    if (buildingAt.buildingType.Contains(this.animalBeingPurchased.buildingTypeILiveIn))
                    {
                        if ((buildingAt.indoors as AnimalHouse).isFull())
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321", new object[0]));
                        }
                        else if (this.animalBeingPurchased.harvestType != 2)
                        {
                            this.namingAnimal = true;
                            this.newAnimalHome = buildingAt;
                            if (this.animalBeingPurchased.sound != null && Game1.soundBank != null)
                            {
                                Cue expr_15B = Game1.soundBank.GetCue(this.animalBeingPurchased.sound);
                                expr_15B.SetVariable("Pitch", (float)(1200 + Game1.random.Next(-200, 201)));
                                expr_15B.Play();
                            }
                            this.textBox.OnEnterPressed += this.e;
                            this.textBox.Text = this.animalBeingPurchased.displayName;
                            Game1.keyboardDispatcher.Subscriber = this.textBox;
                            if (Game1.options.SnappyMenus)
                            {
                                this.currentlySnappedComponent = base.getComponentWithID(104);
                                this.snapCursorToCurrentSnappedComponent();
                            }
                        }
                        else if (Game1.player.money >= this.priceOfAnimal)
                        {
                            this.newAnimalHome = buildingAt;
                            this.animalBeingPurchased.home = this.newAnimalHome;
                            this.animalBeingPurchased.homeLocation = new Vector2((float)this.newAnimalHome.tileX, (float)this.newAnimalHome.tileY);
                            this.animalBeingPurchased.setRandomPosition(this.animalBeingPurchased.home.indoors);
                            (this.newAnimalHome.indoors as AnimalHouse).animals.Add(this.animalBeingPurchased.myID, this.animalBeingPurchased);
                            (this.newAnimalHome.indoors as AnimalHouse).animalsThatLiveHere.Add(this.animalBeingPurchased.myID);
                            this.newAnimalHome = null;
                            this.namingAnimal = false;
                            if (this.animalBeingPurchased.sound != null && Game1.soundBank != null)
                            {
                                Cue expr_2DC = Game1.soundBank.GetCue(this.animalBeingPurchased.sound);
                                expr_2DC.SetVariable("Pitch", (float)(1200 + Game1.random.Next(-200, 201)));
                                expr_2DC.Play();
                            }
                            Game1.player.money -= this.priceOfAnimal;
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11324", new object[]
                            {
                                this.animalBeingPurchased.displayType
                            }), Color.LimeGreen, 3500f));
                            this.animalBeingPurchased = new FarmAnimal(this.animalBeingPurchased.type, MultiplayerUtility.getNewID(), Game1.player.uniqueMultiplayerID);
                        }
                        else if (Game1.player.money < this.priceOfAnimal)
                        {
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                        }
                    }
                    else
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11326", new object[]
                        {
                            this.animalBeingPurchased.displayType
                        }));
                    }
                }
                if (this.namingAnimal)
                {
                    if (this.doneNamingButton.containsPoint(x, y))
                    {
                        this.textBoxEnter(this.textBox);
                        Game1.playSound("smallSelect");
                    }
                    else if (this.namingAnimal && this.randomButton.containsPoint(x, y))
                    {
                        this.animalBeingPurchased.name = Dialogue.randomName();
                        this.animalBeingPurchased.displayName = this.animalBeingPurchased.name;
                        this.textBox.Text = this.animalBeingPurchased.displayName;
                        this.randomButton.scale = this.randomButton.baseScale;
                        Game1.playSound("drumkit6");
                    }
                    this.textBox.Update();
                    return;
                }
            }
            else
            {
                if (this.previousFarmButton.containsPoint(x, y))
                {
                    currentFarm = framework.swapFarm(currentFarm);
                    this.populateAnimalStock();
                    this.previousFarmButton.scale = this.previousFarmButton.baseScale;
                }
                if (this.nextFarmButton.containsPoint(x, y))
                {
                    currentFarm = framework.swapFarm(currentFarm);
                    this.populateAnimalStock();
                    this.nextFarmButton.scale = this.nextFarmButton.baseScale;
                }
                foreach (ClickableTextureComponent current in this.animalsToPurchase)
                {
                    if (current.containsPoint(x, y) && (current.item as Object).type == null)
                    {
                        int num = current.item.salePrice();
                        if (Game1.player.money >= num)
                        {
                            Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForAnimalPlacement), 0.02f);
                            Game1.playSound("smallSelect");
                            this.onFarm = true;
                            this.animalBeingPurchased = new FarmAnimal(current.hoverText, MultiplayerUtility.getNewID(), Game1.player.uniqueMultiplayerID);
                            this.priceOfAnimal = num;
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325", new object[0]), Color.Red, 3500f));
                        }
                    }
                }
            }
        }

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return this.onFarm && !this.namingAnimal;
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (b == Buttons.B && !Game1.globalFade && this.onFarm && this.namingAnimal)
            {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForReturnToShopMenu), 0.02f);
                Game1.playSound("smallSelect");
            }
            if (!this.onFarm && b == Buttons.LeftTrigger)
            {
                currentFarm = framework.swapFarm(currentFarm);
                this.populateAnimalStock();
                this.previousFarmButton.scale = this.previousFarmButton.baseScale;
            }
            if (!this.onFarm && b == Buttons.RightTrigger)
            {
                currentFarm = framework.swapFarm(currentFarm);
                this.populateAnimalStock();
                this.nextFarmButton.scale = this.nextFarmButton.baseScale;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.globalFade || this.freeze)
            {
                return;
            }
            if (!Game1.globalFade && this.onFarm)
            {
                if (!this.namingAnimal)
                {
                    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
                    {
                        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForReturnToShopMenu), 0.02f);
                        return;
                    }
                    if (!Game1.options.SnappyMenus)
                    {
                        if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                        {
                            Game1.panScreen(0, 4);
                            return;
                        }
                        if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                        {
                            Game1.panScreen(4, 0);
                            return;
                        }
                        if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                        {
                            Game1.panScreen(0, -4);
                            return;
                        }
                        if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                        {
                            Game1.panScreen(-4, 0);
                            return;
                        }
                    }
                }
                else if (Game1.options.SnappyMenus)
                {
                    if (!this.textBox.Selected && Game1.options.doesInputListContain(Game1.options.menuButton, key))
                    {
                        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForReturnToShopMenu), 0.02f);
                        Game1.playSound("smallSelect");
                        return;
                    }
                    if (!this.textBox.Selected || !Game1.options.doesInputListContain(Game1.options.menuButton, key))
                    {
                        base.receiveKeyPress(key);
                        return;
                    }
                }
            }
            else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.globalFade)
            {
                if (this.readyToClose())
                {
                    Game1.player.forceCanMove();
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                    return;
                }
            }
            else if (Game1.options.SnappyMenus)
            {
                base.receiveKeyPress(key);
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (this.onFarm && !this.namingAnimal)
            {
                int num = Game1.getOldMouseX() + Game1.viewport.X;
                int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
                if (num - Game1.viewport.X < Game1.tileSize)
                {
                    Game1.panScreen(-8, 0);
                }
                else if (num - (Game1.viewport.X + Game1.viewport.Width) >= -Game1.tileSize)
                {
                    Game1.panScreen(8, 0);
                }
                if (num2 - Game1.viewport.Y < Game1.tileSize)
                {
                    Game1.panScreen(0, -8);
                }
                else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -Game1.tileSize)
                {
                    Game1.panScreen(0, 8);
                }
                Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
                for (int i = 0; i < pressedKeys.Length; i++)
                {
                    Keys key = pressedKeys[i];
                    this.receiveKeyPress(key);
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            this.hovered = null;
            if (Game1.globalFade || this.freeze)
            {
                return;
            }
            if (this.okButton != null)
            {
                if (this.okButton.containsPoint(x, y))
                {
                    this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
                }
                else
                {
                    this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
                }
            }
            if (this.onFarm)
            {
                if (!this.namingAnimal)
                {
                    Vector2 tile = new Vector2((float)((x + Game1.viewport.X) / Game1.tileSize), (float)((y + Game1.viewport.Y) / Game1.tileSize));
                    foreach (Building current in currentFarm.buildings)
                    {
                        current.color = Color.White;
                    }
                    Building buildingAt = currentFarm.getBuildingAt(tile);
                    if (buildingAt != null)
                    {
                        if (buildingAt.buildingType.Contains(this.animalBeingPurchased.buildingTypeILiveIn) && !(buildingAt.indoors as AnimalHouse).isFull())
                        {
                            buildingAt.color = Color.LightGreen * 0.8f;
                        }
                        else
                        {
                            buildingAt.color = Color.Red * 0.8f;
                        }
                    }
                }
                if (this.doneNamingButton != null)
                {
                    if (this.doneNamingButton.containsPoint(x, y))
                    {
                        this.doneNamingButton.scale = Math.Min(1.1f, this.doneNamingButton.scale + 0.05f);
                    }
                    else
                    {
                        this.doneNamingButton.scale = Math.Max(1f, this.doneNamingButton.scale - 0.05f);
                    }
                }
                this.randomButton.tryHover(x, y, 0.5f);
                return;
            }
            foreach (ClickableTextureComponent current in this.animalsToPurchase)
            {
                if (current.containsPoint(x, y))
                {
                    current.scale = Math.Min(current.scale + 0.05f, 4.1f);
                    this.hovered = current;
                }
                else
                {
                    current.scale = Math.Max(4f, current.scale - 0.025f);
                }
            }
        }

        public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
        {
            this.resetBounds();
        }

        public override void draw(SpriteBatch b)
        {
            if (!this.onFarm && !Game1.dialogueUp && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                SpriteText.drawStringWithScrollBackground(b, farmName[framework.expansionSelected(currentFarm.Name) ? 1 : 0], xPositionOnScreen + width / 4 + Game1.tileSize / 5, yPositionOnScreen, "Expansion", 1f, -1);
                SpriteText.drawStringWithScrollBackground(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11354", new object[0]), this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize, this.yPositionOnScreen + this.height + -Game1.tileSize / 2 + IClickableMenu.spaceToClearTopBorder / 2, "Truffle Pig", 1f, -1);
                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false);
                Game1.dayTimeMoneyBox.drawMoneyBox(b, -1, -1);
                this.previousFarmButton.draw(b);
                this.nextFarmButton.draw(b);
                using (List<ClickableTextureComponent>.Enumerator enumerator = this.animalsToPurchase.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ClickableTextureComponent current = enumerator.Current;
                        current.draw(b, ((current.item as Object).type != null) ? (Color.Black * 0.4f) : Color.White, 0.87f);
                    }
                    goto IL_2AE;
                }
            }
            if (!Game1.globalFade && this.onFarm)
            {
                string s = Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11355", new object[]
                {
                    this.animalBeingPurchased.displayHouse,
                    this.animalBeingPurchased.displayType
                });
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, Game1.tileSize / 4, "", 1f, -1);
                if (this.namingAnimal)
                {
                    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                    Game1.drawDialogueBox(Game1.viewport.Width / 2 - Game1.tileSize * 4, Game1.viewport.Height / 2 - Game1.tileSize * 3 - Game1.tileSize / 2, Game1.tileSize * 8, Game1.tileSize * 3, false, true, null, false);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11357", new object[0]), Game1.dialogueFont, new Vector2((float)(Game1.viewport.Width / 2 - Game1.tileSize * 4 + Game1.tileSize / 2 + 8), (float)(Game1.viewport.Height / 2 - Game1.tileSize * 2 + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                    this.textBox.Draw(b);
                    this.doneNamingButton.draw(b);
                    this.randomButton.draw(b);
                }
            }
            IL_2AE:
            if (!Game1.globalFade && this.okButton != null)
            {
                this.okButton.draw(b);
            }
            if (this.hovered != null)
            {
                if ((this.hovered.item as Object).type != null)
                {
                    IClickableMenu.drawHoverText(b, Game1.parseText((this.hovered.item as Object).type, Game1.dialogueFont, Game1.tileSize * 5), Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
                }
                else
                {
                    string animalTitle = PurchaseAnimalsMenu.getAnimalTitle(this.hovered.hoverText);
                    SpriteText.drawStringWithScrollBackground(b, animalTitle, this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize + Game1.tileSize / 2, this.yPositionOnScreen + this.height + Game1.tileSize - Game1.tileSize / 4 + IClickableMenu.spaceToClearTopBorder / 2, "Truffle Pig", 1f, -1);
                    SpriteText.drawStringWithScrollBackground(b, "$" + Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", new object[]
                    {
                        this.hovered.item.salePrice()
                    }), this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2, this.yPositionOnScreen + this.height + Game1.tileSize * 2 + IClickableMenu.spaceToClearTopBorder / 2, "$99999999g", (Game1.player.Money >= this.hovered.item.salePrice()) ? 1f : 0.5f, -1);
                    string animalDescription = PurchaseAnimalsMenu.getAnimalDescription(this.hovered.hoverText);
                    IClickableMenu.drawHoverText(b, Game1.parseText(animalDescription, Game1.smallFont, Game1.tileSize * 5), Game1.smallFont, 0, 0, -1, animalTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
                }
            }
            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }
    }
}
