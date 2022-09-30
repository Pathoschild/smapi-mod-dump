/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using Custom_Farm_Loader.Lib;
using StardewValley.BellsAndWhistles;
using StardewValley.GameData;
using StardewModdingAPI;

namespace Custom_Farm_Loader.Menus
{
    public class CustomFarmSelection : IClickableMenu
    {
        private static Texture2D MissingMapIcon;
        private static Texture2D MissingMapPreview;
        private static Texture2D CurrentFarmPreview;

        private static List<ModFarmType> ModFarms = new List<ModFarmType>();

        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            MissingMapIcon = ModEntry._Helper.ModContent.Load<Texture2D>("assets/MissingMapIcon.png");
            MissingMapPreview = ModEntry._Helper.ModContent.Load<Texture2D>("assets/MissingMapPreview.png");
        }

        public const int Region_upArrow = 97865;
        public const int Region_downArrow = 97866;
        private Rectangle ScrollBarRunner;

        public ClickableTextureComponent UpArrow;
        public ClickableTextureComponent DownArrow;
        public ClickableTextureComponent ScrollBar;

        public const int ItemsPerPage = 4;
        private bool Scrolling;

        public List<ClickableComponent> CustomFarmButtons = new List<ClickableComponent>();
        private List<CustomFarm> CustomFarms = new List<CustomFarm>();
        private CustomFarm CurrentCustomFarm = null;

        public int currentItemIndex;

        protected int currentTabHover;

        public CustomFarmSelection(int default_selection_id) : base(Game1.uiViewport.Width / 2 - 320, Game1.uiViewport.Height - 64 - 192, 640, 192)
        {
            updatePosition();
            initializeUpperRightCloseButton();

            createClickTableTextures();
            applyTab();
            updateCustomFarmButtonNeighbors();

            populateClickableComponentList();

            if (CustomFarms.Count() > 0) {
                currentlySnappedComponent = CustomFarmButtons[0];
                if (Game1.options.SnappyMenus)
                    snapCursorToCurrentSnappedComponent();
            }

        }

        public void updateCustomFarmButtonNeighbors()
        {
            ClickableComponent last_valid_button = CustomFarmButtons[0];
            for (int i = 0; i < CustomFarmButtons.Count; i++) {
                ClickableComponent button = CustomFarmButtons[i];
                button.upNeighborImmutable = true;
                button.downNeighborImmutable = true;
                button.upNeighborID = ((i > 0) ? (i + 3546 - 1) : (-7777));
                button.downNeighborID = ((i < 3 && i < CustomFarms.Count - 1) ? (i + 3546 + 1) : (-7777));
                if (i >= CustomFarms.Count) {
                    if (button == currentlySnappedComponent) {
                        currentlySnappedComponent = last_valid_button;
                        if (Game1.options.SnappyMenus)
                            snapCursorToCurrentSnappedComponent();

                    }
                } else {
                    last_valid_button = button;
                }
            }
        }

        private void createClickTableTextures()
        {
            UpArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 16, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f) {
                myID = 97865,
                downNeighborID = 106,
                leftNeighborID = 3546
            };
            DownArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f) {
                myID = 106,
                upNeighborID = 97865,
                leftNeighborID = 3546
            };
            ScrollBar = new ClickableTextureComponent(new Rectangle(UpArrow.bounds.X + 12, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            ScrollBarRunner = new Rectangle(ScrollBar.bounds.X, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, ScrollBar.bounds.Width, height - 64 - UpArrow.bounds.Height - 28);

            for (int i = 0; i < ItemsPerPage; i++) {
                CustomFarmButtons.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + i * ((height - 256) / 4), width - 32, (height - 256) / 4 + 4), string.Concat(i)) {
                    myID = i + 3546,
                    rightNeighborID = 97865,
                    fullyImmutable = true
                });
            }
        }

        public void applyTab()
        {
            Game1.playSound("shwip");
            this.CustomFarms = CustomFarm.getAll();

            if (ModFarms.Count() == 0) {
                List<ModFarmType> modFarms = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");

                foreach (ModFarmType farm in modFarms) {
                    if (CustomFarm.get(farm.ID) != null)
                        continue;

                    ModFarms.Add(farm);

                    CustomFarm newCustomFarm = new CustomFarm(farm);

                    newCustomFarm.ID = farm.ID;
                    if (farm.MapName != "")
                        newCustomFarm.Name = farm.MapName.Replace("_", " ");
                    if (farm.TooltipStringPath != "")
                        newCustomFarm.Description = Game1.content.LoadString(farm.TooltipStringPath);
                    if (farm.IconTexture != "")
                        newCustomFarm.Icon = Helper.GameContent.Load<Texture2D>(farm.IconTexture);
                    if (farm.WorldMapTexture != null)
                        newCustomFarm.WorldMapOverlay = Helper.GameContent.Load<Texture2D>(farm.WorldMapTexture);
                    if (farm.ID.Contains("."))
                        newCustomFarm.Author = farm.ID.Split(".").First();

                    CustomFarms.Add(newCustomFarm);
                }

                CustomFarms = CustomFarms.OrderBy(o => o.Name).ToList();
            }

            if (Game1.whichFarm == 7)
                CurrentCustomFarm = CustomFarms.Find(e => e.ID == Game1.whichModFarm.ID);
            assignCurrentFarmPreview();

            currentItemIndex = 0;
            setScrollBarToCurrentIndex();
            updateCustomFarmButtonNeighbors();
        }

        private void assignCurrentFarmPreview()
        {
            CurrentFarmPreview = null;

            if (CurrentCustomFarm == null || CurrentCustomFarm.Preview == "")
                return;

            try {
                CurrentFarmPreview = ModEntry._Helper.ModContent.Load<Texture2D>(CurrentCustomFarm.Preview);
            } catch (Exception ex) {
                ModEntry._Monitor.Log($"Unable to load the map preview in:\n{CurrentCustomFarm.Preview}", StardewModdingAPI.LogLevel.Warn);
            }


        }

        public void updatePosition()
        {
            width = 1000 + IClickableMenu.borderWidth * 2;
            height = 600 + IClickableMenu.borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - (1000 + IClickableMenu.borderWidth * 2) / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
        }

        public override void snapToDefaultClickableComponent()
        {
            return;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            updatePosition();
            initializeUpperRightCloseButton();

            UpArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 16, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            DownArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            ScrollBar = new ClickableTextureComponent(new Rectangle(UpArrow.bounds.X + 12, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            ScrollBarRunner = new Rectangle(ScrollBar.bounds.X, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, ScrollBar.bounds.Width, height - 64 - UpArrow.bounds.Height - 28);
            for (int i = 0; i < 4; i++) {
                CustomFarmButtons.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + i * ((height - 256) / 4), width - 32, (height - 256) / 4 + 4), string.Concat(i)));
            }
        }

        private void downArrowPressed()
        {
            DownArrow.scale = DownArrow.baseScale;
            currentItemIndex++;
            setScrollBarToCurrentIndex();
            updateCustomFarmButtonNeighbors();
        }

        private void upArrowPressed()
        {
            if (currentItemIndex == 0)
                return;

            UpArrow.scale = UpArrow.baseScale;
            currentItemIndex--;
            setScrollBarToCurrentIndex();
            updateCustomFarmButtonNeighbors();
        }

        private void setScrollBarToCurrentIndex()
        {
            if (CustomFarms.Count > 0) {
                ScrollBar.bounds.Y = ScrollBarRunner.Height / Math.Max(1, CustomFarms.Count - 4 + 1) * currentItemIndex + UpArrow.bounds.Bottom + 4;
                if (currentItemIndex == CustomFarms.Count - 4) {
                    ScrollBar.bounds.Y = DownArrow.bounds.Y - ScrollBar.bounds.Height - 4;
                }
            }
        }

        public void selectListElement(CustomFarm customFarm)
        {
            Game1.playSound("coin");
            CurrentCustomFarm = customFarm;
            Game1.whichFarm = 7;
            if (ModFarms.Exists(e => e.ID == customFarm.ID))
                Game1.whichModFarm = ModFarms.Find(e => e.ID == customFarm.ID);
            else
                Game1.whichModFarm = CurrentCustomFarm.asModFarmType();
            Game1.spawnMonstersAtNight = customFarm.Properties.SpawnMonstersAtNight;
            assignCurrentFarmPreview();
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            exitThisMenu();
            base.receiveRightClick(x, y, playSound);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y);

            if (DownArrow.containsPoint(x, y) && currentItemIndex < Math.Max(0, CustomFarms.Count - 4)) {
                downArrowPressed();
                Game1.playSound("shwip");
            } else if (UpArrow.containsPoint(x, y)) {
                upArrowPressed();
                Game1.playSound("shwip");
            } else if (ScrollBar.containsPoint(x, y)) {
                Scrolling = true;
            } else if (base.upperRightCloseButton.containsPoint(x, y)) {
                exitThisMenu();
            } else if (!DownArrow.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height) {
                Scrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }


            for (int i = 0; i < CustomFarmButtons.Count; i++) {
                if (currentItemIndex + i >= CustomFarms.Count || !CustomFarmButtons[i].containsPoint(x, y))
                    continue;

                int index = currentItemIndex + i;

                selectListElement(CustomFarms[index]);

                updateCustomFarmButtonNeighbors();
                setScrollBarToCurrentIndex();
                return;
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (Scrolling) {
                int y2 = ScrollBar.bounds.Y;
                ScrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - ScrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + UpArrow.bounds.Height + 20));
                float percentage = (float)(y - ScrollBarRunner.Y) / (float)ScrollBarRunner.Height;
                currentItemIndex = Math.Min(Math.Max(0, CustomFarms.Count - 4), Math.Max(0, (int)((float)CustomFarms.Count * percentage)));
                setScrollBarToCurrentIndex();
                updateCustomFarmButtonNeighbors();
                if (y2 != ScrollBar.bounds.Y) {
                    Game1.playSound("shiny4");
                }
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            Scrolling = false;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && currentItemIndex > 0) {
                upArrowPressed();
                Game1.playSound("shiny4");
            } else if (direction < 0 && currentItemIndex < Math.Max(0, CustomFarms.Count - 4)) {
                downArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            switch (direction) {
                case 2: {
                        if (currentItemIndex < Math.Max(0, CustomFarms.Count - 4)) {
                            downArrowPressed();

                            currentlySnappedComponent = getComponentWithID(0);
                            snapCursorToCurrentSnappedComponent();
                        }
                        break;
                    }
                case 0:
                    if (currentItemIndex > 0) {
                        upArrowPressed();

                        currentlySnappedComponent = getComponentWithID(3546);
                        snapCursorToCurrentSnappedComponent();
                    }
                    break;
            }
        }

        public override bool readyToClose()
        {
            GamePadState currentPadState = Game1.input.GetGamePadState();

            if (((currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start)) || (currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B))))
                exitThisMenu();

            return false;
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height), Color.Black * 0.75f);

            //Top/Bottom Window w Borders
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height - 256 + 32 + 4, Color.White, 4f);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen + height - 256 + 40, width, height - 448, Color.White, 4f);

            draw_custom_farm_buttons(b);
            draw_farm_preview(b);

            if (CustomFarms.Count > 4) {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarRunner.X, ScrollBarRunner.Y, ScrollBarRunner.Width, ScrollBarRunner.Height, Color.White, 4f);
                ScrollBar.draw(b);
                UpArrow.draw(b);
                DownArrow.draw(b);
            }

            base.draw(b);
            drawMouse(b);
        }

        private void draw_custom_farm_buttons(SpriteBatch b)
        {
            for (int k = 0; k < CustomFarmButtons.Count; k++) {
                if (currentItemIndex + k >= CustomFarms.Count)
                    continue;

                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), CustomFarmButtons[k].bounds.X, CustomFarmButtons[k].bounds.Y, CustomFarmButtons[k].bounds.Width, CustomFarmButtons[k].bounds.Height, (CustomFarmButtons[k].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !Scrolling) ? Color.Wheat : Color.White, 4f, drawShadow: false);
                CustomFarm item = CustomFarms[currentItemIndex + k];

                //Icons
                Texture2D iconTexture = item.Icon != null ? item.Icon : MissingMapIcon;
                b.Draw(iconTexture, new Rectangle(CustomFarmButtons[k].bounds.X + 16, CustomFarmButtons[k].bounds.Y + 16, 72, 80), Color.White);

                //display Name
                string displayName = item.MaxPlayers > 0 ? $"{item.Name} ({item.MaxPlayers}P)" : item.Name;
                SpriteText.drawString(b, displayName, CustomFarmButtons[k].bounds.X + 96 + 8, CustomFarmButtons[k].bounds.Y + 28, 999999, -1, 999999, 1f, 0.88f, junimoText: false, -1, "", -1);

                if (item.ID == CurrentCustomFarm?.ID && item.Name == CurrentCustomFarm?.Name)
                    //Red farm selection rectangle
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), CustomFarmButtons[k].bounds.X + 4, CustomFarmButtons[k].bounds.Y + 4, CustomFarmButtons[k].bounds.Width - 8, CustomFarmButtons[k].bounds.Height - 8, Color.White, 4f, drawShadow: false);

            }

        }

        private void draw_farm_preview(SpriteBatch b)
        {
            if (CurrentCustomFarm == null)
                return;

            Texture2D previewTexture = CurrentFarmPreview == null ? CurrentCustomFarm.WorldMapOverlay : CurrentFarmPreview;
            if (previewTexture == null)
                previewTexture = MissingMapPreview;
            Rectangle previewRectangle = new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + height - 216 + 18, (int)Math.Round(width * 0.4), height - 448 - 32);
            b.Draw(previewTexture, previewRectangle, Color.White);


            string displayName = CurrentCustomFarm.MaxPlayers > 0 ? $"{CurrentCustomFarm.Name} ({CurrentCustomFarm.MaxPlayers}P)" : CurrentCustomFarm.Name;
            string title = $"{displayName} by {CurrentCustomFarm.Author}";
            SpriteText.drawStringWithScrollBackground(b, title, previewRectangle.X + 16, yPositionOnScreen + height - 250);

            string localizedDescription = Game1.content.LoadString(CurrentCustomFarm.asModFarmType().TooltipStringPath);

            //string description = localizedDescription.Count() > 70 ? localizedDescription.Substring(0, 67) + " (...)" : localizedDescription;

            if (localizedDescription.Count() > 70)
                drawDescription(b, localizedDescription,
                    x: previewRectangle.X + previewRectangle.Width + 16,
                    y: yPositionOnScreen + height - 216 + 30,
                    width: (int)Math.Round(width * 0.6) - 20);
            else
                SpriteText.drawString(b, localizedDescription,
                    x: previewRectangle.X + previewRectangle.Width + 16,
                    y: yPositionOnScreen + height - 216 + 30,
                    characterPosition: 999999,
                    width: (int)Math.Round(width * 0.6) - 20,
                    height: 1
                    );

        }

        public static void drawDescription(SpriteBatch b, string description, int x, int y, int width)
        {
            string descriptionString = Game1.parseText(description, Game1.smallFont, width);
            b.DrawString(Game1.smallFont, descriptionString, new Vector2(x, y), Game1.textColor * 0.75f);
        }

    }
}

