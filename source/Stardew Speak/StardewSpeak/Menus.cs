/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSpeak
{
    public static class Menus
    {
        public static dynamic SerializeMenu(dynamic menu, Point mousePosition) 
        {
            if (menu == null) return null;
            string menuName = menu.GetType().Name;
            var toCall = Utils.GetStaticMethod(typeof(Menus), "Serialize_" + menuName);
            if (toCall == null) return null;
            bool containsMouse = menu.isWithinBounds(mousePosition.X, mousePosition.Y);
            dynamic menuProps = new ExpandoObject();
            menuProps.xPositionOnScreen = menu.xPositionOnScreen;
            menuProps.yPositionOnScreen = menu.yPositionOnScreen;
            menuProps.upperRightCloseButton = Utils.SerializeClickableCmp(menu.upperRightCloseButton, mousePosition);
            menuProps.containsMouse = containsMouse;
            menuProps.classType = menuName;
            _ = toCall.Invoke(null, new object[] { menuProps, menu, mousePosition }) as IDictionary<String, System.Object>;
            var serialized = new Dictionary<String, System.Object>();
            foreach (var property in menuProps)
            {
                serialized[property.Key] = SerializeValue(property.Value, mousePosition);
            }
            return serialized;
        }


        public static void Serialize_OptionsPage(dynamic menu, OptionsPage op, Point cursorPosition)
        {
            menu.menuType = "optionsPage";
            var optionSlots = new List<dynamic>();
            Rectangle? activeDDBounds = null;
            for (int i = 0; i < op.optionSlots.Count; i++) 
            {
                ClickableComponent cmp = op.optionSlots[i];
                OptionsElement element = op.options[op.currentItemIndex + i];
                if (element.greyedOut|| (activeDDBounds.HasValue && cmp.bounds.Intersects((Rectangle)activeDDBounds))) continue;
                if (element is OptionsPlusMinus)
                {
                    OptionsPlusMinus pm = element as OptionsPlusMinus;
                    var minusRect = new Rectangle(cmp.bounds.X + pm.minusButton.X, cmp.bounds.Y + pm.minusButton.Y, pm.minusButton.Width, pm.minusButton.Height);
                    var minusCmp = Utils.Merge(Utils.RectangleToClickableComponent(minusRect, cursorPosition), new { label = pm.label + " minus" });
                    var plusRect = new Rectangle(cmp.bounds.X + pm.plusButton.X, cmp.bounds.Y + pm.plusButton.Y, pm.plusButton.Width, pm.plusButton.Height);
                    var plusCmp = Utils.Merge(Utils.RectangleToClickableComponent(plusRect, cursorPosition), new { label = pm.label + " plus" });
                    optionSlots.Add(minusCmp);
                    optionSlots.Add(plusCmp);

                }
                else if (element is OptionsCheckbox) 
                {
                    var rect = new Rectangle(cmp.bounds.X + element.bounds.X, cmp.bounds.Y + element.bounds.Y, element.bounds.Width, element.bounds.Height);
                    optionSlots.Add(Utils.Merge(Utils.RectangleToClickableComponent(rect, cursorPosition), new { element.label }));
                }
                else if (element is OptionsDropDown)
                {
                    OptionsDropDown odd = element as OptionsDropDown;
                    int y = cmp.bounds.Y + odd.bounds.Y;
                    if (!(System.Object.ReferenceEquals(OptionsDropDown.selected, odd)))
                    {
                        var rect = new Rectangle(cmp.bounds.X + odd.bounds.X, y, element.bounds.Width, element.bounds.Height);
                        var focusTarget = new List<int>{ rect.X + 18, rect.Center.Y };
                        optionSlots.Add(Utils.Merge(Utils.RectangleToClickableComponent(rect, cursorPosition), new { element.label, focusTarget }));
                    }
                    else
                    {
                        activeDDBounds = new Rectangle(cmp.bounds.X + odd.dropDownBounds.X, y, odd.dropDownBounds.Width, odd.dropDownBounds.Height);
                        int heightPerOption = odd.dropDownBounds.Height / odd.dropDownDisplayOptions.Count;
                        for (int j = 0; j < odd.dropDownDisplayOptions.Count; j++)
                        {   
                            string option = odd.dropDownDisplayOptions[j];
                            var optY = y + heightPerOption*j;
                            var rect = new Rectangle(cmp.bounds.X + odd.bounds.X, optY, element.bounds.Width, heightPerOption);
                            var focusTarget = new List<int> { rect.X + 18, rect.Center.Y };
                            optionSlots.Add(Utils.Merge(Utils.RectangleToClickableComponent(rect, cursorPosition), new { label = option, focusTarget }));
                        }
                    }
                }


            }
            menu.optionSlots = optionSlots;
        }
        public static void Serialize_ProfileMenu(dynamic menu, ProfileMenu pm, Point cursorPosition) 
        {
            menu.menuType = "profileMenu";
            menu.backButton = pm.backButton;
            menu.forwardButton = pm.forwardButton;
            menu.previousCharacterButton = pm.previousCharacterButton;
            menu.nextCharacterButton = pm.nextCharacterButton;
            menu.downArrow = pm.downArrow;
            menu.upArrow = pm.upArrow;
            menu.clickableProfileItems = pm.clickableProfileItems;
        }


        public static void Serialize_ShippingMenu(dynamic menu, ShippingMenu sm, Point cursorPosition)
        {
            int introTimer = Utils.GetPrivateField(sm, "introTimer");
            menu.menuType = "shippingMenu";
            if (sm.currentPage == -1)
            {
                if (introTimer <= 0)
                {
                    menu.okButton = sm.okButton;
                }
                menu.categories = sm.categories;
            }
            else 
            {
                menu.backButton = sm.backButton;
                menu.forwardButton = sm.forwardButton;
            }
        }

        public static void Serialize_PurchaseAnimalsMenu(dynamic menu, PurchaseAnimalsMenu pam, Point cursorPosition)
        {
            menu.menuType = "purchaseAnimalsMenu";
            menu.onFarm = Utils.GetPrivateField(pam, "onFarm");
            if (!menu.onFarm) menu.animalsToPurchase = pam.animalsToPurchase;
            menu.okButton = pam.okButton;
            menu.namingAnimal = Utils.GetPrivateField(pam, "namingAnimal");
            if (menu.namingAnimal)
            {
                menu.randomButton = pam.randomButton;
                menu.doneNamingButton = pam.doneNamingButton;
            }
        }

        public static void Serialize_BobberBar(dynamic menu, BobberBar bb, Point cursorPosition)
        {
            menu.menuType = "fishingMenu";
        }

        public static void Serialize_MineElevatorMenu(dynamic menu, MineElevatorMenu mem, Point cursorPosition)
        {
            menu.menuType = "mineElevatorMenu";
            menu.elevators = mem.elevators;
        }

        public static void Serialize_ItemGrabMenu(dynamic menu, ItemGrabMenu igm, Point cursorPosition)
        {
            var lastShippedHolder = igm.shippingBin ? igm.lastShippedHolder : null;
            var itemsToGrabMenu = igm.showReceivingMenu ? igm.ItemsToGrabMenu : null;
            menu.menuType = "itemsToGrabMenu";
            menu.trashCan = igm.trashCan;
            menu.inventoryMenu = igm.inventory;
            menu.itemsToGrabMenu = itemsToGrabMenu;
            menu.okButton = igm.okButton;
            menu.organizeButton = igm.organizeButton;
            menu.shippingBin = igm.shippingBin;
            menu.lastShippedHolder = lastShippedHolder;
            menu.colorPickerToggleButton = igm.colorPickerToggleButton;
            menu.discreteColorPickerCC = igm.discreteColorPickerCC;
            menu.fillStacksButton = igm.fillStacksButton;
            menu.junimoNoteIcon = igm.junimoNoteIcon;
        }

        public static void Serialize_GameMenu(dynamic menu, GameMenu gm, Point cursorPosition)
        {
            menu.menuType = "gameMenu";
            menu.currentPage = gm.pages[gm.currentTab];
            menu.tabs = gm.tabs;
        }
        public static void Serialize_SkillsPage(dynamic menu, SkillsPage page, Point mousePosition)
        {
            menu.menuType = "skillsPage";
            menu.skillAreas = page.skillAreas.Where(x => x.hoverText.Length > 0).ToList();
            menu.skillBars = page.skillBars.Where(x => x.hoverText.Length > 0 && !x.name.Equals("-1")).ToList();
            menu.specialItems = page.specialItems;
        }
        public static void Serialize_CataloguePage(dynamic menu, CataloguePage page, Point mousePosition)
        {
            menu.menuType = "cataloguePage";
            
        }

        public static void Serialize_SocialPage(dynamic menu, SocialPage page, Point mousePosition)
        {
            menu.menuType = "socialPage";
            menu.downArrow = (ClickableTextureComponent)Utils.GetPrivateField(page, "downButton");
            menu.upArrow = (ClickableTextureComponent)Utils.GetPrivateField(page, "upButton");
            menu.slotPosition = (int)Utils.GetPrivateField(page, "slotPosition");
            menu.characterSlots = page.characterSlots.GetRange(menu.slotPosition, 5);
            menu.names = page.names;
        }

        public static void Serialize_GiftLog(dynamic menu, SocialPage page, Point mousePosition)
        {
            menu.menuType = "socialPage";
            menu.downArrow = (ClickableTextureComponent)Utils.GetPrivateField(page, "downButton");
            menu.upArrow = (ClickableTextureComponent)Utils.GetPrivateField(page, "upButton");
            menu.slotPosition = (int)Utils.GetPrivateField(page, "slotPosition");
            menu.characterSlots = page.characterSlots.GetRange(menu.slotPosition, 5);
        }

        public static void Serialize_InventoryPage(dynamic menu, InventoryPage page, Point mousePosition)
        {
            var equipmentIcons = Utils.SerializeComponentList(page.equipmentIcons, mousePosition);
            menu.menuType = "inventoryPage";
            menu.equipmentIcons = page.equipmentIcons;
            menu.inventory = page.inventory;
            menu.trashCan = page.trashCan;
        }
        public static void Serialize_CraftingPage(dynamic menu, CraftingPage page, Point mousePosition)
        {
            int currentCraftingPageIndex = (int)Utils.GetPrivateField(page, "currentCraftingPage");
            var recipePage = page.pagesOfCraftingRecipes[currentCraftingPageIndex];
            var currentRecipePage = new List<List<dynamic>>();
            foreach (var pair in recipePage)
            {
                var r = pair.Value;
                var cmp = Utils.SerializeClickableCmp(pair.Key, mousePosition);
                var recipe = new { r.name, r.description, itemType = r.ItemType };
                currentRecipePage.Add(new List<dynamic> { cmp, recipe });
            }
            menu.menuType = "craftingPage";
            menu.currentCraftingPageIndex = currentCraftingPageIndex;
            menu.downArrow = page.downButton;
            menu.currentRecipePage = currentRecipePage;
            menu.inventory = page.inventory;
            menu.trashCan = page.trashCan;
            menu.upArrow = page.upButton;
        }
        public static void Serialize_CollectionsPage(dynamic menu, CollectionsPage page, Point mousePosition)
        {
            menu.menuType = "collectionsPage";
            int currentPageNumber = page.currentPage;
            var currentCollection = page.collections[page.currentTab];
            var currentPage = currentCollection[currentPageNumber];
            if (currentPageNumber > 0) menu.backButton = page.backButton;
            if (currentPageNumber < currentCollection.Count - 1) menu.forwardButton = page.forwardButton;
            menu.currentPage = currentPage;
            menu.tabs = page.sideTabs.Values.ToList();
        }

        public static void Serialize_FarmInfoPage(dynamic menu, FarmInfoPage page, Point mousePosition)
        {
            menu.menuType = "farmInfoPage";
        }
        public static void Serialize_ExitPage(dynamic menu, ExitPage page, Point mousePosition)
        {
            menu.menuType = "exitPage";
            menu.exitToDesktop = page.exitToDesktop;
            menu.exitToTitle = page.exitToTitle;
        }

    public static dynamic SerializeValue(dynamic val, Point cursorPosition)
        {
            if (val is ClickableComponent || val is ClickableTextureComponent)
            {
                return Utils.SerializeClickableCmp(val, cursorPosition);
            }
            if (val is IClickableMenu) 
            {
                return Utils.SerializeMenu(val);
            }
            if (val is IList && val.GetType().IsGenericType)
            {
                var listVal = new List<dynamic>();
                foreach (var item in val)
                {
                    listVal.Add(SerializeValue(item, cursorPosition));
                }
                return listVal;
            }
            return val;
        }   

    }
}
