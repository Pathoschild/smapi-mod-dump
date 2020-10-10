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
using StardewValley.Menus;
using StardewModdingAPI;
using Kisekae.Config;
using Kisekae.Menu;
using Attr = Kisekae.Config.LocalConfig.Attribute;

namespace Kisekae.Framework {
    internal class MenuCustomize : TabMenu {
        /*********
        ** Properties
        *********/
        /// <summary>The global config settings.</summary>
        private readonly GlobalConfig m_globalConfig;
        /// <summary>Core component to manipulate player appearance.</summary>
        private readonly FarmerMakeup m_farmerMakeup;
        /// <summary>Our local menu texture.</summary>
        Texture2D m_menuTextures;
        /// <summary>Parent menu.</summary>
        private readonly IClickableMenu m_parent;

        /// <summary>The color picker for the character's pants.</summary>
        private AutoColorPicker m_pantsColorPicker;
        /// <summary>The color picker for the character's hair.</summary>
        private AutoColorPicker m_hairColorPicker;
        /// <summary>The color picker for the character's eyes.</summary>
        private AutoColorPicker m_eyeColorPicker;
        /// <summary>The labels for arrow selectors, which also show the currently selected value.</summary>
        private readonly Dictionary<string, LabelComponent> m_selectValueLabels = new Dictionary<string, LabelComponent>();
        /// <summary>The outline around the male option button when it's selected.</summary>
        private ClickableTextureButton m_maleOutlineButton;
        /// <summary>The outline around the female option button when it's selected.</summary>
        private ClickableTextureButton m_femaleOutlineButton;
        /// <summary>The 'quick load favorite' buttons.</summary>
        private ClickableTextureButton[] m_quickLoadFavButtons = new ClickableTextureButton[0];

        /// <summary>The last color picker the player interacted with.</summary>
        private AutoColorPicker m_lastHeldColorPicker;
        /// <summary>The delay until the the character preview should be updated with the last colour picker change.</summary>
        private int m_colorPickerTimer;
        /// <summary>The tooltip text to draw next to the cursor.</summary>
        private string m_hoverText = "";
        /// <summary>How many times the player pressed the 'random' buttom since the menu was opened.</summary>
        private int m_timesRandomised;
        /*********
        ** Public methods
        *********/
        public MenuCustomize(IMod env, GlobalConfig globalConfig, LocalConfig playerConfig, FarmerMakeup farmerMakeup, IClickableMenu parent = null)
            : base(env, 0, 0,
                  width: 700 + s_borderSize,
                  height: 580 + s_borderSize
            ) {
            m_drawCursor = false;
            m_globalConfig = globalConfig;
            m_farmerMakeup = farmerMakeup;
            m_parent = parent;
            exitFunction = exit;
            m_menuTextures = Game1.content.Load<Texture2D>(ContentHelper.s_MenuTextureKey);

            updateLayout();
        }
        /// <summary>The method invoked while the player is holding down the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void leftClickHeld(int x, int y) {
            // update color pickers
            m_colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (m_lastHeldColorPicker == null || m_colorPickerTimer > 0) {
                return;
            }
            if (m_lastHeldColorPicker.m_name.Equals("HairColorPicker")) {
                m_farmerMakeup.ChangeHairColor(m_lastHeldColorPicker.clickHeld(x, y));
            } else if (m_lastHeldColorPicker.m_name.Equals("PantsColorPicker")) {
                m_farmerMakeup.ChangeBottomsColor(m_lastHeldColorPicker.clickHeld(x, y));
            } else if (m_lastHeldColorPicker.m_name.Equals("EyeColorPicker")) {
                m_farmerMakeup.ChangeEyeColor(m_lastHeldColorPicker.clickHeld(x, y));
            }
            m_colorPickerTimer = 100;
        }
        /// <summary>The method invoked when the player releases the left mouse button.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void releaseLeftClick(int x, int y) {
            // update color pickers
            if (m_lastHeldColorPicker != null) {
                m_lastHeldColorPicker.releaseClick();
            }
            m_lastHeldColorPicker = null;
            return;
        }
        /// <summary>The method invoked when the cursor is over a given position.</summary>
        /// <param name="x">The X mouse position.</param>
        /// <param name="y">The Y mouse position.</param>
        public override void performHoverAction(int x, int y) {
            base.performHoverAction(x, y);
            // hover text
            m_hoverText = "";
            for (int i = 0; i < this.m_quickLoadFavButtons.Length; i++) {
                if (this.m_quickLoadFavButtons[i].containsPoint(x, y)) {
                    m_hoverText = m_farmerMakeup.m_config.HasFavSlot(i + 1) ? "" : "No Favorite Is Set";
                    break;
                }
            }
        }
        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch to which to draw.</param>
        public override void draw(SpriteBatch spriteBatch) {
            base.draw(spriteBatch);

            // cursor
            if (m_hoverText.Equals("No Favorite Is Set")) {
                spriteBatch.Draw(m_menuTextures, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 6, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
                IClickableMenu.drawHoverText(spriteBatch, m_hoverText, Game1.smallFont, 20, 20);
            } else {
                spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }
        }
        /// <summary>Update the menu layout for a change in the zoom level or viewport size.</summary>
        public override void updateLayout() {
            // reset window position
            this.xPositionOnScreen = (Game1.viewport.Width - this.width) / 2;
            this.yPositionOnScreen = (Game1.viewport.Height - this.height) / 2;
            this.yPositionOnScreen += Game1.tileSize / 2;

            int xBase = xPositionOnScreen + IClickableMenu.borderWidth;
            int yBase = yPositionOnScreen + IClickableMenu.borderWidth;

            m_alerts.Clear();
            m_components.Clear();
            m_selectValueLabels.Clear();

            // random button
            m_components.Add(new ClickableTextureButton("Random", new Rectangle(xBase, yBase, Game1.pixelZoom * 10, Game1.pixelZoom * 10), null, "Random", Game1.mouseCursors, new Rectangle(381, 361, 10, 10), Game1.pixelZoom) { m_hoverScale = 0.25f });
            // portrait
            m_components.Add(new PortraitComponent(xBase + Game1.tileSize * 3 / 4, yBase));
            // direction buttons
            int xOffset = xBase + Game1.tileSize / 4;
            int yOffset = yBase + Game1.tileSize / 4 + Game1.tileSize * 2;
            m_components.Add(new ClickableTextureButton("Direction", new Rectangle(xOffset, yOffset, Game1.tileSize, Game1.tileSize), "", "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f) { m_par = -1, m_hoverScale = 0.1f });
            m_components.Add(new ClickableTextureButton("Direction", new Rectangle(xOffset + Game1.tileSize * 2, yOffset, Game1.tileSize, Game1.tileSize), "", "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f) { m_par = 1, m_hoverScale = 0.1f });
            // gender buttons
            if (m_globalConfig.CanChangeGender) {
                int scale = Game1.pixelZoom / 2;
                m_components.Add(new ClickableTextureButton("Male", new Rectangle(xBase + Game1.tileSize * 3, yBase, Game1.tileSize, Game1.tileSize), null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), scale));
                m_components.Add(new ClickableTextureButton("Female", new Rectangle(xBase + Game1.tileSize * 3, yBase + Game1.tileSize, Game1.tileSize, Game1.tileSize), null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), scale));
                this.m_maleOutlineButton = new ClickableTextureButton("", new Rectangle(
                    xBase + Game1.tileSize * 3 - 3,
                    yBase - 3,
                    Game1.tileSize, Game1.tileSize), "", "", m_menuTextures, new Rectangle(19, 38, 19, 19), scale) {
                        m_visible = Game1.player.isMale
                    };
                this.m_femaleOutlineButton = new ClickableTextureButton("", new Rectangle(
                    xBase + Game1.tileSize * 3 - 3,
                    yBase + Game1.tileSize - 3,
                    Game1.tileSize, Game1.tileSize), "", "", m_menuTextures, new Rectangle(19, 38, 19, 19), scale) {
                        m_visible = !Game1.player.isMale
                    };
                m_components.Add(m_maleOutlineButton);
                m_components.Add(m_femaleOutlineButton);
            }
            // color pickers
            {
                // eye color
                xOffset = xBase + Game1.tileSize * 4 + Game1.tileSize / 4;
                yOffset = yBase;
                int xPickerOffset = Game1.tileSize * 3 - Game1.tileSize / 4;
                int yLabelOffset = 16;
                m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + yLabelOffset, 1, 1), "Eye Color:"));
                this.m_eyeColorPicker = new AutoColorPicker("EyeColorPicker", xOffset + xPickerOffset, yOffset);
                this.m_eyeColorPicker.setColor(Game1.player.newEyeColor);
                m_components.Add(m_eyeColorPicker);
                // hair color
                yOffset += Game1.tileSize + 8;
                m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + yLabelOffset, 1, 1), "Hair Color:"));
                this.m_hairColorPicker = new AutoColorPicker("HairColorPicker", xOffset + xPickerOffset, yOffset);
                this.m_hairColorPicker.setColor(Game1.player.hairstyleColor);
                m_components.Add(m_hairColorPicker);
                // pants color
                yOffset += Game1.tileSize + 8;
                m_components.Add(new LabelComponent(new Rectangle(xOffset, yOffset + yLabelOffset, 1, 1), "Pants Color:"));
                this.m_pantsColorPicker = new AutoColorPicker("PantsColorPicker", xOffset + xPickerOffset, yOffset);
                this.m_pantsColorPicker.setColor(Game1.player.pantsColor);
                m_components.Add(m_pantsColorPicker);
            }
            // type selectors
            {
                xOffset = xBase + Game1.tileSize / 4;
                yOffset = yBase + Game1.tileSize * 3 + Game1.tileSize / 2;
                int xGap = xBase + Game1.tileSize * 4 - Game1.tileSize / 4;
                int[] xSelectorOffset = new int[] { Game1.tileSize / 4, Game1.tileSize, Game1.tileSize * 2 };
                int[] xSelectorRightOffset = new int[] { Game1.tileSize / 4, Game1.tileSize + 8, Game1.tileSize * 3 };
                selectorLayout(xOffset, xSelectorOffset, yOffset, Attr.Hair);
                m_components.Add(new ClickableTextureButton("MultiplayerFix", new Rectangle(
                    xGap + xSelectorRightOffset[0] + 5,
                    yOffset + Game1.tileSize / 4,
                    36, 36), null, null, Game1.mouseCursors, new Rectangle(m_farmerMakeup.m_config.MutiplayerFix ? 236 : 227, 425, 9, 9), 4f, false));
                m_components.Add(new LabelComponent(new Rectangle(xGap + xSelectorRightOffset[1], yOffset + Game1.tileSize / 4, 1, 1), "Multiplayer Fix"));

                yOffset += Game1.tileSize + 4;
                selectorLayout(xOffset, xSelectorOffset, yOffset, Attr.Skin);
                selectorLayout(xGap, xSelectorRightOffset, yOffset, Attr.Face);
                yOffset += Game1.tileSize + 4;
                selectorLayout(xOffset, xSelectorOffset, yOffset, Attr.Shirt);
                selectorLayout(xGap, xSelectorRightOffset, yOffset, Attr.Nose);
                yOffset += Game1.tileSize + 4;
                selectorLayout(xOffset, xSelectorOffset, yOffset, Attr.ShoeColor);
                selectorLayout(xGap, xSelectorRightOffset, yOffset, Attr.Bottoms);
                yOffset += Game1.tileSize + 4;
                selectorLayout(xOffset, xSelectorOffset, yOffset, Attr.Accessory);
                selectorLayout(xGap, xSelectorRightOffset, yOffset, Attr.Shoes);
            }
            // quick favorite star buttons
            {
                xOffset = xBase + Game1.tileSize * 8 + Game1.tileSize / 2;
                yOffset = yBase + Game1.tileSize * 3 + Game1.tileSize / 2;
                // text above quick favorite buttons
                m_components.Add(new LabelComponent(new Rectangle(
                    xOffset + 10,
                    yOffset,
                    1, 1), "Load"));
                m_components.Add(new LabelComponent(new Rectangle(
                    xOffset - Game1.tileSize / 4 + 5,
                    yOffset + 25,
                    1, 1), "Favorite"));

                yOffset += 75;
                int size = Game1.pixelZoom * 10;
                int zoom = Game1.pixelZoom;
                int y1 = m_farmerMakeup.m_config.HasFavSlot(1) ? 26 : 67;
                int y2 = m_farmerMakeup.m_config.HasFavSlot(2) ? 26 : 67;
                int y3 = m_farmerMakeup.m_config.HasFavSlot(3) ? 26 : 67;
                int y4 = m_farmerMakeup.m_config.HasFavSlot(4) ? 26 : 67;
                int y5 = m_farmerMakeup.m_config.HasFavSlot(5) ? 26 : 67;
                int y6 = m_farmerMakeup.m_config.HasFavSlot(6) ? 26 : 67;
                this.m_quickLoadFavButtons = new[] {
                    new ClickableTextureButton("Fav", new Rectangle(xOffset +  0, yOffset +   0, size, size), m_menuTextures, new Rectangle(24, y1, 8, 8), zoom) { m_par = 0, m_hoverScale = 0.2f },
                    new ClickableTextureButton("Fav", new Rectangle(xOffset +  0, yOffset + Game1.tileSize + 4, size, size), m_menuTextures, new Rectangle(8, y2, 8, 8), zoom) { m_par = 1, m_hoverScale = 0.2f },
                    new ClickableTextureButton("Fav", new Rectangle(xOffset +  0, yOffset + (Game1.tileSize + 4)*2, size, size), m_menuTextures, new Rectangle(0, y3, 8, 8), zoom) { m_par = 2, m_hoverScale = 0.2f },
                    new ClickableTextureButton("Fav", new Rectangle(xOffset + 45, yOffset +   0, size, size), m_menuTextures, new Rectangle(24, y4, 8, 8), zoom) { m_par = 3, m_hoverScale = 0.2f },
                    new ClickableTextureButton("Fav", new Rectangle(xOffset + 45, yOffset +  Game1.tileSize + 4, size, size), m_menuTextures, new Rectangle(8, y5, 8, 8), zoom) { m_par = 4, m_hoverScale = 0.2f },
                    new ClickableTextureButton("Fav", new Rectangle(xOffset + 45, yOffset + (Game1.tileSize + 4)*2, size, size), m_menuTextures, new Rectangle(0, y6, 8, 8), zoom) { m_par = 5, m_hoverScale = 0.2f }
                };
                for (int i = 0; i < m_quickLoadFavButtons.Length; ++i) {
                    m_components.Add(m_quickLoadFavButtons[i]);
                }
                m_components.Add(new ClickableTextureButton("OK", new Rectangle(xOffset + 8, yOffset + (Game1.tileSize + 4) * 3 - Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f) { m_hoverScale = 0.1f });
            }
        }
        /// <summary>Perform the action associated with a component.</summary>
        /// <param name="cpt">The component.</param>
        /// <return>whether the component action is processed</return>
        public override bool handleLeftClick(IAutoComponent cpt) {
            AutoColorPicker p = null;
            ClickableTextureButton btn = null;
            if (cpt is ClickableTextureButton) {
                btn = (ClickableTextureButton)cpt;
            } else if (cpt is AutoColorPicker) {
                p = (AutoColorPicker)cpt;
            }
            switch (cpt.m_name) {
                case "Male":
                    m_farmerMakeup.ChangeGender(true);
                    m_farmerMakeup.ChangeHairStyle(0);
                    m_maleOutlineButton.m_visible = true;
                    m_femaleOutlineButton.m_visible = false;
                    refreshLabelAttrs();
                    break;
                case "Female":
                    m_farmerMakeup.ChangeGender(false);
                    m_farmerMakeup.ChangeHairStyle(16);
                    m_maleOutlineButton.m_visible = false;
                    m_femaleOutlineButton.m_visible = true;
                    refreshLabelAttrs();
                    break;
                case "Direction":
                    Game1.player.faceDirection((Game1.player.facingDirection - btn.m_par + 4) % 4);
                    Game1.player.FarmerSprite.StopAnimation();
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    btn.scale = Math.Max(0.75f, btn.scale - 0.25f);
                    return true;
                case "OK":
                    if (m_parent != null) {
                        m_parent.exitThisMenuNoSound();
                    } else {
                        exitThisMenuNoSound();
                    }
                    break;
                case "Random":
                    this.randomiseCharacter();
                    btn.scale = Game1.pixelZoom - 0.5f;
                    this.m_eyeColorPicker.setColor(Game1.player.newEyeColor);
                    this.m_hairColorPicker.setColor(Game1.player.hairstyleColor);
                    this.m_pantsColorPicker.setColor(Game1.player.pantsColor);
                    refreshLabelAttrs();
                    return true;
                case "MultiplayerFix":
                    m_farmerMakeup.TougleMultiplayerFix();
                    btn.sourceRect.X = btn.sourceRect.X == 227 ? 236 : 227;
                    Game1.playSound("drumkit6");
                    return true;
                case "EyeColorPicker":
                    m_farmerMakeup.ChangeEyeColor(p.click(Game1.getOldMouseX(), Game1.getOldMouseY()));
                    m_lastHeldColorPicker = p;
                    return true;
                case "HairColorPicker":
                    m_farmerMakeup.ChangeHairColor(p.click(Game1.getOldMouseX(), Game1.getOldMouseY()));
                    m_lastHeldColorPicker = p;
                    return true;
                case "PantsColorPicker":
                    m_farmerMakeup.ChangeBottomsColor(p.click(Game1.getOldMouseX(), Game1.getOldMouseY()));
                    m_lastHeldColorPicker = p;
                    return true;
                case "Fav":
                    int i = btn.m_par;
                    if (m_farmerMakeup.LoadFavorite(i + 1)) {
                        Game1.playSound("yoba");
                        this.m_eyeColorPicker.setColor(Game1.player.newEyeColor);
                        this.m_hairColorPicker.setColor(Game1.player.hairstyleColor);
                        this.m_pantsColorPicker.setColor(Game1.player.pantsColor);
                        refreshLabelAttrs();
                    } else {
                        m_alerts.Add(new Alert(Game1.mouseCursors, new Rectangle(268, 470, 16, 16), Game1.viewport.Width / 2 - (700 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (500 + IClickableMenu.borderWidth * 2) / 2, "Uh oh! No Favorite is Set!", 1000, false));
                        if (m_parent is MenuFarmerMakeup mk) {
                            mk.ShowFavTabArrow = true;
                        }
                    }
                    return true;
                default:
                    if (btn != null) {
                        btn.scale = Math.Max(0.75f, btn.scale - 2.5f);
                        return this.handleSelectorChange(btn.name, btn.m_par);
                    }
                    return false;
            }
            Game1.playSound("coin");
            return true;
        }
        /*********
        ** Private methods
        *********/
        /// <summary>construct an farmer attribute selector.</summary>
        private void selectorLayout(int xBase, int[] selectorOffset, int y, Attr attr) {
            selectorLayout(xBase, selectorOffset, y, attr.ToString(), getShownText(attr), getValueText(attr));
        }
        /// <summary>construct a ganeral value selector.</summary>
        private void selectorLayout(int xBase, int[] selectorOffset, int y, string name, string label, string value) {
            ClickableTextureButton b;
            b =  new ClickableTextureButton(name, new Rectangle(xBase + selectorOffset[0], y, Game1.tileSize, Game1.tileSize), "", "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 0.75f) { m_par = -1, m_hoverScale = 0.1f };
            m_components.Add(b);
            LabelComponent l;
            l = new LabelComponent(new Rectangle(xBase + selectorOffset[1], y, selectorOffset[2] - selectorOffset[1], 1), label) { m_baseX = xBase + selectorOffset[1] };
            l.centerLabelX();
            m_components.Add(l);
            l = new LabelComponent(new Rectangle(xBase + selectorOffset[1], y + Game1.tileSize / 2, selectorOffset[2] - selectorOffset[1], 1), value) { m_baseX = xBase + selectorOffset[1] };
            if (value.Length > 3) {
                l.centerLabelX();
            } else {
                l.rightCenterLabelX((int)Game1.smallFont.MeasureString("0").X);
            }
            m_selectValueLabels[name] = l;
            m_components.Add(l);
            b = new ClickableTextureButton(name, new Rectangle(xBase + selectorOffset[2], y, Game1.tileSize, Game1.tileSize), "", "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 0.75f) { m_par = 1, m_hoverScale = 0.1f };
            m_components.Add(b);
        }
        /// <summary>get display name text for attribute.</summary>
        private string getShownText(LocalConfig.Attribute attribute) {
            switch (attribute) {
                case LocalConfig.Attribute.Hair:
                case LocalConfig.Attribute.Skin:
                case LocalConfig.Attribute.Shirt:
                case LocalConfig.Attribute.Bottoms:
                    return attribute.ToString();
                case LocalConfig.Attribute.Face:
                    return "Face Type";
                case LocalConfig.Attribute.Nose:
                    return "Nose Type";
                case LocalConfig.Attribute.ShoeColor:
                    return "Shoes";
                case LocalConfig.Attribute.Accessory:
                    return "Acc.";
                case LocalConfig.Attribute.Shoes:
                    return "Shoe Type";
                default:
                    return "";
            }
        }
        /// <summary>get display value text for attribute.</summary>
        private string getValueText(LocalConfig.Attribute attribute) {
            switch (attribute) {
                case LocalConfig.Attribute.Hair:
                    return string.Concat(m_farmerMakeup.m_config.ChosenHairstyle[0] + 1);
                case LocalConfig.Attribute.Skin:
                    return string.Concat(m_farmerMakeup.m_config.ChosenSkin[0] + 1);
                case LocalConfig.Attribute.Shirt:
                    return string.Concat(m_farmerMakeup.m_config.ChosenShirt[0] + 1);
                case LocalConfig.Attribute.Bottoms:
                    return string.Concat(m_farmerMakeup.m_config.ChosenBottoms[0] + 1);
                case LocalConfig.Attribute.Face:
                    return string.Concat(m_farmerMakeup.m_config.ChosenFace[0] + 1);
                case LocalConfig.Attribute.Nose:
                    return string.Concat(m_farmerMakeup.m_config.ChosenNose[0] + 1);
                case LocalConfig.Attribute.ShoeColor:
                    if (m_farmerMakeup.m_config.ChosenShoeColor[0] == -1) {
                        return "Boot";
                    } else {
                        return string.Concat(m_farmerMakeup.m_config.ChosenShoeColor[0] + 1);
                    }
                case LocalConfig.Attribute.Accessory:
                    return string.Concat(m_farmerMakeup.AccIndexToLogic(m_farmerMakeup.m_config.ChosenAccessory[0]));
                case LocalConfig.Attribute.Shoes:
                    return string.Concat(m_farmerMakeup.m_config.ChosenShoes[0] + 1);
                default:
                    return "";
            }
        }
        /// <summary>refresh all labeled attribute values.</summary>
        private void refreshLabelAttrs() {
            foreach (KeyValuePair<string, LabelComponent> p in m_selectValueLabels) {
                Attr a;
                if (!Enum.TryParse<Attr>(p.Key, true, out a)) {
                    continue;
                }
                p.Value.label = getValueText(a);
                if (p.Value.label.Length > 3) {
                    p.Value.centerLabelX();
                } else {
                    p.Value.rightCenterLabelX((int)Game1.smallFont.MeasureString("0").X);
                }
            }
        }
        /// <summary>Randomise the character attributes.</summary>
        private void randomiseCharacter() {
            // play sound
            string cueName = "drumkit6";
            string[] sounds = new string[] { "drumkit1", "dirtyHit" , "axchop", "hoeHit", "fishSlap", "drumkit6" , "drumkit5", "drumkit6", "junimoMeep1", "coin", "axe", "hammer", "drumkit2", "drumkit4", "drumkit3" };
            if (this.m_timesRandomised > 0) {
                cueName = sounds[Game1.random.Next(15)];
            }
            Game1.playSound(cueName);
            this.m_timesRandomised = 1;

            m_farmerMakeup.Randomize();
        }
        /// <summary>Perform the action associated with a selector.</summary>
        /// <param name="name">The selector name.</param>
        /// <param name="change">The change value.</param>
        /// <return>whether the selector action is processed</return>
        private bool handleSelectorChange(string name, int change) {
            Attr a;
            if (!Enum.TryParse<Attr>(name, true, out a)) {
                return false;
            }
            switch (a) {
                case Attr.Skin:
                    m_farmerMakeup.ChangeSkinColor(m_farmerMakeup.m_config.ChosenSkin[0] + change);
                    Game1.playSound("skeletonStep");
                    break;
                case Attr.Hair:
                    m_farmerMakeup.ChangeHairStyle(m_farmerMakeup.m_config.ChosenHairstyle[0] + change);
                    Game1.playSound("grassyStep");
                    break;
                case Attr.Shirt:
                    m_farmerMakeup.ChangeShirt(m_farmerMakeup.m_config.ChosenShirt[0] + change);
                    Game1.playSound("coin");
                    break;
                case Attr.Accessory:
                    m_farmerMakeup.ChangeAccessory(m_farmerMakeup.AccIndexToLogic(m_farmerMakeup.m_config.ChosenAccessory[0]) + change);
                    Game1.playSound("purchase");
                    break;
                case Attr.Face:
                    m_farmerMakeup.ChangeFace(m_farmerMakeup.m_config.ChosenFace[0] + change);
                    Game1.playSound("skeletonStep");
                    break;
                case Attr.Nose:
                    m_farmerMakeup.ChangeNose(m_farmerMakeup.m_config.ChosenNose[0] + change);
                    Game1.playSound("grassyStep");
                    break;
                case Attr.Bottoms:
                    m_farmerMakeup.ChangeBottoms(m_farmerMakeup.m_config.ChosenBottoms[0] + change);
                    Game1.playSound("coin");
                    break;
                case Attr.Shoes:
                    m_farmerMakeup.ChangeShoes(m_farmerMakeup.m_config.ChosenShoes[0] + change);
                    Game1.playSound("purchase");
                    break;
                case Attr.ShoeColor:
                    m_farmerMakeup.ChangeShoeColor(m_farmerMakeup.m_config.ChosenShoeColor[0] + change);
                    Game1.playSound("axe");
                    break;
                default:
                    return false;
            }
            m_selectValueLabels[name].label = getValueText(a);
            if (m_selectValueLabels[name].label.Length > 3) {
                m_selectValueLabels[name].centerLabelX();
            } else {
                m_selectValueLabels[name].rightCenterLabelX((int)Game1.smallFont.MeasureString("0").X);
            }
            return true;
        }
        /// <summary>Exit the menu.</summary>
        private void exit() {
            m_farmerMakeup.m_config.save();
        }
    }
}
