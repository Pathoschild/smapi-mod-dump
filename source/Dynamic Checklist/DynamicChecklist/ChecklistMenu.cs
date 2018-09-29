namespace DynamicChecklist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DynamicChecklist.ObjectLists;
    using DynamicChecklist.Options;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.BellsAndWhistles;
    using StardewValley.Menus;

    public class ChecklistMenu : IClickableMenu
    {
        private static int iSelectedTab = 0;
        private Rectangle menuRect;
        private List<OptionsElement> options = new List<OptionsElement>();
        private List<ClickableComponent> tabs = new List<ClickableComponent>();
        private List<string> tabNames = new List<string> { "Checklist", "Settings" };
        private ClickableComponent selectedTab;
        private ModConfig config;

        public ChecklistMenu(ModConfig config)
        {
            this.config = config;

            Game1.playSound("bigSelect");
            this.menuRect = CreateCenteredRectangle(Game1.viewport, Game1.tileSize * 13, Game1.tileSize * 9);
            this.initialize(this.menuRect.X, this.menuRect.Y, this.menuRect.Width, this.menuRect.Height, true);

            int lblWidth = 150;
            int lblx = this.xPositionOnScreen - lblWidth;
            int lbly = this.yPositionOnScreen + 20;
            int lblSeperation = 80;
            int lblHeight = 60;
            int i = 0;
            foreach (string s in Enum.GetNames(typeof(TabName)))
            {
                this.tabs.Add(new ClickableComponent(new Rectangle(lblx, lbly + lblSeperation * i++, lblWidth, lblHeight), s));
            }

            this.selectedTab = this.tabs[iSelectedTab];
            int lineHeight;
            switch (this.selectedTab.name)
            {
                case "Checklist":
                    lineHeight = 50;
                    int j = 0;
                    foreach (ObjectList ol in ObjectLists)
                    {
                        if (ol.ShowInMenu)
                        {
                            var checkbox = new DynamicSelectableCheckbox(ol, this.menuRect.X + 50, this.menuRect.Y + 50 + lineHeight * j);
                            this.options.Add(checkbox);
                            j++;
                        }
                    }

                    break;
                case "Settings":
                    lineHeight = 65;
                    this.options.Add(new DCOptionsCheckbox("Show All Tasks", 3, config, this.menuRect.X + 50, this.menuRect.Y + 50 + lineHeight * 0));
                    this.options.Add(new DCOptionsCheckbox("Allow Multiple Overlays", 4, config, this.menuRect.X + 50, this.menuRect.Y + 50 + lineHeight * 1));
                    this.options.Add(new DCOptionsCheckbox("Show Arrow to Nearest Task", 5, config, this.menuRect.X + 50, this.menuRect.Y + 50 + lineHeight * 2));
                    this.options.Add(new DCOptionsCheckbox("Show Task Overlay", 6, config, this.menuRect.X + 50, this.menuRect.Y + 50 + lineHeight * 3));
                    this.options.Add(new DCOptionsDropDown("Button Position", 1, config, this.menuRect.X + 50, this.menuRect.Y + 50 + lineHeight * 4));
                    this.options.Add(new DCOptionsInputListener("Open Menu Key", 2, this.menuRect.Width - 50, config, this.menuRect.X + 50, this.menuRect.Y + 50 + lineHeight * 5));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private enum TabName
        {
            Checklist, Settings
        }

        public static List<ObjectList> ObjectLists { get; set; }

        public static void Open(ModConfig config)
        {
            Game1.activeClickableMenu = new ChecklistMenu(config);
        }

        public override void receiveKeyPress(Keys key)
        {
            foreach (OptionsElement o in this.options)
            {
                o.receiveKeyPress(key);
            }

            base.receiveKeyPress(key);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, "Checklist", this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen - Game1.tileSize, string.Empty, 1f, -1, 0, 0.88f, false);
            drawTextureBox(Game1.spriteBatch, this.menuRect.X, this.menuRect.Y, this.menuRect.Width, this.menuRect.Height, Color.White);
            var mouseX = Game1.getMouseX();
            var mouseY = Game1.getMouseY();

            int j = 0;
            foreach (ClickableComponent t in this.tabs)
            {
                drawTextureBox(Game1.spriteBatch, t.bounds.X, t.bounds.Y, t.bounds.Width, t.bounds.Height, Color.White * (iSelectedTab == j ? 1F : 0.7F));
                b.DrawString(Game1.smallFont, t.name, new Vector2(t.bounds.X + 15, t.bounds.Y + 15), Color.Black);
                j++;
            }

            foreach (OptionsElement o in this.options)
            {
                o.draw(b, -1, -1);
            }

            base.draw(b);
            this.drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            for (int i = 0; i < this.tabs.Count; i++)
            {
                if (this.tabs[i].bounds.Contains(x, y))
                {
                    iSelectedTab = i;
                    Game1.activeClickableMenu = new ChecklistMenu(this.config);
                }
            }

            foreach (OptionsElement o in this.options)
            {
                if (o.bounds.Contains(x, y))
                {
                    o.receiveLeftClick(x, y);
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            foreach (OptionsElement o in this.options)
            {
                o.leftClickHeld(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            foreach (OptionsElement o in this.options)
            {
                o.leftClickReleased(x, y);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = false)
        {
        }

        // TODO: Controller support
        public override void setUpForGamePadMode()
        {
            base.setUpForGamePadMode();
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            base.snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            base.customSnapBehavior(direction, oldRegion, oldID);
        }

        private static Rectangle CreateCenteredRectangle(xTile.Dimensions.Rectangle v, int width, int height)
        {
            var x = v.Width / 2 - width / 2;
            var y = v.Height / 2 - height / 2;
            return new Rectangle(x, y, width, height);
        }
    }
}