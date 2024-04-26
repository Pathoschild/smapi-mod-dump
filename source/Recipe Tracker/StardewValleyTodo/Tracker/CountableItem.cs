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
    /// <summary>
    /// Game item.
    /// </summary>
    class CountableItem : TrackableItemBase {
        /// <summary>
        /// Count to craft.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="id">Item id</param>
        /// <param name="name">Item name</param>
        /// <param name="count">Count to craft</param>
        public CountableItem(string id, string name, int count): base(id, name) {
            Count = count;
        }

        /// <inheritdoc />
        public override Vector2 Draw(SpriteBatch batch, Vector2 position, Inventory inventory) {
            var hasItems = inventory.Get(Id);
            var display = $"{DisplayName} ({hasItems}/{Count})";
            var color = hasItems >= Count ? Color.LightGreen : Color.White;
            batch.DrawString(Game1.smallFont, display, position, color);

            var size = Game1.smallFont.MeasureString(display);
            return size;
        }
    }
}
