using StardewModdingAPI.Utilities;
using SObject = StardewValley.Object;

namespace CustomCropsDecay
{
    public interface IApi
    {
        SDate getCropHarvestDate(SObject crop);
        int getCropDecayDays(SObject crop);
    }
}
