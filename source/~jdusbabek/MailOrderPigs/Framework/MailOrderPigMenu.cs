/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

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

namespace MailOrderPigs.Framework
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
        private bool NamingAnimal;
        private bool Freeze;
        private FarmAnimal AnimalBeingPurchased;
        private readonly TextBox TextBox;
        private readonly TextBoxEvent TextBoxEvent;
        private Building NewAnimalHome;
        private int PriceOfAnimal;
        private readonly Func<long> GetNewId;


        /*********
        ** Public methods
        *********/
        public MailOrderPigMenu(List<Object> stock, Func<long> getNewId)
          : base(Game1.viewport.Width / 2 - MenuWidth / 2 - borderWidth * 2, Game1.viewport.Height / 2 - MenuHeight - borderWidth * 2, MenuWidth + borderWidth * 2, MenuHeight + borderWidth)
        {
            GetNewId = getNewId;

            height += Game1.tileSize;
            for (int index = 0; index < stock.Count; ++index)
            {
                List<ClickableTextureComponent> animalsToPurchase = AnimalsToPurchase;
                ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(string.Concat(stock[index].salePrice()), new Rectangle(xPositionOnScreen + borderWidth + index % 3 * Game1.tileSize * 2, yPositionOnScreen + spaceToClearTopBorder + borderWidth / 2 + index / 3 * (Game1.tileSize + Game1.tileSize / 3), Game1.tileSize * 2, Game1.tileSize), null, stock[index].Name, Game1.mouseCursors, new Rectangle(index % 3 * 16 * 2, 448 + index / 3 * 16, 32, 16), 4f, stock[index].Type == null);
                textureComponent1.item = stock[index];
                ClickableTextureComponent textureComponent2 = textureComponent1;
                animalsToPurchase.Add(textureComponent2);
            }
            OkButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 4, yPositionOnScreen + height - Game1.tileSize - borderWidth, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            RandomButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game1.tileSize * 4 / 5 + Game1.tileSize, Game1.viewport.Height / 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), Game1.pixelZoom);
            TextBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor)
            {
                X = Game1.viewport.Width / 2 - Game1.tileSize * 3,
                Y = Game1.viewport.Height / 2,
                Width = Game1.tileSize * 4,
                Height = Game1.tileSize * 3
            };
            TextBoxEvent = TextBoxEnter;
            RandomButton = new ClickableTextureComponent(new Rectangle(TextBox.X + TextBox.Width + Game1.tileSize + Game1.tileSize * 3 / 4 - Game1.pixelZoom * 2, Game1.uiViewport.Height / 2 + Game1.pixelZoom, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), Game1.pixelZoom);
            DoneNamingButton = new ClickableTextureComponent(new Rectangle(TextBox.X + TextBox.Width + Game1.tileSize / 2 + Game1.pixelZoom, Game1.uiViewport.Height / 2 - Game1.pixelZoom * 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade || Freeze)
                return;

            if (OkButton != null && OkButton.containsPoint(x, y) && readyToClose())
            {
                if (NamingAnimal)
                {
                    Game1.globalFadeToBlack(SetUpForReturnToShopMenu);
                    Game1.playSound("smallSelect");
                }
                else
                {
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                }
            }

            if (NamingAnimal)
            {
                TextBox.OnEnterPressed += TextBoxEvent;
                Game1.keyboardDispatcher.Subscriber = TextBox;
                //this.textBox.Text = this.animalBeingPurchased.name;
                TextBox.Selected = true;

                if (DoneNamingButton.containsPoint(x, y))
                {
                    AnimalBeingPurchased.Name = TextBox.Text;
                    TextBoxEnter(TextBox);
                    Game1.playSound("smallSelect");
                }
                else
                {
                    if (RandomButton.containsPoint(x, y))
                    {
                        AnimalBeingPurchased.Name = Dialogue.randomName();
                        TextBox.Text = AnimalBeingPurchased.Name;
                        RandomButton.scale = RandomButton.baseScale;
                        Game1.playSound("drumkit6");
                    }
                }
            }

            foreach (ClickableTextureComponent textureComponent in AnimalsToPurchase)
            {
                if (textureComponent.containsPoint(x, y) && ((Object)textureComponent.item).Type == null)
                {
                    int price = Convert.ToInt32(textureComponent.name);
                    if (Game1.player.Money >= price)
                    {
                        //Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForAnimalPlacement), 0.02f);
                        //Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForAnimalPlacement), 0.02f);
                        Game1.playSound("smallSelect");
                        //this.onFarm = true;
                        AnimalBeingPurchased = new FarmAnimal(textureComponent.hoverText, GetNewId(), Game1.player.UniqueMultiplayerID);
                        PriceOfAnimal = price;

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
                        NamingAnimal = true;
                    }
                    else
                        Game1.addHUDMessage(new HUDMessage("Not Enough Money", Color.Red, 3500f));
                }
            }

        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.globalFade || Freeze)
                return;

            if (!Game1.options.doesInputListContain(Game1.options.menuButton, key) || Game1.globalFade || !readyToClose())
                return;

            if (NamingAnimal)
                return;

            Game1.player.forceCanMove();
            Game1.exitActiveMenu();
            Game1.playSound("bigDeSelect");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            Hovered = null;
            if (Game1.globalFade || Freeze)
                return;
            if (OkButton != null)
            {
                OkButton.scale = OkButton.containsPoint(x, y)
                    ? Math.Min(1.1f, OkButton.scale + 0.05f)
                    : Math.Max(1f, OkButton.scale - 0.05f);
            }

            if (NamingAnimal)
            {
                if (DoneNamingButton != null)
                {
                    DoneNamingButton.scale = DoneNamingButton.containsPoint(x, y)
                        ? Math.Min(1.1f, DoneNamingButton.scale + 0.05f)
                        : Math.Max(1f, DoneNamingButton.scale - 0.05f);
                }
                RandomButton.tryHover(x, y, 0.5f);
            }
            else
            {
                foreach (ClickableTextureComponent textureComponent in AnimalsToPurchase)
                {
                    if (textureComponent.containsPoint(x, y))
                    {
                        textureComponent.scale = Math.Min(textureComponent.scale + 0.05f, 4.1f);
                        Hovered = textureComponent;
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
                SpriteText.drawStringWithScrollBackground(b, "Livestock:", xPositionOnScreen + Game1.tileSize * 3 / 2, yPositionOnScreen);
                Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
                Game1.dayTimeMoneyBox.drawMoneyBox(b);
                foreach (ClickableTextureComponent textureComponent in AnimalsToPurchase)
                    textureComponent.draw(b, ((Object)textureComponent.item).Type != null ? Color.Black * 0.4f : Color.White, 0.87f);
            }
            if (!Game1.globalFade)
                OkButton?.draw(b);

            if (NamingAnimal)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                Game1.drawDialogueBox(Game1.uiViewport.Width / 2 - Game1.tileSize * 4, Game1.uiViewport.Height / 2 - Game1.tileSize * 3 - Game1.tileSize / 2, Game1.tileSize * 8, Game1.tileSize * 3, false, true);
                Utility.drawTextWithShadow(b, "Name your new animal: ", Game1.dialogueFont, new Vector2(Game1.uiViewport.Width / 2 - Game1.tileSize * 4 + Game1.tileSize / 2 + 8, Game1.uiViewport.Height / 2 - Game1.tileSize * 2 + 8), Game1.textColor);
                TextBox.Draw(b);
                DoneNamingButton.draw(b);
                RandomButton.draw(b);
            }

            if (Hovered != null)
            {
                if (((Object)Hovered.item).Type != null)
                {
                    drawHoverText(b, Game1.parseText(((Object)Hovered.item).Type, Game1.dialogueFont, Game1.tileSize * 5), Game1.dialogueFont);
                }
                else
                {
                    SpriteText.drawStringWithScrollBackground(b, Hovered.hoverText, xPositionOnScreen + spaceToClearSideBorder + Game1.tileSize, yPositionOnScreen + height + -Game1.tileSize / 2 + spaceToClearTopBorder / 2 + 8, "Truffle Pig");
                    SpriteText.drawStringWithScrollBackground(b, "$" + Hovered.name + "g", xPositionOnScreen + spaceToClearSideBorder + Game1.tileSize * 2, yPositionOnScreen + height + Game1.tileSize + spaceToClearTopBorder / 2 + 8, "$99999g", Game1.player.Money >= Convert.ToInt32(Hovered.name) ? 1f : 0.5f);
                    drawHoverText(b, Game1.parseText(GetAnimalDescription(Hovered.hoverText), Game1.smallFont, Game1.tileSize * 5), Game1.smallFont, 0, 0, -1, Hovered.hoverText);
                }
            }
            drawMouse(b);
        }


        /*********
        ** Private methods
        *********/
        private void TextBoxEnter(TextBox sender)
        {
            if (!NamingAnimal)
                return;
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is MailOrderPigMenu))
            {
                TextBox.OnEnterPressed -= TextBoxEvent;
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
                    NewAnimalHome = ((AnimalHouse)Game1.player.currentLocation).getBuilding();
                    TextBox.OnEnterPressed -= TextBoxEvent;
                    AnimalBeingPurchased.Name = sender.Text;
                    //StardewLib.Log.ERROR("Named Animal: " + sender.Text);
                    AnimalBeingPurchased.home = ((AnimalHouse)Game1.player.currentLocation).getBuilding();
                    AnimalBeingPurchased.homeLocation.Value = new Vector2(NewAnimalHome.tileX.Value, NewAnimalHome.tileY.Value);
                    AnimalBeingPurchased.setRandomPosition(AnimalBeingPurchased.home.indoors.Value);
                    ((AnimalHouse)NewAnimalHome.indoors.Value).animals.Add(AnimalBeingPurchased.myID.Value, AnimalBeingPurchased);
                    ((AnimalHouse)NewAnimalHome.indoors.Value).animalsThatLiveHere.Add(AnimalBeingPurchased.myID.Value);
                    NewAnimalHome = null;
                    Game1.player.Money -= PriceOfAnimal;
                    NamingAnimal = false;

                    //Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForReturnAfterPurchasingAnimal), 0.02f);
                    Game1.globalFadeToClear();
                    OkButton.bounds.X = xPositionOnScreen + width + 4;
                    Game1.displayHUD = true;
                    Game1.displayFarmer = true;
                    Freeze = false;
                    TextBox.OnEnterPressed -= TextBoxEvent;
                    TextBox.Selected = false;
                    Game1.viewportFreeze = false;
                    Game1.globalFadeToClear(MarnieAnimalPurchaseMessage);
                }
            }
        }

        private void SetUpForReturnToShopMenu()
        {
            Game1.globalFadeToClear();
            OkButton.bounds.X = xPositionOnScreen + width + 4;
            OkButton.bounds.Y = yPositionOnScreen + height - Game1.tileSize - borderWidth;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            NamingAnimal = false;
            TextBox.OnEnterPressed -= TextBoxEvent;
            TextBox.Selected = false;
        }

        private void MarnieAnimalPurchaseMessage()
        {
            exitThisMenu();
            Game1.player.forceCanMove();
            Freeze = false;
        }

        private static string GetAnimalDescription(string name)
        {
            return name switch
            {
                "Chicken" => Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11334") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335"), 
                "Duck" => Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11337") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335"), 
                "Rabbit" => Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11340") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335"), 
                "Dairy Cow" => Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11343") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344"), 
                "Pig" => Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11346") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344"), 
                "Goat" => Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11349") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344"), 
                "Sheep" => Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11352") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344"), 
                "Ostrich" => Game1.content.LoadString("Strings\\StringsFromCSFiles:Ostrich_Description") + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344"), 
                _ => "", 
            };
        }
    }
}