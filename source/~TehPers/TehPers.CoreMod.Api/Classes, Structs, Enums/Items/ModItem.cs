/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Items {
    public abstract class ModItem : IModItem {
        /// <summary>This object's translation helper.</summary>
        protected ICoreTranslationHelper TranslationHelper { get; }

        /// <summary>The raw name of this object.</summary>
        protected virtual string RawName { get; }

        /// <inheritdoc />
        public virtual ISprite Sprite { get; }

        protected ModItem(ICoreTranslationHelper translationHelper, string rawName, ISprite sprite) {
            this.TranslationHelper = translationHelper;
            this.RawName = rawName;
            this.Sprite = sprite;
        }

        /// <inheritdoc />
        public void OverrideDraw(IDrawingInfo info, Vector2 sourcePositionOffsetPercentage, Vector2 sourceSizePercentage) {
            this.Sprite.Draw(info.Batch, info.Destination, info.Tint, info.Rotation, info.Origin, info.Effects, info.Depth);
            info.Cancel();
        }
    }
}