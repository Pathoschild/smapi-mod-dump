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
using StardewValleyTodo.Game;

namespace StardewValleyTodo.Tracker {
    /// <summary>
    /// Base trackable item.
    /// </summary>
    public abstract class TrackableItemBase {
        public string Id {get;}

        /// <summary>
        /// Item name (identifier).
        /// </summary>
        public string DisplayName { get; }

        protected TrackableItemBase(string id, string displayName) {
            Id = id;
            DisplayName = displayName;
        }

        /// <summary>
        /// Draws item onto the sprite batch.
        /// </summary>
        /// <param name="sb">Sprite batch</param>
        /// <param name="position">Start position</param>
        /// <param name="inventory">Player's inventory</param>
        /// <returns>Drawn object size</returns>
        public abstract Vector2 Draw(SpriteBatch sb, Vector2 position, Inventory inventory);
    }
}
