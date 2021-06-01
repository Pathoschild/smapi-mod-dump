/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using StardewValley;
using System;

namespace QuestEssentials.Messages
{
    public class SellItemMessage : StoryMessage
    {
        public Item Item { get; }
        public int ItemValue { get; }
        public bool Ship { get; }

        public SellItemMessage(Item item, int itemValue, bool ship = false) : base("SellItem")
        {
            this.Item = item ?? throw new ArgumentNullException(nameof(item));
            this.ItemValue = itemValue;
            this.Ship = ship;
        }
    }
}
