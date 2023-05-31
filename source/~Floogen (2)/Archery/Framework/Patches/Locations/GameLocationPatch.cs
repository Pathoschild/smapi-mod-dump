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
using Archery.Framework.Utilities;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Patches.Locations
{
    internal class GameLocationPatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(GameLocation);

        public GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.isActionableTile), new[] { typeof(int), typeof(int), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(IsActionableTilePostfix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.checkAction), new[] { typeof(xTile.Dimensions.Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(CheckActionPatchPostfix)));
        }

        private static void IsActionableTilePostfix(GameLocation __instance, ref bool __result, int xTile, int yTile, Farmer who)
        {
            if (__instance.Name == Toolkit.ARENA_MAP_NAME)
            {
                string actionProperty = __instance.doesTileHaveProperty(xTile, yTile, "Action", "Buildings");
                if (actionProperty == "ArcheryShop")
                {
                    __result = true;
                    return;
                }

                __result = false;
            }
        }

        private static void CheckActionPatchPostfix(GameLocation __instance, ref bool __result, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            if (__instance.Name == Toolkit.ARENA_MAP_NAME)
            {
                string actionProperty = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                if (actionProperty == "ArcheryShop")
                {
                    var shopMenu = new ShopMenu(new Dictionary<ISalable, int[]>());

                    // Add the weapons
                    AddToShopMenu(Archery.modelManager.GetAllModels().Where(m => m is WeaponModel).ToList(), shopMenu);

                    // Add the ammo
                    AddToShopMenu(Archery.modelManager.GetAllModels().Where(m => m is AmmoModel).ToList(), shopMenu);

                    // Add the recipes
                    foreach (var recipe in Archery.modelManager.GetRecipesForSale())
                    {
                        if (Game1.player.knowsRecipe(recipe.ParentId))
                        {
                            continue;
                        }

                        Item item;
                        switch (Archery.modelManager.GetSpecificModel<BaseModel>(recipe.ParentId))
                        {
                            case WeaponModel weaponModel:
                                item = Bow.CreateRecipe(weaponModel);
                                break;
                            case AmmoModel ammoModel:
                                item = Arrow.CreateRecipe(ammoModel);
                                break;
                            default:
                                continue;
                        }
                        item.Stack = 1;

                        shopMenu.forSale.Add(item);
                        shopMenu.itemPriceAndStock.Add(item, new int[2]
                        {
                            0,
                            1
                        });
                    }

                    Game1.activeClickableMenu = shopMenu;

                    __result = true;
                    return;
                }

                __result = false;
            }
        }

        private static void AddToShopMenu(List<BaseModel> models, ShopMenu shopMenu)
        {
            foreach (var model in models)
            {
                Item item;
                switch (model)
                {
                    case WeaponModel weaponModel:
                        item = Bow.CreateInstance(weaponModel);
                        break;
                    case AmmoModel ammoModel:
                        item = Arrow.CreateInstance(ammoModel);
                        break;
                    default:
                        continue;
                }
                item.Stack = 999;

                shopMenu.forSale.Add(item);
                shopMenu.itemPriceAndStock.Add(item, new int[2]
                {
                    0,
                    int.MaxValue
                });
            }
        }
    }
}