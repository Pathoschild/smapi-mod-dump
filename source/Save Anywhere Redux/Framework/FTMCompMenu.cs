/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RealSweetPanda/SaveAnywhereRedux
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace SaveAnywhere.Framework
{
    /// <summary>The menu which lets the player choose their birthday.</summary>
    public class FTMCompMenu : IClickableMenu
    {
        public override bool shouldDrawCloseButton() => false;
        public override void update(GameTime time)
        {
            exitThisMenu(false);
        }
    }
}