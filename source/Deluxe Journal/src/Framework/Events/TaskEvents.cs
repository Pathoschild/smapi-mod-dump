/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI.Events;
using DeluxeJournal.Events;

namespace DeluxeJournal.Framework.Events
{
    internal class TaskEvents : ITaskEvents
    {
        private readonly TaskEventManager _eventManager;

        public event EventHandler<ItemReceivedEventArgs> ItemCollected
        {
            add => _eventManager.ItemCollected += value;
            remove => _eventManager.ItemCollected -= value;
        }

        public event EventHandler<ItemReceivedEventArgs> ItemCrafted
        {
            add => _eventManager.ItemCrafted += value;
            remove => _eventManager.ItemCrafted -= value;
        }

        public event EventHandler<GiftEventArgs> ItemGifted
        {
            add => _eventManager.ItemGifted += value;
            remove => _eventManager.ItemGifted -= value;
        }

        public IModEvents ModEvents => _eventManager.ModEvents;

        public TaskEvents(TaskEventManager eventManager)
        {
            _eventManager = eventManager;
        }
    }
}
