using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal class Sprite : SpriteBase {
        public override SRectangle? SourceRectangle { get; }

        public Sprite(int index, ISpriteSheet parentSheet, SRectangle sourceRectangle) : base(index, parentSheet) {
            this.SourceRectangle = sourceRectangle;
        }
    }
}