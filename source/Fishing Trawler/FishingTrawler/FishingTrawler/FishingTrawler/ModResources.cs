/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Objects.Rewards;
using FishingTrawler.Objects.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingTrawler
{
    public static class ModResources
    {
        internal static string assetFolderPath;
        internal static string murphyTexturePath;
        internal static string murphyDialoguePath;

        internal static Texture2D ancientFlagsTexture;
        internal static Texture2D anglerRingTexture;
        internal static Texture2D murphyPortraitTexture;
        internal static Texture2D boatTexture;
        internal static Texture2D bucketTexture;
        internal static Texture2D uiTexture;

        internal static void SetUpAssets(IModHelper helper)
        {
            assetFolderPath = helper.Content.GetActualAssetKey("assets", ContentSource.ModFolder);
            murphyTexturePath = Path.Combine(assetFolderPath, "Characters", "Murphy.png");
            murphyDialoguePath = Path.Combine(assetFolderPath, "Characters", "Dialogue", "Murphy.json");

            // Load in textures
            murphyPortraitTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "Characters", "MurphyPortrait.png"));
            boatTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "Objects", "Trawler.png"));
            ancientFlagsTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "Objects", "Flags.png"));
            anglerRingTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "Objects", "AnglerRing.png"));
            bucketTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "Objects", "BailingBucket.png"));
            uiTexture = helper.Content.Load<Texture2D>(Path.Combine(assetFolderPath, "UI", "TrawlerUI.png"));

            // Set any static Texture assets
            AncientFlag.flagTexture = ancientFlagsTexture;
            AnglerRing.ringTexture = anglerRingTexture;
        }

        internal static void ConvertCustomItemsToBase(GameLocation location)
        {
            foreach (Furniture customFurniture in location.furniture.ToList())
            {
                if (IsItemCustom(customFurniture))
                {
                    if (customFurniture.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY))
                    {
                        location.furniture.Remove(customFurniture);
                        location.furniture.Add(new Furniture(1900, customFurniture.TileLocation) { modData = customFurniture.modData });
                        continue;
                    }

                    ModEntry.monitor.Log($"Custom furniture {customFurniture.Name} at {location.NameOrUniqueName} failed to convert back to base furniture. Serializer will likely fail.", LogLevel.Trace);
                }
            }

            // Handle digging through chests to see if modded item is stored inside
            foreach (var tileToObject in location.Objects.Pairs)
            {
                if (tileToObject.Value is Chest)
                {
                    Chest chest = tileToObject.Value as Chest;
                    foreach (Item customItem in chest.items.Where(i => i != null && IsItemCustom(i)).ToList())
                    {
                        switch (customItem)
                        {
                            case BailingBucket bucket:
                                chest.items.Remove(customItem);
                                chest.items.Add(new MilkPail() { modData = customItem.modData });
                                break;
                            case Furniture furniture:
                                if (furniture.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY))
                                {
                                    chest.items.Remove(customItem);
                                    chest.items.Add(new Furniture(1900, Vector2.Zero) { modData = customItem.modData });
                                }
                                break;
                            default:
                                ModEntry.monitor.Log($"Custom item {customItem.Name} at {location.NameOrUniqueName} within chest {chest.Name} failed to convert back to base item. Serializer will likely fail.", LogLevel.Trace);
                                continue;
                        }
                    }
                }
            }
        }

        internal static void ConvertBaseItemsToCustom(GameLocation location)
        {
            foreach (Furniture baseFurniture in location.furniture.ToList())
            {
                if (IsItemCustom(baseFurniture))
                {
                    if (baseFurniture.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY))
                    {
                        if (!System.Enum.TryParse(baseFurniture.modData[ModEntry.ANCIENT_FLAG_KEY], out FlagType flagType))
                        {
                            ModEntry.monitor.Log($"Failed to convert base furniture {baseFurniture.Name} at {location.NameOrUniqueName} back into an Ancient Flag: Unable to determine FlagType.", LogLevel.Trace);
                            break;
                        }

                        location.furniture.Remove(baseFurniture);
                        location.furniture.Add(new AncientFlag(flagType, baseFurniture.TileLocation) { modData = baseFurniture.modData });
                        continue;
                    }

                    ModEntry.monitor.Log($"Base furniture {baseFurniture.Name} at {location.NameOrUniqueName} failed to convert to custom furniture.", LogLevel.Trace);
                }
            }
            foreach (var tileToObject in location.Objects.Pairs)
            {
                if (IsItemCustom(tileToObject.Value))
                {
                    Vector2 tile = tileToObject.Key;
                    Object baseObject = tileToObject.Value;
                    switch (baseObject)
                    {
                        case Furniture furniture:
                            if (furniture.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY))
                            {
                                if (!System.Enum.TryParse(furniture.modData[ModEntry.ANCIENT_FLAG_KEY], out FlagType flagType))
                                {
                                    ModEntry.monitor.Log($"Failed to convert placed object {baseObject.Name} at {location.NameOrUniqueName} back into an Ancient Flag: Unable to determine FlagType.", LogLevel.Trace);
                                    break;
                                }

                                location.Objects.Remove(tile);
                                location.Objects.Add(tile, new AncientFlag(flagType, tile) { modData = baseObject.modData });
                            }
                            break;
                        default:
                            ModEntry.monitor.Log($"Base object {baseObject.Name} at {location.NameOrUniqueName} failed to convert to custom item.", LogLevel.Trace);
                            continue;
                    }
                }

                // Handle digging through chests to see if modded item is stored inside
                if (tileToObject.Value is Chest)
                {
                    Chest chest = tileToObject.Value as Chest;
                    foreach (Item baseItem in chest.items.Where(i => i != null && IsItemCustom(i)).ToList())
                    {
                        switch (baseItem)
                        {
                            case MilkPail pail:
                                chest.items.Remove(baseItem);
                                chest.items.Add(new BailingBucket() { modData = baseItem.modData });
                                break;
                            case Furniture furniture:
                                if (furniture.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY))
                                {
                                    if (!System.Enum.TryParse(furniture.modData[ModEntry.ANCIENT_FLAG_KEY], out FlagType flagType))
                                    {
                                        ModEntry.monitor.Log($"Failed to convert {baseItem.Name} at {location.NameOrUniqueName} within chest {chest.Name} back into an Ancient Flag: Unable to determine FlagType.", LogLevel.Trace);
                                        break;
                                    }

                                    chest.items.Remove(baseItem);
                                    chest.items.Add(new AncientFlag(flagType) { modData = baseItem.modData });
                                }
                                break;
                            default:
                                ModEntry.monitor.Log($"Base item {baseItem.Name} at {location.NameOrUniqueName} within chest {chest.Name} failed to convert back to custom item.", LogLevel.Trace);
                                continue;
                        }
                    }
                }
            }
        }

        internal static void ConvertInventoryCustomItemsToBase(Farmer who)
        {
            foreach (Item customItem in who.items.Where(i => i != null && IsItemCustom(i)).ToList())
            {
                switch (customItem)
                {
                    case BailingBucket bucket:
                        who.removeItemFromInventory(customItem);
                        who.addItemToInventory(new MilkPail() { modData = customItem.modData });
                        break;
                    case Furniture furniture:
                        if (furniture.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY))
                        {
                            who.removeItemFromInventory(customItem);
                            who.addItemToInventory(new Furniture(1900, Vector2.Zero) { modData = customItem.modData });
                        }
                        break;
                    default:
                        ModEntry.monitor.Log($"Custom item {customItem.Name} at {who.currentLocation.NameOrUniqueName} within player {who.Name} failed to convert back to base item. Serializer will likely fail.", LogLevel.Trace);
                        continue;
                }
            }
        }

        internal static void ConvertInventoryBaseItemsToCustom(Farmer who)
        {
            foreach (Item baseItem in who.items.Where(i => i != null && IsItemCustom(i)).ToList())
            {
                switch (baseItem)
                {
                    case MilkPail pail:
                        who.removeItemFromInventory(baseItem);
                        who.addItemToInventory(new BailingBucket() { modData = baseItem.modData });
                        break;
                    case Furniture furniture:
                        if (furniture.modData.ContainsKey(ModEntry.ANCIENT_FLAG_KEY))
                        {
                            if (!System.Enum.TryParse(furniture.modData[ModEntry.ANCIENT_FLAG_KEY], out FlagType flagType))
                            {
                                ModEntry.monitor.Log($"Failed to convert {baseItem.Name} at {who.currentLocation.NameOrUniqueName} within player {who.Name} back into an Ancient Flag: Unable to determine FlagType.", LogLevel.Trace);
                                break;
                            }

                            who.removeItemFromInventory(baseItem);
                            who.addItemToInventory(new AncientFlag(flagType) { modData = baseItem.modData });
                        }
                        break;
                    default:
                        ModEntry.monitor.Log($"Custom item {baseItem.Name} at {who.currentLocation.NameOrUniqueName} within player {who.Name} failed to convert back to base item. Serializer will likely fail.", LogLevel.Trace);
                        continue;
                }
            }
        }

        private static bool IsItemCustom(Item item)
        {
            return item.modData.Keys.Any(k => k.Contains("PeacefulEnd.FishingTrawler_"));
        }
    }
}
