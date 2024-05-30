/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValleyTodo.Game;

namespace StardewValleyTodo.Tracker {
    public class TrackableDynamicItem : TrackableItemBase {
        private readonly Func<int> _getTotal;
        private readonly Func<int> _getCurrent;
        private readonly bool _showProgress;

        public TrackableDynamicItem(string displayName, Func<int> getTotal, Func<int> getCurrent,
            bool showProgress = true)
            : base(displayName, displayName) {
            _getTotal = getTotal;
            _getCurrent = getCurrent;
            _showProgress = showProgress;
        }

        public override Vector2 Draw(SpriteBatch sb, Vector2 position, Inventory inventory) {
            var hasItems = _getCurrent();
            var total = _getTotal();

            var display = DisplayName;
            if (_showProgress) {
                display += $" ({hasItems}/{total})";
            }

            var color = hasItems >= total ? Color.LightGreen : Color.White;
            sb.DrawString(Game1.smallFont, display, position, color);

            var size = Game1.smallFont.MeasureString(display);
            return size;
        }
    }
}
