/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Test
{
    public class Tester
    {
        private IModHelper _helper;
        private IMonitor _monitor;
        private ItemParser _itemParser;
        private Mailman _mail;
        private Random _random;

        public Tester(IModHelper helper, IMonitor monitor, Mailman mail)
        {
            _helper = helper;
            _monitor = monitor;
            _mail = mail;
            _random = new Random();
        }

        public void TestGetSpecificItem(string arg1, string[] arg2)
        {
            if (arg2.Length < 2)
            {
                return;
            }
            var amount = 1;
            amount = int.Parse(arg2[0]);

            var itemName = string.Join(" ", arg2.Skip(1).ToArray());
            var receivedItem = new ReceivedItem("locationName", itemName, "playerName", 1, 2, 3, _random.Next(10000, int.MaxValue));

            _itemParser = new ItemParser(new StardewItemManager(), new UnlockManager());
            try
            {
                var attachment = _itemParser.ProcessItem(receivedItem);
                attachment.SendToPlayer(_mail);
            }
            catch (Exception)
            {
                _monitor.Log($"Item: \"{itemName}\" was not processed properly by the mod", LogLevel.Error);
            }
        }

        public void TestGetAllItems(string arg1, string[] arg2)
        {
            _itemParser = new ItemParser(new StardewItemManager(), new UnlockManager());

            var itemsTable = _helper.Data.ReadJsonFile<Dictionary<string, JObject>>("stardew_valley_item_table.json");
            var items = itemsTable["items"];
            var attachments = new List<LetterItemAttachment>();
            foreach (var (key, jEntry) in items)
            {
                var code = jEntry["code"].Value<long>();
                var classification = Enum.Parse<ItemClassification>(jEntry["classification"].Value<string>(), true);
                var item = new ArchipelagoItem(key, code, classification);
                var receivedItem = new ReceivedItem("locationName", key, "playerName", 1, code, 3, _random.Next(10000, int.MaxValue));
                try
                {
                    var attachment = _itemParser.ProcessItem(receivedItem);
                    attachment.SendToPlayer(_mail);
                }
                catch (Exception)
                {
                    _monitor.Log($"Item: \"{key}\" was not processed properly by the mod", LogLevel.Error);
                }
            }
        }

        public void TestSendAllLocations(string arg1, string[] arg2)
        {
            throw new NotImplementedException();
        }
    }
}
