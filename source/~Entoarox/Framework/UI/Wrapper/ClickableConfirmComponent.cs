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
    internal class ClickableConfirmComponent : ClickableTextureComponent
    {
        /*********
        ** Public methods
        *********/
        public ClickableConfirmComponent(Point position, ClickHandler handler = null)
            : base(new Rectangle(position.X, position.Y, 16, 16), Game1.mouseCursors, handler, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), true) { }
    }
}
