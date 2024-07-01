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
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Archipelago.Gifting;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Items.Traps;
using StardewArchipelago.Locations;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Items
{
    public class ItemManager
    {
        public ItemParser ItemParser => _itemParser;

        private ArchipelagoClient _archipelago;
        private ItemParser _itemParser;
        private Mailman _mail;
        private HashSet<ReceivedItem> _itemsAlreadyProcessed;

        public ItemManager(IMonitor monitor, IModHelper helper, Harmony harmony, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager itemManager, Mailman mail, TileChooser tileChooser, BabyBirther babyBirther, GiftSender giftSender, IEnumerable<ReceivedItem> itemsAlreadyProcessed)
        {
            _archipelago = archipelago;
            _itemParser = new ItemParser(monitor, helper, harmony, archipelago, locationChecker, itemManager, tileChooser, babyBirther, giftSender);
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
