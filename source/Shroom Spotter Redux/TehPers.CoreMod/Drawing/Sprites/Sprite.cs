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