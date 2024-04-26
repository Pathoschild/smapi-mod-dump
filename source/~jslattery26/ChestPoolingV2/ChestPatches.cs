/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jslattery26/stardew_mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

#nullable enable
namespace ChestPoolingV2
{
    public class ChestPatches
    {
        public static IMonitor? StaticMonitor { get; private set; }
        public static void Log(string s, LogLevel l = LogLevel.Trace) => StaticMonitor?.Log(s, l);

        public static void Initialize(IMonitor monitor)
        {

            StaticMonitor = monitor;
        }

        public static List<Chest>? GetChests()
        {
            if (!Game1.hasLoadedGame || Game1.currentLocation == null)
                return null;

            Farm farm = Game1.getFarm();

            List<Chest> chestList =
            [
                //get chests
                .. Game1.locations
                    .Where(location => location != null)
                    .SelectMany(location => location.Objects.Values.OfType<Chest>()),
                //get fridge
                .. Game1.locations
                    .OfType<FarmHouse>()
                    .Select(house => house.fridge.Value)
                    .Where(fridge => fridge != null),
                //fridge
                ..farm.buildings
                    .Select(building => building.indoors.Value)
                    .Where(indoors => indoors != null)
                    .SelectMany(indoors => indoors.Objects.Values.OfType<Chest>()),

            ];

            return chestList;
        }
        public static bool Chest_grabItemFromInventory_Prefix(Chest __instance, Item item, Farmer who)
        {

            Chest chest = __instance;

            List<Chest>? chestList = GetChests();
            if (chestList == null)
                return true;



            Log($"Item removed: {item.Name} - {item.Quality} - {item.QualifiedItemId} - {item.itemId}");

            if (ChestPoolingV2Mod.ModInstance?.Disabled == true)
            {
                Log("Chest Pooling is disabled");
                return true;
            }
            if (chest.specialChestType.Value == Chest.SpecialChestTypes.MiniShippingBin)
            {
                Log("Mini shipping bin, skipping");
                return true;
            }

            if (chest.ChestAlreadyHasItems(item))
            {
                Log("Already has this item");
                return true;
            }
            if (!chestList.Any(c => c != chest && c.ChestAlreadyHasItems(item)))
            {
                Log("No other chest has this item");
                return true;
            }

            return ChestPoolingV2Mod.SearchForBestChest(chest, chestList, item, who);
        }
    }
}