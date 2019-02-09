using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.Core.Multiplayer.Synchronized;

namespace TehPers.Core.Items.Managed {
    public interface IApiManagedObject : ISynchronized {
        /// <summary>Gets this object's display name. It should be localized by the mod.</summary>
        /// <returns>The display name.</returns>
        string GetDisplayName();

        /// <summary>Gets this object's description. It should be localized by the mod.</summary>
        /// <returns>The description.</returns>
        string GetDescription();

        /// <summary>Gets the <see cref="Texture2D"/> to draw this object from. </summary>
        /// <returns>The <see cref="Texture2D"/> to draw from.</returns>
        Texture2D GetTexture();

        /// <summary>Gets the rectangle representing where this object is on the texture returned from <see cref="GetTexture"/>.</summary>
        /// <returns>The source rectangle for this object's texture, or <c>null</c> if the whole source texture is used.</returns>
        Rectangle? GetSourceRectangle();
    }
}