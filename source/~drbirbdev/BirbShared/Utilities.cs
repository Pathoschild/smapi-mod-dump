/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using BirbShared.APIs;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

namespace BirbShared
{
    internal class Utilities
    {
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
                    return new StardewValley.Object(168, 1);
                }
                if (dynamicGameAssets == null && parts[0].StartsWith("dga_"))
                {
                    Log.Error($"Tried to parse DynamicGameAssets trash drop but mod isn't loaded {id}");
                    return new StardewValley.Object(168, 1);
                }

                int externalId = -1;
                switch (itemType)
                {
                    case "item":
                        return new StardewValley.Object(int.Parse(itemId), itemCount);
                    case "bigcraftable":
                        return new StardewValley.Object(Vector2.Zero, int.Parse(itemId));
                    case "bedfurniture":
                        return new BedFurniture(int.Parse(itemId), Vector2.Zero);
                    case "boots":
                        return new Boots(int.Parse(itemId));
                    case "clothing":
                        return new Clothing(int.Parse(itemId));
                    case "furniture":
                        return new Furniture(int.Parse(itemId), Vector2.Zero);
                    case "hat":
                        return new Hat(int.Parse(itemId));
                    case "ring":
                        return new Ring(int.Parse(itemId));
                    case "storagefurniture":
                        return new StorageFurniture(int.Parse(itemId), Vector2.Zero);
                    case "weapon":
                        return new MeleeWeapon(int.Parse(itemId));
                    case "ja_item":
                        externalId = jsonAssets.GetObjectId(itemId);
                        if (externalId < 0)
                        {
                            Log.Error($"Tried to parse JsonAssets trash drop, but item was unknown {id}");
                            return new StardewValley.Object(168, 1);
                        }
                        return new StardewValley.Object(externalId, itemCount);
                    case "ja_bigcraftable":
                        externalId = jsonAssets.GetBigCraftableId(itemId);
                        if (externalId < 0)
                        {
                            Log.Error($"Tried to parse JsonAssets trash drop, but item was unknown {id}");
                            return new StardewValley.Object(168, 1);
                        }
                        return new StardewValley.Object(Vector2.Zero, externalId);
                    case "ja_hat":
                        externalId = jsonAssets.GetHatId(itemId);
                        if (externalId < 0)
                        {
                            Log.Error($"Tried to parse JsonAssets trash drop, but item was unknown {id}");
                            return new StardewValley.Object(168, 1);
                        }
                        return new Hat(externalId);
                    case "ja_weapon":
                        externalId = jsonAssets.GetWeaponId(itemId);
                        if (externalId < 0)
                        {
                            Log.Error($"Tried to parse JsonAssets trash drop, but item was unknown {id}");
                            return new StardewValley.Object(168, 1);
                        }
                        return new MeleeWeapon(externalId);
                    case "ja_clothing":
                        externalId = jsonAssets.GetClothingId(itemId);
                        if (externalId < 0)
                        {
                            Log.Error($"Tried to parse JsonAssets trash drop, but item was unknown {id}");
                            return new StardewValley.Object(168, 1);
                        }
                        return new Clothing(externalId);
                    case "dga_item":
                        object dgaItem = dynamicGameAssets.SpawnDGAItem(itemId);
                        if (dgaItem != null && dgaItem is Item item)
                        {
                            item.Stack = itemCount;
                            return item;
                        }
                        Log.Error($"Tried to parse DynamicGameAssets trash drop, but item was unknown {id}");
                        return new StardewValley.Object(168, 1);

                    default:
                        Log.Error($"Failed to parse drop type {id}");
                        return new StardewValley.Object(168, 1);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to parse drop id {id}\n{ex}");
                return new StardewValley.Object(168, 1);
            }
        }
    }
}
