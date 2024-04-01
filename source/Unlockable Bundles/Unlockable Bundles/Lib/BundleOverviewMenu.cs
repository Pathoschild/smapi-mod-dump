/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using Unlockable_Bundles.Lib.Enums;
using Unlockable_Bundles.Lib.ShopTypes;
using static StardewValley.LocalizedContentManager;

namespace Unlockable_Bundles.Lib
{
    public class BundleOverviewMenu : IClickableMenu
    {
        private static Texture2D BGTexture;

        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            BGTexture = Helper.ModContent.Load<Texture2D>("assets\\BundleOverviewBG.png");
        }

        private Rectangle ScrollBarRunner;

        public ClickableTextureComponent UpArrow;
        public ClickableTextureComponent DownArrow;
        public ClickableTextureComponent ScrollBar;

        public const int ListElementsPerPage = 4;
        public int DescriptionRowsPerPage = 3;
        public const int RequirementsPerPage = 10;
        private bool Scrolling;

        public List<ClickableComponent> ListElement = new List<ClickableComponent>();
        private List<ShopObject> Bundles = new();
        public ShopObject CurrentBundle = null;

        public int currentItemIndex;

        protected int currentTabHover;


        protected int DescriptionScrollIndex = 0;
        protected string[] SplitPreviewDescription = new string[] { };
        public ClickableTextureComponent DescriptionUpArrow;
        public ClickableTextureComponent DescriptionDownArrow;

        protected int RequirementPageIndex = 0;
        public List<ClickableRequirementTexture> RequirementIcons = new();
        private ClickableRequirementTexture HoveredComponent = null;
        public ClickableTextureComponent RequirementUpArrow;
        public ClickableTextureComponent RequirementDownArrow;

        private Item NextItem = null;
        private string NextId = "";

        public BundleOverviewMenu() : base(Game1.uiViewport.Width / 2 - 320, Game1.uiViewport.Height - 64 - 192, 640, 192)
        {
            updatePosition();
            initializeUpperRightCloseButton();

            createClickTableTextures();
            applyTab();
            updateListElementNeighbors();

            populateClickableComponentList();

            if (Bundles.Count() > 0) {
                currentlySnappedComponent = ListElement[0];
                if (Game1.options.SnappyMenus)
                    snapCursorToCurrentSnappedComponent();
            }

            calculateDescriptionRowsPerPage();
        }

        public void calculateDescriptionRowsPerPage()
        {
            DescriptionRowsPerPage = Helper.GameContent.CurrentLocaleConstant switch {
                LanguageCode.de => 4,
                LanguageCode.en => 3,
                LanguageCode.es => 3,
                LanguageCode.fr => 3,
                LanguageCode.hu => 3,
                LanguageCode.it => 3,
                LanguageCode.ja => 4,
                LanguageCode.ko => 2,
                LanguageCode.pt => 3,
                LanguageCode.ru => 4,
                LanguageCode.tr => 3,
                LanguageCode.zh => 4,
                _ => 3
            };
        }

        public void updateListElementNeighbors()
        {
            ClickableComponent last_valid_button = ListElement[0];
            for (int i = 0; i < ListElement.Count; i++) {
                ClickableComponent button = ListElement[i];
                button.upNeighborImmutable = true;
                button.downNeighborImmutable = true;
                button.upNeighborID = ((i > 0) ? (i + 3546 - 1) : (-7777));
                button.downNeighborID = ((i < 3 && i < Bundles.Count - 1) ? (i + 3546 + 1) : (-7777));
                if (i >= Bundles.Count) {
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
            DownArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 260, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f) {
                myID = 106,
                upNeighborID = 97865,
                downNeighborID = 803,
                leftNeighborID = 3546,
            };
            ScrollBar = new ClickableTextureComponent(new Rectangle(UpArrow.bounds.X + 12, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            ScrollBarRunner = new Rectangle(ScrollBar.bounds.X, UpArrow.bounds.Y + UpArrow.bounds.Height + 4, ScrollBar.bounds.Width, DownArrow.bounds.Y - UpArrow.bounds.Y - 64);

            for (int i = 0; i < ListElementsPerPage; i++) {
                ListElement.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + i * ((height - 256) / 4), width - 32, (height - 256) / 4 + 4), string.Concat(i)) {
                    myID = i + 3546,
                    rightNeighborID = 97865,
                    fullyImmutable = true
                });
            }

            DescriptionUpArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 24, yPositionOnScreen + height - 200, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f) {
                myID = 801,
                downNeighborID = 802,
                upNeighborID = 3546,
                rightNeighborID = 803
            };
            DescriptionDownArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 24, yPositionOnScreen + height - 42, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f) {
                myID = 802,
                upNeighborID = 801,
                rightNeighborID = 804
            };

            RequirementUpArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 200, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f) {
                myID = 803,
                downNeighborID = 804,
                upNeighborID = 106,
                leftNeighborID = 801
            };
            RequirementDownArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 42, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f) {
                myID = 804,
                upNeighborID = 803,
                leftNeighborID = 802
            };

            createBundleRequirementTextures();
        }

        private void createBundleRequirementTextures()
        {
            RequirementIcons.Clear();

            if (CurrentBundle is null)
                return;

            var unlockable = CurrentBundle.Unlockable;
            RequirementIcons = BundleMenu.createRequirementTextures(xPositionOnScreen + width - 160, yPositionOnScreen + height - 100, unlockable, RequirementPageIndex, RequirementsPerPage);
        }

        private void setNextItem()
        {
            NextItem = null;
            NextId = "";

            var u = CurrentBundle.Unlockable;
            var last = u._price.Pairs.Last();
            foreach (var req in u._price.Pairs) {
                if (u._alreadyPaid.ContainsKey(req.Key) && !(req.Key == last.Key && NextId == ""))
                    continue;

                NextId = Unlockable.getFirstIDFromReqKey(req.Key);
                if (NextId == "money")
                    return;

                NextItem = Unlockable.parseItem(NextId, req.Value);
                return;
            }
        }

        public void applyTab()
        {
            Game1.playSound("shwip");
            Bundles = ShopObject.getAll();
            Bundles.RemoveAll(el => !el.WasDiscovered);
            DescriptionScrollIndex = 0;
            SplitPreviewDescription = new string[] { };

            currentItemIndex = 0;
            setScrollBarToCurrentIndex();
            updateListElementNeighbors();

            if (Bundles.Count == 1)
                selectListElement(Bundles.First());
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

            ListElement.Clear();
            createClickTableTextures();
            setScrollBarToCurrentIndex();
        }

        private void downArrowPressed()
        {
            DownArrow.scale = DownArrow.baseScale;
            currentItemIndex++;
            setScrollBarToCurrentIndex();
            updateListElementNeighbors();
        }

        private void upArrowPressed()
        {
            if (currentItemIndex == 0)
                return;

            UpArrow.scale = UpArrow.baseScale;
            currentItemIndex--;
            setScrollBarToCurrentIndex();
            updateListElementNeighbors();
        }

        private void setScrollBarToCurrentIndex()
        {
            if (Bundles.Count > 0) {
                ScrollBar.bounds.Y = (int)((float)(ScrollBarRunner.Height - ScrollBar.bounds.Height) / Math.Max(1, Bundles.Count - 4) * currentItemIndex + UpArrow.bounds.Bottom + 4);
                if (currentItemIndex == Bundles.Count - 4) {
                    ScrollBar.bounds.Y = DownArrow.bounds.Y - ScrollBar.bounds.Height - 4;
                }
            }
        }

        public void selectListElement(ShopObject bundle)
        {
            Game1.playSound("coin");
            CurrentBundle = bundle;
            DescriptionScrollIndex = 0;
            RequirementPageIndex = 0;
            SplitPreviewDescription = new string[] { };
            createBundleRequirementTextures();
            setNextItem();
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            exitThisMenu();
            base.receiveRightClick(x, y, playSound);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y);

            if (DownArrow.containsPoint(x, y) && isScrollbarDrawn() && currentItemIndex < Math.Max(0, Bundles.Count - 4)) {
                downArrowPressed();
                Game1.playSound("shwip");
            } else if (UpArrow.containsPoint(x, y) && isScrollbarDrawn()) {
                upArrowPressed();
                Game1.playSound("shwip");
            } else if (ScrollBarRunner.Contains(x, y) && isScrollbarDrawn()) {
                Scrolling = true;
            } else if (base.upperRightCloseButton.containsPoint(x, y)) {
                exitThisMenu();
                return;
            } else if (DescriptionUpArrow.containsPoint(x, y) && DescriptionScrollIndex != 0) {
                DescriptionScrollIndex--;
                Game1.playSound("shwip");
            } else if (DescriptionDownArrow.containsPoint(x, y) && hasNextDescriptionRow()) {
                DescriptionScrollIndex++;
                Game1.playSound("shwip");
            } else if(RequirementUpArrow.containsPoint(x, y) && RequirementPageIndex != 0) {
                RequirementPageIndex--;
                Game1.playSound("shwip");
                createBundleRequirementTextures();
            } else if (RequirementDownArrow.containsPoint(x, y) &&  hasNextRequirementPage()) {
                RequirementPageIndex++;
                Game1.playSound("shwip");
                createBundleRequirementTextures();
            }


            for (int i = 0; i < ListElement.Count; i++) {
                if (currentItemIndex + i >= Bundles.Count || !ListElement[i].containsPoint(x, y))
                    continue;

                int index = currentItemIndex + i;

                selectListElement(Bundles[index]);

                updateListElementNeighbors();
                setScrollBarToCurrentIndex();
                return;
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (Scrolling) {
                int y2 = ScrollBar.bounds.Y;
                ScrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - ScrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + UpArrow.bounds.Height + 20));
                float percentage = (float)(y - ScrollBarRunner.Y) / (float)ScrollBarRunner.Height;
                currentItemIndex = Math.Min(Bundles.Count - 4, Math.Max(0, (int)Math.Round((double)((Bundles.Count - 4) * percentage))));
                setScrollBarToCurrentIndex();
                updateListElementNeighbors();
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
            if (!isScrollbarDrawn())
                return;

            if (direction > 0 && currentItemIndex > 0) {
                upArrowPressed();
                Game1.playSound("shiny4");
            } else if (direction < 0 && currentItemIndex < Math.Max(0, Bundles.Count - 4)) {
                downArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (b == Buttons.LeftShoulder && DescriptionScrollIndex != 0) {
                DescriptionScrollIndex--;
                Game1.playSound("shwip");
            } else if (b == Buttons.RightShoulder && hasNextDescriptionRow()) {
                DescriptionScrollIndex++;
                Game1.playSound("shwip");
            } else if (b == Buttons.LeftTrigger && RequirementPageIndex != 0) {
                RequirementPageIndex--;
                Game1.playSound("shwip");
                createBundleRequirementTextures();
            } else if (b == Buttons.RightTrigger && hasNextRequirementPage()) {
                RequirementPageIndex++;
                Game1.playSound("shwip");
                createBundleRequirementTextures();
            }

            base.receiveGamePadButton(b);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.menuButton.Contains(new InputButton(key)))
                exitThisMenu();

            base.receiveKeyPress(key);
        }

        public override void performHoverAction(int x, int y)
        {
            UpArrow.tryHover(x, y, 0.5f);
            DownArrow.tryHover(x, y, 0.5f);
            DescriptionUpArrow.tryHover(x, y, 0.5f);
            DescriptionDownArrow.tryHover(x, y, 0.5f);
            upperRightCloseButton.tryHover(x, y, 0.5f);
            RequirementUpArrow.tryHover(x, y, 0.5f);
            RequirementDownArrow.tryHover(x, y, 0.5f);

            HoveredComponent = RequirementIcons.FirstOrDefault(c => c.bounds.Contains(x, y));
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (!isScrollbarDrawn())
                return;

            switch (direction) {
                case 2: {
                        if (currentItemIndex < Math.Max(0, Bundles.Count - 4)) {
                            downArrowPressed();
                            Game1.playSound("shiny4");

                            currentlySnappedComponent = getComponentWithID(0);
                            snapCursorToCurrentSnappedComponent();
                        }
                        break;
                    }
                case 0:
                    if (currentItemIndex > 0) {
                        upArrowPressed();
                        Game1.playSound("shiny4");

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
            b.Draw(BGTexture, new Rectangle(xPositionOnScreen -30, yPositionOnScreen - 30, width + 140, height + 80), Color.White);

            drawListElements(b);
            drawBundleSummary(b);

            if (isScrollbarDrawn()) {
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarRunner.X, ScrollBarRunner.Y, ScrollBarRunner.Width, ScrollBarRunner.Height, Color.White, 4f);
                ScrollBar.draw(b);
                UpArrow.draw(b);
                DownArrow.draw(b);
            }

            if (HoveredComponent != null && HoveredComponent.item != null)
                drawToolTip(b, HoveredComponent.item.getDescription(), HoveredComponent.item.DisplayName, HoveredComponent.item);
            else if (HoveredComponent != null)
                drawHoverText(b, HoveredComponent.hoverText, Game1.dialogueFont);

            base.draw(b);
            drawMouse(b);
        }

        private void drawListElements(SpriteBatch b)
        {
            for (int k = 0; k < ListElement.Count; k++) {
                if (currentItemIndex + k >= Bundles.Count)
                    continue;

                var le = ListElement[k];

                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), le.bounds.X, le.bounds.Y, le.bounds.Width, le.bounds.Height, (le.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) && !Scrolling) ? Color.Wheat * 0.5f : Color.White * 0.5f, 4f, drawShadow: false);
                ShopObject bundle = Bundles[currentItemIndex + k];
                Unlockable unlockable = bundle.Unlockable;

                //ShopTexture
                bundle.drawInMenu(b, new Vector2(le.bounds.X + 24, le.bounds.Y + 15), 1.3f);

                //display Name
                SpriteText.drawString(b, unlockable.getDisplayName(), le.bounds.X + 82, le.bounds.Y + 28, 999999, -1, 999999, 1f, 0.88f, junimoText: false, -1, "");

                //Location + Coordinates
                var loc = unlockable.getGameLocation();
                var locationDisplay = $"{loc.DisplayName} X{bundle.TileLocation.X} Y{bundle.TileLocation.Y}";
                b.DrawString(Game1.smallFont, locationDisplay, new Vector2(le.bounds.X + le.bounds.Width - 14 - Game1.smallFont.MeasureString(locationDisplay).Length(), le.bounds.Y + 60), Color.Gray);

                //Progress
                var progressDisplay = $"{unlockable._alreadyPaid.Count()}/{(unlockable.ShopType is ShopType.CCBundle or ShopType.AltCCBundle ? unlockable.BundleSlots : unlockable._price.Count())}";
                b.DrawString(Game1.smallFont, progressDisplay, new Vector2(le.bounds.X + le.bounds.Width - 14 - Game1.smallFont.MeasureString(progressDisplay).Length(), le.bounds.Y + 20), Color.Gray);

                if (bundle == CurrentBundle)
                    //Red selection rectangle
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), le.bounds.X + 4, le.bounds.Y + 4, le.bounds.Width - 8, le.bounds.Height - 8, Color.White, 4f, drawShadow: false);

            }

        }

        private void drawBundleSummary(SpriteBatch b)
        {
            if(Bundles.Count == 0) {
                string emptyMenuDescription = Helper.Translation.Get("ub_empty_overview");
                emptyMenuDescription = Game1.parseText(emptyMenuDescription, Game1.dialogueFont, width - 72);
                b.DrawString(Game1.dialogueFont, emptyMenuDescription, new Vector2(xPositionOnScreen + 42, yPositionOnScreen + height - 200), Game1.textColor);
            }

            if (CurrentBundle is null)
                return;

            var u = CurrentBundle.Unlockable;

            //Title Scroll
            string title = addSpacesForTitleIcon() + u.getDisplayName();
            var xPosTitle = xPositionOnScreen + ((width + 60) / 2) - (SpriteText.getWidthOfString(title) / 2);
            SpriteText.drawStringWithScrollBackground(b, title, xPosTitle, yPositionOnScreen + height - 250);
            CurrentBundle.drawInMenu(b, new Vector2(xPosTitle + 12, yPositionOnScreen + height - 247), 0.85f);

            drawRequirementIcons(b);

            //Location + Coordinates
            var loc = u.getGameLocation();
            var locationDisplay = $"{loc.DisplayName} X{CurrentBundle.TileLocation.X} Y{CurrentBundle.TileLocation.Y}";
            b.DrawString(Game1.dialogueFont, locationDisplay, new Vector2(xPositionOnScreen + 42, yPositionOnScreen + height - 186), Color.DarkSlateGray);

            //Progress
            var progressDisplay = $"{u._alreadyPaid.Count()}/{(u.ShopType is ShopType.CCBundle or ShopType.AltCCBundle ? u.BundleSlots : u._price.Count())}";
            var progressPos = Game1.dialogueFont.MeasureString(locationDisplay).X < 699 - Game1.dialogueFont.MeasureString(progressDisplay).Length()
                    ? new Vector2(xPositionOnScreen + 700 - Game1.smallFont.MeasureString(progressDisplay).Length(), yPositionOnScreen + height - 186)
                    : new Vector2(xPositionOnScreen + 42, yPositionOnScreen + height - 220);
            b.DrawString(Game1.dialogueFont, progressDisplay, progressPos, Color.DarkSlateGray);

            var moneyString = u.ShopType == ShopType.ParrotPerch ? Helper.Translation.Get("ub_parrot_money") : Helper.Translation.Get("ub_speech_money");
            var description = u.getTranslatedOverviewDescription();
            description = description.Replace("{{item}}", NextId == "money" ? moneyString : NextItem.DisplayName);

            if(SplitPreviewDescription.Length == 0 && description != "")
                SplitPreviewDescription = Game1.parseText(description, Game1.dialogueFont, 670).Split(Environment.NewLine);

            description = "";
            for (int i = DescriptionScrollIndex; i < DescriptionScrollIndex + DescriptionRowsPerPage; i++) {
                if (SplitPreviewDescription.Length > i)
                    description += SplitPreviewDescription[i] + Environment.NewLine;
            }
            b.DrawString(Game1.dialogueFont, description, new Vector2(xPositionOnScreen + 42, yPositionOnScreen + height - 145), Game1.textColor);

            if (DescriptionScrollIndex != 0)
                DescriptionUpArrow.draw(b);

            if (hasNextDescriptionRow())
                DescriptionDownArrow.draw(b);

            if (RequirementPageIndex != 0)
                RequirementUpArrow.draw(b);

            if (hasNextRequirementPage())
                RequirementDownArrow.draw(b);
        }

        private string addSpacesForTitleIcon()
        { //I am using spaces to create space for the bundle shoptexture in the scroll, but spaces have different width depending on language
            var s = " ";
            while (SpriteText.getWidthOfString(s) < 52)
                s += " ";

            return s;
        }

        public void drawRequirementIcons(SpriteBatch b)
        {
            var unlockable = CurrentBundle.Unlockable;

            for (int i = 0; i < RequirementIcons.Count; i++) {
                float alpha_mult = 1f;

                //if (CurrentPartialRequirementIndex >= 0 && CurrentPartialRequirementIndex != i)
                //    alpha_mult = 0.25f;

                ClickableRequirementTexture c = RequirementIcons[i];
                bool completed = unlockable._alreadyPaid.ContainsKey(c.ReqKey);

                if (!completed)
                    b.Draw(Game1.shadowTexture, new Vector2(c.bounds.Center.X - Game1.shadowTexture.Bounds.Width * 4 / 2 - 4, c.bounds.Center.Y + 4), Game1.shadowTexture.Bounds, Color.White * alpha_mult, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);

                if (c.ReqItemId == "money") {
                    c.draw(b, Color.White * (completed ? 0.25f : alpha_mult), 0.89f);
                    UtilityMisc.drawKiloFormat(b, c.ReqValue, c.bounds.X, c.bounds.Y, Color.White * (completed ? 0.25f : alpha_mult));

                } else if (c.item != null && c.visible)
                    c.item.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale / 4f, 1f, 0.9f, StackDrawType.Draw, Color.White * (completed ? 0.25f : alpha_mult), drawShadow: false);
            }
        }

        public bool isScrollbarDrawn() => Bundles.Count > ListElementsPerPage;
        public bool hasNextDescriptionRow() => SplitPreviewDescription.Length > DescriptionScrollIndex + DescriptionRowsPerPage;
        public bool hasNextRequirementPage() => CurrentBundle?.Unlockable._price.Pairs.Count() > ((RequirementPageIndex + 1) * RequirementsPerPage);
    }
}

