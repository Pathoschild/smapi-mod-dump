namespace SB_BQMS
{
    public class allSeedMakerValueContainer
    {
        public StardewValley.Object droppedObject;
        public bool hasBeenChecked = false;

        public allSeedMakerValueContainer(StardewValley.Object droppedObject, bool isChecked)
        {
            this.droppedObject = droppedObject;
            hasBeenChecked = isChecked;
        }
    }
}
