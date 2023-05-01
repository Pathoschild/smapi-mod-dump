/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

namespace FishingTrawler.Messages
{
    public enum SyncType
    {
        Unknown,
        WaterLevel,
        FishCaught,
        Fuel,
        RestartGPS,
        TripTimer,
        GPSCooldown
    }

    internal class TrawlerSyncMessage
    {
        public SyncType SyncType { get; set; }
        public int Quantity { get; set; }

        public TrawlerSyncMessage()
        {

        }

        public TrawlerSyncMessage(SyncType syncType, int waterLevel)
        {
            SyncType = syncType;
            Quantity = waterLevel;
        }
    }
}
