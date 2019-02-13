using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Inventory;
using TehPers.CoreMod.Api.Items.Recipes;
using TehPers.CoreMod.Items.Inventory;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Items.Crafting {
    internal class CraftingManager {
        private static readonly ConditionalWeakTable<CraftingPage, CraftingPageData> _extraCraftingPageData = new ConditionalWeakTable<CraftingPage, CraftingPageData>();
        private static CraftingManager _instance;

        private readonly IMod _coreMod;
        private readonly ItemDelegator _itemDelegator;
        private readonly Dictionary<string, IRecipe> _addedRecipes = new Dictionary<string, IRecipe>();

        private CraftingManager(IMod coreMod, ItemDelegator itemDelegator) {
            this._coreMod = coreMod;
            this._itemDelegator = itemDelegator;

            // Patches
            this.Patch();
        }

        public void Initialize() {
            this._coreMod.Helper.Content.AssetEditors.Add(new CraftingRecipeAssetEditor(this));
        }

        public IEnumerable<string> GetAddedRecipes() {
            return this._addedRecipes.Keys;
        }

        private void Patch() {
            HarmonyInstance harmony = HarmonyInstance.Create("TehPers.CoreMod.CraftingManager");

            // CraftingPage.layoutRecipes(List<string> playerRecipes)
            MethodBase target = typeof(CraftingPage).GetMethod("layoutRecipes", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_layoutRecipes_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.clickCraftingRecipe(ClickableTextureComponent c, bool playSound)
            target = typeof(CraftingPage).GetMethod("clickCraftingRecipe", BindingFlags.NonPublic | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_clickCraftingRecipe_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.readyToClose()
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.readyToClose), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_readyToClose_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.receiveKeyPress(Keys key)
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.receiveKeyPress), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_receiveKeyPress_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingPage.draw(SpriteBatch b)
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.draw), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_draw_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.createItem()
            target = typeof(CraftingRecipe).GetMethod(nameof(CraftingRecipe.createItem), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingRecipe_createItem), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.getNumberOfIngredients()
            target = typeof(CraftingRecipe).GetMethod(nameof(CraftingRecipe.getNumberOfIngredients), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingRecipe_getNumberOfIngredients_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.drawRecipeDescription(SpriteBatch b, Vector2 position, int width)
            target = typeof(CraftingRecipe).GetMethod(nameof(CraftingRecipe.drawRecipeDescription), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingRecipe_drawRecipeDescription_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.getDescriptionHeight(int width)
            target = typeof(CraftingRecipe).GetMethod(nameof(CraftingRecipe.getDescriptionHeight), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingRecipe_getDescriptionHeight), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.receiveLeftClick(int x, int y, bool playSound)
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.receiveLeftClick), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_receiveLeftClick_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));

            // CraftingRecipe.receiveRightClick(int x, int y, bool playSound)
            target = typeof(CraftingPage).GetMethod(nameof(CraftingPage.receiveRightClick), BindingFlags.Public | BindingFlags.Instance);
            prefix = typeof(CraftingManager).GetMethod(nameof(CraftingManager.CraftingPage_receiveRightClick_Prefix), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(target, prefix: new HarmonyMethod(prefix));
        }

        public string AddRecipe(IRecipe recipe) {
            string key = $"tcm_recipe{this._addedRecipes.Count}";
            this._addedRecipes.Add(key, recipe);
            this._coreMod.Helper.Content.InvalidateCache("Data/CraftingRecipes");
            CraftingRecipe.InitShared();
            return key;
        }

        public static CraftingManager GetCraftingManager(IMod mod, ItemDelegator itemDelegator) {
            if (CraftingManager._instance == null) {
                CraftingManager._instance = new CraftingManager(mod, itemDelegator);
            }

            return CraftingManager._instance;
        }

        private static CustomCraftingRecipe CreateRecipe(string name, bool cooking) {
            CraftingManager._instance._coreMod.Monitor.Log($"Creating {nameof(CustomCraftingRecipe)} for {name}", LogLevel.Trace);

            if (CraftingManager._instance._addedRecipes.TryGetValue(name, out IRecipe modRecipe)) {
                return new ModCraftingRecipe(name, modRecipe, cooking);
            }

            ICoreApi coreApi = (CraftingManager._instance._coreMod.GetApi() as ICoreApiFactory)?.GetApi(CraftingManager._instance._coreMod);
            return new GameCraftingRecipe(coreApi, name, cooking);
        }

        private static CraftingPageData GetExtraData(CraftingPage page) {
            if (!CraftingManager._extraCraftingPageData.TryGetValue(page, out CraftingPageData extraData)) {
                extraData = new CraftingPageData(page);
                CraftingManager._extraCraftingPageData.Add(page, extraData);
            }

            return extraData;
        }

        private static void TrashHeldItems(CraftingPage craftingPage, ref Item heldItem) {
            // Get extra data about this crafting page
            CraftingPageData extraData = CraftingManager.GetExtraData(craftingPage);

            // Make sure the original method doesn't remove any items as well
            heldItem = null;

            int removed = extraData.HeldItems.RemoveAll(item => {
                // Check if the item can be trashed
                if (!item.canBeTrashed()) {
                    // Don't remove the item
                    return false;
                }

                // Remove it from the special items list if it's a special item
                if (item is SObject) {
                    Game1.player.specialItems.Remove(item.ParentSheetIndex);
                }

                // Remove the item
                return true;
            });

            // Play the trashcan sound if any items were removed
            if (removed > 0) {
                Game1.playSound("trashcan");
            }
        }

        private static void RightClickInventory(CraftingPage craftingPage, int x, int y, bool playSound) {
            CraftingPageData extraData = CraftingManager.GetExtraData(craftingPage);

            // Check if a component was clicked
            if (!(craftingPage.inventory.inventory.FirstOrDefault(c => c.containsPoint(x, y)) is ClickableComponent slot)) {
                return;
            }

            // Check if the component was a valid slot
            if (!int.TryParse(slot.name, out int slotId) || slotId >= craftingPage.inventory.actualInventory.Count) {
                return;
            }

            // Check if there's an item in the slot
            if (!(craftingPage.inventory.actualInventory[slotId] is Item slotItem)) {
                return;
            }

            // Not sure what this does
            if (!craftingPage.inventory.highlightMethod(slotItem)) {
                return;
            }

            // When right clicking a tool, either attach the held item to the tool, or take the attachment off the tool if possible
            if (slotItem is Tool tool) {
                // Try to get the first held item
                if (!(extraData.HeldItems.FirstOrDefault() is Item heldItem)) {
                    return;
                }

                // Check if the first item can be attached to it
                if (!(heldItem is SObject heldObj && tool.canThisBeAttached(heldObj))) {
                    return;
                }

                // Attach the item
                SObject replacement = tool.attach(heldObj);
                if (heldObj.Stack == 0) {
                    extraData.HeldItems.RemoveAt(0);
                } else {
                    extraData.HeldItems[0] = replacement;
                }
            } else if (!extraData.HeldItems.Any()) {
                // Check if the item doesn't have a max stack size?
                if (slotItem.maximumStackSize() == -1) {
                    return;
                }

                // Notify the item that it has stopped being held if needed
                if (slotId == Game1.player.CurrentToolIndex && slotItem.Stack == 1) {
                    slotItem.actionWhenStopBeingHeld(Game1.player);
                }

                // Grab one of the item
                Item newItem = slotItem.getOne();

                // Try to split the item if needed
                if (slotItem.Stack > 1 && Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] { new InputButton(Keys.LeftShift) })) {
                    newItem.Stack = (slotItem.Stack + 1) / 2;
                    slotItem.Stack -= newItem.Stack;
                } else {
                    slotItem.Stack--;
                }

                // Check the slot's stack size
                if (slotItem.Stack <= 0) {
                    craftingPage.inventory.actualInventory[slotId] = null;
                }

                // Play sound if needed
                if (playSound) {
                    Game1.playSound("dwop");
                }

                extraData.AddHeldItem(newItem);
            } else {
                // Max number of items that should be grabbed
                int maxToGrab = Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] { new InputButton(Keys.LeftShift) }) ? (slotItem.Stack + 1) / 2 : 1;

                // Max number of items that can be held in addition to what is already held
                int maxCanHold = extraData.HeldItems.Where(heldItem => slotItem.canStackWith(heldItem)).Sum(heldItem => heldItem.maximumStackSize() - heldItem.Stack);

                // If none can be picked up, then don't continue
                if (maxCanHold == 0) {
                    return;
                }

                // Number of items that should be held
                int amountToGrab = new[] { slotItem.Stack, maxToGrab, maxCanHold }.Min();

                // Grab that many items
                Item addedItem = slotItem.getOne();
                slotItem.Stack -= amountToGrab;
                addedItem.Stack = amountToGrab;
                extraData.AddHeldItem(addedItem);

                // Play sound if needed
                if (playSound) {
                    Game1.playSound("dwop");
                }

                // Clean up slot
                if (slotItem.Stack <= 0) {
                    // Notify item if it stopped being held
                    if (slotId == Game1.player.CurrentToolIndex) {
                        slotItem.actionWhenStopBeingHeld(Game1.player);
                    }

                    craftingPage.inventory.actualInventory[slotId] = null;
                }
            }
        }

        private static void LeftClickInventory(CraftingPage craftingPage, int x, int y, bool playSound) {
            CraftingPageData extraData = CraftingManager.GetExtraData(craftingPage);

            // Check if a component was clicked
            if (!(craftingPage.inventory.inventory.FirstOrDefault(c => c.containsPoint(x, y)) is ClickableComponent slot)) {
                return;
            }

            // Check if the component was a valid slot
            if (!int.TryParse(slot.name, out int slotId) || slotId >= craftingPage.inventory.actualInventory.Count) {
                return;
            }

            // Get the item in the slot
            Item slotItem = craftingPage.inventory.actualInventory[slotId];
            Item heldItem = extraData.HeldItems.FirstOrDefault();

            if (slotItem != null && !craftingPage.inventory.highlightMethod(slotItem) && !slotItem.canStackWith(heldItem)) {
                return;
            }

            // Check if an item is in the slot
            if (slotItem != null) {
                if (heldItem == null) {
                    // Play the sound
                    if (playSound) {
                        Game1.playSound("dwop");
                    }

                    // Move the item from the slot to the cursor
                    extraData.AddHeldItem(Utility.removeItemFromInventory(slotId, craftingPage.inventory.actualInventory));
                } else {
                    // Make sure the held item should be placed in the slot
                    if (!(craftingPage.inventory.highlightMethod(slotItem) || slotItem.canStackWith(heldItem))) {
                        return;
                    }

                    // Play the sound
                    if (playSound) {
                        Game1.playSound("stoneStep");
                    }

                    // Add the item to the inventory
                    Item remaining = Utility.addItemToInventory(heldItem, slotId, craftingPage.inventory.actualInventory, craftingPage.inventory.onAddItem);

                    // Update held item
                    extraData.HeldItems.RemoveAt(0);
                    if (remaining != null && remaining.Stack > 0) {
                        extraData.AddHeldItem(remaining);
                    }
                }
            } else {
                // Make sure an item is being held
                if (heldItem == null) {
                    return;
                }

                // Play the sound
                if (playSound) {
                    Game1.playSound("stoneStep");
                }

                // Add the held item to the inventory
                Item remaining = Utility.addItemToInventory(heldItem, slotId, craftingPage.inventory.actualInventory, craftingPage.inventory.onAddItem);

                // Update held item
                extraData.HeldItems.RemoveAt(0);
                if (remaining != null && remaining.Stack > 0) {
                    extraData.AddHeldItem(remaining);
                }
            }
        }

        private static void DrawRecipeDescription(CustomCraftingRecipe recipe, SpriteBatch b, Vector2 position, int width, string description, out int height) {
            // Get all ingredients required for this recipe
            IRecipePart[] ingredients = recipe.Recipe.Ingredients.ToArray();

            // Create the inventories
            IInventory inventory = new FarmerInventory(Game1.player);
            ChestInventory fridge = new ChestInventory(Utility.getHomeOfFarmer(Game1.player).fridge.Value);

            // Draw results if there are several
            float yOffset = 28f;
            IRecipePart[] results = recipe.Recipe.Results.ToArray();
            if (results.Length > 1) {
                // Draw "Results:"
                string resultsStr = CraftingManager._instance._coreMod.Helper.Translation.Get("crafting.results").ToString();
                if (b != null) Utility.drawTextWithShadow(b, resultsStr, Game1.smallFont, position + new Vector2(8f, yOffset), Game1.textColor * 0.75f);
                yOffset += 4 + Game1.smallFont.MeasureString(resultsStr).Y;

                // Draw horizontal separator
                b?.Draw(Game1.staminaRect, new Rectangle((int) (position.X + 8.0), (int) (position.Y + yOffset) - 4 - 2, width - 32, 2), Game1.textColor * 0.35f);
                yOffset += 2;

                foreach (IRecipePart result in results) {
                    if (b != null) {
                        // Draw the result
                        result.Sprite.Draw(b, new Vector2(position.X, position.Y + yOffset), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);

                        // Draw quantity
                        Utility.drawTinyDigits(result.Quantity, b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(result.Quantity.ToString()).X, position.Y + yOffset + 21f), 2f, 0.87f, Color.AntiqueWhite);

                        // Draw result name
                        Utility.drawTextWithShadow(b, $"{(result.TryCreateOne(out Item item) ? item.DisplayName : "Invalid item")}", Game1.smallFont, new Vector2(position.X + 32f + 8f, position.Y + yOffset + 4f), Game1.textColor);
                    }

                    // Update y offset
                    yOffset += 36f;
                }

                // Draw horizontal separator
                b?.Draw(Game1.staminaRect, new Rectangle((int) position.X + 8, (int) (position.Y + yOffset) + 4, width - 32, 2), Game1.textColor * 0.35f);
                yOffset += 12;
            } else if (results.Length == 0) {
                // Draw "No results."
                string resultsStr = CraftingManager._instance._coreMod.Helper.Translation.Get("crafting.no-results").ToString();
                if (b != null) Utility.drawTextWithShadow(b, resultsStr, Game1.smallFont, position + new Vector2(8f, yOffset), Game1.textColor * 0.75f);
                yOffset += 8 + Game1.smallFont.MeasureString(resultsStr).Y;
            }

            // Draw "Ingredients:"
            string ingredientsStr = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567");
            if (b != null) Utility.drawTextWithShadow(b, ingredientsStr, Game1.smallFont, position + new Vector2(8f, yOffset), Game1.textColor * 0.75f);
            yOffset += 4 + Game1.smallFont.MeasureString(ingredientsStr).Y;

            // Draw horizontal separator
            b?.Draw(Game1.staminaRect, new Rectangle((int) (position.X + 8.0), (int) (position.Y + yOffset) - 4 - 2, width - 32, 2), Game1.textColor * 0.35f);
            yOffset += 2;

            foreach (IRecipePart ingredient in ingredients) {
                if (b != null) {
                    // Get ingredient color (red or normal depending on if it's available)
                    Color ingredientColor = inventory.Contains(ingredient) ? Game1.textColor : Color.Red;
                    if (recipe.isCookingRecipe && fridge.Contains(recipe.Recipe.Ingredients)) {
                        ingredientColor = Game1.textColor;
                    }

                    // Draw the ingredient
                    ingredient.Sprite.Draw(b, new Vector2(position.X, position.Y + yOffset), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);

                    // Draw quantity
                    Utility.drawTinyDigits(ingredient.Quantity, b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(ingredient.Quantity.ToString()).X, position.Y + yOffset + 21f), 2f, 0.87f, Color.AntiqueWhite);

                    // Draw ingredient name
                    Utility.drawTextWithShadow(b, ingredient.GetDisplayName(), Game1.smallFont, new Vector2(position.X + 32f + 8f, position.Y + yOffset + 4f), ingredientColor);
                }

                // Update y offset
                yOffset += 36f;
            }

            // Draw horizontal separator
            b?.Draw(Game1.staminaRect, new Rectangle((int) position.X + 8, (int) (position.Y + yOffset) + 4, width - 32, 2), Game1.textColor * 0.35f);
            yOffset += 12;

            // Draw description
            string wrappedDescription = Game1.parseText(description, Game1.smallFont, width - 8);
            if (b != null) Utility.drawTextWithShadow(b, description, Game1.smallFont, position + new Vector2(0f, yOffset), Game1.textColor * 0.75f);
            yOffset += Game1.smallFont.MeasureString(wrappedDescription).Y;

            // Set height
            height = (int) yOffset;
        }

        #region CraftingPage Patches
        private static bool CraftingPage_layoutRecipes_Prefix(CraftingPage __instance, List<string> playerRecipes, bool ___cooking) {
            // Just run the original method if no recipes were added
            // if (!CraftingManager._instance._addedRecipes.Any()) {
            //     return true;
            // }

            // Layout the crafting pages
            CraftingManager._instance._coreMod.Monitor.Log($"Laying out {playerRecipes.Count} recipes", LogLevel.Trace);

            int startX = __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - 16;
            int startY = __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 16;
            const int pageWidth = 10;
            const int pageHeight = 4;
            const int perPage = pageWidth * pageHeight;
            const int recipePadding = 8;

            // Create a queue of all the recipe objects to add
            Queue<CustomCraftingRecipe> recipesRemaining = new Queue<CustomCraftingRecipe>(playerRecipes.Select(name => CraftingManager.CreateRecipe(name, ___cooking)).OrderByDescending(r => Math.Max(r.ComponentWidth, r.ComponentHeight)));

            // For each recipe, create a clickable texture component and assign it a page and location within the page
            Dictionary<CraftingPageLocation, (CustomCraftingRecipe recipe, ClickableTextureComponent component)> components = new Dictionary<CraftingPageLocation, (CustomCraftingRecipe recipe, ClickableTextureComponent component)>();
            while (recipesRemaining.Any()) {
                CustomCraftingRecipe recipe = recipesRemaining.Dequeue();
                CraftingManager._instance._coreMod.Monitor.Log($"Placing recipe {recipe.name}", LogLevel.Trace);

                // Find the first available spot
                CraftingPageLocation curLocation = new CraftingPageLocation(0, 0, 0);
                while (!IsValidSpaceFor(recipe, curLocation)) {
                    curLocation = curLocation.NextLocation(pageWidth, pageHeight);
                }

                (int page, int y, int x) = curLocation;

                Rectangle bounds = new Rectangle(startX + x * (64 + recipePadding), startY + y * (64 + recipePadding), 64, recipe.bigCraftable ? 128 : 64);
                string hoverText = !___cooking || Game1.player.cookingRecipes.ContainsKey(recipe.name) ? "" : "ghosted";

                // Create the component
                ModRecipeComponent component = new ModRecipeComponent("", bounds, null, hoverText, recipe.Recipe.Sprite, 4f) {
                    myID = 200 + page * perPage + y * pageWidth + x,
                    fullyImmutable = true,
                    region = 8000
                };

                // Add it to the dictionary in each slot it takes up
                CraftingManager._instance._coreMod.Monitor.Log($"Adding {recipe.name} to the components dictionary", LogLevel.Trace);
                for (int slotX = x; slotX < x + recipe.ComponentWidth; slotX++) {
                    for (int slotY = y; slotY < y + recipe.ComponentHeight; slotY++) {
                        components.Add(new CraftingPageLocation(page, slotY, slotX), (recipe, component));
                    }
                }
            }

            // Assign each component's neighbors and add them to the crafting recipe pages
            Dictionary<ClickableTextureComponent, CraftingRecipe>[] pages = Enumerable.Range(0, components.Keys.Max(location => location.Page) + 1).Select(_ => new Dictionary<ClickableTextureComponent, CraftingRecipe>()).ToArray();
            foreach (((int page, int y, int x), (CustomCraftingRecipe recipe, ClickableTextureComponent component)) in components) {
                CraftingManager._instance._coreMod.Monitor.Log($"Assigning neighbors for {recipe.name}", LogLevel.Trace);

                // Add the current component and recipe to the appropriate page
                // This doesn't use .Add because big craftables will be iterated over multiple times during this process
                pages[page][component] = recipe;

                // Left neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y, x - 1), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) leftNeighbor)) {
                    if (leftNeighbor.component != component) {
                        component.leftNeighborID = leftNeighbor.component.myID;
                    }
                } else {
                    component.leftNeighborID = -1;
                }

                // Right neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y, x + 1), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) rightNeighbor)) {
                    if (rightNeighbor.component != component) {
                        component.rightNeighborID = rightNeighbor.component.myID;
                    }
                } else {
                    component.rightNeighborID = y >= 2 || page == 0 ? 89 : 88;
                }

                // Up neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y - 1, x), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) upNeighbor)) {
                    if (upNeighbor.component != component) {
                        component.upNeighborID = upNeighbor.component.myID;
                    }
                } else {
                    component.upNeighborID = 12344;
                }

                // Down neighbor
                if (components.TryGetValue(new CraftingPageLocation(page, y + 1, x), out (CustomCraftingRecipe recipe, ClickableTextureComponent component) downNeighbor)) {
                    if (downNeighbor.component != component) {
                        component.downNeighborID = downNeighbor.component.myID;
                    }
                } else {
                    component.downNeighborID = x;
                }
            }

            // Update pages on the instance
            CraftingManager._instance._coreMod.Monitor.Log("Converting recipe pages to a list", LogLevel.Trace);
            __instance.pagesOfCraftingRecipes = pages.ToList();

            // Prevent the original method from executing
            CraftingManager._instance._coreMod.Monitor.Log("Done laying out recipes", LogLevel.Trace);
            return false;

            bool IsValidSpaceFor(CustomCraftingRecipe recipe, CraftingPageLocation location) {
                // For each row this recipe will take up
                for (int y = location.Y; y < location.Y + recipe.ComponentHeight; y++) {
                    if (y >= pageHeight) {
                        return false;
                    }

                    // For each space on the row this recipe will take up
                    for (int x = location.X; x < location.X + recipe.ComponentWidth; x++) {
                        if (x >= pageWidth) {
                            return false;
                        }

                        // Check if the space is available
                        if (components.ContainsKey(new CraftingPageLocation(location.Page, y, x))) {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        private static bool CraftingPage_clickCraftingRecipe_Prefix(CraftingPage __instance, ClickableTextureComponent c, bool playSound, int ___currentCraftingPage, bool ___cooking) {
            if (!__instance.pagesOfCraftingRecipes[___currentCraftingPage].TryGetValue(c, out CraftingRecipe baseRecipe)) {
                return true;
            }

            if (!(baseRecipe is CustomCraftingRecipe customRecipe)) {
                return true;
            }

            // Try to get all the extra data for the crafting page
            CraftingPageData extraData = CraftingManager.GetExtraData(__instance);

            // TODO: Config option to allow for stacking items under the cursor - default is disabled (similar to vanilla)
            const bool canStack = false;

            // Check if the result can be held
            if (!canStack && extraData.HeldItems.Any()) {
                // Try to create each result
                IEnumerable<(bool success, Item item)> craftResults = customRecipe.Recipe.Results.Select(r => r.TryCreateOne(out Item item) ? (true, item) : (false, null));

                // Make sure all results can be successfully created and held
                if (craftResults.Any(r => !r.success || !extraData.HeldItems.Any(heldItem => heldItem.canStackWith(r.item) && heldItem.Stack + r.item.Stack < heldItem.maximumStackSize()))) {
                    return false;
                }
            }

            // Try to craft the recipe
            if (!customRecipe.Recipe.TryCraft(new FarmerInventory(Game1.player), out IEnumerable<Item> results)) {
                // If not cooking, the recipe can't be crafted
                if (!___cooking) {
                    return false;
                }

                // Check if the recipe can be crafted with ingredients from the fridge
                if (!(Game1.currentLocation is FarmHouse farmHouse) || !customRecipe.Recipe.TryCraft(new ChestInventory(farmHouse.fridge.Value), out results)) {
                    return false;
                }
            }

            // Play the crafting sound
            if (playSound) {
                Game1.playSound("coin");
            }

            // Get all results
            foreach (Item result in results) {
                // Update stats and achievements
                if (___cooking) {
                    // Notify the player that the item was cooked
                    Game1.player.cookedRecipe(result.ParentSheetIndex);

                    // Check for achievements
                    Game1.stats.checkForCookingAchievements();
                } else {
                    // Update times crafted
                    if (Game1.player.craftingRecipes.TryGetValue(customRecipe.name, out int timesCrafted)) {
                        Game1.player.craftingRecipes[customRecipe.name] = timesCrafted + customRecipe.numberProducedPerCraft;
                    }

                    // Check for achievements
                    Game1.stats.checkForCraftingAchievements();
                }

                if (Game1.options.gamepadControls && Game1.player.couldInventoryAcceptThisItem(result)) {
                    // Add it to the player's inventory if they're using a gamepad
                    Game1.player.addItemToInventoryBool(result);
                } else {
                    // Grab the item
                    extraData.AddHeldItem(result);
                }
            }

            return false;
        }

        private static bool CraftingPage_readyToClose_Prefix(CraftingPage __instance, ref bool __result) {
            __result = !CraftingManager.GetExtraData(__instance).HeldItems.Any();
            return false;
        }

        private static void CraftingPage_receiveKeyPress_Prefix(CraftingPage __instance, Keys key, ref Item ___heldItem) {
            // Check if this should remove the held item
            if (key == Keys.Delete) {
                // Trash held items
                CraftingManager.TrashHeldItems(__instance, ref ___heldItem);
            }
        }

        private static bool CraftingPage_draw_Prefix(CraftingPage __instance, SpriteBatch b, bool ___cooking, int ___currentCraftingPage, Item ___hoverItem, string ___hoverText, string ___hoverTitle, CraftingRecipe ___hoverRecipe, Item ___lastCookingHover) {
            // Get extra data about this crafting page
            CraftingPageData extraData = CraftingManager.GetExtraData(__instance);
            bool isHoldingItem = extraData.HeldItems.Any();

            // Draw the background for cooking
            if (___cooking) {
                Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen, __instance.width, __instance.height, false, true);
            }

            // Draw the separator
            DrawHorizontalPartition(__instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256);

            // Draw the inventory
            __instance.inventory.draw(b);

            // Draw the trashcan
            if (__instance.trashCan != null) {
                __instance.trashCan.draw(b);
                b.Draw(Game1.mouseCursors, new Vector2(__instance.trashCan.bounds.X + 60, __instance.trashCan.bounds.Y + 40), new Rectangle(686, 256, 18, 10), Color.White, __instance.trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
            }

            // Draw each recipe
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            foreach ((ClickableTextureComponent component, CraftingRecipe recipe) in __instance.pagesOfCraftingRecipes[___currentCraftingPage]) {
                // Draw the component
                if (component.hoverText.Equals("ghosted")) {
                    // Unknown recipe
                    DrawRecipeComponent2(component, Color.Black * 0.35f, 0.89f);
                } else {
                    bool canCraft;
                    if (recipe is CustomCraftingRecipe customRecipe) {
                        // Check if the player has the required items to craft this recipe
                        canCraft = customRecipe.Recipe.CanCraft(new FarmerInventory(Game1.player));

                        // If it's a cooking recipe, check the fridge as well
                        if (!canCraft && recipe.isCookingRecipe && Game1.currentLocation is FarmHouse farm) {
                            canCraft = customRecipe.Recipe.CanCraft(new ChestInventory(farm.fridge.Value));
                        }
                    } else {
                        canCraft = recipe.doesFarmerHaveIngredientsInInventory(___cooking ? (Game1.currentLocation as FarmHouse)?.fridge.Value.items : null);
                    }

                    if (!canCraft) {
                        // Can't be crafted, but known
                        DrawRecipeComponent2(component, Color.LightGray * 0.4f, 0.89f);
                    } else {
                        // Can be crafted
                        DrawRecipeComponent(component);

                        // Show how many are crafted
                        if (recipe.numberProducedPerCraft > 1) {
                            NumberSprite.draw(recipe.numberProducedPerCraft, b, new Vector2(component.bounds.X + 64 - 2, component.bounds.Y + 64 - 2), Color.Red, (float) (0.5 * (component.scale / 4.0)), 0.97f, 1f, 0);
                        }
                    }
                }
            }

            // Draw the held items and tooltip
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            // Draw the tooltip
            if (___hoverItem != null) {
                IClickableMenu.drawToolTip(b, ___hoverText, ___hoverTitle, ___hoverItem, isHoldingItem);
            } else if (!string.IsNullOrEmpty(___hoverText)) {
                IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont, isHoldingItem ? 64 : 0, isHoldingItem ? 64 : 0);
            }

            // Draw the first 3 held items
            foreach ((Item heldItem, int i) in extraData.HeldItems.Take(3).Select((heldItem, i) => (heldItem, i)).Reverse()) {
                // Each item's transparency increases by 0.25
                Vector2 location = new Vector2(Game1.getOldMouseX() + 16 + 8 * i, Game1.getOldMouseY() + 16 + 8 * i);
                heldItem.drawInMenu(b, location, 1f, 1f, 0.9f + 0.01f * i, true, new Color(1f, 1f, 1f, 1 - i * 0.25f), true);
            }

            // TODO: Draw a +N if there are more than three stacks of items

            // Draw the upper right close button if it exists
            __instance.upperRightCloseButton?.draw(b);

            // Draw the scroll down button
            if (__instance.downButton != null && ___currentCraftingPage < __instance.pagesOfCraftingRecipes.Count - 1) {
                __instance.downButton.draw(b);
            }

            // Draw the scroll up button
            if (__instance.upButton != null && ___currentCraftingPage > 0) {
                __instance.upButton.draw(b);
            }

            // Draw the mouse if this is the cooking menu
            if (___cooking) {
                __instance.drawMouse(b);
            }

            // Draw the hovered recipe's description
            if (___hoverRecipe != null) {
                // Get the offset
                int xOffset = isHoldingItem ? 48 : 0;
                int yOffset = isHoldingItem ? 48 : 0;

                // Get the buff icons
                string[] buffIconsToDisplay;
                if (___cooking && ___lastCookingHover != null && Game1.objectInformation[___lastCookingHover.ParentSheetIndex].Split('/').Length > 7) {
                    buffIconsToDisplay = Game1.objectInformation[___lastCookingHover.ParentSheetIndex].Split('/')[7].Split(' ');
                } else {
                    buffIconsToDisplay = null;
                }

                // Draw the recipe's description
                IClickableMenu.drawHoverText(b, " ", Game1.smallFont, xOffset, yOffset, -1, ___hoverRecipe.DisplayName, -1, buffIconsToDisplay, ___lastCookingHover, 0, -1, -1, -1, -1, 1f, ___hoverRecipe);
            }

            // Don't use the original drawing code
            return false;

            void DrawHorizontalPartition(int yPosition) {
                b.Draw(Game1.menuTexture, new Vector2(__instance.xPositionOnScreen, yPosition), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 4), Color.White);
                b.Draw(Game1.menuTexture, new Rectangle(__instance.xPositionOnScreen + 64, yPosition, __instance.width - 128, 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 6), Color.White);
                b.Draw(Game1.menuTexture, new Vector2(__instance.xPositionOnScreen + __instance.width - 64, yPosition), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 7), Color.White);
            }

            void DrawRecipeComponent(ClickableTextureComponent component) {
                if (component is ModRecipeComponent modComponent) {
                    modComponent.Draw(b);
                } else {
                    component.draw(b);
                }
            }

            void DrawRecipeComponent2(ClickableTextureComponent component, Color tint, float layerDepth) {
                if (component is ModRecipeComponent modComponent) {
                    modComponent.Draw(b, tint, layerDepth);
                } else {
                    component.draw(b, tint, layerDepth);
                }
            }
        }

        private static bool CraftingPage_receiveLeftClick_Prefix(CraftingPage __instance, int x, int y, bool playSound, ref int ___currentCraftingPage, bool ___cooking, ref Item ___heldItem) {
            CraftingPageData extraData = CraftingManager.GetExtraData(__instance);

            // Check if the close button is clicked
            if (__instance.upperRightCloseButton != null && __instance.readyToClose() && __instance.upperRightCloseButton.containsPoint(x, y)) {
                if (playSound) {
                    Game1.playSound("bigDeSelect");
                }

                __instance.exitThisMenu();
                return false;
            }

            // Inventory clicked
            CraftingManager.LeftClickInventory(__instance, x, y, playSound);

            // Page up button
            if (__instance.upButton != null && __instance.upButton.containsPoint(x, y) && ___currentCraftingPage > 0) {
                Game1.playSound("coin");
                ___currentCraftingPage = Math.Max(0, ___currentCraftingPage - 1);
                __instance.upButton.scale = __instance.upButton.baseScale;
                __instance.upButton.leftNeighborID = __instance.pagesOfCraftingRecipes[___currentCraftingPage].Last().Key.myID;
            }

            // Page down button
            if (__instance.downButton != null && __instance.downButton.containsPoint(x, y) && ___currentCraftingPage < __instance.pagesOfCraftingRecipes.Count - 1) {
                Game1.playSound("coin");
                ___currentCraftingPage = Math.Min(__instance.pagesOfCraftingRecipes.Count - 1, ___currentCraftingPage + 1);
                __instance.downButton.scale = __instance.downButton.baseScale;
                __instance.downButton.leftNeighborID = __instance.pagesOfCraftingRecipes[___currentCraftingPage].Last().Key.myID;
            }

            // Check if the trash can was clicked
            if (__instance.trashCan != null && __instance.trashCan?.containsPoint(x, y) == true) {
                CraftingManager.TrashHeldItems(__instance, ref ___heldItem);
            }

            // Check if the area outside the crafting page was clicked
            if (!__instance.isWithinBounds(x, y) && extraData.HeldItems.FirstOrDefault() is Item thrownItem && thrownItem.canBeTrashed()) {
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(thrownItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                extraData.HeldItems.RemoveAt(0);
            }

            // Get the clicked component
            if (!(__instance.pagesOfCraftingRecipes[___currentCraftingPage].Keys.FirstOrDefault(k => k.containsPoint(x, y)) is ClickableTextureComponent component)) {
                return false;
            }

            // Make sure this isn't an unknown recipe
            if (component.hoverText.Equals("ghosted")) {
                return false;
            }

            // Craft the item (clickCraftingRecipe will check if it can be crafted)
            CraftingManager.CraftingPage_clickCraftingRecipe_Prefix(__instance, component, playSound, ___currentCraftingPage, ___cooking);

            return false;
        }

        private static bool CraftingPage_receiveRightClick_Prefix(CraftingPage __instance, int x, int y, bool playSound, int ___currentCraftingPage, bool ___cooking) {
            // Handle items moving around when the inventory is right clicked
            CraftingManager.RightClickInventory(__instance, x, y, playSound);

            // Get the clicked component
            if (!(__instance.pagesOfCraftingRecipes[___currentCraftingPage].Keys.FirstOrDefault(k => k.containsPoint(x, y)) is ClickableTextureComponent component)) {
                return false;
            }

            // Make sure this isn't an unknown recipe
            if (component.hoverText.Equals("ghosted")) {
                return false;
            }

            // Craft the item (clickCraftingRecipe will check if it can be crafted)
            CraftingManager.CraftingPage_clickCraftingRecipe_Prefix(__instance, component, playSound, ___currentCraftingPage, ___cooking);

            return false;
        }
        #endregion

        #region CraftingRecipe Patches
        private static bool CraftingRecipe_createItem(CraftingRecipe __instance, ref Item __result) {
            if (!(__instance is ModCraftingRecipe recipe)) {
                return true;
            }

            // throw new InvalidOperationException($"The recipe {recipe.DisplayName} was created by code that should not be called");
            __result = recipe.Recipe.Results.FirstOrDefault() is IRecipePart firstResult && firstResult.TryCreateOne(out Item resultItem) ? resultItem : new SObject(Vector2.Zero, Objects.Torch, 1);
            return false;
        }

        private static bool CraftingRecipe_getNumberOfIngredients_Prefix(CraftingRecipe __instance, ref int __result) {
            if (!(__instance is ModCraftingRecipe modRecipe)) {
                return true;
            }

            __result = modRecipe.Recipe.Ingredients.Count();
            return false;
        }

        private static bool CraftingRecipe_drawRecipeDescription_Prefix(CraftingRecipe __instance, SpriteBatch b, Vector2 position, int width, string ___description) {
            if (!(__instance is ModCraftingRecipe modRecipe)) {
                return true;
            }

            // Draw the description
            CraftingManager.DrawRecipeDescription(modRecipe, b, position, width, ___description, out _);

            // Don't call original method
            return false;
        }

        private static bool CraftingRecipe_getDescriptionHeight(CraftingRecipe __instance, int width, ref int __result, ref string ___description) {
            if (!(__instance is ModCraftingRecipe modRecipe)) {
                return true;
            }

            CraftingManager.DrawRecipeDescription(modRecipe, null, Vector2.Zero, width, ___description, out __result);
            return false;

        }
        #endregion

        private class CraftingPageData {
            private readonly CraftingPage _page;
            public List<Item> HeldItems { get; } = new List<Item>();

            public CraftingPageData(CraftingPage page) {
                this._page = page;
            }

            public void AddHeldItem(Item addedItem) {
                Stack<Item> modifiedItems = new Stack<Item>();

                for (int i = 0; i < this.HeldItems.Count; i++) {
                    Item item = this.HeldItems[i];

                    // Check if the two items can be combined
                    if (!addedItem.canStackWith(item)) {
                        continue;
                    }

                    // Add as much as possible to the current item
                    int added = Math.Min(addedItem.Stack, item.maximumStackSize() - item.Stack);
                    addedItem.Stack -= added;
                    item.Stack += added;
                    modifiedItems.Push(item);

                    // Remove the modified item so it can be added back to the front
                    this.HeldItems.RemoveAt(i);
                    i--;

                    // Check if any of the added item remain
                    if (addedItem.Stack == 0) {
                        break;
                    }
                }

                // Add all the modified items back
                while (modifiedItems.Any()) {
                    this.HeldItems.Add(modifiedItems.Pop());
                }

                // Add whatever remains of the item
                if (addedItem.Stack > 0) {
                    this.HeldItems.Add(addedItem);
                }
            }
        }

        private class ModRecipeComponent : ClickableTextureComponent {
            private readonly ISprite _sprite;

            public ModRecipeComponent(string name, Rectangle bounds, string label, string hoverText, ISprite sprite, float scale, bool drawShadow = false) : base(name, bounds, label, hoverText, sprite.ParentSheet.TrackedTexture.CurrentTexture, new Rectangle(sprite.U, sprite.V, sprite.Width, sprite.Height), scale, drawShadow) {
                this._sprite = sprite;
            }

            public void Draw(SpriteBatch b) {
                // Don't draw if invisible
                if (!this.visible) {
                    return;
                }

                // Draw the texture
                this.Draw(b, Color.White, (float) (0.860000014305115 + (double) this.bounds.Y / 20000.0));
            }

            public void Draw(SpriteBatch b, Color c, float layerDepth) {
                // Don't draw if invisible
                if (!this.visible) {
                    return;
                }

                // Draw the texture
                if (!this.drawShadow) {
                    this._sprite.Draw(b, new Vector2(this.bounds.X + this.sourceRect.Width / 2f * this.baseScale, this.bounds.Y + this.sourceRect.Height / 2f * this.baseScale), c, 0.0f, new Vector2(this.sourceRect.Width / 2f, this.sourceRect.Height / 2f), this.scale, SpriteEffects.None, layerDepth);
                } else {
                    Vector2 position = new Vector2(this.bounds.X + this.sourceRect.Width / 2f * this.baseScale, this.bounds.Y + this.sourceRect.Height / 2f * this.baseScale);
                    Vector2 origin = new Vector2(this.sourceRect.Width / 2f, this.sourceRect.Height / 2f);

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (this.scale == -1.0) {
                        this.scale = 4f;
                    }

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (layerDepth == -1.0) {
                        layerDepth = position.Y / 10000f;
                    }

                    this._sprite.Draw(b, position + new Vector2(-4, -4), Color.Black * 0.35f, 0.0f, origin, this.scale, SpriteEffects.None, layerDepth - 0.0001f);
                    this._sprite.Draw(b, position, c, 0.0f, origin, this.scale, SpriteEffects.None, layerDepth);
                }

                // Don't draw the label if there is none
                if (string.IsNullOrEmpty(this.label)) {
                    return;
                }

                // Draw the label
                b.DrawString(Game1.smallFont, this.label, new Vector2(this.bounds.X + this.bounds.Width, this.bounds.Y + (this.bounds.Height / 2 - Game1.smallFont.MeasureString(this.label).Y / 2f)), Game1.textColor);
            }
        }
    }
}