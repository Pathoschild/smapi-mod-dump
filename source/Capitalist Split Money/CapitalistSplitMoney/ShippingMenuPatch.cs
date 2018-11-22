using Netcode;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapitalistSplitMoney
{
    class ShippingMenuMoneyPatcher : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(ShippingMenu), "parseItems");

        public static bool Prefix(IList<Item> items, List<int> ___categoryTotals, List<List<Item>> ___categoryItems, 
            ShippingMenu __instance, List<MoneyDial> ___categoryDials)
        {

            if (Game1.IsMasterGame)
            {
                var MoneyPerPlayer = new Dictionary<long, int>();

                foreach (var pair in ModEntry.OldItems)
                {
                    if (pair.Key is StardewValley.Object o)
                    {
                        int individualPrice = o.sellToStorePrice();
                        int stack = o.Stack;

                        int value = individualPrice * stack;
                        if (MoneyPerPlayer.ContainsKey(pair.Value))
                            MoneyPerPlayer[pair.Value] += value;
                        else
                            MoneyPerPlayer.Add(pair.Value, value);
                    }
                }


                var multiplayer = ModEntry.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();


                foreach (var pair in MoneyPerPlayer)
                {
                    if (pair.Key == Game1.player.UniqueMultiplayerID)
                    {
                        Game1.player.Money += pair.Value;
                        Console.WriteLine($"Host money += {pair.Value}");
                    }
                    else
                    {
                        NetRoot<FarmerTeam> fakeTeamRoot = new NetRoot<FarmerTeam>(new FarmerTeam());
                        fakeTeamRoot.Value.money.Minimum = null;

                        int moneyTheyGain = pair.Value;
                        if (ModEntry.MoneyData.PlayerMoney.ContainsKey(pair.Key))
                            ModEntry.MoneyData.PlayerMoney[pair.Key] += moneyTheyGain;
                        else
                            ModEntry.MoneyData.PlayerMoney.Add(pair.Key, Math.Max(0, ModEntry.Config.StartingMoney + moneyTheyGain));

                        fakeTeamRoot.Value.money.Set(moneyTheyGain);

                        var fakeData = multiplayer.writeObjectDeltaBytes(fakeTeamRoot);
                        BroadcastFarmerDeltasPatch.AllowOnce = true;
						if (fakeData != null)
						{
							if (Game1.getOnlineFarmers().Any((f) => f.UniqueMultiplayerID == pair.Key))
								Game1.server.sendMessage(pair.Key, 13, Game1.player, fakeData);
							else
								Console.WriteLine($"Player disconnected, do not need to send data to {pair.Key}");
						}
						else
						{
							Console.WriteLine($"Data is null, can't send data money for {pair.Key}");
						}
                        BroadcastFarmerDeltasPatch.AllowOnce = false;

                        Console.WriteLine($"{pair.Key} money += {moneyTheyGain}");
                    }
                }

                ModEntry.OldItems.Clear();
            }

            ////

            Utility.consolidateStacks(items);

            for (int k = 0; k < 6; k++)
            {
                ___categoryItems.Add(new List<Item>());
                ___categoryTotals.Add(0);
                ___categoryDials.Add(new MoneyDial(7, k == 5));
            }
            foreach (Item item in items)
            {
                if (item is StardewValley.Object)
                {
                    StardewValley.Object o = item as StardewValley.Object;
                    int category = __instance.getCategoryIndexForObject(o);
                    ___categoryItems[category].Add(o);
                    int index = category;
                    ___categoryTotals[index] += o.sellToStorePrice() * o.Stack;
                    Game1.stats.itemsShipped += (uint)o.Stack;
                    if (o.Category == -75 || o.Category == -79)
                    {
                        Game1.stats.CropsShipped += (uint)o.Stack;
                    }
                    if (o.countsForShippedCollection())
                    {
                        // Game1.player.shippedBasic(o.parentSheetIndex, o.stack);
                        Game1.player.shippedBasic(o.ParentSheetIndex, o.Stack);
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                ___categoryTotals[5] += ___categoryTotals[i];
                ___categoryItems[5].AddRange(___categoryItems[i]);
                ___categoryDials[i].currentValue = ___categoryTotals[i];
                ___categoryDials[i].previousTargetValue = ___categoryDials[i].currentValue;
            }
            ___categoryDials[5].currentValue = ___categoryTotals[5];


            //if (Game1.IsMasterGame)
            //{
            //    Game1.player.Money += ___categoryTotals[5];
            //}


            Game1.setRichPresence("earnings", ___categoryTotals[5]);

            ////
            
            

            return false;
        }
    }
}
