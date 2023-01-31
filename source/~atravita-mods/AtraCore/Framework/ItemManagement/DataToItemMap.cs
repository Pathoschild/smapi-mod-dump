/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Wrappers;

using CommunityToolkit.Diagnostics;

namespace AtraCore.Framework.ItemManagement;

/// <summary>
/// Handles looking up the id of an item by its name and type.
/// </summary>
public static class DataToItemMap
{
    private static readonly SortedList<ItemTypeEnum, IAssetName> enumToAssetMap = new(7);

    private static readonly SortedList<ItemTypeEnum, Lazy<Dictionary<string, int>>> nameToIDMap = new(8);

    private static Lazy<HashSet<int>> actuallyRings = new(() => GetAll(ItemTypeEnum.Ring).ToHashSet());

    /// <summary>
    /// Given an ItemType and a name, gets the id.
    /// </summary>
    /// <param name="type">type of the item.</param>
    /// <param name="name">name of the item.</param>
    /// <param name="resolveRecipesSeperately">Whether or not to ignore the recipe bit.</param>
    /// <returns>Integer ID, or -1 if not found.</returns>
    public static int GetID(ItemTypeEnum type, string name, bool resolveRecipesSeperately = false)
    {
        Guard.IsNotNullOrWhiteSpace(name, nameof(name));

        if (!resolveRecipesSeperately)
        {
            type &= ~ItemTypeEnum.Recipe;
        }
        if (type == ItemTypeEnum.ColoredSObject)
        {
            type = ItemTypeEnum.SObject;
        }
        if (nameToIDMap.TryGetValue(type, out Lazy<Dictionary<string, int>>? asset)
            && asset.Value.TryGetValue(name, out int id))
        {
            return id;
        }
        return -1;
    }

    /// <summary>
    /// If the SObject index is actually a ring.
    /// </summary>
    /// <param name="id">int id</param>
    /// <returns>true for rings, false otherwise.</returns>
    public static bool IsActuallyRing(int id) => actuallyRings.Value.Contains(id);

    /// <summary>
    /// Gets all indexes associated with an asset type.
    /// </summary>
    /// <param name="type">Asset type.</param>
    /// <returns>ienumerable of ints.</returns>
    /// <remarks>Use this to filter out weird duplicates and stuff.</remarks>
    public static IEnumerable<int> GetAll(ItemTypeEnum type)
        => nameToIDMap.TryGetValue(type, out Lazy<Dictionary<string, int>>? asset) ? asset.Value.Values : Enumerable.Empty<int>();

    /// <summary>
    /// Sets up various maps.
    /// </summary>
    /// <param name="helper">GameContentHelper.</param>
    internal static void Init(IGameContentHelper helper)
    {
        // Populate item-to-asset-enumToAssetMap.
        // Note: Rings are in ObjectInformation, because
        // nothing is nice. So are boots, but they have their own data asset as well.
        enumToAssetMap.Add(ItemTypeEnum.BigCraftable, helper.ParseAssetName(@"Data\BigCraftablesInformation"));
        enumToAssetMap.Add(ItemTypeEnum.Boots, helper.ParseAssetName(@"Data\Boots"));
        enumToAssetMap.Add(ItemTypeEnum.Clothing, helper.ParseAssetName(@"Data\ClothingInformation"));
        enumToAssetMap.Add(ItemTypeEnum.Furniture, helper.ParseAssetName(@"Data\Furniture"));
        enumToAssetMap.Add(ItemTypeEnum.Hat, helper.ParseAssetName(@"Data\hats"));
        enumToAssetMap.Add(ItemTypeEnum.SObject, helper.ParseAssetName(@"Data\ObjectInformation"));
        enumToAssetMap.Add(ItemTypeEnum.Weapon, helper.ParseAssetName(@"Data\weapons"));

        // load the lazies.
        Reset();
    }

    /// <summary>
    /// Resets the requested name-to-id maps.
    /// </summary>
    /// <param name="assets">Assets to reset, or null for all.</param>
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        bool ShouldReset(IAssetName name) => assets is null || assets.Contains(name);

        if (ShouldReset(enumToAssetMap[ItemTypeEnum.SObject]))
        {
            if (!nameToIDMap.TryGetValue(ItemTypeEnum.SObject, out Lazy<Dictionary<string, int>>? sobj) || sobj.IsValueCreated)
            {
                nameToIDMap[ItemTypeEnum.SObject] = new(() =>
                {
                    ModEntry.ModMonitor.DebugOnlyLog("Building map to resolve normal objects.", LogLevel.Info);

                    Dictionary<string, int> mapping = new(Game1Wrappers.ObjectInfo.Count)
                    {
                        // Special cases
                        ["Egg"] = 176,
                        ["Brown Egg"] = 180,
                        ["Large Egg"] = 174,
                        ["Large Brown Egg"] = 182,
                        ["Strange Doll"] = 126,
                        ["Strange Doll 2"] = 127,
                    };

                    // Processing from the data.
                    foreach ((int id, string data) in Game1Wrappers.ObjectInfo)
                    {
                        // category asdf should never end up in the player inventory.
                        ReadOnlySpan<char> cat = data.GetNthChunk('/', SObject.objectInfoTypeIndex);
                        if (cat.Equals("asdf", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        ReadOnlySpan<char> name = data.GetNthChunk('/', SObject.objectInfoNameIndex);
                        if (name.Equals("Stone", StringComparison.OrdinalIgnoreCase) && id != 390)
                        {
                            continue;
                        }
                        if (name.Equals("Weeds", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("SupplyCrate", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("Twig", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("Rotten Plant", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("Warp Totem: Qi's Arena", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("???", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("DGA Dummy Object", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("Egg", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("Large Egg", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("Strange Doll", StringComparison.OrdinalIgnoreCase)
                            || name.Equals("Lost Book", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        if (!mapping.TryAdd(name.ToString(), id))
                        {
                            ModEntry.ModMonitor.Log($"{name.ToString()} with {id} seems to be a duplicate SObject and may not be resolved correctly.", LogLevel.Warn);
                        }
                    }
                    return mapping;
                });
            }
            if (!nameToIDMap.TryGetValue(ItemTypeEnum.Ring, out Lazy<Dictionary<string, int>>? rings) || rings.IsValueCreated)
            {
                nameToIDMap[ItemTypeEnum.Ring] = new(() =>
                {
                    ModEntry.ModMonitor.DebugOnlyLog("Building map to resolve rings.", LogLevel.Info);

                    Dictionary<string, int> mapping = new(10);
                    foreach ((int id, string data) in Game1Wrappers.ObjectInfo)
                    {
                        ReadOnlySpan<char> cat = data.GetNthChunk('/', 3);

                        // wedding ring (801) isn't a real ring.
                        // JA rings are registered as "Basic -96"
                        if (id == 801 || (!cat.Equals("Ring", StringComparison.Ordinal) && !cat.Equals("Basic -96", StringComparison.Ordinal)))
                        {
                            continue;
                        }

                        string? name = data.GetNthChunk('/', SObject.objectInfoNameIndex).ToString();
                        if (!mapping.TryAdd(name, id))
                        {
                            ModEntry.ModMonitor.Log($"{name} with {id} seems to be a duplicate Ring and may not be resolved correctly.", LogLevel.Warn);
                        }
                    }
                    return mapping;
                });
            }
            if (actuallyRings.IsValueCreated)
            {
                actuallyRings = new(GetAll(ItemTypeEnum.Ring).ToHashSet());
            }
        }

        if (ShouldReset(enumToAssetMap[ItemTypeEnum.Boots])
            && (!nameToIDMap.TryGetValue(ItemTypeEnum.Boots, out Lazy<Dictionary<string, int>>? boots) || boots.IsValueCreated))
        {
            nameToIDMap[ItemTypeEnum.Boots] = new(() =>
            {
                ModEntry.ModMonitor.DebugOnlyLog("Building map to resolve Boots", LogLevel.Info);

                Dictionary<string, int> mapping = new(20);
                foreach ((int id, string data) in Game1.content.Load<Dictionary<int, string>>(enumToAssetMap[ItemTypeEnum.Boots].BaseName))
                {
                    string name = data.GetNthChunk('/', SObject.objectInfoNameIndex).ToString();
                    if (!mapping.TryAdd(name, id))
                    {
                        ModEntry.ModMonitor.Log($"{name} with {id} seems to be a duplicate Boots and may not be resolved correctly.", LogLevel.Warn);
                    }
                }
                return mapping;
            });
        }
        if (ShouldReset(enumToAssetMap[ItemTypeEnum.BigCraftable])
            && (!nameToIDMap.TryGetValue(ItemTypeEnum.BigCraftable, out Lazy<Dictionary<string, int>>? bc) || bc.IsValueCreated))
        {
            nameToIDMap[ItemTypeEnum.BigCraftable] = new(() =>
            {
                ModEntry.ModMonitor.DebugOnlyLog("Building map to resolve BigCraftables", LogLevel.Info);

                Dictionary<string, int> mapping = new(Game1.bigCraftablesInformation.Count)
                {
                    // special cases
                    ["House Plant"] = 0,
                    ["Tub o' Flowers"] = Game1.currentSeason.Equals("winter") || Game1.currentSeason.Equals("fall") ? 109 : 108,
                    ["Rarecrow 1"] = 110,
                    ["Rarecrow 2"] = 113,
                    ["Rarecrow 3"] = 126,
                    ["Rarecrow 4"] = 136,
                    ["Rarecrow 5"] = 137,
                    ["Rarecrow 6"] = 138,
                    ["Rarecrow 7"] = 139,
                    ["Rarecrow 8"] = 140,
                    ["Seasonal Plant"] = 184,
                    ["Seasonal Plant 1"] = 188,
                    ["Seasonal Plant 2"] = 192,
                    ["Seasonal Plant 3"] = 196,
                    ["Seasonal Plant 4"] = 200,
                    ["Seasonal Plant 5"] = 204,
                };

                // House plants :P
                for (int i = 1; i <= 7; i++)
                {
                    mapping["House Plant " + i.ToString()] = i;
                }
                foreach ((int id, string data) in Game1.bigCraftablesInformation)
                {
                    ReadOnlySpan<char> nameSpan = data.GetNthChunk('/', SObject.objectInfoNameIndex);
                    if (nameSpan.Equals("House Plant", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Wood Chair", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Door", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Locked Door", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Tub o' Flowers", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Seasonal Plant", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Rarecrow", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Crate", StringComparison.OrdinalIgnoreCase)
                        || nameSpan.Equals("Barrel", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string name = nameSpan.ToString();
                    if (!mapping.TryAdd(name, id))
                    {
                        ModEntry.ModMonitor.Log($"{name} with {id} seems to be a duplicate BigCraftable and may not be resolved correctly.", LogLevel.Warn);
                    }
                }
                return mapping;
            });
        }
        if (ShouldReset(enumToAssetMap[ItemTypeEnum.Clothing])
            && (!nameToIDMap.TryGetValue(ItemTypeEnum.Clothing, out Lazy<Dictionary<string, int>>? clothing) || clothing.IsValueCreated))
        {
            nameToIDMap[ItemTypeEnum.Clothing] = new(() =>
            {
                ModEntry.ModMonitor.DebugOnlyLog("Building map to resolve Clothing", LogLevel.Info);

                Dictionary<string, int> mapping = new(Game1.clothingInformation.Count)
                {
                    ["Prismatic Shirt"] = 1999,
                    ["Dark Prismatic Shirt"] = 1998,
                };

                foreach ((int id, string data) in Game1.clothingInformation)
                {
                    ReadOnlySpan<char> nameSpan = data.GetNthChunk('/', SObject.objectInfoNameIndex);
                    if (nameSpan.Equals("Prismatic Shirt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string name = nameSpan.ToString();
                    if (!mapping.TryAdd(name, id))
                    {
                        ModEntry.ModMonitor.Log($"{name} with {id} seems to be a duplicate ClothingItem and may not be resolved correctly.", LogLevel.Warn);
                    }
                }
                return mapping;
            });
        }
        if (ShouldReset(enumToAssetMap[ItemTypeEnum.Furniture])
            && (!nameToIDMap.TryGetValue(ItemTypeEnum.Furniture, out Lazy<Dictionary<string, int>>? furniture) || furniture.IsValueCreated))
        {
            nameToIDMap[ItemTypeEnum.Furniture] = new(() =>
            {
                ModEntry.ModMonitor.DebugOnlyLog("Building map to resolve Furniture", LogLevel.Info);

                Dictionary<string, int> mapping = new(300);
                foreach ((int id, string data) in Game1.content.Load<Dictionary<int, string>>(enumToAssetMap[ItemTypeEnum.Furniture].BaseName))
                {
                    string name = data.GetNthChunk('/', SObject.objectInfoNameIndex).ToString();
                    if (!mapping.TryAdd(name, id))
                    {
                        ModEntry.ModMonitor.Log($"{name} with {id} seems to be a duplicate Furniture Item and may not be resolved correctly.", LogLevel.Warn);
                    }
                }
                return mapping;
            });
        }
        if (ShouldReset(enumToAssetMap[ItemTypeEnum.Hat])
            && (!nameToIDMap.TryGetValue(ItemTypeEnum.Hat, out Lazy<Dictionary<string, int>>? hats) || hats.IsValueCreated))
        {
            nameToIDMap[ItemTypeEnum.Hat] = new(() =>
            {
                ModEntry.ModMonitor.DebugOnlyLog("Building map to resolve Hats", LogLevel.Info);

                Dictionary<string, int> mapping = new(100);

                foreach ((int id, string data) in Game1.content.Load<Dictionary<int, string>>(enumToAssetMap[ItemTypeEnum.Hat].BaseName))
                {
                    ReadOnlySpan<char> nameSpan = data.GetNthChunk('/', SObject.objectInfoNameIndex);

                    string name = nameSpan.ToString();
                    if (!mapping.TryAdd(name, id))
                    {
                        ModEntry.ModMonitor.Log($"{name} with {id} seems to be a duplicate Hat and may not be resolved correctly.", LogLevel.Warn);
                    }
                }
                return mapping;
            });
        }
    }
}
