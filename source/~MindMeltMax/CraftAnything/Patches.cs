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
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Text;

namespace CraftAnything
{
    internal static class Patches
    {
        public static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.GetItemData)),
                prefix: new(typeof(Patches), nameof(CraftingRecipe_GetItemData_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.drawMenuView)),
                transpiler: new(typeof(Patches), nameof(CraftingRecipe_DrawMenuView_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), "layoutRecipes"),
                transpiler: new(typeof(Patches), nameof(CraftingPage_LayoutRecipes_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.FirstMethod(typeof(IClickableMenu), x => x.Name == nameof(IClickableMenu.drawHoverText) && x.GetParameters().ElementAt(1).ParameterType == typeof(StringBuilder)), //If you honestly believe I'm typing out all those params...
                prefix: new(typeof(Patches), nameof(IClickableMenu_DrawHoverText_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), nameof(CraftingPage.draw), [typeof(SpriteBatch)]),
                transpiler: new(typeof(Patches), nameof(CraftingPage_Draw_Transpiler))
            );
        }

        internal static bool CraftingRecipe_GetItemData_Prefix(CraftingRecipe __instance, bool useFirst, ref ParsedItemData __result)
        {
            try
            {
                if (!isValid(__instance, out var typeDef))
                    return true;
                string? str = useFirst ? __instance.itemToProduce.FirstOrDefault() : Game1.random.ChooseFrom(__instance.itemToProduce);
                __result = ItemRegistry.GetDataOrErrorItem(typeDef.Trim() + str);
                return false;
            }
            catch (Exception ex)
            {
                ModEntry.IMonitor.Log($"Failed Patching {nameof(CraftingRecipe.GetItemData)}", LogLevel.Error);
                ModEntry.IMonitor.Log($"[{nameof(CraftingRecipe_GetItemData_Prefix)}] {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            }
            return true;
        }

        internal static IEnumerable<CodeInstruction> CraftingRecipe_DrawMenuView_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            matcher.Start().MatchStartForward([
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld),
                new(OpCodes.Brtrue_S),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt),
                new(OpCodes.Br_S),
                new(OpCodes.Ldstr, "(BC)"),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt),
                new(OpCodes.Call),
                new(OpCodes.Call),
                new(OpCodes.Dup)
            ]).Advance(1).RemoveInstructions(9).InsertAndAdvance([
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(getDataForDraw)))
            ]).Labels.Clear();

            return matcher.Instructions();
        }

        internal static IEnumerable<CodeInstruction> CraftingPage_LayoutRecipes_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            matcher.Start().MatchStartForward([
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldfld),
                new(OpCodes.Brtrue_S),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Br_S),
                new(OpCodes.Ldstr, "(BC)"),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Call),
                new(OpCodes.Call),
                new(OpCodes.Dup),
            ]).Advance(1).RemoveInstructions(7).InsertAndAdvance([
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(getDataForDraw)))
            ]).Labels.Clear();

            matcher.Start().MatchEndForward([
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, AccessTools.Method(typeof(CraftingPage), "craftingPageY")),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldc_I4_S),
                new(OpCodes.Mul),
                new(OpCodes.Add),
                new(OpCodes.Ldc_I4_S)
            ]);
            matcher.RemoveInstructions(7).InsertAndAdvance([
                new(OpCodes.Ldloc_S, 9),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(getComponentWidth))),
                new(OpCodes.Ldloc_S, 9),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(getComponentHeight))),
            ]).Labels.Clear();

            matcher.Start().MatchStartForward([
                new(OpCodes.Ldc_I4, 200),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Add),
                new(OpCodes.Stloc_S)
            ]).InsertAndAdvance([
                new(OpCodes.Ldarg_0), 
                new(OpCodes.Ldloca_S, 6),
                new(OpCodes.Ldloca_S, 2),
                new(OpCodes.Ldloca_S, 3),
                new(OpCodes.Ldloca_S, 4),
                new(OpCodes.Ldloc_S, 9),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(createNewPageIfNeeded)))
            ]);

            CodeInstruction startInsert = new(OpCodes.Ldloca_S, 6);

            matcher.Start().MatchEndForward([
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Add),
                new(OpCodes.Ldloc_S),
                new(OpCodes.Call),
                new(OpCodes.Ldloca_S)
            ]).Instruction.MoveLabelsTo(startInsert);
            matcher.InsertAndAdvance([
                startInsert,
                new(OpCodes.Ldloc_3),
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Ldloc_S, 14),
                new(OpCodes.Ldloc_S, 9),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(setOccupiedSpace)))
            ]);

            return matcher.Instructions();
        }

        internal static void IClickableMenu_DrawHoverText_Prefix(ref Item hoveredItem, CraftingRecipe craftingIngredients)
        {
            if (craftingIngredients is null || !isValid(craftingIngredients))
                return;
            hoveredItem = null; //Because of weapon and boot icons, their hover boxes are too high, fix by setting the hoveredItem to null for custom crafting recipes
        }

        internal static IEnumerable<CodeInstruction> CraftingPage_Draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new(instructions, generator);

            CodeInstruction startInsert = new(OpCodes.Ldarg_0);

            matcher.Start().MatchStartForward([
                new(OpCodes.Ldloca_S),
                new(OpCodes.Call),
                new(OpCodes.Brtrue),
                new(OpCodes.Leave_S),
            ]).CreateLabel(out var l1);

            matcher.Start().MatchStartForward([
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldfld),
                new(OpCodes.Ldstr, "ghosted"),
                new(OpCodes.Callvirt),
                new(OpCodes.Brfalse_S),
            ]).CreateLabel(out var l2);
            matcher.InsertAndAdvance([
                startInsert,
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(drawnOverride))),
                new(OpCodes.Brfalse_S, l2),
                new(OpCodes.Br_S, l1)
            ]);

            return matcher.Instructions();
        }

        private static ParsedItemData getDataFor(string typeDef, string itemId) => ItemRegistry.GetDataOrErrorItem(typeDef.Trim() + itemId);

        private static string getDataForDraw(CraftingRecipe recipe)
        {
            string indexOfMenuView = recipe.getIndexOfMenuView();
            if (!isValid(recipe, out var typeDef))
                return recipe.bigCraftable ? "(BC)" + indexOfMenuView : indexOfMenuView;
            return typeDef + indexOfMenuView;
        }

        internal static bool isValid(CraftingRecipe recipe, [NotNullWhen(true)] out string? typeDef)
        {
            typeDef = null;
            return !recipe.isCookingRecipe &&
                   CraftingRecipe.craftingRecipes.TryGetValue(recipe.name, out string? data) &&
                   ArgUtility.TryGet(data.Split('/'), 6, out typeDef, out _, false) &&
                   typeDef != ItemRegistry.type_object &&
                   typeDef != ItemRegistry.type_bigCraftable;
        }

        internal static bool isValid(CraftingRecipe recipe) => isValid(recipe, out _);

        private static int getComponentWidth(CraftingRecipe recipe)
        {
            if (isValid(recipe, out var typeDef) && (typeDef == ItemRegistry.type_wallpaper || typeDef == ItemRegistry.type_floorpaper))
                return 64;
            return recipe.GetItemData().GetSourceRect().Width * 4;
        }

        private static int getComponentHeight(CraftingRecipe recipe)
        {
            if (isValid(recipe, out var typeDef) && (typeDef == ItemRegistry.type_wallpaper || typeDef == ItemRegistry.type_floorpaper))
                return 64;
            return recipe.GetItemData().GetSourceRect().Height * 4;
        }

        private static void setOccupiedSpace(ref ClickableTextureComponent[,] spaces, int x, int y, ClickableTextureComponent component, CraftingRecipe recipe)
        {
            if (recipe is null || !isValid(recipe, out var typeDef))
                return;
            if (typeDef == ItemRegistry.type_wallpaper || typeDef == ItemRegistry.type_floorpaper)
                return;
            var sourceRect = recipe.GetItemData().GetSourceRect();
            for (int i = 0; i < sourceRect.Width / 16; i++)
                for (int j = 0; j < sourceRect.Height / 16; j++)
                    spaces[x + i, y + j] = component;
        }

        private static bool drawnOverride(CraftingPage menu, SpriteBatch b, ClickableTextureComponent cmp)
        {
            if (menu is null || cmp is null)
                return false;
            var recipe = menu.pagesOfCraftingRecipes[menu.currentCraftingPage][cmp];
            if (!isValid(recipe, out var typeDef) || (typeDef != ItemRegistry.type_wallpaper && typeDef != ItemRegistry.type_floorpaper))
                return false;
            bool hasEnoughItems = recipe.doesFarmerHaveIngredientsInInventory(ModEntry.IHelper.Reflection.GetMethod(menu, "getContainerContents")?.Invoke<IList<Item>>());
            Color color = cmp.hoverText.Equals("ghosted") ? Color.Black * .35f : (!hasEnoughItems ? Color.DimGray * .4f : Color.White);
            recipe.createItem().drawInMenu(b, new(cmp.bounds.X, cmp.bounds.Y), cmp.scale / 4, 1f, 0.89f, StackDrawType.Hide, color, cmp.drawShadow);
            return true;
        }

        private static void createNewPageIfNeeded(CraftingPage menu, ref ClickableTextureComponent[,] newPageLayout, ref Dictionary<ClickableTextureComponent, CraftingRecipe> newPage, ref int x, ref int y, CraftingRecipe recipe)
        {
            if (recipe is null || !isValid(recipe, out var typeDef))
                return;
            if (typeDef == ItemRegistry.type_wallpaper || typeDef == ItemRegistry.type_floorpaper)
                return;
            var sourceRect = recipe.GetItemData().GetSourceRect();
            for (int i = 0; i < sourceRect.Width / 16; i++)
            {
                if (x + i >= 10)
                {
                    x = 0;
                    ++y;
                }
                for (int j = 0; j < sourceRect.Height / 16; j++)
                {
                    if (y + j >= 4)
                    {
                        newPage = ModEntry.IHelper.Reflection.GetMethod(menu, "createNewPage").Invoke<Dictionary<ClickableTextureComponent, CraftingRecipe>>(null);
                        newPageLayout = ModEntry.IHelper.Reflection.GetMethod(menu, "createNewPageLayout").Invoke<ClickableTextureComponent[,]>(null);
                        x = 0;
                        y = 0;
                        return;
                    }

                }
            }
        }
    }
}
