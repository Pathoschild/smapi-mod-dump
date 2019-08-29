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
        public List<Vector2> TempQualityFertilizer = new List<Vector2>(),
                              TempQualityIIFertilizer = new List<Vector2>();
        public List<Vector2> TempWaterOneDay = new List<Vector2>(),
                              TempWaterTwoDays = new List<Vector2>(),
                                TempWaterStop = new List<Vector2>();

        public Dictionary<Vector2, SavableItemList> ComposterContents = new Dictionary<Vector2, SavableItemList>();
        public Dictionary<Vector2, int> CompostAppliedDays = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> ComposterDaysLeft = new Dictionary<Vector2, int>();
        public Dictionary<Vector2, int> ComposterCompostLeft = new Dictionary<Vector2, int>();
    }
}
