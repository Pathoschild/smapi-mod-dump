using Kisekae.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Kisekae.Menu;
using System;

namespace Kisekae.Framework {
    internal class MenuAbout : TabMenu {
        /*********
        ** Properties
        *********/
        /// <summary>The global config settings.</summary>
        private readonly GlobalConfig m_globalConfig;
        /// <summary>Our local menu texture.</summary>
        Texture2D m_menuTextures;

        /// <summary>The button which zooms out the menu.</summary>
        private ClickableTextureButton m_zoomOutButton;
        /// <summary>The button which zooms im the menu.</summary>
        private ClickableTextureButton m_zoomInButton;
        /// <summary>The button which controls zoom lock.</summary>
        private ClickableTextureButton m_zoomLockButton;
        /// <summary>The label showing access key.</summary>
        private LabelComponent m_accessKeyLabel;
        /// <summary>The label showing number of bottoms.</summary>
        private LabelComponent m_bottomNumberLabel;
        /// <summary>The label showing zoom info.</summary>
        private LabelComponent m_zoomLevelLabel;
        /// <summary>The label showing whether show dresser.</summary>
        private LabelComponent m_showDresserLabel;

        /// <summary>Whether the player is currently setting the menu key via <see cref="SetAccessKeyButton"/>.</summary>
        private bool m_isSettingAccessMenuKey;
        /// <summary>The zoom level before the menu was opened.</summary>
        private float m_playerZoomLevel;
        /*********
        ** Public methods
        *********/
        public MenuAbout(IMod env, GlobalConfig globalConfig, ref float playerZoomLevel)
            : base(env, 0, 0,
                  width: 700 + s_borderSize,
                  height: 580 + s_borderSize
            ) {
            m_globalConfig = globalConfig;
            m_playerZoomLevel = playerZoomLevel;
            m_menuTextures = Game1.content.Load<Texture2D>(ContentHelper.s_MenuTextureKey);
            updateLayout();
        }
        /// <summary>The method invoked when the player presses a keyboard button.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveKeyPress(Keys key) {
            if (m_isSettingAccessMenuKey) {
                m_globalConfig.MenuAccessKey = (SButton)key;
                m_env.Helper.WriteConfig(m_globalConfig);
                m_isSettingAccessMenuKey = false;
                m_alerts.Add(new Alert(m_menuTextures, new Rectangle(96, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Menu Access Key Changed.", 1200, false));
                m_accessKeyLabel.label = "Open Menu Key:  " + m_globalConfig.MenuAccessKey;
                Game1.playSound("coin");
            }
        }
        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch to which to draw.</param>
        public override void draw(SpriteBatch spriteBatch) {
            base.draw(spriteBatch);
            // set menu access key overlay
            if (m_isSettingAccessMenuKey) {
                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.90f);
                Vector2 strMeasure = Game1.dialogueFont.MeasureString("Press new key...");
                spriteBatch.DrawString(Game1.dialogueFont, "Press new key...", new Vector2((Game1.viewport.Width - strMeasure.X) / 2, (Game1.viewport.Height - strMeasure.Y) / 2), Color.White); // xPositionOnScreen + 225, yPositionOnScreen + 280
            }
        }
        /// <summary>Update the menu layout for a change in the zoom level or viewport size.</summary>
        public override void updateLayout() {
            // reset window position
            this.xPositionOnScreen = (Game1.viewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.viewport.Height - this.height) / 2;
            this.yPositionOnScreen += Game1.tileSize / 2;

            m_alerts.Clear();
            m_components.Clear();

            // info
            int yOffset = this.yPositionOnScreen + IClickableMenu.borderWidth;
            int xOffset = this.xPositionOnScreen + IClickableMenu.borderWidth;
            m_components.Add(new LabelComponent(xOffset, yOffset, "Kisekae") { m_isTitle = true });
            yOffset += 50;
            m_components.Add(new LabelComponent(xOffset, yOffset, "A modified version of Get Dressed to work with SDV 1.3"));
            m_components.Add(new LabelComponent(xOffset, yOffset + 26, $"You are using version:  {m_env.ModManifest.Version}"));
            yOffset += 60;
            m_components.Add(new LabelComponent(xOffset, yOffset, "Settings:") { m_isTitle = true });
            m_components.Add(new LabelComponent(xOffset, yOffset + 60, "Face Types (M-F): " + m_globalConfig.MaleFaceTypes + "-" + m_globalConfig.FemaleFaceTypes));
            m_components.Add(new LabelComponent(xOffset + 300, yOffset + 60, "Nose Types (M-F): " + m_globalConfig.MaleNoseTypes + "-" + m_globalConfig.FemaleNoseTypes));
            m_components.Add(new LabelComponent(xOffset, yOffset + 110, "Shoes Types (M-F): " + m_globalConfig.MaleShoeTypes + "-" + m_globalConfig.FemaleShoeTypes));
            m_bottomNumberLabel = new LabelComponent(xOffset + 300, yOffset + 110, "Bottoms Types (M-F): " + (m_globalConfig.HideMaleSkirts ? 2 : m_globalConfig.MaleBottomsTypes) + "-" + m_globalConfig.FemaleBottomsTypes);
            m_components.Add(m_bottomNumberLabel);
            m_components.Add(new LabelComponent(xOffset, yOffset + 160, "Stove in Corner: " + m_globalConfig.StoveInCorner));
            m_showDresserLabel = new LabelComponent(xOffset + 300, yOffset + 160, "Show Dresser: " + m_globalConfig.ShowDresser);
            m_components.Add(m_showDresserLabel);
            m_components.Add(new ClickableTextureButton("SetDresser", new Rectangle(xOffset + 560, yOffset + 160, Game1.pixelZoom * 15, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f) { m_hoverScale = 0.25f });
            // set menu access key
            m_accessKeyLabel = new LabelComponent(xOffset, yOffset + 210, "Open Menu Key: " + m_globalConfig.MenuAccessKey);
            m_components.Add(m_accessKeyLabel);
            m_components.Add(new ClickableTextureButton("ClearAccessKey", new Rectangle(xOffset + 510, yOffset + 208, 30, 30), m_menuTextures, new Rectangle(50, 146, 12, 12), 3f) { m_hoverScale = 0.25f });
            m_components.Add(new ClickableTextureButton("SetAccessKey", new Rectangle(xOffset + 560, yOffset + 210, 21 * 3, 11 * 3), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f) { m_hoverScale = 0.25f });
            // toggle skirs for male characters
            m_components.Add(new LabelComponent(xOffset, yOffset + 260, "Toggle Skirts for Male Characters"));
            m_components.Add(new ClickableTextureButton("SetMaleShirt", new Rectangle(xOffset + 560, yOffset + 260, Game1.pixelZoom * 15, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f) { m_hoverScale = 0.25f });
            // set gender change
            m_components.Add(new LabelComponent(xOffset, yOffset + 310, "Can Change Gender"));
            m_components.Add(new ClickableTextureButton("SetGenderButton", new Rectangle(xOffset + 560, yOffset + 310, Game1.pixelZoom * 15, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f) { m_hoverScale = 0.25f });
            // set zoom level
            int zoomValue = (int)Math.Round((double)Game1.options.zoomLevel * 100.0);
            bool canZoomOut = (m_globalConfig.MenuZoomLock && !m_globalConfig.MenuZoomOut) || (!m_globalConfig.MenuZoomLock && zoomValue > 75);
            bool canZoomIn = (m_globalConfig.MenuZoomLock && m_globalConfig.MenuZoomOut) || (!m_globalConfig.MenuZoomLock && zoomValue < 125);
            m_zoomLevelLabel = new LabelComponent(xOffset, yOffset + 360, (m_globalConfig.MenuZoomLock ? "Independent Zoom Level: ": "Game Zoom Level: ")+(zoomValue) +"%");
            m_components.Add(m_zoomLevelLabel);
            m_zoomOutButton = new ClickableTextureButton("ZoomOut", new Rectangle(xOffset + 530, yOffset + 360, Game1.pixelZoom * 10, Game1.pixelZoom * 10), m_menuTextures, new Rectangle(0, canZoomOut ? 167 : 177, 7, 9), 3f) { m_hoverScale = canZoomOut ? 0.25f : 0f };
            m_zoomLockButton = new ClickableTextureButton("ZoomLock", new Rectangle(xOffset + 575, yOffset + 360 - 4, (int)(12*2.5), (int)(14*2.5)), m_menuTextures, new Rectangle(m_globalConfig.MenuZoomLock ? 178:162, m_globalConfig.MenuZoomLock ? 145 : 161, 12, 14), 2.5f) { m_hoverScale = 0.25f };
            m_zoomInButton = new ClickableTextureButton("ZoomIn", new Rectangle(xOffset + 630, yOffset + 360, Game1.pixelZoom * 10, Game1.pixelZoom * 10), m_menuTextures, new Rectangle(10, canZoomIn ? 167 : 177, 7, 9), 3f) { m_hoverScale = canZoomIn ? 0.25f : 0f };
            m_components.Add(m_zoomOutButton);
            m_components.Add(m_zoomInButton);
            m_components.Add(m_zoomLockButton);
            // reset config options
            m_components.Add(new LabelComponent(xOffset, yOffset + 410, "Reset Options to Default"));
            m_components.Add(new ClickableTextureButton("Reset", new Rectangle(xOffset + 560, yOffset + 410, Game1.pixelZoom * 15, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), 3f) { m_hoverScale = 0.25f });
        }
        /// <summary>Perform the action associated with a component.</summary>
        /// <param name="cpt">The component.</param>
        /// <return>whether the component action is processed</return>
        public override bool handleLeftClick(IAutoComponent cpt) {
            switch (cpt.m_name) {
                case "SetDresser":
                    m_globalConfig.ShowDresser = !m_globalConfig.ShowDresser;
                    this.m_showDresserLabel.label = "Show Dresser: " + m_globalConfig.ShowDresser;
                    m_alerts.Add(new Alert(m_menuTextures, new Rectangle(m_globalConfig.ShowDresser ? 80 : 48, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Restart game required.", 1200, false));
                    m_env.Helper.WriteConfig(m_globalConfig);
                    Game1.playSound("coin");
                    return true;
                case "ClearAccessKey":
                    m_globalConfig.MenuAccessKey = SButton.None;
                    m_accessKeyLabel.label = "Open Menu Key:  " + m_globalConfig.MenuAccessKey;
                    m_env.Helper.WriteConfig(m_globalConfig);
                    m_alerts.Add(new Alert(m_menuTextures, new Rectangle(96, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Menu Access Key Cleared.", 1200, false));
                    Game1.playSound("breathin");
                    return true;
                case "SetAccessKey":
                    m_isSettingAccessMenuKey = true;
                    m_env.Helper.WriteConfig(m_globalConfig);
                    Game1.playSound("breathin");
                    return true;
                case "SetMaleShirt":
                    m_globalConfig.HideMaleSkirts = !m_globalConfig.HideMaleSkirts;
                    m_bottomNumberLabel.label = "Bottoms Types (M-F): " + (m_globalConfig.HideMaleSkirts ? 2 : m_globalConfig.MaleBottomsTypes) + "-" + m_globalConfig.FemaleBottomsTypes;
                    m_env.Helper.WriteConfig(m_globalConfig);
                    m_alerts.Add(new Alert(m_menuTextures, new Rectangle(m_globalConfig.HideMaleSkirts ? 48 : 80, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Skirts " + (m_globalConfig.HideMaleSkirts ? "Hidden" : "Unhidden") + " for Males.", 1200, false));
                    Game1.playSound("coin");
                    FarmerMakeup.MaleBottomsTypes = m_globalConfig.HideMaleSkirts ? 2 : m_globalConfig.MaleBottomsTypes;
                    return true;
                case "SetGenderButton":
                    m_globalConfig.CanChangeGender = !m_globalConfig.CanChangeGender;
                    m_env.Helper.WriteConfig(m_globalConfig);
                    m_alerts.Add(new Alert(m_menuTextures, new Rectangle(!m_globalConfig.CanChangeGender ? 48 : 80, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, (m_globalConfig.CanChangeGender ? "Enable" : "Disable") + " gender change.", 1200, false));
                    Game1.playSound("axe");
                    return true;
                case "ZoomLock":
                    m_globalConfig.MenuZoomLock = !m_globalConfig.MenuZoomLock;
                    if (m_globalConfig.MenuZoomLock) {
                        m_playerZoomLevel = Game1.options.zoomLevel;
                        Game1.options.zoomLevel = m_globalConfig.MenuZoomOut? 0.75f : 1f;
                    } else {
                        Game1.options.zoomLevel = m_playerZoomLevel;
                    }
                    Game1.overrideGameMenuReset = true;
                    Game1.game1.refreshWindowSettings();
                    m_env.Helper.WriteConfig(m_globalConfig);
                    Game1.playSound("coin");
                    return true;
                case "ZoomOut":
                    if (m_globalConfig.MenuZoomLock) {
                        if (m_globalConfig.MenuZoomOut) {
                            return false;
                        }
                        Game1.options.zoomLevel = 0.75f;
                        Game1.overrideGameMenuReset = true;
                        Game1.game1.refreshWindowSettings();

                        m_globalConfig.MenuZoomOut = true;
                        m_env.Helper.WriteConfig(m_globalConfig);
                        Game1.playSound("coin");
                        m_alerts.Add(new Alert(m_menuTextures, new Rectangle(80, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Zoom Setting Changed.", 1200, false, 200));
                    } else if (Game1.options.zoomLevel > 0.77) {
                        Game1.options.zoomLevel -= 0.05f;
                        Game1.overrideGameMenuReset = true;
                        Game1.game1.refreshWindowSettings();
                    }
                    return true;
                case "ZoomIn":
                    if (m_globalConfig.MenuZoomLock) {
                        if (!m_globalConfig.MenuZoomOut) {
                            return false;
                        }
                        Game1.options.zoomLevel = 1f;
                        Game1.overrideGameMenuReset = true;
                        Game1.game1.refreshWindowSettings();

                        m_globalConfig.MenuZoomOut = false;
                        m_env.Helper.WriteConfig(m_globalConfig);
                        Game1.playSound("drumkit6");
                        m_alerts.Add(new Alert(m_menuTextures, new Rectangle(80, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Zoom Setting Changed.", 1200, false, 200));
                    } else if (Game1.options.zoomLevel <1.23) {
                        Game1.options.zoomLevel += 0.05f;
                        Game1.overrideGameMenuReset = true;
                        Game1.game1.refreshWindowSettings();
                    }
                    return true;
                case "Reset":
                    m_globalConfig.HideMaleSkirts = false;
                    m_globalConfig.MenuAccessKey = SButton.C;
                    m_globalConfig.MenuZoomOut = false;
                    m_globalConfig.CanChangeGender = false;
                    m_globalConfig.ShowDresser = true;
                    m_env.Helper.WriteConfig(m_globalConfig);
                    Game1.options.zoomLevel = 1f;
                    Game1.overrideGameMenuReset = true;
                    Game1.game1.refreshWindowSettings();
                    updateLayout();
                    m_alerts.Add(new Alert(m_menuTextures, new Rectangle(160, 144, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Options Reset to Default", 1200, false, 200));
                    Game1.playSound("coin");
                    return true;
                default:
                    return false;
            }
        }
    }
}
