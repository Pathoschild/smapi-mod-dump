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
using ItemBags.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ItemBags
{
    public static class AutofillHandler
    {
        private static IModHelper Helper { get; set; }
        private static IMonitor Monitor { get { return ItemBagsMod.ModInstance.Monitor; } }

        /// <summary>Adds Autofill feature, which allows picked up items to automatically be placed inside of bags in the player's inventory</summary>
        internal static void OnModEntry(IModHelper Helper)
        {
            AutofillHandler.Helper = Helper;
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
            Helper.Events.Player.InventoryChanged += Player_InventoryChanged;
            Helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        }

        private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (IsChestMenu(e.OldMenu, out Item PreviousMenuHeldItem) || IsChestMenu(e.NewMenu, out Item NewMenuHeldItem))
            {
                // Apparently, adding or removing items to/from a chest still fires a Display.MenuChanged event. Not sure if that's intentional but whatever
                bool IsCompletelyNewMenu = e.OldMenu != e.NewMenu && (!(e.OldMenu is ItemGrabMenu OldIGM) || !(e.NewMenu is ItemGrabMenu NewIGM) || OldIGM.context != NewIGM.context);
                if  (IsCompletelyNewMenu)
                    PreviousChestMenuContents = null;
            }

            //  Prevent items that were just removed from a bag from getting immediately autofilled into another bag the moment they close the bag menu
            if (e.OldMenu is ItemBagMenu || e.NewMenu is ItemBagMenu)
            {
                PreviousChestMenuContents = null;
            }
        }

        private static Item PreviousInventoryCursorSlotItem = null;
        private static Item PreviousGeodeMenuCursorSlotItem = null;
        private static Item PreviousFishingChestCursorSlotItem = null;
        private static List<Item> PreviousChestMenuContents = null;

        private static void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                PreviousInventoryCursorSlotItem = Game1.player.CursorSlotItem;

                //  Geode menu
                if (Game1.activeClickableMenu is GeodeMenu GM)
                    PreviousGeodeMenuCursorSlotItem = GM.heldItem;
                else
                    PreviousGeodeMenuCursorSlotItem = null;

                //  Fishing Treasure menu
                if (IsFishingTreasureChestMenu(Game1.activeClickableMenu, out Item FishingChestHeldItem))
                    PreviousFishingChestCursorSlotItem = FishingChestHeldItem;
                else
                    PreviousFishingChestCursorSlotItem = null;

                try
                {
                    //  Chest menu
                    if (ItemBagsMod.UserConfig.AllowAutofillInsideChest && IsChestMenu(Game1.activeClickableMenu, out Item ChestHeldItem))
                    {
                        ItemGrabMenu IGM = Game1.activeClickableMenu as ItemGrabMenu;
                        Chest Chest = IGM.context as Chest;

                        //  Compare previous chest items to current chest items to see if any items were just added to chest
                        List<Item> CurrentChestMenuContents = Chest.Items.Where(x => x != null).ToList();
                        if (PreviousChestMenuContents != null)
                        {
                            List<Item> AddedItems = CurrentChestMenuContents.Where(x => !PreviousChestMenuContents.Contains(x)).ToList();
                            if (AddedItems.Any())
                                OnItemsAddedToChest(IGM, Chest, AddedItems);
                        }

                        PreviousChestMenuContents = CurrentChestMenuContents;
                    }
                }
                catch (Exception ex)
                {
                    PreviousChestMenuContents = null;
                    ItemBagsMod.ModInstance.Monitor.Log(string.Format("Error while attempting to autofill bags inside of the active chest: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error);
                }
            }
        }

        private static void OnItemsAddedToChest(ItemGrabMenu ChestMenu, Chest Chest, IList<Item> NewItemStacks)
        {
            if (ItemBagsMod.UserConfig.AllowAutofillInsideChest)
            {
                //  Autofill the added items to any bags inside of the chest
                List<ItemBag> AutofillableBags = GetAutofillableBags(Chest.Items, out HashSet<ItemBag> NestedBags);
                if (AutofillableBags.Any())
                {
                    foreach (Item NewItem in NewItemStacks)
                    {
                        TryAutofill(Chest.Items, AutofillableBags, NestedBags, NewItem, out int AutofilledQuantity);
                    }
                }
            }
        }

        private static bool IsFishingTreasureChestMenu(IClickableMenu Menu, out Item HeldItem)
        {
            if (Menu is ItemGrabMenu IGM && IGM.context is FishingRod)
            {
                HeldItem = IGM.heldItem;
                return true;
            }
            else
            {
                HeldItem = null;
                return false;
            }
        }

        private static bool IsChestMenu(IClickableMenu Menu, out Item HeldItem)
        {
            if (Menu is ItemGrabMenu IGM && IGM.context is Chest)
            {
                HeldItem = IGM.heldItem;
                return true;
            }
            else
            {
                HeldItem = null;
                return false;
            }
        }

        private const int HayId = 178;
        private static bool IsHay(Item item)
        {
            return item != null && item.ParentSheetIndex == HayId && item is Object obj && !obj.bigCraftable.Value;
        }

        private static bool IsHandlingInventoryChanged { get; set; } = false;

        private static void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            bool CanAutofill = false;
            if (e.IsLocalPlayer && !IsHandlingInventoryChanged)
            {
                if (Game1.activeClickableMenu == null)
                {
                    CanAutofill = true;
                }
                else
                {
                    if (Game1.activeClickableMenu is DialogueBox DB)
                    {
                        IReflectedField<Dialogue> ReflectionDialogue = Helper.Reflection.GetField<Dialogue>(DB, "characterDialogue", false);
                        //  (characterDialogue is null when attempting to eat a food item)
                        CanAutofill = ReflectionDialogue != null && ReflectionDialogue.GetValue() == null;
                    }
                    else if (Game1.activeClickableMenu is GameMenu GameMenu && GameMenu.currentTab != GameMenu.craftingTab)
                    {
                        //  Ignore InventoryChanged events that are due to an inventory item being dragged/dropped to/from the cursor
                        bool DidCursorSlotItemChange = Game1.player.CursorSlotItem != PreviousInventoryCursorSlotItem;
                        CanAutofill = !DidCursorSlotItemChange;
                    }
                    else if (Game1.activeClickableMenu is GeodeMenu GeodeMenu)
                    {
                        //  Ignore InventoryChanged events that are due to an inventory item being dragged/dropped to/from the cursor
                        bool DidCursorSlotItemChange = GeodeMenu.heldItem != PreviousGeodeMenuCursorSlotItem;
                        CanAutofill = !DidCursorSlotItemChange;
                    }
                    else if (IsFishingTreasureChestMenu(Game1.activeClickableMenu, out Item FishingChestHeldItem))
                    {
                        //  Ignore InventoryChanged events that are due to an inventory item being dragged/dropped to/from the cursor
                        bool DidCursorSlotItemChange = FishingChestHeldItem != PreviousFishingChestCursorSlotItem;
                        CanAutofill = !DidCursorSlotItemChange;
                    }

                    //Possible TODO
                    //  Improve autofilling logic by allowing autofill even if certain menus are open.
                    //  Currently, autofilling is disabled while a menu is active (with certain exceptions), because we don't know if the inventory changed event is due to an item being picked up, or due to some other action happening.
                    //  (such as by receiving a gift in the mail, or buying from a shop, or from a quest reward, or using a mod command to spawn items in inventory etc).

                    if (Game1.CurrentEvent != null)
                        CanAutofill = false;
                }
            }

            if (CanAutofill)
            {
                try
                {
                    IsHandlingInventoryChanged = true;

                    //  Get all bags in the player's inventory that can be autofilled
                    List<ItemBag> AutofillableBags = GetAutofillableBags(e.Player.Items, out HashSet<ItemBag> NestedBags);

                    if (AutofillableBags.Any())
                    {
                        foreach (Item NewItem in e.Added)
                        {
                            //  Skip autofilling hay that was just grabbed from the hopper, since the player probably wants to place it on the feeding bench
                            if (IsHay(NewItem) && Game1.player.currentLocation is AnimalHouse && Game1.activeClickableMenu == null)
                                continue;

                            TryAutofill(Game1.player.Items, AutofillableBags, NestedBags, NewItem, out int AutofilledQuantity);
                        }
                    }
                }
                finally
                {
                    IsHandlingInventoryChanged = false;
                }
            }
        }

        internal static bool TryAutofill(IList<Item> Source, List<ItemBag> AutofillableBags, HashSet<ItemBag> NestedBags, Item Item, out int AutofilledQuantity)
        {
            AutofilledQuantity = 0;

            if (AutofillableBags.Any() && Item != null && Item is Object Obj && Item.Stack > 0)
            {
                List<ItemBag> ValidTargets = new List<ItemBag>();
                foreach (ItemBag Bag in AutofillableBags.Where(x => x.IsValidBagObject(Obj) && !x.IsFull(Obj)))
                {
                    //  Don't allow Rucksacks to be autofilled with the new item unless they already have an existing stack of it
                    if (Bag is Rucksack && !Bag.Contents.Any(x => x != null && ItemBag.AreItemsEquivalent(Obj, x, false, true)))
                        continue;
                    //  Don't allow standard bags to be autofilled with items that the user explicitly set to ignore
                    else if (Bag is BoundedBag BB && !BB.CanAutofillWithItem(Item as Object))
                        continue;
                    else
                        ValidTargets.Add(Bag);
                }

                if (ValidTargets.Any())
                {
                    List<ItemBag> SortedTargets = ValidTargets.OrderBy(x =>
                    {
                        int NestedPenalty = NestedBags.Contains(x) ? 10 : 0; // Items nested inside of Omni Bags have lower priority than non-nested bags
                        if (x is BundleBag)
                            return 0 + NestedPenalty; // Prioritize filling Bundle Bags first
                        else if (x is Rucksack RS)
                        {
                            int Priority = RS.AutofillPriority == AutofillPriority.High ? 1 : 4;
                            return Priority + NestedPenalty; // Prioritize Rucksacks with HighPriority over BoundedBags
                        }
                        else if (x is BoundedBag BB)
                        {
                            //  Prioritize BoundedBags that already have an existing stack of the item over BoundedBags that don't
                            if (x.Contents.Any(BagItem => BagItem != null && ItemBag.AreItemsEquivalent(Obj, BagItem, false, true)))
                                return 2 + NestedPenalty;
                            else
                                return 3 + NestedPenalty;
                        }
                        else
                            throw new NotImplementedException(string.Format("Unexpected Bag type in Autofill sorter: {0}", x.GetType().ToString()));
                    }).ToList();

                    for (int i = 0; i < SortedTargets.Count; i++)
                    {
                        ItemBag Target = SortedTargets[i];
                        Target.MoveToBag(Obj, Obj.Stack, out int MovedQty, false, Source);
                        AutofilledQuantity += MovedQty;
                        if (MovedQty > 0 && ItemBagsMod.UserConfig.ShowAutofillMessage)
                        {
                            //Game1.addHUDMessage(new HUDMessage(string.Format("Moved {0} to {1}", Item.DisplayName, Target.DisplayName), MovedQty, true, Color.White, Target));
                            HUDMessage msg = new HUDMessage($"Moved {Item.DisplayName} to {Target.DisplayName}");
                            msg.number = MovedQty;
                            msg.messageSubject = Target;
                            msg.type = Target.DisplayName;
                            Game1.addHUDMessage(msg);
                        }

                        if (Obj.Stack <= 0)
                            break;
                    }
                }
            }

            return AutofilledQuantity > 0;
        }

        internal static List<ItemBag> GetAutofillableBags(IList<Item> SourceItems, out HashSet<ItemBag> NestedBags)
        {
            NestedBags = new HashSet<ItemBag>();

            List<ItemBag> AutofillableBags = new List<ItemBag>();
            foreach (Item Item in SourceItems)
            {
                if (Item != null && Item is ItemBag)
                {
                    if (Item is BoundedBag BB)
                    {
                        if (BB.Autofill)
                            AutofillableBags.Add(BB);
                    }
                    else if (Item is Rucksack RS)
                    {
                        if (RS.Autofill)
                            AutofillableBags.Add(RS);
                    }
                    else if (Item is OmniBag OB)
                    {
                        foreach (ItemBag NestedBag in OB.NestedBags)
                        {
                            if (NestedBag is BoundedBag NestedBB)
                            {
                                if (NestedBB.Autofill)
                                {
                                    AutofillableBags.Add(NestedBB);
                                    NestedBags.Add(NestedBag);
                                }
                            }
                            else if (NestedBag is Rucksack NestedRS)
                            {
                                if (NestedRS.Autofill)
                                {
                                    AutofillableBags.Add(NestedRS);
                                    NestedBags.Add(NestedBag);
                                }
                            }
                        }
                    }
                }
            }
            return AutofillableBags;
        }
    }
}
