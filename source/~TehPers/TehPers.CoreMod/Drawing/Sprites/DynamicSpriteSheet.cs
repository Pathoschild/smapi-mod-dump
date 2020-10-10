/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Collections;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal class DynamicSpriteSheet : SpriteSheetBase {
        private readonly IMod _coreMod;
        private readonly ITrackedTextureInternal _trackedTexture;
        private readonly List<MoveableSprite> _sprites = new List<MoveableSprite>();
        private bool _dirty = false;
        private bool _warnOnAdd = false;

        public override ITrackedTexture TrackedTexture => this.GetTrackedTexture();

        public DynamicSpriteSheet(IMod coreMod) {
            this._coreMod = coreMod;
            Texture2D initialTexture = new Texture2D(Game1.graphics.GraphicsDevice, 16, 16);
            this._trackedTexture = new TrackedTexture(initialTexture);

            coreMod.Helper.Events.GameLoop.OneSecondUpdateTicking += (sender, args) => {
                this._warnOnAdd = true;
                this.RegenerateSpriteSheetIfNeeded();
            };
        }

        public override bool TryGetSprite(int index, out ISprite sprite) {
            // Make sure the index is valid
            if (index < 0 || index >= this._sprites.Count) {
                sprite = default;
                return false;
            }

            // Return the associated sprite
            sprite = this._sprites[index];
            return true;
        }

        public override int GetIndex(int u, int v) {
            // Search each sprite for one which contains the given point
            return this._sprites.FirstOrDefault(s => s.GetSourceRectangle().Contains(u, v))?.Index ?? throw new ArgumentException($"There is no sprite at the coordinates ({u}, {v})");
        }

        public ISprite Add(IApiHelper coreApiHelper, Texture2D texture, Rectangle sourceRectangle) {
            MoveableSprite moveableSprite = new MoveableSprite(this._sprites.Count, this, texture, sourceRectangle);
            this._sprites.Add(moveableSprite);
            this._dirty = true;

            if (this._warnOnAdd) {
                coreApiHelper.Log("A sprite was added to the custom item sprite sheet after initialization. The sprite sheet will be regenerated the next time it is used. Item sprites should only be registered during initialization so the sprite sheet doesn't need to be constantly regenerated.", LogLevel.Warn);
            } else {
                coreApiHelper.Log($"Added a sprite to the custom item sprite sheet with index {moveableSprite.Index}", LogLevel.Trace);
            }

            return moveableSprite;
        }

        private ITrackedTexture GetTrackedTexture() {
            this.RegenerateSpriteSheetIfNeeded();
            return this._trackedTexture;
        }

        public bool RegenerateSpriteSheetIfNeeded() {
            // Only regenerate if changes have been made
            if (!this._dirty) {
                return false;
            }

            this._dirty = false;
            this._coreMod.Monitor.Log("Regenerating custom item sprite sheet...", LogLevel.Debug);

            // Calculate where to put each sprite
            SpritePacker packer = new SpritePacker();
            packer.Fit(this._sprites, out int width, out int height, out IEnumerable<SpritePacker.LinkedSprite> linkedSprites);

            // If there were no sprites, then don't bother replacing the sprite sheet
            if (width == 0 && height == 0) {
                return true;
            }

            // Put each sprite on the texture where it belongs
            Color[] newData = new Color[width * height];
            Dictionary<ISprite, Rectangle> newSpriteLocations = linkedSprites.ToDictionary(s => s.SourceSprite, s => s.Destination);
            foreach (MoveableSprite sprite in this._sprites) {
                // Update the sprite's source rectangle
                Rectangle newSourceRectangle = newSpriteLocations[sprite];
                sprite.SetSourceRectangle(newSourceRectangle);

                // Place the sprite on the new sprite sheet
                for (int y = 0; y < sprite.Height; y++) {
                    for (int x = 0; x < sprite.Width; x++) {
                        int newDataIndex = (newSourceRectangle.Y + y) * width + (newSourceRectangle.X + x);
                        if (newDataIndex < 0 || newDataIndex >= newData.Length) {
                            throw new Exception();
                        }

                        int spriteDataIndex = y * sprite.Width + x;
                        if (spriteDataIndex < 0 || spriteDataIndex >= sprite.Data.Length) {
                            throw new Exception();
                        }

                        // Copy over the pixel to the new texture
                        newData[newDataIndex] = sprite.Data[spriteDataIndex];
                    }
                }
            }

            // Create a new texture
            Texture2D newSpriteSheet = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            newSpriteSheet.SetData(newData);

            // Update the tracked texture
            this.TrackedTexture.CurrentTexture.Dispose();
            this._trackedTexture.CurrentTexture = newSpriteSheet;

            return true;
        }

        private class MoveableSprite : SpriteBase {
            private Rectangle _sourceRectangle;
            public override SRectangle? SourceRectangle => this._sourceRectangle;
            public Color[] Data { get; }

            public MoveableSprite(int index, ISpriteSheet parentSheet, Texture2D texture, Rectangle sourceRectangle) : base(index, parentSheet) {
                this._sourceRectangle = sourceRectangle;
                this.Data = MoveableSprite.GetDataFromTexture(texture, sourceRectangle);
            }

            public void SetSourceRectangle(Rectangle sourceRectangle) {
                this._sourceRectangle = sourceRectangle;
            }

            public Rectangle GetSourceRectangle() {
                return this._sourceRectangle;
            }

            private static Color[] GetDataFromTexture(Texture2D texture, Rectangle sourceRectangle) {
                if (texture.Format == SurfaceFormat.Color) {
                    // Extract the color data from the texture
                    Color[] data = new Color[texture.Width * texture.Height];
                    texture.GetData(data);

                    // For simplicity, check if the source rectangle is the entire texture
                    if (texture.Bounds == sourceRectangle) {
                        return data;
                    }

                    // Copy the data to a new array
                    Color[] spriteData = new Color[sourceRectangle.Width * sourceRectangle.Height];
                    for (int x = 0; x < sourceRectangle.Width; x++) {
                        for (int y = 0; y < sourceRectangle.Height; y++) {
                            spriteData[y * sourceRectangle.Width + x] = data[(y + sourceRectangle.Y) * texture.Width + x + sourceRectangle.X];
                        }
                    }

                    return spriteData;
                } else {
                    throw new NotSupportedException($"Textures with the surface format {texture.Format} are not supported.");
                }
            }
        }
    }
}
