/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Listeners
{
    /// <summary>Listens for Quest.type_harvest completion checks.</summary>
    [Obsolete]
    internal class ItemHarvestListener : QuestEventListener
    {
        public event EventHandler<ItemReceivedEventArgs>? ItemHarvested;

        public ItemHarvestListener() : base(type_harvest)
        {
        }

        protected override void OnChecked(NPC? npc, int index, int count, Item? item, string? str)
        {
            if (item is SObject collected && !collected.HasBeenInInventory)
            {
                ItemHarvested?.Invoke(null, new ItemReceivedEventArgs(Game1.player, collected, count));
            }
        }
    }
}
