/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Patches;
using AlternativeTextures.Framework.Patches.Buildings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static AlternativeTextures.Framework.Models.AlternativeTextureModel;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.UI
{
    internal class SprayCanMenu : PaintBucketMenu
    {
        public ClickableTextureComponent radiusSubtractButton;
        public ClickableTextureComponent radiusAddButton;
        public ClickableComponent allButton;
        public ClickableComponent noneButton;

        private int _tileRadius;
        private Vector2 _sideBarPosition;

        public SprayCanMenu(Object target, Vector2 position, TextureType textureType, string modelName, string uiTitle = "Spray Can", int textureTileWidth = -1) : base(target, position, textureType, modelName, uiTitle, textureTileWidth, isSprayCan: true)
        {
            if (!target.modData.ContainsKey("AlternativeTextureOwner") || !target.modData.ContainsKey("AlternativeTextureName"))
            {
                this.exitThisMenu();
                return;
            }
            _title = uiTitle;
            _sideBarPosition = new Vector2(base.xPositionOnScreen + width - IClickableMenu.spaceToClearSideBorder + 12, base.yPositionOnScreen + 24);

            _tileRadius = 1;
            if (Game1.player.modData.ContainsKey(AlternativeTextures.SPRAY_CAN_RADIUS) is false || int.TryParse(Game1.player.modData[AlternativeTextures.SPRAY_CAN_RADIUS], out _tileRadius) is false)
            {
                Game1.player.modData[AlternativeTextures.SPRAY_CAN_RADIUS] = "1";
            }

            this.radiusSubtractButton = new ClickableTextureComponent(new Rectangle((int)(_sideBarPosition.X + 16), (int)(_sideBarPosition.Y + 64), 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 3f)
            {
                myID = 8998,
                rightNeighborID = 0
            };
            this.radiusAddButton = new ClickableTextureComponent(new Rectangle((int)(_sideBarPosition.X + 96), (int)(_sideBarPosition.Y + 64), 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 3f)
            {
                myID = 8997
            };

            this.allButton = new ClickableComponent(new Rectangle((int)_sideBarPosition.X + 23, (int)radiusAddButton.bounds.Y + 125, 100, 45), "allButton")
            {
                myID = 8991
            };
            this.noneButton = new ClickableComponent(new Rectangle((int)_sideBarPosition.X + 23, (int)radiusAddButton.bounds.Y + 175, 100, 45), "noneButton")
            {
                myID = 8992
            };
        }

        private void SetEnabledTexture(ModDataDictionary modData, bool? forceEnable = null)
        {
            if (modData.ContainsKey("AlternativeTextureName") && modData.ContainsKey("AlternativeTextureOwner") && modData.ContainsKey("AlternativeTextureVariation") && Int32.TryParse(modData["AlternativeTextureVariation"], out int variation))
            {
                SetEnabledTexture(modData["AlternativeTextureName"], modData["AlternativeTextureOwner"], variation, forceEnable);
            }
        }

        private void SetEnabledTexture(string textureName, string owner, int variation, bool? forceEnable = null)
        {
            if (_selectedIdsToModels.ContainsKey(textureName))
            {
                if (_selectedIdsToModels[textureName].Variations.Contains(variation))
                {
                    if (forceEnable is not true)
                    {
                        _selectedIdsToModels[textureName].Variations.Remove(variation);

                        if (_selectedIdsToModels[textureName].Variations.Count == 0)
                        {
                            _selectedIdsToModels.Remove(textureName);
                        }
                    }
                }
                else if (forceEnable is not false)
                {
                    _selectedIdsToModels[textureName].Variations.Add(variation);
                }
            }
            else if (forceEnable is not false)
            {
                _selectedIdsToModels[textureName] = new SelectedTextureModel()
                {
                    Owner = owner,
                    TextureName = textureName,
                    Variations = new List<int>() { variation }
                };
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Escape && base.readyToClose())
            {
                Game1.player.modData[AlternativeTextures.ENABLED_SPRAY_CAN_TEXTURES] = JsonConvert.SerializeObject(_selectedIdsToModels);
                Game1.player.modData[AlternativeTextures.SPRAY_CAN_RADIUS] = _tileRadius.ToString();

                base.exitThisMenu();
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = false)
        {
            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            if (radiusSubtractButton.containsPoint(x, y))
            {
                _tileRadius -= 1;
                if (_tileRadius < 1)
                {
                    _tileRadius = 1;
                }
                return;
            }
            else if (radiusAddButton.containsPoint(x, y))
            {
                _tileRadius += 1;
                if (_tileRadius > 5)
                {
                    _tileRadius = 5;
                }
                return;
            }
            else if (allButton.containsPoint(x, y))
            {
                foreach (Item item in this.cachedTextureOptions)
                {
                    SetEnabledTexture(item.modData, true);
                }
                return;
            }
            else if (noneButton.containsPoint(x, y))
            {
                foreach (Item item in this.cachedTextureOptions)
                {
                    SetEnabledTexture(item.modData, false);
                }
                return;
            }

            if (_startingRow > 0 && this.backButton.containsPoint(x, y))
            {
                _startingRow--;
                Game1.playSound("shiny4");
                return;
            }
            if ((_maxRows + _startingRow) * _texturesPerRow < this.filteredTextureOptions.Count && this.forwardButton.containsPoint(x, y))
            {
                _startingRow++;
                Game1.playSound("shiny4");
                return;
            }

            foreach (ClickableTextureComponent c in this.availableTextures)
            {
                if (c.containsPoint(x, y) && c.item != null)
                {
                    SetEnabledTexture(c.item.modData);
                }
            }

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), (int)_sideBarPosition.X, (int)_sideBarPosition.Y, 150, 450, Color.White, 4f, drawShadow: false);
            Utility.drawTextWithShadow(b, "Radius", Game1.smallFont, new Vector2(_sideBarPosition.X + 35, _sideBarPosition.Y + 32), Game1.textColor);

            radiusSubtractButton.draw(b);
            Utility.drawTextWithShadow(b, _tileRadius.ToString(), Game1.smallFont, new Vector2(radiusSubtractButton.bounds.X + 48, radiusSubtractButton.bounds.Y), Game1.textColor);
            radiusAddButton.draw(b);

            Utility.drawTextWithShadow(b, "  ---\nQuick\nSelect", Game1.smallFont, new Vector2(_sideBarPosition.X + 38, (int)radiusAddButton.bounds.Y + 30), Game1.textColor);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), allButton.bounds.X, allButton.bounds.Y, allButton.bounds.Width, allButton.bounds.Height, Color.White, 4f, drawShadow: false);
            Utility.drawTextWithShadow(b, "All", Game1.smallFont, new Vector2(allButton.bounds.X + 35, allButton.bounds.Y + 5), Game1.textColor);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), noneButton.bounds.X, noneButton.bounds.Y, noneButton.bounds.Width, noneButton.bounds.Height, Color.White, 4f, drawShadow: false);
            Utility.drawTextWithShadow(b, "None", Game1.smallFont, new Vector2(noneButton.bounds.X + 25, noneButton.bounds.Y + 7), Game1.textColor);

            var enabledTextureSum = _selectedIdsToModels.Sum(t => t.Value.Variations.Count).ToString();
            var numberOfDigitsInSum = enabledTextureSum.Length;
            Utility.drawTextWithShadow(b, "  ---", Game1.smallFont, new Vector2(_sideBarPosition.X + 38, (int)noneButton.bounds.Y + 60), Game1.textColor);
            Utility.drawTextWithShadow(b, "Selected\nTextures", Game1.smallFont, new Vector2(_sideBarPosition.X + 25, (int)noneButton.bounds.Y + 85), Game1.textColor);
            Utility.drawTextWithShadow(b, enabledTextureSum, Game1.smallFont, new Vector2(_sideBarPosition.X + 65 - (enabledTextureSum.Length >= 2 ? 5 * (enabledTextureSum.Length - 1) : 0), (int)noneButton.bounds.Y + 145), Game1.textColor);

            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }
    }
}