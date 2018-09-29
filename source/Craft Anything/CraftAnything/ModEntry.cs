using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace CraftAnything
{
    public class ModEntry : Mod
    {
        public static IModHelper helper;

        public class ListenerCraftingPage : CraftingPage
        {
            private ItemReplacer replacer = new ItemReplacer();
            bool cooking = false;

            public ListenerCraftingPage(CraftingPage page, ItemReplacer replacer) :
                base(page.xPositionOnScreen, page.yPositionOnScreen, page.width, page.height,
                    ModEntry.helper.Reflection.GetField<bool>(page, "cooking").GetValue())
            {
                this.cooking = ModEntry.helper.Reflection.GetField<bool>(page, "cooking").GetValue();
                this.replacer = replacer;
            }

            private void replaceHeldItem(int baseQuantity = 0)
            {
                Item heldItem = ModEntry.helper.Reflection.GetField<Item>(this, "heldItem").GetValue();
                if (heldItem == null)
                {
                    return;
                }

                var newItem = this.replacer.ReplaceItem(heldItem);

                // No replacement available, exit out
                if (newItem == heldItem) 
                    return;

                if (baseQuantity > 0)
                {
                    // How many of this craft have we actually done?
                    int multiplier = heldItem.Stack / baseQuantity;
                    // Modify the replacement item to match
                    newItem.Stack *= multiplier;
                }
                ModEntry.helper.Reflection.GetField<Item>(this, "heldItem").SetValue(newItem);
            }

            private bool shouldReplace(int x, int y, out Item itemToCraft)
            {
                int currentCraftingPage = ModEntry.helper.Reflection.GetField<int>(this, "currentCraftingPage").GetValue();
                itemToCraft = null;

                foreach (ClickableTextureComponent key in pagesOfCraftingRecipes[currentCraftingPage].Keys)
                {
                    if (key.containsPoint(x, y) && !key.hoverText.Equals("ghosted") && this.pagesOfCraftingRecipes[currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(this.cooking ? this.fridge() : (IList<Item>)null))
                    {
                        itemToCraft = this.pagesOfCraftingRecipes[currentCraftingPage][key].createItem();
                        return true;
                    }
                }

                return false;
            }

            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                bool replace = this.shouldReplace(x, y, out Item itemToCraft);

                base.receiveLeftClick(x, y, playSound);
                if (replace)
                    this.replaceHeldItem(itemToCraft.Stack);
            }

            public override void receiveRightClick(int x, int y, bool playSound = true)
            {
                bool replace = this.shouldReplace(x, y, out Item itemToCraft);
                itemToCraft = replacer.ReplaceItem(itemToCraft);
                Item heldItem = ModEntry.helper.Reflection.GetField<Item>(this, "heldItem").GetValue();


                if (itemToCraft != null && heldItem != null && itemToCraft.Name == heldItem.Name)
                {
                    ModEntry.helper.Reflection.GetField<Item>(this, "heldItem").SetValue(null);
                    base.receiveRightClick(x, y, playSound);
                    if (replace)
                        this.replaceHeldItem();
                    heldItem.addToStack(itemToCraft.Stack);
                    ModEntry.helper.Reflection.GetField<Item>(this, "heldItem").SetValue(heldItem);
                }
                else
                {
                    base.receiveRightClick(x, y, playSound);
                    if (replace)
                        this.replaceHeldItem();
                }

            }
        }

        private ItemReplacer replacer = new ItemReplacer();

        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;

            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        public void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            System.Console.Write($"Type of new menu is {e.NewMenu.GetType().ToString()}");

            if (e.NewMenu is CraftingPage page && !(e.NewMenu is ListenerCraftingPage))
            {
                ListenerCraftingPage replacement = new ListenerCraftingPage(page, this.replacer);
                Game1.activeClickableMenu = replacement;
            }

            if (e.NewMenu is GameMenu)
            {
                IReflectedField<List<IClickableMenu>> pages = this.Helper.Reflection.GetField<List<IClickableMenu>>(e.NewMenu, "pages");
                var menuPages = pages.GetValue();
                for (int i = 0; i < menuPages.Count; i++) {
                    if (menuPages[i] is CraftingPage craftingPage)
                    {
                        menuPages[i] = new ListenerCraftingPage(craftingPage, this.replacer);
                        break;
                    }
                }

                pages.SetValue(menuPages);
            }
        }

        public override object GetApi()
        {
            return this.replacer;
        }
    }
}
