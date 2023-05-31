/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Weapons;
using Leclair.Stardew.BetterCrafting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;

namespace Archery.Framework.Models.Crafting
{
    internal class Recipe : IRecipe
    {
        private readonly BaseModel _baseModel;

        public Recipe(BaseModel baseModel, List<IIngredient> ingredients)
        {
            _baseModel = baseModel;

            Ingredients = ingredients.ToArray();
        }

        public int SortValue => int.MaxValue;

        public string Name => _baseModel.Id;

        public string DisplayName => _baseModel.GetTranslation(_baseModel.DisplayName);

        public string Description => _baseModel.GetTranslation(_baseModel.Description);

        public CraftingRecipe CraftingRecipe => new CraftingRecipe(_baseModel.Name, false);

        public Texture2D Texture => _baseModel.Texture;

        public Rectangle SourceRectangle => _baseModel.Icon.Source;

        public int GridHeight => 1;

        public int GridWidth => 1;

        public int QuantityPerCraft => _baseModel.Recipe.OutputAmount;

        public IIngredient[] Ingredients { get; protected set; }

        public bool Stackable => _baseModel is not WeaponModel;

        public bool CanCraft(Farmer who)
        {
            return true;
        }

        public Item CreateItem()
        {
            switch (_baseModel)
            {
                case AmmoModel ammoModel:
                    return Arrow.CreateInstance(ammoModel, _baseModel.Recipe.OutputAmount);
                case WeaponModel weaponModel:
                    return Bow.CreateInstance(weaponModel);
            }

            return null;
        }

        public int GetTimesCrafted(Farmer who)
        {
            return 0;
        }

        public string GetTooltipExtra(Farmer who)
        {
            return string.Empty;
        }

        public bool HasRecipe(Farmer who)
        {
            return _baseModel.Recipe.HasRequirements(who);
        }
    }

    internal class RecipeProvider : IRecipeProvider
    {
        private IBetterCraftingApi _api;

        public RecipeProvider(IBetterCraftingApi api)
        {
            _api = api;
        }

        public int RecipePriority => int.MaxValue;

        public bool CacheAdditionalRecipes => false;

        public IEnumerable<IRecipe> GetAdditionalRecipes(bool cooking)
        {
            return null;
        }

        public IRecipe GetRecipe(CraftingRecipe recipe)
        {
            var baseModel = Archery.modelManager.GetSpecificModel<BaseModel>(recipe.name);
            if (baseModel is null)
            {
                return null;
            }

            List<IIngredient> ingredients = new List<IIngredient>();
            foreach (var ingredient in baseModel.Recipe.Ingredients)
            {
                var ingredientId = ingredient.GetObjectId();
                if (ingredientId is null)
                {
                    continue;
                }

                ingredients.Add(_api.CreateBaseIngredient(ingredientId.Value, ingredient.Amount));
            }

            return new Recipe(baseModel, ingredients);
        }
    }
}
