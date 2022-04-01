/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Listeners;

namespace DeluxeJournal.Framework.Events
{
    /// <summary>Manages task events.</summary>
    /// <remarks>
    /// All of this is super hacky. We create hidden quests to listen for calls to Quest.checkIfComplete in order to
    /// fire our own events at the correct time. Primarily, this allows us to intercept items before they are marked
    /// as "has been in inventory" or to detect actions that would be difficult to discern as an outside observer.
    /// </remarks>
    internal class TaskEventManager
    {
        public event EventHandler<ItemReceivedEventArgs> ItemCollected
        {
            add => _itemHarvestListener.ItemHarvested += value;
            remove => _itemHarvestListener.ItemHarvested -= value;
        }

        public event EventHandler<ItemReceivedEventArgs> ItemCrafted
        {
            add => _craftingListener.ItemCrafted += value;
            remove => _craftingListener.ItemCrafted -= value;
        }

        public event EventHandler<GiftEventArgs> ItemGifted
        {
            add => _itemDeliveryListener.ItemGifted += value;
            remove => _itemDeliveryListener.ItemGifted -= value;
        }

        private readonly ItemHarvestListener _itemHarvestListener;
        private readonly CraftingListener _craftingListener;
        private readonly ItemDeliveryListener _itemDeliveryListener;

        public IModEvents ModEvents { get; }

        private IMonitor Monitor { get; }

        public TaskEventManager(IModEvents modEvents, IMonitor montior)
        {
            ModEvents = modEvents;
            Monitor = montior;

            _itemHarvestListener = new ItemHarvestListener();
            _craftingListener = new CraftingListener();
            _itemDeliveryListener = new ItemDeliveryListener();
        }

        public void DeployListeners()
        {
            // IMPORTANT: At most 3 custom hidden quests can be added (totalling 5 including vanilla)
            //  due to an indexing bug in the QuestLog.paginateQuests() function.
            Game1.player.questLog.Add(_itemHarvestListener);
            Game1.player.questLog.Add(_craftingListener);
            Game1.player.questLog.Add(_itemDeliveryListener);

            if (Game1.player.questLog.Count - Game1.player.visibleQuestCount > 5)
            {
                Monitor.Log("Too many hidden quests!", LogLevel.Alert);
            }
        }

        public void CleanupListeners()
        {
            Game1.player.questLog.Remove(_itemHarvestListener);
            Game1.player.questLog.Remove(_craftingListener);
            Game1.player.questLog.Remove(_itemDeliveryListener);
        }
    }
}
