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
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Api.Drawing {
    public interface IReadonlyDrawingInfo {
        /// <summary>The source texture to draw from.</summary>
        Texture2D Texture { get; }

        /// <summary>The rectangle on the source texture representing the area to be drawn, or <c>null</c> if the whole source texture should be drawn.</summary>
        SRectangle? SourceRectangle { get; }

        /// <summary>The rectangle the texture will be drawn to.</summary>
        SRectangle Destination { get; }

        /// <summary>The color being applied to the texture while drawing. The formula used depends on the state of the <see cref="SpriteBatch"/>, but usually this color is multiplied by the source to calculate the destination color.</summary>
        SColor Tint { get; }

        /// <summary>The batch being used to draw the texture. Calls to this batch will not be intercepted by Teh's Core Mod during texture drawing events.</summary>
        SpriteBatch Batch { get; }

        /// <summary>The rotational and scaling origin of the texture.</summary>
        Vector2 Origin { get; }

        /// <summary>The amount to rotate the texture by.</summary>
        float Rotation { get; }

        /// <summary>The effects applied to the texture when drawing, including flipping horizontally or flipping vertically.</summary>
        SpriteEffects Effects { get; }

        /// <summary>The layer depth to draw the texture at. This can determine draw order when drawn as part of a batch.</summary>
        float Depth { get; }

        /// <summary>Gets the amount the source is being scaled before being drawn to the destination.</summary>
        /// <returns>The scaling amount for each dimension of the source.</returns>
        Vector2 GetScale();
    }
}