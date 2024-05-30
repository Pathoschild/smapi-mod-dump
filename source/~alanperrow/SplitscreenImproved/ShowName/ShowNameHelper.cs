/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace SplitscreenImproved.ShowName
{
    internal class ShowNameHelper
    {
        public static void DrawPlayerNameScroll(SpriteBatch sb)
        {
            // TODO: Maybe use transpiler instead of post-render call.
            //       I don't want the player name scroll to block useful information if the screen is very small.

            if (!ModEntry.Config.IsModEnabled || !ModEntry.Config.ShowNameFeature.IsFeatureEnabled)
            {
                return;
            }

            if (ModEntry.Config.ShowNameFeature.IsSplitscreenOnly && GameRunner.instance.gameInstances.Count == 1)
            {
                // We are not currently playing splitscreen.
                return;
            }

            var menu = Game1.activeClickableMenu;
            if (menu is null)
            {
                return;
            }

            if (menu is ShippingMenu or LevelUpMenu)
            {
                int posY = ModEntry.Config.ShowNameFeature.Position switch
                {
                    ShowNamePosition.Bottom => Game1.uiViewport.Height - 70,
                    _ => 30,
                };

                SpriteText.drawStringWithScrollCenteredAt(sb, Game1.player.Name, Game1.uiViewport.Width / 2, posY);

                return;
            }
        }
    }
}
