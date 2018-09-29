using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Kisekae.Config;
using Kisekae.Menu;

namespace Kisekae.Framework {
    /// <summary>The menu which lets the player customise their character's appearance.</summary>
    internal class MenuFarmerMakeup : AutoMenu {
        /*********
        ** Properties
        *********/
        #region Metadata
        /// <summary>The global config settings.</summary>
        private readonly GlobalConfig m_globalConfig;
        /// <summary>Core component to manipulate player appearance.</summary>
        private readonly FarmerMakeup m_farmerMakeup;
        /// <summary>Our local menu texture.</summary>
        private readonly Texture2D m_menuTextures;
        #endregion

        #region GUI Components
        /// <summary>The customisation menu tabs.</summary>
        private enum MenuTab {
            /// <summary>The main character customisation screen.</summary>
            Customise = 0,
            /// <summary>The mange favorites screen.</summary>
            ManageFavorites,
            /// <summary>The main 'favorites' screen.</summary>
            Favorites,
            /// <summary>The favorites 'extra outfits' submenu.</summary>
            FavoritesExtras,
            /// <summary>The main 'about' screen.</summary>
            About,
        }
        /// <summary>Tabs and subtabs.</summary>
        private List<TabMenu> m_tabs = new List<TabMenu>();
        /// <summary>Parent of Tab.</summary>
        private List<int> m_tabParents = new List<int>();
        /// <summary>The tabs used to switch submenu.</summary>
        private List<ClickableTextureComponent> m_tabMenus = new List<ClickableTextureComponent>();

        private ClickableTextureComponent CancelButton;
        /// <summary>A floating arrow which brings attention to the 'favorites' tab.</summary>
        private TemporaryAnimatedSprite FavTabArrow;
        /// <summary>A 'new' sprite that brings attention to the tabs.</summary>
        private TemporaryAnimatedSprite FloatingNew;
        #endregion

        #region States
        /// <summary>Whether to show the <see cref="FavTabArrow"/>.</summary>
        public bool ShowFavTabArrow = false;
        /// <summary>The current tab being viewed.</summary>
        private int m_curTab = (int)MenuTab.Customise;
        /// <summary>The tooltip text to draw next to the cursor.</summary>
        private string HoverText;
        /// <summary>The zoom level before the menu was opened.</summary>
        private float m_playerZoomLevel;
        #endregion

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="contentHelper">Encapsulates the underlying mod texture management.</param>
        /// <param name="modHelper">Provides simplified APIs for writing mods.</param>
        /// <param name="modVersion">The current mod version.</param>
        /// <param name="globalConfig">The global config settings.</param>
        /// <param name="playerConfig">The current per-save config settings.</param>
        /// <param name="zoomLevel">The zoom level before the menu was opened.</param>
        public MenuFarmerMakeup(IMod env, FarmerMakeup farmerMakeup, GlobalConfig globalConfig)
            : base(env, 0, 0,
                  width: 700 + s_borderSize,
                  height: 580 + s_borderSize
            ) {
            // save metadata
            m_globalConfig = globalConfig;
            m_farmerMakeup = farmerMakeup;
            m_farmerMakeup.m_farmer = Game1.player;
            m_playerZoomLevel = Game1.options.zoomLevel;
            exitFunction = exit;
            m_menuTextures = Game1.content.Load<Texture2D>(ContentHelper.s_MenuTextureKey);

            // build menu
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();

            if (m_globalConfig.MenuZoomLock) {
                Game1.options.zoomLevel = m_globalConfig.MenuZoomOut ? 0.75f : 1f;
            }
            Game1.overrideGameMenuReset = true;
            Game1.game1.refreshWindowSettings();
            m_tabs.Add(new MenuCustomize(env, globalConfig, m_farmerMakeup.m_config, m_farmerMakeup, this));
            m_tabParents.Add(-1);
            m_tabs.Add(null);
            m_tabParents.Add(-1);
            m_tabs.Add(new MenuFavorites(env, m_farmerMakeup, this));
            m_tabParents.Add((int)MenuTab.ManageFavorites);
            m_tabs.Add(new MenuFavoritesExtras(env, m_farmerMakeup));
            m_tabParents.Add((int)MenuTab.ManageFavorites);
            m_tabs.Add(new MenuAbout(env, globalConfig, ref m_playerZoomLevel));
            m_tabParents.Add(-1);

            this.updateLayout();
        }

        #region EventHandler
        /// <summary>The method invoked when the player presses the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            // tabs
            for (int i = 0; i < m_tabMenus.Count; ++i) {
                if (IsVisibleTab(i) && m_tabMenus[i].containsPoint(x, y)) {
                    if (i == m_tabParents[m_curTab]) {
                        return;
                    }
                    if (m_tabs[i] == null) {
                        ++i;
                    }
                    Game1.playSound("smallSelect");
                    this.SetTab(i);
                    return;
                }
            }
            if (this.CancelButton.containsPoint(x, y)) {
                exitThisMenuNoSound();
            }
            // hide 'new' button
            if (m_globalConfig.ShowIntroBanner) {
                m_globalConfig.ShowIntroBanner = false;
                m_env.Helper.WriteConfig(m_globalConfig);
            }
            // tab contents
            m_tabs[m_curTab].receiveLeftClick(x, y, playSound);
        }
        /// <summary>The method invoked while the player is holding down the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void leftClickHeld(int x, int y) {
            try {
                m_tabs[m_curTab].leftClickHeld(x, y);
            } catch { }
        }
        /// <summary>The method invoked when the player releases the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void releaseLeftClick(int x, int y) {
            try {
                m_tabs[m_curTab].releaseLeftClick(x, y);
            } catch { }
        }
        /// <summary>The method invoked when the player presses the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) {
            try {
                m_tabs[m_curTab].receiveRightClick(x, y, playSound);
            } catch { }
        }
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveKeyPress(Keys key) {
            // exit menu
            if (Game1.options.menuButton.Contains(new InputButton(key)) && this.readyToClose()) {
                exitThisMenuNoSound();
                return;
            }
            try {
                m_tabs[m_curTab].receiveKeyPress(key);
            } catch { }
        }
        /// <summary>Update the menu state.</summary>
        /// <param name="time">The elapsed game time.</param>
        public override void update(GameTime time) {
            base.update(time);

            m_tabs[m_curTab].update(time);

            // update tab arrows
            this.FavTabArrow?.update(time);
            this.FloatingNew?.update(time);
        }
        /// <summary>The method invoked when the cursor is over a given position.</summary>
        /// <param name="x">The X mouse position.</param>
        /// <param name="y">The Y mouse position.</param>
        public override void performHoverAction(int x, int y) {
            base.performHoverAction(x, y);

            // reset hover text
            this.HoverText = "";

            // check subtab hovers
            foreach (ClickableComponent current in m_tabMenus) {
                if (current.containsPoint(x, y)) {
                    if (current.name.Equals("Quick Outfits") || current.name.Equals("Extra Outfits")) {
                        if (this.m_curTab == (int)MenuTab.Favorites || this.m_curTab == (int)MenuTab.FavoritesExtras)
                            this.HoverText = current.name;
                    }
                    return;
                }
            }

            // cancel buttons
            this.CancelButton.tryHover(x, y, 0.25f);
            this.CancelButton.tryHover(x, y, 0.25f);

            // tab contents
            m_tabs[m_curTab].performHoverAction(x, y);
        }
        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch to which to draw.</param>
        public override void draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);

            // menu background
            Game1.drawDialogueBox(xPositionOnScreen - Game1.tileSize / 4, yPositionOnScreen - Game1.tileSize - Game1.tileSize/4, width + Game1.tileSize / 2, height + Game1.tileSize + Game1.tileSize/2, false, true);

            // tabs
            {
                // check selected tab
                bool isCustomiseTab = this.m_curTab == (int)MenuTab.Customise;
                bool isFavoriteTab = this.m_curTab == (int)MenuTab.Favorites || this.m_curTab == (int)MenuTab.FavoritesExtras;
                bool isAboutTab = this.m_curTab == (int)MenuTab.About;
                bool isMainOutfitsTab = this.m_curTab == (int)MenuTab.Favorites;
                bool isExtraOutfitsTab = this.m_curTab == (int)MenuTab.FavoritesExtras;

                // get tab positions
                Vector2 character = new Vector2(xPositionOnScreen + 45 + 64 * 0, yPositionOnScreen - Game1.tileSize + (isCustomiseTab ? 9 : 0));
                Vector2 favorites = new Vector2(xPositionOnScreen + 45 + 64 * 1, yPositionOnScreen - Game1.tileSize + (isFavoriteTab ? 9 : 0));
                Vector2 about = new Vector2(xPositionOnScreen + 45 + 64 * 2, yPositionOnScreen - Game1.tileSize + (isAboutTab ? 9 : 0));
                Vector2 quickFavorites = new Vector2(xPositionOnScreen - (isMainOutfitsTab ? 57 : 64), yPositionOnScreen + Game1.tileSize / 2);
                Vector2 extraFavorites = new Vector2(xPositionOnScreen - (isExtraOutfitsTab ? 57 : 64), yPositionOnScreen + Game1.tileSize / 2 + 64);

                // customise tab
                spriteBatch.Draw(Game1.mouseCursors, character, new Rectangle(16, 368, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                Game1.player.FarmerRenderer.drawMiniPortrat(spriteBatch, new Vector2(xPositionOnScreen + 53, yPositionOnScreen - Game1.tileSize + (Game1.player.isMale ? (isCustomiseTab ? 19 : 10) : (isCustomiseTab ? 16 : 7))), 0.00011f, 3f, 2, Game1.player);
                // favorite tab
                spriteBatch.Draw(Game1.mouseCursors, favorites, new Rectangle(16, 368, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                m_tabMenus[(int)MenuTab.ManageFavorites].draw(spriteBatch);
                // about tab
                spriteBatch.Draw(Game1.mouseCursors, about, new Rectangle(16, 368, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                m_tabMenus[(int)MenuTab.About].draw(spriteBatch);
                // favorite subtabs
                if (isFavoriteTab) {
                    spriteBatch.Draw(m_menuTextures, quickFavorites, new Rectangle(52, 202, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                    spriteBatch.Draw(m_menuTextures, extraFavorites, new Rectangle(52, 202, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);
                    m_tabMenus[(int)MenuTab.Favorites].draw(spriteBatch);
                    m_tabMenus[(int)MenuTab.FavoritesExtras].draw(spriteBatch);
                }
            }

            // cancel button
            this.CancelButton.draw(spriteBatch);

            // tab floaters
            if (m_globalConfig.ShowIntroBanner)
                FloatingNew?.draw(spriteBatch, true, 400, 950);
            if (this.ShowFavTabArrow)
                FavTabArrow?.draw(spriteBatch, true, 400, 950);

            // tab contents
            m_tabs[m_curTab].draw(spriteBatch);

            // hovertext
            IClickableMenu.drawHoverText(spriteBatch, this.HoverText, Game1.smallFont);
        }
        /// <summary>Update the menu layout for a change in the zoom level or viewport size.</summary>
        public override void updateLayout() {
            // reset window position
            this.xPositionOnScreen = (Game1.viewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.viewport.Height - this.height) / 2;
            this.yPositionOnScreen += Game1.tileSize / 2;

            // tabs
            m_tabMenus.Clear();
            m_tabMenus.Add(new ClickableTextureComponent("Customize Character", new Rectangle(xPositionOnScreen + 62, yPositionOnScreen - Game1.tileSize + 24, 50, 50), "", "", m_menuTextures, new Rectangle(9, 48, 8, 11), Game1.pixelZoom));
            m_tabMenus.Add(new ClickableTextureComponent("Manage Favorites", new Rectangle(xPositionOnScreen + 125, yPositionOnScreen - Game1.tileSize + 24, 50, 50), "", "", m_menuTextures, new Rectangle(24, 26, 8, 8), Game1.pixelZoom));
            m_tabMenus.Add(new ClickableTextureComponent("Quick Outfits", new Rectangle(xPositionOnScreen - 40, yPositionOnScreen + Game1.tileSize / 2 + 15, 50, 50), "", "", m_menuTextures, new Rectangle(8, 26, 8, 8), Game1.pixelZoom));
            m_tabMenus.Add(new ClickableTextureComponent("Extra Outfits", new Rectangle(xPositionOnScreen - 33, yPositionOnScreen + Game1.tileSize / 2 + 15 + Game1.tileSize, 50, 50), "", "", m_menuTextures, new Rectangle(0, 26, 8, 8), Game1.pixelZoom));
            m_tabMenus.Add(new ClickableTextureComponent("About", new Rectangle(xPositionOnScreen + 188, yPositionOnScreen - Game1.tileSize + 17, 50, 50), "", "", m_menuTextures, new Rectangle(0, 48, 8, 11), Game1.pixelZoom));

            // tab positions
            UpdateTabFloaters();
            UpdateTabPositions();

            // cancel button
            this.CancelButton = new ClickableTextureComponent(new Rectangle((xPositionOnScreen + 675) + Game1.pixelZoom * 12, yPositionOnScreen - Game1.tileSize, Game1.pixelZoom * 10, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), Game1.pixelZoom);

            m_tabs[m_curTab].updateLayout();
        }
        #endregion

        /*********
        ** Private methods
        *********/
        /// <summary>Reinitialise components which bring attention to tabs.</summary>
        private void UpdateTabFloaters() {
            this.FloatingNew = new TemporaryAnimatedSprite(ContentHelper.s_MenuTextureKey, new Rectangle(0, 102, 23, 9), 115, 5, 1, new Vector2(xPositionOnScreen - 90, yPositionOnScreen), false, false, 0.89f, 0f, Color.White, Game1.pixelZoom, 0f, 0f, 0f, true) {
                totalNumberOfLoops = 1,
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = Game1.tileSize / 8f
            };

            this.FavTabArrow = new TemporaryAnimatedSprite(ContentHelper.s_MenuTextureKey, new Rectangle(1, 120, 12, 14), 100f, 3, 5, new Vector2(xPositionOnScreen + 120, yPositionOnScreen - Game1.tileSize), false, false, 0.89f, 0f, Color.White, 3f, 0f, 0f, 0f, true) {
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = Game1.tileSize / 8f
            };
        }

        /// <summary>Recalculate the positions for all tabs and subtabs.</summary>
        private void UpdateTabPositions() {
            m_tabMenus[(int)MenuTab.Customise].bounds.Y = this.yPositionOnScreen - Game1.tileSize + (this.m_curTab == (int)MenuTab.Customise ? 34 : 24);
            m_tabMenus[(int)MenuTab.ManageFavorites].bounds.Y = this.yPositionOnScreen - Game1.tileSize + (this.m_curTab == (int)MenuTab.Favorites || this.m_curTab == (int)MenuTab.FavoritesExtras ? 34 : 24);
            m_tabMenus[(int)MenuTab.Favorites].bounds.X = this.xPositionOnScreen - (this.m_curTab == (int)MenuTab.Favorites ? 33 : 40);
            m_tabMenus[(int)MenuTab.FavoritesExtras].bounds.X = this.xPositionOnScreen - (this.m_curTab == (int)MenuTab.FavoritesExtras ? 33 : 40);
            m_tabMenus[(int)MenuTab.About].bounds.Y = this.yPositionOnScreen - Game1.tileSize + (this.m_curTab == (int)MenuTab.About ? 27 : 17);
        }
        /// <summary>Whether the tab shuold be shown on screen.</summary>
        private bool IsVisibleTab(int tabIndex) {
            return (m_tabParents[tabIndex] < 0 || m_tabParents[tabIndex] == m_tabParents[m_curTab]);
        }
        /// <summary>Switch to the given tab.</summary>
        /// <param name="tab">The tab to display.</param>
        private void SetTab(int tab) {
            if (this.m_curTab == tab) {
                return;
            }
            this.m_curTab = tab;
            m_tabs[tab].onSwitchBack();
            this.UpdateTabPositions();
        }
        /// <summary>call on exit.</summary>
        private void exit() {
            for (int i = 0; i < m_tabs.Count; ++i) {
                if (m_tabs[i]?.exitFunction != null) {
                    m_tabs[i].exitFunction();
                }
            }
            Game1.playSound("yoba");

            if (m_globalConfig.MenuZoomLock) {
                Game1.options.zoomLevel = m_playerZoomLevel;
            }
            Game1.overrideGameMenuReset = true;
            Game1.game1.refreshWindowSettings();
            Game1.player.canMove = true;
            Game1.flashAlpha = 1f;
        }
    }
}
