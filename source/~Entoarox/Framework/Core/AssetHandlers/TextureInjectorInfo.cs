/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class TextureInjectorInfo
    {
        /*********
        ** Accessors
        *********/
        public Texture2D Texture { get; }
        public Rectangle? Source { get; }
        public Rectangle? Destination { get; }


        /*********
        ** Public methods
        *********/
        public TextureInjectorInfo(Texture2D texture, Rectangle? source, Rectangle? destination)
        {
            this.Texture = texture;
            this.Source = source;
            this.Destination = destination;
        }
    }
}
