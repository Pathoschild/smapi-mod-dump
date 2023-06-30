/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace QualityBait
{
    internal static class Patches
    {
        private static IMonitor IMonitor;
        private static IModHelper IHelper;
        private static string CrabPotKey => $"{IHelper.ModRegistry.ModID}.LastQuality";

        public static void Patch(IMonitor monitor, IModHelper helper)
        {
            IMonitor = monitor;
            IHelper = helper;
            Harmony harmony = new(helper.ModRegistry.ModID);

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), "clickCraftingRecipe", new[] { typeof(ClickableTextureComponent), typeof(bool) }),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.ClickCraftingRecipePrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), nameof(CraftingPage.draw), new[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.DrawPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.DayUpdatePrefix)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.DayUpdatePostfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.pullFishFromWater)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.PullFishFromWaterPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.ConsumeIngredientsPrefix))
            );
        }

        private static int GetQualityForRecipe(CraftingRecipe recipe)
        {
            if (recipe.name.Contains("(Silver)"))
                return SObject.medQuality;
            if (recipe.name.Contains("(Gold)"))
                return SObject.highQuality;
            if (recipe.name.Contains("(Iridium)"))
                return SObject.bestQuality;
            return SObject.lowQuality;
        }

        private static Rectangle GetSourceRectForQuality(int quality)
        {
            return quality switch
            {
                SObject.medQuality => new(338, 400, 8, 8),
                SObject.highQuality => new(346, 400, 8, 8),
                SObject.bestQuality => new(346, 392, 8, 8),
                _ => new(338, 392, 8, 8)
            };
        }

        private static bool IsTrashObject(SObject obj) => obj.ParentSheetIndex >= 168 && obj.ParentSheetIndex < 173;

        private static int GetQualityForCatch(int originalQuality, int baitQuality) => ModEntry.GetQualityForCatch(originalQuality, baitQuality);

        private static bool ClickCraftingRecipePrefix(CraftingPage __instance, ClickableTextureComponent c, bool playSound = true)
        {
            try
            {
                int page = IHelper.Reflection.GetField<int>(__instance, "currentCraftingPage").GetValue();
                var heldItem = IHelper.Reflection.GetField<Item>(__instance, "heldItem");
                var recipe = __instance.pagesOfCraftingRecipes[page][c];
                if (!ModEntry.Recipes.ContainsKey(recipe.name))
                    return true;
                SObject bait = (SObject)recipe.createItem();
                bait.Quality = GetQualityForRecipe(recipe);
                if (heldItem.GetValue() is null)
                {
                    recipe.consumeIngredients(__instance._materialContainers);
                    heldItem.SetValue(bait);
                    if (playSound)
                        Game1.playSound("coin");
                }
                else
                {
                    var heldItemValue = heldItem.GetValue();
                    if (heldItemValue.Name != bait.Name || !heldItemValue.getOne().canStackWith(bait.getOne()) || heldItemValue.Stack + recipe.numberProducedPerCraft - 1 >= heldItemValue.maximumStackSize())
                        return false;
                    heldItemValue.Stack += recipe.numberProducedPerCraft;
                    recipe.consumeIngredients(__instance._materialContainers);
                    if (playSound)
                        Game1.playSound("coin");
                }
                Game1.player.checkForQuestComplete(null, -1, -1, bait, null, 2);//Don't think it's needed, but i'm not taking anymore chances

                if (Game1.player.craftingRecipes.ContainsKey(__instance.pagesOfCraftingRecipes[page][c].name))
                    Game1.player.craftingRecipes[recipe.name] += recipe.numberProducedPerCraft;
                Game1.stats.checkForCraftingAchievements();
                if (!Game1.options.gamepadControls || heldItem.GetValue() is null || !Game1.player.couldInventoryAcceptThisItem(heldItem.GetValue()))
                    return false;
                Game1.player.addItemToInventoryBool(heldItem.GetValue());
                heldItem.SetValue(null);
                return false;
            }
            catch(Exception ex)
            {
                IMonitor.Log($"Failed patching clickCraftingRecipe", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        // Yes I'm redrawing the entire menu, believe me I tried to do it differently
        private static bool DrawPrefix(CraftingPage __instance, SpriteBatch b)
        {
            try
            {
                if (IHelper.Reflection.GetField<bool>(__instance, "cooking").GetValue())
                    return true;
                int page = IHelper.Reflection.GetField<int>(__instance, "currentCraftingPage").GetValue();
                if (!__instance.pagesOfCraftingRecipes[page].Any(x => ModEntry.Recipes.ContainsKey(x.Value.name)))
                    return true;

                var heldItem = IHelper.Reflection.GetField<Item>(__instance, "heldItem");
                var getContainerContents = IHelper.Reflection.GetMethod(__instance, "getContainerContents");
                var hoverItem = IHelper.Reflection.GetField<Item>(__instance, "hoverItem");
                var hoverText = IHelper.Reflection.GetField<string>(__instance, "hoverText");
                var hoverTitle = IHelper.Reflection.GetField<string>(__instance, "hoverTitle");
                var hoverAmount = IHelper.Reflection.GetField<int>(__instance, "hoverAmount");
                var hoverRecipe = IHelper.Reflection.GetField<CraftingRecipe>(__instance, "hoverRecipe");

                bool standalone = IHelper.Reflection.GetField<bool>(__instance, "_standaloneMenu").GetValue();

                ClickableTextureComponent trashCan = __instance.trashCan;

                if (standalone)
                    Game1.drawDialogueBox(__instance.xPositionOnScreen, __instance.yPositionOnScreen, __instance.width, __instance.height, false, true);
                IHelper.Reflection.GetMethod(__instance, "drawHorizontalPartition").Invoke(new object?[] { b, __instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256, false, -1, -1, -1 });
                __instance.inventory.draw(b);
                if (trashCan is not null)
                {
                    trashCan.draw(b);
                    b.Draw(Game1.mouseCursors, new Vector2(trashCan.bounds.X + 60, trashCan.bounds.Y + 40), new(564 + Game1.player.trashCanLevel * 18, 129, 18, 10), Color.White, __instance.trashCanLidRotation, new(16f, 10f), 4f, SpriteEffects.None, 0.86f);
                }
                b.End();
                b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);

                foreach (var key in __instance.pagesOfCraftingRecipes[page].Keys)
                {
                    var recipe = __instance.pagesOfCraftingRecipes[page][key];
                    if (!recipe.doesFarmerHaveIngredientsInInventory(getContainerContents.Invoke<IList<Item>>()))
                    {
                        key.draw(b, Color.DimGray * 0.4f, 0.89f);
                        if (recipe.numberProducedPerCraft > 1)
                            NumberSprite.draw(recipe.numberProducedPerCraft, b, new(key.bounds.X + 64 - 2, key.bounds.Y + 64 - 2), Color.LightGray * .75f, 0.5f * (key.scale / 4.0f), .97f, 1f, 0);
                    }
                    else
                    {
                        key.draw(b);
                        if (recipe.numberProducedPerCraft > 1)
                            NumberSprite.draw(recipe.numberProducedPerCraft, b, new(key.bounds.X + 64 - 2, key.bounds.Y + 64 - 2), Color.White, 0.5f * (key.scale / 4.0f), .97f, 1f, 0);
                    }
                    if (ModEntry.Recipes.ContainsKey(recipe.name))
                    {
                        int quality = GetQualityForRecipe(recipe);
                        Rectangle sourceRect = GetSourceRectForQuality(quality);
                        float num = quality < 4 ? 0.0f : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                        b.Draw(Game1.mouseCursors, new(key.bounds.X + 12, key.bounds.Y + 52), sourceRect, Color.White, 0.0f, new(4f), (float)(3.0 * 1.0 * (1.0 + num)), SpriteEffects.None, .97f);
                    }
                }

                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);

                if (hoverItem.GetValue() is not null)
                    IClickableMenu.drawToolTip(b, hoverText.GetValue(), hoverTitle.GetValue(), hoverItem.GetValue(), heldItem.GetValue() is not null);
                else if (!string.IsNullOrEmpty(hoverText.GetValue()))
                {
                    if (hoverAmount.GetValue() > 0)
                        IClickableMenu.drawToolTip(b, hoverText.GetValue(), hoverTitle.GetValue(), null, true, moneyAmountToShowAtBottom: hoverAmount.GetValue());
                    else
                        IClickableMenu.drawHoverText(b, hoverText.GetValue(), Game1.smallFont, heldItem.GetValue() is not null ? 64 : 0, heldItem.GetValue() is not null ? 64 : 0);
                }
                heldItem.GetValue()?.drawInMenu(b, new(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);
                if (__instance.upperRightCloseButton is not null && __instance.shouldDrawCloseButton()) // <- These two lines are the same as base.draw(b);
                    __instance.upperRightCloseButton.draw(b);                                           // <-
                if (__instance.downButton is not null && page < __instance.pagesOfCraftingRecipes.Count - 1)
                    __instance.downButton.draw(b);
                if (__instance.upButton is not null && page > 0)
                    __instance.upButton.draw(b);
                if (standalone)
                {
                    Game1.mouseCursorTransparency = 1f;
                    __instance.drawMouse(b);
                }

                if (hoverRecipe.GetValue() is null)
                    return false;
                int xOffset = heldItem.GetValue() is not null ? 48 : 0;
                int yOffset = heldItem.GetValue() is not null ? 48 : 0;
                string boldTitleText = hoverRecipe.GetValue().DisplayName + (hoverRecipe.GetValue().numberProducedPerCraft > 1 ? $" x{hoverRecipe.GetValue().numberProducedPerCraft}" : "");
                IList<Item> containerContents = getContainerContents.Invoke<IList<Item>>();
                IClickableMenu.drawHoverText(b, " ", Game1.smallFont, xOffset, yOffset, boldTitleText: boldTitleText, craftingIngredients: hoverRecipe.GetValue(), additional_craft_materials: containerContents);
                return false;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(CraftingPage.draw)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static bool DayUpdatePrefix(CrabPot __instance, GameLocation location)
        {
            try
            {
                if (__instance.heldObject.Value is null && __instance.bait.Value is not null)
                    __instance.modData[CrabPotKey] = $"{__instance.bait.Value.Quality}";
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(CrabPot.DayUpdate)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
            return true;
        }

        private static void DayUpdatePostfix(CrabPot __instance, GameLocation location)
        {
            try
            {
                if (__instance.heldObject.Value is not null and SObject obj && !IsTrashObject(obj) && __instance.modData.ContainsKey(CrabPotKey))
                {
                    __instance.heldObject.Value.Quality = GetQualityForCatch(__instance.heldObject.Value.Quality, Convert.ToInt32(__instance.modData[CrabPotKey]));
                    __instance.modData.Remove(CrabPotKey);
                }
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(CrabPot.DayUpdate)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static bool PullFishFromWaterPrefix(FishingRod __instance, int whichFish, int fishSize, ref int fishQuality, int fishDifficulty, bool treasureCaught, bool wasPerfect, bool fromFishPond, bool caughtDouble = false, string itemCategory = "Object")
        {
            try
            {
                if (__instance.attachments.Count > 0 && __instance.attachments[0] is not null and SObject obj && !IsTrashObject(new SObject(whichFish, 1)))
                    fishQuality = GetQualityForCatch(fishQuality, obj.Quality);
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(FishingRod.pullFishFromWater)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
            return true;
        }

        // Re-wrote CraftingRecipe.consumeIngredients to sort by quality
        private static bool ConsumeIngredientsPrefix(CraftingRecipe __instance, List<Chest> additional_materials)
        {
            try
            {
                if (!ModEntry.Recipes.ContainsKey(__instance.name))
                    return true;
                for (int i = __instance.recipeList.Count - 1; i >= 0; --i)
                {
                    var ingredient = __instance.recipeList.ElementAt(i);
                    int ingredientCount = ingredient.Value;
                    bool foundAll = false;
                    var items = GetAllItemsWithId(ingredient.Key, Game1.player.Items).OrderBy(x => x.Value.Quality);

                    for (int j = 0; j < items.Count(); j++) 
                    {
                        int num = ingredientCount;
                        ingredientCount -= items.ElementAt(j).Value.Stack;
                        Game1.player.Items[items.ElementAt(j).Key].Stack -= num;
                        if (Game1.player.Items[items.ElementAt(j).Key].Stack <= 0)
                            Game1.player.Items[items.ElementAt(j).Key] = null;
                        if (ingredientCount <= 0)
                        {
                            foundAll = true;
                            break;
                        }
                    }

                    if (additional_materials is not null && !foundAll)
                    {
                        for (int k = 0; k < additional_materials.Count; ++k)
                        {
                            Chest chest = additional_materials[k];
                            if (chest is null)
                                continue;
                            var chestItems = GetAllItemsWithId(ingredient.Key, chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID)).OrderBy(x => x.Value.Quality);
                            
                            for (int l = 0; l < chestItems.Count(); l++)
                            {
                                int num = ingredientCount;
                                ingredientCount -= chestItems.ElementAt(l).Value.Stack;
                                chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID)[chestItems.ElementAt(l).Key].Stack -= num;
                                if (chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID)[chestItems.ElementAt(l).Key].Stack <= 0)
                                    chest.GetItemsForPlayer(Game1.player.UniqueMultiplayerID)[chestItems.ElementAt(l).Key] = null;
                                if (ingredientCount <= 0)
                                {
                                    foundAll = true;
                                    break;
                                }
                            }

                            if (foundAll)
                                break;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                IMonitor.Log($"Failed patching {nameof(CraftingRecipe.consumeIngredients)}", LogLevel.Error);
                IMonitor.Log($"{ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                return true;
            }
        }

        private static Dictionary<int, SObject> GetAllItemsWithId(int id, IEnumerable<Item> inventory)
        {
            Dictionary<int, SObject> items = new();
            for (int i = 0; i < inventory.Count(); i++)
                if (inventory.ElementAt(i) is not null and SObject obj && !obj.bigCraftable.Value && obj.ParentSheetIndex == id)
                    items.Add(i, obj);
            return items;
        }
    }
}
