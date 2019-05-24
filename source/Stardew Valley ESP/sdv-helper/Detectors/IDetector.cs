using StardewValley;

namespace sdv_helper.Detectors
{
    interface IDetector
    {
        IDetector SetLocation(GameLocation loc);
        EntityList Detect();
    }
}
