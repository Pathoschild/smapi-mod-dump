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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Quests;
using StardewValleyTodo.Game;

namespace StardewValleyTodo.Tracker {
    public class TrackableQuest : TrackableItemBase {
        public IQuest Quest { get; private set; }

        private List<TrackableItemBase> _items;

        public TrackableQuest(string name, List<TrackableItemBase> items, IQuest quest) : base(name, name) {
            _items = items;
            Quest = quest;
        }

        public TrackableQuest(string name, TrackableItemBase item, IQuest quest) : base(name, name) {
            _items = new List<TrackableItemBase> { item };
            Quest = quest;
        }

        public override Vector2 Draw(SpriteBatch sb, Vector2 position, Inventory inventory) {
            var display = $"{DisplayName}";

            if (Quest.IsTimedQuest()) {
                display += $" ({Quest.GetDaysLeft()} days left)";
            }

            var size = Game1.smallFont.MeasureString(display);
            sb.DrawString(Game1.smallFont, display, position, Color.Yellow);
            position.Y += size.Y;

            foreach (var item in _items) {
                var itemSize = item.Draw(sb, position, inventory);
                position.Y += itemSize.Y;

                size.X = MathHelper.Max(size.X, itemSize.X);
                size.Y += itemSize.Y;
            }

            return size;
        }
    }
}
