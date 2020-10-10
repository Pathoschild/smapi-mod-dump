/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewModdingAPI;
using Kisekae.Config;
using Kisekae.Menu;

namespace Kisekae.Framework {
    internal class MenuFavorites : TabMenu {
        /*********
        ** Properties
        *********/
        /// <summary>Core component to manipulate player appearance.</summary>
        private readonly FarmerMakeup m_farmerMakeup;

        private readonly IClickableMenu m_parent;

        /// <summary>The 'set' buttons on the 'manage favorites' submenu.</summary>
        private ClickableTextureButton[] SetFavButtons = new ClickableTextureButton[0];

        /*********
        ** Public methods
        *********/
        public MenuFavorites(IMod env, FarmerMakeup makeup, IClickableMenu parent)
            : base(env, 0, 0,
                  width: 700 + s_borderSize,
                  height: 580 + s_borderSize
            ) {
            m_farmerMakeup = makeup;
            m_parent = parent;
            updateLayout();
        }
        /// <summary>Update the menu layout for a change in the zoom level or viewport size.</summary>
        public override void updateLayout() {
            // reset window position
            this.xPositionOnScreen = (Game1.viewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.viewport.Height - this.height) / 2;
            this.yPositionOnScreen += Game1.tileSize / 2;
            
            int xOffset = this.xPositionOnScreen + IClickableMenu.borderWidth;
            int yOffset = this.yPositionOnScreen + IClickableMenu.borderWidth;

            m_alerts.Clear();
            m_components.Clear();

            // portrait
            m_components.Add(new PortraitComponent(xOffset + Game1.tileSize * 8, yOffset + +Game1.tileSize / 2));

            // labels
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset, 1, 1), "You can set up to 6 quick favorite appearance"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 25, 1, 1), "configurations for each character."));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 110, 1, 1), "Your current appearance is shown on"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 135, 1, 1), "the right, use one of the buttons below"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 160, 1, 1), "to set it as a favorite :"));

            yOffset += Game1.tileSize + Game1.tileSize * 3;
            // favorite icons
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 0, 1, 1), "1st Favorite"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 75, 1, 1), "2nd Favorite"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 150, 1, 1), "3rd Favorite"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset + Game1.tileSize * 6, yOffset + 0, 1, 1), "4th Favorite"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset + Game1.tileSize * 6, yOffset + 75, 1, 1), "5th Favorite"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset + Game1.tileSize * 6, yOffset + 150, 1, 1), "6th Favorite"));

            m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + 225, 1, 1), "Hint: Click the SET button lined up with each Favorite to"));
            m_components.Add(new LabelComponent(new Rectangle(xOffset + 60, yOffset + 250, 1, 1), "set your current appearance as that Favorite."));

            int xSize = Game1.pixelZoom * 15;
            int ySize = Game1.pixelZoom * 10;
            int zoom = 3;
            this.SetFavButtons = new[] {
                new ClickableTextureButton("QFav", new Rectangle(xOffset + Game1.tileSize * 3, yOffset, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom) { m_par = 0, m_hoverScale = 0.25f },
                new ClickableTextureButton("QFav",new Rectangle(xOffset + Game1.tileSize * 3, yOffset + 75, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom) { m_par = 1, m_hoverScale = 0.25f },
                new ClickableTextureButton("QFav",new Rectangle(xOffset + Game1.tileSize * 3, yOffset + 150, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom) { m_par = 2, m_hoverScale = 0.25f },
                new ClickableTextureButton("QFav",new Rectangle(xOffset + Game1.tileSize * 9, yOffset, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom) { m_par = 3, m_hoverScale = 0.25f },
                new ClickableTextureButton("QFav",new Rectangle(xOffset + Game1.tileSize * 9, yOffset + 75, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom) { m_par = 4, m_hoverScale = 0.25f },
                new ClickableTextureButton("QFav",new Rectangle(xOffset + Game1.tileSize * 9, yOffset + 150, xSize, ySize), Game1.mouseCursors, new Rectangle(294, 428, 21, 11), zoom) { m_par = 5, m_hoverScale = 0.25f }
            };
            for (int i = 0; i < this.SetFavButtons.Length; ++i) {
                m_components.Add(this.SetFavButtons[i]);
            }
        }
        /// <summary>Perform the action associated with a component.</summary>
        /// <param name="cpt">The component.</param>
        /// <return>whether the component action is processed</return>
        public override bool handleLeftClick(IAutoComponent cpt) {
            switch (cpt.m_name) {
                case "QFav":
                    ClickableTextureButton btn = (ClickableTextureButton)cpt;
                    m_farmerMakeup.SaveFavorite(btn.m_par + 1);
                    this.m_alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(310, 392, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "New Favorite Saved.", 1200, false));
                    Game1.playSound("purchase");
                    return true;
                default:
                    return false;
            }
        }

        public override void onSwitchBack() {
            if (m_parent is MenuFarmerMakeup mk) {
                mk.ShowFavTabArrow = false;
            }
            base.onSwitchBack();
        }
    }
}
