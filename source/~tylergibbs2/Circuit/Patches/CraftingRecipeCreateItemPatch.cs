/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(CraftingPage), "clickCraftingRecipe")]
    internal class CraftingRecipeCreateItemPatch
    {
        public static void Postfix(CraftingPage __instance, ClickableTextureComponent c)
        {
            if (!ModEntry.ShouldPatch())
                return;

            int currentCraftingPage = (int)__instance.GetType().GetField("currentCraftingPage", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;
            Item crafted = __instance.pagesOfCraftingRecipes[currentCraftingPage][c].createItem();

            ModEntry.Instance.TaskManager?.OnItemCrafted(crafted);
        }
    }
}
