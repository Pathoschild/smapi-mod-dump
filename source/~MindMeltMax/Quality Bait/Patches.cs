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
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QualityBait
{
    internal static class Patches
    {
        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), nameof(CraftingPage.draw), [typeof(SpriteBatch)]),
                transpiler: new(typeof(Patches), nameof(CraftingPage_Draw_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
                prefix: new(typeof(Patches), nameof(CrabPot_DayUpdate_Prefix)),
                postfix: new(typeof(Patches), nameof(CrabPot_DayUpdate_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.pullFishFromWater)),
                prefix: new(typeof(Patches), nameof(FishingRod_PullFishFromWater_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients)),
                prefix: new(typeof(Patches), nameof(CraftingRecipe_ConsumeIngredients_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory)),
                prefix: new(typeof(Patches), nameof(CraftingRecipe_DoesFarmerHaveIngredientsInInventory_Prefix)) //Holy shit, that's a long name
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MachineDataUtility), nameof(MachineDataUtility.GetOutputItem)),
                postfix: new(typeof(Patches), nameof(MachineDataUtility_GetOutputItem_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.drawMenuView)),
                postfix: new(typeof(Patches), nameof(CraftingRecipe_DrawMenuView_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.createItem)),
                postfix: new(typeof(Patches), nameof(CraftingRecipe_CreateItem_Postfix))
            );
        }

        #region Transpilers

        internal static IEnumerable<CodeInstruction> CraftingPage_Draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            CodeInstruction startInsert = new(OpCodes.Ldarg_0);

            matcher.Start().MatchStartForward([ //move next iterator of foreach loop
                new(OpCodes.Ldloca_S),
                new(OpCodes.Call),
                new(OpCodes.Brtrue),
                new(OpCodes.Leave_S),
            ]).Instruction.MoveLabelsTo(startInsert); //move labels from exit loop index to custom draw stack

            matcher.InsertAndAdvance([
                startInsert, //this
                new(OpCodes.Ldarg_1), //b
                new(OpCodes.Ldloc_2), //key
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(drawQualityIfNeeded))),
            ]);

            return matcher.Instructions();
        }

        #endregion

        #region Patches

        internal static void CrabPot_DayUpdate_Prefix(CrabPot __instance, ref (bool, int) __state)
        {
            __state = (false, 0);
            if (__instance.heldObject.Value is null)
                __state = (true, __instance.bait.Value?.Quality ?? 0);
        }

        internal static void CrabPot_DayUpdate_Postfix(CrabPot __instance, (bool, int) __state)
        {
            if (__instance.heldObject.Value is Object obj && __state.Item1)
                __instance.heldObject.Value.Quality = getQualityForCatch(obj.ItemId, obj.Quality, __state.Item2);
        }

        internal static void FishingRod_PullFishFromWater_Prefix(FishingRod __instance, string fishId, ref int fishQuality)
        {
            if (__instance.attachments.Count > 0 && __instance.attachments[0] is not null and Object obj)
                fishQuality = getQualityForCatch(fishId, fishQuality, obj.Quality);
        }

        // Choosing to still prefix this instead of transpiling, because it's easier that way
        internal static bool CraftingRecipe_ConsumeIngredients_Prefix(CraftingRecipe __instance, List<IInventory> additionalMaterials)
        {
            if (!isKnownRecipe(__instance))
                return true;
            var recipe = __instance;
            var quality = getQualityForRecipe(recipe);
            foreach (var ingredient in recipe.recipeList)
            {
                int ingredientCount = ingredient.Value;
                var items = getItemsWithId(ingredient.Key, Game1.player.Items, quality);
                for (int i = 0; i < items.Count; i++)
                {
                    int index = Game1.player.Items.IndexOf(items[i]);
                    int num = ingredientCount;
                    ingredientCount -= items[i].Stack;
                    if ((Game1.player.Items[index].Stack -= num) <= 0)
                        Game1.player.Items[index] = null;
                    if (ingredientCount <= 0)
                        break;
                }

                if (ingredientCount <= 0)
                    continue;

                for (int i = 0; i < additionalMaterials.Count; i++)
                {
                    IInventory inventory = additionalMaterials[i];
                    if (inventory is null)
                        continue;
                    items = getItemsWithId(ingredient.Key, Game1.player.Items, quality);
                    for (int j = 0; j < items.Count; j++)
                    {
                        int index = inventory.IndexOf(items[j]);
                        int num = ingredientCount;
                        ingredientCount -= items[j].Stack;
                        if ((inventory[index].Stack -= num) <= 0)
                        {
                            inventory[index] = null;
                            inventory.RemoveEmptySlots();
                        }
                        if (ingredientCount <= 0)
                            break;
                    }
                    if (ingredientCount <= 0)
                        break;
                }
            }
            return false;
        }

        //Same as above, just can't be arsed to go through a more difficult transpiler
        internal static bool CraftingRecipe_DoesFarmerHaveIngredientsInInventory_Prefix(CraftingRecipe __instance, IList<Item> extraToCheck, ref bool __result)
        {
            if (!isKnownRecipe(__instance))
                return true;
            var recipe = __instance;
            __result = hasIngredientsForRecipe(recipe, Game1.player, extraToCheck);
            return false;
        }

        internal static void MachineDataUtility_GetOutputItem_Postfix(Object machine, Item inputItem, ref Item __result)
        {
            if (__result is null || machine.QualifiedItemId != "(BC)BaitMaker" || !ModEntry.IConfig.BaitMakerQuality)
                return;
            __result.Quality = ModEntry.GetQualityForBait(__result.Quality, inputItem.Quality);
        }

        internal static void CraftingRecipe_DrawMenuView_Postfix(CraftingRecipe __instance, SpriteBatch b, int x, int y)
        {
            if (!isKnownRecipe(__instance))
                return;
            int quality = getQualityForRecipe(__instance);
            Rectangle sourceRect = getSourceRectForQuality(quality);
            float num = quality < 4 ? 0.0f : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
            b.Draw(Game1.mouseCursors, new(x + 12, y + 52), sourceRect, Color.White, 0.0f, new(4f), (float)(3.0 * 1.0 * (1.0 + num)), SpriteEffects.None, .97f);
        }

        internal static void CraftingRecipe_CreateItem_Postfix(CraftingRecipe __instance, ref Item __result)
        {
            if (!isKnownRecipe(__instance) || __result is null)
                return;
            int quality = getQualityForRecipe(__instance);
            __result.Quality = quality;
        }

        #endregion

        #region Utility

        private static bool isKnownRecipe(CraftingRecipe recipe) => ModEntry.Recipes.ContainsKey(recipe.name);

        private static int getQualityForRecipe(CraftingRecipe recipe)
        {
            if (recipe.name.Contains("(Silver)"))
                return Object.medQuality;
            if (recipe.name.Contains("(Gold)"))
                return Object.highQuality;
            if (recipe.name.Contains("(Iridium)"))
                return Object.bestQuality;
            return Object.lowQuality;
        }

        private static Rectangle getSourceRectForQuality(int quality)
        {
            return quality switch
            {
                Object.medQuality => new(338, 400, 8, 8),
                Object.highQuality => new(346, 400, 8, 8),
                Object.bestQuality => new(346, 392, 8, 8),
                _ => new(338, 392, 8, 8)
            };
        }

        private static void drawQualityIfNeeded(CraftingPage menu, SpriteBatch b, ClickableTextureComponent component)
        {
            if (component is null)
                return;
            var recipe = menu.pagesOfCraftingRecipes[menu.currentCraftingPage][component];
            if (!isKnownRecipe(recipe))
                return;
            int quality = getQualityForRecipe(recipe);
            Rectangle sourceRect = getSourceRectForQuality(quality);
            float num = quality < 4 ? 0.0f : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
            b.Draw(Game1.mouseCursors, new(component.bounds.X + 12, component.bounds.Y + 52), sourceRect, Color.White, 0.0f, new(4f), (float)(3.0 * 1.0 * (1.0 + num)), SpriteEffects.None, .97f);
        }

        private static int getQualityForCatch(string itemId, int originalQuality, int baitQuality) => ModEntry.GetQualityForCatch(itemId, originalQuality, baitQuality);

        private static List<Item> getItemsWithId(string id, IEnumerable<Item> source, int baseQuality)
        {
            List<Item> matchingItems = [];
            foreach (var item in source)
                if (item?.ItemId == id && (!ModEntry.IConfig.ForceLowerQuality || item.Quality < baseQuality))
                    matchingItems.Add(item);
            matchingItems.Sort((a, b) => a.Quality.CompareTo(b.Quality));
            return matchingItems;
        }

        private static int getCountOfItemsWithId(string id, IEnumerable<Item> source, int baseQuality)
        {
            int num = 0;
            foreach (var item in source)
                if (item?.ItemId == id && (!ModEntry.IConfig.ForceLowerQuality || item.Quality < baseQuality))
                    num += item.Stack;
            return num;
        }

        private static bool hasIngredientsForRecipe(CraftingRecipe recipe, Farmer who, IList<Item> extraItems, bool original = false)
        {
            if (!isKnownRecipe(recipe)) 
                return original;
            int quality = getQualityForRecipe(recipe);
            foreach (var ingredient in recipe.recipeList)
            {
                int num = ingredient.Value - getCountOfItemsWithId(ingredient.Key, who.Items, quality);
                if (extraItems is not null)
                    num -= getCountOfItemsWithId(ingredient.Key, extraItems, quality);
                if (num > 0)
                    return false;
            }
            return true;
        }

        #endregion
    }
}
