using FarmAnimalVarietyRedux.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux.UI
{
    /// <summary>The menu for purchasing an animal.</summary>
    public class AnimalPurchaseMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>The width of the menu.</summary>
        private int MenuWidth;

        /// <summary>The height of the menu.</summary>
        private int MenuHeight;

        /// <summary>The number of animal clickable components can fit on a row on a 720p -> 1080p resolution viewport</summary>
        private const int NumberOfIconsPerRow1280 = 5; // the number of icons that can fit in a row at 720p resolution

        /// <summary>The number of animal clickable components can fit on a row on a 1080p+ resolution viewport.</summary>
        private const int NumberOfIconsPerRow1920 = 9; // the number of icons that can fit in a row at 1080p resolution

        /// <summary>The width of an animal clickable component.</summary>
        private const int IconWidth = 32;

        /// <summary>The list of animal clickable components.</summary>
        /// <remarks>This must be public as the base game requires it to be when it looks for the clickable component fields for controller support.</remarks>
        public List<ClickableTextureComponent> AnimalComponents = new List<ClickableTextureComponent>();

        /// <summary>The animal clickable component currently being hovered on.</summary>
        private ClickableComponent HoveredItem;

        /// <summary>A collection of all the animal clickable components and whether the player is able to buy the animal. This is used for greying out unavailable animals etc.</summary>
        private Dictionary<ClickableComponent, bool> AnimalBuyable = new Dictionary<ClickableComponent, bool>();


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        public AnimalPurchaseMenu()
        {
            // determine the number of icons per row to use, this is to fill the screen as best as possible
            int numberOfIconsPerRow;
            if (Game1.graphics.GraphicsDevice.Viewport.Width >= 1920)
                numberOfIconsPerRow = NumberOfIconsPerRow1920;
            else
                numberOfIconsPerRow = NumberOfIconsPerRow1280;

            // calculate menu dimensions
            MenuWidth = IconWidth * Math.Min(numberOfIconsPerRow, ModEntry.Animals.Count) * 4 + (IClickableMenu.borderWidth * 2); // 4 is sprite scale
            int numberOfIconRows = (int)Math.Ceiling(ModEntry.Animals.Count / (decimal)numberOfIconsPerRow);
            MenuHeight = numberOfIconRows * 85 + 64 + (IClickableMenu.borderWidth * 2);

            // get the top left position for the background asset
            Vector2 backgroundTopLeftPosition = Utility.getTopLeftPositionForCenteringOnScreen(MenuWidth, MenuHeight);
            this.xPositionOnScreen = (int)backgroundTopLeftPosition.X;
            this.yPositionOnScreen = (int)backgroundTopLeftPosition.Y - MenuHeight / 3;

            // add a clickable texture for each animal
            for (int i = 0; i < ModEntry.Animals.Count; i++)
            {
                var animal = ModEntry.Animals.ElementAt(i);

                var shopIcon = animal.ShopIcon;
                var animalComponent = new ClickableTextureComponent(
                    name: animal.Name,
                    bounds: new Rectangle(
                        x: this.xPositionOnScreen + IClickableMenu.borderWidth + i % numberOfIconsPerRow * IconWidth * 4, // 4 is the sprite scale
                        y: this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + i / numberOfIconsPerRow * 85,
                        width: IconWidth * 4,
                        height: 16 * 4),
                    label: "",
                    hoverText: animal.Data.Description,
                    texture: shopIcon,
                    sourceRect: new Rectangle(0, 0, 32, 16),
                    scale: 4,
                    drawShadow: true
                );

                // calculate neighbours for controller support
                animalComponent.myID = i;
                animalComponent.upNeighborID = i - numberOfIconsPerRow;
                animalComponent.leftNeighborID = i % numberOfIconsPerRow == 0 ? -1 : i - 1;
                animalComponent.rightNeighborID = i % numberOfIconsPerRow == numberOfIconsPerRow - 1 ? -1 : i + 1;
                animalComponent.downNeighborID = i + numberOfIconsPerRow;

                AnimalComponents.Add(animalComponent);
                AnimalBuyable.Add(animalComponent, IsAnimalBuyable(animal));
            }

            if (Game1.options.SnappyMenus)
            {
                this.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        /// <summary>Invoked when the player starts navigating the menu with snappy menus enabled.</summary>
        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        /// <summary>Perform the cursor hover action.</summary>
        /// <param name="x">The X position of the cursor.</param>
        /// <param name="y">The Y position of the cursor.</param>
        public override void performHoverAction(int x, int y)
        {
            HoveredItem = null;

            // check which, if any, animal icon is currenly being hovered over
            foreach (var animalComponent in AnimalComponents)
            {
                if (animalComponent.containsPoint(x, y))
                {
                    animalComponent.scale = Math.Min(4.1f, animalComponent.scale + 0.05f);
                    HoveredItem = animalComponent;
                }
                else
                {
                    animalComponent.scale = Math.Max(4f, animalComponent.scale - 0.025f);
                }
            }
        }

        /// <summary>Invoked when the player resizes the viewport.</summary>
        /// <param name="oldBounds">The old viewport resolution.</param>
        /// <param name="newBounds">The new viewport resolution.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            Game1.activeClickableMenu = new AnimalPurchaseMenu();
        }

        /// <summary>The draw call for the menu.</summary>
        /// <param name="spriteBatch">The sprite batch to draw the menu to.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            // dark background
            spriteBatch.Draw(
                texture: Game1.fadeToBlackRect,
                destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                color: Color.Black * 0.75f
            );

            // menu background
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, MenuWidth, MenuHeight, false, true);

            // 'live stock' label
            SpriteText.drawStringWithScrollBackground(
                b: spriteBatch,
                s: "Livestock:",
                x: this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 64,
                y: this.yPositionOnScreen - 8,
                placeHolderWidthText: "",
                alpha: 1f,
                color: -1,
                scroll_text_alignment: SpriteText.ScrollTextAlignment.Center
            );

            // redraw the money box over the dark overlay
            Game1.dayTimeMoneyBox.drawMoneyBox(spriteBatch, -1, -1);

            // animal icons
            foreach (var animalComponent in AnimalComponents)
            {
                // if the animal is non buyable, it should be blacked out
                Color componentColour = Color.White;
                if (!AnimalBuyable[animalComponent])
                    componentColour = Color.Black * .4f;

                animalComponent.draw(
                    b: spriteBatch,
                    c: componentColour,
                    layerDepth: 0
                );
            }

            // hovered item tool tip and info
            if (HoveredItem != null)
            {
                var animal = ModEntry.Animals.Where(a => a.Name == HoveredItem.name).FirstOrDefault();

                // ensure the hovered item is buyable before displaying buying information
                if (AnimalBuyable[HoveredItem])
                {
                    // animal name
                    SpriteText.drawStringWithScrollBackground(
                        b: spriteBatch,
                        s: HoveredItem.name,
                        x: this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 64,
                        y: this.yPositionOnScreen + MenuHeight - 32 + IClickableMenu.spaceToClearTopBorder / 2
                    );

                    // animal price
                    SpriteText.drawStringWithScrollBackground(
                        b: spriteBatch,
                        s: $"${animal.Data.BuyPrice}g",
                        x: this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 64,
                        y: this.yPositionOnScreen + MenuHeight + 64 + IClickableMenu.spaceToClearTopBorder / 2 - 8
                    );
                }

                // tooltip
                if (AnimalBuyable[HoveredItem])
                {
                    IClickableMenu.drawHoverText(
                        b: spriteBatch,
                        text: Game1.parseText(
                            text: animal.Data.Description,
                            whichFont: Game1.smallFont,
                            width: 320),
                        font: Game1.smallFont,
                        boldTitleText: HoveredItem.name
                    );
                }
                else
                {
                    IClickableMenu.drawHoverText(
                        b: spriteBatch,
                        text: Game1.parseText(
                            text: $"Requires construction of one of the following buildings: {ConstructBuildingsString(animal)}",
                            whichFont: Game1.smallFont,
                            width: 320),
                        font: Game1.smallFont
                    );
                }
            }

            // cursor
            this.drawMouse(spriteBatch);
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Get whether the animal is buyable.</summary>
        /// <param name="animal">The animal to check.</param>
        /// <returns>Whether the player is able to buy the animal.</returns>
        private bool IsAnimalBuyable(Animal animal)
        {
            foreach (var building in animal.Data.Buildings)
            {
                if (Game1.getFarm().isBuildingConstructed(building))
                    return true;
            }

            return false;
        }

        /// <summary>Construct all the liveable buildings into a string for an animal.</summary>
        /// <param name="animal">The animal that contains the collection of buildings.</param>
        /// <returns>A string of concatenated buildings the animal can live in.</returns>
        private string ConstructBuildingsString(Animal animal)
        {
            string buildingsString = "";
            for (int i = 0; i < animal.Data.Buildings.Count; i++)
            {
                var building = animal.Data.Buildings[i];
                buildingsString += building;

                if (i != animal.Data.Buildings.Count - 1)
                    buildingsString += ", ";
            }

            return buildingsString;
        }
    }
}
