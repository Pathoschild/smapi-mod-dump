/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/CustomCrops
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CustomCrops
{
    public class CropData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Type_
        {
            Vegetable = -75,
            Fruit = -79,
            Flower = -80,
        }

        public string Id { get; set; }
        public string ProductName { get; set; }
        public string SeedName { get; set; }
        public string ProductDescription { get; set; }
        public string SeedDescription { get; set; }
        public Type_ Type { get; set; } = Type_.Vegetable;

        public IList<string> Seasons { get; set; } = new List<string>();
        public IList<int> Phases { get; set; } = new List<int>();
        public int RegrowthPhase { get; set; } = -1;
        public bool HarvestWithScythe { get; set; } = false;
        public bool TrellisCrop { get; set; } = false;
        public IList<Color> Colors { get; set; } = new List<Color>();
        public class Bonus_
        {
            public int MinimumPerHarvest { get; set; }
            public int MaximumPerHarvest { get; set; }
            public int MaxIncreasePerFarmLevel { get; set; }
            public double ExtraChance { get; set; }
        }
        public Bonus_ Bonus { get; set; } = null;

        public IList<string> SeedPurchaseRequirements { get; set; } = new List<string>();
        public int SeedPurchasePrice { get; set; }
        public int ProductSellPrice { get; set; }
        public int Edibility { get; set; }

        private int seedId, productId, cropId;
        internal int GetSeedId() { return seedId; }
        internal int GetProductId() { return productId; }
        internal int GetCropSpriteIndex() { return cropId; }
        internal string GetProductObjectInformation()
        {
            var itype = (int)Type;
            return $"{ProductName}/{ProductSellPrice}/{Edibility}/Basic {itype}/{ProductName}/{ProductDescription}";
        }
        internal string GetSeedObjectInformation()
        {
            return $"{SeedName}/{SeedPurchasePrice}/-300/Seeds -74/{SeedName}/{SeedDescription}";
        }
        internal string GetCropInformation()
        {
            string str = "";
            //str += GetProductId() + "/";
            foreach ( var phase in Phases )
            {
                str += phase + " ";
            }
            str = str.Substring(0, str.Length - 1) + "/";
            foreach (var season in Seasons)
            {
                str += season + " ";
            }
            str = str.Substring(0, str.Length - 1) + "/";
            str += $"{GetCropSpriteIndex()}/{GetProductId()}/{RegrowthPhase}/";
            str += (HarvestWithScythe ? "1" : "0") + "/";
            if (Bonus != null)
                str += $"true {Bonus.MinimumPerHarvest} {Bonus.MaximumPerHarvest} {Bonus.MaxIncreasePerFarmLevel} {Bonus.ExtraChance}/";
            else str += "false/";
            str += (TrellisCrop ? "true" : "false") + "/";
            if (Colors != null && Colors.Count > 0)
            {
                str += "true";
                foreach (var color in Colors)
                    str += $" {color.R} {color.G} {color.B}";
            }
            else
                str += "false";
            return str;
        }
        internal string GetSeedPurchaseRequirementString()
        {
            var str = $"{seedId}";
            foreach (var cond in SeedPurchaseRequirements)
                str += $"/{cond}";
            return str;
        }

        internal class Ids
        {
            public int Product;
            public int Seeds;
            public int Crop;
            internal static int MostRecentObject = 849;
            internal static int MostRecentCrop = 39;
        }
        internal static Dictionary<string, Ids> ids = new Dictionary<string, Ids>();
        internal static Dictionary<string, Ids> savedIds = new Dictionary<string, Ids>();

        internal static Dictionary<string, CropData> crops = new Dictionary<string, CropData>();
        public static void Register( CropData entry )
        {
            if ( !crops.ContainsKey( entry.Id ) )
                crops.Add(entry.Id, entry);

            Ids ids = new Ids();
            if ( savedIds.ContainsKey( entry.Id ) )
            {
                ids = savedIds[entry.Id];
            }
            else
            {
                ids.Product = ++Ids.MostRecentObject;
                if (entry.Colors != null && entry.Colors.Count > 0)
                    ++Ids.MostRecentObject;
                ids.Seeds = ++Ids.MostRecentObject;
                ids.Crop = ++Ids.MostRecentCrop;
            }

            entry.productId = ids.Product;
            entry.seedId = ids.Seeds;
            entry.cropId = ids.Crop;

            if (!savedIds.ContainsKey(entry.Id))
                savedIds.Add(entry.Id, ids);
        }
    }
}
