/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MysticalBuildings
**
*************************************************/

using CaveOfMemories.Framework.GameLocations;
using CaveOfMemories.Framework.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveOfMemories.Framework.UI
{
    public class EventSelectionMenu : IClickableMenu
    {
        private int _currentPage;
        private string _hoverText = "";
        private const int EVENTS_PER_PAGE = 6;

        public ClickableTextureComponent backButton;
        public ClickableTextureComponent forwardButton;
        public List<ClickableComponent> eventButtons = new List<ClickableComponent>();

        private NPC _targetNpc;
        private CaveOfMemoriesLocation _caveOfMemories;
        private List<EventFragment> _eventFragments;
        private List<List<EventFragment>> _pages;

        public EventSelectionMenu(NPC targetNpc, List<EventFragment> eventFragments, CaveOfMemoriesLocation caveOfMemories) : base(0, 0, 700, 550, showUpperRightCloseButton: true)
        {
            // Set up menu structure
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                base.height += 64;
            }

            _targetNpc = targetNpc;
            _eventFragments = eventFragments.OrderBy(e => e.RequiredHearts).ToList();
            _caveOfMemories = caveOfMemories;

            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y;

            Game1.playSound("bigSelect");
            PaginatePacks();

            // Establish the buttons that will be used to select the events
            for (int i = 0; i <= EVENTS_PER_PAGE; i++)
            {
                ClickableComponent packButton = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 16, base.yPositionOnScreen + 16 + i * ((base.height - 32) / 6), base.width - 32, (base.height - 32) / 6 + 4), string.Concat(i))
                {
                    myID = i,
                    downNeighborID = -7777,
                    upNeighborID = ((i > 0) ? (i - 1) : (-1)),
                    rightNeighborID = i + 103,
                    leftNeighborID = -7777,
                    fullyImmutable = true
                };
                eventButtons.Add(packButton);
            }

            // Set up the various other buttons
            backButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen - 64, base.yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 102,
                rightNeighborID = -7777
            };
            forwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 64 - 48, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 101
            };
            base.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width - 20, base.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
        }

        public void PaginatePacks()
        {
            _pages = new List<List<EventFragment>>();

            int count = _eventFragments.Count - 1;
            foreach (var contentPack in _eventFragments)
            {
                int which = _eventFragments.Count - 1 - count;
                int page = which / EVENTS_PER_PAGE;

                while (_pages.Count <= page)
                {
                    _pages.Add(new List<EventFragment>());
                }

                _pages[page].Add(contentPack);

                count--;
            }

            if (_pages.Count == 0)
            {
                _pages.Add(new List<EventFragment>());
            }
            _currentPage = Math.Min(Math.Max(_currentPage, 0), _pages.Count - 1);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && _currentPage > 0)
            {
                _currentPage--;
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && _currentPage < _pages.Count - 1)
            {
                _currentPage++;
                Game1.playSound("shiny4");
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key != 0)
            {
                if (key == Keys.Escape && base.readyToClose())
                {
                    base.exitThisMenu();
                }
                else if (Game1.options.snappyMenus && Game1.options.gamepadControls && !base.overrideSnappyMenuCursorMovementBan())
                {
                    this.applyMovementKey(key);
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            if (base.upperRightCloseButton != null && base.readyToClose() && base.upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound)
                {
                    Game1.playSound("bigDeSelect");
                }

                base.exitThisMenu();
            }

            for (int i = 0; i < eventButtons.Count; i++)
            {
                if (!(_pages.Count > 0 && _pages[_currentPage].Count > i))
                {
                    continue;
                }

                // Check if the events are being clicked
                if (eventButtons[i].containsPoint(x, y) && _pages[_currentPage][i] is not null)
                {
                    Game1.activeClickableMenu = new DialogueBox(String.Format(CaveOfMemories.i18n.Get("Dialogue.Memory.CharacterName"), _targetNpc.displayName));
                    Game1.afterDialogues = delegate
                    {
                        _caveOfMemories.StartEventRemembrance(_pages[_currentPage][i]);
                    };

                    base.exitThisMenu();
                    return;
                }
            }

            if (_currentPage < _pages.Count - 1 && forwardButton.containsPoint(x, y))
            {
                _currentPage++;
                Game1.playSound("shwip");
                if (Game1.options.SnappyMenus && _currentPage == _pages.Count - 1)
                {
                    base.currentlySnappedComponent = base.getComponentWithID(0);
                    snapCursorToCurrentSnappedComponent();
                }
                return;
            }
            if (_currentPage > 0 && backButton.containsPoint(x, y))
            {
                _currentPage--;
                Game1.playSound("shwip");
                if (Game1.options.SnappyMenus && _currentPage == 0)
                {
                    base.currentlySnappedComponent = base.getComponentWithID(0);
                    snapCursorToCurrentSnappedComponent();
                }
                return;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = String.Empty;

            for (int i = 0; i < eventButtons.Count; i++)
            {
                if (!(_pages.Count > 0 && _pages[_currentPage].Count > i))
                {
                    continue;
                }

                // Check if the events are being hovered
                if (eventButtons[i].containsPoint(x, y))
                {
                    if (_pages[_currentPage][i].Name.Length > 18)
                    {
                        _hoverText = $"{_pages[_currentPage][i].Name}";
                        return;
                    }
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.dialogueUp || Game1.IsFading())
            {
                return;
            }

            if (_eventFragments is null || _eventFragments.Count == 0)
            {
                base.exitThisMenu();
                return;
            }

            // General UI (title, background)
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, CaveOfMemories.i18n.Get("Menu.Event.Title"), base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen - 64);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f);

            // Draw the content pack buttons
            for (int j = 0; j < eventButtons.Count; j++)
            {
                if (_pages.Count() > 0 && _pages[_currentPage].Count() > j)
                {
                    var packName = _pages[_currentPage][j].Name;

                    if (packName.Length > 18)
                    {
                        packName = $"{packName.Substring(0, 18).TrimEnd()}...";
                    }

                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), eventButtons[j].bounds.X, eventButtons[j].bounds.Y, eventButtons[j].bounds.Width, eventButtons[j].bounds.Height, eventButtons[j].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, drawShadow: false);
                    SpriteText.drawStringHorizontallyCenteredAt(b, packName, eventButtons[j].bounds.X + eventButtons[j].bounds.Width / 2, eventButtons[j].bounds.Y + 20);
                }
            }

            if (_currentPage < _pages.Count - 1)
            {
                this.forwardButton.draw(b);
            }
            if (_currentPage > 0)
            {
                this.backButton.draw(b);
            }

            // Draw hover text
            if (!_hoverText.Equals(""))
            {
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                IClickableMenu.drawHoverText(b, _hoverText, Game1.smallFont);
            }
            base.upperRightCloseButton.draw(b);

            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }
    }
}
