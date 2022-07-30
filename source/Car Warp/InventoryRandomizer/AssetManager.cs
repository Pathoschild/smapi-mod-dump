/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace InventoryRandomizer;

internal static class AssetManager
{
    internal static readonly List<string> SupportedAssets = new()
    {
        "Data\\BigCraftablesInformation",
        "Data\\Boots",
        "Data\\ClothingInformation",
        "Data\\Furniture",
        "Data\\hats",
        "Data\\ObjectInformation",
        "Data\\weapons",
        "Data\\CraftingRecipes",
        "Data\\CookingRecipes"
    };

    internal static readonly List<int> ObjectIdBlacklist = new()
    {
        0, 2, 4, 75, 76, 77, 79, 95, 290, 294, 295, 313, 314, 315, 316, 317, 318, 319, 320, 321, 449, 450, 452, 668, 670,
        674, 675, 676, 677, 678, 679, 747, 748, 750, 751, 760, 762, 764, 765, 784, 785, 786, 792, 793, 794, 816,
        817, 818, 819, 842, 843, 844, 845, 846, 847, 849, 850, 880, 882, 883, 884, 892, 922, 923, 924, 925, 929, 930
    };

    internal static readonly List<int> BigCraftableIdBlacklist = new()
    {
        118, 119, 120, 121, 122, 123, 124, 125, 174, 175
    };

    internal static Dictionary<int, string> CachedBigCraftablesInfo = new();
    internal static Dictionary<int, string> CachedBoots = new();
    internal static Dictionary<int, string> CachedClothingInfo = new();
    internal static Dictionary<int, string> CachedFurniture = new();
    internal static Dictionary<int, string> CachedHats = new();
    internal static Dictionary<int, string> CachedObjectInfo = new();
    internal static Dictionary<int, string> CachedWeapons = new();
    internal static Dictionary<string, string> CachedRecipes = new();

    internal static int CachedBigCraftablesCount;
    internal static int CachedBootsCount;
    internal static int CachedClothingCount;
    internal static int CachedFurnitureCount;
    internal static int CachedHatsCount;
    internal static int CachedObjectsCount;
    internal static int CachedWeaponsCount;

    /// <summary>
    ///     Reload all data assets and cache the results.
    /// </summary>
    internal static void ReloadObjectData()
    {
        ReloadBigCraftables();
        ReloadBoots();
        ReloadClothing();
        ReloadFurniture();
        ReloadHats();
        ReloadObjects();
        ReloadWeapons();
        ReloadRecipes();
    }

    /// <summary>
    ///     Reload the given data asset and cache the result.
    /// </summary>
    /// <param name="baseName">Name of the asset to reload.</param>
    internal static void ReloadObjectData(string baseName)
    {
        switch (baseName)
        {
            case "Data\\BigCraftablesInformation":
                ReloadBigCraftables();
                return;

            case "Data\\Boots":
                ReloadBoots();
                return;

            case "Data\\ClothingInformation":
                ReloadClothing();
                return;

            case "Data\\Furniture":
                ReloadFurniture();
                return;

            case "Data\\hats":
                ReloadHats();
                return;

            case "Data\\ObjectInformation":
                ReloadObjects();
                return;

            case "Data\\weapons":
                ReloadWeapons();
                return;

            case "Data\\CraftingRecipes" or "Data\\CookingRecipes":
                ReloadRecipes();
                return;
        }
    }

    private static void ReloadBigCraftables()
    {
        CachedBigCraftablesInfo = Game1.content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation")
            .Where(kvp => !BigCraftableIdBlacklist.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        CachedBigCraftablesCount = CachedBigCraftablesInfo.Count;
    }

    private static void ReloadBoots()
    {
        CachedBoots = Game1.content.Load<Dictionary<int, string>>("Data\\Boots");
        CachedBootsCount = CachedBoots.Count;
    }

    private static void ReloadClothing()
    {
        CachedClothingInfo = Game1.content.Load<Dictionary<int, string>>("Data\\ClothingInformation");
        CachedClothingCount = CachedClothingInfo.Count;
    }

    private static void ReloadFurniture()
    {
        CachedFurniture = Game1.content.Load<Dictionary<int, string>>("Data\\Furniture");
        CachedFurnitureCount = CachedFurniture.Count;
    }

    private static void ReloadHats()
    {
        CachedHats = Game1.content.Load<Dictionary<int, string>>("Data\\hats");
        CachedHatsCount = CachedHats.Count;
    }

    private static void ReloadObjects()
    {
        CachedObjectInfo = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation")
            .Where(kvp => !ObjectIdBlacklist.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        CachedObjectsCount = CachedObjectInfo.Count;
    }

    private static void ReloadWeapons()
    {
        CachedWeapons = Game1.content.Load<Dictionary<int, string>>("Data\\weapons");
        CachedWeaponsCount = CachedWeapons.Count;
    }

    private static void ReloadRecipes()
    {
        CachedRecipes = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes")
            .Concat(Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes"))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public static bool AssetIsSupported(string assetName)
    {
        return SupportedAssets.Contains(assetName);
    }
}
