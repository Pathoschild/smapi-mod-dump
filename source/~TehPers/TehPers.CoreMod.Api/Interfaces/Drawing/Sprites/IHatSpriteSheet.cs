/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.CoreMod.Api.Drawing.Sprites {
    public interface IHatSpriteSheet : ISpriteSheet {
        /// <summary>Tries to get a reference to a specific hat sprite on this sprite sheet, given the direction the wearer is facing.</summary>
        /// <param name="hatId">The ID for the hat.</param>
        /// <param name="direction">The direction the wearer is facing.</param>
        /// <param name="sprite">The sprite for the hat.</param>
        /// <returns>True if the hat was found, false otherwise.</returns>
        bool TryGetSprite(int hatId, FacingDirection direction, out ISprite sprite);
    }
}