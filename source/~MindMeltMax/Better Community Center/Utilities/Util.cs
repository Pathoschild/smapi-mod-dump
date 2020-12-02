/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using BCC.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BCC.Utilities
{
    public class Util
    {
        public static ClickableTextureComponent nextFridgeButton;
        public static ClickableTextureComponent previousFridgeButton;

        public static ItemGrabMenu menu = null;
        public static VaultGold Stored;

        public static int CurrentFridgeTab = 1;
        public static int item = new int();
        public static int CompletedRequestIndex = 0;

        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static ITranslationHelper i18n;

        public static List<NPC> allNPCS = new List<NPC>();
        public static List<string> NPCnamesToday = new List<string>();
        public static List<string> NPCnamesYesterday = new List<string>();
        public static List<SObject> items = new List<SObject>();
        public static List<int> Categories = new List<int>();
        public static List<int> RequestableItemCategories = new List<int>();
        public static List<Response> responses;
        public static List<Request> removableRequests;

        public static string previousName;
        public static string previousDonatedName;
        public static string CompletionString;

        public Util(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;

            i18n = Helper.Translation;

            Categories.Add(-4);
            Categories.Add(-5);
            Categories.Add(-6);
            Categories.Add(-14);
            Categories.Add(-25);
            Categories.Add(-26);
            Categories.Add(-27);
            Categories.Add(-75);
            Categories.Add(-79);
            Categories.Add(-81);

            RequestableItemCategories.Add(-2);
            RequestableItemCategories.Add(-4);
            RequestableItemCategories.Add(-5);
            RequestableItemCategories.Add(-6);
            RequestableItemCategories.Add(-7);
            RequestableItemCategories.Add(-8);
            RequestableItemCategories.Add(-12);
            RequestableItemCategories.Add(-14);
            RequestableItemCategories.Add(-25);
            RequestableItemCategories.Add(-26);
            RequestableItemCategories.Add(-27);
            RequestableItemCategories.Add(-75);
            RequestableItemCategories.Add(-79);
            RequestableItemCategories.Add(-80);
            RequestableItemCategories.Add(-81);

            responses = new List<Response>();
            responses.Add(new Response("Deposit", i18n.Get("Deposit")));
            responses.Add(new Response("Withdraw", i18n.Get("Withdraw")));
            responses.Add(new Response("Close", i18n.Get("Close")));
        }

        #region Kitchen
        public static bool IsCookableItem(Item i) => i != null && (i.Category == -4 || i.Category == -5 || i.Category == -6 || i.Category == -14 || i.Category == -25 || i.Category == -26 || i.Category == -27 || i.Category == -75 || i.Category == -79 || i.Category == -81);
        public static bool IsEdibleCookableItem(Item i) => i != null && IsCookableItem(i) && i.staminaRecoveredOnConsumption() > 0 || i.healthRecoveredOnConsumption() > 0;

        public static void PantryExit(bool hasDonated)
        {
            /*if (hasDonated)
            {
                foreach(Item item in PantryMenu.ItemsDonated)
                {
                    if (item.Name != previousDonatedName)
                    {
                        BCC.TodaysDonations += $"{Game1.player.Name} : {item.Name} X {PantryMenu.Amount}^";
                        previousDonatedName = item.Name;
                    }
                    else
                    {
                        BCC.TodaysDonations.Replace($"{Game1.player.Name} : {item.Name} X {PantryMenu.Amount}^", $"{Game1.player.Name} : {item.Name} X {PantryMenu.Amount}^");
                    }  
                }
            }*/
        }

        public static void addFridgeSelectionButtons()
        {
            if (Game1.activeClickableMenu is ItemGrabMenu)
            {
                menu = Game1.activeClickableMenu as ItemGrabMenu;
                menu.chestColorPicker = null;
                menu.colorPickerToggleButton = null;

                int X = menu.xPositionOnScreen + Game1.tileSize / 2;
                int Y = (int)(menu.yPositionOnScreen + Game1.tileSize * 3.3F);

                nextFridgeButton = new ClickableTextureComponent(new Rectangle(X + 12 + 12 * Game1.tileSize, Y + 24, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, true);
                previousFridgeButton = new ClickableTextureComponent(new Rectangle(X + -Game1.tileSize, Y + 24, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, true);
                //Helper.Events.Input.ButtonPressed += Input_ButtonPressed_Util;
            }
        }

        public static void getAllVillagers()
        {
            foreach (GameLocation loc in Game1.locations)
            {
                foreach (NPC person in loc.getCharacters())
                {
                    if (person.isVillager())
                    {
                        allNPCS.Add(person);
                    }
                }
            }
        }

        public static string getRandomNPCName(int which, List<NPC> npcs)
        {
            for (int i = 0; i < npcs.Count; ++i)
            {
                if (i == which)
                {
                    var npc = npcs[i];
                    return npc.Name;
                }
            }
            return null;
        }

        public static void getNpcsForDonation(int count)
        {
            getAllVillagers();
            List<NPC> npcs = Util.allNPCS;
            for (int i = 0; i < count; ++i)
            {
                int RndmNum = Game1.random.Next(1, npcs.Count);
                string name = getRandomNPCName(RndmNum, npcs);
                if (name == "Dwarf" || name == "Wizard" || name == "Krobus" || name == "Sandy" || name.Contains("Qi"))
                    getNpcsForDonation(count - i);
                if (previousName != name)
                {
                    NPCnamesToday.Add(name);
                    previousName = name;
                }
                else
                {
                    getNpcsForDonation(count - i);
                }
            }
            for (int i = NPCnamesToday.Count; i > 3; --i)
                NPCnamesToday.RemoveAt(i-1);
        }

        public static void populateDonatableItems()
        {
            for(int i=0; i<=815; i++)
            {
                SObject tempSObject = new SObject(Vector2.Zero, i, 1);
                for(int e=0; e<Categories.Count; e++)
                {
                    if (tempSObject != null && tempSObject.Category == Categories[e])
                        items.Add(tempSObject);
                }
            }
        }

        public static SObject getRandomItemForDonation(NPC Donator)
        {
            if(items.Count == 0)
                populateDonatableItems();
            int e = Game1.random.Next(items.Count);
                SObject donationItem = items[e];
            for (int i = 0; i < Categories.Count; i++)
            {
                if (donationItem != null && donationItem.category == Categories[i])
                {
                    if (donationItem.Category == -81 && donationItem.edibility > 0)
                        return donationItem;
                    else if (donationItem.Category != -81)
                        return donationItem;
                    else if (donationItem.Category == -81 && donationItem.edibility <= 0)
                        getRandomItemForDonation(Donator);
                }
            }
            return null;
        }

        public static bool DoesChestHaveItem(Item item)
        {
            if (PantryMenu.CCFridgeChests != null)
            {
                foreach(Chest chest in PantryMenu.CCFridgeChests)
                {
                    if (chest.items.Contains(item))
                        return true;
                }
            }
            return false;
        }


        //Discarded For Now
        /*private static void Input_ButtonPressed_Util(SObject sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {

            Point mouse = new Point(Game1.getMouseX(), Game1.getMouseY());

            if (Util.nextFridgeButton.containsPoint(mouse.X, mouse.Y) && Game1.activeClickableMenu is ItemGrabMenu)
                Util.nextFridgeTab();
            else if (Util.previousFridgeButton.containsPoint(mouse.X, mouse.Y) && Game1.activeClickableMenu is ItemGrabMenu)
                Util.previousFridgeTab();
        }

        public static void nextFridgeTab()
        {
            if (CurrentFridgeTab != 5)
            {
                ++CurrentFridgeTab;
                openFridge(CurrentFridgeTab);
            }
            else
                return;
        }

        public static void previousFridgeTab()
        {
            if(CurrentFridgeTab != 1)
            {
                --CurrentFridgeTab;
                openFridge(CurrentFridgeTab);
            }
            else
                return;
        }

        public static void openFridge(int Tab)
        {
            switch (Tab)
            {
                case 1:
                    Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)BCC.CCFridge1Chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge1Chest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge1Chest.grabItemFromChest),
                                                    false, true, true, true, true, 1, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)BCC.CCFridge1Chest.fridge ? (Item)null : (Item)BCC.CCFridge1Chest), context: ((SObject)BCC.CCFridge1Chest));
                    break;
                case 2:
                    Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)BCC.CCFridge2Chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge2Chest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge2Chest.grabItemFromChest),
                                                    false, true, true, true, true, 2, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)BCC.CCFridge2Chest.fridge ? (Item)null : (Item)BCC.CCFridge2Chest), context: ((SObject)BCC.CCFridge2Chest));
                    break;
                case 3:
                    Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)BCC.CCFridge3Chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge3Chest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge3Chest.grabItemFromChest),
                                                    false, true, true, true, true, 3, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)BCC.CCFridge3Chest.fridge ? (Item)null : (Item)BCC.CCFridge3Chest), context: ((SObject)BCC.CCFridge3Chest));
                    break;
                case 4:
                    Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)BCC.CCFridge4Chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge4Chest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge4Chest.grabItemFromChest),
                                                    false, true, true, true, true, 4, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)BCC.CCFridge4Chest.fridge ? (Item)null : (Item)BCC.CCFridge4Chest), context: ((SObject)BCC.CCFridge4Chest));
                    break;
                case 5:
                    Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)BCC.CCFridge5Chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge5Chest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge5Chest.grabItemFromChest),
                                                    false, true, true, true, true, 5, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)BCC.CCFridge5Chest.fridge ? (Item)null : (Item)BCC.CCFridge5Chest), context: ((SObject)BCC.CCFridge5Chest));
                    break;
                default:
                    Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu((IList<Item>)BCC.CCFridge1Chest.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                                                    new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge1Chest.grabItemFromInventory), (string)null, new ItemGrabMenu.behaviorOnItemSelect(BCC.CCFridge1Chest.grabItemFromChest),
                                                    false, true, true, true, true, 1, sourceItem: ((bool)(NetFieldBase<bool, NetBool>)BCC.CCFridge1Chest.fridge ? (Item)null : (Item)BCC.CCFridge1Chest), context: ((SObject)BCC.CCFridge1Chest));
                    break;
            }
        }*/

        #endregion Kitchen


        #region CommonArea

        public static void innitCunt()
        {

        }

        #endregion CommonArea


        #region Vault

        public static bool openVault()
        {
            string text = i18n.Get("Stored") + " : " + Stored.StoredGold.ToString() + "g.";
            Game1.currentLocation.createQuestionDialogue(text, responses.ToArray(), VaultMenu);
            return true;
        }
        private static void VaultMenu(Farmer who, string key)
        {
            if (key == "Close")
                return;
            string text = responses.Find(k => k.responseKey == key).responseText;
            Game1.activeClickableMenu = new NumberSelectionMenu(text, (nr, cost, farmer) => process(nr, cost, farmer, key), -1, 0, (key != "Withdraw") ? (int)Game1.player.Money : (int)Stored.StoredGold);
        }

        private static void process(int num, int cost, Farmer who, string key)
        {
            if(key == "Deposit")
            {
                Game1.player.Money -= num;
                Stored.StoredGold += num;
            }
            else if(key == "Withdraw")
            {
                Game1.player.totalMoneyEarned -= (uint)num;
                Game1.player.Money += num;
                Stored.StoredGold -= num;
            }
            Game1.activeClickableMenu = null;
        }

        #endregion Vault


        #region Bulletin

        public static List<SObject> getAllItemsForRequestList()
        {
            //Look into Game1.SObjectInformation
            List<SObject> AllSObjects = new List<SObject>();
            for (int i = 0; i <= 815; i++)
            {
                SObject tempSObject = new SObject(Vector2.Zero, i, 1);
                AllSObjects.Add(tempSObject);
            }
            return AllSObjects;
        }

        public static bool IsRequestableItem(int key)
        {
            if (Game1.player.basicShipped.ContainsKey(key) || Game1.player.mineralsFound.ContainsKey(key) || Game1.player.archaeologyFound.ContainsKey(key) || Game1.player.fishCaught.ContainsKey(key))
            {
                return true;
            }
            return false;
        }

        public static string getItemName(int index)
        {
            string[] array = Game1.objectInformation[index].Split('/');

            string name = array[0];
            return name;
        }

        public static string getItemDescription(int index)
        {
            string[] array = Game1.objectInformation[index].Split('/');
            string description = array[5];
            return description;
        }

        public static void createItemRequest(int itemIndex, int itemCount)
        {
            if (Requests.RequestList.Count >= 3)
                return;
            string[] array = Game1.objectInformation[itemIndex].Split('/');
            int itemPrice = Convert.ToInt32(array[1]);
            int totalPrice = itemCount * (itemPrice * (Game1.random.Next((int)1.5, (int)2.5)));
            if (totalPrice > itemCount * (itemPrice * 2))
                totalPrice = itemCount * (itemPrice * 2);
            int dateToday = Game1.Date.TotalDays;

            Requests.RequestList.Add(new Request(itemIndex, itemCount, totalPrice, dateToday));

            Game1.activeClickableMenu = (IClickableMenu)new ItemRequestBoard(Monitor);
        }

        public static void TryToRemoveRequests()
        {
            int Days = Game1.Date.TotalDays;
            foreach (Request r in Requests.RequestList)
            {
                if (Days - r.CreationDate <= 7 && CompletedRequestIndex == 0)
                    return;
                else if (Days - r.CreationDate > 7 || CompletedRequestIndex == r.itemIndex)
                {
                    removableRequests.Add(r);
                    break;
                }
            }
            foreach (Request r in removableRequests)
                Requests.RequestList.Remove(r);
            Game1.activeClickableMenu = (IClickableMenu)new DialogueBox($"Old and completed requests have been cleared");
            CompletedRequestIndex = 0;
            CompletionString = null;
        }

        public static void CompleteRequest(int itemIndex)
        {
            getAllVillagers();
            List<NPC> npcs = Util.allNPCS;
            int RndmNum = Game1.random.Next(1, npcs.Count);
            string npcName = getRandomNPCName(RndmNum, npcs);
            foreach (Request r in Requests.RequestList)
            {
                if(r.itemIndex == itemIndex)
                {
                    CompletedRequestIndex = itemIndex;
                    CompletionString = $"Request filled by {npcName}";
                }
            }
        }

        public static void CPBulletinDialogueExit()
        {
            Game1.activeClickableMenu = (IClickableMenu)new ItemRequestBoard(Monitor);
        }

        #endregion Bulletin


        #region Boiler

        /*public static bool IsFuelItem(Item i) => i != null && i.parentSheetIndex == 382 || (i.parentSheetIndex == 388 && i.Stack >= 20) || (i.parentSheetIndex == 709 && i.Stack >= 5);
        public static bool IsSmeltableItem(Item i) => i != null && (i.Name.ToLower().Contains("ore") || i.Name.ToLower().Contains("quartz")) && (i.category.Value == -12 || i.category.Value == -2 || i.category.Value == -15);
        public static bool IsStackedSmeltableItem(Item i) => i != null && IsSmeltableItem(i) && i.Stack >= 5;
        public static void CoalChestExit()
        {
        }

        public static void BoilerExit()
        {
            foreach(ClickableComponent c in BoilerMenu.inputComponents)
            {
                if(c.item != null)
                {
                    BoilerData.dataList.Add(new data(c.item.parentSheetIndex, c.item.Stack, c.myID));
                }
            }
            foreach (ClickableComponent c in BoilerMenu.outputComponents)
            {
                if (c.item != null)
                {
                    BoilerData.dataList.Add(new data(c.item.parentSheetIndex, c.item.Stack, c.myID));
                }
            }
            Helper.Events.GameLoop.UpdateTicked += tempAwait;
        }

        private static void tempAwait(object sender, UpdateTickedEventArgs e)
        {
            BoilerMenu menu = new BoilerMenu();
            if (!Game1.isTimePaused)
            {
                bool temp = menu.checkIfCanSmelt();
                if (temp)
                {
                    Helper.Events.GameLoop.UpdateTicked -= tempAwait;
                }
            }
        }

        public static void Multi_Smelt(SObject smeltable, bool probe, Farmer who)
        {
            //Hold Unitll later
        }*/

        #endregion Boiler
    }
}
