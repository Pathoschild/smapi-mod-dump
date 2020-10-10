using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace BetterJunimos.Utils {
    public class JunimoPayments {
        internal ModConfig.JunimoPayments Payment;

        public bool WereJunimosPaidToday;
        public Dictionary<int, List<int>> JunimoPaymentsToday = new Dictionary<int, List<int>>();

        internal JunimoPayments(ModConfig.JunimoPayments Payment) {
            this.Payment = Payment;
        }

        public bool ReceivePaymentItems(JunimoHut hut) {
            Farm farm = Game1.getFarm();
            Chest chest = hut.output.Value;
            bool paidForage = ReceiveItems(chest, Payment.DailyWage.ForagedItems, Util.ForageCategory);
            bool paidFlowers = ReceiveItems(chest, Payment.DailyWage.Flowers, Util.FlowerCategory);
            bool paidFruit = ReceiveItems(chest, Payment.DailyWage.Fruit, Util.FruitCategory);
            bool paidWine = ReceiveItems(chest, Payment.DailyWage.Wine, Util.WineCategory);

            return paidForage && paidFlowers && paidFruit && paidWine;
        }

        public bool ReceiveItems(Chest chest, int needed, int type) {
            if (needed == 0) return true;
            List<int> items;
            if (!JunimoPaymentsToday.TryGetValue(type, out items)) {
                items = new List<int>();
                JunimoPaymentsToday[type] = items;
            }
            int paidSoFar = items.Count();
            if (paidSoFar == needed) return true;

            foreach (int i in Enumerable.Range(paidSoFar, needed)) {
                Item foundItem = chest.items.FirstOrDefault(item => item != null && item.Category == type);
                if (foundItem != null) {
                    items.Add(foundItem.ParentSheetIndex);
                    Util.RemoveItemFromChest(chest, foundItem);
                }
            }
            return items.Count() == needed;
        }
    }
}
