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
    /// <summary>Listens for Quest.type_itemDelivery completion checks.</summary>
    internal class ItemDeliveryListener : QuestEventListener
    {
        public event EventHandler<GiftEventArgs>? ItemGifted;

        public ItemDeliveryListener() : base(type_itemDelivery)
        {
        }

        protected override void OnChecked(NPC? npc, int index, int count, Item? item, string? str)
        {
            if (npc != null && item != null)
            {
                ItemGifted?.Invoke(null, new GiftEventArgs(npc, item));
            }
        }
    }
}
