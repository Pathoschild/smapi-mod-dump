/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Xebeth/StardewValley-SpeedMod
**
*************************************************/

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
