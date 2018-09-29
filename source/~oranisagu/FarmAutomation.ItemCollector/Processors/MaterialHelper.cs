using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmAutomation.ItemCollector.Processors
{
    public class MaterialHelper
    {
        private readonly int[] _ores = {
            Object.copper,
            Object.iron,
            Object.gold,
            Object.iridium
        };

        private int[] _seedmakerDropIns;

        public Object FindMaterialForMachine(string machineName, Chest chest)
        {
            if (chest == null)
            {
                return null;
            }

            switch (machineName)
            {
                case "Keg":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsKegMaterial(i));
                case "Preserves Jar":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsPreservesJarMaterial(i));
                case "Cheese Press":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsCheesePressMaterial(i));
                case "Mayonnaise Machine":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsMayonnaiseMachineMaterial(i));
                case "Loom":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsLoomMaterial(i));
                case "Oil Maker":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsOilMakerMaterial(i));
                case "Recycling Machine":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsRecyclingMachineMaterial(i));
                case "Furnace":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsFurnaceMaterial(i));
                case "Coal":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && i.parentSheetIndex == Object.coal);
                case "Seed Maker":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsSeedMakerMaterial(i));
                case "Crab Pot":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && IsCrabPotMaterial(i));
                case "Charcoal Kiln":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && i.parentSheetIndex == Object.wood);
                case "Slime Egg-Press":
                    return (Object)chest.items.FirstOrDefault(i => i is Object && i.Name == "Slime");
                default:
                    return null;
            }
        }

        private bool IsCrabPotMaterial(Item item)
        {
            return item.category == Object.baitCategory;
        }

        private bool IsSeedMakerMaterial(Item item)
        {
            if (_seedmakerDropIns == null)
            {
                if (Game1.temporaryContent == null)
                {
                    Game1.temporaryContent = new ContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
                }
                Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
                _seedmakerDropIns = dictionary.Values.Select(v => v.Split('/')).Select(s => Convert.ToInt32(s[3])).ToArray();
            }
            return _seedmakerDropIns.Contains(item.parentSheetIndex);
        }

        /// <summary>
        /// most machines only take 1 object, a few exceptions have to be handled though
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public int GetMaterialAmountForMachine(string machineName, Object material)
        {
            if (machineName == "Furnace" && material.parentSheetIndex != Object.coal && material.parentSheetIndex != Object.quartzIndex)
            {
                return 5;
            }
            if (machineName == "Slime Egg-Press" && material.Name == "Slime")
            {
                return 100;
            }
            if (machineName == "Charcoal Kiln" && material.parentSheetIndex == Object.wood)
            {
                return 10;
            }
            return 1;
        }

        private bool IsFurnaceMaterial(Item item)
        {
            if (item.parentSheetIndex == Object.quartzIndex)
            {
                return true;
            }
            if (_ores.Contains(item.parentSheetIndex) && item.Stack >= 5)
            {
                return true;
            }
            return false;
        }

        public bool IsRecyclingMachineMaterial(Item item)
        {
            return
                item.parentSheetIndex == 168
                || item.parentSheetIndex == 169
                || item.parentSheetIndex == 170
                || item.parentSheetIndex == 171
                || item.parentSheetIndex == 172
                ;
        }

        public bool IsOilMakerMaterial(Item item)
        {
            return item.parentSheetIndex == 270
                   || item.parentSheetIndex == 421
                   || item.parentSheetIndex == 430
                   || item.parentSheetIndex == 431;
        }

        public bool IsMayonnaiseMachineMaterial(Item item)
        {
            return item.parentSheetIndex == 174
                   || item.parentSheetIndex == 107
                   || item.parentSheetIndex == 176
                   || item.parentSheetIndex == 180
                   || item.parentSheetIndex == 182
                   || item.parentSheetIndex == 442;
        }

        public bool IsCheesePressMaterial(Item item)
        {
            return item.parentSheetIndex == 184
                   || item.parentSheetIndex == 186
                   || item.parentSheetIndex == 436
                   || item.parentSheetIndex == 438;
        }

        public bool IsPreservesJarMaterial(Item item)
        {
            return item.category == Object.FruitsCategory
                   || item.category == Object.VegetableCategory
                ;
        }

        public bool IsKegMaterial(Item i)
        {
            return i.category == Object.FruitsCategory
                   || i.category == Object.VegetableCategory
                   || i.Name == "Wheat"
                   || i.Name == "Hops";
        }

        public bool IsLoomMaterial(Item i)
        {
            return i.parentSheetIndex == 440;
        }
    }
}
