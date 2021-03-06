/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Drawing.Sprites {
    public class TintedSprite : ITintedSprite {
        /// <inheritdoc />
        public ISprite Sprite { get; }

        /// <inheritdoc />
        public SColor Tint { get; }

        /// <inheritdoc />
        public int Index => this.Sprite.Index;

        /// <inheritdoc />
        public ISpriteSheet ParentSheet => this.Sprite.ParentSheet;

        /// <inheritdoc />
        public SRectangle? SourceRectangle => this.Sprite.SourceRectangle;

        /// <inheritdoc />
        public int U => this.Sprite.U;

        /// <inheritdoc />
        public int V => this.Sprite.V;

        /// <inheritdoc />
        public int Width => this.Sprite.Width;

        /// <inheritdoc />
        public int Height => this.Sprite.Height;

        public TintedSprite(ISprite sprite, SColor tint) {
            this.Sprite = sprite;
            this.Tint = tint;
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, Vector2 position, Color color) {
            this.Sprite.Draw(batch, position, color * this.Tint);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            this.Sprite.Draw(batch, position, color * this.Tint, rotation, origin, scale, effects, layerDepth);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            this.Sprite.Draw(batch, position, color * this.Tint, rotation, origin, scale, effects, layerDepth);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color) {
            this.Sprite.Draw(batch, destinationRectangle, color * this.Tint);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
            this.Sprite.Draw(batch, destinationRectangle, color * this.Tint, rotation, origin, effects, layerDepth);
        }

        /// <inheritdoc />
        public event EventHandler<IDrawingInfo> Drawing {
            add => this.Sprite.Drawing += value;
            remove => this.Sprite.Drawing -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IReadonlyDrawingInfo> Drawn {
            add => this.Sprite.Drawn += value;
            remove => this.Sprite.Drawn -= value;
        }
    }
}
