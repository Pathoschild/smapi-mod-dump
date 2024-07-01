/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Menus;

namespace CraftAnything.Integrations.BetterCrafting
{
    internal static class BetterCrafting
    {
        internal static IBetterCrafting? BetterCraftingApi;

        public static void Register()
        {
            BetterCraftingApi = ModEntry.IHelper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
            if (BetterCraftingApi is null)
                return;
            RegisterCategories();
            BetterCraftingApi.AddRecipeProvider(new CAFRecipeProvider());
        }

        private static void RegisterCategories()
        {
            string id = ModEntry.IHelper.ModRegistry.ModID;
            var i18n = ModEntry.IHelper.Translation;

            //These render the full length wall / floor paper for the icon sprite, leaving them in misc for now
            //BetterCraftingApi!.CreateDefaultCategory(false, $"{id}_Wallpaper", () => i18n.Get("BetterCrafting.Category.Wallpapers"), null, null, true, [new CategoryRules.WallpaperRule()]);
            //BetterCraftingApi.CreateDefaultCategory(false, $"{id}_Flooring", () => i18n.Get("BetterCrafting.Category.Flooring"), null, null, true, [new CategoryRules.FlooringRule()]);
            BetterCraftingApi!.CreateDefaultCategory(false, $"{id}_Hats", () => i18n.Get("BetterCrafting.Category.Hats"), null, null, true, [new CategoryRules.HatsRule()]);
            BetterCraftingApi.CreateDefaultCategory(false, $"{id}_Boots", () => i18n.Get("BetterCrafting.Category.Boots"), null, null, true, [new CategoryRules.BootsRule()]);
            BetterCraftingApi.CreateDefaultCategory(false, $"{id}_Weapons", () => i18n.Get("BetterCrafting.Category.Weapons"), null, null, true, [new CategoryRules.WeaponsRule()]);
            BetterCraftingApi.CreateDefaultCategory(false, $"{id}_Tools", () => i18n.Get("BetterCrafting.Category.Tools"), null, null, true, [new CategoryRules.ToolsRule()]);
            BetterCraftingApi.CreateDefaultCategory(false, $"{id}_Clothes", () => i18n.Get("BetterCrafting.Category.Clothing"), null, null, true, [new CategoryRules.ClothesRule()]);
        }
    }

    public class CAFRecipeProvider : IRecipeProvider
    {
        public int RecipePriority => 1;

        public IRecipe? GetRecipe(CraftingRecipe recipe)
        {
            if (!Patches.isValid(recipe, out var typeDef))
                return null;
            Vector2 gridSize = getGridSizeForRecipe(recipe);
            return BetterCrafting.BetterCraftingApi!.RecipeBuilder(recipe)
                                                    .SetDrawFunction((b, bounds, color, _, _, layerDepth, cmp) => drawCAFRecipe(b, bounds, color, layerDepth, cmp, recipe), () => typeDef == ItemRegistry.type_wallpaper || typeDef == ItemRegistry.type_floorpaper)
                                                    .Texture(() => recipe.GetItemData().GetTexture())
                                                    .Source(() => recipe.GetItemData().GetSourceRect())
                                                    .GridSize((int)gridSize.X, (int)gridSize.Y)
                                                    .Build();
        }

        public bool CacheAdditionalRecipes => false;

        public IEnumerable<IRecipe>? GetAdditionalRecipes(bool cooking) => null;

        private void drawCAFRecipe(SpriteBatch b, Rectangle bounds, Color color, float layerDepth, ClickableTextureComponent? cmp, CraftingRecipe recipe)
        {
            var itemData = recipe.GetItemData();
            if (CraftingRecipe.craftingRecipes.TryGetValue(recipe.name, out var data) && ArgUtility.TryGet(data.Split('/'), 6, out var typeDef, out _, false))
            {
                if (typeDef == ItemRegistry.type_wallpaper || typeDef == ItemRegistry.type_floorpaper)
                {
                    recipe.createItem().drawInMenu(b, new(bounds.X, bounds.Y), (cmp?.scale ?? 4f) / 4, 1f, layerDepth, StackDrawType.Hide, color, cmp?.drawShadow ?? false);
                    return;
                }
            }
            b.Draw(itemData.GetTexture(), new(bounds.X, bounds.Y), itemData.GetSourceRect(), color, 0f, Vector2.Zero, cmp?.scale ?? 4f, SpriteEffects.None, layerDepth);
        }

        private Vector2 getGridSizeForRecipe(CraftingRecipe recipe)
        {
            var sourceRect = recipe.GetItemData().GetSourceRect();
            if (CraftingRecipe.craftingRecipes.TryGetValue(recipe.name, out var data) && ArgUtility.TryGet(data.Split('/'), 6, out var typeDef, out _, false) && (typeDef == ItemRegistry.type_wallpaper || typeDef == ItemRegistry.type_floorpaper))
                return Vector2.One;
            return new(Math.Max(1, sourceRect.Width / 16), Math.Max(1, sourceRect.Height / 16));
        }
    }

    public class CategoryRules
    {
        public class WallpaperRule : IDynamicRuleData
        {
            public string Id => "ContextTag";

            public IDictionary<string, JToken> Fields => new Dictionary<string, JToken>()
            {
                { "Input", "item_wallpaper" }
            };
        }

        public class FlooringRule : IDynamicRuleData
        {
            public string Id => "ContextTag";

            public IDictionary<string, JToken> Fields => new Dictionary<string, JToken>()
            {
                { "Input", "item_flooring" }
            };
        }

        public class HatsRule : IDynamicRuleData
        {
            public string Id => "Category";

            public IDictionary<string, JToken> Fields => new Dictionary<string, JToken>()
            {
                { "Input", "-95" }
            };
        }

        public class BootsRule : IDynamicRuleData
        {
            public string Id => "Category";

            public IDictionary<string, JToken> Fields => new Dictionary<string, JToken>()
            {
                { "Input", "-97" }
            };
        }

        public class WeaponsRule : IDynamicRuleData
        {
            public string Id => "Category";

            public IDictionary<string, JToken> Fields => new Dictionary<string, JToken>()
            {
                { "Input", "-98" }
            };
        }

        public class ToolsRule : IDynamicRuleData
        {
            public string Id => "Category";

            public IDictionary<string, JToken> Fields => new Dictionary<string, JToken>()
            {
                { "Input", "-99" }
            };
        }

        public class ClothesRule : IDynamicRuleData
        {
            public string Id => "Category";

            public IDictionary<string, JToken> Fields => new Dictionary<string, JToken>()
            {
                { "Input", "-100" }
            };
        }
    }
}
