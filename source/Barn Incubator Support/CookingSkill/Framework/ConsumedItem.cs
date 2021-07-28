/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using SObject = StardewValley.Object;

namespace CookingSkill.Framework
{
    internal class ConsumedItem
    {
        public SObject Item { get; }
        public int Amount { get; }

        public ConsumedItem(SObject item)
        {
            this.Item = item;
            this.Amount = this.Item.Stack;
        }
    }
}
