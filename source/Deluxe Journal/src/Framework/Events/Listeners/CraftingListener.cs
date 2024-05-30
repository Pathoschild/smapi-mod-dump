/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using DeluxeJournal.Events;
using StardewValley;

namespace DeluxeJournal.Framework.Listeners
{
    /// <summary>Listens for Quest.type_crafting completion checks.</summary>
    [Obsolete]
    internal class CraftingListener : QuestEventListener
    {
        public event EventHandler<ItemReceivedEventArgs>? ItemCrafted;

        public CraftingListener() : base(type_crafting)
        {
        }

        protected override void OnChecked(NPC? npc, int index, int count, Item? item, string? str)
        {
            if (item is SObject crafted)
            {
                ItemCrafted?.Invoke(null, new ItemReceivedEventArgs(Game1.player, crafted, 1));
            }
        }
    }
}
