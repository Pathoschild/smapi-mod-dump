using StardewValley;

namespace BetterQualityMoreSeeds
{
    internal class AllChestsValueContainer
    {
        public Item previousItem;
        public GameLocation location;
        public bool hasBeenChecked;

        public AllChestsValueContainer(Item item, GameLocation whereat, bool isChecked)
        {
            previousItem = item;
            location = whereat;
            hasBeenChecked = isChecked;
        }
    }
}
