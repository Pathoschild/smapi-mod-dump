using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace PelicanFiber.Framework
{
    internal class BuyAnimalMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        private readonly string WhereToGo;
        private static readonly int MenuHeight = Game1.tileSize * 5;
        private static readonly int MenuWidth = Game1.tileSize * 7;
        private readonly List<ClickableTextureComponent> AnimalsToPurchase = new List<ClickableTextureComponent>();
        private readonly ClickableTextureComponent OkButton;
        private readonly ClickableTextureComponent DoneNamingButton;
        private readonly ClickableTextureComponent RandomButton;
        private ClickableTextureComponent Hovered;
        private readonly ClickableTextureComponent BackButton;
        private bool OnFarm;
        private bool NamingAnimal;
        private bool Freeze;
        private FarmAnimal AnimalBeingPurchased;
        private readonly TextBox TextBox;
        private readonly TextBoxEvent TextBoxEvent;
        private Building NewAnimalHome;
        private int PriceOfAnimal;
        private readonly Action OnMenuOpened;
        private readonly Func<long> GetNewId;


        /*********
        ** Public methods
        *********/
        public BuyAnimalMenu(List<Object> stock, Action onMenuOpened, Func<long> getNewId)
          : base(Game1.viewport.Width / 2 - BuyAnimalMenu.MenuWidth / 2 - IClickableMenu.borderWidth * 2, Game1.viewport.Height / 2 - BuyAnimalMenu.MenuHeight - IClickableMenu.borderWidth * 2, BuyAnimalMenu.MenuWidth + IClickableMenu.borderWidth * 2, BuyAnimalMenu.MenuHeight + IClickableMenu.borderWidth)
        {
            this.OnMenuOpened = onMenuOpened;
            this.GetNewId = getNewId;
            this.WhereToGo = Game1.player.currentLocation.Name;

            this.height += Game1.tileSize;
            for (int index = 0; index < stock.Count; ++index)
            {
                List<ClickableTextureComponent> animalsToPurchase = this.AnimalsToPurchase;
                ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(string.Concat(stock[index].salePrice()), new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + index % 3 * Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + index / 3 * (Game1.tileSize + Game1.tileSize / 3), Game1.tileSize * 2, Game1.tileSize), null, stock[index].Name, Game1.mouseCursors, new Rectangle(index % 3 * 16 * 2, 448 + index / 3 * 16, 32, 16), 4f, stock[index].Type == null);
                textureComponent1.item = stock[index];
                ClickableTextureComponent textureComponent2 = textureComponent1;
                animalsToPurchase.Add(textureComponent2);
            }
            this.OkButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - Game1.tileSize - IClickableMenu.borderWidth, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            this.RandomButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize * 4 / 5 + Game1.tileSize, Game1.viewport.Height / 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), Game1.pixelZoom);
            this.TextBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor)
            {
                X = Game1.viewport.Width / 2 - Game1.tileSize * 3,
                Y = Game1.viewport.Height / 2,
                Width = Game1.tileSize * 4,
                Height = Game1.tileSize * 3
            };
            this.TextBoxEvent = this.TextBoxEnter;
            this.RandomButton = new ClickableTextureComponent(new Rectangle(this.TextBox.X + this.TextBox.Width + Game1.tileSize + Game1.tileSize * 3 / 4 - Game1.pixelZoom * 2, Game1.viewport.Height / 2 + Game1.pixelZoom, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), Game1.pixelZoom);
            this.DoneNamingButton = new ClickableTextureComponent(new Rectangle(this.TextBox.X + this.TextBox.Width + Game1.tileSize / 2 + Game1.pixelZoom, Game1.viewport.Height / 2 - Game1.pixelZoom * 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            this.BackButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - 10, this.yPositionOnScreen + 10, 12 * Game1.pixelZoom, 11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade || this.Freeze)
                return;

            if (this.BackButton != null && this.BackButton.containsPoint(x, y))
            {
                this.BackButton.scale = this.BackButton.baseScale;
                this.BackButtonPressed();
            }

            if (this.OkButton != null && this.OkButton.containsPoint(x, y) && this.readyToClose())
            {
                if (this.OnFarm)
                {
                    Game1.globalFadeToBlack(this.SetUpForReturnToShopMenu);
                    Game1.playSound("smallSelect");
                }
                else
                {
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                }
            }
            if (this.OnFarm)
            {
                Building building = Game1.getFarm().getBuildingAt(new Vector2((x + Game1.viewport.X) / Game1.tileSize, (y + Game1.viewport.Y) / Game1.tileSize));
                if (building != null && !this.NamingAnimal)
                {
                    if (building.buildingType.Contains(this.AnimalBeingPurchased.buildingTypeILiveIn.Value))
                    {
                        AnimalHouse animalHouse = (AnimalHouse)building.indoors.Value;
                        if (animalHouse.isFull())
                            Game1.showRedMessage("That Building Is Full");
                        else if (this.AnimalBeingPurchased.harvestType.Value != 2)
                        {
                            this.NamingAnimal = true;
                            this.NewAnimalHome = building;
                            if (this.AnimalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                ICue cue = Game1.soundBank.GetCue(this.AnimalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }
                            this.TextBox.OnEnterPressed += this.TextBoxEvent;
                            Game1.keyboardDispatcher.Subscriber = this.TextBox;
                            this.TextBox.Text = this.AnimalBeingPurchased.Name;
                            this.TextBox.Selected = true;
                        }
                        else if (Game1.player.Money >= this.PriceOfAnimal)
                        {
                            this.NewAnimalHome = building;
                            this.AnimalBeingPurchased.home = this.NewAnimalHome;
                            this.AnimalBeingPurchased.homeLocation.Value = new Vector2(this.NewAnimalHome.tileX.Value, this.NewAnimalHome.tileY.Value);
                            this.AnimalBeingPurchased.setRandomPosition(this.AnimalBeingPurchased.home.indoors.Value);
                            ((AnimalHouse)this.NewAnimalHome.indoors.Value).animals.Add(this.AnimalBeingPurchased.myID.Value, this.AnimalBeingPurchased);
                            ((AnimalHouse)this.NewAnimalHome.indoors.Value).animalsThatLiveHere.Add(this.AnimalBeingPurchased.myID.Value);
                            this.NewAnimalHome = null;
                            this.NamingAnimal = false;
                            if (this.AnimalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                ICue cue = Game1.soundBank.GetCue(this.AnimalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }
                            Game1.player.Money -= this.PriceOfAnimal;
                            Game1.addHUDMessage(new HUDMessage("Purchased " + this.AnimalBeingPurchased.type.Value, Color.LimeGreen, 3500f));
                            this.AnimalBeingPurchased = new FarmAnimal(this.AnimalBeingPurchased.type.Value, this.GetNewId(), Game1.player.UniqueMultiplayerID);
                        }
                        else if (Game1.player.Money < this.PriceOfAnimal)
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                    }
                    else
                        Game1.showRedMessage(this.AnimalBeingPurchased.type.Value.Split(' ').Last() + "s Can't Live There.");
                }
                if (this.NamingAnimal && this.DoneNamingButton.containsPoint(x, y))
                {
                    this.TextBoxEnter(this.TextBox);
                    Game1.playSound("smallSelect");
                }
                else
                {
                    if (!this.NamingAnimal || !this.RandomButton.containsPoint(x, y))
                        return;
                    this.AnimalBeingPurchased.Name = Dialogue.randomName();
                    this.TextBox.Text = this.AnimalBeingPurchased.Name;
                    this.RandomButton.scale = this.RandomButton.baseScale;
                    Game1.playSound("drumkit6");
                }
            }
            else
            {
                foreach (ClickableTextureComponent textureComponent in this.AnimalsToPurchase)
                {
                    if (textureComponent.containsPoint(x, y) && ((Object)textureComponent.item).Type == null)
                    {
                        int int32 = Convert.ToInt32(textureComponent.name);
                        if (Game1.player.Money >= int32)
                        {
                            Game1.globalFadeToBlack(this.SetUpForAnimalPlacement);
                            Game1.playSound("smallSelect");
                            this.OnFarm = true;
                            this.AnimalBeingPurchased = new FarmAnimal(textureComponent.hoverText, this.GetNewId(), Game1.player.UniqueMultiplayerID);
                            this.PriceOfAnimal = int32;
                        }
                        else
                            Game1.addHUDMessage(new HUDMessage("Not Enough Money", Color.Red, 3500f));
                    }
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.globalFade || this.Freeze)
                return;
            if (!Game1.globalFade && this.OnFarm)
            {
                if (this.NamingAnimal)
                    return;
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
                    Game1.globalFadeToBlack(this.SetUpForReturnToShopMenu);
                else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    Game1.panScreen(0, 4);
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    Game1.panScreen(4, 0);
                else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                {
                    Game1.panScreen(0, -4);
                }
                else
                {
                    if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                        return;
                    Game1.panScreen(-4, 0);
                }
            }
            else
            {
                if (!Game1.options.doesInputListContain(Game1.options.menuButton, key) || Game1.globalFade || !this.readyToClose())
                    return;
                Game1.player.forceCanMove();
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (!this.OnFarm || this.NamingAnimal)
                return;
            int num1 = Game1.getOldMouseX() + Game1.viewport.X;
            int num2 = Game1.getOldMouseY() + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < Game1.tileSize)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -Game1.tileSize)
                Game1.panScreen(8, 0);
            if (num2 - Game1.viewport.Y < Game1.tileSize)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -Game1.tileSize)
                Game1.panScreen(0, 8);
            foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
                this.receiveKeyPress(pressedKey);
        }

        public override void performHoverAction(int x, int y)
        {
            this.Hovered = null;
            if (Game1.globalFade || this.Freeze)
                return;
            if (this.OkButton != null)
            {
                this.OkButton.scale = this.OkButton.containsPoint(x, y)
                    ? Math.Min(1.1f, this.OkButton.scale + 0.05f)
                    : Math.Max(1f, this.OkButton.scale - 0.05f);
            }
            if (this.OnFarm)
            {
                Vector2 tile = new Vector2((x + Game1.viewport.X) / Game1.tileSize, (y + Game1.viewport.Y) / Game1.tileSize);
                Farm locationFromName = Game1.getFarm();
                foreach (Building building in locationFromName.buildings)
                    building.color.Value = Color.White;
                Building buildingAt = locationFromName.getBuildingAt(tile);
                if (buildingAt != null)
                    buildingAt.color.Value = !buildingAt.buildingType.Contains(this.AnimalBeingPurchased.buildingTypeILiveIn.Value) || ((AnimalHouse)buildingAt.indoors.Value).isFull() ? Color.Red * 0.8f : Color.LightGreen * 0.8f;
                if (this.DoneNamingButton != null)
                {
                    this.DoneNamingButton.scale = this.DoneNamingButton.containsPoint(x, y)
                        ? Math.Min(1.1f, this.DoneNamingButton.scale + 0.05f)
                        : Math.Max(1f, this.DoneNamingButton.scale - 0.05f);
                }
                this.RandomButton.tryHover(x, y, 0.5f);
            }
            else
            {
                foreach (ClickableTextureComponent textureComponent in this.AnimalsToPurchase)
                {
                    if (textureComponent.containsPoint(x, y))
                    {
                        textureComponent.scale = Math.Min(textureComponent.scale + 0.05f, 4.1f);
                        this.Hovered = textureComponent;
                    }
                    else
                        textureComponent.scale = Math.Max(4f, textureComponent.scale - 0.025f);
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!this.OnFarm && !Game1.dialogueUp && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                SpriteText.drawStringWithScrollBackground(b, "Livestock:", this.xPositionOnScreen + Game1.tileSize * 3 / 2, this.yPositionOnScreen);
                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
                Game1.dayTimeMoneyBox.drawMoneyBox(b);
                foreach (ClickableTextureComponent textureComponent in this.AnimalsToPurchase)
                    textureComponent.draw(b, ((Object)textureComponent.item).Type != null ? Color.Black * 0.4f : Color.White, 0.87f);

                this.BackButton.draw(b);
            }
            else if (!Game1.globalFade && this.OnFarm)
            {
                string s = "Choose a " + this.AnimalBeingPurchased.buildingTypeILiveIn.Value + " for your new " + this.AnimalBeingPurchased.type.Value.Split(' ').Last();
                SpriteText.drawStringWithScrollBackground(b, s, Game1.viewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, Game1.tileSize / 4);
                if (this.NamingAnimal)
                {
                    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                    Game1.drawDialogueBox(Game1.viewport.Width / 2 - Game1.tileSize * 4, Game1.viewport.Height / 2 - Game1.tileSize * 3 - Game1.tileSize / 2, Game1.tileSize * 8, Game1.tileSize * 3, false, true);
                    Utility.drawTextWithShadow(b, "Name your new animal: ", Game1.dialogueFont, new Vector2(Game1.viewport.Width / 2 - Game1.tileSize * 4 + Game1.tileSize / 2 + 8, Game1.viewport.Height / 2 - Game1.tileSize * 2 + 8), Game1.textColor);
                    this.TextBox.Draw(b);
                    this.DoneNamingButton.draw(b);
                    this.RandomButton.draw(b);
                }
            }
            if (!Game1.globalFade)
                OkButton?.draw(b);
            if (this.Hovered != null)
            {
                if (((Object)this.Hovered.item).Type != null)
                {
                    IClickableMenu.drawHoverText(b, Game1.parseText(((Object)this.Hovered.item).Type, Game1.dialogueFont, Game1.tileSize * 5), Game1.dialogueFont);
                }
                else
                {
                    SpriteText.drawStringWithScrollBackground(b, this.Hovered.hoverText, this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize, this.yPositionOnScreen + this.height + -Game1.tileSize / 2 + IClickableMenu.spaceToClearTopBorder / 2 + 8, "Truffle Pig");
                    SpriteText.drawStringWithScrollBackground(b, "$" + this.Hovered.name + "g", this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2, this.yPositionOnScreen + this.height + Game1.tileSize + IClickableMenu.spaceToClearTopBorder / 2 + 8, "$99999g", Game1.player.Money >= Convert.ToInt32(this.Hovered.name) ? 1f : 0.5f);
                    IClickableMenu.drawHoverText(b, Game1.parseText(this.GetAnimalDescription(this.Hovered.hoverText), Game1.smallFont, Game1.tileSize * 5), Game1.smallFont, 0, 0, -1, this.Hovered.hoverText);
                }
            }
            this.drawMouse(b);
        }


        /*********
        ** Private methods
        *********/
        private void TextBoxEnter(TextBox sender)
        {
            if (!this.NamingAnimal)
                return;
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is BuyAnimalMenu))
            {
                this.TextBox.OnEnterPressed -= this.TextBoxEvent;
            }
            else
            {
                if (sender.Text.Length < 1)
                    return;
                if (Utility.areThereAnyOtherAnimalsWithThisName(sender.Text))
                {
                    Game1.showRedMessage("Name Unavailable");
                }
                else
                {
                    this.TextBox.OnEnterPressed -= this.TextBoxEvent;
                    this.AnimalBeingPurchased.Name = sender.Text;
                    this.AnimalBeingPurchased.home = this.NewAnimalHome;
                    this.AnimalBeingPurchased.homeLocation.Value = new Vector2(this.NewAnimalHome.tileX.Value, this.NewAnimalHome.tileY.Value);
                    this.AnimalBeingPurchased.setRandomPosition(this.AnimalBeingPurchased.home.indoors.Value);
                    ((AnimalHouse)this.NewAnimalHome.indoors.Value).animals.Add(this.AnimalBeingPurchased.myID.Value, this.AnimalBeingPurchased);
                    ((AnimalHouse)this.NewAnimalHome.indoors.Value).animalsThatLiveHere.Add(this.AnimalBeingPurchased.myID.Value);
                    this.NewAnimalHome = null;
                    this.NamingAnimal = false;
                    Game1.player.Money -= this.PriceOfAnimal;
                    Game1.globalFadeToBlack(this.SetUpForReturnAfterPurchasingAnimal);
                }
            }
        }

        private void SetUpForReturnAfterPurchasingAnimal()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(this.WhereToGo);
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            this.OnFarm = false;
            this.OkButton.bounds.X = this.xPositionOnScreen + this.width + 4;
            Game1.displayHUD = true;
            Game1.displayFarmer = true;
            this.Freeze = false;
            this.TextBox.OnEnterPressed -= this.TextBoxEvent;
            this.TextBox.Selected = false;
            Game1.viewportFreeze = false;
            Game1.globalFadeToClear(this.MarnieAnimalPurchaseMessage);
        }

        private void MarnieAnimalPurchaseMessage()
        {
            this.exitThisMenu();
            Game1.player.forceCanMove();
            this.Freeze = false;

            Game1.activeClickableMenu = new BuyAnimalMenu(Utility.getPurchaseAnimalStock(), this.OnMenuOpened, this.GetNewId);
            this.OnMenuOpened();
        }

        private void BackButtonPressed()
        {
            if (this.readyToClose())
                this.exitThisMenu();
        }

        private void SetUpForAnimalPlacement()
        {
            Game1.displayFarmer = false;
            Game1.currentLocation = Game1.getFarm();
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            this.OnFarm = true;
            this.Freeze = false;
            this.OkButton.bounds.X = Game1.viewport.Width - Game1.tileSize * 2;
            this.OkButton.bounds.Y = Game1.viewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
        }

        private void SetUpForReturnToShopMenu()
        {
            this.Freeze = false;
            Game1.displayFarmer = true;
            Game1.currentLocation.cleanupBeforePlayerExit();
            //Game1.currentLocation = Game1.getLocationFromName("AnimalShop");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            this.OnFarm = false;
            this.OkButton.bounds.X = this.xPositionOnScreen + this.width + 4;
            this.OkButton.bounds.Y = this.yPositionOnScreen + this.height - Game1.tileSize - IClickableMenu.borderWidth;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            this.NamingAnimal = false;
            this.TextBox.OnEnterPressed -= this.TextBoxEvent;
            this.TextBox.Selected = false;
        }

        private string GetAnimalDescription(string name)
        {
            switch (name)
            {
                case "Chicken":
                    return "Well cared-for adult chickens lay eggs every day." + Environment.NewLine + "Lives in the coop.";
                case "Duck":
                    return "Happy adults lay duck eggs every other day." + Environment.NewLine + "Lives in the coop.";
                case "Rabbit":
                    return "These are wooly rabbits! They shed precious wool every few days." + Environment.NewLine + "Lives in the coop.";
                case "Dairy Cow":
                    return "Adults can be milked daily. A milk pail is required to harvest the milk." + Environment.NewLine + "Lives in the barn.";
                case "Pig":
                    return "These pigs are trained to find truffles!" + Environment.NewLine + "Lives in the barn.";
                case "Goat":
                    return "Happy adults provide goat milk every other day. A milk pail is required to harvest the milk." + Environment.NewLine + "Lives in the barn.";
                case "Sheep":
                    return "Adults can be shorn for wool. Sheep who form a close bond with their owners can grow wool faster. A pair of shears is required to harvest the wool." + Environment.NewLine + "Lives in the barn.";
                default:
                    return "";
            }
        }
    }
}
