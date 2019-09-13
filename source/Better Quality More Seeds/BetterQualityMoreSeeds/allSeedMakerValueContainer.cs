using StardewValley;
using SObject = StardewValley.Object;

namespace BetterQualityMoreSeeds
{
    internal class AllSeedMakerValueContainer
    {
        public SObject droppedObject;
        public GameLocation location;
        public bool hasBeenChecked;

        public AllSeedMakerValueContainer(SObject firstObject, GameLocation whereat, bool isChecked)
        {
            droppedObject = firstObject;
            location = whereat;
            hasBeenChecked = isChecked;
        }
    }
}
