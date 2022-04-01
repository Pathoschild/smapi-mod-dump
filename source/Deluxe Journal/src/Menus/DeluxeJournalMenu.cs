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
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using DeluxeJournal.Framework;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>The active menu the replaces the vanilla QuestLog. Displays journal pages and tabs.</summary>
    /// <remarks>Custom pages should be registered using the API.</remarks>
    public class DeluxeJournalMenu : IClickableMenu
    {
        private const int ActiveTabOffset = 8;

        public static int ActiveTab { get; private set; }

        private readonly List<ClickableTextureComponent> _tabs;
        private readonly List<IPage> _pages;
        private string _hoverText;

        public IReadOnlyList<ClickableComponent> Tabs => _tabs;

        public IReadOnlyList<IPage> Pages => _pages;

        private string HoverText
        {
            get
            {
                string pageHoverText = _pages[ActiveTab].HoverText;
                return pageHoverText.Length > 0 ? pageHoverText : _hoverText;
            }

            set
            {
                _hoverText = value;
            }
        }

        internal DeluxeJournalMenu(PageManager pageManager) : base(0, 0, 832, 576, showUpperRightCloseButton: true)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ||
                LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                height += 64;
            }

            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height, 0, 32);
            xPositionOnScreen = (int)topLeft.X;
            yPositionOnScreen = (int)topLeft.Y;
            upperRightCloseButton.bounds = new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen - 8, 48, 48);

            _pages = pageManager.GetPages(new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height));
            _tabs = new List<ClickableTextureComponent>();
            _hoverText = "";

            foreach (IPage page in _pages)
            {
                _tabs.Add(page.GetTabComponent());
            }

            ChainNeighborsUpDown(_tabs);

            if (ActiveTab == 0 && Game1.player.visibleQuestCount == 0)
            {
                ActiveTab = 1;
            }
            else if (Game1.player.hasPendingCompletedQuests)
            {
                ActiveTab = 0;
            }
            
            _tabs[ActiveTab].bounds.X += ActiveTabOffset;
            _pages[ActiveTab].populateClickableComponentList();
            _pages[ActiveTab].OnVisible();
            AddTabsToClickableComponents(_pages[ActiveTab]);

            if (Game1.options.SnappyMenus)
            {
                snapToDefaultClickableComponent();
            }

            Game1.playSound("bigSelect");

            exitFunction = () => _pages[ActiveTab].OnHidden();
        }

        public IPage GetActivePage()
        {
            return _pages[ActiveTab];
        }

        public void ChangeTab(int tab, bool playSound = true)
        {
            if (tab == ActiveTab || tab < 0 || tab >= _tabs.Count)
            {
                return;
            }

            if (playSound)
            {
                Game1.playSound("smallSelect");
            }

            _tabs[ActiveTab].bounds.X -= ActiveTabOffset;
            _pages[ActiveTab].OnHidden();
            ActiveTab = tab;

            _tabs[ActiveTab].bounds.X += ActiveTabOffset;
            _pages[ActiveTab].populateClickableComponentList();
            _pages[ActiveTab].OnVisible();
            AddTabsToClickableComponents(_pages[ActiveTab]);

            if (Game1.options.SnappyMenus)
            {
                if (_pages[ActiveTab] is PageBase page)
                {
                    page.SnapToActiveTabComponent();
                }
                else
                {
                    _pages[ActiveTab].snapToDefaultClickableComponent();
                }
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
        }

        public void AddTabsToClickableComponents(IPage page)
        {
            page.AllClickableComponents.AddRange(_tabs);
        }

        public override ClickableComponent getCurrentlySnappedComponent()
        {
            return _pages[ActiveTab].getCurrentlySnappedComponent();
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            _pages[ActiveTab].setCurrentlySnappedComponentTo(id);
        }

        public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
        {
            _pages[ActiveTab].automaticSnapBehavior(direction, oldRegion, oldID);
        }

        public override void snapToDefaultClickableComponent()
        {
            _pages[ActiveTab].snapToDefaultClickableComponent();
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            _pages[ActiveTab].snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            _pages[ActiveTab].receiveGamePadButton(b);
        }

        public override void setUpForGamePadMode()
        {
            _pages[ActiveTab].setUpForGamePadMode();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!_pages[ActiveTab].ChildHasFocus())
            {
                base.receiveLeftClick(x, y, playSound);

                for (int i = 0; i < _tabs.Count; ++i)
                {
                    if (_tabs[i].containsPoint(x, y))
                    {
                        ChangeTab(i);
                        return;
                    }
                }
            }

            _pages[ActiveTab].receiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            _pages[ActiveTab].leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            _pages[ActiveTab].releaseLeftClick(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            _pages[ActiveTab].receiveRightClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            _pages[ActiveTab].receiveScrollWheelAction(direction);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!_pages[ActiveTab].ChildHasFocus() && !_pages[ActiveTab].KeyboardHasFocus())
            {
                if ((Game1.options.doesInputListContain(Game1.options.menuButton, key) ||
                    Game1.options.doesInputListContain(Game1.options.journalButton, key)) &&
                    readyToClose())
                {
                    exitThisMenu();
                }
            }

            _pages[ActiveTab].receiveKeyPress(key);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _hoverText = "";

            _pages[ActiveTab].performHoverAction(x, y);

            if (!_pages[ActiveTab].ChildHasFocus())
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
            return _pages[ActiveTab].readyToClose();
        }

        public override bool shouldDrawCloseButton()
        {
            return _pages[ActiveTab].shouldDrawCloseButton();
        }

        public override void update(GameTime time)
        {
            _pages[ActiveTab].update(time);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, _tabs[ActiveTab].hoverText, xPositionOnScreen + width / 2, yPositionOnScreen - 64);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f);

            foreach (ClickableTextureComponent tab in _tabs)
            {
                tab.draw(b);
            }
            
            Game1.mouseCursorTransparency = 1f;

            _pages[ActiveTab].draw(b);
            base.draw(b);
            drawMouse(b);

            if (HoverText.Length > 0)
            {
                drawHoverText(b, HoverText, Game1.dialogueFont);
            }
        }
    }
}
