/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Weapons;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Archery.Framework.Patches.Objects
{
    internal class CraftingRecipePatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(CraftingRecipe);

        public CraftingRecipePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            if (_helper.ModRegistry.IsLoaded("leclair.bettercrafting") is false)
            {
                _monitor.Log($"Applying CraftingRecipePatch...", LogLevel.Trace);

                harmony.Patch(AccessTools.Constructor(_object, new[] { typeof(string), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(CraftingRecipePostfix)));

                harmony.Patch(AccessTools.Method(_object, nameof(CraftingRecipe.drawMenuView), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawMenuViewPrefix)));
                harmony.Patch(AccessTools.Method(_object, nameof(CraftingRecipe.createItem), null), postfix: new HarmonyMethod(GetType(), nameof(CreateItemPostfix)));
            }
            else
            {
                _monitor.Log($"Skipped applying CraftingRecipePatch, due to Better Crafting being loaded!", LogLevel.Trace);
            }
        }

        private static void CraftingRecipePostfix(CraftingRecipe __instance, ref string ___DisplayName, ref string ___description, string name, bool isCookingRecipe)
        {
            var baseModel = Archery.modelManager.GetSpecificModel<BaseModel>(name);
            if (baseModel is null)
            {
                return;
            }

            ___DisplayName = baseModel.GetTranslation(baseModel.DisplayName);
            ___description = baseModel.GetTranslation(baseModel.Description);
        }

        private static bool DrawMenuViewPrefix(CraftingRecipe __instance, string ___name, SpriteBatch b, int x, int y, float layerDepth = 0.88f, bool shadow = true)
        {
            var baseModel = Archery.modelManager.GetSpecificModel<BaseModel>(___name);
            if (baseModel is null)
            {
                return true;
            }

            Utility.drawWithShadow(b, baseModel.Texture, new Vector2(x, y), baseModel.Icon.Source, Color.White, 0f, Vector2.Zero, 4f, flipped: false, layerDepth);
            return false;
        }

        private static void CreateItemPostfix(CraftingRecipe __instance, string ___name, ref Item __result)
        {
            var baseModel = Archery.modelManager.GetSpecificModel<BaseModel>(___name);
            if (baseModel is null || baseModel.Recipe is null || baseModel.Recipe.IsValid() is false)
            {
                return;
            }

            switch (baseModel)
            {
                case AmmoModel ammoModel:
                    __result = Arrow.CreateInstance(ammoModel, baseModel.Recipe.OutputAmount);
                    break;
                case WeaponModel weaponModel:
                    __result = Bow.CreateInstance(weaponModel);
                    break;
            }
        }
    }
}