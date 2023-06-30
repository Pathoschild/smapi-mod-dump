/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces;
using Archery.Framework.Interfaces.Internal;
using Archery.Framework.Models.Crafting;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Utilities.Enchantments;
using Archery.Framework.Utilities.SpecialAttacks;
using Leclair.Stardew.BetterCrafting;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;

namespace Archery.Framework.Managers
{
    internal class ApiManager
    {
        private IMonitor _monitor;
        private IFashionSenseApi _fashionSenseApi;
        private IDynamicGameAssetsApi dynamicGameAssetsApi;
        private IJsonAssetsApi jsonAssetsApi;
        private IBetterCraftingApi betterCraftingApi;

        public ApiManager(IMonitor monitor)
        {
            _monitor = monitor;
        }

        internal bool IsFashionSenseLoaded()
        {
            return _fashionSenseApi is not null;
        }

        internal bool IsFashionSenseDrawOverrideActive()
        {
            return IsFashionSenseLoaded() is true && _fashionSenseApi.IsDrawOverrideActive(Interfaces.IFashionSenseApi.Type.Sleeves, Archery.manifest).Key is true;
        }

        internal bool HookIntoFashionSense(IModHelper helper)
        {
            _fashionSenseApi = helper.ModRegistry.GetApi<IFashionSenseApi>("PeacefulEnd.FashionSense");

            if (_fashionSenseApi is null)
            {
                _monitor.Log("Failed to hook into PeacefulEnd.FashionSense.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into PeacefulEnd.FashionSense.", LogLevel.Debug);
            return true;
        }

        public IFashionSenseApi GetFashionSenseApi()
        {
            return _fashionSenseApi;
        }

        internal bool HookIntoDynamicGameAssets(IModHelper helper)
        {
            dynamicGameAssetsApi = helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

            if (dynamicGameAssetsApi is null)
            {
                _monitor.Log("Failed to hook into spacechase0.DynamicGameAssets.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into spacechase0.DynamicGameAssets.", LogLevel.Debug);
            return true;
        }

        public IDynamicGameAssetsApi GetDynamicGameAssetsApi()
        {
            return dynamicGameAssetsApi;
        }

        internal bool HookIntoBetterCrafting(IModHelper helper)
        {
            betterCraftingApi = helper.ModRegistry.GetApi<IBetterCraftingApi>("leclair.bettercrafting");

            if (betterCraftingApi is null)
            {
                _monitor.Log("Failed to hook into leclair.bettercrafting.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into leclair.bettercrafting.", LogLevel.Debug);
            return true;
        }

        public IBetterCraftingApi GetBetterCraftingApi()
        {
            return betterCraftingApi;
        }

        internal bool HookIntoJsonAssets(IModHelper helper)
        {
            jsonAssetsApi = helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");

            if (jsonAssetsApi is null)
            {
                _monitor.Log("Failed to hook into spacechase0.JsonAssets.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into spacechase0.JsonAssets.", LogLevel.Debug);
            return true;
        }

        public IJsonAssetsApi GetJsonAssetsApi()
        {
            return jsonAssetsApi;
        }

        public void SyncRecipesWithBetterCrafting()
        {
            // Create and add the recipe provider
            betterCraftingApi.AddRecipeProvider(new RecipeProvider(betterCraftingApi));

            // Get the available models IDs with valid recipes
            var validModelIds = Archery.modelManager.GetModelsWithValidRecipes().Select(m => m.Id);

            // Get a random bow ID that is unlocked
            var archeryIcon = String.Empty;

            var availableWeapons = Archery.modelManager.GetModelsWithValidRecipes().Where(m => m is WeaponModel && m.Recipe.HasRequirements(Game1.player)).ToList();
            if (availableWeapons.Count > 0)
            {
                archeryIcon = availableWeapons[Game1.random.Next(availableWeapons.Count)].Id;
            }

            // Create the Archery category
            betterCraftingApi.CreateDefaultCategory(false, "PeacefulEnd.Archery", () => "Archery", iconRecipe: archeryIcon);

            // Add to the Archery category
            betterCraftingApi.AddRecipesToDefaultCategory(false, "PeacefulEnd.Archery", validModelIds);
        }

        public void RegisterNativeSpecialAttacks()
        {
            Archery.internalApi.RegisterSpecialAttack(Archery.manifest, "Snapshot", WeaponType.Any, (arguments) => "Snapshot", (arguments) => "Fires two arrows in quick succession.", (arguments) => 3000, Snapshot.HandleSpecialAttack);
            Archery.internalApi.RegisterSpecialAttack(Archery.manifest, "Snipe", WeaponType.Any, (arguments) => "Snipe", Snipe.GetDescription, Snipe.GetCooldown, Snipe.HandleSpecialAttack);
        }

        public void RegisterNativeEnchantments()
        {
            Archery.internalApi.RegisterEnchantment(Archery.manifest, "Seeker", AmmoType.Any, TriggerType.OnFire, (arguments) => "Seeker", (arguments) => "Faintly moves toward enemies.", Seeker.HandleEnchantment);
            Archery.internalApi.RegisterEnchantment(Archery.manifest, "Vampiric", AmmoType.Any, TriggerType.OnImpact, (arguments) => "Vampiric", Vampiric.GetDescription, Vampiric.HandleEnchantment);
            Archery.internalApi.RegisterEnchantment(Archery.manifest, "Drain", AmmoType.Any, TriggerType.OnImpact, (arguments) => "Drain", Drain.GetDescription, Drain.HandleEnchantment);
            Archery.internalApi.RegisterEnchantment(Archery.manifest, "Shock", AmmoType.Any, TriggerType.OnImpact, (arguments) => "Shock", Shock.GetDescription, Shock.HandleEnchantment);
        }
    }
}