/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Patches.Objects
{
    internal class CraftingPagePatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(CraftingPage);

        public CraftingPagePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            if (_helper.ModRegistry.IsLoaded("leclair.bettercrafting") is false)
            {
                _monitor.Log($"Applying CraftingPagePatch...", LogLevel.Trace);

                harmony.Patch(AccessTools.Method(_object, "layoutRecipes", new[] { typeof(List<string>) }), prefix: new HarmonyMethod(GetType(), nameof(LayoutRecipesPrefix)));
                harmony.Patch(AccessTools.Method(_object, "layoutRecipes", new[] { typeof(List<string>) }), postfix: new HarmonyMethod(GetType(), nameof(LayoutRecipesPostfix)));

                harmony.CreateReversePatcher(AccessTools.Method(_object, "spaceOccupied", null), new HarmonyMethod(GetType(), nameof(SpaceOccupiedReversePatch))).Patch();
                harmony.CreateReversePatcher(AccessTools.Method(_object, "craftingPageY", null), new HarmonyMethod(GetType(), nameof(CraftingPageYReversePatch))).Patch();
                harmony.CreateReversePatcher(AccessTools.Method(_object, "createNewPageLayout", null), new HarmonyMethod(GetType(), nameof(CreateNewPageLayoutReversePatch))).Patch();
                harmony.CreateReversePatcher(AccessTools.Method(_object, "createNewPage", null), new HarmonyMethod(GetType(), nameof(CreateNewPageReversePatch))).Patch();
            }
            else
            {
                _monitor.Log($"Skipped applying CraftingPagePatch, due to Better Crafting being loaded!", LogLevel.Trace);
            }
        }

        [HarmonyPriority(Priority.High)]
        private static bool LayoutRecipesPrefix(CraftingPage __instance, List<string> playerRecipes)
        {
            if (playerRecipes is null)
            {
                return true;
            }

            foreach (var model in Archery.modelManager.GetModelsWithValidRecipes().Where(m => m.Recipe.HasRequirements(Game1.player)))
            {
                if (playerRecipes.Contains(model.Id))
                {
                    playerRecipes.Remove(model.Id);
                }
            }

            return true;
        }

        private static void LayoutRecipesPostfix(CraftingPage __instance, bool ___cooking, List<string> playerRecipes)
        {
            // Skip if this is the cooking crafting page
            if (___cooking)
            {
                return;
            }

            int craftingPageX = __instance.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - 16;
            int spaceBetweenCraftingIcons = 8;
            Dictionary<ClickableTextureComponent, CraftingRecipe> currentPage = CreateNewPageReversePatch(__instance);
            int x = 0;
            int y = 0;
            int i = 0;

            ClickableTextureComponent[,] pageLayout = CreateNewPageLayoutReversePatch(__instance);
            List<ClickableTextureComponent[,]> pageLayouts = new List<ClickableTextureComponent[,]>
            {
                pageLayout
            };

            foreach (var model in Archery.modelManager.GetModelsWithValidRecipes().Where(m => m.Recipe.HasRequirements(Game1.player)))
            {
                var playerRecipe = model.Id;

                i++;
                CraftingRecipe recipe = new CraftingRecipe(playerRecipe, false);
                while (SpaceOccupiedReversePatch(__instance, pageLayout, x, y, recipe))
                {
                    x++;
                    if (x >= 10)
                    {
                        x = 0;
                        y++;
                        if (y >= 4)
                        {
                            currentPage = CreateNewPageReversePatch(__instance);
                            pageLayout = CreateNewPageLayoutReversePatch(__instance);
                            pageLayouts.Add(pageLayout);
                            x = 0;
                            y = 0;
                        }
                    }
                }

                int id = 200 + i;
                ClickableTextureComponent component = new ClickableTextureComponent("", new Rectangle(craftingPageX + x * (64 + spaceBetweenCraftingIcons), CraftingPageYReversePatch(__instance) + y * 72, 64, 64), null, String.Empty, model.Texture, model.Icon.Source, model.Icon.Scale)
                {
                    myID = id,
                    rightNeighborID = -99998,
                    leftNeighborID = -99998,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    fullyImmutable = true,
                    region = 8000
                };
                currentPage.Add(component, recipe);
                pageLayout[x, y] = component;
            }
        }

        private static bool SpaceOccupiedReversePatch(CraftingPage __instance, ClickableTextureComponent[,] pageLayout, int x, int y, CraftingRecipe recipe)
        {
            return false;
        }

        private static int CraftingPageYReversePatch(CraftingPage __instance)
        {
            return 0;
        }

        private static ClickableTextureComponent[,] CreateNewPageLayoutReversePatch(CraftingPage __instance)
        {
            return new ClickableTextureComponent[10, 4];
        }

        private static Dictionary<ClickableTextureComponent, CraftingRecipe> CreateNewPageReversePatch(CraftingPage __instance)
        {
            return new Dictionary<ClickableTextureComponent, CraftingRecipe>();
        }
    }
}