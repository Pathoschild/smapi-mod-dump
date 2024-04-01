/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using ConvenientChests.CategorizeChests.Interface.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ConvenientChests.CategorizeChests.Interface {
    class TooltipManager : ITooltipManager {
        private Widget Tooltip;

        public void ShowTooltipThisFrame(Widget tooltip) {
            Tooltip = tooltip;
        }

        public void Draw(SpriteBatch batch) {
            if (Tooltip == null)
                return;

            var mousePosition = Game1.getMousePosition(true);

            Tooltip.Position = new Point(
                mousePosition.X + 8 * Game1.pixelZoom,
                mousePosition.Y + 8 * Game1.pixelZoom
            );

            Tooltip.Draw(batch);
            Tooltip = null;
        }
    }
}