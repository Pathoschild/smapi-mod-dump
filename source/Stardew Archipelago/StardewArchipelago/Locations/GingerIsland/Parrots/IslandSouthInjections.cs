/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.Locations.GingerIsland.Parrots
{
    public class IslandSouthInjections : IParrotReplacer
    {
        private const string AP_WESTERN_TURTLE = "Island West Turtle";
        private const string AP_RESORT = "Island Resort";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        private IslandLocation _islandLocation;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public IslandSouthInjections()
        {
            _islandLocation = (IslandSouth)Game1.getLocationFromName("IslandSouth");
        }

        public void ReplaceParrots()
        {
            _islandLocation.parrotUpgradePerches.Clear();
            AddResortParrot(_islandLocation);
            AddWesternTurtleParrot(_islandLocation);
        }

        private static void AddResortParrot(IslandLocation __instance)
        {
            var resortParrot = new ParrotUpgradePerchArchipelago(AP_RESORT, _archipelago, __instance, new Point(17, 22),
                new Rectangle(12, 18, 14, 7), 20, PurchaseResortParrot, IsResortParrotPurchased,
                "Resort", "Island_UpgradeHouse");
            __instance.parrotUpgradePerches.Add(resortParrot);
        }

        private static void PurchaseResortParrot()
        {
            _locationChecker.AddCheckedLocation(AP_RESORT);
        }

        private static bool IsResortParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_RESORT);
        }

        private static void AddWesternTurtleParrot(IslandLocation __instance)
        {
            var westernParrot = new ParrotUpgradePerchArchipelago(AP_WESTERN_TURTLE, _archipelago, __instance,
                new Point(5, 9),
                new Rectangle(1, 10, 3, 4), 10,
                PurchaseWesternTurtleParrot,
                IsWesternTurtleParrotPurchased, "Turtle", "Island_FirstParrot");
            __instance.parrotUpgradePerches.Add(westernParrot);
        }

        private static void PurchaseWesternTurtleParrot()
        {
            _locationChecker.AddCheckedLocation(AP_WESTERN_TURTLE);
        }

        private static bool IsWesternTurtleParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_WESTERN_TURTLE);
        }
    }
}
