/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewDruid.Map;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewDruid.Journal
{
    internal class Druid : IClickableMenu
    {
        public Texture2D iconTexture;
        public Texture2D targetTexture;
        public Dictionary<string, Rectangle> iconFrames;
        public const int region_forwardButton = 101;
        public const int region_backButton = 102;
        public List<List<Page>> pages;
        public List<ClickableComponent> questLogButtons;
        private int currentPage;
        private int questPage = -1;
        public ClickableTextureComponent forwardButton;
        public ClickableTextureComponent endButton;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent startButton;
        protected Page _shownPage;
        protected List<string> _objectiveText;
        protected List<string> _transcriptText;
        protected float _contentHeight;
        protected float _scissorRectHeight;
        public float scrollAmount;
        public ClickableTextureComponent upArrow;
        public ClickableTextureComponent downArrow;
        public ClickableTextureComponent scrollBar;
        private bool scrolling;
        public Rectangle scrollBarBounds;
        private string hoverText = "";

        public Druid()
          : base(0, 0, 0, 0, true)
        {
            iconTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Icons.png"));
            targetTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Target.png"));
            iconFrames = new Dictionary<string, Rectangle>()
            {
                ["Effigy"] = new Rectangle(0, 0, 8, 8),
                ["Weald"] = new Rectangle(8, 0, 8, 8),
                ["Mists"] = new Rectangle(16, 0, 8, 8),
                ["Stars"] = new Rectangle(24, 0, 8, 8),
                ["Jester"] = new Rectangle(0, 8, 8, 8),
                ["Fates"] = new Rectangle(8, 8, 8, 8),
                ["Ether"] = new Rectangle(16, 8, 8, 8),
                ["Shadow"] = new Rectangle(24, 8, 8, 8),
                ["Chaos"] = new Rectangle(0, 16, 8, 8)
            };
            Game1.playSound("bigSelect");
            setupPages();
            width = 832;
            height = 576;
            /*if (LocalizedContentManager.CurrentLanguageCode == 9 || LocalizedContentManager.CurrentLanguageCode == 8)
                height += 64;*/
            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height, 0, 0);
            xPositionOnScreen = (int)centeringOnScreen.X;
            yPositionOnScreen = (int)centeringOnScreen.Y + 32;
            questLogButtons = new List<ClickableComponent>();
            for (int index = 0; index < 6; ++index)
                questLogButtons.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + index * ((height - 32) / 6), width - 32, (height - 32) / 6 + 4), index.ToString() ?? "")
                {
                    myID = index,
                    downNeighborID = -7777,
                    upNeighborID = index > 0 ? index - 1 : -1,
                    rightNeighborID = -7777,
                    leftNeighborID = -7777,
                    fullyImmutable = true
                });
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f, false);
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 64, yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false);
            textureComponent1.myID = 102;
            textureComponent1.rightNeighborID = -7777;
            backButton = textureComponent1;
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 64 - 48, yPositionOnScreen + height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false);
            textureComponent2.myID = 101;
            forwardButton = textureComponent2;
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 64, yPositionOnScreen + 72, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false);
            textureComponent3.myID = 103;
            startButton = textureComponent3;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 100, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false);
            textureComponent4.myID = 104;
            endButton = textureComponent4;
            int num = xPositionOnScreen + width + 16;
            upArrow = new ClickableTextureComponent(new Rectangle(num, yPositionOnScreen + 96, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
            downArrow = new ClickableTextureComponent(new Rectangle(num, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
            scrollBarBounds = new Rectangle();
            scrollBarBounds.X = upArrow.bounds.X + 12;
            scrollBarBounds.Width = 24;
            scrollBarBounds.Y = upArrow.bounds.Y + upArrow.bounds.Height + 4;
            scrollBarBounds.Height = downArrow.bounds.Y - 4 - scrollBarBounds.Y;
            scrollBar = new ClickableTextureComponent(new Rectangle(scrollBarBounds.X, scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
            if (!Game1.options.SnappyMenus)
                return;
            populateClickableComponentList();
            base.snapToDefaultClickableComponent();
        }

        public void setupPages()
        {

            pages = JournalData.RetrievePages();

        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (oldID >= 0 && oldID < 6 && questPage == -1)
            {
                switch (direction)
                {
                    case 1:
                        if (currentPage < pages.Count - 1)
                        {
                            currentlySnappedComponent = getComponentWithID(101);
                            currentlySnappedComponent.leftNeighborID = oldID;
                            break;
                        }
                        break;
                    case 2:
                        if (oldID < 5 && pages[currentPage].Count - 1 > oldID)
                        {
                            currentlySnappedComponent = getComponentWithID(oldID + 1);
                            break;
                        }
                        break;
                    case 3:
                        if (currentPage > 0)
                        {
                            currentlySnappedComponent = getComponentWithID(102);
                            currentlySnappedComponent.rightNeighborID = oldID;
                            break;
                        }
                        break;
                }
            }
            else if (oldID == 102)
            {
                if (questPage != -1)
                    return;
                currentlySnappedComponent = getComponentWithID(0);
            }
            snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (b == Buttons.RightTrigger && questPage == -1 && currentPage < pages.Count - 1)
            {
                nonQuestPageForwardButton();
            }
            else if (b == Buttons.LeftTrigger && questPage == -1)
            {

                if (currentPage > 0)
                {
                    nonQuestPageBackButton();

                }
                else
                {
                    pageEndButton();
                }

            }

        }

        public bool NeedsScroll()
        {
            return questPage != -1 && _contentHeight > (double)_scissorRectHeight;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (NeedsScroll())
            {
                float num = scrollAmount - Math.Sign(direction) * 64 / 2;
                if ((double)num < 0.0)
                    num = 0.0f;
                if ((double)num > _contentHeight - (double)_scissorRectHeight)
                    num = _contentHeight - _scissorRectHeight;
                if (scrollAmount != (double)num)
                {
                    scrollAmount = num;
                    Game1.playSound("shiny4");
                    SetScrollBarFromAmount();
                }
            }
            base.receiveScrollWheelAction(direction);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            hoverText = "";
            base.performHoverAction(x, y);
            forwardButton.tryHover(x, y, 0.2f);
            backButton.tryHover(x, y, 0.2f);
            endButton.tryHover(x, y, 0.2f);
            startButton.tryHover(x, y, 0.2f);
            if (!NeedsScroll())
                return;
            upArrow.tryHover(x, y, 0.1f);
            downArrow.tryHover(x, y, 0.1f);
            scrollBar.tryHover(x, y, 0.1f);
            int num = scrolling ? 1 : 0;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.isAnyGamePadButtonBeingPressed() && questPage != -1 && Game1.options.doesInputListContain(Game1.options.menuButton, key))
                exitQuestPage();
            else
                base.receiveKeyPress(key);
            if (Game1.options.doesInputListContain(Game1.options.journalButton, key) && readyToClose())
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
            if (!Mod.instance.RiteButtonPressed())
                return;
            Game1.exitActiveMenu();
        }

        private void nonQuestPageForwardButton()
        {
            ++currentPage;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != pages.Count - 1)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        private void nonQuestPageBackButton()
        {
            --currentPage;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != 0)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        private void pageEndButton()
        {
            currentPage = pages.Count - 1;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != 0)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        private void pageStartButton()
        {
            currentPage = 0;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != 0)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.leftClickHeld(x, y);
            if (scrolling)
                SetScrollFromY(y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
            scrolling = false;
        }

        public void SetScrollFromY(int y)
        {
            int y1 = scrollBar.bounds.Y;
            scrollAmount = Utility.Clamp((y - scrollBarBounds.Y) / (float)(scrollBarBounds.Height - scrollBar.bounds.Height), 0.0f, 1f) * (_contentHeight - _scissorRectHeight);
            SetScrollBarFromAmount();
            if (y1 == scrollBar.bounds.Y)
                return;
            Game1.playSound("shiny4");
        }

        public void UpArrowPressed()
        {
            upArrow.scale = upArrow.baseScale;
            scrollAmount -= 64f;
            if (scrollAmount < 0.0)
                scrollAmount = 0.0f;
            SetScrollBarFromAmount();
        }

        public void DownArrowPressed()
        {
            downArrow.scale = downArrow.baseScale;
            scrollAmount += 64f;
            if (scrollAmount > _contentHeight - (double)_scissorRectHeight)
                scrollAmount = _contentHeight - _scissorRectHeight;
            SetScrollBarFromAmount();
        }

        private void SetScrollBarFromAmount()
        {
            if (!NeedsScroll())
            {
                scrollAmount = 0.0f;
            }
            else
            {
                if (scrollAmount < 8.0)
                    scrollAmount = 0.0f;
                if (scrollAmount > _contentHeight - (double)_scissorRectHeight - 8.0)
                    scrollAmount = _contentHeight - _scissorRectHeight;
                scrollBar.bounds.Y = (int)(scrollBarBounds.Y + (scrollBarBounds.Height - scrollBar.bounds.Height) / (double)Math.Max(1f, _contentHeight - _scissorRectHeight) * scrollAmount);
            }
        }

        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);
            if (!NeedsScroll())
                return;
            switch (direction)
            {
                case 0:
                    UpArrowPressed();
                    break;
                case 2:
                    DownArrowPressed();
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (Game1.activeClickableMenu == null)
                return;
            if (questPage == -1)
            {
                for (int index = 0; index < questLogButtons.Count; ++index)
                {
                    if (pages.Count > 0 && pages[currentPage].Count > index && questLogButtons[index].containsPoint(x, y))
                    {
                        Game1.playSound("smallSelect");
                        questPage = index;
                        _shownPage = pages[currentPage][index];
                        _objectiveText = _shownPage.objectives;
                        _transcriptText = _shownPage.transcript;
                        scrollAmount = 0.0f;
                        SetScrollBarFromAmount();
                        if (!Game1.options.SnappyMenus)
                            return;
                        currentlySnappedComponent = getComponentWithID(102);
                        currentlySnappedComponent.rightNeighborID = -7777;
                        currentlySnappedComponent.downNeighborID = 104;
                        snapCursorToCurrentSnappedComponent();
                        return;
                    }
                }
                if (currentPage == 0 && backButton.containsPoint(x, y))
                    exitThisMenu(true);
                else if (currentPage < pages.Count - 1 && forwardButton.containsPoint(x, y))
                    nonQuestPageForwardButton();
                else if (currentPage > 0 && backButton.containsPoint(x, y))
                    nonQuestPageBackButton();
                else if (currentPage > 0 && startButton.containsPoint(x, y))
                    pageStartButton();
                else if (currentPage < pages.Count - 1 && endButton.containsPoint(x, y))
                    pageEndButton();
                else
                    exitThisMenu(true);
            }
            else
            {
                if (!NeedsScroll() || backButton.containsPoint(x, y))
                    exitQuestPage();
                if (!NeedsScroll())
                    return;
                if (downArrow.containsPoint(x, y) && scrollAmount < _contentHeight - (double)_scissorRectHeight)
                {
                    DownArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (upArrow.containsPoint(x, y) && scrollAmount > 0.0)
                {
                    UpArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (scrollBar.containsPoint(x, y))
                    scrolling = true;
                else if (scrollBarBounds.Contains(x, y))
                    scrolling = true;
                else if (!downArrow.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height)
                {
                    scrolling = true;
                    base.leftClickHeld(x, y);
                    base.releaseLeftClick(x, y);
                }
            }
        }

        public void exitQuestPage()
        {
            questPage = -1;
            setupPages();
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus)
                return;
            base.snapToDefaultClickableComponent();
        }

        public override void update(GameTime time) => base.update(time);

        public override void draw(SpriteBatch b)
        {
            SpriteBatch spriteBatch1 = b;
            Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
            Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
            Rectangle bounds = viewport.Bounds;
            Color color = Color.Black * 0.75f;
            spriteBatch1.Draw(fadeToBlackRect, bounds, color);
            SpriteText.drawStringWithScrollCenteredAt(b, "Stardew Druid", xPositionOnScreen + width / 2, yPositionOnScreen - 64, 16, 1f, null, 0, 0.88f, false);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f, true, -1f);
            if (questPage == -1)
            {
                for (int index = 0; index < questLogButtons.Count; ++index)
                {
                    if (pages.Count<List<Page>>() > 0 && pages[currentPage].Count<Page>() > index)
                    {
                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), questLogButtons[index].bounds.X, questLogButtons[index].bounds.Y, questLogButtons[index].bounds.Width, questLogButtons[index].bounds.Height, questLogButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, false, -1f);
                        if (pages[currentPage][index].active)
                            SpriteText.drawString(b, "*", questLogButtons[index].bounds.Right - 40, questLogButtons[index].bounds.Y + 40, 999999, -1, 999999, 1f, 0.88f, false, -1, "", null, 0);
                        SpriteText.drawString(b, pages[currentPage][index].title, questLogButtons[index].bounds.X + 100, questLogButtons[index].bounds.Y + 24, 999999, -1, 999999, 1f, 0.88f, false, -1, "", null, 0);
                        Utility.drawWithShadow(b, iconTexture, new Vector2(questLogButtons[index].bounds.X + 32, questLogButtons[index].bounds.Y + 28), iconFrames[pages[currentPage][index].icon], Color.White, 0.0f, Vector2.Zero, 5f, false, 0.99f, -1, -1, 0.35f);
                    }
                }
            }
            else
            {
                SpriteText.drawStringHorizontallyCenteredAt(b, _shownPage.title, xPositionOnScreen + width / 2, yPositionOnScreen + 32, 999999, -1, 999999, 1f, 0.88f, false, null, 99999);
                string text1 = Game1.parseText(_shownPage.description, Game1.dialogueFont, width - 128);
                Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
                Vector2 vector2 = Game1.dialogueFont.MeasureString(text1);
                Rectangle rectangle = new Rectangle()
                {
                    X = xPositionOnScreen + 32,
                    Y = yPositionOnScreen + 96
                };
                rectangle.Height = yPositionOnScreen + height - 32 - rectangle.Y;
                rectangle.Width = width - 64;
                _scissorRectHeight = rectangle.Height;
                Rectangle screen = Utility.ConstrainScissorRectToScreen(rectangle);
                b.End();
                SpriteBatch spriteBatch2 = b;
                BlendState alphaBlend = BlendState.AlphaBlend;
                SamplerState pointClamp = SamplerState.PointClamp;
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.ScissorTestEnable = true;
                Matrix? nullable = new Matrix?();
                spriteBatch2.Begin(0, alphaBlend, pointClamp, null, rasterizerState, null, nullable);
                Game1.graphics.GraphicsDevice.ScissorRectangle = screen;
                Utility.drawTextWithShadow(b, text1, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, (float)(yPositionOnScreen - (double)scrollAmount + 96.0)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                float num1 = (float)(yPositionOnScreen + 96 + (double)vector2.Y + 32.0) - scrollAmount;
                for (int index = 0; index < _objectiveText.Count; ++index)
                {
                    string str = _objectiveText[index];
                    int num2 = width - 128;
                    SpriteFont dialogueFont = Game1.dialogueFont;
                    int num3 = num2;
                    string text2 = Game1.parseText(str, dialogueFont, num3);
                    Color darkBlue = Color.DarkBlue;
                    Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, num1 - 8f), darkBlue, 1f, -1f, -1, -1, 1f, 3);
                    num1 += Game1.dialogueFont.MeasureString(text2).Y;
                    _contentHeight = num1 + scrollAmount - screen.Y;
                }

                if (_transcriptText.Count > 0)
                {

                    num1 += 16;
                    for (int index = 0; index < _transcriptText.Count; ++index)
                    {
                        string str = _transcriptText[index];
                        int num2 = width - 128;
                        SpriteFont dialogueFont = Game1.dialogueFont;
                        int num3 = num2;
                        string text2 = Game1.parseText(str, dialogueFont, num3);
                        Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, num1 - 8f), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                        num1 += Game1.dialogueFont.MeasureString(text2).Y;
                        _contentHeight = num1 + scrollAmount - screen.Y;
                    }
                }

                b.End();
                b.GraphicsDevice.ScissorRectangle = scissorRectangle;
                b.Begin(0, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, new Matrix?());
                if (NeedsScroll())
                {
                    if (scrollAmount > 0.0)
                        b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Top, screen.Width, 4), Color.Black * 0.15f);
                    if (scrollAmount < _contentHeight - (double)_scissorRectHeight)
                        b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Bottom - 4, screen.Width, 4), Color.Black * 0.15f);
                }
            }
            if (NeedsScroll())
            {
                upArrow.draw(b);
                downArrow.draw(b);
                scrollBar.draw(b);
            }
            if (currentPage < pages.Count - 1 && questPage == -1)
            {
                forwardButton.draw(b);
                b.Draw(targetTexture, new Vector2(endButton.bounds.X - 12, endButton.bounds.Y + 48), new Rectangle?(new Rectangle(0, 0, 64, 64)), endButton.scale > 4.0 ? new Color(0.0f, 1f, 0.0f, 1f) : new Color(0.5f, 1f, 0.5f, 1f), -1.57079637f, Vector2.Zero, 2f, 0, 999f);
            }
            if (currentPage > 0 && questPage == -1)
            {
                b.Draw(targetTexture, new Vector2(startButton.bounds.X + 60, startButton.bounds.Y - 8), new Rectangle?(new Rectangle(0, 0, 64, 64)), startButton.scale > 4.0 ? new Color(0.0f, 1f, 0.0f, 1f) : new Color(0.5f, 1f, 0.5f, 1f), 1.57079637f, Vector2.Zero, 2f, 0, 999f);
            }
            backButton.draw(b);
            base.draw(b);
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b, false, -1);
            if (hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, null, -1, -1, -1, 1f, null, null);
        }
    }
}
