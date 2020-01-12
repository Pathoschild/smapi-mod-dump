using StardewValley;

namespace SpeedMod
{
    internal class TeleportSuccessArgs
    {
        public Farmer Farmer { get; }

        public TeleportSuccessArgs(Farmer farmer)
        {
            Farmer = farmer;
        }
    }
}
