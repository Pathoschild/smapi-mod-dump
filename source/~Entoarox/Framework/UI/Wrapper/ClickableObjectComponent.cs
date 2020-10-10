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
using StardewValley;

namespace Entoarox.Framework.UI
{
    internal class ClickableObjectComponent : ClickableTextureComponent
    {
        /*********
        ** Public methods
        *********/
        public ClickableObjectComponent(Point position, int index, ClickHandler handler = null, bool scaleOnHover = true)
            : base(new Rectangle(position.X, position.Y, 16, 16), Game1.objectSpriteSheet, handler, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, index, 16, 16), scaleOnHover) { }
    }
}
