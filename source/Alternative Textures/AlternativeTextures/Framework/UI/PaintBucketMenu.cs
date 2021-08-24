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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.UI
{
    internal class PaintBucketMenu : IClickableMenu
    {
        public ClickableTextureComponent hovered;
        public ClickableTextureComponent forwardButton;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent queryButton;

        public List<Item> filteredTextureOptions = new List<Item>();
        public List<Item> cachedTextureOptions = new List<Item>();
        public List<ClickableTextureComponent> availableTextures = new List<ClickableTextureComponent>();

        // Textbox
        private TextBox _searchBox;
        private ClickableComponent _searchBoxCC;

        private string _cachedTextBoxValue;

        private int _startingRow = 0;
        private int _texturesPerRow = 6;
        private int _maxRows = 4;

        private Object _textureTarget;

        public PaintBucketMenu(Object target, string modelName, bool isFlooring = false) : base(0, 0, 832, 576, showUpperRightCloseButton: true)
        {
            if (!target.modData.ContainsKey("AlternativeTextureOwner") || !target.modData.ContainsKey("AlternativeTextureName"))
            {
                this.exitThisMenu();
                return;
            }

            // Set up menu structure
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                base.height += 64;
            }

            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y + 32;

            // Populate the texture selection components
            var availableModels = AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation));
            for (int m = 0; m < availableModels.Count; m++)
            {
                var manualVariations = availableModels[m].ManualVariations.Where(v => v.Id != -1).ToList();
                if (manualVariations.Count() > 0)
                {
                    for (int v = 0; v < manualVariations.Count(); v++)
                    {
                        var objectWithVariation = target.getOne();
                        objectWithVariation.modData["AlternativeTextureOwner"] = availableModels[m].Owner;
                        objectWithVariation.modData["AlternativeTextureName"] = availableModels[m].GetId();
                        objectWithVariation.modData["AlternativeTextureVariation"] = manualVariations[v].Id.ToString();
                        objectWithVariation.modData["AlternativeTextureSeason"] = availableModels[m].Season;

                        if (target is Furniture furniture)
                        {
                            (objectWithVariation as Furniture).currentRotation.Value = furniture.currentRotation;
                            (objectWithVariation as Furniture).updateRotation();
                        }

                        this.filteredTextureOptions.Add(objectWithVariation);
                        this.cachedTextureOptions.Add(objectWithVariation);
                    }
                }
                else
                {
                    for (int v = 0; v < availableModels[m].Variations; v++)
                    {
                        var objectWithVariation = target.getOne();
                        objectWithVariation.modData["AlternativeTextureOwner"] = availableModels[m].Owner;
                        objectWithVariation.modData["AlternativeTextureName"] = availableModels[m].GetId();
                        objectWithVariation.modData["AlternativeTextureVariation"] = v.ToString();
                        objectWithVariation.modData["AlternativeTextureSeason"] = availableModels[m].Season;

                        if (target is Furniture furniture)
                        {
                            (objectWithVariation as Furniture).currentRotation.Value = furniture.currentRotation;
                            (objectWithVariation as Furniture).updateRotation();
                        }

                        this.filteredTextureOptions.Add(objectWithVariation);
                        this.cachedTextureOptions.Add(objectWithVariation);
                    }
                }
            }

            // Add the vanilla version
            var vanillaObject = target.getOne();
            vanillaObject.modData["AlternativeTextureOwner"] = AlternativeTextures.DEFAULT_OWNER;
            vanillaObject.modData["AlternativeTextureName"] = $"{vanillaObject.modData["AlternativeTextureOwner"]}.{modelName}";
            vanillaObject.modData["AlternativeTextureVariation"] = $"{-1}";
            vanillaObject.modData["AlternativeTextureSeason"] = String.Empty;

            if (target is Furniture)
            {
                (vanillaObject as Furniture).currentRotation.Value = (target as Furniture).currentRotation;
                (vanillaObject as Furniture).updateRotation();
            }

            this.filteredTextureOptions.Insert(0, vanillaObject);
            this.cachedTextureOptions.Insert(0, vanillaObject);

            var sourceRect = isFlooring ? new Rectangle(0, 0, 16, 32) : GetSourceRectangle(target, availableModels.First().TextureWidth, availableModels.First().TextureHeight, -1);
            for (int r = 0; r < _maxRows; r++)
            {
                for (int c = 0; c < _texturesPerRow; c++)
                {
                    var componentId = c + r * _texturesPerRow;
                    this.availableTextures.Add(new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + IClickableMenu.borderWidth + componentId % _texturesPerRow * 64 * 2, base.yPositionOnScreen + sourceRect.Height + componentId / _texturesPerRow * (4 * sourceRect.Height), 4 * sourceRect.Width, 4 * sourceRect.Height), availableModels.First().Texture, new Rectangle(), 4f, false)
                    {
                        myID = componentId,
                        downNeighborID = componentId + _texturesPerRow,
                        upNeighborID = r >= _texturesPerRow ? componentId - _texturesPerRow : -1,
                        rightNeighborID = c == 5 ? 9997 : componentId + 1,
                        leftNeighborID = c > 0 ? componentId - 1 : 9998
                    });
                }
            }

            // Cache the input object to easily reference the vanilla texture
            _textureTarget = target;

            this.backButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen - 64, base.yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 9998,
                rightNeighborID = 0
            };
            this.forwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 64 - 48, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 9997
            };

            // Textbox related
            var xTextbox = base.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 320;
            var yTextbox = base.yPositionOnScreen - 58;
            _searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = xTextbox,
                Y = yTextbox,
                Width = 384,
                limitWidth = false,
                Text = String.Empty
            };

            _searchBoxCC = new ClickableComponent(new Rectangle(xTextbox, yTextbox, 192, 48), "")
            {
                myID = 9999,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            Game1.keyboardDispatcher.Subscriber = this._searchBox;
            _searchBox.Selected = true;

            this.queryButton = new ClickableTextureComponent(new Rectangle(xTextbox - 32, base.yPositionOnScreen - 48, 48, 44), Game1.mouseCursors, new Rectangle(208, 320, 16, 16), 2f)
            {
                myID = -1
            };

            // Call snap functions
            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }

        }

        public override void performHoverAction(int x, int y)
        {
            this.hovered = null;
            if (Game1.IsFading())
            {
                return;
            }

            foreach (ClickableTextureComponent c in this.availableTextures)
            {
                if (c.containsPoint(x, y))
                {
                    c.scale = Math.Min(c.scale + 0.05f, 4.1f);
                    this.hovered = c;
                }
                else
                {
                    c.scale = Math.Max(4f, c.scale - 0.025f);
                }
            }

            this.forwardButton.tryHover(x, y, 0.2f);
            this.backButton.tryHover(x, y, 0.2f);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Escape)
            {
                base.receiveKeyPress(key);
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (_searchBox.Text != _cachedTextBoxValue)
            {
                _startingRow = 0;
                _cachedTextBoxValue = _searchBox.Text;

                if (String.IsNullOrEmpty(_searchBox.Text))
                {
                    filteredTextureOptions = cachedTextureOptions;
                }
                else
                {
                    filteredTextureOptions = cachedTextureOptions.Where(i => !i.modData["AlternativeTextureName"].Contains(AlternativeTextures.DEFAULT_OWNER) && AlternativeTextures.textureManager.GetSpecificTextureModel(i.modData["AlternativeTextureName"]) is AlternativeTextureModel model && model.HasKeyword(i.modData["AlternativeTextureVariation"], _searchBox.Text)).ToList();
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = false)
        {
            base.receiveLeftClick(x, y, playSound);
            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            foreach (ClickableTextureComponent c in this.availableTextures)
            {
                if (c.containsPoint(x, y) && c.item != null)
                {
                    if (PatchTemplate.GetObjectAt(Game1.currentLocation, (int)_textureTarget.TileLocation.X * 64, (int)_textureTarget.TileLocation.Y * 64) != null)
                    {
                        _textureTarget.modData.Clear();
                        foreach (string key in c.item.modData.Keys)
                        {
                            _textureTarget.modData[key] = c.item.modData[key];
                        }
                    }
                    else if (PatchTemplate.GetTerrainFeatureAt(Game1.currentLocation, (int)_textureTarget.TileLocation.X * 64, (int)_textureTarget.TileLocation.Y * 64) is Flooring flooring)
                    {
                        flooring.modData.Clear();
                        foreach (string key in c.item.modData.Keys)
                        {
                            flooring.modData[key] = c.item.modData[key];
                        }
                    }

                    // Draw coloring animation
                    for (int j = 0; j < 12; j++)
                    {
                        var randomColor = new Color(Game1.random.Next(256), Game1.random.Next(256), Game1.random.Next(256));
                        AlternativeTextures.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(6, _textureTarget.tileLocation.Value * 64f, randomColor, 8, flipped: false, 50f)
                        {
                            motion = new Vector2((float)Game1.random.Next(-10, 11) / 10f, -Game1.random.Next(1, 3)),
                            acceleration = new Vector2(0f, (float)Game1.random.Next(1, 3) / 100f),
                            accelerationChange = new Vector2(0f, -0.001f),
                            scale = 0.8f,
                            layerDepth = (_textureTarget.tileLocation.Y + 1f) * 64f / 10000f,
                            interval = Game1.random.Next(20, 90)
                        });
                    }
                    //Game1.player.currentLocation.localSound("crit");
                    this.exitThisMenu();
                    return;
                }
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
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && _startingRow > 0)
            {
                _startingRow--;
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && (_maxRows + _startingRow) * _texturesPerRow < this.filteredTextureOptions.Count)
            {
                _startingRow++;
                Game1.playSound("shiny4");
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.dialogueUp && !Game1.IsFading())
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                SpriteText.drawStringWithScrollCenteredAt(b, "Paint Bucket", base.xPositionOnScreen + base.width / 4, base.yPositionOnScreen - 64);
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f);

                for (int i = 0; i < this.availableTextures.Count; i++)
                {
                    this.availableTextures[i].item = null;
                    this.availableTextures[i].texture = null;

                    // Note: The ordering of the PatchTemplate.GetObject & PatchTemplate.GetTerrainFeatureAt is import, as it prioritizes objects (like Mini-Obelisks) over terrain (like craftable paths)
                    var textureIndex = i + _startingRow * _texturesPerRow;
                    if (textureIndex < filteredTextureOptions.Count)
                    {
                        var target = filteredTextureOptions[textureIndex];
                        var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(target.modData["AlternativeTextureName"]);
                        var variation = Int32.Parse(target.modData["AlternativeTextureVariation"]);

                        this.availableTextures[i].item = target;
                        if (variation == -1)
                        {
                            if (_textureTarget is Fence)
                            {
                                this.availableTextures[i].texture = (_textureTarget as Fence).loadFenceTexture();
                                this.availableTextures[i].sourceRect = this.GetFenceSourceRect(_textureTarget as Fence, this.availableTextures[i].sourceRect.Height, -1);
                                this.availableTextures[i].draw(b, Color.White, 0.87f);
                            }
                            else if (PatchTemplate.GetObjectAt(Game1.currentLocation, (int)_textureTarget.TileLocation.X * 64, (int)_textureTarget.TileLocation.Y * 64) != null)
                            {
                                this.availableTextures[i].item.drawInMenu(b, new Vector2(this.availableTextures[i].bounds.X, this.availableTextures[i].bounds.Y + 32f), 2f, 1f, 0.87f, StackDrawType.Hide, Color.White, false);
                            }
                            else if (PatchTemplate.GetTerrainFeatureAt(Game1.currentLocation, (int)_textureTarget.TileLocation.X * 64, (int)_textureTarget.TileLocation.Y * 64) is Flooring flooring)
                            {
                                this.availableTextures[i].texture = Game1.GetSeasonForLocation(flooring.currentLocation)[0] == 'w' && (flooring.currentLocation == null || !flooring.currentLocation.isGreenhouse) ? Flooring.floorsTextureWinter : Flooring.floorsTexture;
                                this.availableTextures[i].sourceRect = this.GetFlooringSourceRect(flooring, this.availableTextures[i].sourceRect.Height, -1);
                                this.availableTextures[i].draw(b, Color.White, 0.87f);
                            }
                        }
                        else if (PatchTemplate.GetObjectAt(Game1.currentLocation, (int)_textureTarget.TileLocation.X * 64, (int)_textureTarget.TileLocation.Y * 64) is Furniture)
                        {
                            this.availableTextures[i].item.drawInMenu(b, new Vector2(this.availableTextures[i].bounds.X, this.availableTextures[i].bounds.Y + 32f), 2f, 1f, 0.87f, StackDrawType.Hide, Color.White, false);
                        }
                        else if (PatchTemplate.GetObjectAt(Game1.currentLocation, (int)_textureTarget.TileLocation.X * 64, (int)_textureTarget.TileLocation.Y * 64) != null)
                        {
                            this.availableTextures[i].texture = textureModel.Texture;
                            this.availableTextures[i].sourceRect = GetSourceRectangle(_textureTarget, textureModel.TextureWidth, textureModel.TextureHeight, variation);
                            this.availableTextures[i].draw(b, Color.White, 0.87f);
                        }
                        else if (PatchTemplate.GetTerrainFeatureAt(Game1.currentLocation, (int)_textureTarget.TileLocation.X * 64, (int)_textureTarget.TileLocation.Y * 64) is Flooring flooring)
                        {
                            this.availableTextures[i].texture = textureModel.Texture;
                            this.availableTextures[i].sourceRect = this.GetFlooringSourceRect(flooring, textureModel.TextureHeight, variation);
                            this.availableTextures[i].draw(b, Color.White, 0.87f);
                        }
                    }
                }

                _searchBox.Draw(b);
                queryButton.draw(b);
            }

            if (this.hovered != null && this.hovered.item != null)
            {
                if (this.hovered.item.modData.ContainsKey("AlternativeTextureName") && this.hovered.item.modData.ContainsKey("AlternativeTextureVariation"))
                {
                    var displayName = String.Concat(this.hovered.item.modData["AlternativeTextureName"], "_", Int32.Parse(this.hovered.item.modData["AlternativeTextureVariation"]) + 1);
                    IClickableMenu.drawHoverText(b, Game1.parseText(displayName, Game1.dialogueFont, 320), Game1.dialogueFont);
                }
            }

            if (_startingRow > 0)
            {
                this.backButton.draw(b);
            }
            if ((_maxRows + _startingRow) * _texturesPerRow < this.filteredTextureOptions.Count)
            {
                this.forwardButton.draw(b);
            }

            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }

        private Rectangle GetSourceRectangle(Object target, int textureWidth, int textureHeight, int variation)
        {
            var textureOffset = variation > 0 ? textureHeight * variation : 0;
            var sourceRect = new Rectangle(0, textureOffset, textureWidth, textureHeight);
            if (target is Fence fence)
            {
                sourceRect = this.GetFenceSourceRect(fence, textureHeight, variation);
            }
            else if (target is Furniture furniture)
            {
                sourceRect = furniture.sourceRect.Value;
                sourceRect.X -= furniture.defaultSourceRect.X;
                sourceRect.Y = textureOffset;
            }

            if (sourceRect.Width > 32)
            {
                sourceRect.Width = 32;
            }
            return sourceRect;
        }

        private Rectangle GetFenceSourceRect(Fence fence, int textureHeight, int variation)
        {
            int sourceRectPosition = 1;
            var textureOffset = variation == -1 ? 0 : variation * textureHeight;
            if ((float)fence.health > 1f || fence.repairQueued.Value)
            {
                int drawSum = fence.getDrawSum(Game1.currentLocation);
                sourceRectPosition = Fence.fenceDrawGuide[drawSum];

                var gateOffset = fence.isGate && variation != -1 ? 128 : 0;
                if ((bool)fence.isGate)
                {
                    Vector2 offset = new Vector2(0f, 0f);
                    switch (drawSum)
                    {
                        case 10:
                            return new Rectangle(((int)fence.gatePosition == 88) ? 24 : 0, textureOffset + (192 - gateOffset) + 16, 24, 32);
                        case 100:
                            return new Rectangle(((int)fence.gatePosition == 88) ? 24 : 0, textureOffset + (240 - gateOffset) + 16, 24, 32);
                        case 1000:
                            return new Rectangle(((int)fence.gatePosition == 88) ? 24 : 0, textureOffset + (288 - gateOffset), 24, 32);
                        case 500:
                            return new Rectangle(((int)fence.gatePosition == 88) ? 24 : 0, textureOffset + (320 - gateOffset), 24, 32);
                        case 110:
                            return new Rectangle(((int)fence.gatePosition == 88) ? 24 : 0, textureOffset + (128 - gateOffset), 24, 32);
                        case 1500:
                            return new Rectangle(((int)fence.gatePosition == 88) ? 16 : 0, textureOffset + (160 - gateOffset), 16, 16);
                    }
                    sourceRectPosition = 5;
                }
            }

            return new Rectangle((sourceRectPosition * Fence.fencePieceWidth % fence.fenceTexture.Value.Bounds.Width), textureOffset + (sourceRectPosition * Fence.fencePieceWidth / fence.fenceTexture.Value.Bounds.Width * Fence.fencePieceHeight), Fence.fencePieceWidth, Fence.fencePieceHeight);
        }

        private Rectangle GetFlooringSourceRect(Flooring flooring, int textureHeight, int variation)
        {
            int sourceRectOffset = variation == -1 ? (int)flooring.whichFloor * 4 * 64 : textureHeight * variation;
            byte drawSum = 0;
            Vector2 surroundingLocations = flooring.currentTileLocation;
            surroundingLocations.X += 1f;
            if (Game1.currentLocation.terrainFeatures.ContainsKey(surroundingLocations) && Game1.currentLocation.terrainFeatures[surroundingLocations] is Flooring)
            {
                drawSum = (byte)(drawSum + 2);
            }
            surroundingLocations.X -= 2f;
            if (Game1.currentLocation.terrainFeatures.ContainsKey(surroundingLocations) && Game1.currentLocation.terrainFeatures[surroundingLocations] is Flooring)
            {
                drawSum = (byte)(drawSum + 8);
            }
            surroundingLocations.X += 1f;
            surroundingLocations.Y += 1f;
            if (Game1.currentLocation.terrainFeatures.ContainsKey(surroundingLocations) && Game1.currentLocation.terrainFeatures[surroundingLocations] is Flooring)
            {
                drawSum = (byte)(drawSum + 4);
            }
            surroundingLocations.Y -= 2f;
            if (Game1.currentLocation.terrainFeatures.ContainsKey(surroundingLocations) && Game1.currentLocation.terrainFeatures[surroundingLocations] is Flooring)
            {
                drawSum = (byte)(drawSum + 1);
            }

            int sourceRectPosition = Flooring.drawGuide[drawSum];
            if ((bool)flooring.isSteppingStone)
            {
                sourceRectPosition = Flooring.drawGuideList[flooring.whichView.Value];
            }

            if (variation == -1)
            {
                return new Rectangle((int)flooring.whichFloor % 4 * 64 + sourceRectPosition * 16 % 256, sourceRectPosition / 16 * 16 + (int)flooring.whichFloor / 4 * 64, 16, 16);
            }

            return new Rectangle(sourceRectPosition % 16 * 16, sourceRectPosition / 16 * 16 + sourceRectOffset, 16, 16);
        }
    }
}