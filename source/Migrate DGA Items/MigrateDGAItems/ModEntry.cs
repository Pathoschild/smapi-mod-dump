/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MigrateDGAItems
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using MigrateDGAItems.DGAClasses;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using SObject = StardewValley.Object;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using StardewValley.GameData.Fences;
using System.Xml.Linq;

namespace MigrateDGAItems
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }


        /*********
        ** Private methods
        *********/

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var spacecore = this.Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");

            if (spacecore is null)
            {
                Monitor.Log("No SpaceCore API found! Mod will not work!", LogLevel.Error);
            }
            else
            {
                spacecore.RegisterSerializerType(typeof(CustomObject));
                spacecore.RegisterSerializerType(typeof(CustomBasicFurniture));
                spacecore.RegisterSerializerType(typeof(CustomBedFurniture));
                spacecore.RegisterSerializerType(typeof(CustomTVFurniture));
                spacecore.RegisterSerializerType(typeof(CustomFishTankFurniture));
                spacecore.RegisterSerializerType(typeof(CustomStorageFurniture));
                spacecore.RegisterSerializerType(typeof(CustomCrop));
                spacecore.RegisterSerializerType(typeof(CustomGiantCrop));
                spacecore.RegisterSerializerType(typeof(CustomMeleeWeapon));
                spacecore.RegisterSerializerType(typeof(CustomBoots));
                spacecore.RegisterSerializerType(typeof(CustomHat));
                spacecore.RegisterSerializerType(typeof(CustomFence));
                spacecore.RegisterSerializerType(typeof(CustomBigCraftable));
                spacecore.RegisterSerializerType(typeof(CustomFruitTree));
                spacecore.RegisterSerializerType(typeof(CustomShirt));
                spacecore.RegisterSerializerType(typeof(CustomPants));
                Monitor.Log("Registered subclasses with SpaceCore!", LogLevel.Trace);
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Add a error item for debugging
            //Game1.player.addItemByMenuIfNecessary(new CustomObject());

            Utility.ForEachItem(fixItem);
            Utility.ForEachLocation(l => fixTerrainFeatures(l));

            // Debugging stuff to alert on furniture in the farmhouse
            //foreach (Furniture f in Game1.getLocationFromName("Farmhouse").furniture)
            //{
            //    Monitor.Log($"Furniture found in farmhouse with name {f.Name}", LogLevel.Trace);
            //}
        }

        private bool fixItem(Item item, Action remove, Action<Item> replaceWith)
        {
            // If it's a DGA furniture
            if (item is CustomBasicFurniture or CustomBedFurniture or CustomFishTankFurniture or CustomStorageFurniture or CustomTVFurniture)
            {
                // Type = (F)
                string locName = "unknown location";
                if (((Furniture)item).Location != null && ((Furniture)item).Location.Name != null)
                {
                    locName = ((Furniture)item).Location.Name;
                }
                Monitor.Log($"Error item found with name: {item.Name} in {locName}", LogLevel.Trace);
                string itemId = getBestItemGuess("(F)", item.Name);
                Furniture newItem = (Furniture)ItemRegistry.Create(itemId);
                if (item is CustomStorageFurniture or CustomFishTankFurniture && newItem is StorageFurniture)
                {
                    foreach (var heldThing in ((StorageFurniture)item).heldItems)
                    {
                        ((StorageFurniture)newItem).heldItems.Add(heldThing);
                    }
                }
                Monitor.Log($"Replacing {item.Name} with {newItem.QualifiedItemId}", LogLevel.Trace);
                replaceWith(newItem);
            }
            // If it's a DGA object
            else if (item is CustomObject)
            {
                // Type = (O)
                Monitor.Log($"Error item found with name: {item.Name}", LogLevel.Trace);
                string itemId = getBestItemGuess("(O)",item.Name);
                SObject newObject = (SObject)ItemRegistry.Create(itemId);
                Monitor.Log($"Replacing {item.Name} with {newObject.QualifiedItemId}", LogLevel.Trace);
                replaceWith(newObject);
            }
            // If it's a DGA big craftable
            else if (item is CustomBigCraftable)
            {
                // Type = (BC)
                Monitor.Log($"Error item found with name: {item.Name}", LogLevel.Trace);
                string itemId = getBestItemGuess("(BC)", item.Name);
                Monitor.Log($"Replacing {item.Name} with {ItemRegistry.Create(itemId).QualifiedItemId}", LogLevel.Trace);
                replaceWith(ItemRegistry.Create(itemId));
            }
            // If it's a DGA weapon
            else if (item is CustomMeleeWeapon)
            {
                // Type = (W)
                Monitor.Log($"Error item found with name: {item.Name}", LogLevel.Trace);
                string itemId = getBestItemGuess("(W)", item.Name);
                Monitor.Log($"Replacing {item.Name} with {ItemRegistry.Create(itemId).QualifiedItemId}", LogLevel.Trace);
                replaceWith(ItemRegistry.Create(itemId));
            }
            // If it's a DGA clothing item
            else if (item is CustomPants or CustomShirt or CustomHat or CustomBoots)
            {
                // Type = (P)
                Monitor.Log($"Error item found with name: {item.Name}", LogLevel.Trace);
                string itemId = getBestItemGuess("(P)", item.Name);
                Monitor.Log($"Replacing {item.Name} with {ItemRegistry.Create(itemId).QualifiedItemId}", LogLevel.Trace);
                replaceWith(ItemRegistry.Create(itemId));
            }
            else if (item is CustomShirt)
            {
                // Type = (S)
                Monitor.Log($"Error item found with name: {item.Name}", LogLevel.Trace);
                string itemId = getBestItemGuess("(S)", item.Name);
                Monitor.Log($"Replacing {item.Name} with {ItemRegistry.Create(itemId).QualifiedItemId}", LogLevel.Trace);
                replaceWith(ItemRegistry.Create(itemId));
            }
            else if (item is CustomHat)
            {
                // Type = (H)
                Monitor.Log($"Error item found with name: {item.Name}", LogLevel.Trace);
                string itemId = getBestItemGuess("(H)", item.Name);
                Monitor.Log($"Replacing {item.Name} with {ItemRegistry.Create(itemId).QualifiedItemId}", LogLevel.Trace);
                replaceWith(ItemRegistry.Create(itemId));
            }
            else if (item is CustomBoots)
            {
                // Type = (B)
                Monitor.Log($"Error item found with name: {item.Name}", LogLevel.Trace);
                string itemId = getBestItemGuess("(B)", item.Name);
                Monitor.Log($"Replacing {item.Name} with {ItemRegistry.Create(itemId).QualifiedItemId}", LogLevel.Trace);
                replaceWith(ItemRegistry.Create(itemId));
            }
            return true;
        }

        private bool fixTerrainFeatures(GameLocation l)
        {
            Dictionary<Vector2, SObject> fencesToAdd = new();
            Dictionary<Vector2, SObject> fencesToRemove = new();
            foreach (KeyValuePair<Vector2, SObject> pair in l.objects.Pairs)
            {
                if (pair.Value is CustomFence)
                {
                    // Fix the fence
                    fencesToRemove.Add(pair.Key, pair.Value);
                    string bestGuessId = getBestFenceGuess(((CustomFence)pair.Value).Name);
                    Fence newFence = new Fence(((CustomFence)pair.Value).TileLocation, bestGuessId, ((CustomFence)pair.Value).isGate);
                    fencesToAdd.Add(pair.Key, newFence);
                }
            }
            Dictionary<Vector2, TerrainFeature> terrainFeaturesToAdd = new();
            Dictionary<Vector2, TerrainFeature> terrainFeaturesToRemove = new();
            foreach (KeyValuePair<Vector2, TerrainFeature> pair in l.terrainFeatures.Pairs)
            {
                if (pair.Value is CustomFruitTree)
                {
                    // Replace the fruit tree properly
                }
                else if (pair.Value is CustomGiantCrop)
                {
                    // Replace the custom giant crop properly
                }
                else if (pair.Value is HoeDirt)
                {
                    HoeDirt hoeDirt = (HoeDirt)pair.Value;
                    if (hoeDirt.crop is not null && hoeDirt.crop is CustomCrop)
                    {
                        // replace the crop properly!
                    }
                }
            }
            return true;
        }

        private string getBestItemGuess(string type, string itemName)
        {
            // Do some fancy string splitting on the item's name, assuming DGA formatting
            string name = itemName.Split("/").Last();
            string packName = itemName.Split("/").First();
            string packNameWithoutDGA = packName.Split(".").First() + packName.Split(".").Last();

            // Build the dictionary of name to qualified item ID strings to search over
            IItemDataDefinition itemType = ItemRegistry.GetTypeDefinition(type);
            Dictionary<string, string> allItems = new Dictionary<string, string>();
            foreach (string itemId in itemType.GetAllIds())
            {
                ParsedItemData itemData = itemType.GetData(itemId);
                if (!allItems.ContainsKey(itemData.InternalName))
                {
                    allItems[itemData.InternalName] = itemType.Identifier + itemId;
                }
            }

            // Check if itemName exactly is there
            if (allItems.Keys.Contains(itemName))
            {
                return allItems[itemName];
            }
            // Check it {{ModId}}_ItemName is there
            if (allItems.Keys.Contains(packName + "_" + name))
            {
                return allItems[packName + "_" + name];
            }
            // Check it {{ModId}}.ItemName is there
            if (allItems.Keys.Contains(packName + "." + name))
            {
                return allItems[packName + "." + name];
            }
            // Check it {{ModId}}_ItemName is there without DGA
            if (allItems.Keys.Contains(packNameWithoutDGA + "_" + name))
            {
                return allItems[packNameWithoutDGA + "_" + name];
            }
            // Check it {{ModId}}.ItemName is there without DGA
            if (allItems.Keys.Contains(packNameWithoutDGA + "." + name))
            {
                return allItems[packNameWithoutDGA + "." + name];
            }

            // Try the stupid thing of just fuzzy searching on the whole name
            string fuzzyResult = Utility.fuzzySearch(itemName, allItems.Keys);
            if (fuzzyResult is not null)
            {
                return allItems[fuzzyResult];
            }

            // Try searching for the item name generated by {{ModId}}_ItemName
            fuzzyResult = Utility.fuzzySearch(packName + "_" + name, allItems.Keys);
            if (fuzzyResult is not null)
            {
                return allItems[fuzzyResult];
            }

            // Try searching for the item name generated by {{ModId}}.ItemName
            fuzzyResult = Utility.fuzzySearch(packName + "." + name, allItems.Keys);
            if (fuzzyResult is not null)
            {
                return allItems[fuzzyResult];
            }

            // Try searching for the item name generated by {{ModId}}_ItemName without DGA
            fuzzyResult = Utility.fuzzySearch(packNameWithoutDGA + "_" + name, allItems.Keys);
            if (fuzzyResult is not null)
            {
                return allItems[fuzzyResult];
            }

            // Try searching for the item name generated by {{ModId}}.ItemName without DGA
            fuzzyResult = Utility.fuzzySearch(packNameWithoutDGA + "." + name, allItems.Keys);
            if (fuzzyResult is not null)
            {
                return allItems[fuzzyResult];
            }

            return type + itemName;
        }

        private string getBestFenceGuess(string originalName)
        {
            // Do some fancy string splitting on the item's name, assuming DGA formatting
            string name = originalName.Split("/").Last();
            string packName = originalName.Split("/").First();
            string packNameWithoutDGA = packName.Split(".").First() + packName.Split(".").Last();

            if (Fence.TryGetData(originalName, out _))
            {
                return originalName;
            }
            else if (Fence.TryGetData(packName + "_" + name, out _))
            {
                return packName + "_" + name;
            }
            else if (Fence.TryGetData(packName + "_" + name, out _))
            {
                return packName + "." + name;
            }
            else if (Fence.TryGetData(packName + "_" + name, out _))
            {
                return packNameWithoutDGA + "_" + name;
            }
            else if (Fence.TryGetData(packName + "_" + name, out _))
            {
                return packNameWithoutDGA + "." + name;
            }
            return "-1";
        }

        private string getBestFruitTreeGuess(string originalName)
        {
            // Do some fancy string splitting on the item's name, assuming DGA formatting
            string name = originalName.Split("/").Last();
            string packName = originalName.Split("/").First();
            string packNameWithoutDGA = packName.Split(".").First() + packName.Split(".").Last();

            if (FruitTree.TryGetData(originalName, out _))
            {
                return originalName;
            }
            else if (FruitTree.TryGetData(packName + "_" + name, out _))
            {
                return packName + "_" + name;
            }
            else if (FruitTree.TryGetData(packName + "_" + name, out _))
            {
                return packName + "." + name;
            }
            else if (FruitTree.TryGetData(packName + "_" + name, out _))
            {
                return packNameWithoutDGA + "_" + name;
            }
            else if (FruitTree.TryGetData(packName + "_" + name, out _))
            {
                return packNameWithoutDGA + "." + name;
            }
            return "-1";
        }
    }
}