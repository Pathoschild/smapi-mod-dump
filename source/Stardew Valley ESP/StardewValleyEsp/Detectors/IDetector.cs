using StardewValley;

namespace StardewValleyEsp.Detectors
{
    interface IDetector
    {
        IDetector SetLocation(GameLocation loc);
        EntityList Detect();
    }
}
