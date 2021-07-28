/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LukeSeewald/PublicStardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SObject = StardewValley.Object;

// Some functions are derived from code by Pathoschild: https://github.com/Pathoschild/StardewMods/blob/develop/LookupAnything/Framework/ItemScanning/WorldItemScanner.cs

namespace WhatAreYouMissing
{
    public class OwnedItems
    {
        /// <summary>Simplifies access to protected code.</summary>
        private readonly IReflectionHelper Reflection;

        //<parentSheetIndex, <quality, obj>>
        private Dictionary<int, Dictionary<int, SObject>> PlayerItems;
        private Dictionary<int, int> AmountOfItems;

        public OwnedItems(IReflectionHelper reflection)
        {
            Reflection = reflection;
            PlayerItems = new Dictionary<int, Dictionary<int, SObject>>();
            AmountOfItems = new Dictionary<int, int>();
            CondenseItems();
            FindAmountOfItems();
        }

        public Dictionary<int, Dictionary<int, SObject>> GetPlayerItems()
        {
            return PlayerItems;
        }

        public int GetTotalAmountOfItem(int parentSheetIndex)
        {
            return AmountOfItems.ContainsKey(parentSheetIndex) ? AmountOfItems[parentSheetIndex] : 0;
        }

        private void FindAmountOfItems()
        {
            foreach(KeyValuePair<int, Dictionary<int, SObject>> obj in PlayerItems)
            {
                int totalAmountOfItem = 0;
                foreach(KeyValuePair<int, SObject> item in obj.Value)
                {
                    totalAmountOfItem += item.Value.Stack;
                }
                AmountOfItems.Add(obj.Key, totalAmountOfItem);
            }
        }

        public int GetTotalAmountOfFishEggsOrMilk(int category)
        {
            switch (category)
            {
                case Constants.EGG_CATEGORY:
                    return GetTotalEggs();
                case Constants.FISH_CATEGORY:
                    return GetTotalFish();
                case Constants.MILK_CATEGORY:
                    return GetTotalMilk();
                default:
                    return 0;
            }
        }

        private int GetTotalFish()
        {
            int totalFish = 0;
            foreach(KeyValuePair<int, Dictionary<int, SObject>> obj in PlayerItems)
            {
                foreach(KeyValuePair<int, SObject> qualityObj in obj.Value)
                {
                    if (qualityObj.Value.Category == SObject.FishCategory)
                    {
                        totalFish += qualityObj.Value.Stack;
                    }
                }
            }
            return totalFish;
        }

        private int GetTotalMilk()
        {
            int totalMilk = 0;
            foreach (KeyValuePair<int, Dictionary<int, SObject>> obj in PlayerItems)
            {
                foreach (KeyValuePair<int, SObject> qualityObj in obj.Value)
                {
                    if (qualityObj.Value.Category == Constants.MILK_CATEGORY)
                    {
                        totalMilk += qualityObj.Value.Stack;
                    }
                }
            }
            return totalMilk;
        }

        private int GetTotalEggs()
        {
            int totalEggs = 0;
            foreach (KeyValuePair<int, Dictionary<int, SObject>> obj in PlayerItems)
            {
                foreach (KeyValuePair<int, SObject> qualityObj in obj.Value)
                {
                    if (qualityObj.Value.Category == Constants.EGG_CATEGORY)
                    {
                        totalEggs += qualityObj.Value.Stack;
                    }
                }
            }
            return totalEggs;
        }

        /// <summary>
        /// Condenses player items and filters out big craftables
        /// </summary>
        private void CondenseItems()
        {
            foreach (Item item in GetAllOwnedItems())
            {
                if (item is SObject obj && obj.Category != SObject.BigCraftableCategory)
                {
                    int key = obj.ParentSheetIndex;
                    SObject copyOfObj = new SObject(key, obj.Stack, quality: obj.Quality);
                    if (!PlayerItems.ContainsKey(key))
                    {
                        Dictionary<int, SObject> dict = new Dictionary<int, SObject>();
                        dict.Add(copyOfObj.Quality, copyOfObj);
                        PlayerItems.Add(key, dict);
                    }
                    else
                    {
                        Dictionary<int, SObject> quality_obj = PlayerItems[key];

                        if (!quality_obj.ContainsKey(copyOfObj.Quality))
                        {
                            quality_obj.Add(copyOfObj.Quality, copyOfObj);
                        }
                        else
                        {
                            //Make sure this modifies the actual dictionary
                            quality_obj[copyOfObj.Quality].Stack += copyOfObj.Stack;

                        }
                    }
                }
                else
                {
                    //don't add it (things like pickaxe, I don't care about that sort of stuff anyways for this mod)
                }
            }
            
        }

        private IEnumerable<Item> GetAllOwnedItems()
        {
            List<Item> items = new List<Item>();
            ISet<Item> itemsSeen = new HashSet<Item>(new ObjectReferenceComparer<Item>());

            // in locations
            foreach (GameLocation location in GetLocations())
            {
                // furniture
                foreach (Furniture furniture in location.furniture)
                    ScanAndTrack(tracked: items, itemsSeen: itemsSeen, root: furniture, parent: location, isRootInWorld: true);

                // farmhouse fridge
                Chest fridge;
                switch (location) {
                    case FarmHouse house:
                        fridge = house.fridge.Value;
                        break;
                    case IslandFarmHouse house:
                        fridge = house.fridge.Value;
                        break;
                    default:
                        fridge = null;
                        break;
                }

                ScanAndTrack(tracked: items, itemsSeen: itemsSeen, root: fridge, parent: location, includeRoot: false);

                // character hats
                foreach (NPC npc in location.characters)
                {
                    Hat hat =
                        (npc as Child)?.hat.Value
                        ?? (npc as Horse)?.hat.Value;
                    ScanAndTrack(tracked: items, itemsSeen: itemsSeen, root: hat, parent: npc);
                }

                // building output
                if (location is BuildableGameLocation buildableLocation)
                {
                    foreach (var building in buildableLocation.buildings)
                    {
                        switch (building)
                        {
                            case Mill mill:
                                ScanAndTrack(tracked: items, itemsSeen: itemsSeen, root: mill.output.Value, parent: mill, includeRoot: false);
                                break;

                            case JunimoHut hut:
                                ScanAndTrack(tracked: items, itemsSeen: itemsSeen, root: hut.output.Value, parent: hut, includeRoot: false);
                                break;
                        }
                    }
                }

                // map objects
                foreach (SObject item in location.objects.Values)
                {
                    if (item is Chest || !IsSpawnedWorldItem(item))
                        ScanAndTrack(tracked: items, itemsSeen: itemsSeen, root: item, parent: location, isRootInWorld: true);
                }
            }

            // inventory
            ScanAndTrack(tracked: items, itemsSeen: itemsSeen, roots: Game1.player.Items, parent: Game1.player, isInInventory: true);
            ScanAndTrack(
                tracked: items,
                itemsSeen: itemsSeen,
                roots: new Item[]
                {
                    Game1.player.shirtItem.Value,
                    Game1.player.pantsItem.Value,
                    Game1.player.boots.Value,
                    Game1.player.hat.Value,
                    Game1.player.leftRing.Value,
                    Game1.player.rightRing.Value
                },
                parent: Game1.player,
                isInInventory: true
            );

            // hay in silos
            Farm farm = Game1.getFarm();
            int hayCount = farm?.piecesOfHay.Value ?? 0;
            while (hayCount > 0)
            {
                SObject hay = new SObject(178, 1);
                hay.Stack = Math.Min(hayCount, hay.maximumStackSize());
                hayCount -= hay.Stack;
                ScanAndTrack(tracked: items, itemsSeen: itemsSeen, root: hay, parent: farm);
            }

            IItemBagsAPI itemBagsAPI = ModEntry.HelperInstance.ModRegistry.GetApi<IItemBagsAPI>("SlayerDharok.Item_Bags");
            if (itemBagsAPI != null)
            {
                IList<SObject> bagItems = itemBagsAPI.GetObjectsInsideBags(items, true);
                items.AddRange(bagItems);
            }

            return items;
        }

        /// <summary>Get all game locations.</summary>
        /// <param name="includeTempLevels">Whether to include temporary mine/dungeon locations.</param>
        public static IEnumerable<GameLocation> GetLocations(bool includeTempLevels = false)
        {
            var locations = Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );

            if (includeTempLevels)
                locations = locations.Concat(MineShaft.activeMines).Concat(VolcanoDungeon.activeLevels);

            return locations;
        }

        /// <summary>Get whether an item was spawned automatically. This is heuristic and only applies for items placed in the world, not items in an inventory.</summary>
        /// <param name="item">The item to check.</param>
        /// <remarks>Derived from the <see cref="SObject"/> constructors.</remarks>
        private bool IsSpawnedWorldItem(Item item)
        {
            return
                item is SObject obj
                && (
                    obj.IsSpawnedObject
                    || obj.isForage(null) // location argument is only used to check if it's on the beach, in which case everything is forage
                    || (!(obj is Chest) && (obj.Name == "Weeds" || obj.Name == "Stone" || obj.Name == "Twig"))
                );
        }

        /// <summary>Recursively find all items contained within a root item (including the root item itself) and add them to the <paramref name="tracked"/> list.</summary>
        /// <param name="tracked">The list to populate.</param>
        /// <param name="itemsSeen">The items which have already been scanned.</param>
        /// <param name="root">The root item to search.</param>
        /// <param name="parent">The parent entity which contains the <paramref name="root"/> (e.g. location, chest, furniture, etc).</param>
        /// <param name="isInInventory">Whether the item being scanned is in the current player's inventory.</param>
        /// <param name="isRootInWorld">Whether the item is placed directly in the world.</param>
        /// <param name="includeRoot">Whether to include the root item in the returned values.</param>
        private void ScanAndTrack(List<Item> tracked, ISet<Item> itemsSeen, Item root, object parent, bool isInInventory = false, bool isRootInWorld = false, bool includeRoot = true)
        {
            foreach (Item found in Scan(itemsSeen, root, parent, isInInventory, isRootInWorld, includeRoot))
                tracked.Add(found);
        }

        /// <summary>Recursively find all items contained within a root item (including the root item itself) and add them to the <paramref name="tracked"/> list.</summary>
        /// <param name="tracked">The list to populate.</param>
        /// <param name="itemsSeen">The items which have already been scanned.</param>
        /// <param name="roots">The root items to search.</param>
        /// <param name="parent">The parent entity which contains the <paramref name="roots"/> (e.g. location, chest, furniture, etc).</param>
        /// <param name="isInInventory">Whether the item being scanned is in the current player's inventory.</param>
        /// <param name="isRootInWorld">Whether the item is placed directly in the world.</param>
        /// <param name="includeRoots">Whether to include the root items in the returned values.</param>
        private void ScanAndTrack(List<Item> tracked, ISet<Item> itemsSeen, IEnumerable<Item> roots, object parent, bool isInInventory = false, bool isRootInWorld = false, bool includeRoots = true)
        {
            foreach (Item found in roots.SelectMany(root => Scan(itemsSeen, root, parent, isInInventory, isRootInWorld, includeRoots)))
                tracked.Add(found);
        }

        /// <summary>Recursively find all items contained within a root item (including the root item itself).</summary>
        /// <param name="itemsSeen">The items which have already been scanned.</param>
        /// <param name="root">The root item to search.</param>
        /// <param name="parent">The parent entity which contains the <paramref name="root"/> (e.g. location, chest, furniture, etc).</param>
        /// <param name="isInInventory">Whether the item being scanned is in the current player's inventory.</param>
        /// <param name="isRootInWorld">Whether the item is placed directly in the world.</param>
        /// <param name="includeRoot">Whether to include the root item in the returned values.</param>
        private IEnumerable<Item> Scan(ISet<Item> itemsSeen, Item root, object parent, bool isInInventory, bool isRootInWorld, bool includeRoot = true)
        {
            if (root == null || !itemsSeen.Add(root))
                yield break;

            // get root
            yield return root;

            // get direct contents
            foreach (Item child in GetDirectContents(root, isRootInWorld).SelectMany(p => Scan(itemsSeen, p, root, isInInventory, isRootInWorld: false)))
                yield return child;
        }

        /// <summary>Get the items contained by an item. This is not recursive and may return null values.</summary>
        /// <param name="root">The root item to search.</param>
        /// <param name="isRootInWorld">Whether the item is placed directly in the world.</param>
        private IEnumerable<Item> GetDirectContents(Item root, bool isRootInWorld)
        {
            if (root == null)
                yield break;

            // held object
            if (root is SObject obj)
            {
                if (obj.MinutesUntilReady <= 0 || obj is Cask) // cask output can be retrieved anytime
                    yield return obj.heldObject.Value;
            }
            else if (IsCustomItemClass(root))
            {
                // convention for custom mod items
                Item heldItem =
                    Reflection.GetField<Item>(root, nameof(SObject.heldObject), required: false)?.GetValue()
                    ?? Reflection.GetProperty<Item>(root, nameof(SObject.heldObject), required: false)?.GetValue();
                if (heldItem != null)
                    yield return heldItem;
            }

            // inventories
            switch (root)
            {
                case StorageFurniture dresser:
                    foreach (Item item in dresser.heldItems)
                        yield return item;
                    break;

                case Chest chest when (!isRootInWorld || chest.playerChest.Value):
                    foreach (Item item in chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID))
                        yield return item;
                    break;

                case Tool tool:
                    foreach (SObject item in tool.attachments)
                        yield return item;
                    break;
            }
        }

        /// <summary>Get whether an item instance is a custom mod subclass.</summary>
        /// <param name="item">The item to check.</param>
        private bool IsCustomItemClass(Item item)
        {
            string itemNamespace = item.GetType().Namespace ?? "";
            return itemNamespace != "StardewValley" && !itemNamespace.StartsWith("StardewValley.");
        }
    }

    /// <summary>A comparer which considers two references equal if they point to the same instance.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    internal class ObjectReferenceComparer<T> : IEqualityComparer<T>
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public bool Equals(T x, T y)
        {
            return object.ReferenceEquals(x, y);
        }

        /// <summary>Get a hash code for the specified object.</summary>
        /// <param name="obj">The value.</param>
        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
