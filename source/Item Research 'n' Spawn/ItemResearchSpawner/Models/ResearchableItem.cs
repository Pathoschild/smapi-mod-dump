/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using ItemResearchSpawner.Components;
using Object = StardewValley.Object;

namespace ItemResearchSpawner.Models
{
    internal class ResearchableItem
    {
        public SpawnableItem Item { get; set; }

        public ResearchProgression Progression { get; set; }

        public int GetAvailableQuantity(int money, ItemQuality requestedQuality, out ItemQuality maxAvailableQuality)
        {
            while (true)
            {
                int quantity;

                switch (requestedQuality)
                {
                    case ItemQuality.Silver:
                        if (Progression.ResearchCountSilver >= Item.ProgressionLimit)
                        {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0)
                            {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    case ItemQuality.Gold:
                        if (Progression.ResearchCountGold >= Item.ProgressionLimit)
                        {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0)
                            {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    case ItemQuality.Iridium:
                        if (Progression.ResearchCountIridium >= Item.ProgressionLimit)
                        {
                            quantity = GetQuantityForQuality(money, requestedQuality);

                            if (quantity > 0)
                            {
                                maxAvailableQuality = requestedQuality;
                                return quantity;
                            }
                        }

                        requestedQuality = requestedQuality.GetPrevious();
                        continue;

                    default:
                    {
                        maxAvailableQuality = requestedQuality;
                        return GetQuantityForQuality(money, ItemQuality.Normal);
                    }
                }
            }
        }

        private int GetQuantityForQuality(int money, ItemQuality requestedQuality)
        {
            var item = Item.CreateItem();

            if (item is Object obj)
            {
                obj.Quality = (int) requestedQuality;
            }

            try
            {
                return money / ModManager.Instance.GetItemPrice(item);
            }
            
            catch (Exception)
            {
                return item.maximumStackSize();
            }
        }

        public ItemQuality GetAvailableQuality(ItemQuality requestedQuality)
        {
            while (true)
            {
                switch (requestedQuality)
                {
                    case ItemQuality.Silver:
                        if (Progression.ResearchCountSilver >= Item.ProgressionLimit) return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Gold:
                        if (Progression.ResearchCountGold >= Item.ProgressionLimit) return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    case ItemQuality.Iridium:
                        if (Progression.ResearchCountIridium >= Item.ProgressionLimit) return requestedQuality;
                        requestedQuality = requestedQuality.GetPrevious();
                        continue;
                    default:
                        return ItemQuality.Normal;
                }
            }
        }
    }
}