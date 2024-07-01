/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using DeluxeJournal.Framework;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>Replacement QuestLog that displays journal pages and tabs.</summary>
    /// <remarks>Custom pages should be registered using the DeluxeJournal API.</remarks>
    public class DeluxeJournalMenu : IClickableMenu
    {
        public const int ActiveTabOffset = 8;

        private static readonly PerScreen<int> ActiveTabPerScreen = new PerScreen<int>();

        /// <summary>The index of the currently active tab.</summary>
        public static int ActiveTab
        {
            get
            {
                return ActiveTabPerScreen.Value;
            }

            private set
            {
                ActiveTabPerScreen.Value = value;
            }
        }

        private readonly List<ClickableTextureComponent> _tabs;
        private readonly List<IPage> _pages;
        private string _hoverText;

        /// <summary>Read-only list of all tabs.</summary>
        public IReadOnlyList<ClickableTextureComponent> Tabs => _tabs;

        /// <summary>Read-only list of all pages.</summary>
        public IReadOnlyList<IPage> Pages => _pages;

        /// <summary>Get the currently displayed page.</summary>
        public IPage ActivePage => _pages[ActiveTab];

        private string HoverText
        {
            get
            {
                string pageHoverText = ActivePage.HoverText;
                return pageHoverText.Length > 0 ? pageHoverText : _hoverText;
            }

            set
            {
                _hoverText = value;
            }
        }

        internal DeluxeJournalMenu() : base(0, 0, 832, 576, showUpperRightCloseButton: true)
        {
            Vector2 topLeft;

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ||
                LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height + 64, 0, 32);
            }
            else
            {
                topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height, 0, 32);
            }

            xPositionOnScreen = (int)topLeft.X;
            yPositionOnScreen = (int)topLeft.Y;

            _pages = PageRegistry.CreatePages(new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height));
            _tabs = [];
            _hoverText = string.Empty;

            upperRightCloseButton.bounds = new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen - 8, 48, 48);

            foreach (IPage page in _pages)
            {
                _tabs.Add(page.GetTabComponent());
            }

            ChainNeighborsUpDown(_tabs);

            if (ActiveTab == 0 && !Game1.player.hasVisibleQuests)
            {
                ActiveTab = 1;
            }
            else if (Game1.player.hasPendingCompletedQuests)
            {
                ActiveTab = 0;
            }
            
            _tabs[ActiveTab].bounds.X += ActiveTabOffset;
            PopulatePageClickableComponents(ActivePage);
            ActivePage.OnVisible();

            if (Game1.options.SnappyMenus)
            {
                snapToDefaultClickableComponent();
            }

            exitFunction = ActivePage.OnHidden;
            Game1.playSound("bigSelect");
        }

        public void ChangeTab(int tab, bool playSound = true)
        {
            if (!readyToClose() || tab == ActiveTab || tab < 0 || tab >= _tabs.Count)
            {
                return;
            }

            if (playSound)
            {
                Game1.playSound("smallSelect");
            }

            ActivePage.OnHidden();
            _tabs[ActiveTab].bounds.X -= ActiveTabOffset;
            ActiveTab = tab;

            _tabs[ActiveTab].bounds.X += ActiveTabOffset;
            PopulatePageClickableComponents(ActivePage);
            ActivePage.OnVisible();

            if (Game1.options.SnappyMenus)
            {
                if (ActivePage is PageBase page)
                {
                    page.SnapToActiveTabComponent();
                }
                else
                {
                    ActivePage.snapToDefaultClickableComponent();
                }
            }
        }

        public void PopulatePageClickableComponents(IPage page)
        {
            page.populateClickableComponentList();
            page.allClickableComponents.AddRange(_tabs);
        }

        public void SetQuestLog(QuestLog questLog)
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                if (_pages[i] is QuestLogPage questLogPage)
                {
                    questLogPage.QuestLog = questLog;

                    if (ActiveTab == i)
                    {
                        questLogPage.PopulateQuestLogClickableComponentsList();
                    }

                    return;
                }
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            GetActiveMenu().gameWindowSizeChanged(oldBounds, newBounds);
        }

        public override ClickableComponent getCurrentlySnappedComponent()
        {
            return GetActiveMenu().getCurrentlySnappedComponent();
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            GetActiveMenu().setCurrentlySnappedComponentTo(id);
        }

        public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
        {
            GetActiveMenu().automaticSnapBehavior(direction, oldRegion, oldID);
        }

        public override void snapToDefaultClickableComponent()
        {
            // Protected against the base QuestLog constructor calling before the pages are created
            if (_pages != null)
            {
                GetActiveMenu().snapToDefaultClickableComponent();
            }
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            GetActiveMenu().snapCursorToCurrentSnappedComponent();
        }

        public override void applyMovementKey(int direction)
        {
            GetActiveMenu().applyMovementKey(direction);
        }

        public override void receiveGamePadButton(Buttons b)
        {
            GetActiveMenu().receiveGamePadButton(b);
        }

        public override void gamePadButtonHeld(Buttons b)
        {
            GetActiveMenu().gamePadButtonHeld(b);
        }

        public override void setUpForGamePadMode()
        {
            GetActiveMenu().setUpForGamePadMode();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (ElementsEnabled())
            {
                if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y) && readyToClose())
                {
                    exitThisMenu(playSound);
                }

                for (int i = 0; i < _tabs.Count; ++i)
                {
                    if (_tabs[i].containsPoint(x, y))
                    {
                        ChangeTab(i);
                        return;
                    }
                }
            }

            GetActiveMenu().receiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            GetActiveMenu().leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            GetActiveMenu().releaseLeftClick(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            GetActiveMenu().receiveRightClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            GetActiveMenu().receiveScrollWheelAction(direction);
        }

        public override void receiveKeyPress(Keys key)
        {
            GetActiveMenu().receiveKeyPress(key);

            if (ElementsEnabled() && !ActivePage.KeyboardHasFocus())
            {
                if (Game1.options.doesInputListContain(Game1.options.journalButton, key) && readyToClose())
                {
                    exitThisMenu();
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _hoverText = string.Empty;

            GetActiveMenu().performHoverAction(x, y);

            if (ElementsEnabled())
            {
                foreach (ClickableTextureComponent tab in _tabs)
                {
                    if (tab.containsPoint(x, y))
                    {
                        _hoverText = tab.hoverText;
                        return;
                    }
                }
            }
        }

        public override bool readyToClose()
        {
            return GetActiveMenu().readyToClose();
        }

        public override bool shouldDrawCloseButton()
        {
            return ElementsEnabled();
        }

        public override void update(GameTime time)
        {
            GetActiveMenu().update(time);
        }

        public override void draw(SpriteBatch b)
        {
            Game1.mouseCursorTransparency = 1f;

            if (ActivePage is QuestLogPage questLogPage && questLogPage.QuestLog != null)
            {
                questLogPage.draw(b);

                foreach (ClickableTextureComponent tab in _tabs)
                {
                    tab.draw(b);
                }
            }
            else
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f);
                SpriteText.drawStringWithScrollCenteredAt(b, _tabs[ActiveTab].hoverText, xPositionOnScreen + width / 2, yPositionOnScreen - 64);

                foreach (ClickableTextureComponent tab in _tabs)
                {
                    tab.draw(b);
                }

                for (IClickableMenu menu = ActivePage; menu != null; menu = menu.GetChildMenu())
                {
                    menu.draw(b);
                }
            }

            if (upperRightCloseButton != null && shouldDrawCloseButton())
            {
                upperRightCloseButton.draw(b);
            }

            drawMouse(b, false, Game1.mouseCursor == Game1.cursor_default && Game1.options.SnappyMenus ? Game1.cursor_gamepad_pointer : Game1.mouseCursor);

            if (HoverText.Length > 0 && ElementsEnabled())
            {
                drawHoverText(b, HoverText, Game1.dialogueFont);
            }
        }

        protected override void cleanupBeforeExit()
        {
            IClickableMenu menu = GetActiveMenu();

            while (menu.GetParentMenu() is IClickableMenu parent)
            {
                menu.exitThisMenuNoSound();
                menu = parent;
            }

            foreach (IPage page in Pages)
            {
                if (page != menu)
                {
                    page.exitThisMenuNoSound();
                }
            }

            menu.exitThisMenuNoSound();
        }

        private bool ElementsEnabled()
        {
            return !ActivePage.ParentElementsDisabled && ActivePage.GetChildMenu() == null;
        }

        private IClickableMenu GetActiveMenu()
        {
            IClickableMenu menu = ActivePage;

            while (menu.GetChildMenu() is IClickableMenu child)
            {
                menu = child;
            }

            return menu;
        }
    }
}
