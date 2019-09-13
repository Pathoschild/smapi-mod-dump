using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SavableItemList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, int>>;

namespace CompostPestsCultivation
{
    public class SaveData
    {
        public List<Vector2> InfestedCrops = new List<Vector2>();

        public Dictionary<int, List<CropTrait>> CropTraits = new Dictionary<int, List<CropTrait>>();
        public Dictionary<int, int> CropSeeds = new Dictionary<int, int>();
        public Dictionary<string, List<Vector2>> TempQualityFertilizer = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } },
                              TempQualityIIFertilizer = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } };
        public Dictionary<string, List<Vector2>> TempWaterOneDay = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } },
                              TempWaterTwoDays = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } },
                                TempWaterStop = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } };

        public Dictionary<Vector2, SavableItemList> ComposterContents = new Dictionary<Vector2, SavableItemList>();
        public Dictionary<Vector2, int> CompostAppliedDays = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> ComposterDaysLeft = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> ComposterCompostLeft = new Dictionary<Vector2, int>();

        public void InitNullValues()
        {
            if (InfestedCrops == null)
                InfestedCrops = new List<Vector2>();
            if (CropTraits == null)
                CropTraits = new Dictionary<int, List<CropTrait>>();
            if (CropSeeds == null)
                CropSeeds = new Dictionary<int, int>();
            if (TempQualityFertilizer == null)
                TempQualityFertilizer = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } };
            if (TempQualityIIFertilizer == null)
                TempQualityIIFertilizer = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } };
            if (TempWaterStop == null)
                TempWaterStop = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } };
            if (TempWaterOneDay == null)
                TempWaterOneDay = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } };
            if (TempWaterTwoDays == null)
                TempWaterTwoDays = new Dictionary<string, List<Vector2>>() { { Cultivation.FarmName, new List<Vector2>() }, { Cultivation.GreenhouseName, new List<Vector2>() } };
            if (ComposterContents == null)
                ComposterContents = new Dictionary<Vector2, SavableItemList>();
            if (CompostAppliedDays == null)
                CompostAppliedDays = new Dictionary<Vector2, int>();
            if (ComposterDaysLeft == null)
                ComposterDaysLeft = new Dictionary<Vector2, int>();
            if (ComposterCompostLeft == null)
                ComposterCompostLeft = new Dictionary<Vector2, int>();
    }
    }
}
