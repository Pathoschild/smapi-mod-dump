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
using System.Collections.Generic;
using System.Linq;

namespace StardewValleyTodo.Tracker {
    /// <summary>
    /// Crafting recipe.
    /// </summary>
    class TrackableRecipe : TrackableItemBase {
        /// <summary>
        /// Recipe components.
        /// </summary>
        public List<TrackableItemBase> Items { get; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="name">Recipe name</param>
        /// <param name="items">Recipe components</param>
        public TrackableRecipe(string name, IEnumerable<TrackableItemBase> items) : base(name, name) {
            Items = items.ToList();
        }

        /// <inheritdoc />
        public override Vector2 Draw(SpriteBatch batch, Vector2 position, Inventory inventory) {
            var display = $"{DisplayName}:";
            var size = Game1.smallFont.MeasureString(display);
            batch.DrawString(Game1.smallFont, display, position, Color.Yellow);
            position.Y += size.Y;

            foreach (var item in Items) {
                var itemSize = item.Draw(batch, position, inventory);
                position.Y += itemSize.Y;

                size.X = MathHelper.Max(size.X, itemSize.X);
                size.Y += itemSize.Y;
            }

            return size;
        }
    }
}
