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
    public class IslandWestInjections : IParrotReplacer
    {
        private const string AP_FARM_OBELISK_PARROT = "Farm Obelisk";
        private const string AP_MAILBOX_PARROT = "Island Mailbox";
        private const string AP_FARMHOUSE_PARROT = "Island Farmhouse";
        private const string AP_PARROT_EXPRESS_PARROT = "Parrot Express";

        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        private IslandLocation _islandWest;

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        public IslandWestInjections()
        {
            _islandWest = (IslandWest)Game1.getLocationFromName("IslandWest");
        }

        public void ReplaceParrots()
        {
            _islandWest.parrotUpgradePerches.Clear();
            AddFarmObeliskParrot(_islandWest);
            AddMailboxParrot(_islandWest);
            AddFarmhouseParrot(_islandWest);
            AddParrotExpressParrot(_islandWest);
        }

        private static void AddFarmObeliskParrot(IslandLocation islandWest)
        {
            islandWest.parrotUpgradePerches.Add(new ParrotUpgradePerchArchipelago(AP_FARM_OBELISK_PARROT, _archipelago, islandWest,
                new Point(72, 37),
                new Rectangle(71, 29, 3, 8), 20,
                PurchaseFarmObeliskParrot,
                IsFarmObeliskParrotPurchased,
                "Obelisk",
                "Island_UpgradeHouse_Mailbox"));
        }

        private static void PurchaseFarmObeliskParrot()
        {
            _locationChecker.AddCheckedLocation(AP_FARM_OBELISK_PARROT);
        }

        private static bool IsFarmObeliskParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_FARM_OBELISK_PARROT);
        }
        
        private static void AddMailboxParrot(IslandLocation islandWest)
        {
            islandWest.parrotUpgradePerches.Add(new ParrotUpgradePerchArchipelago(AP_MAILBOX_PARROT, _archipelago, islandWest,
                new Point(81, 40),
                new Rectangle(80, 39, 3, 2), 
                5,
                PurchaseMailboxParrot,
                IsMailboxParrotPurchased,
                "House_Mailbox",
                "Island_UpgradeHouse"));
        }

        private static void PurchaseMailboxParrot()
        {
            _locationChecker.AddCheckedLocation(AP_MAILBOX_PARROT);
        }

        private static bool IsMailboxParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_MAILBOX_PARROT);
        }
        
        private static void AddFarmhouseParrot(IslandLocation islandWest)
        {
            islandWest.parrotUpgradePerches.Add(new ParrotUpgradePerchArchipelago(AP_FARMHOUSE_PARROT, _archipelago, islandWest,
                new Point(81, 40),
                new Rectangle(74, 36, 7, 4),
                20,
                PurchaseFarmhouseParrot,
                IsFarmhouseParrotPurchased,
                "House"));
        }

        private static void PurchaseFarmhouseParrot()
        {
            _locationChecker.AddCheckedLocation(AP_FARMHOUSE_PARROT);
        }

        private static bool IsFarmhouseParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_FARMHOUSE_PARROT);
        }
        
        private static void AddParrotExpressParrot(IslandLocation islandWest)
        {
            islandWest.parrotUpgradePerches.Add(new ParrotUpgradePerchArchipelago(AP_PARROT_EXPRESS_PARROT, _archipelago, islandWest,
                new Point(72, 10),
                new Rectangle(73, 5, 3, 5),
                10,
                PurchaseParrotExpressParrot,
                IsParrotExpressParrotPurchased,
                "ParrotPlatforms"));
        }

        private static void PurchaseParrotExpressParrot()
        {
            _locationChecker.AddCheckedLocation(AP_PARROT_EXPRESS_PARROT);
        }

        private static bool IsParrotExpressParrotPurchased()
        {
            return _locationChecker.IsLocationChecked(AP_PARROT_EXPRESS_PARROT);
        }
    }
}
