using System.Collections.Generic;
using Microsoft.Xna.Framework;

using StardewValley;

namespace CompostPestsCultivation
{
    public class SaveData
    {
        public List<Vector2> InfestedCrops { get; set; } = new List<Vector2>();
        public const string _InfestedCrops = "InfestedCrops";

        public Dictionary<int, List<CropTrait>> CropTraits { get; set; } = new Dictionary<int, List<CropTrait>>();
        public const string _CropTraits = "CropTraits";
        public Dictionary<int, int> CropSeeds { get; set; } = new Dictionary<int, int>();
        public const string _CropSeeds = "CropSeeds";
        public List<Vector2> TempQualityFertilizer = new List<Vector2>(),
                              TempQualityIIFertilizer = new List<Vector2>();
        public const string _TempQualityFertilizer = "TempQualityFertilizer";
        public const string _TempQualityIIFertilizer = "TempQualityIIFertilizer";
        public List<Vector2> TempWaterOneDay = new List<Vector2>(),
                              TempWaterTwoDays = new List<Vector2>(),
                                TempWaterStop = new List<Vector2>();
        public const string _TempWaterOneDay = "TempWaterOneDay";
        public const string _TempWaterTwoDays = "TempWaterTwoDays";
        public const string _TempWaterStop = "TempWaterStop";

        public Dictionary<Vector2, List<Object>> CompostContents { get; set; } = new Dictionary<Vector2, List<Object>>();
        public const string _CompostContents = "CompostContents";

        public List<Vector2> CompostApplied { get; set; } = new List<Vector2>();
        public const string _CompostApplied = "CompostApplied";
    }
}
