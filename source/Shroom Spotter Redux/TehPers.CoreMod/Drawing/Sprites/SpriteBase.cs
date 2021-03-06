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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal abstract class SpriteBase : ISprite {
        public int Index { get; }
        public ISpriteSheet ParentSheet { get; }
        public abstract SRectangle? SourceRectangle { get; }
        public int U => this.SourceRectangle?.X ?? 0;
        public int V => this.SourceRectangle?.Y ?? 0;
        public int Width => this.SourceRectangle?.Width ?? this.ParentSheet.TrackedTexture.CurrentTexture.Width;
        public int Height => this.SourceRectangle?.Height ?? this.ParentSheet.TrackedTexture.CurrentTexture.Height;

        private readonly Dictionary<EventHandler<IDrawingInfo>, EventHandler<IDrawingInfo>> _createdDrawingHandlers = new Dictionary<EventHandler<IDrawingInfo>, EventHandler<IDrawingInfo>>();
        private readonly Dictionary<EventHandler<IReadonlyDrawingInfo>, EventHandler<IReadonlyDrawingInfo>> _createdDrawnHandlers = new Dictionary<EventHandler<IReadonlyDrawingInfo>, EventHandler<IReadonlyDrawingInfo>>();

        protected SpriteBase(int index, ISpriteSheet parentSheet) {
            this.Index = index;
            this.ParentSheet = parentSheet;
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color) {
            batch.Draw(this.ParentSheet.TrackedTexture.CurrentTexture, position, this.SourceRectangle, color);
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) {
            batch.Draw(this.ParentSheet.TrackedTexture.CurrentTexture, position, this.SourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) {
            batch.Draw(this.ParentSheet.TrackedTexture.CurrentTexture, position, this.SourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color) {
            batch.Draw(this.ParentSheet.TrackedTexture.CurrentTexture, destinationRectangle, this.SourceRectangle, color);
        }

        public void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) {
            batch.Draw(this.ParentSheet.TrackedTexture.CurrentTexture, destinationRectangle, this.SourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        private EventHandler<T> CreateSpriteHandler<T>(EventHandler<T> handler, IDictionary<EventHandler<T>, EventHandler<T>> cache) where T : IReadonlyDrawingInfo {
            // Caching is used because delegates are reference types, and any two identical methods aren't necessarily equal
            // Aka if caching weren't used, then there'd be no way to remove event handlers from this object's events
            if (!cache.TryGetValue(handler, out EventHandler<T> result)) {
                result = (sender, info) => {
                    if (info.SourceRectangle == this.SourceRectangle) {
                        handler(sender, info);
                    }
                };

                cache.Add(handler, result);
            }

            return result;
        }

        public event EventHandler<IDrawingInfo> Drawing {
            add => this.ParentSheet.Drawing += this.CreateSpriteHandler(value, this._createdDrawingHandlers);
            remove => this.ParentSheet.Drawing -= this.CreateSpriteHandler(value, this._createdDrawingHandlers);
        }

        public event EventHandler<IReadonlyDrawingInfo> Drawn {
            add => this.ParentSheet.Drawn += this.CreateSpriteHandler(value, this._createdDrawnHandlers);
            remove => this.ParentSheet.Drawn -= this.CreateSpriteHandler(value, this._createdDrawnHandlers);
        }
    }
}