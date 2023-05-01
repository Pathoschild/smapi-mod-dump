/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace AFKTimePause
{
    public class AFKMenu : IClickableMenu
    {
        public override void draw(SpriteBatch b)
        {
            if (ModEntry.Config.ShowAFKText)
            {
                SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.Config.AFKText, Game1.viewport.Width / 2, Game1.viewport.Height / 2);
            }
        }
    }
}