/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Reflection;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Locations.InGameLocations;

namespace StardewArchipelago.Constants
{
    public static class IDProvider
    {
        public static readonly string AP_LOCATION = CreateId<ArchipelagoLocation>();
        public static readonly string MONEY = CreateId("Money");
        public static readonly string QI_GEM = CreateId("QiGem");
        public static readonly string QI_COIN = CreateId("QiCoin");
        public static readonly string STAR_TOKEN = CreateId("StarToken");
        public static readonly string METAL_DETECTOR_ITEMS = CreateId("MetalDetectorItems");
        public static readonly string TRAVELING_CART_DAILY_CHECK = CreateId("TravelingCartDailyCheck");
        public static readonly string ARCHIPELAGO_EQUIPMENTS = CreateId("ArchipelagoEquipments");
        public const string ARCHIPELAGO_EQUIPMENTS_SALE = "Sale";
        public const string ARCHIPELAGO_EQUIPMENTS_RECOVERY = "Recovery";


        public static string CreateApLocationItemId(string locationName)
        {
            return $"{QualifiedItemIds.ARCHIPELAGO_QUALIFER}{AP_LOCATION} {locationName}";
        }

        public static string CreateId(string name)
        {
            return $"{ModEntry.Instance.ModManifest.UniqueID}.{name}";
        }

        private static string CreateId(MemberInfo t)
        {
            return CreateId(t.Name);
        }

        private static string CreateId<T>()
        {
            return CreateId(typeof(T));
        }
    }
}
