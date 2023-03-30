/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Utilities;
using StardewValley.Objects;

namespace FishingTrawler.Framework.Objects.Items.Rewards
{
    public class AnglerRing
    {
        private const int JUKEBOX_RING_BASE_ID = 528;

        public static Ring CreateInstance()
        {
            var ring = new Ring(JUKEBOX_RING_BASE_ID);
            ring.modData[ModDataKeys.ANGLER_RING_KEY] = true.ToString();

            return ring;
        }
    }
}