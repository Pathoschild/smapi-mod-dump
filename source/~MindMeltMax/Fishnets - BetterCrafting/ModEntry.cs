/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Leclair.Stardew.BetterCrafting;
using Leclair.Stardew.Common.Crafting;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Fishnets.BetterCrafting
{
    internal class ModEntry : Mod
    {
        internal static IBetterCraftingApi BetterCraftingApi;
        internal static IModHelper IHelper;

        public override void Entry(IModHelper helper)
        {
            IHelper = helper;
            if (!Helper.ModRegistry.IsLoaded("MindMeltMax.Fishnets") || !Helper.ModRegistry.IsLoaded("leclair.bettercrafting"))
            {
                Monitor.Log("Missing one or more requirements, mod will not be loaded", LogLevel.Error);
                return;
            }

            Helper.Events.GameLoop.GameLaunched += (_, _) =>
            {
                BetterCraftingApi = Helper.ModRegistry.GetApi<IBetterCraftingApi>("leclair.bettercrafting");
                if (BetterCraftingApi is null)
                {
                    Monitor.Log("Could not get better crafting api", LogLevel.Error);
                    return;
                }
                BetterCraftingApi.AddRecipeProvider(new RecipeProvider());
                BetterCraftingApi.AddRecipesToDefaultCategory(false, "fishing", new[] { "Fish Net" });
            };
        }
    }

    public interface IBetterCraftingApi
    {
        void AddRecipeProvider(IRecipeProvider provider);

        IRecipeBuilder RecipeBuilder(CraftingRecipe recipe);

        void AddRecipesToDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames);
    }
}
