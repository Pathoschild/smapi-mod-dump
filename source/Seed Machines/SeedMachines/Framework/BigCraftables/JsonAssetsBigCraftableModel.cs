using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework.BigCraftables
{
    public class JsonAssetsBigCraftableModel
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public int Price { get; set; }
        public bool IsDefault { get; set; }
        public bool ProvidesLight { get; set; }
        public int ReserveExtraIndexCount { get; set; }
        public JARecipe Recipe { get; set; }
        public IDictionary<String, String> NameLocalization { get; set; }
        public IDictionary<String, String> DescriptionLocalization { get; set; }
    }

    public class JARecipe
    {
        public int ResultCount { get; set; }
        public List<IDictionary<String, int>> Ingredients { get; set; }
        public bool CanPurchase { get; set; }
    }
}
