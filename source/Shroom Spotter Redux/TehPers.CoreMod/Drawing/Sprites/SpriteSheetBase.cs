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
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal abstract class SpriteSheetBase : ISpriteSheet {
        public abstract ITrackedTexture TrackedTexture { get; }

        public abstract bool TryGetSprite(int index, out ISprite sprite);
        public abstract int GetIndex(int u, int v);

        public event EventHandler<IDrawingInfo> Drawing {
            add => this.TrackedTexture.Drawing += value;
            remove => this.TrackedTexture.Drawing -= value;
        }

        public event EventHandler<IReadonlyDrawingInfo> Drawn {
            add => this.TrackedTexture.Drawn += value;
            remove => this.TrackedTexture.Drawn -= value;
        }
    }
}