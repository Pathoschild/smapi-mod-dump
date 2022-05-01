/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace BetterJunimos.Utils {
    public class JunimoPayments {
        private readonly ModConfig.JunimoPayments _payment;

        public bool WereJunimosPaidToday;
        internal readonly Dictionary<int, List<int>> JunimoPaymentsToday = new();

        internal JunimoPayments(ModConfig.JunimoPayments payment) {
            _payment = payment;
        }

        public bool ReceivePaymentItems(JunimoHut hut) {
            var chest = hut.output.Value;
            var paidForage = ReceiveItems(chest, _payment.DailyWage.ForagedItems, Util.ForageCategory);
            var paidFlowers = ReceiveItems(chest, _payment.DailyWage.Flowers, Util.FlowerCategory);
            var paidFruit = ReceiveItems(chest, _payment.DailyWage.Fruit, Util.FruitCategory);
            var paidWine = ReceiveItems(chest, _payment.DailyWage.Wine, Util.WineCategory);

            return paidForage && paidFlowers && paidFruit && paidWine;
        }

        internal string PaymentOutstanding() {
            var needs = new List<string>();
            var cats = new List<(int, int, string)> {
                (_payment.DailyWage.ForagedItems, Util.ForageCategory, "Forage"),
                (_payment.DailyWage.Flowers, Util.FlowerCategory, "Flowers"),
                (_payment.DailyWage.Fruit, Util.FruitCategory, "Fruit"),
                (_payment.DailyWage.Wine, Util.WineCategory, "Wine")
            };
            foreach (var cat in cats) {
                var (needed, type, name) = cat;
                if (needed == 0) continue;
                if (!JunimoPaymentsToday.TryGetValue(type, out var items)) {
                    needs.Add($"{needed} {name}");
                } else if (items.Count < needed) {
                    needs.Add($"{needed - items.Count} {name}");
                }
            }

            return string.Join(", ", needs);
        }

        private bool ReceiveItems(Chest chest, int needed, int type) {
            if (needed == 0) return true;
            if (!JunimoPaymentsToday.TryGetValue(type, out var items)) {
                items = new List<int>();
                JunimoPaymentsToday[type] = items;
            }

            var paidSoFar = items.Count;
            if (paidSoFar == needed) return true;

            foreach (var unused in Enumerable.Range(paidSoFar, needed)) {
                var foundItem = chest.items.FirstOrDefault(item => item != null && item.Category == type);
                if (foundItem == null) continue;
                items.Add(foundItem.ParentSheetIndex);
                Util.RemoveItemFromChest(chest, foundItem);
            }

            return items.Count >= needed;
        }
    }
}