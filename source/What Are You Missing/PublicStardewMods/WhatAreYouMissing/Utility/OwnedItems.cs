using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace WhatAreYouMissing
{
    public class OwnedItems
    {
        //<parentSheetIndex, <quality, obj>>
        private Dictionary<int, Dictionary<int, SObject>> PlayerItems;
        private Dictionary<int, int> AmountOfItems;

        public OwnedItems()
        {
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
            foreach (Item item in FindOwnedItems())
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

        private IEnumerable<Item> FindOwnedItems()
        {
            List<Item> ownedItems = new List<Item>();

            //add the current inventory to the list
            ownedItems.AddRange(Game1.player.Items);

            foreach (GameLocation location in GetAllLocations())
            {
                foreach (SObject obj in location.Objects.Values)
                {
                    //this should add everything that is a chest
                    //it might not add if something is like done in the furnace/keg/preserve jar

                    if (obj is Chest chest)
                    {
                        if (chest.playerChest.Value)
                        {
                            ownedItems.Add(chest);
                            ownedItems.AddRange(chest.items);
                        }
                    }
                    //auto grabber
                    else if (obj.ParentSheetIndex == 165 && obj.heldObject.Value is Chest grabberChest)
                    {
                        ownedItems.Add(obj);
                        ownedItems.AddRange(grabberChest.items);
                    }

                    //cask
                    else if (obj is Cask)
                    {
                        ownedItems.Add(obj);
                        ownedItems.Add(obj.heldObject.Value); // cask contents can be retrieved anytime
                    }

                    //craftable
                    else if (obj.bigCraftable.Value)
                    {
                        ownedItems.Add(obj);
                        if (obj.MinutesUntilReady == 0)
                        {
                            ownedItems.Add(obj.heldObject.Value);
                        }
                    }

                    //anything else
                    else if (!obj.IsSpawnedObject)
                    {
                        ownedItems.Add(obj);
                        ownedItems.Add(obj.heldObject.Value);
                    }
                }

                if (location is Farm farm)
                {
                    foreach (Building building in farm.buildings)
                    {
                        if (building is JunimoHut hut)
                        {
                            ownedItems.AddRange(hut.output.Value.items);
                        }

                        if (building is Mill mill)
                        {
                            ownedItems.AddRange(mill.output.Value.items);
                        }
                    }
                }

                if (location is DecoratableLocation decoratableLocation)
                {
                    foreach (Furniture furniture in decoratableLocation.furniture)
                    {
                        ownedItems.Add(furniture);
                        ownedItems.Add(furniture.heldObject.Value);
                    }
                }
            }
            return ownedItems.Where(p => p != null);
        }

        private IEnumerable<GameLocation> GetAllLocations()
        {
            return Game1.locations
                .Concat(
                from location in Game1.locations.OfType<BuildableGameLocation>()
                from building in location.buildings
                where building.indoors.Value != null
                select building.indoors.Value
                );
        }
    }
}
