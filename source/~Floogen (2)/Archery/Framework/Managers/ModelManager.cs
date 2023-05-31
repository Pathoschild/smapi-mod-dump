/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using Archery.Framework.Models;
using Archery.Framework.Models.Crafting;
using Archery.Framework.Models.Weapons;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Managers
{
    internal class ModelManager
    {
        private IMonitor _monitor;
        private List<BaseModel> _contentPackModels;

        public ModelManager(IMonitor monitor)
        {
            _monitor = monitor;

            Reset();
        }

        public void Reset(string packId = null)
        {
            if (String.IsNullOrEmpty(packId) is true)
            {
                _contentPackModels = new List<BaseModel>();
            }
            else
            {
                _contentPackModels = _contentPackModels.Where(a => a.ContentPack.Manifest.UniqueID.Equals(packId, StringComparison.OrdinalIgnoreCase) is false).ToList();
            }
        }

        internal void AddModel(BaseModel baseModel)
        {
            _contentPackModels.Add(baseModel);
        }

        internal List<BaseModel> GetAllModels()
        {
            return _contentPackModels;
        }

        internal bool DoesModelExist<T>(string modelId) where T : BaseModel
        {
            return _contentPackModels.FirstOrDefault(t => String.Equals(t.Id, modelId, StringComparison.OrdinalIgnoreCase) && t is T) is not null;
        }

        internal T GetSpecificModel<T>(string modelId) where T : BaseModel
        {
            return (T)_contentPackModels.FirstOrDefault(t => String.Equals(t.Id, modelId, StringComparison.OrdinalIgnoreCase) && t is T);
        }

        internal List<BaseModel> GetModelsForSale()
        {
            return GetAllModels().Where(m => m.Shop is not null && m.Shop.IsValid()).ToList();
        }

        internal List<RecipeModel> GetRecipesForSale()
        {
            return GetValidRecipes().Where(m => m.Shop is not null && m.Shop.IsValid()).ToList();
        }

        internal List<BaseModel> GetModelsWithValidRecipes()
        {
            return GetAllModels().Where(m => m.Recipe is not null && m.Recipe.IsValid()).ToList();
        }

        internal List<RecipeModel> GetValidRecipes()
        {
            return GetModelsWithValidRecipes().Select(m => m.Recipe).ToList();
        }

        internal WeaponModel GetRandomWeaponModel(WeaponType type)
        {
            var typedModels = GetAllModels().Where(m => m is WeaponModel weaponModel && weaponModel.Type == type);
            if (typedModels.Count() == 0)
            {
                return null;
            }

            var randomModelIndex = Game1.random.Next(typedModels.Count());
            return (WeaponModel)typedModels.ToList()[randomModelIndex];
        }

        internal AmmoModel GetRandomAmmoModel(AmmoType type)
        {
            var typedModels = GetAllModels().Where(m => m is AmmoModel ammoModel && ammoModel.Type == type).ToList();
            if (typedModels.Count() == 0)
            {
                return null;
            }

            var randomModelIndex = Game1.random.Next(typedModels.Count());
            return (AmmoModel)typedModels[randomModelIndex];
        }
    }
}
