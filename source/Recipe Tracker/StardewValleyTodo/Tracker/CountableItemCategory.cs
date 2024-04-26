/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValleyTodo.Game;

namespace StardewValleyTodo.Tracker {
    public class CountableItemCategory : TrackableItemBase {
        /// <summary>
        /// Count to craft.
        /// </summary>
        public int Count { get; }

        public CountableItemCategory(string id, string displayName, int count) : base(id, displayName) {
            Count = count;
        }

        public override Vector2 Draw(SpriteBatch sb, Vector2 position, Inventory inventory) {
            var hasItems = inventory.GetCountByCategory(Id);
            var display = $"{DisplayName} ({hasItems}/{Count})";
            var color = hasItems >= Count ? Color.LightGreen : Color.White;
            sb.DrawString(Game1.smallFont, display, position, color);

            var size = Game1.smallFont.MeasureString(display);
            return size;
        }
    }
}
