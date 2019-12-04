using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace BetterSprinklers.Framework
{
    internal class SprinklerShapeEditMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private readonly SprinklerModConfig Config;

        /// <summary>The callback invoked when the sprinkler settings are changed.</summary>
        private readonly Action OnEdited;

        private readonly Texture2D WhitePixel;

        private readonly int MaxArraySize = 15;
        private readonly int DefaultTileSize = 32;
        private int ArraySize = 15;
        private int CenterTile = 7;
        private int TileSize = 32; //including padding
        private int Padding = 2;

        private int HoveredItemX;
        private int HoveredItemY;

        private readonly Color[] Colors = { Color.Tomato, Color.ForestGreen, Color.LightSteelBlue };

        private readonly int MinLeftMargin = 32;
        private readonly int MinTopMargin = 32;
        private int LeftMargin;
        private int TopMargin;

        private readonly int TabDistanceFromMenu = -32;
        private readonly int TabItemWidth = 64;
        private readonly int TabItemHeight = 64;
        private readonly int TabDistanceVerticalBetweenTabs = 16;

        private readonly int TabLeftMargin = 16;
        private readonly int TabVerticalMargins = 16;
        private readonly int TabRightMargin = 32;

        private int[,] SprinklerGrid;

        private int ActiveSprinklerSheet;

        private readonly List<ClickableComponent> Tabs;
        private readonly ClickableTextureComponent OkButton;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="modConfig">The mod configuration.</param>
        /// <param name="onEdited">The callback invoked when the sprinkler settings are changed.</param>
        public SprinklerShapeEditMenu(SprinklerModConfig modConfig, Action onEdited)
        {
            this.Config = modConfig;
            this.OnEdited = onEdited;

            int menuWidth = this.MaxArraySize * this.DefaultTileSize + this.MinLeftMargin * 2;
            int menuHeight = this.MaxArraySize * this.DefaultTileSize + this.MinTopMargin * 2;
            int menuX = Game1.viewport.Width / 2 - menuWidth / 2;
            int menuY = Game1.viewport.Height / 2 - menuHeight / 2;
            this.initialize(menuX, menuY, menuWidth, menuHeight, true);

            this.Tabs = new List<ClickableComponent>();
            int tabWidth = this.TabItemWidth + this.TabLeftMargin + this.TabRightMargin;
            int tabHeight = this.TabItemHeight + this.TabVerticalMargins * 2;
            this.Tabs.Add(new ClickableComponent(new Rectangle(menuX - this.TabDistanceFromMenu - tabWidth, menuY + tabHeight * 0 + this.TabDistanceVerticalBetweenTabs, tabWidth, tabHeight), new Object(Vector2.Zero, 599)));
            this.Tabs.Add(new ClickableComponent(new Rectangle(menuX - this.TabDistanceFromMenu - tabWidth, this.Tabs[0].bounds.Y + tabHeight + this.TabDistanceVerticalBetweenTabs, tabWidth, tabHeight), new Object(Vector2.Zero, 621)));
            this.Tabs.Add(new ClickableComponent(new Rectangle(menuX - this.TabDistanceFromMenu - tabWidth, this.Tabs[1].bounds.Y + tabHeight + this.TabDistanceVerticalBetweenTabs, tabWidth, tabHeight), new Object(Vector2.Zero, 645)));

            this.OkButton = new ClickableTextureComponent("save-changes", new Rectangle(this.xPositionOnScreen + this.width - Game1.tileSize / 2, this.yPositionOnScreen + this.height - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize), "", "Save Changes", Game1.mouseCursors, new Rectangle(128, 256, 64, 64), 1f);

            this.WhitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.WhitePixel.SetData(new[] { Color.White });

            this.SetActiveSprinklerSheetIndex(599);
        }

        public override void draw(SpriteBatch b)
        {
            foreach (ClickableComponent tab in this.Tabs)
            {
                IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), tab.bounds.X, tab.bounds.Y, tab.bounds.Width, tab.bounds.Height, Color.White);
                Game1.spriteBatch.Draw(Game1.objectSpriteSheet, new Rectangle(tab.bounds.X + this.TabLeftMargin, tab.bounds.Y + this.TabVerticalMargins, this.TabItemWidth, this.TabItemHeight), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tab.item.ParentSheetIndex, 16, 16), Color.White);
            }

            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);

            //draw our grid
            int countX = 0;
            int x;
            int y;

            if (this.HoveredItemX > -1 && this.HoveredItemY > -1)
            {
                x = this.xPositionOnScreen + this.LeftMargin + this.HoveredItemX * this.TileSize;
                y = this.yPositionOnScreen + this.TopMargin + this.HoveredItemY * this.TileSize;
                Game1.spriteBatch.Draw(this.WhitePixel, new Rectangle(x, y, this.TileSize, this.TileSize), Color.AntiqueWhite);
            }

            while (countX < this.ArraySize)
            {
                int countY = 0;
                while (countY < this.ArraySize)
                {
                    x = this.xPositionOnScreen + this.LeftMargin + this.Padding + countX * this.TileSize;
                    y = this.yPositionOnScreen + this.TopMargin + this.Padding + countY * this.TileSize;
                    Game1.spriteBatch.Draw(this.WhitePixel, new Rectangle(x, y, this.TileSize - this.Padding * 2, this.TileSize - this.Padding * 2), this.Colors[this.SprinklerGrid[countX, countY]]);
                    ++countY;
                }
                ++countX;
            }

            x = this.xPositionOnScreen + this.LeftMargin + this.Padding + this.CenterTile * this.TileSize;
            y = this.yPositionOnScreen + this.TopMargin + this.Padding + this.CenterTile * this.TileSize;
            Game1.spriteBatch.Draw(Game1.objectSpriteSheet, new Rectangle(x, y, this.TileSize - this.Padding * 2, this.TileSize - this.Padding * 2), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.ActiveSprinklerSheet, 16, 16), Color.White);
            this.OkButton.draw(Game1.spriteBatch);

            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);

            base.draw(b);
        }

        public override void update(GameTime time)
        {
            base.update(time);

            int mouseGridRelX = Game1.getOldMouseX() - this.xPositionOnScreen - this.LeftMargin - this.Padding;
            int mouseGridRelY = Game1.getOldMouseY() - this.yPositionOnScreen - this.TopMargin - this.Padding;

            if (mouseGridRelX > 0 && mouseGridRelY > 0 && mouseGridRelX < this.ArraySize * this.TileSize - this.Padding && mouseGridRelY < this.ArraySize * this.TileSize - this.Padding)
            {
                this.HoveredItemX = mouseGridRelX / this.TileSize;
                this.HoveredItemY = mouseGridRelY / this.TileSize;
            }
            else
            {
                this.HoveredItemX = -1;
                this.HoveredItemY = -1;
            }

            this.OkButton.tryHover(Game1.getOldMouseX(), Game1.getOldMouseY());
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            int mouseGridRelX = x - this.xPositionOnScreen - this.LeftMargin - this.Padding;
            int mouseGridRelY = y - this.yPositionOnScreen - this.TopMargin - this.Padding;

            if (mouseGridRelX > 0 && mouseGridRelY > 0 && mouseGridRelX < this.ArraySize * this.TileSize - this.Padding && mouseGridRelY < this.ArraySize * this.TileSize - this.Padding)
            {
                this.HoveredItemX = mouseGridRelX / this.TileSize;
                this.HoveredItemY = mouseGridRelY / this.TileSize;
                if (this.SprinklerGrid[this.HoveredItemX, this.HoveredItemY] != 2)
                {
                    this.SprinklerGrid[this.HoveredItemX, this.HoveredItemY] = 1 - this.SprinklerGrid[this.HoveredItemX, this.HoveredItemY];
                    Game1.playSound("select");
                }
            }
            else
            {
                this.HoveredItemX = -1;
                this.HoveredItemY = -1;

                foreach (ClickableComponent tab in this.Tabs)
                {
                    if (tab.containsPoint(x, y))
                    {
                        Game1.playSound("select");
                        this.SetActiveSprinklerSheetIndex(tab.item.ParentSheetIndex);
                    }
                }

                if (this.OkButton.containsPoint(x, y))
                {
                    Game1.playSound("select");
                    foreach (KeyValuePair<int, int[,]> sprinklerGrid in this.Config.SprinklerShapes)
                    {
                        int counter = 0;
                        int originalArea = 0;
                        foreach (int stateRequest in sprinklerGrid.Value)
                        {
                            if (stateRequest == 1) ++counter;
                            if (stateRequest == 2) ++originalArea;
                        }
                        this.Config.SprinklerPrices[sprinklerGrid.Key] = (counter / originalArea) + 1;
                    }
                    this.OnEdited();
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) { }


        /*********
        ** Private methods
        *********/
        private void SetActiveSprinklerSheetIndex(int type)
        {
            this.ActiveSprinklerSheet = type;

            this.HoveredItemX = -1;
            this.HoveredItemY = -1;

            this.SprinklerGrid = this.Config.SprinklerShapes[type];

            switch (type)
            {
                case 599:
                    this.ArraySize = 7;
                    this.CenterTile = this.ArraySize / 2;
                    this.TileSize = 64;

                    this.SprinklerGrid[this.CenterTile, this.CenterTile] = 2;
                    this.SprinklerGrid[this.CenterTile - 1, this.CenterTile] = 2;
                    this.SprinklerGrid[this.CenterTile + 1, this.CenterTile] = 2;
                    this.SprinklerGrid[this.CenterTile, this.CenterTile - 1] = 2;
                    this.SprinklerGrid[this.CenterTile, this.CenterTile + 1] = 2;
                    break;
                case 621:
                    this.TileSize = 32;
                    this.ArraySize = 11;
                    this.CenterTile = this.ArraySize / 2;

                    for (int x = this.CenterTile - 1; x < this.CenterTile + 2; x++)
                    {
                        for (int y = this.CenterTile - 1; y < this.CenterTile + 2; y++)
                            this.SprinklerGrid[x, y] = 2;
                    }
                    break;
                case 645:
                    this.TileSize = 32;
                    this.ArraySize = 15;
                    this.CenterTile = this.ArraySize / 2;

                    for (int x = this.CenterTile - 2; x < this.CenterTile + 3; x++)
                    {
                        for (int y = this.CenterTile - 2; y < this.CenterTile + 3; y++)
                            this.SprinklerGrid[x, y] = 2;
                    }
                    break;
            }

            this.LeftMargin = (this.width - (this.ArraySize * this.TileSize)) / 2;
            this.TopMargin = (this.height - (this.ArraySize * this.TileSize)) / 2;
        }
    }
}
