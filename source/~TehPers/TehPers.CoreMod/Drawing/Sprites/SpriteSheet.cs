/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal class SpriteSheet : SpriteSheetBase {
        public override ITrackedTexture TrackedTexture { get; }

        private readonly int _spriteWidth;
        private readonly int _spriteHeight;
        private readonly int _widthInTiles;
        private readonly Dictionary<int, ISprite> _spriteCache = new Dictionary<int, ISprite>();

        public SpriteSheet(ITrackedTexture trackedTexture, int spriteWidth, int spriteHeight) {
            this.TrackedTexture = trackedTexture;
            this._spriteWidth = spriteWidth;
            this._spriteHeight = spriteHeight;
            this._widthInTiles = trackedTexture.CurrentTexture.Width / this._spriteWidth;
        }

        public override bool TryGetSprite(int index, out ISprite sprite) {
            // Try to grab a cached sprite
            if (!this._spriteCache.TryGetValue(index, out sprite)) {
                // Create a new sprite
                sprite = new Sprite(index, this, this.GetSourceRectangleFromIndex(index));
                this._spriteCache.Add(index, sprite);
            }

            return true;
        }

        public override int GetIndex(int u, int v) {
            int x = u / this._spriteWidth;
            int y = v / this._spriteHeight;
            return y * this._widthInTiles + x;
        }

        private SRectangle GetSourceRectangleFromIndex(int index) {
            int sheetWidth = this.TrackedTexture.CurrentTexture.Width;
            int u = index * this._spriteWidth % sheetWidth;
            int v = index * this._spriteWidth / sheetWidth * this._spriteHeight;
            return new SRectangle(u, v, this._spriteWidth, this._spriteHeight);
        }
    }
}