using AnimalHusbandryMod.animals.data;
using System.Collections.Generic;
using AnimalHusbandryMod.animals;

namespace AnimalHusbandryMod.farmer
{
    public class FarmerData
    {
        public List<PregnancyItem> PregnancyData;
        public List<AnimalStatus> AnimalData;
        public List<AnimalContestItem> AnimalContestData;

        public FarmerData()
        {
            PregnancyData = new List<PregnancyItem>();
            AnimalData = new List<AnimalStatus>();
            AnimalContestData = new List<AnimalContestItem>();
        }
    }
}
