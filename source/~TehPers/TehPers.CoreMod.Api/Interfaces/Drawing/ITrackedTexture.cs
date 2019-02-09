using Microsoft.Xna.Framework.Graphics;

namespace TehPers.CoreMod.Api.Drawing {
    /// <summary>A tracked texture, not bound to a specific reference to a <seealso cref="Texture2D"/>.</summary>
    public interface ITrackedTexture : ITextureEvents {
        /// <summary>A reference to the current texture being tracked. This may change over time, so it is advised not to store this anywhere for long term.</summary>
        Texture2D CurrentTexture { get; }
    }
}