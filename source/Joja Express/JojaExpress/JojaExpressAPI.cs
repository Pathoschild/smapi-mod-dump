/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/SDV_JojaExpress
**
*************************************************/

using StardewValley;
using StardewValley.Internal;

namespace JojaExpress
{
    public class JojaExpressAPI : IJojaExpress
    {
        public int calculateGlobalProductCost(IDictionary<string, int> products)
        {
            int cost = 0;
            float percentage = getCurrentCarriageFee();
            foreach(var item in products)
            {
                cost += (int)(percentage * ItemRegistry.Create(item.Key, item.Value).salePrice());
            }
            return cost;
        }

        public int calculateGlobalProductCost(IDictionary<ISalable, int> products)
        {
            int cost = 0;
            float percentage = getCurrentCarriageFee();
            foreach (var item in products)
            {
                cost += (int)(percentage * item.Key.salePrice() * item.Value);
            }
            return cost;
        }

        public int calculateGlobalProductCost(IEnumerable<Item> products)
        {
            int cost = 0;
            float percentage = getCurrentCarriageFee();
            foreach (var item in products)
            {
                cost += (int)(percentage * item.salePrice() * item.Stack);
            }
            return cost;
        }

        public int calculateLocalProductCost(IDictionary<string, int> products, out IDictionary<string, int> trades, out bool stockEnough)
        {
            int cost = 0;
            var stock = ShopBuilder.GetShopStock("ofts.JojaExp.jojaLocal");
            trades = new Dictionary<string, int>();
            stockEnough = true;

            foreach (var item in products)
            {
                ItemStockInformation info = stock[ItemRegistry.Create(item.Key)];
                cost += info.Price * item.Value;
                if (info.TradeItemCount != null && info.TradeItemCount > 0) trades.Add(info.TradeItem, (int)info.TradeItemCount);
                if (info.Stock < item.Value) stockEnough = false;
            }
            return cost;
        }

        public int calculateLocalProductCost(IDictionary<ISalable, int> products, out IDictionary<ISalable, int> trades, out bool stockEnough)
        {
            int cost = 0;
            var stock = ShopBuilder.GetShopStock("ofts.JojaExp.jojaLocal");
            trades = new Dictionary<ISalable, int>();
            stockEnough = true;

            foreach (var item in products)
            {
                ItemStockInformation info = stock[item.Key];
                cost += info.Price * item.Value;
                if (info.TradeItemCount != null && info.TradeItemCount > 0) trades.Add(ItemRegistry.Create(info.TradeItem), (int)info.TradeItemCount);
                if (info.Stock < item.Value) stockEnough = false;
            }
            return cost;
        }

        public int calculateLocalProductCost(IEnumerable<Item> products, out IEnumerable<Item> trades, out bool stockEnough)
        {
            int cost = 0;
            var stock = ShopBuilder.GetShopStock("ofts.JojaExp.jojaLocal");
            trades = new List<Item>();
            stockEnough = true;

            foreach (var item in products)
            {
                ItemStockInformation info = stock[item];
                cost += info.Price * item.Stack;
                if (info.TradeItemCount != null && info.TradeItemCount > 0) trades.Append(ItemRegistry.Create(info.TradeItem, (int)info.TradeItemCount));
                if (info.Stock < item.Stack) stockEnough = false;
            }
            return cost;
        }

        public bool canBuyProductFromJojaGlobal(IDictionary<string, int> products, Farmer who)
        {
            return calculateGlobalProductCost(products) <= who.Money;
        }

        public bool canBuyProductFromJojaGlobal(IDictionary<ISalable, int> products, Farmer who)
        {
            return calculateGlobalProductCost(products) <= who.Money;
        }

        public bool canBuyProductFromJojaGlobal(IEnumerable<Item> products, Farmer who)
        {
            return calculateGlobalProductCost(products) <= who.Money;
        }

        public bool canBuyProductFromJojaLocal(IDictionary<string, int> products, Farmer who)
        {
            try
            {
                int price = calculateLocalProductCost(products, out var trades, out bool enough);
                if (!enough) return false;
                if (price > who.Money) return false;

                foreach (var item in trades)
                {
                    if (!who.Items.ContainsId(item.Key, item.Value)) return false;
                }
                return true;
            }catch (Exception) { return false; }
        }

        public bool canBuyProductFromJojaLocal(IDictionary<ISalable, int> products, Farmer who)
        {
            try
            {
                int price = calculateLocalProductCost(products, out var trades, out bool enough);
                if (!enough) return false;
                if (price > who.Money) return false;

                foreach (var item in trades)
                {
                    if (!who.Items.ContainsId(item.Key.QualifiedItemId, item.Value)) return false;
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool canBuyProductFromJojaLocal(IEnumerable<Item> products, Farmer who)
        {
            try
            {
                int price = calculateLocalProductCost(products, out var trades, out bool enough);
                if (!enough) return false;
                if (price > who.Money) return false;

                foreach (var item in trades)
                {
                    if (!who.Items.ContainsId(item.QualifiedItemId, item.Stack)) return false;
                }
                return true;
            }catch (Exception) { return false; }
        }

        public float getCurrentCarriageFee() => ModEntry.getPriceModifier();

        public void sendGlobalPackage(IDictionary<string, int> products, Farmer who)
        {
            ModEntry.needMail = true;
            foreach(var item in products)
            {
                PlayerInteractionHandler.mailPurchasedItem(ItemRegistry.Create(item.Key), who, item.Value);
            }
        }

        public void sendGlobalPackage(IDictionary<ISalable, int> products, Farmer who)
        {
            ModEntry.needMail = true;
            foreach (var item in products)
            {
                PlayerInteractionHandler.mailPurchasedItem(item.Key, who, item.Value);
            }
        }

        public void sendGlobalPackage(IEnumerable<Item> products, Farmer who)
        {
            ModEntry.needMail = true;
            foreach (var item in products)
            {
                PlayerInteractionHandler.mailPurchasedItem(item, who, item.Stack);
            }
        }

        public void sendLocalPackage(IDictionary<string, int> products, Farmer who)
        {
            GUI.sendPackage(who);
            ModEntry.localReceived = new Dictionary<string, int>(products);
        }

        public void sendLocalPackage(IDictionary<ISalable, int> products, Farmer who)
        {
            GUI.sendPackage(who);
            ModEntry.localReceived.Clear();
            foreach(var item in products)
            {
                ModEntry.localReceived.Add(item.Key.QualifiedItemId, item.Value);
            }
        }

        public void sendLocalPackage(IEnumerable<Item> products, Farmer who)
        {
            GUI.sendPackage(who);
            ModEntry.localReceived.Clear();
            foreach (var item in products)
            {
                ModEntry.localReceived.Add(item.QualifiedItemId, item.Stack);
            }
        }

        public bool tryBuyGlobalProducts(IDictionary<string, int> products, Farmer who)
        {
            int price = calculateGlobalProductCost(products);
            if (price > who.Money) return false;
            who.Money -= price;
            sendGlobalPackage(products, who);
            return true;
        }

        public bool tryBuyGlobalProducts(IDictionary<ISalable, int> products, Farmer who)
        {
            int price = calculateGlobalProductCost(products);
            if (price > who.Money) return false;
            who.Money -= price;
            sendGlobalPackage(products, who);
            return true;
        }

        public bool tryBuyGlobalProducts(IEnumerable<Item> products, Farmer who)
        {
            int price = calculateGlobalProductCost(products);
            if (price > who.Money) return false;
            who.Money -= price;
            sendGlobalPackage(products, who);
            return true;
        }

        public bool tryBuyLocalProducts(IDictionary<string, int> products, Farmer who)
        {
            try
            {
                int price = calculateLocalProductCost(products, out var trades, out bool enough);
                if (!enough) return false;
                if (price > who.Money) return false;

                foreach (var item in trades)
                {
                    if (!who.Items.ContainsId(item.Key, item.Value)) return false;
                }

                who.Money -= price;
                foreach (var item in trades)
                {
                    who.Items.ReduceId(item.Key, item.Value);
                }
                sendLocalPackage(products, who);
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool tryBuyLocalProducts(IDictionary<ISalable, int> products, Farmer who)
        {
            try
            {
                int price = calculateLocalProductCost(products, out var trades, out bool enough);
                if (!enough) return false;
                if (price > who.Money) return false;

                foreach (var item in trades)
                {
                    if (!who.Items.ContainsId(item.Key.QualifiedItemId, item.Value)) return false;
                }

                who.Money -= price;
                foreach (var item in trades)
                {
                    who.Items.ReduceId(item.Key.QualifiedItemId, item.Value);
                }
                sendLocalPackage(products, who);
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool tryBuyLocalProducts(IEnumerable<Item> products, Farmer who)
        {
            try
            {
                int price = calculateLocalProductCost(products, out var trades, out bool enough);
                if (!enough) return false;
                if (price > who.Money) return false;

                foreach (var item in trades)
                {
                    if (!who.Items.ContainsId(item.QualifiedItemId, item.Stack)) return false;
                }

                who.Money -= price;
                foreach (var item in trades)
                {
                    who.Items.ReduceId(item.QualifiedItemId, item.Stack);
                }
                sendLocalPackage(products, who);
                return true;
            }
            catch (Exception) { return false; }
        }

        public void openJojaExpressShoppingMenu() => PlayerInteractionHandler.openMenu();
    }
}
