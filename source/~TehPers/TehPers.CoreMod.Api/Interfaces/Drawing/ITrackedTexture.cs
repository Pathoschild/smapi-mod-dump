/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace TehPers.CoreMod.Api.Drawing {
    /// <summary>A tracked texture, not bound to a specific reference to a <seealso cref="Texture2D"/>.</summary>
    public interface ITrackedTexture : ITextureEvents {
        /// <summary>A reference to the current texture being tracked. This may change over time, so it is advised not to store this anywhere for long term.</summary>
        Texture2D CurrentTexture { get; }
    }
}