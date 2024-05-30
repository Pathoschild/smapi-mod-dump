/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using ResourceStorage.BetterCrafting;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace ResourceStorage
{
    public partial class ModEntry
    {
        public static long ModifyResourceLevel(Farmer instance, string id, int amountToAdd, bool auto = true, bool notifyBetterCraftingIntegration = true)
        {
            id = ItemRegistry.QualifyItemId(id);
            if (id == null)
            {
                return 0;
            }

            Dictionary<string, long> dict = GetFarmerResources(instance);

            if (!dict.TryGetValue(id, out long oldAmount))
            {
                SMonitor.Log($"{instance.Name} does not have {id}");
                oldAmount = 0;
            }

            if (auto && !CanAutoStore(id) && amountToAdd > 0)
                return 0;

            var newAmount = Math.Max(oldAmount + amountToAdd, 0);
            if (newAmount != oldAmount)
            {
                SMonitor.Log($"Modified {instance.Name}'s resource {id} from {oldAmount} to {newAmount}");
                if (Config.ShowMessage)
                {
                    Object item = ItemRegistry.Create<Object>(id, (int)(newAmount - oldAmount));
                    try
                    {
                        var hm = new HUDMessage(string.Format(newAmount > oldAmount ? SHelper.Translation.Get("added-x-y") : SHelper.Translation.Get("removed-x-y"), (int)Math.Abs(newAmount - oldAmount), item.DisplayName), 1000) { whatType = newAmount > oldAmount ? 4 : 3 };
                        Game1.addHUDMessage(hm);
                    }
                    catch { }
                }
            }
            if (newAmount <= 0)
                dict.Remove(id);
            else
                dict[id] = newAmount;

            if (notifyBetterCraftingIntegration)
            {
                BetterCraftingIntegration.NotifyResourceChange(id, amountToAdd, instance.UniqueMultiplayerID);
            }

            return newAmount - oldAmount;
        }

        public static Dictionary<string, long> GetFarmerResources(Farmer instance)
        {
            //SMonitor.Log($"Getting resource dictionary for {instance.Name}");
            if (!resourceDict.TryGetValue(instance.UniqueMultiplayerID, out var dict))
            {
                SMonitor.Log($"Checking mod data for {instance.Name}");
                if(instance.modData.TryGetValue(dictKey, out var str))
                {
                    SMonitor.Log("Deserializing dictionary!");
                    dict = DeserializeDictionary(str);
                }
                else
                {
                    SMonitor.Log("No mod data found; making a new dictionary.");
                    dict = new();
                }
                resourceDict[instance.UniqueMultiplayerID] = dict;
            }
            return dict;
        }

        public static Dictionary<string, long> DeserializeDictionary(string json)
        {
            return MigrateDictionary(JsonConvert.DeserializeObject<Dictionary<string, long>>(json));
        }

        /// <summary>
        /// The resource dictionary used to (pre-SDV 1.6) use the first four items in an object's object data as the key. I now use the qualified item id. This migrates
        /// the old keys so that old saves don't lose their resources.
        /// </summary>
        /// <param name="oldDictionary"></param>
        /// <returns></returns>
        public static Dictionary<string, long> MigrateDictionary(Dictionary<string, long> oldDictionary)
        {
            Dictionary<string, long> newDictionary = new();

            bool first = true;

            foreach(var kvp in oldDictionary)
            {
                if(first && ItemRegistry.IsQualifiedItemId(kvp.Key)) // If the is a qualified item id, it has already been migrated
                {
                    SMonitor.Log($"The dictionary has already been migrated. The first key is {kvp.Key}.");
                    return oldDictionary;
                }
                else
                {
                    first = false;
                }

                string[] oldData = kvp.Key.Split("/");
                if(oldData.Length != 4) // The old dictionary used the first four items as keys, so it should be four long.
                {
                    SMonitor.Log($"The dictionary has not been migrated but the data count is not four! Key: {kvp.Key}");
                    continue;
                }

                foreach(var objectKvp in Game1.objectData)
                {
                    if(objectKvp.Value.Name == oldData[0] && objectKvp.Value.Category != Object.litterCategory) // The litter check is necessary to prevent stone from turning into a gem rock because those are called stone too and they come before the material stone in the dictionary
                    {
                        newDictionary["(O)" + objectKvp.Key] = kvp.Value;
                        break;
                    }
                }
            }

            return newDictionary;
        }

        public static long GetResourceAmount(Farmer instance, string id)
        {
            if(id == null)
            {
                return 0;
            }

            Dictionary<string, long> dict = GetFarmerResources(instance);

            long count = dict.TryGetValue(id, out long amount) ? amount : 0;

            //SMonitor.Log($"Getting {instance.Name}'s {id} ({count})");

            return count;
        }
        public static bool CanStore(Object obj)
        {
            bool output = !(obj.Quality > 0 || obj.preserve.Value is not null || obj.orderData.Value is not null || obj.preservedParentSheetIndex.Value is not null || obj.bigCraftable.Value || obj.GetType() != typeof(Object) || obj.maximumStackSize() == 1);
            //SMonitor.Log(output ? $"Can store {obj.DisplayName}" : $"Cannot store {obj.DisplayName}");
            return output;
        }
        public static bool CanAutoStore(Object obj)
        {
            if (!CanStore(obj))
                return false;
            
            return CanAutoStore(obj.ItemId);
        }

        public static bool CanAutoStore(string id)
        {
            ParsedItemData data = ItemRegistry.GetDataOrErrorItem(id);
            string name = data.InternalName.ToLower();
                
            foreach (var str in Config.AutoStore.Split(','))
            {
                if(str.Trim().ToLower() == name && data.Category != Object.litterCategory)
                {
                    //SMonitor.Log($"Can astore {data.DisplayName}");
                    return true;
                }
            }
            //SMonitor.Log($"Cannot astore {data.DisplayName}");
            return false;
        }

        public static string DequalifyItemId(string id)
        {
            return ItemRegistry.GetMetadata(id).LocalItemId;
        }

        private static int AddIngredientAmount(int ingredient_count, string itemId)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return ingredient_count;

            int newAmount = ingredient_count + GetMatchesForCrafting(Game1.player, itemId);

            //SMonitor.Log($"Updated ingredient amount {ingredient_count} -> {newAmount}");

            return newAmount;
        }

        public static bool TryGetInventoryOwner(Inventory inventory, out Farmer farmer)
        {
            foreach(Farmer f in Game1.getAllFarmers())
            {
                if(ReferenceEquals(f.Items, inventory))
                {
                    //SMonitor.Log($"{f.Name} is the inventory owner");
                    farmer = f;
                    return true;
                }
            }

            farmer = null;
            return false;
        }

        public static int GetMatchesForCrafting(Farmer farmer, string itemId)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return 0;

            //SMonitor.Log($"Getting {farmer.Name}'s crafting matches for {itemId}");

            int output = 0;
            foreach (var kvp in GetFarmerResources(farmer))
            {
                if (CraftingRecipe.ItemMatchesForCrafting(ItemRegistry.Create(kvp.Key), itemId))
                {
                    //SMonitor.Log($"{kvp.Key} mathces. Adding {kvp.Value}");
                    output += (int)kvp.Value;
                }
            }
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="itemId"></param>
        /// <param name="maxAmount"></param>
        /// <returns>The amount removed from the storage.</returns>
        public static int ConsumeItemsForCrafting(Farmer farmer, string itemId, int maxAmount)
        {
            if (!Config.ModEnabled || !Config.AutoUse)
                return 0;

            SMonitor.Log($"Consuming {maxAmount} {itemId} from {farmer.Name} for crafting.");

            int totalConsumed = 0;

            foreach(var kvp in GetFarmerResources(farmer))
            {
                if (CraftingRecipe.ItemMatchesForCrafting(ItemRegistry.Create(kvp.Key), itemId))
                {
                    //SMonitor.Log($"Consuming {kvp.Key}");
                    totalConsumed -= (int)ModifyResourceLevel(farmer, kvp.Key, -(maxAmount - totalConsumed), auto: true);
                    //SMonitor.Log($"Total consumed thus far: {totalConsumed}");
                    if (totalConsumed >= maxAmount)
                    {
                        return totalConsumed;
                    }
                }
            }

            return totalConsumed;
        }

        public static void SetupResourceButton(IClickableMenu menu)
        {
            if(resourceButton.Value is null)
            {
                resourceButton.Value = new ClickableTextureComponent("Up", new Rectangle(menu.xPositionOnScreen + menu.width + 8 + Config.IconOffsetX, menu.yPositionOnScreen + 256 + Config.IconOffsetY, 44, 44), "", SHelper.Translation.Get("resources"), Game1.mouseCursors, new Rectangle(116, 442, 22, 22), 2)
                {
                    myID = 42999,
                    upNeighborID = 106,
                    downNeighborID = 105,
                    leftNeighborID = 11
                };
            }
            else
            {
                resourceButton.Value.bounds = new Rectangle(menu.xPositionOnScreen + menu.width + 8 + Config.IconOffsetX, menu.yPositionOnScreen + 256 + Config.IconOffsetY, 44, 44);
            }
        }

        public static int Farmer_GetItemCountTranspilerMethod(Farmer farmer, IList<Item> items, string itemId)
        {
            // Ignore deprecation warning because this is the method that GetItemCount uses by default
            #pragma warning disable CS0618 // Type or member is obsolete
            int count = farmer.getItemCountInList(items, itemId);
            #pragma warning restore CS0618 // Type or member is obsolete

            // The above is not in the try b/c it is literally what the method would do if we didn't transpile it
            try
            {
                if (Config.ModEnabled && Config.AutoUse)
                    count += GetMatchesForCrafting(farmer, itemId);
            }
            catch (Exception e)
            {
                SMonitor.Log($"Failed in {Farmer_GetItemCountTranspilerMethod}: {e}");
            }

            return count;
        }
        public static void SaveResourceDictionary(Farmer farmer)
        {
            if (resourceDict.TryGetValue(farmer.UniqueMultiplayerID, out var dict))
            {
                SMonitor.Log($"Saving resource dictionary for {farmer.Name}");
                farmer.modData[dictKey] = JsonConvert.SerializeObject(dict);
            }
        }
    }
}