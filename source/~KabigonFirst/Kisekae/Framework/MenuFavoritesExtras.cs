using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using Kisekae.Config;
using Kisekae.Menu;

namespace Kisekae.Framework {
    class MenuFavoritesExtras : TabMenu {
        /*********
        ** Properties
        *********/
        /// <summary>Core component to manipulate player appearance.</summary>
        private readonly FarmerMakeup m_farmerMakeup;

        /// <summary>Labels for save and load.</summary>
        private LabelComponent m_selectFavLabel;
        private LabelComponent m_saveLabel;
        private LabelComponent m_loadLabel;
        /// <summary>The additional favorite icons.</summary>
        private readonly List<ClickableTextureButton> m_extraFavButtons = new List<ClickableTextureButton>();
        /// <summary>The 'load' button when a favorite is selected.</summary>
        private ClickableTextureButton m_loadFavButton;
        /// <summary>The 'save' button when a favorite is selected.</summary>
        private ClickableTextureButton m_saveFavButton;

        /// <summary>The current selected favorite to load or save (or <c>-1</c> if none selected).</summary>
        private int m_currentFav = -1;

        /*********
        ** Public methods
        *********/
        public MenuFavoritesExtras(IMod env, FarmerMakeup makeup)
            : base(env, 0, 0,
                  width: 700 + s_borderSize,
                  height: 580 + s_borderSize
            ) {
            m_farmerMakeup = makeup;
            updateLayout();
        }

        /// <summary>Perform the action associated with a component.</summary>
        /// <param name="cpt">The component.</param>
        /// <return>whether the component action is processed</return>
        public override bool handleLeftClick(IAutoComponent cpt) {
            switch (cpt.m_name) {
                case "Save":
                    if (m_currentFav > -1) {
                        m_farmerMakeup.SaveFavorite(m_currentFav + 1);
                        m_extraFavButtons[m_currentFav - 6].sourceRect.Y = 26;
                        m_alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(310, 392, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Favorite Saved To Slot " + (m_currentFav + 1) + " .", 1200, false));
                        Game1.playSound("purchase");
                    }
                    return true;
                case "Load":
                    if (m_currentFav > -1) {
                        if (m_farmerMakeup.LoadFavorite(m_currentFav + 1)) {
                            Game1.playSound("yoba");
                        } else {
                            m_alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(268, 470, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Uh oh! No Favorite is Set!", 1000, false));
                        }
                    }
                    return true;
                case "Fav":
                    ClickableTextureButton btn = (ClickableTextureButton)cpt;
                    int i = btn.m_par;
                    if (m_currentFav == -1) {
                        m_selectFavLabel.visible = false;
                        m_saveLabel.visible = true;
                        m_loadLabel.visible = true;
                        m_saveFavButton.visible = true;
                        m_loadFavButton.visible = true;
                    } else {
                        m_extraFavButtons[m_currentFav - 6].drawShadow = false;
                    }
                    m_extraFavButtons[i].drawShadow = true;
                    m_currentFav = i + 6;
                    m_saveLabel.label = "Currently selected: " + (m_currentFav + 1) + ".";
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>Update the menu layout for a change in the zoom level or viewport size.</summary>
        public override void updateLayout() {
            // reset window position
            this.xPositionOnScreen = (Game1.viewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.viewport.Height - this.height) / 2;
            this.yPositionOnScreen += Game1.tileSize / 2;

            Texture2D menuTextures = Game1.content.Load<Texture2D>(ContentHelper.s_MenuTextureKey);

            // reset current components ans status
            m_alerts.Clear();
            m_components.Clear();
            m_extraFavButtons.Clear();
            m_currentFav = -1;

            //constants
            const int firstExtraFavIndex = 7;
            const int nRow = 3, nCol = 10;
            const float facBtnZoom = 4.5f;
            const int extraFavTextureW = 8, extraFavTextureH = 8;
            const int xGapForExtraFav = 50, yGapForExtraFav = 50;
            const int extraFavBtnW = (int)(facBtnZoom * extraFavTextureW), extraFavBtnH = (int)(facBtnZoom * extraFavTextureH);

            int xOffset = xPositionOnScreen + IClickableMenu.borderWidth;
            int yOffset = yPositionOnScreen + IClickableMenu.borderWidth;

            // portrait
            m_components.Add(new PortraitComponent(xOffset + Game1.tileSize * 8, yOffset + +Game1.tileSize / 2));

            // hint labels
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset, 1, 1), "You can set up to 30 additional favorite appearance"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 25, 1, 1), "configurations for each character."));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 110, 1, 1), "Your current appearance is shown on"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 135, 1, 1), "the right, select a favorite below to"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 160, 1, 1), "save your appearance in it or load the"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 185, 1, 1), "appearance saved in it :"));

            xOffset = this.xPositionOnScreen + (this.width - (nCol - 1) * xGapForExtraFav - extraFavBtnW) / 2;
            yOffset += Game1.tileSize * 6;
            // save load buttons
            m_loadFavButton = new ClickableTextureButton("Load", new Rectangle(xOffset + Game1.tileSize * 6, yOffset - 100, Game1.pixelZoom * 20, Game1.pixelZoom * 10), menuTextures, new Rectangle(0, 207, 26, 11), 3f) { m_hoverScale = 0.25f };
            m_loadFavButton.visible = m_currentFav != -1;
            m_components.Add(m_loadFavButton);
            m_saveFavButton = new ClickableTextureButton("Save", new Rectangle(xOffset + Game1.tileSize * 6, yOffset - 50, Game1.pixelZoom * 20, Game1.pixelZoom * 10), menuTextures, new Rectangle(0, 193, 26, 11), 3f) { m_hoverScale = 0.25f };
            m_saveFavButton.visible = m_currentFav != -1;
            m_components.Add(m_saveFavButton);

            m_selectFavLabel = new LabelComponent(new Rectangle(xOffset, yOffset - 100, 1, 1), "Please select a favorite...");
            m_selectFavLabel.visible = m_currentFav == -1;
            m_components.Add(m_selectFavLabel);
            m_saveLabel = new LabelComponent(new Rectangle(xOffset, yOffset - 100, 1, 1), "");
            m_saveLabel.visible = m_currentFav != -1;
            m_components.Add(m_saveLabel);
            m_loadLabel = new LabelComponent(new Rectangle(xOffset, yOffset - 50, 1, 1), "Overwrite Fav. Slot");
            m_loadLabel.visible = m_currentFav != -1;
            m_components.Add(m_loadLabel);

            // extra favorites buttons
            for (int i = 0; i < nRow; i++) {
                for (int j = 0; j < nCol; j++) {
                    int y = m_farmerMakeup.m_config.HasFavSlot(i * nCol + j + firstExtraFavIndex) ? 26 : 67;
                    ClickableTextureButton c = new ClickableTextureButton("Fav", new Rectangle(xOffset + j * xGapForExtraFav, yOffset + i * yGapForExtraFav, extraFavBtnW, extraFavBtnH), menuTextures, new Rectangle(0, y, extraFavTextureW, extraFavTextureH), facBtnZoom) { m_par = j + i * nCol };
                    m_extraFavButtons.Add(c);
                    m_components.Add(c);
                }
            }
        }
    }
}
