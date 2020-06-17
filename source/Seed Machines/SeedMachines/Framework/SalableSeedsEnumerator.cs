using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework
{
    class SalableSeedsEnumerator
    {
        public IEnumerator<StardewValley.Object> GetEnumerator()
        {
            foreach (int id in Game1.objectInformation.Keys)
            {
                string[] fields = Game1.objectInformation[id]?.Split('/');
                if (
                    id != 79 //Secret Note
                    && !(id != 801 && fields?.Length >= 4 && fields[3] == "Ring") //Rings
                    && id != 812 // Roe
                )
                {
                    StardewValley.Object item = null;
                    item = new StardewValley.Object(id, 1);
                    if (item == null)
                        continue;
                    if (
                        /*item.Category != StardewValley.Object.FruitsCategory
                        && item.Category != StardewValley.Object.VegetableCategory
                        && item.Category != StardewValley.Object.flowersCategory
                        && item.Category != StardewValley.Object.sellAtFishShopCategory*/
                        item.Category == StardewValley.Object.SeedsCategory
                    )
                    {
                        yield return item;
                    }
                }
            }
        }

        public static Dictionary<ISalable, int[]> getSeedsForSale()
        {
            Dictionary<ISalable, int[]> result = new Dictionary<ISalable, int[]>();
            SalableSeedsEnumerator salableObjects = new SalableSeedsEnumerator();
            foreach (ISalable obj in salableObjects)
            {
                int finalPrice = obj.salePrice() <= 1 ? ModEntry.settings.seedMachinePriceForNonSalableSeeds : obj.salePrice();
                result.Add(obj, new int[2] { Convert.ToInt32(Math.Ceiling(finalPrice * ModEntry.settings.seedMachinePriceMultiplier)), int.MaxValue });
            }
            return result;
        }
    }
}
