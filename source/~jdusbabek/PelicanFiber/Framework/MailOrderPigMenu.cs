using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace PelicanFiber.Framework
{
    internal class MailOrderPigMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        private static readonly int MenuHeight = Game1.tileSize * 5;
        private static readonly int MenuWidth = Game1.tileSize * 7;
        private readonly List<ClickableTextureComponent> AnimalsToPurchase = new List<ClickableTextureComponent>();
        private readonly ClickableTextureComponent OkButton;
        private readonly ClickableTextureComponent DoneNamingButton;
        private readonly ClickableTextureComponent RandomButton;
        private ClickableTextureComponent Hovered;
        private readonly ClickableTextureComponent BackButton;
        private bool NamingAnimal;
        private bool Freeze;
        private FarmAnimal AnimalBeingPurchased;
        private readonly TextBox TextBox;
        private readonly TextBoxEvent TextBoxEvent;
        private Building NewAnimalHome;
        private int PriceOfAnimal;
        private readonly ItemUtils ItemUtils;
        private readonly Action OnMenuOpened;
        private readonly Func<long> GetNewId;


        /*********
        ** Public methods
        *********/
        public MailOrderPigMenu(List<Object> stock, ItemUtils itemUtils, Action onMenuOpened, Func<long> getNewId)
          : base(Game1.viewport.Width / 2 - MailOrderPigMenu.MenuWidth / 2 - IClickableMenu.borderWidth * 2, Game1.viewport.Height / 2 - MailOrderPigMenu.MenuHeight - IClickableMenu.borderWidth * 2, MailOrderPigMenu.MenuWidth + IClickableMenu.borderWidth * 2, MailOrderPigMenu.MenuHeight + IClickableMenu.borderWidth)
        {
            this.ItemUtils = itemUtils;
            this.OnMenuOpened = onMenuOpened;
            this.GetNewId = getNewId;

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

            if (this.BackButton.containsPoint(x, y))
            {
                this.BackButton.scale = this.BackButton.baseScale;
                this.BackButtonPressed();
            }

            if (this.OkButton != null && this.OkButton.containsPoint(x, y) && this.readyToClose())
            {
                if (this.NamingAnimal)
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

            if (this.NamingAnimal)
            {
                this.TextBox.OnEnterPressed += this.TextBoxEvent;
                Game1.keyboardDispatcher.Subscriber = this.TextBox;
                //this.textBox.Text = this.animalBeingPurchased.name;
                this.TextBox.Selected = true;

                if (this.DoneNamingButton.containsPoint(x, y))
                {
                    this.AnimalBeingPurchased.Name = this.TextBox.Text;
                    this.TextBoxEnter(this.TextBox);
                    Game1.playSound("smallSelect");
                }
                else
                {
                    if (this.RandomButton.containsPoint(x, y))
                    {
                        this.AnimalBeingPurchased.Name = Dialogue.randomName();
                        this.TextBox.Text = this.AnimalBeingPurchased.Name;
                        this.RandomButton.scale = this.RandomButton.baseScale;
                        Game1.playSound("drumkit6");
                    }
                }
            }

            foreach (ClickableTextureComponent textureComponent in this.AnimalsToPurchase)
            {
                if (textureComponent.containsPoint(x, y) && ((Object)textureComponent.item).Type == null)
                {
                    int int32 = Convert.ToInt32(textureComponent.name);
                    if (Game1.player.Money >= int32)
                    {
                        //Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForAnimalPlacement), 0.02f);
                        //Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForAnimalPlacement), 0.02f);
                        Game1.playSound("smallSelect");
                        //this.onFarm = true;
                        this.AnimalBeingPurchased = new FarmAnimal(textureComponent.hoverText, this.GetNewId(), Game1.player.UniqueMultiplayerID);
                        this.PriceOfAnimal = int32;

                        //this.newAnimalHome = ((AnimalHouse)Game1.player.currentLocation).getBuilding();
                        //this.animalBeingPurchased.name = "John" + new Random().NextDouble();
                        //this.animalBeingPurchased.home = this.newAnimalHome;
                        //this.animalBeingPurchased.homeLocation = new Vector2((float)this.newAnimalHome.tileX, (float)this.newAnimalHome.tileY);
                        //this.animalBeingPurchased.setRandomPosition(this.animalBeingPurchased.home.indoors);
                        //(this.newAnimalHome.indoors as AnimalHouse).animals.Add(this.animalBeingPurchased.myID, this.animalBeingPurchased);
                        //(this.newAnimalHome.indoors as AnimalHouse).animalsThatLiveHere.Add(this.animalBeingPurchased.myID);
                        //this.newAnimalHome = (Building)null;
                        //this.namingAnimal = false;
                        //Game1.player.money -= this.priceOfAnimal;

                        //Game1.exitActiveMenu();
                        this.NamingAnimal = true;
                    }
                    else
                        Game1.addHUDMessage(new HUDMessage("Not Enough Money", Color.Red, 3500f));
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.globalFade || this.Freeze)
                return;

            if (!Game1.options.doesInputListContain(Game1.options.menuButton, key) || Game1.globalFade || !this.readyToClose())
                return;

            if (this.NamingAnimal)
                return;

            Game1.player.forceCanMove();
            Game1.exitActiveMenu();
            Game1.playSound("bigDeSelect");
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

            if (this.NamingAnimal)
            {
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
            if (!Game1.dialogueUp && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                SpriteText.drawStringWithScrollBackground(b, "Livestock:", this.xPositionOnScreen + Game1.tileSize * 3 / 2, this.yPositionOnScreen);
                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
                Game1.dayTimeMoneyBox.drawMoneyBox(b);
                foreach (ClickableTextureComponent textureComponent in this.AnimalsToPurchase)
                    textureComponent.draw(b, ((Object)textureComponent.item).Type != null ? Color.Black * 0.4f : Color.White, 0.87f);

                this.BackButton.draw(b);
            }
            if (!Game1.globalFade)
                OkButton?.draw(b);

            if (this.NamingAnimal)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                Game1.drawDialogueBox(Game1.viewport.Width / 2 - Game1.tileSize * 4, Game1.viewport.Height / 2 - Game1.tileSize * 3 - Game1.tileSize / 2, Game1.tileSize * 8, Game1.tileSize * 3, false, true);
                Utility.drawTextWithShadow(b, "Name your new animal: ", Game1.dialogueFont, new Vector2(Game1.viewport.Width / 2 - Game1.tileSize * 4 + Game1.tileSize / 2 + 8, Game1.viewport.Height / 2 - Game1.tileSize * 2 + 8), Game1.textColor);
                this.TextBox.Draw(b);
                this.DoneNamingButton.draw(b);
                this.RandomButton.draw(b);
            }

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
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is MailOrderPigMenu))
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
                    this.NewAnimalHome = ((AnimalHouse)Game1.player.currentLocation).getBuilding();
                    this.TextBox.OnEnterPressed -= this.TextBoxEvent;
                    this.AnimalBeingPurchased.Name = sender.Text;
                    //StardewLib.Log.ERROR("Named Animal: " + sender.Text);
                    this.AnimalBeingPurchased.home = ((AnimalHouse)Game1.player.currentLocation).getBuilding();
                    this.AnimalBeingPurchased.homeLocation.Value = new Vector2(this.NewAnimalHome.tileX.Value, this.NewAnimalHome.tileY.Value);
                    this.AnimalBeingPurchased.setRandomPosition(this.AnimalBeingPurchased.home.indoors.Value);
                    ((AnimalHouse)this.NewAnimalHome.indoors.Value).animals.Add(this.AnimalBeingPurchased.myID.Value, this.AnimalBeingPurchased);
                    ((AnimalHouse)this.NewAnimalHome.indoors.Value).animalsThatLiveHere.Add(this.AnimalBeingPurchased.myID.Value);
                    this.NewAnimalHome = null;
                    Game1.player.Money -= this.PriceOfAnimal;
                    this.NamingAnimal = false;

                    //Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForReturnAfterPurchasingAnimal), 0.02f);
                    Game1.globalFadeToClear();
                    this.OkButton.bounds.X = this.xPositionOnScreen + this.width + 4;
                    Game1.displayHUD = true;
                    Game1.displayFarmer = true;
                    this.Freeze = false;
                    this.TextBox.OnEnterPressed -= this.TextBoxEvent;
                    this.TextBox.Selected = false;
                    Game1.viewportFreeze = false;
                    Game1.globalFadeToClear(this.MarnieAnimalPurchaseMessage);
                }
            }
        }

        private void SetUpForReturnToShopMenu()
        {
            Game1.globalFadeToClear();
            this.OkButton.bounds.X = this.xPositionOnScreen + this.width + 4;
            this.OkButton.bounds.Y = this.yPositionOnScreen + this.height - Game1.tileSize - IClickableMenu.borderWidth;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            this.NamingAnimal = false;
            this.TextBox.OnEnterPressed -= this.TextBoxEvent;
            this.TextBox.Selected = false;
        }

        private void MarnieAnimalPurchaseMessage()
        {
            this.exitThisMenu();
            Game1.player.forceCanMove();
            this.Freeze = false;

            Game1.activeClickableMenu = new MailOrderPigMenu(this.ItemUtils.GetPurchaseAnimalStock(), this.ItemUtils, this.OnMenuOpened, this.GetNewId);
            this.OnMenuOpened();
        }

        private void BackButtonPressed()
        {
            if (this.readyToClose())
                this.exitThisMenu();
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
