/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ItemBags.Bags;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

using Object = StardewValley.Object;

namespace ItemBags
{
#if NEVER
    public static class ItemPatches
    {
        [HarmonyPatch(typeof(Item), nameof(Item.ConsumeStack))]
        public static bool ConsumeStack_Prefix(Item __instance, int amount, ref Item __result)
        {
            try
            {
                // From decompiled game code, File version 1.6.3.24087
                //  Notice that "Stack -= amount" does not execute when Stack-amount<=0
                /*
                if (amount == 0)
                {
                    return this;
                }

                if (Stack - amount <= 0)
                {
                    return null;
                }

                Stack -= amount;
                return this;
                */

                //  Maybe instead should check if Stack <= amount, but who knows if a negative Stack value will cause weird issues elsewhere,
                //  and doing Math.Max(0, Stack - amount) also seems a little bit risky. Let's only handle the most common case
                if (__instance.Stack == amount)
                {
                    __instance.Stack -= amount;
                    __result = null;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log($"Error in {nameof(ItemPatches)}.{nameof(ConsumeStack_Prefix)}: {ex.Message}", LogLevel.Error);
                return true;
            }
        }

        [HarmonyPatch(typeof(Item), nameof(Item.ConsumeStack))]
        public static void ConsumeStack_Postfix(Item __instance, int amount, ref Item __result)
        {
            try
            {

            }
            catch (Exception ex) { ItemBagsMod.ModInstance.Monitor.Log($"Error in {nameof(ItemPatches)}.{nameof(ConsumeStack_Postfix)}: {ex.Message}", LogLevel.Error); }
        }
    }
#endif

    public static class CraftingHandler
    {
        private const string CookingSkillModUniqueId = "spacechase0.CookingSkill";
        private static bool IsCookingSkillModCompatible;
        private const string BetterCraftingUniqueId = "leclair.bettercrafting";

        private static IModHelper Helper { get; set; }

        /// <summary>Adds functionality that allows you to use items within your bags as input materials for crafting from the main GameMenu's CraftingPage, or from a cooking CraftingPage.</summary>
        internal static void OnModEntry(IModHelper Helper)
        {
            CraftingHandler.Helper = Helper;

            //  "Better Crafting" mod replaces the vanilla StardewValley.Menus.CraftingPage object with a custom IClickableMenu that doesn't have the same _materialContainers field
            bool IsBetterCraftingInstalled = Helper.ModRegistry.IsLoaded(BetterCraftingUniqueId);
            if (IsBetterCraftingInstalled)
            {
                //ItemBagsMod.ModInstance.Monitor.Log($"'Better Crafting' mod ({BetterCraftingUniqueId}) detected. " +
                //    $"You will not be able to craft using items inside of bags.", LogLevel.Info);
            }
            else
            {

                Helper.Events.Display.MenuChanged += Display_MenuChanged;
                Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

                IsCookingSkillModCompatible = Helper.ModRegistry.IsLoaded(CookingSkillModUniqueId) &&
                    Helper.ModRegistry.Get(CookingSkillModUniqueId).Manifest.Version.IsNewerThan("1.1.4"); // "_materialContainers" field was added to CookingSkill.NewCraftingPage in version 1.1.5
            }

#if NEVER
            Harmony Harmony = new Harmony(ItemBagsMod.ModInstance.ModManifest.UniqueID);
            Harmony.Patch(
                original: AccessTools.Method(typeof(Item), nameof(Item.ConsumeStack)),
                prefix: new HarmonyMethod(typeof(ItemPatches), nameof(ItemPatches.ConsumeStack_Prefix)),
                postfix: new HarmonyMethod(typeof(ItemPatches), nameof(ItemPatches.ConsumeStack_Postfix))
            );
#endif
        }

        private static HashSet<ItemBag> BagsInUse = null;
        internal static bool IsUsingForCrafting(ItemBag Bag) { return BagsInUse != null && BagsInUse.Contains(Bag); }
        private static List<ItemBagCraftingInventory> ItemBagInventories = null;

        /// <summary>Initializes extra data for the Crafting Page so it can search for and use materials within bags in your inventory.</summary>
        private static void OnCraftingPageActivated(IClickableMenu CraftingMenu)
        {
            //  Allow the CraftingPage to search for and use items inside of bags
            bool AllowUsingBundleBagItemsForCrafting = false;

            List<ItemBag> BagsInInventory = Game1.player.Items.Where(x => x != null && x is ItemBag).Cast<ItemBag>().ToList();
            List<ItemBag> SearchedBags = BagsInInventory.ToList();

            //  Get the "_materialContainers" protected field that defines additional item containers to search for when using up materials during crafting
            IReflectedField<List<IInventory>> ReflectionResult = Helper.Reflection.GetField<List<IInventory>>(CraftingMenu, nameof(CraftingPage._materialContainers), true);
            List<IInventory> MaterialContainers = ReflectionResult.GetValue();
            if (MaterialContainers != null)
            {
                SearchedBags.AddRange(MaterialContainers.SelectMany(x => x).Where(x => x != null && x is ItemBag).Cast<ItemBag>());
            }

            if (SearchedBags.Any())
            {
                BagsInUse = new HashSet<ItemBag>(SearchedBags);

                if (MaterialContainers == null)
                {
                    MaterialContainers = new List<IInventory>();
                    ReflectionResult.SetValue(MaterialContainers);
                }

                ItemBagInventories = new();

                //  Create a temporary IInventory from the items of each bag, and add the chest to _materialContainers
                foreach (ItemBag IB in SearchedBags.Where(x => AllowUsingBundleBagItemsForCrafting || !(x is BundleBag)))
                {
                    ItemBagCraftingInventory BagInventory = new(IB);
                    ItemBagInventories.Add(BagInventory);
                    MaterialContainers.Add(BagInventory);
                }
            }
        }

        private static void OnCraftingPageDeactivated()
        {
            try
            {
                //  If the player had the CraftingPage of the GameMenu opened, they may have crafted items using materials that were inside their bags,
                //  so we must Resynchronize the bag contents in multiplayer
                if (BagsInUse != null && BagsInUse.Any())
                {
                    ItemBagInventories?.ForEach(x => x.Close());

                    foreach (ItemBag Bag in BagsInUse)
                    {
                        if (Bag is OmniBag OB)
                        {
                            foreach (ItemBag NestedBag in OB.NestedBags)
                                NestedBag.Contents.RemoveAll(x => x == null || x.Stack <= 0);
                        }
                        else
                        {
                            Bag.Contents.RemoveAll(x => x == null || x.Stack <= 0);
                        }

                        Bag.Resync();
                    }
                }
            }
            finally
            {
                BagsInUse = null;
                ItemBagInventories = null;
            }
        }

        private static void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (IsViewingGameMenu && Game1.activeClickableMenu is GameMenu GM)
            {
                int CurrentTab = GM.currentTab;
                if (!PreviousGameMenuTab.HasValue || PreviousGameMenuTab.Value != CurrentTab)
                {
                    OnGameMenuTabChanged(PreviousGameMenuTab, CurrentTab);
                }
            }
        }

        private static int? PreviousGameMenuTab { get; set; }

        private static void OnGameMenuTabChanged(int? PreviousTab, int? CurrentTab)
        {
            try
            {
                GameMenu GM = Game1.activeClickableMenu as GameMenu;
                if (CurrentTab.HasValue && CurrentTab.Value == GameMenu.craftingTab && GM.pages.Any(x => x is CraftingPage))
                {
                    OnCraftingPageActivated(GM.pages.First(x => x is CraftingPage) as CraftingPage);
                }
                else if (PreviousTab.HasValue && PreviousTab.Value == GameMenu.craftingTab)
                {
                    OnCraftingPageDeactivated();
                }
            }
            finally { PreviousGameMenuTab = CurrentTab; }
        }

        private static bool IsViewingGameMenu { get; set; } = false;

        private static void OnGameMenuOpened()
        {
            IsViewingGameMenu = true;
            GameMenu GM = Game1.activeClickableMenu as GameMenu;
            OnGameMenuTabChanged(null, GM.currentTab);
        }

        private static void OnGameMenuClosed()
        {
            OnGameMenuTabChanged(PreviousGameMenuTab, null);
            IsViewingGameMenu = false;
        }

        private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is GameMenu)
            {
                OnGameMenuOpened();
            }
            else if (e.OldMenu is GameMenu)
            {
                OnGameMenuClosed();
            }

            if (IsCompatibleCraftingPage(e.NewMenu))
            {
                OnCraftingPageActivated(e.NewMenu);
            }
            else if (IsCompatibleCraftingPage(e.OldMenu))
            {
                OnCraftingPageDeactivated();
            }
        }

        private static bool IsCompatibleCraftingPage(IClickableMenu Menu)
        {
            return Menu is CraftingPage ||
                (Menu?.GetType().FullName == "CookingSkill.NewCraftingPage" && IsCookingSkillModCompatible); // CookingSkill.NewCraftingPage is a menu defined in the "Cooking Skill" mod
        }
    }

    public class ItemBagCraftingInventory : IInventory
    {
        public ItemBag Source { get; }
        private List<Item> Items { get; }
        private Dictionary<string, List<Item>> ItemsById { get; }
        private Dictionary<Object, List<Object>> SplitStacks { get; } = new();

        public ItemBagCraftingInventory(ItemBag bag)
        {
            Source = bag;
            Items = new();
            ItemsById = new();

            List<List<Object>> ItemLists = new();
            if (bag is OmniBag ob)
            {
                ItemLists.AddRange(ob.NestedBags.Select(x => x.Contents));
            }
            else
            {
                ItemLists.Add(bag.Contents);
            }

            foreach (List<Object> ItemList in ItemLists)
            {
                foreach (Object Item in ItemList)
                {
                    string ItemId = Item.QualifiedItemId;
                    if (Item.Stack > 999)
                    {
                        List<Object> Chunks = new List<Object>();

                        int TotalStack = Item.Stack;
                        int DistributedAmt = 0;
                        while (DistributedAmt < TotalStack)
                        {
                            int CurrentStackAmt = Math.Min(999, TotalStack - DistributedAmt);
                            Object Chunk = ItemBag.CreateCopy(Item);
                            ItemBag.ForceSetQuantity(Chunk, CurrentStackAmt);
                            DistributedAmt += CurrentStackAmt;
                            Chunks.Add(Chunk);
                        }

                        SplitStacks.Add(Item, Chunks);
                        Items.AddRange(Chunks);
                        if (!ItemsById.TryGetValue(ItemId, out List<Item> IndexedItems))
                            ItemsById.Add(ItemId, new List<Item>());
                        ItemsById[ItemId].AddRange(Chunks);
                    }
                    else
                    {
                        Items.Add(Item);
                        if (!ItemsById.TryGetValue(ItemId, out List<Item> IndexedItems))
                            ItemsById.Add(ItemId, new List<Item>());
                        ItemsById[ItemId].Add(Item);
                    }
                }
            }
        }

        public void Close()
        {
            //  Recombine item Stacks that were split to avoid having > 999
            if (SplitStacks != null && SplitStacks.Any())
            {
                foreach (KeyValuePair<Object, List<Object>> KVP in SplitStacks)
                {
                    int NewQuantity = KVP.Value.Sum(x => x.Stack);
                    ItemBag.ForceSetQuantity(KVP.Key, NewQuantity);
                }
            }
        }

        private const string NotSupportedErrorMsg = $"{nameof(ItemBagCraftingInventory)} does not support adding/replacing its items. " +
            $"This object is only intended to be used by crafting pages, and only supports removing items.";

        public Item this[int index]
        {
            get => Items[index];
            set
            {
                //  As of game version 1.6.3.24087, this is the decompiled logic for StardewValley.Item.ConsumeStack(int amount):
                //  Notice that "Stack -= amount" does not execute when Stack-amount<=0
                /*
                if (amount == 0)
                {
                    return this;
                }

                if (Stack - amount <= 0)
                {
                    return null;
                }

                Stack -= amount;
                return this;
                */

                //  CraftingPage.clickCraftingRecipe calls CraftingRecipe.consumeIngredients, which then calls Item.ConsumeStack.
                //  Since the Item.Stack amount isn't decreased when the required amount is >= the current Item.Stack, it causes issues where the bags aren't correctly consuming the inputs
                //  The below if-statement attempts to reconcile this issue
                if (value == null)
                    Items[index].Stack = 0;

                Items[index] = value;
            }
        }

        public long LastTickSlotChanged => throw new NotImplementedException();

        public int Count => Items.Count;

        public bool IsReadOnly => false;

        public void Add(Item item) => throw new InvalidOperationException(NotSupportedErrorMsg);
        public void AddRange(ICollection<Item> collection) => throw new InvalidOperationException(NotSupportedErrorMsg);
        public void Insert(int index, Item item) => throw new InvalidOperationException(NotSupportedErrorMsg);
        public void Clear() => throw new InvalidOperationException(NotSupportedErrorMsg);
        public void OverwriteWith(IList<Item> list) => throw new InvalidOperationException(NotSupportedErrorMsg);
        public void CopyTo(Item[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public bool Contains(Item item) => Items.Contains(item);
        public bool ContainsId(string itemId) => GetById(itemId).Any();
        public bool ContainsId(string itemId, int minimum) => CountId(itemId) >= minimum;
        public int CountId(string itemId) => GetById(itemId).Sum(x => x.Stack);
        public int CountItemStacks() => ItemsById.Values.Sum(x => x.Count);

        public IEnumerable<Item> GetById(string itemId)
        {
            itemId = ItemRegistry.QualifyItemId(itemId);
            if (itemId == null || ItemsById.TryGetValue(itemId, out List<Item> results))
                return Enumerable.Empty<Item>();
            else
                return results;
        }

        public bool HasAny() => Items.Any();
        public bool HasEmptySlots() => Count > CountItemStacks();
        public int IndexOf(Item item) => Items.IndexOf(item);

        public int ReduceId(string itemId, int count)
        {
            if (count <= 0)
                return 0;

            itemId = ItemRegistry.QualifyItemId(itemId);
            if (itemId == null || ItemsById.TryGetValue(itemId, out List<Item> items))
                return 0;

            bool revalidateItemsList = false;

            int totalConsumed = 0;
            int remaining = count;
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];

                int amtToConsume = Math.Min(item.Stack, remaining);
                item.Stack -= amtToConsume;
                remaining -= amtToConsume;
                totalConsumed += amtToConsume;

                if (item.Stack <= 0)
                {
                    revalidateItemsList = true;
                    items.RemoveAt(i);
                    i--;
                }

                if (remaining <= 0)
                    break;
            }

            if (revalidateItemsList)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Item item = Items[i];
                    if (item?.Stack == 0)
                        Items[i] = null;
                }
            }

            return totalConsumed;
        }

        //  These implementations are probably fine... but may need to remove from the ItemsById dictionary too.
        public bool Remove(Item item) => Items.Remove(item);
        public void RemoveAt(int index) => Items.RemoveAt(index);
        public void RemoveRange(int index, int count) => Items.RemoveRange(index, count);

        public bool RemoveButKeepEmptySlot(Item item)
        {
            int Index = IndexOf(item);
            if (Index >= 0)
            {
                Items[Index] = null;
                return true;
            }
            else
                return false;
        }

        public void RemoveEmptySlots()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                if (Items[i] == null)
                    Items.RemoveAt(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
        public IEnumerator<Item> GetEnumerator() => Items.GetEnumerator();
        public IList<Item> GetRange(int index, int count) => Items.GetRange(index, count);
    }
}
