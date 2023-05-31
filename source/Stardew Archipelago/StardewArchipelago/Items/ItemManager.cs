/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Items.Traps;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Items
{
    public class ItemManager
    {
        private ArchipelagoClient _archipelago;
        private ItemParser _itemParser;
        private Mailman _mail;
        private HashSet<ReceivedItem> _itemsAlreadyProcessed;

        public ItemManager(IModHelper helper, ArchipelagoClient archipelago, StardewItemManager itemManager, Mailman mail, IEnumerable<ReceivedItem> itemsAlreadyProcessed)
        {
            _archipelago = archipelago;
            _itemParser = new ItemParser(helper, archipelago, itemManager);
            _mail = mail;
            _itemsAlreadyProcessed = itemsAlreadyProcessed.ToHashSet();
        }

        public TrapManager TrapManager => _itemParser.TrapManager;

        /*public void RegisterAllUnlocks()
        {
            var allReceivedItems = _archipelago.GetAllReceivedItemNamesAndCounts();
            foreach (var (itemName, numberReceived) in allReceivedItems)
            {
                _itemParser.ProcessUnlockWithoutGivingNewItems(itemName, numberReceived);
            }
        }*/

        public void ReceiveAllNewItems(bool immediatelyIfPossible)
        {
            var allReceivedItems = _archipelago.GetAllReceivedItems();

            foreach (var receivedItem in allReceivedItems)
            {
                ReceiveNewItem(receivedItem, immediatelyIfPossible);
            }
        }

        private void ReceiveNewItem(ReceivedItem receivedItem, bool immediatelyIfPossible)
        {
            if (_itemsAlreadyProcessed.Contains(receivedItem))
            {
                return;
            }

            ProcessItem(receivedItem, immediatelyIfPossible);
            _itemsAlreadyProcessed.Add(receivedItem);
        }

        private void ProcessItem(ReceivedItem receivedItem, bool immediatelyIfPossible)
        {
            if (immediatelyIfPossible)
            {
                if (_itemParser.TrySendItemImmediately(receivedItem))
                {
                    return;
                }
            }
            var attachment = _itemParser.ProcessItemAsLetter(receivedItem);
            attachment.SendToPlayer(_mail);
        }

        public List<ReceivedItem> GetAllItemsAlreadyProcessed()
        {
            return _itemsAlreadyProcessed.ToList();
        }
    }
}
