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
using System.Linq;
using Microsoft.Xna.Framework;
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
            : base(Game1.uiViewport.Width / 2 - MenuWidth / 2 - borderWidth * 2,
                Game1.uiViewport.Height / 2 - MenuHeight - borderWidth * 2, MenuWidth + borderWidth * 2,
                MenuHeight + borderWidth)
        {
            OnMenuOpened = onMenuOpened;
            GetNewId = getNewId;
            WhereToGo = Game1.player.currentLocation.Name;

            height += Game1.tileSize;
            for (var index = 0; index < stock.Count; ++index)
            {
                var animalsToPurchase = AnimalsToPurchase;
                var textureComponent1 = new ClickableTextureComponent(string.Concat(stock[index].salePrice()),
                    new Rectangle(xPositionOnScreen + borderWidth + index % 3 * Game1.tileSize * 2,
                        yPositionOnScreen + spaceToClearTopBorder + borderWidth / 2 +
                        index / 3 * (Game1.tileSize + Game1.tileSize / 3), Game1.tileSize * 2, Game1.tileSize), null,
                    stock[index].Name, Game1.mouseCursors,
                    new Rectangle(index % 3 * 16 * 2, 448 + index / 3 * 16, 32, 16), 4f, stock[index].Type == null);
                textureComponent1.item = stock[index];
                var textureComponent2 = textureComponent1;
                animalsToPurchase.Add(textureComponent2);
            }

            OkButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + 4, yPositionOnScreen + height - Game1.tileSize - borderWidth,
                    Game1.tileSize, Game1.tileSize), Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
            RandomButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + Game1.tileSize * 4 / 5 + Game1.tileSize,
                    Game1.uiViewport.Height / 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors,
                new Rectangle(381, 361, 10, 10), Game1.pixelZoom);
            TextBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor)
            {
                X = Game1.uiViewport.Width / 2 - Game1.tileSize * 3,
                Y = Game1.uiViewport.Height / 2,
                Width = Game1.tileSize * 4,
                Height = Game1.tileSize * 3
            };
            TextBoxEvent = TextBoxEnter;
            RandomButton = new ClickableTextureComponent(
                new Rectangle(TextBox.X + TextBox.Width + Game1.tileSize + Game1.tileSize * 3 / 4 - Game1.pixelZoom * 2,
                    Game1.uiViewport.Height / 2 + Game1.pixelZoom, Game1.tileSize, Game1.tileSize), Game1.mouseCursors,
                new Rectangle(381, 361, 10, 10), Game1.pixelZoom);
            DoneNamingButton = new ClickableTextureComponent(
                new Rectangle(TextBox.X + TextBox.Width + Game1.tileSize / 2 + Game1.pixelZoom,
                    Game1.uiViewport.Height / 2 - Game1.pixelZoom * 2, Game1.tileSize, Game1.tileSize),
                Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            BackButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen - 10, yPositionOnScreen + 10, 12 * Game1.pixelZoom,
                    11 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), Game1.pixelZoom);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade || Freeze)
                return;

            if (BackButton != null && BackButton.containsPoint(x, y))
            {
                BackButton.scale = BackButton.baseScale;
                BackButtonPressed();
            }

            if (OkButton != null && OkButton.containsPoint(x, y) && readyToClose())
            {
                if (OnFarm)
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

            if (OnFarm)
            {
                var building = Game1.getFarm().getBuildingAt(new Vector2((x + Game1.uiViewport.X) / Game1.tileSize,
                    (y + Game1.uiViewport.Y) / Game1.tileSize));
                if (building != null && !NamingAnimal)
                {
                    if (building.buildingType.Contains(AnimalBeingPurchased.buildingTypeILiveIn.Value))
                    {
                        var animalHouse = (AnimalHouse) building.indoors.Value;
                        if (animalHouse.isFull())
                        {
                            Game1.showRedMessage("That Building Is Full");
                        }
                        else if (AnimalBeingPurchased.harvestType.Value != 2)
                        {
                            NamingAnimal = true;
                            NewAnimalHome = building;
                            if (AnimalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                var cue = Game1.soundBank.GetCue(AnimalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }

                            TextBox.OnEnterPressed += TextBoxEvent;
                            Game1.keyboardDispatcher.Subscriber = TextBox;
                            TextBox.Text = AnimalBeingPurchased.Name;
                            TextBox.Selected = true;
                        }
                        else if (Game1.player.Money >= PriceOfAnimal)
                        {
                            NewAnimalHome = building;
                            AnimalBeingPurchased.home = NewAnimalHome;
                            AnimalBeingPurchased.homeLocation.Value =
                                new Vector2(NewAnimalHome.tileX.Value, NewAnimalHome.tileY.Value);
                            AnimalBeingPurchased.setRandomPosition(AnimalBeingPurchased.home.indoors.Value);
                            ((AnimalHouse) NewAnimalHome.indoors.Value).animals.Add(AnimalBeingPurchased.myID.Value,
                                AnimalBeingPurchased);
                            ((AnimalHouse) NewAnimalHome.indoors.Value).animalsThatLiveHere.Add(AnimalBeingPurchased
                                .myID.Value);
                            NewAnimalHome = null;
                            NamingAnimal = false;
                            if (AnimalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                var cue = Game1.soundBank.GetCue(AnimalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }

                            Game1.player.Money -= PriceOfAnimal;
                            Game1.addHUDMessage(new HUDMessage("Purchased " + AnimalBeingPurchased.type.Value,
                                Color.LimeGreen, 3500f));
                            AnimalBeingPurchased = new FarmAnimal(AnimalBeingPurchased.type.Value, GetNewId(),
                                Game1.player.UniqueMultiplayerID);
                        }
                        else if (Game1.player.Money < PriceOfAnimal)
                        {
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                        }
                    }
                    else
                    {
                        Game1.showRedMessage(AnimalBeingPurchased.type.Value.Split(' ').Last() + "s Can't Live There.");
                    }
                }

                if (NamingAnimal && DoneNamingButton.containsPoint(x, y))
                {
                    TextBoxEnter(TextBox);
                    Game1.playSound("smallSelect");
                }
                else
                {
                    if (!NamingAnimal || !RandomButton.containsPoint(x, y))
                        return;
                    AnimalBeingPurchased.Name = Dialogue.randomName();
                    TextBox.Text = AnimalBeingPurchased.Name;
                    RandomButton.scale = RandomButton.baseScale;
                    Game1.playSound("drumkit6");
                }
            }
            else
            {
                foreach (var textureComponent in AnimalsToPurchase)
                    if (textureComponent.containsPoint(x, y) && ((Object) textureComponent.item).Type == null)
                    {
                        var int32 = Convert.ToInt32(textureComponent.name);
                        if (Game1.player.Money >= int32)
                        {
                            Game1.globalFadeToBlack(SetUpForAnimalPlacement);
                            Game1.playSound("smallSelect");
                            OnFarm = true;
                            AnimalBeingPurchased = new FarmAnimal(textureComponent.hoverText, GetNewId(),
                                Game1.player.UniqueMultiplayerID);
                            PriceOfAnimal = int32;
                        }
                        else
                        {
                            Game1.addHUDMessage(new HUDMessage("Not Enough Money", Color.Red, 3500f));
                        }
                    }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.globalFade || Freeze)
                return;
            if (!Game1.globalFade && OnFarm)
            {
                if (NamingAnimal)
                    return;
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
                {
                    Game1.globalFadeToBlack(SetUpForReturnToShopMenu);
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                {
                    Game1.panScreen(0, 4);
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                {
                    Game1.panScreen(4, 0);
                }
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
                if (!Game1.options.doesInputListContain(Game1.options.menuButton, key) || Game1.globalFade ||
                    !readyToClose())
                    return;
                Game1.player.forceCanMove();
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (!OnFarm || NamingAnimal)
                return;
            var num1 = Game1.getOldMouseX() + Game1.uiViewport.X;
            var num2 = Game1.getOldMouseY() + Game1.uiViewport.Y;
            if (num1 - Game1.uiViewport.X < Game1.tileSize)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.uiViewport.X + Game1.uiViewport.Width) >= -Game1.tileSize)
                Game1.panScreen(8, 0);
            if (num2 - Game1.uiViewport.Y < Game1.tileSize)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.uiViewport.Y + Game1.uiViewport.Height) >= -Game1.tileSize)
                Game1.panScreen(0, 8);
            foreach (var pressedKey in Game1.oldKBState.GetPressedKeys())
                receiveKeyPress(pressedKey);
        }

        public override void performHoverAction(int x, int y)
        {
            Hovered = null;
            if (Game1.globalFade || Freeze)
                return;
            if (OkButton != null)
                OkButton.scale = OkButton.containsPoint(x, y)
                    ? Math.Min(1.1f, OkButton.scale + 0.05f)
                    : Math.Max(1f, OkButton.scale - 0.05f);
            if (OnFarm)
            {
                var tile = new Vector2((x + Game1.uiViewport.X) / Game1.tileSize,
                    (y + Game1.uiViewport.Y) / Game1.tileSize);
                var locationFromName = Game1.getFarm();
                foreach (var building in locationFromName.buildings)
                    building.color.Value = Color.White;
                var buildingAt = locationFromName.getBuildingAt(tile);
                if (buildingAt != null)
                    buildingAt.color.Value =
                        !buildingAt.buildingType.Contains(AnimalBeingPurchased.buildingTypeILiveIn.Value) ||
                        ((AnimalHouse) buildingAt.indoors.Value).isFull()
                            ? Color.Red * 0.8f
                            : Color.LightGreen * 0.8f;
                if (DoneNamingButton != null)
                    DoneNamingButton.scale = DoneNamingButton.containsPoint(x, y)
                        ? Math.Min(1.1f, DoneNamingButton.scale + 0.05f)
                        : Math.Max(1f, DoneNamingButton.scale - 0.05f);
                RandomButton.tryHover(x, y, 0.5f);
            }
            else
            {
                foreach (var textureComponent in AnimalsToPurchase)
                    if (textureComponent.containsPoint(x, y))
                    {
                        textureComponent.scale = Math.Min(textureComponent.scale + 0.05f, 4.1f);
                        Hovered = textureComponent;
                    }
                    else
                    {
                        textureComponent.scale = Math.Max(4f, textureComponent.scale - 0.025f);
                    }
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!OnFarm && !Game1.dialogueUp && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                SpriteText.drawStringWithScrollBackground(b, "Livestock:", xPositionOnScreen + Game1.tileSize * 3 / 2,
                    yPositionOnScreen);
                Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
                Game1.dayTimeMoneyBox.drawMoneyBox(b);
                foreach (var textureComponent in AnimalsToPurchase)
                    textureComponent.draw(b,
                        ((Object) textureComponent.item).Type != null ? Color.Black * 0.4f : Color.White, 0.87f);

                BackButton.draw(b);
            }
            else if (!Game1.globalFade && OnFarm)
            {
                var s = "Choose a " + AnimalBeingPurchased.buildingTypeILiveIn.Value + " for your new " +
                        AnimalBeingPurchased.type.Value.Split(' ').Last();
                SpriteText.drawStringWithScrollBackground(b, s,
                    Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, Game1.tileSize / 4);
                if (NamingAnimal)
                {
                    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                    Game1.drawDialogueBox(Game1.uiViewport.Width / 2 - Game1.tileSize * 4,
                        Game1.uiViewport.Height / 2 - Game1.tileSize * 3 - Game1.tileSize / 2, Game1.tileSize * 8,
                        Game1.tileSize * 3, false, true);
                    Utility.drawTextWithShadow(b, "Name your new animal: ", Game1.dialogueFont,
                        new Vector2(Game1.uiViewport.Width / 2 - Game1.tileSize * 4 + Game1.tileSize / 2 + 8,
                            Game1.uiViewport.Height / 2 - Game1.tileSize * 2 + 8), Game1.textColor);
                    TextBox.Draw(b);
                    DoneNamingButton.draw(b);
                    RandomButton.draw(b);
                }
            }

            if (!Game1.globalFade)
                OkButton?.draw(b);
            if (Hovered != null)
            {
                if (((Object) Hovered.item).Type != null)
                {
                    drawHoverText(b,
                        Game1.parseText(((Object) Hovered.item).Type, Game1.dialogueFont, Game1.tileSize * 5),
                        Game1.dialogueFont);
                }
                else
                {
                    SpriteText.drawStringWithScrollBackground(b, Hovered.hoverText,
                        xPositionOnScreen + spaceToClearSideBorder + Game1.tileSize,
                        yPositionOnScreen + height + -Game1.tileSize / 2 + spaceToClearTopBorder / 2 + 8,
                        "Truffle Pig");
                    SpriteText.drawStringWithScrollBackground(b, "$" + Hovered.name + "g",
                        xPositionOnScreen + spaceToClearSideBorder + Game1.tileSize * 2,
                        yPositionOnScreen + height + Game1.tileSize + spaceToClearTopBorder / 2 + 8, "$99999g",
                        Game1.player.Money >= Convert.ToInt32(Hovered.name) ? 1f : 0.5f);
                    drawHoverText(b,
                        Game1.parseText(GetAnimalDescription(Hovered.hoverText), Game1.smallFont, Game1.tileSize * 5),
                        Game1.smallFont, 0, 0, -1, Hovered.hoverText);
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
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is BuyAnimalMenu))
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
                    TextBox.OnEnterPressed -= TextBoxEvent;
                    AnimalBeingPurchased.Name = sender.Text;
                    AnimalBeingPurchased.home = NewAnimalHome;
                    AnimalBeingPurchased.homeLocation.Value =
                        new Vector2(NewAnimalHome.tileX.Value, NewAnimalHome.tileY.Value);
                    AnimalBeingPurchased.setRandomPosition(AnimalBeingPurchased.home.indoors.Value);
                    ((AnimalHouse) NewAnimalHome.indoors.Value).animals.Add(AnimalBeingPurchased.myID.Value,
                        AnimalBeingPurchased);
                    ((AnimalHouse) NewAnimalHome.indoors.Value).animalsThatLiveHere.Add(AnimalBeingPurchased.myID
                        .Value);
                    NewAnimalHome = null;
                    NamingAnimal = false;
                    Game1.player.Money -= PriceOfAnimal;
                    Game1.globalFadeToBlack(SetUpForReturnAfterPurchasingAnimal);
                }
            }
        }

        private void SetUpForReturnAfterPurchasingAnimal()
        {
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = Game1.getLocationFromName(WhereToGo);
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            OnFarm = false;
            OkButton.bounds.X = xPositionOnScreen + width + 4;
            Game1.displayHUD = true;
            Game1.displayFarmer = true;
            Freeze = false;
            TextBox.OnEnterPressed -= TextBoxEvent;
            TextBox.Selected = false;
            Game1.viewportFreeze = false;
            Game1.globalFadeToClear(MarnieAnimalPurchaseMessage);
        }

        private void MarnieAnimalPurchaseMessage()
        {
            exitThisMenu();
            Game1.player.forceCanMove();
            Freeze = false;

            Game1.activeClickableMenu = new BuyAnimalMenu(Utility.getPurchaseAnimalStock(), OnMenuOpened, GetNewId);
            OnMenuOpened();
        }

        private void BackButtonPressed()
        {
            if (readyToClose())
                exitThisMenu();
        }

        private void SetUpForAnimalPlacement()
        {
            Game1.displayFarmer = false;
            Game1.currentLocation = Game1.getFarm();
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            OnFarm = true;
            Freeze = false;
            OkButton.bounds.X = Game1.uiViewport.Width - Game1.tileSize * 2;
            OkButton.bounds.Y = Game1.uiViewport.Height - Game1.tileSize * 2;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.uiViewport.Location = new Location(49 * Game1.tileSize, 5 * Game1.tileSize);
            Game1.panScreen(0, 0);
        }

        private void SetUpForReturnToShopMenu()
        {
            Freeze = false;
            Game1.displayFarmer = true;
            Game1.currentLocation.cleanupBeforePlayerExit();
            //Game1.currentLocation = Game1.getLocationFromName("AnimalShop");
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();
            OnFarm = false;
            OkButton.bounds.X = xPositionOnScreen + width + 4;
            OkButton.bounds.Y = yPositionOnScreen + height - Game1.tileSize - borderWidth;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            NamingAnimal = false;
            TextBox.OnEnterPressed -= TextBoxEvent;
            TextBox.Selected = false;
        }

        private string GetAnimalDescription(string name)
        {
            switch (name)
            {
                case "Chicken":
                    return "Well cared-for adult chickens lay eggs every day." + Environment.NewLine +
                           "Lives in the coop.";
                case "Duck":
                    return "Happy adults lay duck eggs every other day." + Environment.NewLine + "Lives in the coop.";
                case "Rabbit":
                    return "These are wooly rabbits! They shed precious wool every few days." + Environment.NewLine +
                           "Lives in the coop.";
                case "Dairy Cow":
                    return "Adults can be milked daily. A milk pail is required to harvest the milk." +
                           Environment.NewLine + "Lives in the barn.";
                case "Pig":
                    return "These pigs are trained to find truffles!" + Environment.NewLine + "Lives in the barn.";
                case "Goat":
                    return
                        "Happy adults provide goat milk every other day. A milk pail is required to harvest the milk." +
                        Environment.NewLine + "Lives in the barn.";
                case "Sheep":
                    return
                        "Adults can be shorn for wool. Sheep who form a close bond with their owners can grow wool faster. A pair of shears is required to harvest the wool." +
                        Environment.NewLine + "Lives in the barn.";
                default:
                    return "";
            }
        }
    }
}