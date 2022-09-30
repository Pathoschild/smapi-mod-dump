/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omegasis.StardustCore.UIUtilities
{
    /// <summary>A class that keeps track of a collection of textures that are layered one on top of the others.</summary>
    public class LayeredTexture
    {
        public List<KeyValuePair<Rectangle, Texture2DExtended>> textureLayers;

        public LayeredTexture(List<KeyValuePair<Rectangle, Texture2DExtended>> textures)
        {
            this.textureLayers = textures;
        }

        /// <summary>Adds a new texture as the top layer.</summary>
        public void addTexture(KeyValuePair<Rectangle, Texture2DExtended> texture)
        {
            this.textureLayers.Add(texture);
        }

        /// <summary>Adds a new texture at a specific layer depth.</summary>
        public void addTexture(KeyValuePair<Rectangle, Texture2DExtended> texture, int index)
        {
            this.textureLayers.Insert(index, texture);
        }

        public LayeredTexture Copy()
        {
            return new LayeredTexture(this.textureLayers);
        }

        public void draw(SpriteBatch b, Color color, float layerDepth)
        {
            foreach (var texture in this.textureLayers)
                b.Draw(texture.Value.getTexture(), texture.Key, color);
        }
    }
}
