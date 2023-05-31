/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using Omegasis.Revitalize.Framework.Menus;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Omegasis.StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;
using Omegasis.Revitalize.Framework.Crafting.JsonContent;

namespace Omegasis.Revitalize.Framework.Crafting
{
    public class CraftingRecipeBook
    {


        /// <summary>
        /// All of the crafting recipes contained by this crafting list.
        /// </summary>
        public Dictionary<string, UnlockableCraftingRecipe> craftingRecipes;


        /// <summary>
        /// All of the crafting tabs to be used for the menu.
        /// </summary>
        public Dictionary<string, AnimatedButton> craftingMenuTabs;

        /// <summary>
        /// Which group of crafting recipes this book belongs to.
        /// </summary>
        public string craftingRecipeBookId;

        public string defaultTab;

        public CraftingRecipeBook()
        {

        }

        /// <summary>
        /// Constructor used when loading crafting recipes from disk.
        /// </summary>
        /// <param name="CraftingBookId"></param>
        /// <param name="CraftingTabs"></param>
        /// <param name="CraftingRecipes"></param>
        public CraftingRecipeBook(JsonCraftingRecipeBookDefinition CraftingBookId, List<JsonCraftingMenuTab> CraftingTabs, List<UnlockableJsonCraftingRecipe> CraftingRecipes)
        {

            this.craftingRecipeBookId = CraftingBookId.craftingRecipeBookId;

            this.craftingRecipes = new Dictionary<string, UnlockableCraftingRecipe>();
            this.craftingMenuTabs = new Dictionary<string, AnimatedButton>();

            foreach (JsonCraftingMenuTab menuTab in CraftingTabs)
            {
                this.addInCraftingTab(menuTab.craftingTabName, menuTab.getAnimatedButton(), menuTab.isDefaultTab);
            }
            foreach(UnlockableJsonCraftingRecipe craftingRecipe in CraftingRecipes)
            {
                this.addCraftingRecipe(craftingRecipe.recipe.craftingRecipeId, craftingRecipe.toUnlockableCraftingRecipe());
            }
        }

        public CraftingRecipeBook(string CraftingGroup)
        {
            this.craftingRecipeBookId = CraftingGroup;
            this.craftingRecipes = new Dictionary<string, UnlockableCraftingRecipe>();
            this.craftingMenuTabs = new Dictionary<string, AnimatedButton>();
        }

        /// <summary>
        /// Adds in a new crafting recipe.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Recipe"></param>
        public void addCraftingRecipe(string Id, UnlockableCraftingRecipe Recipe)
        {
            if (this.craftingRecipes.ContainsKey(Id) == false)
                this.craftingRecipes.Add(Id, Recipe);
            else
                throw new Exception(string.Format("This crafting book already contains a recipe with the same id! RecipeBookId: {0} ExistingRecipeId {1}",this.craftingRecipeBookId,Id));
        }

        /// <summary>
        /// Adds in a crafting recipe.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Recipe"></param>
        /// <param name="Unlocked">Is this recipe already unlocked?</param>
        public void addCraftingRecipe(string Id, Recipe Recipe, bool Unlocked)
        {
            UnlockableCraftingRecipe recipe = new UnlockableCraftingRecipe(this.craftingRecipeBookId, Recipe, Unlocked);
            this.addCraftingRecipe(Id, recipe);
        }

        /// <summary>
        /// Adds in a crafting tab to the recipe book to be used for the menu generated for crafting recipes.
        /// </summary>
        /// <param name="TabName"></param>
        /// <param name="TabSprite"></param>
        /// <param name="IsDefaultTab"></param>
        public void addInCraftingTab(string TabName, AnimatedButton TabSprite, bool IsDefaultTab)
        {
            if (this.craftingMenuTabs.ContainsKey(TabName))
                throw new Exception("A tab with the same name already exists!");
            else
                this.craftingMenuTabs.Add(TabName, TabSprite);
            if (IsDefaultTab)
                this.defaultTab = TabName;
        }

        /// <summary>
        /// Gets the crafting recipe by it's name.
        /// </summary>
        /// <param name="CraftingRecipeId"></param>
        /// <returns></returns>
        public UnlockableCraftingRecipe getCraftingRecipe(string CraftingRecipeId)
        {
            if (this.containsCraftingRecipe(CraftingRecipeId))
                return this.craftingRecipes[CraftingRecipeId];
            else return null;
        }

        /// <summary>
        /// Checks to see if this crafting recipe book contains a given recipe.
        /// </summary>
        /// <param name="CraftingRecipeId"></param>
        /// <returns></returns>
        public virtual bool containsCraftingRecipe(string CraftingRecipeId)
        {

            return this.craftingRecipes.ContainsKey(CraftingRecipeId);
        }

        /// <summary>
        /// Checks to see if a crafting recipe has been unlocked.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool hasUnlockedCraftingRecipe(string Id)
        {
            UnlockableCraftingRecipe recipe = this.getCraftingRecipe(Id);
            if (recipe == null) return false;
            else return recipe.hasUnlocked;
        }

        /// <summary>
        /// Unlocks the crating recipe so that it can be shown in the menu.
        /// </summary>
        /// <param name="Id"></param>
        public void unlockRecipe(string Id)
        {
            UnlockableCraftingRecipe recipe = this.getCraftingRecipe(Id);
            if (recipe == null) return;
            else recipe.unlock();
        }

        /// <summary>
        /// Opens up a crafting menu from this crafting book.
        /// </summary>
        public void openCraftingMenu()
        {
            CraftingMenuV1 menu = new Framework.Menus.CraftingMenuV1(100, 100, 400, 700, Color.White, Game1.player.Items);
            //menu.addInCraftingPageTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));

            foreach (KeyValuePair<string, AnimatedButton> pair in this.craftingMenuTabs)
                menu.addInCraftingPageTab(pair.Key, pair.Value);

            foreach (KeyValuePair<string, UnlockableCraftingRecipe> pair in this.craftingRecipes)
                if (pair.Value.hasUnlocked)
                    menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(pair.Value.recipe, null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), pair.Value.whichTab);
            menu.currentTab = this.defaultTab;
            menu.sortRecipes();
            if (Game1.activeClickableMenu == null) Game1.activeClickableMenu = menu;
        }

        public CraftingMenuV1 getCraftingMenuForMachine(int x, int y, int width, int height, ref IList<Item> Items, ref IList<Item> Output, Machine Machine)
        {
            CraftingMenuV1 menu = new Framework.Menus.CraftingMenuV1(x, y, width, height, Color.White, ref Items, ref Output, Machine);
            //menu.addInCraftingPageTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));

            foreach (KeyValuePair<string, AnimatedButton> pair in this.craftingMenuTabs)
                menu.addInCraftingPageTab(pair.Key, pair.Value);

            foreach (KeyValuePair<string, UnlockableCraftingRecipe> pair in this.craftingRecipes)
                if (pair.Value.hasUnlocked)
                    menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(pair.Value.recipe, null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), pair.Value.whichTab);
            menu.currentTab = this.defaultTab;
            menu.sortRecipes();
            return menu;
        }

        public void openCraftingMenu(int x, int y, int width, int height, ref IList<Item> items)
        {
            CraftingMenuV1 menu = new Framework.Menus.CraftingMenuV1(x, y, width, height, Color.White, items);
            //menu.addInCraftingPageTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));

            foreach (KeyValuePair<string, AnimatedButton> pair in this.craftingMenuTabs)
                menu.addInCraftingPageTab(pair.Key, pair.Value);

            foreach (KeyValuePair<string, UnlockableCraftingRecipe> pair in this.craftingRecipes)
                if (pair.Value.hasUnlocked)
                    menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(pair.Value.recipe, null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), pair.Value.whichTab);
            menu.currentTab = this.defaultTab;
            menu.sortRecipes();
            if (Game1.activeClickableMenu == null) Game1.activeClickableMenu = menu;
        }
    }
}
