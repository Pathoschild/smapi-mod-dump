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
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Items {
    public interface IModItem {
        /// <summary>The sprite for this item.</summary>
        ISprite Sprite { get; }

        /// <summary>Overrides the drawing information to properly draw the object.</summary>
        /// <param name="info">The drawing information that should be updated.</param>
        /// <param name="sourcePositionOffsetPercentage">The percentage of the source rectangle's size the source rectangle's position is offset from the top-left of the texture.</param>
        /// <param name="sourceSizePercentage">The percentage the source rectangle's size is of the original sprite.</param>
        void OverrideDraw(IDrawingInfo info, Vector2 sourcePositionOffsetPercentage, Vector2 sourceSizePercentage);
    }
}