/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slothsoft.Informant.Api;
using StardewValley.Menus;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Slothsoft.Informant.Implementation;

internal class NewRecipeDisplayable : IDisplayable {

    private static string DisplayableId => "new-recipe";
    private static readonly Rectangle NewSourceRectangle = new(144, 440, 16, 7);
        
    private readonly IModHelper _modHelper;
    private readonly Harmony _harmony;
    
    private static Dictionary<ClickableTextureComponent, CraftingRecipe>? _componentToRecipe;

    public NewRecipeDisplayable(IModHelper modHelper, string? uniqueId = null) {
        _modHelper = modHelper;
        _harmony = new Harmony(uniqueId ?? InformantMod.Instance!.ModManifest.UniqueID);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(ClickableTextureComponent),
                nameof(ClickableTextureComponent.draw),
                new[] {
                    typeof(SpriteBatch),
                    typeof(Color),
                    typeof(float),
                    typeof(int),
                }
            ),
            postfix: new HarmonyMethod(typeof(NewRecipeDisplayable), nameof(DrawOverlayIfNecessary))
        );
        
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(CraftingPage),
                nameof(CraftingPage.draw),
                new[] {
                    typeof(SpriteBatch),
                }
            ),
            prefix: new HarmonyMethod(typeof(NewRecipeDisplayable), nameof(BeforeCraftingPageDraw)),
            postfix: new HarmonyMethod(typeof(NewRecipeDisplayable), nameof(AfterCraftingPageDraw))
        );
    }

    public string Id => DisplayableId;
    public string DisplayName => _modHelper.Translation.Get("NewRecipeDisplayable");
    public string Description => _modHelper.Translation.Get("NewRecipeDisplayable.Description");

    private static void DrawOverlayIfNecessary(ClickableTextureComponent __instance, SpriteBatch b) {
        if (_componentToRecipe == null) {
            // this can be any kind of clickable in the game - we ignore everything that is no recipe
            return;
        }
        
        var recipe = _componentToRecipe.GetValueOrDefault(__instance);
        if (recipe == null) {
            // we are on the recipe page, but have no recipe? ignore!
            return;
        }
        
        var config = InformantMod.Instance?.Config ?? new InformantConfig();
        if (!config.DisplayIds.GetValueOrDefault(DisplayableId, true)) {
            return; // this "decorator" is deactivated
        }

        // recipe.timesCrafted is not updated it seems
        var timesCrafted = recipe.isCookingRecipe 
                ? (Game1.player.recipesCooked.ContainsKey(recipe.getIndexOfMenuView()) ? Game1.player.recipesCooked[recipe.getIndexOfMenuView()] : 0)
                : (Game1.player.craftingRecipes.ContainsKey(recipe.name) ? Game1.player.craftingRecipes[recipe.name] : 0);
        if (timesCrafted > 0) {
            // we are on the recipe page, have a recipe which was already craftet? nice, it's not new
            return;
        }

        var scale = recipe.isCookingRecipe ? 2f : 3f;
        b.Draw(Game1.mouseCursors, __instance.bounds with {
                Width = (int) (NewSourceRectangle.Width * scale),
                Height = (int) (NewSourceRectangle.Height * scale),
            } , NewSourceRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 1);
    }


    private static void BeforeCraftingPageDraw(CraftingPage __instance) {
        _componentToRecipe = __instance.pagesOfCraftingRecipes
            .SelectMany(p => p)
            .ToDictionary(e => e.Key, e => e.Value);
    }
    
    private static void AfterCraftingPageDraw() {
        _componentToRecipe = null;
    }
}