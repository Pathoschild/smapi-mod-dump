/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Community_Center;
using ItemBags.Persistence;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ItemBags.Helpers
{
    public class SaveLoadHelpers
    {
        /// <summary>An arbitrary offset for the <see cref="Item.ParentSheetIndex"/> property of encoded custom items, so that we can guaranteeably differentiate between 
        /// items managed by this mod and items that are coincidentally the same type of item but weren't created by this mod.</summary>
        private const int EncodedItemStartIndex = 5000;

        /// <summary>Converts all custom items used by this mod into items that actually exist in the vanilla StardewValley game, so that the game won't crash while trying to save.<para/>
        /// The custom items are saved to a separate file using <see cref="StardewModdingAPI.IDataHelper.WriteSaveData{TModel}(string, TModel)"/></summary>
        internal static void OnSaving()
        {
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                ItemBagsMod.ModInstance.Monitor.Log("ItemBags OnSaving started.", LogLevel.Debug);

                int CurrentBagId = 0;
                List<BagInstance> BagInstances = new List<BagInstance>();

                //  Pre-emptive error-handling - try to find any encoded bags that, for whatever unknown reason, weren't able to be converted back into ItemBags during a Load.
                //  If any were found, then we need to retain the SaveData's BagInstance associated with that item (and not re-use it's Bag InstanceId when assigning to the bags that didn't have any issues during loading)
                //  so that the mod can still try again to load that bag during the next load.
                //  If someone mysteriously loses a bag, I can at least do some manual save editing to restore it, as the data will still be there.
                List<Item> CorruptedBags = new List<Item>();
                ReplaceAllInstances(x => IsEncodedCustomItem(x), x =>
                {
                    CorruptedBags.Add(x);
                    return x;
                });
                HashSet<int> CorruptedBagIds = new HashSet<int>(CorruptedBags.Select(x => x.ParentSheetIndex - EncodedItemStartIndex));

                PlayerBags PreviousBagData = null;
                if (CorruptedBagIds.Any())
                {
                    PreviousBagData = PlayerBags.DeserializeFromCurrentSaveFile();
                    if (PreviousBagData != null && PreviousBagData.Bags != null)
                    {
                        Dictionary<int, BagInstance> IndexedInstances = new Dictionary<int, BagInstance>();
                        foreach (BagInstance Instance in PreviousBagData.Bags)
                        {
                            if (!IndexedInstances.ContainsKey(Instance.InstanceId))
                            {
                                IndexedInstances.Add(Instance.InstanceId, Instance);
                            }
                        }

                        foreach (int CorruptedId in CorruptedBagIds)
                        {
                            if (IndexedInstances.TryGetValue(CorruptedId, out BagInstance CorruptedInstance))
                            {
                                BagInstances.Add(CorruptedInstance);
                            }
                        }
                    }
                }

                //  Encode all bags as a regular non-modded item
                ReplaceAllInstances(IsCustomItem, CustomItem =>
                {
                    if (CustomItem is ItemBag IB)
                    {
                        try
                        {
                            while (CorruptedBagIds.Contains(CurrentBagId))
                            {
                                CurrentBagId++;
                            }

                            BagInstance Instance = new BagInstance(CurrentBagId, IB);
                            BagInstances.Add(Instance);

                        //  Replace the Bag with an arbitrary low-value/non-stackable item (in this case, a Rusty Sword) and store the bag instance's Id in the replacement item's ParentSheetIndex
                        MeleeWeapon Replacement = new MeleeWeapon(0);
                            Replacement.ParentSheetIndex = EncodedItemStartIndex + CurrentBagId;
                            return Replacement;
                        }
                        finally { CurrentBagId++; }
                    }
                    else
                    {
                        return CustomItem;
                    }
                });

                PlayerBags OwnedBags = new PlayerBags();
                OwnedBags.Bags = BagInstances.ToArray();
                OwnedBags.SerializeToCurrentSaveFile();

                ItemBagsMod.ModInstance.Monitor.Log("ItemBags OnSaving finished.", LogLevel.Debug);
            }
        }

        internal static void OnSaved() { LoadCustomItems(); }
        internal static void OnLoaded()
        {
            //ItemBagsMod.ModInstance.Monitor.Log("ItemBags OnLoaded started.", LogLevel.Info);

            //if (Context.IsMultiplayer && !Context.IsMainPlayer)
            //    ItemBagsMod.ModInstance.Helper.Multiplayer.SendMessage("RequestBagResync", "RequestBagResync", new string[] { ItemBagsMod.ModUniqueId });

            try { LoadCustomItems(); }
            catch (Exception ex) { ItemBagsMod.ModInstance.Monitor.Log("ItemBags error during LoadCustomItems: " + ex.Message, LogLevel.Error); }

            CommunityCenterBundles.Instance = null;
            CommunityCenterBundles.Instance = new CommunityCenterBundles();

            //ItemBagsMod.ModInstance.Monitor.Log("ItemBags OnLoaded finished.", LogLevel.Info);
        }

        /// <summary>Restores custom items used by this mod that were modified by <see cref="OnSaving"/>. Intended to be used after the game is saved or a save file is loaded.<para/>
        /// (On PC versions of this mod, the bags are saved using PyTK, but PyTK doesn't work with Android)</summary>
        private static void LoadCustomItems()
        {
            PlayerBags OwnedBags = PlayerBags.DeserializeFromCurrentSaveFile();
            if (OwnedBags == null)
            {
                return;
            }
            else
            {
                BagConfig GlobalConfig = ItemBagsMod.BagConfig;

                //  Index the saved bags by their instance ids
                Dictionary<int, BagInstance> IndexedBagInstances = new Dictionary<int, BagInstance>();
                foreach (BagInstance BagInstance in OwnedBags.Bags)
                {
                    if (!IndexedBagInstances.ContainsKey(BagInstance.InstanceId))
                    {
                        IndexedBagInstances.Add(BagInstance.InstanceId, BagInstance);
                    }
                    else
                    {
                        string Warning = string.Format("Warning - multiple bag instances were found with the same InstanceId. Did you manually edit your {0} json file or add multiple .json files to the 'Modded Bags' folder with the same ModUniqueId values? Only the first bag with InstanceId = {1} will be loaded.",
                            PlayerBags.OwnedBagsDataKey, BagInstance.InstanceId);
                        ItemBagsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);
                    }
                }

                //  Decode all of our Encoded custom items back into their original form
                ReplaceAllInstances(IsEncodedCustomItem, Encoded =>
                {
                    int BagInstanceId = Encoded.ParentSheetIndex - EncodedItemStartIndex;
                    if (IndexedBagInstances.TryGetValue(BagInstanceId, out BagInstance BagInstance))
                    {
                        if (BagInstance.TryDecode(out ItemBag Replacement))
                            return Replacement;
                        else
                            return Encoded;
                    }
                    else
                    {
                        string Warning = string.Format("Warning - no saved Bag was found with InstanceId = {0}. Did you manually edit your {1} json file or delete a .json file from 'Modded Bags' folder?",
                            BagInstanceId, PlayerBags.OwnedBagsDataKey);
                        ItemBagsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);
                        return Encoded;
                    }
                });
            }
        }

        /// <summary>Returns true if the given input item is one of this mod's managed custom items.</summary>
        private static bool IsCustomItem(Item item)
        {
            return item is ItemBag;
        }

        /// <summary>Returns true if the given input item is the encoded version of one of this mod's managed custom items.</summary>
        private static bool IsEncodedCustomItem(Item item)
        {
            return item is MeleeWeapon && item.ParentSheetIndex >= EncodedItemStartIndex && item.Name.Equals("Rusty Sword", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>Attempts to find all Items in the entire game (in the player inventory, inside chests, fridges, storage furniture etc) that match the given Predicate</summary>
        public static List<Item> GetAllInstances(Predicate<Item> Predicate)
        {
            List<Item> Results = new List<Item>();
            ReplaceAllInstances(Predicate, x => { Results.Add(x); return x; });
            return Results;
        }

        /// <summary>Attempts to find all Items in the entire game (in the player inventory, inside chests, fridges, storage furniture etc) 
        /// that match the given Predicate, and replace that Item with the return value of the given Replacer function.</summary>
        /// <param name="Predicate">A condition that returns true if the Item should be replaced</param>
        /// <param name="Replacer">A function whose input is the Item to replace, and whose output is the new Item it should be replaced with.<para/>
        /// Probably crashes if Replacer outputs null (not tested).</param>
        public static void ReplaceAllInstances(Predicate<Item> Predicate, Func<Item, Item> Replacer)
        {
            //  Handle items in player inventory
            foreach (Farmer Farmer in Game1.getAllFarmers())
            {
                List<int> InventoryItemIndices = new List<int>();
                for (int i = 0; i < Farmer.Items.Count; i++)
                {
                    Item item = Farmer.Items[i];
                    if (item != null && Predicate(item))
                    {
                        InventoryItemIndices.Add(i);
                    }
                }
                foreach (int Index in InventoryItemIndices)
                {
                    Item ToRemove = Farmer.Items[Index];
                    Item Replacement = Replacer(ToRemove);
                    if (ToRemove != Replacement)
                    {
                        Item Removed = Farmer.removeItemFromInventory(Index);
                        Farmer.addItemToInventory(Replacement, Index);
                    }
                }
            }

            //  This code is a refactored and slightly modified version of iterateChestsAndStorage method shown below this function
            foreach (GameLocation l in Game1.locations)
            {
                foreach (Object o in l.objects.Values)
                {
                    if (o is Chest oChest)
                    {
                        for (int i = 0; i < oChest.items.Count; i++)
                        {
                            Item item = oChest.items[i];
                            if (item != null && Predicate(item))
                            {
                                Item Replacement = Replacer(item);
                                if (item != Replacement)
                                {
                                    oChest.items[i] = Replacement;
                                }
                            }
                        }
                    }
                    if (o.heldObject.Value == null || !(o.heldObject.Value is Chest))
                    {
                        continue;
                    }

                    Chest heldChest = o.heldObject.Value as Chest;
                    for (int i = 0; i < heldChest.items.Count; i++)
                    {
                        Item item = heldChest.items[i];
                        if (item != null && Predicate(item))
                        {
                            Item Replacement = Replacer(item);
                            if (item != Replacement)
                            {
                                heldChest.items[i] = Replacement;
                            }
                        }
                    }
                }
                if (l is FarmHouse lFarmHouse)
                {
                    for (int i = 0; i < lFarmHouse.fridge.Value.items.Count; i++)
                    {
                        Item item = lFarmHouse.fridge.Value.items[i];
                        if (item != null && Predicate(item))
                        {
                            Item Replacement = Replacer(item);
                            if (item != Replacement)
                            {
                                lFarmHouse.fridge.Value.items[i] = Replacement;
                            }
                        }
                    }
                }
                if (l is DecoratableLocation lDecoratable)
                {
                    for (int i = 0; i < lDecoratable.furniture.Count; i++)
                    {
                        Furniture furniture = lDecoratable.furniture[i];

                        if (furniture is StorageFurniture storageFurniture)
                        {
                            for (int j = 0; j < storageFurniture.heldItems.Count; j++)
                            {
                                Item item = storageFurniture.heldItems[j];
                                if (item != null && Predicate(item))
                                {
                                    Item Replacement = Replacer(item);
                                    if (item != Replacement)
                                    {
                                        storageFurniture.heldItems[j] = Replacement;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!(l is BuildableGameLocation))
                {
                    continue;
                }
                foreach (Building b in (l as BuildableGameLocation).buildings)
                {
                    if (b.indoors.Value != null)
                    {
                        foreach (Object o in b.indoors.Value.objects.Values)
                        {
                            if (o is Chest oChest)
                            {
                                for (int i = 0; i < oChest.items.Count; i++)
                                {
                                    Item item = oChest.items[i];
                                    if (item != null && Predicate(item))
                                    {
                                        Item Replacement = Replacer(item);
                                        if (item != Replacement)
                                        {
                                            oChest.items[i] = Replacement;
                                        }
                                    }
                                }
                            }
                            if (o.heldObject.Value == null || !(o.heldObject.Value is Chest))
                            {
                                continue;
                            }

                            Chest heldChest = o.heldObject.Value as Chest;
                            for (int i = 0; i < heldChest.items.Count; i++)
                            {
                                Item item = heldChest.items[i];
                                if (item != null && Predicate(item))
                                {
                                    Item Replacement = Replacer(item);
                                    if (item != Replacement)
                                    {
                                        heldChest.items[i] = Replacement;
                                    }
                                }
                            }
                        }
                        if (!(b.indoors.Value is DecoratableLocation))
                        {
                            continue;
                        }
                        foreach (Furniture furniture in (b.indoors.Value as DecoratableLocation).furniture)
                        {
                            if (!(furniture is StorageFurniture))
                            {
                                continue;
                            }

                            if (furniture is StorageFurniture storageFurniture)
                            {
                                for (int i = 0; i < storageFurniture.heldItems.Count; i++)
                                {
                                    Item item = storageFurniture.heldItems[i];
                                    if (item != null && Predicate(item))
                                    {
                                        Item Replacement = Replacer(item);
                                        if (item != Replacement)
                                        {
                                            storageFurniture.heldItems[i] = Replacement;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!(b is Mill))
                    {
                        if (!(b is JunimoHut))
                        {
                            continue;
                        }

                        if (b is JunimoHut junimoHut)
                        {
                            for (int i = 0; i < junimoHut.output.Value.items.Count; i++)
                            {
                                Item item = junimoHut.output.Value.items[i];
                                if (item != null && Predicate(item))
                                {
                                    Item Replacement = Replacer(item);
                                    if (item != Replacement)
                                    {
                                        junimoHut.output.Value.items[i] = Replacement;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Mill mill = b as Mill;
                        for (int i = 0; i < mill.output.Value.items.Count; i++)
                        {
                            Item item = mill.output.Value.items[i];
                            if (item != null && Predicate(item))
                            {
                                Item Replacement = Replacer(item);
                                if (item != Replacement)
                                {
                                    mill.output.Value.items[i] = Replacement;
                                }
                            }
                        }
                    }
                }
            }
        }

        //  This code was taken directly from decompiling StardewValley.exe (in StardewValley.Utility.iterateChestsAndStorage)
        /*public static void iterateChestsAndStorage(Action<Item> action)
        {
            foreach (GameLocation l in Game1.locations)
            {
                foreach (Object o in l.objects.Values)
                {
                    if (o is Chest)
                    {
                        foreach (Item item in (o as Chest).items)
                        {
                            if (item == null)
                            {
                                continue;
                            }
                            action(item);
                        }
                    }
                    if (o.heldObject.Value == null || !(o.heldObject.Value is Chest))
                    {
                        continue;
                    }
                    foreach (Item item in (o.heldObject.Value as Chest).items)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        action(item);
                    }
                }
                if (l is FarmHouse)
                {
                    foreach (Item item in (l as FarmHouse).fridge.Value.items)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        action(item);
                    }
                }
                if (l is DecoratableLocation)
                {
                    foreach (Furniture furniture in (l as DecoratableLocation).furniture)
                    {
                        if (!(furniture is StorageFurniture))
                        {
                            continue;
                        }
                        foreach (Item item in (furniture as StorageFurniture).heldItems)
                        {
                            if (item == null)
                            {
                                continue;
                            }
                            action(item);
                        }
                    }
                }
                if (!(l is BuildableGameLocation))
                {
                    continue;
                }
                foreach (Building b in (l as BuildableGameLocation).buildings)
                {
                    if (b.indoors.Value != null)
                    {
                        foreach (Object o in b.indoors.Value.objects.Values)
                        {
                            if (o is Chest)
                            {
                                foreach (Item item in (o as Chest).items)
                                {
                                    if (item == null)
                                    {
                                        continue;
                                    }
                                    action(item);
                                }
                            }
                            if (o.heldObject.Value == null || !(o.heldObject.Value is Chest))
                            {
                                continue;
                            }
                            foreach (Item item in (o.heldObject.Value as Chest).items)
                            {
                                if (item == null)
                                {
                                    continue;
                                }
                                action(item);
                            }
                        }
                        if (!(b.indoors.Value is DecoratableLocation))
                        {
                            continue;
                        }
                        foreach (Furniture furniture in (b.indoors.Value as DecoratableLocation).furniture)
                        {
                            if (!(furniture is StorageFurniture))
                            {
                                continue;
                            }
                            foreach (Item item in (furniture as StorageFurniture).heldItems)
                            {
                                if (item == null)
                                {
                                    continue;
                                }
                                action(item);
                            }
                        }
                    }
                    else if (!(b is Mill))
                    {
                        if (!(b is JunimoHut))
                        {
                            continue;
                        }
                        foreach (Item item in (b as JunimoHut).output.Value.items)
                        {
                            if (item == null)
                            {
                                continue;
                            }
                            action(item);
                        }
                    }
                    else
                    {
                        foreach (Item item in (b as Mill).output.Value.items)
                        {
                            if (item == null)
                            {
                                continue;
                            }
                            action(item);
                        }
                    }
                }
            }
        }*/
    }
}
