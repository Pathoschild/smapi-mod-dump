/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BankOfFerngill.Framework.Data;
using BankOfFerngill.Framework.Menu.Pages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using MyStardewMods.Common;

namespace BankOfFerngill.Framework.Menu
{
    internal class BankTabbedMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        
        /// <summary>Encapsulates monitoring and logging.</summary>
        private readonly IMonitor _monitor;

        private readonly ITranslationHelper _i18N;

        private BankData _bankData;

        private ClickableTextureComponent _upArrow;
        private ClickableTextureComponent _downArrow;
        private ClickableTextureComponent _scrollbar;
        private readonly List<ClickableComponent> _tabs = new();
        private ClickableComponent _title;
        
        /// <summary>Whether the mod is running on Android.</summary>
        private readonly bool _isAndroid = Constants.TargetPlatform == GamePlatform.Android;

        private string _hoverText = "";
        private int _currentItemIndex;
        private bool _isScrolling;
        private Rectangle _scrollbarRunner;

        /// <summary>Whether the menu was opened in the current tick.</summary>
        private bool _justOpened = true;


        /*********
        ** Accessors
        *********/
        /// <summary>The currently open tab.</summary>
        private MenuTab CurrentTab { get; }



        private readonly List<IClickableMenu> _pages = new();
        /*********
        ** Public methods
        *********/
        
        public BankTabbedMenu(MenuTab initialTab, IMonitor monitor, ITranslationHelper i18N, BankData bankData, bool isNewMenu)
        {
            
            _monitor = monitor;
            _i18N = i18N;
            _bankData = bankData;
            CurrentTab = initialTab;
            ResetComponents();

            Game1.playSound(isNewMenu
                ? "bigSelect"   // menu open
                : "smallSelect" // tab select
            );
            
            //Lets make our Pages. We'll make one to start
            //pages.Clear();
            //pages.Add(new BankInfoPage(base.xPositionOnScreen, base.yPositionOnScreen, 800, 600));

            ResetComponents();
        }

        /// <summary>Exit the menu if that's allowed for the current state.</summary>
        private void ExitIfValid()
        {
            if (!readyToClose() || GameMenu.forcePreventClose) return;
            Game1.exitActiveMenu();
            Game1.playSound("bigDeSelect");
        }

        /// <summary>Whether controller-style menus should be disabled for this menu.</summary>
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }

        /// <summary>Handle the game window being resized.</summary>
        /// <param name="oldBounds">The previous window bounds.</param>
        /// <param name="newBounds">The new window bounds.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            ResetComponents();
        }

        /// <summary>Handle the player holding the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.leftClickHeld(x, y);
            if (_isScrolling)
            {
                var num = _scrollbar.bounds.Y;
                _scrollbar.bounds.Y = Math.Min(yPositionOnScreen + height - Game1.tileSize - Game1.pixelZoom * 3 - _scrollbar.bounds.Height, Math.Max(y, yPositionOnScreen + _upArrow.bounds.Height + Game1.pixelZoom * 5));
                SetScrollBarToCurrentIndex();
                if (num == _scrollbar.bounds.Y)
                    return;
                Game1.playSound("shiny4");
            }
        }

        /// <summary>Handle the player pressing a keyboard button.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveKeyPress(Keys key)
        {
            // exit menu
            if (Game1.options.menuButton.Contains(new InputButton(key))/* && !this.IsPressNewKeyActive()*/)
                ExitIfValid();

           
        }

        /// <summary>Handle the player pressing a controller button.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveGamePadButton(Buttons key)
        {
            // navigate tabs
            if (key is (Buttons.LeftShoulder or Buttons.RightShoulder)/* && !this.IsPressNewKeyActive()*/)
            {
                // rotate tab index
                var index = _tabs.FindIndex(p => p.name == CurrentTab.ToString());
                if (key == Buttons.LeftShoulder)
                    index--;
                if (key == Buttons.RightShoulder)
                    index++;

                if (index >= _tabs.Count)
                    index = 0;
                if (index < 0)
                    index = _tabs.Count - 1;

                // open menu with new index
                var tabId = GetTabId(_tabs[index]);
                Game1.activeClickableMenu = new BankTabbedMenu(tabId, _monitor, _i18N, _bankData, isNewMenu: false);
            }
        }

        /// <summary>Handle the player scrolling the mouse wheel.</summary>
        /// <param name="direction">The scroll direction.</param>
        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && _currentItemIndex > 0)
                UpArrowPressed();
            else
            {
                if (direction >= 0)
                    return;
                DownArrowPressed();
            }
        }

        /// <summary>Handle the player releasing the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
           
            _isScrolling = false;
        }

        /// <summary>Handle the player clicking the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        /// <param name="playSound">Whether to play a sound if needed.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveLeftClick(x, y, playSound);

            if (_downArrow.containsPoint(x, y))
            {
                DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_upArrow.containsPoint(x, y) && _currentItemIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_scrollbar.containsPoint(x, y))
                _isScrolling = true;
            else if (!_downArrow.containsPoint(x, y) && x > xPositionOnScreen + width && (x < xPositionOnScreen + width + Game1.tileSize * 2 && y > yPositionOnScreen) && y < yPositionOnScreen + height)
            {
                _isScrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }
            

            foreach (var tab in _tabs)
            {
                if (tab.bounds.Contains(x, y))
                {
                    var tabId = GetTabId(tab);
                    Game1.activeClickableMenu = new BankTabbedMenu(tabId, _monitor, _i18N, _bankData, isNewMenu: false);
                    break;
                }
            }
        }

        /// <summary>Handle the player hovering the cursor over the menu.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void performHoverAction(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            _hoverText = "";
            _upArrow.tryHover(x, y);
            _downArrow.tryHover(x, y);
            _scrollbar.tryHover(x, y);
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            if (!Game1.options.showMenuBackground)
                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            base.draw(spriteBatch);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            CommonHelper.DrawTab(_title.bounds.X, _title.bounds.Y, Game1.dialogueFont, _title.name, 1);
            spriteBatch.End();
            //End Title Draw
            //Start Page Draw
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            var i = _tabs.FindIndex(p => p.name == CurrentTab.ToString());
            _pages[i].draw(spriteBatch);
            spriteBatch.End();
            //End Page Draw
            //Begin Tab Draw
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            
            if (!GameMenu.forcePreventClose)
            {
                foreach (var tab in _tabs)
                {
                    var tabId = GetTabId(tab);
                    CommonHelper.DrawTab(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.label, 2, CurrentTab == tabId ? 1F : 0.7F);
                }

                _upArrow.draw(spriteBatch);
                _downArrow.draw(spriteBatch);
                
            }
            if (_hoverText != "")
                drawHoverText(spriteBatch, _hoverText, Game1.smallFont);

            if (!Game1.options.hardwareCursor && !_isAndroid)
                spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);

            // reinitialize the UI to fix Android pinch-zoom scaling issues
            if (_justOpened)
            {
                _justOpened = false;
                if (_isAndroid)
                    ResetComponents();
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Initialize or reinitialize the UI components.</summary>
        [MemberNotNull(nameof(_downArrow), nameof(_scrollbar), nameof(_scrollbarRunner), nameof(_title), nameof(_upArrow))]
        private void ResetComponents()
        {
            // set dimensions
            width = (_isAndroid ? 750 : 1200) + borderWidth * 2;
            height = (_isAndroid ? 550 : 760) + borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width - (int)(Game1.tileSize * 2.4f)) / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;
/*
            xPositionOnScreen = (int)(Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - 
                                     (width / 2);

            yPositionOnScreen = (int)(Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 -
                                     (height / 2);*/
            
            // show close button on Android
            if (_isAndroid)
                initializeUpperRightCloseButton();

            // add title
            _title = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2, yPositionOnScreen, Game1.tileSize * 4, Game1.tileSize), _i18N.Get("bank.info.title"));

            // add tabs
            {
                var i = 0;
                var labelX = (int)(xPositionOnScreen - Game1.tileSize * 4.8f);
                var labelY = (int)(yPositionOnScreen + Game1.tileSize * (_isAndroid ? 1.25f : 1.5f));
                const int labelHeight = (int)(Game1.tileSize * 0.9F);

                _tabs.Clear();
                _tabs.AddRange(new[]
                {
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.BankInfo.ToString(), _i18N.Get("bank.menu.bankinfo")),
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Deposit.ToString(), _i18N.Get("bank.menu.deposit")),
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.Withdraw.ToString(), _i18N.Get("bank.menu.withdraw")),
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTab.TakeOutLoan.ToString(), _i18N.Get("bank.menu.takeloan")),
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i, Game1.tileSize * 5, Game1.tileSize), MenuTab.PayBackLoan.ToString(), _i18N.Get("bank.menu.payloan"))
                });
                
                //add pages
                _pages.Clear();
                _pages.Add(new BankInfoPage(xPositionOnScreen, yPositionOnScreen, 1200, 760, _monitor, _i18N, _bankData));
                _pages.Add(new DepositPage(xPositionOnScreen, yPositionOnScreen, 1200, 760, _monitor, _i18N, _bankData));
               /* _pages.AddRange(new[]
                {
                    new BankInfoPage(xPositionOnScreen, yPositionOnScreen, 1200, 760, _monitor, _i18N, _bankData),
                    new BankInfoPage(xPositionOnScreen, yPositionOnScreen, 1200, 760, _monitor, _i18N, _bankData),
                    new BankInfoPage(xPositionOnScreen, yPositionOnScreen, 1200, 760, _monitor, _i18N, _bankData),
                    new BankInfoPage(xPositionOnScreen, yPositionOnScreen, 1200, 760, _monitor, _i18N, _bankData),
                    new BankInfoPage(xPositionOnScreen, yPositionOnScreen, 1200, 760, _monitor, _i18N, _bankData),
                });*/
            }

            // add scroll UI
            var scrollbarOffset = Game1.tileSize * (_isAndroid ? 1 : 4) / 16;
            _upArrow = new ClickableTextureComponent("up-arrow", new Rectangle(xPositionOnScreen + width + scrollbarOffset, yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            _downArrow = new ClickableTextureComponent("down-arrow", new Rectangle(xPositionOnScreen + width + scrollbarOffset, yPositionOnScreen + height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            _scrollbar = new ClickableTextureComponent("scrollbar", new Rectangle(_upArrow.bounds.X + Game1.pixelZoom * 3, _upArrow.bounds.Y + _upArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            _scrollbarRunner = new Rectangle(_scrollbar.bounds.X, _upArrow.bounds.Y + _upArrow.bounds.Height + Game1.pixelZoom, _scrollbar.bounds.Width, height - Game1.tileSize * 2 - _upArrow.bounds.Height - Game1.pixelZoom * 2);
            SetScrollBarToCurrentIndex();

            
        }
        
        

        
        /// <summary>Get the first button in a key binding, if any.</summary>
        /// <param name="keybindList">The key binding list.</param>
        private SButton GetSingleButton(KeybindList keybindList)
        {
            return (from keybind in keybindList.Keybinds where keybind.IsBound select keybind.Buttons.First()).FirstOrDefault();
        }

        
        /// <summary>Add descriptive text that may extend onto multiple lines if it's too long.</summary>
        /// <param name="text">The text to render.</param>
        private void AddDescription(string text)
        {
            // get text lines
            var maxWidth = width - Game1.tileSize - 10;

            foreach (var originalLine in text.Replace("\r\n", "\n").Split('\n'))
            {
                var line = "";
                foreach (var word in originalLine.Split(' '))
                {
                    if (line == "")
                        line = word;
                    else if (Game1.smallFont.MeasureString(line + " " + word).X <= maxWidth)
                        line += " " + word;
                    else
                    {
                        //this.Options.Add(new DescriptionElement(line));
                        line = word;
                    }
                }
                //if (line != "")
                    //this.Options.Add(new DescriptionElement(line));
            }
        }

       

        

        private void SetScrollBarToCurrentIndex()
        {
            _scrollbar.bounds.Y = _downArrow.bounds.Y - _scrollbar.bounds.Height - Game1.pixelZoom;
        }

        private void DownArrowPressed()
        {
            _downArrow.scale = _downArrow.baseScale;
            ++_currentItemIndex;
            SetScrollBarToCurrentIndex();
        }

        private void UpArrowPressed()
        {
            _upArrow.scale = _upArrow.baseScale;
            --_currentItemIndex;
            SetScrollBarToCurrentIndex();
        }

        /// <summary>Get the tab constant represented by a tab component.</summary>
        /// <param name="tab">The component to check.</param>
        private static MenuTab GetTabId(ClickableComponent tab)
        {
            if (!Enum.TryParse(tab.name, out MenuTab tabId))
                throw new InvalidOperationException($"Couldn't parse tab name '{tab.name}'.");
            return tabId;
        }
    }
}