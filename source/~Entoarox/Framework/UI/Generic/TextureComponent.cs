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

namespace Entoarox.Framework.UI
{
    public class TextureComponent : BaseMenuComponent
    {
        /*********
        ** Public methods
        *********/
        public TextureComponent(Rectangle area, Texture2D texture, Rectangle? crop = null)
            : base(area, texture, crop) { }
    }
}
