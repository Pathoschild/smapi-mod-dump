using System;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Api.Drawing {
    [Obsolete("Use " + nameof(ISpriteSheet) + " instead.")]
    public interface ITextureSourceInfo {
        /// <summary>Converts a set of source coordinates to the <see cref="Item.ParentSheetIndex"/> associated with them for this texture.</summary>
        /// <param name="u">The x-coordinate in the source texture.</param>
        /// <param name="v">The y-coordinate in the source texture.</param>
        /// <returns>The <see cref="Item.ParentSheetIndex"/> associated with the given coordinates.</returns>
        int GetIndexFromUV(int u, int v);
    }
}