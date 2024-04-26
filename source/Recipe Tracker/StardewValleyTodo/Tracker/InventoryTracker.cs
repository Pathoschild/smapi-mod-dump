/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValleyTodo.Game;

namespace StardewValleyTodo.Tracker {
    public class InventoryTracker {
        public List<TrackableItemBase> Items { get; private set; }

        public bool IsVisible { get; set; } = true;

        public InventoryTracker() {
            Items = new List<TrackableItemBase>();
        }

        /// <summary>
        /// Returns true if this list contains an item (or recipe) with specified name.
        /// </summary>
        /// <param name="name">Item (or recipe) name</param>
        /// <returns>True if this list contains specified item (or recipe)</returns>
        public bool Has(string name) {
            return Items.Find(x => x.DisplayName == name) != null;
        }

        /// <summary>
        /// Removes item with specified name from the list.
        /// </summary>
        /// <param name="name">Item name</param>
        public void Off(string name) {
            Items.RemoveAll(x => x.DisplayName == name);
        }

        /// <summary>
        /// Adds an item if there is no such item in the list, otherwise removes existed.
        /// </summary>
        /// <param name="item">Item</param>
        public void Toggle(TrackableItemBase item) {
            var found = Items.Find(x => x.DisplayName == item.DisplayName);
            if (found == null) {
                Items.Add(item);
            } else {
                Items.Remove(found);
            }
        }

        public void Reset() {
            Items = new List<TrackableItemBase>();
        }

        /// <summary>
        /// Updates todo list state.
        /// </summary>
        public void Update() {
            // Remove completed bundles.
            var toRemove = Items.OfType<TrackableJunimoBundle>().Where(x => x.Bundle.IsComplete).ToArray();
            foreach (var item in toRemove) {
                Items.Remove(item);
            }
        }

        /// <summary>
        /// Draws todo list onto the batch.
        /// </summary>
        /// <param name="batch">Sprite batch</param>
        /// <param name="position">Start position</param>
        /// <param name="inventory">Player inventory</param>
        /// <returns>Drawn area size</returns>
        public Vector2 Draw(SpriteBatch batch, Vector2 position, Inventory inventory) {
            if (!IsVisible) {
                return Vector2.Zero;
            }

            var size = Vector2.Zero;
            foreach (var item in Items) {
                var itemSize = item.Draw(batch, position, inventory);
                position.Y += itemSize.Y + 8;
                size.X = MathHelper.Max(size.X, itemSize.X);
                size.Y += itemSize.Y + 8;
            }
            size.Y -= 8;

            return size;
        }
    }
}
