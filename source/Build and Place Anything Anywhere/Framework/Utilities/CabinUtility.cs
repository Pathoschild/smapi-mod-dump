/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;

namespace AnythingAnywhere.Framework.Utilities;
internal static class CabinUtility
{
    public static List<KeyValuePair<string, string>>? GetCabinsToUpgrade(bool toRenovate = false)
    {
        if (!Game1.IsMasterGame)
            return null;

        var cabins = GetCabins();
        List<KeyValuePair<string, string>> cabinPageNames = [];

        foreach (var cabin in cabins)
        {
            bool shouldAddCabin = toRenovate ? cabin.owner.HouseUpgradeLevel >= 2 : cabin.owner.HouseUpgradeLevel < 3;
            bool isUpgrading = cabin.owner.daysUntilHouseUpgrade.Value > 0;

            if (!shouldAddCabin || isUpgrading || cabin.owner.isActive()) continue;

            string msg = Game1.content.LoadString("Strings\\Buildings:Cabin_Name");
            msg = string.IsNullOrEmpty(cabin.owner.Name) ? $"Empty {msg}" : $"{cabin.owner.displayName}'s {msg}";
            cabinPageNames.Add(new KeyValuePair<string, string>(cabin.uniqueName.Value, msg));
        }

        return cabinPageNames;
    }

    public static bool HasCabinsToUpgrade(bool toRenovate = false)
    {
        var cabinsToUpgrade = GetCabinsToUpgrade(toRenovate);
        return cabinsToUpgrade is { Count: > 0 };
    }

    public static List<Cabin> GetCabins()
    {
        List<Cabin> cabins = [];

        foreach (var location in Game1.locations)
        {
            var locationCabins = location.buildings.Where(building => building.isCabin);
            cabins.AddRange(locationCabins.Select(cabin => (Cabin)cabin.indoors.Value));
        }

        return cabins;
    }
}