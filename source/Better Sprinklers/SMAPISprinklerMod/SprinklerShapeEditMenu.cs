using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SMAPISprinklerMod
{
    internal class SprinklerShapeEditMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        private static Texture2D whitePixel;

        private const int maxArraySize = 15;
        private const int defaultTileSize = 32;
        private int arraySize = 15;
        private int centerTile = 7;
        private int tileSize = 32; //including padding
        private int padding = 2;

        private int hoveredItemX;
        private int hoveredItemY;

        private readonly Color[] colors = { Color.Tomato, Color.ForestGreen, Color.LightSteelBlue };

        private const int minLeftMargin = 32;
        private const int minTopMargin = 32;
        private int leftMargin;
        private int topMargin;

        private const int tabDistanceFromMenu = -32;
        private const int tabItemWidth = 64;
        private const int tabItemHeight = 64;
        private const int tabDistanceVerticalBetweenTabs = 16;

        private const int tabLeftMargin = 16;
        private const int tabVerticalMargins = 16;
        private const int tabRightMargin = 32;

        private int[,] sprinklerGrid;

        private int activeSprinklerSheet;

        private readonly List<ClickableComponent> tabs;
        private readonly ClickableTextureComponent okButton;

        //OptionsInputListener thisMenuKeyInput;


        /*********
        ** Public methods
        *********/
        public SprinklerShapeEditMenu()
        {
            //thisMenuKeyInput = new OptionsInputListener("Key To Open This Screen", 1, 50, -1, -1);

            int menuWidth = maxArraySize * defaultTileSize + minLeftMargin * 2;
            int menuHeight = maxArraySize * defaultTileSize + minTopMargin * 2;
            int menuX = Game1.viewport.Width / 2 - menuWidth / 2;
            int menuY = Game1.viewport.Height / 2 - menuHeight / 2;
            initialize(menuX, menuY, menuWidth, menuHeight, true);

            tabs = new List<ClickableComponent>();
            int tabWidth = tabItemWidth + tabLeftMargin + tabRightMargin;
            int tabHeight = tabItemHeight + tabVerticalMargins * 2;
            tabs.Add(new ClickableComponent(new Rectangle(menuX - tabDistanceFromMenu - tabWidth, menuY + tabHeight * 0 + tabDistanceVerticalBetweenTabs, tabWidth, tabHeight), new StardewValley.Object(Vector2.Zero, 599, false)));
            tabs.Add(new ClickableComponent(new Rectangle(menuX - tabDistanceFromMenu - tabWidth, tabs[0].bounds.Y + tabHeight + tabDistanceVerticalBetweenTabs, tabWidth, tabHeight), new StardewValley.Object(Vector2.Zero, 621, false)));
            tabs.Add(new ClickableComponent(new Rectangle(menuX - tabDistanceFromMenu - tabWidth, tabs[1].bounds.Y + tabHeight + tabDistanceVerticalBetweenTabs, tabWidth, tabHeight), new StardewValley.Object(Vector2.Zero, 645, false)));

            okButton = new ClickableTextureComponent("save-changes", new Rectangle(xPositionOnScreen + width - Game1.tileSize / 2, yPositionOnScreen + height - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize), "", "Save Changes", Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1f);

            if (whitePixel == null)
            {
                whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                whitePixel.SetData(new Color[] { Color.White });
            }

            setActiveSprinklerSheetIndex(599);
        }

        public override void draw(SpriteBatch b)
        {
            foreach (ClickableComponent tab in tabs)
            {
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), tab.bounds.X, tab.bounds.Y, tab.bounds.Width, tab.bounds.Height, Color.White, 1f, true);
                Game1.spriteBatch.Draw(Game1.objectSpriteSheet, new Rectangle(tab.bounds.X + tabLeftMargin, tab.bounds.Y + tabVerticalMargins, tabItemWidth, tabItemHeight), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tab.item.parentSheetIndex, 16, 16), Color.White);
            }

            //Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false);
            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), xPositionOnScreen, yPositionOnScreen, this.width, this.height, Color.White, 1f, true);

            //draw our grid
            int countX = 0;
            int countY = 0;

            int x;
            int y;

            if (hoveredItemX > -1 && hoveredItemY > -1)
            {
                x = xPositionOnScreen + leftMargin + hoveredItemX * tileSize;
                y = yPositionOnScreen + topMargin + hoveredItemY * tileSize;
                Game1.spriteBatch.Draw(whitePixel, new Rectangle(x, y, tileSize, tileSize), Color.AntiqueWhite);
            }

            while (countX < arraySize)
            {
                countY = 0;
                while (countY < arraySize)
                {
                    x = xPositionOnScreen + leftMargin + padding + countX * tileSize;
                    y = yPositionOnScreen + topMargin + padding + countY * tileSize;
                    Game1.spriteBatch.Draw(whitePixel, new Rectangle(x, y, tileSize - padding * 2, tileSize - padding * 2), colors[sprinklerGrid[countX, countY]]);
                    ++countY;
                }
                ++countX;
            }

            x = xPositionOnScreen + leftMargin + padding + centerTile * tileSize;
            y = yPositionOnScreen + topMargin + padding + centerTile * tileSize;
            Game1.spriteBatch.Draw(Game1.objectSpriteSheet, new Rectangle(x, y, tileSize - padding * 2, tileSize - padding * 2), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, activeSprinklerSheet, 16, 16), Color.White);
            okButton.draw(Game1.spriteBatch);

            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);

            base.draw(b);
        }

        public override void update(GameTime time)
        {
            base.update(time);

            int mouseGridRelX = Game1.getOldMouseX() - xPositionOnScreen - leftMargin - padding;
            int mouseGridRelY = Game1.getOldMouseY() - yPositionOnScreen - topMargin - padding;

            if (mouseGridRelX > 0 && mouseGridRelY > 0 && mouseGridRelX < arraySize * tileSize - padding && mouseGridRelY < arraySize * tileSize - padding)
            {
                hoveredItemX = mouseGridRelX / tileSize;
                hoveredItemY = mouseGridRelY / tileSize;
            }
            else
            {
                hoveredItemX = -1;
                hoveredItemY = -1;
            }

            okButton.tryHover(Game1.getOldMouseX(), Game1.getOldMouseY());
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            int mouseGridRelX = x - xPositionOnScreen - leftMargin - padding;
            int mouseGridRelY = y - yPositionOnScreen - topMargin - padding;

            if (mouseGridRelX > 0 && mouseGridRelY > 0 && mouseGridRelX < arraySize * tileSize - padding && mouseGridRelY < arraySize * tileSize - padding)
            {
                hoveredItemX = mouseGridRelX / tileSize;
                hoveredItemY = mouseGridRelY / tileSize;
                if (sprinklerGrid[hoveredItemX, hoveredItemY] != 2)
                {
                    sprinklerGrid[hoveredItemX, hoveredItemY] = 1 - sprinklerGrid[hoveredItemX, hoveredItemY];
                    Game1.playSound("select");
                }
            }
            else
            {
                hoveredItemX = -1;
                hoveredItemY = -1;

                foreach (ClickableComponent tab in tabs)
                {
                    if (tab.containsPoint(x, y))
                    {
                        Game1.playSound("select");
                        setActiveSprinklerSheetIndex(tab.item.parentSheetIndex);
                    }
                }

                if (okButton.containsPoint(x, y))
                {
                    Game1.playSound("select");
                    int counter;
                    int originalArea;
                    foreach (KeyValuePair<int, int[,]> sprinklerGrid in SprinklerMod.SprinklerMod.ModConfig.SprinklerShapes)
                    {
                        counter = 0;
                        originalArea = 0;
                        foreach (int stateRequest in sprinklerGrid.Value)
                        {
                            if (stateRequest == 1) ++counter;
                            if (stateRequest == 2) ++originalArea;
                        }
                        SprinklerMod.SprinklerMod.ModConfig.SprinklerPrices[sprinklerGrid.Key] = (counter / originalArea) + 1;
                        //Log.Debug(String.Format("Sprinkler Type {0} has price {1}", sprinklerGrid.Key, SprinklerMod.SprinklerMod.ModConfig.sprinklerPrices[sprinklerGrid.Key]));
                    }
                    SprinklerMod.SprinklerMod.SaveConfig();
                    //Game1.showGlobalMessage("Sprinkler Configurations Saved");
                    Game1.addHUDMessage(new HUDMessage("Sprinkler Configurations Saved", Color.Green, 3500f));
                    SprinklerMod.SprinklerMod.UpdatePrices();
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }


        /*********
        ** Private methods
        *********/
        private void setActiveSprinklerSheetIndex(int type)
        {
            activeSprinklerSheet = type;

            int countX = 0;
            int countY = 0;

            hoveredItemX = -1;
            hoveredItemY = -1;

            sprinklerGrid = SprinklerMod.SprinklerMod.ModConfig.SprinklerShapes[type];

            switch (type)
            {
                case 599:
                    arraySize = 7;
                    centerTile = arraySize / 2;
                    tileSize = 64;

                    sprinklerGrid[centerTile, centerTile] = 2;
                    sprinklerGrid[centerTile - 1, centerTile] = 2;
                    sprinklerGrid[centerTile + 1, centerTile] = 2;
                    sprinklerGrid[centerTile, centerTile - 1] = 2;
                    sprinklerGrid[centerTile, centerTile + 1] = 2;
                    break;
                case 621:
                    tileSize = 32;
                    arraySize = 11;
                    //sprinklerGrid = new int[arraySize, arraySize];
                    centerTile = arraySize / 2;

                    countX = centerTile - 1;
                    countY = centerTile - 1;

                    while (countX < centerTile + 2)
                    {
                        countY = centerTile - 1;
                        while (countY < centerTile + 2)
                        {
                            sprinklerGrid[countX, countY] = 2;
                            ++countY;
                        }
                        ++countX;
                    }
                    break;
                case 645:
                    tileSize = 32;
                    arraySize = 15;
                    //sprinklerGrid = new int[arraySize, arraySize];
                    centerTile = arraySize / 2;

                    countX = centerTile - 2;
                    countY = centerTile - 2;

                    while (countX < centerTile + 3)
                    {
                        countY = centerTile - 2;
                        while (countY < centerTile + 3)
                        {
                            sprinklerGrid[countX, countY] = 2;
                            ++countY;
                        }
                        ++countX;
                    }
                    break;
            }

            leftMargin = (width - (arraySize * tileSize)) / 2;
            topMargin = (height - (arraySize * tileSize)) / 2;
        }
    }
}
