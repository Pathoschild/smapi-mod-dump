using EqualMoneySplit.Models;
using EqualMoneySplit.Networking.Communicators;
using EqualMoneySplit.Networking.Models;
using Newtonsoft.Json;
using StardewValley;
using System;

namespace EqualMoneySplit.MoneyNetwork
{
    public sealed class MoneyListener : Listener
    {
        /// <summary>
        /// Use static property to handle instance; only created once on day start in a single thread
        /// </summary>
        public static Listener Instance { get; private set; } = new MoneyListener();

        /// <summary>
        /// Destination address the message will come from
        /// </summary>
        public override string Address => Constants.MoneySplitListenerAddress;

        /// <summary>
        /// Initializes the listener that will fire when the "EqualMoneySplit.MoneyListener" message is received
        /// </summary>
        /// <returns>The action to be performed when a message is received</returns>
        public override Action<Message> CreateMessageHandler()
        {
            return delegate (Message message)
            {
                MoneyPayload networkMoneyData;

                try
                {
                    object payload = message.Payload;
                    networkMoneyData = JsonConvert.DeserializeObject<MoneyPayload>(payload.ToString());
                }
                catch (Exception e)
                {
                    EqualMoneyMod.Logger.Log("Error deserializing received money payload: " + e.ToString(), StardewModdingAPI.LogLevel.Error);
                    throw;                
                }

                EqualMoneyMod.Logger.Log($"Local farmer {Game1.player.Name} is receiving {networkMoneyData.Money} from {networkMoneyData.Name}");
                EqualMoneyMod.Logger.Log($"Local farmer {Game1.player.Name} previously had {Game1.player.Money} and will now have {Game1.player.Money + networkMoneyData.Money}");

                if (networkMoneyData.EventContext == EventContext.InventoryChanged)
                    Game1.showGlobalMessage($"{networkMoneyData.Name} sent you {networkMoneyData.Money}g!");
                else if (networkMoneyData.EventContext == EventContext.EndOfDay)
                    Game1.chatBox.addInfoMessage($"{networkMoneyData.Name} sent you {networkMoneyData.Money}g!");
                
                Game1.player.Money += networkMoneyData.Money;
            };
        }
    }
}
