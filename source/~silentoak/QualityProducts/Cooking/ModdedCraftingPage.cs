/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using SilentOak.QualityProducts;
using SilentOak.QualityProducts.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

/* Inspired by spacechase0's Cooking Skill mod (https://github.com/spacechase0/CookingSkill/blob/951625f740d26cebf2c11896fab7d69c3ee75c4f/NewCraftingPage.cs) */

namespace QualityProducts.Cooking
{
    /***
     * Modified from StardewValley.Menus.CraftingPage
     ***/
    /// <summary>
    /// Customized Cooking Menu for quality cooking.
    /// </summary>
    internal class ModdedCraftingPage : IClickableMenu
    {
        /*********
         * Fields 
         *********/

        /// <summary>
        /// Amount of recipes that fit on a page.
        /// </summary>
        public const int howManyRecipesFitOnPage = 40;

        /// <summary>
        /// Amount of recipes that fit in a row.
        /// </summary>
        public const int numInRow = 10;

        /// <summary>
        /// Amount of recipes that fit in a column.
        /// </summary>
        public const int numInCol = 4;

        /// <summary>
        /// The region of crafting selection area.
        /// </summary>
        public const int region_craftingSelectionArea = 8000;

        /// <summary>
        /// The region of crafting modifier.
        /// </summary>
        public const int region_craftingModifier = 200;

        /// <summary>
        /// The region of the up arrow.
        /// </summary>
        public const int region_upArrow = 88;

        /// <summary>
        /// The region of the down arrow.
        /// </summary>
        public const int region_downArrow = 89;

        /// <summary>
        /// The up button.
        /// </summary>
        public ClickableTextureComponent upButton;

        /// <summary>
        /// The down button.
        /// </summary>
        public ClickableTextureComponent downButton;

        /// <summary>
        /// The description text.
        /// </summary>
        private string descriptionText = "";

        /// <summary>
        /// The text of the hovered recipe.
        /// </summary>
        private string hoverText = "";

        /// <summary>
        /// The title of the hovered menu.
        /// </summary>
        private string hoverTitle = "";

        /// <summary>
        /// The hovered item.
        /// </summary>
        private Item hoverItem;

        /// <summary>
        /// The current hovered recipe.
        /// </summary>
        private CraftingRecipe hoverRecipe;

        /// <summary>
        /// The last cooking hover.
        /// </summary>
        private Item lastCookingHover;

        /// <summary>
        /// The item held by the player.
        /// </summary>
        private Item heldItem;

        /// <summary>
        /// The player's inventory
        /// </summary>
        public InventoryMenu inventory;

        /// <summary>
        /// The pages of crafting recipes.
        /// </summary>
        public List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes = new List<Dictionary<ClickableTextureComponent, CraftingRecipe>>();

        /// <summary>
        /// The current crafting page.
        /// </summary>
        private int currentCraftingPage;

        /// <summary>
        /// Whether it is a cooking menu.
        /// </summary>
        private readonly bool cooking;

        /// <summary>
        /// The trash can.
        /// </summary>
        public ClickableTextureComponent trashCan;

        /// <summary>
        /// Amount of rotation of the trash can lid.
        /// </summary>
        public float trashCanLidRotation;

        /***
         * For Lookup Anything compatibility.
         ***/
        private Item HoveredItem;


        public ModdedCraftingPage(int x, int y, int width, int height, bool cooking = false)
            : base(x, y, width, height, false)
        {
            this.cooking = cooking;
            inventory = new InventoryMenu(xPositionOnScreen + spaceToClearSideBorder + borderWidth, yPositionOnScreen + spaceToClearTopBorder + borderWidth + 320 - 16, false, null, null, -1, 3, 0, 0, true)
            {
                showGrayedOutSlots = true
            };
            if (cooking)
            {
                initializeUpperRightCloseButton();
            }
            trashCan = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 4, yPositionOnScreen + height - 192 - 32 - borderWidth - 104, 64, 104), Game1.mouseCursors, new Rectangle(669, 261, 16, 26), 4f, false)
            {
                myID = 106
            };
            List<string> list = new List<string>();
            Dictionary<string, string>.KeyCollection.Enumerator enumerator;
            if (!cooking)
            {
                enumerator = CraftingRecipe.craftingRecipes.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        if (Game1.player.craftingRecipes.ContainsKey(current))
                        {
                            list.Add(current);
                        }
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }
            else
            {
                enumerator = CraftingRecipe.cookingRecipes.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current2 = enumerator.Current;
                        list.Add(current2);
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }
            LayoutRecipes(list);
            if (pagesOfCraftingRecipes.Count > 1)
            {
                upButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 768 + 32, CraftingPageY(), 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12, -1, -1), 0.8f, false)
                {
                    myID = 88,
                    downNeighborID = 89,
                    rightNeighborID = 106
                };
                downButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 768 + 32, CraftingPageY() + 192 + 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11, -1, -1), 0.8f, false)
                {
                    myID = 89,
                    upNeighborID = 88,
                    rightNeighborID = 106
                };
            }
        }

        protected virtual IList<Item> fridge()
        {
            if (Game1.currentLocation is FarmHouse)
            {
                return (Game1.currentLocation as FarmHouse).fridge.Value.items;
            }
            return null;
        }

        private int CraftingPageY()
        {
            return yPositionOnScreen + spaceToClearTopBorder + borderWidth - 16;
        }

        private ClickableTextureComponent[,] CreateNewPageLayout()
        {
            return new ClickableTextureComponent[10, 4];
        }

        private Dictionary<ClickableTextureComponent, CraftingRecipe> CreateNewPage()
        {
            Dictionary<ClickableTextureComponent, CraftingRecipe> dictionary = new Dictionary<ClickableTextureComponent, CraftingRecipe>();
            pagesOfCraftingRecipes.Add(dictionary);
            return dictionary;
        }

        private bool SpaceOccupied(ClickableTextureComponent[,] pageLayout, int x, int y, CraftingRecipe recipe)
        {
            if (pageLayout[x, y] != null)
            {
                return true;
            }
            if (!recipe.bigCraftable)
            {
                return false;
            }
            if (y + 1 < 4)
            {
                return pageLayout[x, y + 1] != null;
            }
            return true;
        }

        private int? GetNeighbor(ClickableTextureComponent[,] pageLayout, int x, int y, int dx, int dy)
        {
            if (x >= 0 && y >= 0 && x < pageLayout.GetLength(0) && y < pageLayout.GetLength(1))
            {
                ClickableTextureComponent clickableTextureComponent = pageLayout[x, y];
                ClickableTextureComponent clickableTextureComponent2;
                for (clickableTextureComponent2 = clickableTextureComponent; clickableTextureComponent2 == clickableTextureComponent; clickableTextureComponent2 = pageLayout[x, y])
                {
                    x += dx;
                    y += dy;
                    if (x < 0 || y < 0 || x >= pageLayout.GetLength(0) || y >= pageLayout.GetLength(1))
                    {
                        return null;
                    }
                }
                return clickableTextureComponent2?.myID;
            }
            return null;
        }

        private void LayoutRecipes(List<string> playerRecipes)
        {
            int num = xPositionOnScreen + spaceToClearSideBorder + borderWidth - 16;
            int num2 = 8;
            Dictionary<ClickableTextureComponent, CraftingRecipe> dictionary = CreateNewPage();
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            ClickableTextureComponent[,] array = CreateNewPageLayout();
            List<ClickableTextureComponent[,]> list = new List<ClickableTextureComponent[,]>
            {
                array
            };
            foreach (string playerRecipe in playerRecipes)
            {
                num5++;
                CraftingRecipe craftingRecipe = new CraftingRecipe(playerRecipe, cooking);
                while (SpaceOccupied(array, num3, num4, craftingRecipe))
                {
                    num3++;
                    if (num3 >= 10)
                    {
                        num3 = 0;
                        num4++;
                        if (num4 >= 4)
                        {
                            dictionary = CreateNewPage();
                            array = CreateNewPageLayout();
                            list.Add(array);
                            num3 = 0;
                            num4 = 0;
                        }
                    }
                }
                int myID = 200 + num5;
                ClickableTextureComponent clickableTextureComponent = new ClickableTextureComponent("", new Rectangle(num + num3 * (64 + num2), CraftingPageY() + num4 * 72, 64, craftingRecipe.bigCraftable ? 128 : 64), null, (cooking && !Game1.player.cookingRecipes.ContainsKey(craftingRecipe.name)) ? "ghosted" : "", craftingRecipe.bigCraftable ? Game1.bigCraftableSpriteSheet : Game1.objectSpriteSheet, craftingRecipe.bigCraftable ? Game1.getArbitrarySourceRect(Game1.bigCraftableSpriteSheet, 16, 32, craftingRecipe.getIndexOfMenuView()) : Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, craftingRecipe.getIndexOfMenuView(), 16, 16), 4f, false)
                {
                    myID = myID,
                    rightNeighborID = ((num4 < 2 && pagesOfCraftingRecipes.Count > 0) ? 88 : 89),
                    leftNeighborID = -1,
                    upNeighborID = 12344,
                    downNeighborID = num3,
                    fullyImmutable = true,
                    region = 8000
                };
                dictionary.Add(clickableTextureComponent, craftingRecipe);
                array[num3, num4] = clickableTextureComponent;
                if (craftingRecipe.bigCraftable)
                {
                    array[num3, num4 + 1] = clickableTextureComponent;
                }
            }
            foreach (ClickableTextureComponent[,] item in list)
            {
                for (num3 = 0; num3 < item.GetLength(0); num3++)
                {
                    for (num4 = 0; num4 < item.GetLength(1); num4++)
                    {
                        ClickableTextureComponent clickableTextureComponent2 = item[num3, num4];
                        if (clickableTextureComponent2 != null)
                        {
                            int rightNeighborID = GetNeighbor(item, num3, num4, 1, 0) ?? clickableTextureComponent2.rightNeighborID;
                            int leftNeighborID = GetNeighbor(item, num3, num4, -1, 0) ?? clickableTextureComponent2.leftNeighborID;
                            int upNeighborID = GetNeighbor(item, num3, num4, 0, -1) ?? clickableTextureComponent2.upNeighborID;
                            int downNeighborID = GetNeighbor(item, num3, num4, 0, 1) ?? clickableTextureComponent2.downNeighborID;
                            clickableTextureComponent2.rightNeighborID = rightNeighborID;
                            clickableTextureComponent2.leftNeighborID = leftNeighborID;
                            clickableTextureComponent2.upNeighborID = upNeighborID;
                            clickableTextureComponent2.downNeighborID = downNeighborID;
                        }
                    }
                }
            }
        }

        protected override void noSnappedComponentFound(int direction, int oldRegion, int oldID)
        {
            base.noSnappedComponentFound(direction, oldRegion, oldID);
            if (oldRegion == 8000 && direction == 2)
            {
                currentlySnappedComponent = getComponentWithID(oldID % 10);
                currentlySnappedComponent.upNeighborID = oldID;
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = (currentCraftingPage < pagesOfCraftingRecipes.Count) ? pagesOfCraftingRecipes[currentCraftingPage].First().Key : null;
            snapCursorToCurrentSnappedComponent();
        }

        protected override void actionOnRegionChange(int oldRegion, int newRegion)
        {
            base.actionOnRegionChange(oldRegion, newRegion);
            if (newRegion == 9000 && oldRegion != 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (inventory.inventory.Count > i)
                    {
                        inventory.inventory[i].upNeighborID = currentlySnappedComponent.upNeighborID;
                    }
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (key.Equals(Keys.Delete) && heldItem != null && heldItem.canBeTrashed())
            {
                if (heldItem is SObject && Game1.player.specialItems.Contains(heldItem.parentSheetIndex))
                {
                    Game1.player.specialItems.Remove(heldItem.parentSheetIndex);
                }
                heldItem = null;
                Game1.playSound("trashcan");
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            KeyValuePair<ClickableTextureComponent, CraftingRecipe> keyValuePair;
            if (direction > 0 && currentCraftingPage > 0)
            {
                currentCraftingPage--;
                Game1.playSound("shwip");
                if (Game1.options.SnappyMenus)
                {
                    ClickableTextureComponent clickableTextureComponent = upButton;
                    keyValuePair = pagesOfCraftingRecipes[currentCraftingPage].Last();
                    clickableTextureComponent.leftNeighborID = keyValuePair.Key.myID;
                    setCurrentlySnappedComponentTo(88);
                    snapCursorToCurrentSnappedComponent();
                    ClickableTextureComponent clickableTextureComponent2 = downButton;
                    keyValuePair = pagesOfCraftingRecipes[currentCraftingPage].Last();
                    clickableTextureComponent2.leftNeighborID = keyValuePair.Key.myID;
                }
            }
            else if (direction < 0 && currentCraftingPage < pagesOfCraftingRecipes.Count - 1)
            {
                currentCraftingPage++;
                Game1.playSound("shwip");
                if (Game1.options.SnappyMenus)
                {
                    ClickableTextureComponent clickableTextureComponent3 = downButton;
                    keyValuePair = pagesOfCraftingRecipes[currentCraftingPage].Last();
                    clickableTextureComponent3.leftNeighborID = keyValuePair.Key.myID;
                    setCurrentlySnappedComponentTo(89);
                    snapCursorToCurrentSnappedComponent();
                    ClickableTextureComponent clickableTextureComponent4 = upButton;
                    keyValuePair = pagesOfCraftingRecipes[currentCraftingPage].Last();
                    clickableTextureComponent4.leftNeighborID = keyValuePair.Key.myID;
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, true);
            heldItem = inventory.leftClick(x, y, heldItem, true);
            KeyValuePair<ClickableTextureComponent, CraftingRecipe> keyValuePair;
            if (upButton != null && upButton.containsPoint(x, y) && currentCraftingPage > 0)
            {
                Game1.playSound("coin");
                currentCraftingPage = Math.Max(0, currentCraftingPage - 1);
                upButton.scale = upButton.baseScale;
                ClickableTextureComponent clickableTextureComponent = upButton;
                keyValuePair = pagesOfCraftingRecipes[currentCraftingPage].Last();
                clickableTextureComponent.leftNeighborID = keyValuePair.Key.myID;
            }
            if (downButton != null && downButton.containsPoint(x, y) && currentCraftingPage < pagesOfCraftingRecipes.Count - 1)
            {
                Game1.playSound("coin");
                currentCraftingPage = Math.Min(pagesOfCraftingRecipes.Count - 1, currentCraftingPage + 1);
                downButton.scale = downButton.baseScale;
                ClickableTextureComponent clickableTextureComponent2 = downButton;
                keyValuePair = pagesOfCraftingRecipes[currentCraftingPage].Last();
                clickableTextureComponent2.leftNeighborID = keyValuePair.Key.myID;
            }
            foreach (ClickableTextureComponent key in pagesOfCraftingRecipes[currentCraftingPage].Keys)
            {
                int num = (!Game1.oldKBState.IsKeyDown(Keys.LeftShift)) ? 1 : 5;
                for (int i = 0; i < num; i++)
                {
                    if (key.containsPoint(x, y) && !key.hoverText.Equals("ghosted") && pagesOfCraftingRecipes[currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(cooking ? fridge() : null))
                    {
                        CraftRecipe(pagesOfCraftingRecipes[currentCraftingPage][key], i == 0);
                    }
                }
            }
            if (trashCan != null && trashCan.containsPoint(x, y) && heldItem != null && heldItem.canBeTrashed())
            {
                if (heldItem is SObject && Game1.player.specialItems.Contains(heldItem.parentSheetIndex))
                {
                    Game1.player.specialItems.Remove(heldItem.parentSheetIndex);
                }
                heldItem = null;
                Game1.playSound("trashcan");
            }
            else if (heldItem != null && !isWithinBounds(x, y) && heldItem.canBeTrashed())
            {
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection, null, -1);
                heldItem = null;
            }
        }

        /***
         * Modified from StardewValley.CraftingPage.clickCraftingRecipe
         ***/
         /// <summary>
         /// Crafts the recipe.
         /// </summary>
         /// <param name="recipe">The recipe to craft.</param>
         /// <param name="playSound">If set to <c>true</c> play sound.</param>
        private void CraftRecipe(CraftingRecipe recipe, bool playSound = true)
        {
            List<Ingredient> ingredients = SelectIngredients(recipe);
            int avgQuality = 0;
            int totalIngredients = 0;

            foreach (Ingredient ingredient in ingredients)
            {
                SObject selectedItem = ingredient.Item as SObject;
                avgQuality += selectedItem.Quality * selectedItem.Stack;
                totalIngredients += selectedItem.Stack;
            }

            avgQuality /= totalIngredients;
            if (avgQuality == 3)
            {
                avgQuality = 4;
            }

            Util.Monitor.VerboseLog($"Crafting {recipe.DisplayName} (quality {avgQuality})");

            Item item = recipe.createItem();
            if (item is SObject obj)
            {
                obj.Quality = avgQuality;
            }

            Game1.player.checkForQuestComplete(null, -1, -1, item, null, 2, -1);
            if (heldItem == null)
            {
                heldItem = item;

                foreach (Ingredient ingredient in ingredients)
                {
                    ingredient.Consume();
                }

                if (playSound)
                {
                    Game1.playSound("coin");
                }
            }
            else if (heldItem.canStackWith(item) && heldItem.Stack + recipe.numberProducedPerCraft - 1 < heldItem.maximumStackSize())
            {
                heldItem.Stack += recipe.numberProducedPerCraft;

                foreach (Ingredient ingredient in ingredients)
                {
                    ingredient.Consume();
                }

                if (playSound)
                {
                    Game1.playSound("coin");
                }
            }
            if (!cooking && Game1.player.craftingRecipes.ContainsKey(recipe.name))
            {
                NetStringDictionary<int, NetInt> craftingRecipes = Game1.player.craftingRecipes;
                string name = recipe.name;
                craftingRecipes[name] += recipe.numberProducedPerCraft;
            }
            if (cooking)
            {
                Game1.player.cookedRecipe(heldItem.parentSheetIndex);
            }
            if (!cooking)
            {
                Game1.stats.checkForCraftingAchievements();
            }
            else
            {
                Game1.stats.checkForCookingAchievements();
            }
            if (Game1.options.gamepadControls && heldItem != null && Game1.player.couldInventoryAcceptThisItem(heldItem))
            {
                Game1.player.addItemToInventoryBool(heldItem, false);
                heldItem = null;
            }
        }

        /***
         * Modified from CraftingRecipes.consumeIngredients
         ***/
         /// <summary>
         /// Selects the ingredients for the recipe from player inventory and/or fridge.
         /// </summary>
         /// <returns>The ingredients.</returns>
         /// <param name="recipe">Recipe.</param>
        private List<Ingredient> SelectIngredients(CraftingRecipe recipe)
        {
            Util.Monitor.VerboseLog($"Selecting items: ");
            List<Ingredient> selected = new List<Ingredient>();
            Dictionary<int, int> recipeList = Util.Helper.Reflection.GetField<Dictionary<int, int>>(recipe, "recipeList").GetValue();
            NetObjectList<Item> playerItemList = Game1.player.items;

            Util.Monitor.VerboseLog($"Looking in inventory");
            for (int ingredientIdx = recipeList.Count - 1; ingredientIdx >= 0; ingredientIdx--)
            {
                int ingredientID = recipeList.Keys.ElementAt(ingredientIdx);
                int ingredientAmt = recipeList[ingredientID];
                bool ingredientDone = false;
                for (int itemIdx = playerItemList.Count - 1; itemIdx >= 0; itemIdx--)
                {
                    Item playerItem = playerItemList[itemIdx];
                    if (playerItem != null && playerItem is SObject playerObject && !(playerItem as SObject).bigCraftable && (playerItem.parentSheetIndex == ingredientID || playerItem.Category == ingredientID))
                    {
                        Util.Monitor.VerboseLog($"Selected {playerObject.Stack} {playerObject.DisplayName} (quality {playerObject.Quality})");
                        recipeList[ingredientID] -= playerItem.Stack;

                        selected.Add(new Ingredient(playerItemList, itemIdx, ingredientAmt));

                        if (recipeList[ingredientID] <= 0)
                        {
                            recipeList[ingredientID] = ingredientAmt;
                            ingredientDone = true;
                            break;
                        }
                    }
                }

                if (recipe.isCookingRecipe && !ingredientDone && Game1.currentLocation is FarmHouse farmHouse)
                {
                    Util.Monitor.VerboseLog($"Looking in fridge");
                    NetObjectList<Item> fridgeItemList = farmHouse.fridge.Value.items;
                    for (int itemIdx = fridgeItemList.Count - 1; itemIdx >= 0; itemIdx--)
                    {
                        Item fridgeItem = fridgeItemList[itemIdx];
                        if (fridgeItem != null && fridgeItem is SObject fridgeObject && (fridgeItem.parentSheetIndex == ingredientID || fridgeItem.Category == ingredientID))
                        {
                            Util.Monitor.VerboseLog($"Selected {fridgeObject.Stack} {fridgeObject.DisplayName} with quality {fridgeObject.Quality}");
                            recipeList[ingredientID] -= fridgeItem.Stack;

                            selected.Add(new Ingredient(fridgeItemList, itemIdx, ingredientAmt));

                            if (recipeList[ingredientID] <= 0)
                            {
                                recipeList[ingredientID] = ingredientAmt;
                                ingredientDone = true;
                                break;
                            }
                        }
                    }
                }
            }

            return selected;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            heldItem = inventory.rightClick(x, y, heldItem, true);
            foreach (ClickableTextureComponent key in pagesOfCraftingRecipes[currentCraftingPage].Keys)
            {
                if (key.containsPoint(x, y) && !key.hoverText.Equals("ghosted") && pagesOfCraftingRecipes[currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(cooking ? fridge() : null))
                {
                    CraftRecipe(pagesOfCraftingRecipes[currentCraftingPage][key], true);
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverTitle = "";
            descriptionText = "";
            hoverText = "";
            hoverRecipe = null;
            hoverItem = inventory.hover(x, y, hoverItem);
            if (hoverItem != null)
            {
                hoverTitle = inventory.hoverTitle;
                hoverText = inventory.hoverText;
            }
            foreach (ClickableTextureComponent key in pagesOfCraftingRecipes[currentCraftingPage].Keys)
            {
                if (key.containsPoint(x, y))
                {
                    if (key.hoverText.Equals("ghosted"))
                    {
                        hoverText = "???";
                    }
                    else
                    {
                        hoverRecipe = pagesOfCraftingRecipes[currentCraftingPage][key];
                        if (lastCookingHover == null || !lastCookingHover.Name.Equals(hoverRecipe.name))
                        {
                            lastCookingHover = hoverRecipe.createItem();
                        }
                        key.scale = Math.Min(key.scale + 0.02f, key.baseScale + 0.1f);
                    }
                }
                else
                {
                    key.scale = Math.Max(key.scale - 0.02f, key.baseScale);
                }
            }
            if (upButton != null)
            {
                if (upButton.containsPoint(x, y))
                {
                    upButton.scale = Math.Min(upButton.scale + 0.02f, upButton.baseScale + 0.1f);
                }
                else
                {
                    upButton.scale = Math.Max(upButton.scale - 0.02f, upButton.baseScale);
                }
            }
            if (downButton != null)
            {
                if (downButton.containsPoint(x, y))
                {
                    downButton.scale = Math.Min(downButton.scale + 0.02f, downButton.baseScale + 0.1f);
                }
                else
                {
                    downButton.scale = Math.Max(downButton.scale - 0.02f, downButton.baseScale);
                }
            }
            if (trashCan != null)
            {
                if (trashCan.containsPoint(x, y))
                {
                    if (trashCanLidRotation <= 0f)
                    {
                        Game1.playSound("trashcanlid");
                    }
                    trashCanLidRotation = Math.Min(trashCanLidRotation + 0.06544985f, 1.57079637f);
                }
                else
                {
                    trashCanLidRotation = Math.Max(trashCanLidRotation - 0.06544985f, 0f);
                }
            }

            HoveredItem = hoverItem ?? hoverRecipe?.createItem();
        }

        public override bool readyToClose()
        {
            return heldItem == null;
        }

        public override void draw(SpriteBatch b)
        {
            if (cooking)
            {
                Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true, null, false, false);
            }
            drawHorizontalPartition(b, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 256, false);
            inventory.draw(b);
            if (trashCan != null)
            {
                trashCan.draw(b);
                b.Draw(Game1.mouseCursors, new Vector2(trashCan.bounds.X + 60, trashCan.bounds.Y + 40), new Rectangle(686, 256, 18, 10), Color.White, trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
            }
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);
            foreach (ClickableTextureComponent key in pagesOfCraftingRecipes[currentCraftingPage].Keys)
            {
                if (key.hoverText.Equals("ghosted"))
                {
                    key.draw(b, Color.Black * 0.35f, 0.89f);
                }
                else if (!pagesOfCraftingRecipes[currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(cooking ? fridge() : null))
                {
                    key.draw(b, Color.LightGray * 0.4f, 0.89f);
                }
                else
                {
                    key.draw(b);
                    if (pagesOfCraftingRecipes[currentCraftingPage][key].numberProducedPerCraft > 1)
                    {
                        NumberSprite.draw(pagesOfCraftingRecipes[currentCraftingPage][key].numberProducedPerCraft, b, new Vector2(key.bounds.X + 64 - 2, key.bounds.Y + 64 - 2), Color.Red, 0.5f * (key.scale / 4f), 0.97f, 1f, 0, 0);
                    }
                }
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            if (hoverItem != null)
            {
                drawToolTip(b, hoverText, hoverTitle, hoverItem, heldItem != null, -1, 0, -1, -1, null, -1);
            }
            else if (!string.IsNullOrEmpty(hoverText))
            {
                drawHoverText(b, hoverText, Game1.smallFont, (heldItem != null) ? 64 : 0, (heldItem != null) ? 64 : 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
            if (heldItem != null)
            {
                heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);
            }
            base.draw(b);
            if (downButton != null && currentCraftingPage < pagesOfCraftingRecipes.Count - 1)
            {
                downButton.draw(b);
            }
            if (upButton != null && currentCraftingPage > 0)
            {
                upButton.draw(b);
            }
            if (cooking)
            {
                drawMouse(b);
            }
            if (hoverRecipe != null)
            {
                drawHoverText(b, " ", Game1.smallFont, (heldItem != null) ? 48 : 0, (heldItem != null) ? 48 : 0, -1, hoverRecipe.DisplayName, -1, (cooking && lastCookingHover != null && Game1.objectInformation[lastCookingHover.parentSheetIndex].Split('/').Length > 7) ? Game1.objectInformation[lastCookingHover.parentSheetIndex].Split('/')[7].Split(' ') : null, lastCookingHover, 0, -1, -1, -1, -1, 1f, hoverRecipe);
            }
        }
    }
}
