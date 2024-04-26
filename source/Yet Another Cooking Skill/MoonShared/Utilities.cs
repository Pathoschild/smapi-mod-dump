/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using MoonShared.APIs;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace MoonShared
{
    public class Utilities
    {
        public static int GetIntData(Item item, string key, int defaultValue = 0)
        {
            if (!item.modData.TryGetValue(key, out string value) || !int.TryParse(value, out int intValue))
            {
                intValue = defaultValue;
            }
            return intValue;
        }

        public static int GetRarity(int[] chances)
        {
            Random random = new();
            int rarity = -1;
            for (int i = 0; i < chances.Length; i++)
            {
                if (random.Next(100) < chances[i])
                {
                    rarity++;
                }
                else
                {
                    return rarity;
                }
            }
            Log.Trace($"Using rarity {rarity}");
            return rarity;
        }

        public static string GetRandomDropStringFromLootTable(Dictionary<string, List<string>> table, string primary, string secondary, string tertiary)
        {
            Random random = new();

            List<string> possibleIds = null;
            foreach (string key in GetPossibleKeys(primary, secondary, tertiary))
            {
                if (table.ContainsKey(key))
                {
                    possibleIds = table[key];
                    Log.Trace($"Using key {key} to find possible loot");
                    break;
                }
            }
            if (possibleIds == null)
            {
                possibleIds = new List<string>() { "item.168" };
                Log.Error($"Found no specific or default drops for key {primary}.{secondary}.{tertiary}");

            }

            return possibleIds[random.Next(possibleIds.Count)];
        }

        internal static IEnumerable<string> GetPossibleKeys(string primary, string secondary, string tertiary)
        {
            yield return $"{primary}.{secondary}.{tertiary}";
            yield return $"{primary}.{secondary}.*";
            yield return $"{primary}.*.{tertiary}";
            yield return $"{primary}.*.*";
            yield return $"*.{secondary}.{tertiary}";
            yield return $"*.{secondary}.*";
            yield return $"*.*.{tertiary}";
            yield return $"*.*.*";
        }

        public static Item ParseDropString(string id, IJsonAssetsApi jsonAssets = null, IDynamicGameAssetsApi dynamicGameAssets = null)
        {
            try
            {
                string[] parts = id.Split('.');
                string itemType = parts[0];
                string itemId = parts[1];
                int itemCount = 1;
                if (parts.Length > 2)
                {
                    itemCount = int.Parse(parts[2]);
                }

                if (jsonAssets == null && parts[0].StartsWith("ja_"))
                {
                    Log.Error($"Tried to parse JsonAssets trash drop but mod isn't loaded {id}");
                    return new StardewValley.Object("(O)168", 1);
                }
                if (dynamicGameAssets == null && parts[0].StartsWith("dga_"))
                {
                    Log.Error($"Tried to parse DynamicGameAssets trash drop but mod isn't loaded {id}");
                    return new StardewValley.Object("(O)168", 1);
                }

                string externalId = "(O)168";
                switch (itemType)
                {
                    case "item":
                        return new StardewValley.Object(itemId, itemCount);
                    case "bigcraftable":
                        return new StardewValley.Object(Vector2.Zero, itemId);
                    case "bedfurniture":
                        return new BedFurniture(itemId, Vector2.Zero);
                    case "boots":
                        return new Boots(itemId);
                    case "clothing":
                        return new Clothing(itemId);
                    case "furniture":
                        return new Furniture(itemId, Vector2.Zero);
                    case "hat":
                        return new Hat(itemId);
                    case "ring":
                        return new Ring(itemId);
                    case "storagefurniture":
                        return new StorageFurniture(itemId, Vector2.Zero);
                    case "weapon":
                        return new MeleeWeapon(itemId);
                    case "ja_item":
                        externalId = itemId;
                        return new StardewValley.Object(externalId, itemCount);
                    case "ja_bigcraftable":
                        externalId = itemId;
                        return new StardewValley.Object(Vector2.Zero, externalId);
                    case "ja_hat":
                        externalId = itemId;
                        return new Hat(externalId);
                    case "ja_weapon":
                        externalId = itemId;
                        return new MeleeWeapon(externalId);
                    case "ja_clothing":
                        externalId = itemId;
                        return new Clothing(externalId);
                    case "dga_item":
                        object dgaItem = dynamicGameAssets.SpawnDGAItem(itemId);
                        if (dgaItem != null && dgaItem is Item item)
                        {
                            item.Stack = itemCount;
                            return item;
                        }
                        Log.Error($"Tried to parse DynamicGameAssets trash drop, but item was unknown {id}");
                        return new StardewValley.Object("(O)168", 1);

                    default:
                        Log.Error($"Failed to parse drop type {id}");
                        return new StardewValley.Object("(O)168", 1);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to parse drop id {id}\n{ex}");
                return new StardewValley.Object("(O)168", 1);
            }
        }
    }
}
