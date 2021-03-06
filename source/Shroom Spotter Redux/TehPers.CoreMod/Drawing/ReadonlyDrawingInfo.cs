/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal class ReadonlyDrawingInfo : IReadonlyDrawingInfo {
        public Texture2D Texture { get; }
        public SRectangle? SourceRectangle { get; }
        public SRectangle Destination { get; }
        public SColor Tint { get; }
        public SpriteBatch Batch { get; }
        public Vector2 Origin { get; }
        public float Rotation { get; }
        public SpriteEffects Effects { get; }
        public float Depth { get; }

        public ReadonlyDrawingInfo(IReadonlyDrawingInfo info) {
            this.Texture = info.Texture;
            this.SourceRectangle = info.SourceRectangle;
            this.Destination = info.Destination;
            this.Tint = info.Tint;
            this.Batch = info.Batch;
            this.Origin = info.Origin;
            this.Rotation = info.Rotation;
            this.Effects = info.Effects;
            this.Depth = info.Depth;
        }

        public Vector2 GetScale() {
            SRectangle source = this.SourceRectangle ?? this.Texture.Bounds;
            return new Vector2((float) this.Destination.Width / source.Width, (float) this.Destination.Height / source.Height);
        }
    }
}