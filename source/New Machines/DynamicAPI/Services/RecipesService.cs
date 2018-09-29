using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class RecipesService
    {
        #region Singleton Access

        private RecipesService()
        {
            GameEvents.LoadContent += (s, e) => LoadToGame();
            PlayerEvents.LoadedGame += (s, e) => LoadToPlayer();
        }

        private static RecipesService _instance;

        public static RecipesService Instance => _instance ?? (_instance = new RecipesService());

        #endregion

        #region Private Data

        private readonly List<CraftingRecipeInformation> _craftingRecipeInformations = new List<CraftingRecipeInformation>();
        private readonly List<CookingRecipeInformation> _cookingRecipeInformations = new List<CookingRecipeInformation>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a custom crafting recipe.
        /// </summary>
        public void Register(CraftingRecipeInformation craftingRecipeInformation)
        {
            _craftingRecipeInformations.Add(craftingRecipeInformation);
        }

        /// <summary>
        /// Register a custom cooking recipe.
        /// </summary>
        public void Register(CookingRecipeInformation cookingRecipeInformation)
        {
            _cookingRecipeInformations.Add(cookingRecipeInformation);
        }

        #endregion

        #region	Auxiliary Methods

        private void LoadToGame()
        {
            LoadToGame(CraftingRecipe.craftingRecipes, _craftingRecipeInformations);
            LoadToGame(CraftingRecipe.cookingRecipes, _cookingRecipeInformations);
        }

        private static void LoadToGame(IDictionary<string, string> recipes, IEnumerable<IRecipeInformation> recipeInformations)
        {
            foreach (var recipeInformation in recipeInformations)
            {
                var key = recipeInformation.Name;
                var newValue = recipeInformation.ToString();
                string oldValue;
                if (!recipes.TryGetValue(key, out oldValue))
                {
                    recipes.Add(key, newValue);
                }
                else if (newValue != oldValue)
                {
                    Log.Fail($"Recipe for ID={key} already has another mapping {key}->{oldValue} (current:{newValue})");
                }
            }
        }

        private void LoadToPlayer()
        {
            var player = Game1.player;
            var playerSkills = new Dictionary<Skill, int>
            {
                {Skill.Undefined, -1 },
                {Skill.Combat, player.CombatLevel },
                {Skill.Farming, player.FarmingLevel },
                {Skill.Fishing, player.FishingLevel },
                {Skill.Foraging, player.ForagingLevel },
                {Skill.Luck, player.LuckLevel },
                {Skill.Mining, player.MiningLevel },
            };

            foreach (var recipeInformation in _craftingRecipeInformations)
            {
                if (player.craftingRecipes.ContainsKey(recipeInformation.Name)) continue;

                var wayToGet = recipeInformation.WayToGet;
                if (wayToGet.IsDefault || playerSkills[wayToGet.Skill] >= wayToGet.SkillLevel)
                {
                    player.craftingRecipes.Add(recipeInformation.Name, 0);
                }
            }

            foreach (var recipeInformation in _cookingRecipeInformations)
            {
                if (player.cookingRecipes.ContainsKey(recipeInformation.Name)) continue;

                var wayToGet = recipeInformation.WayToGet;
                if (!string.IsNullOrWhiteSpace(wayToGet.FriendshipWith))
                {
                    if (player.getFriendshipHeartLevelForNPC(wayToGet.FriendshipWith) >= 2 * (wayToGet.Hearts ?? 0.5m))
                    {
                        player.cookingRecipes.Add(recipeInformation.Name, 0);
                    }
                }
                else if (wayToGet.Skill != Skill.Undefined)
                {
                    if (playerSkills[wayToGet.Skill] >= wayToGet.SkillLevel)
                    {
                        player.craftingRecipes.Add(recipeInformation.Name, 0);
                    }
                }
                else
                {
                    var recipes = _cookingRecipeInformations.Select(i => new Object(i.ID, 1, true)).ToArray<Item>();
                    ShopService.Instance.AddShopInfo(new ShopInfo("Saloon", recipes));
                }
            }
        }

        #endregion
    }
}