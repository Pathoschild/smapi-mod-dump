/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/SDV_Mods
**
*************************************************/

using StardewModdingAPI.Utilities;
using SObject = StardewValley.Object;

namespace CustomCropsDecay
{
    public interface IApi
    {
        SDate getCropHarvestDate(SObject crop);
        int getCropDecayDays(SObject crop);
    }
    public class Api : IApi
    {
        public int getCropDecayDays(SObject crop)
        {
            if(crop is CropWithDecay crop1)
            {
                return (int)crop1.decayDays;
            }
            else if (crop is ColoredCropWithDecay crop2)
            {
                return (int)crop2.decayDays;
            }
            return int.MaxValue;
        }

        public SDate getCropHarvestDate(SObject crop)
        {
            if (crop is CropWithDecay crop1)
            {
                return crop1.harvestDate;
            }
            else if (crop is ColoredCropWithDecay crop2)
            {
                return crop2.harvestDate;
            }
            return null;
        }
    }
}
