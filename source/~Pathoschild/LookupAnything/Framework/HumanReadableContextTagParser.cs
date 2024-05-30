/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Pathoschild.Stardew.Common;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using Object = StardewValley.Object;

namespace Pathoschild.Stardew.LookupAnything.Framework
{
    /// <summary>Parses context tags into human-readable representations where possible.</summary>
    internal static class HumanReadableContextTagParser
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get a human-readable representation of a context tag, if available.</summary>
        /// <param name="contextTag">The raw context tag to parse.</param>
        public static string Parse(string contextTag)
        {
            if (string.IsNullOrWhiteSpace(contextTag))
                return contextTag;

            // extract negation
            bool negate = contextTag.StartsWith('!');
            string actualTag = negate
                ? contextTag[1..]
                : contextTag;

            // parse
            if (
                HumanReadableContextTagParser.TryParseCategory(actualTag, out string? parsed)
                || HumanReadableContextTagParser.TryParseItemId(actualTag, out parsed)
                || HumanReadableContextTagParser.TryParsePreservedItemId(actualTag, out parsed)
                || HumanReadableContextTagParser.TryParseSpecial(actualTag, out parsed)
            )
            {
                return negate
                    ? I18n.ConditionOrContextTag_Negate(value: parsed)
                    : parsed;
            }

            return contextTag;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Parse a context tag which refers to a category, if applicable.</summary>
        /// <param name="tag">The context tag.</param>
        /// <param name="parsed">The human-readable form.</param>
        /// <returns>Returns whether it was parsed successfully.</returns>
        /// <remarks>This implementation is based on <see cref="ItemContextTagManager.GetBaseContextTags"/>.</remarks>
        private static bool TryParseCategory(string tag, [NotNullWhen(true)] out string? parsed)
        {
            int? category = tag switch
            {
                "category_artisan_goods" => Object.artisanGoodsCategory,
                "category_bait" => Object.baitCategory,
                "category_big_craftable" => Object.BigCraftableCategory,
                "category_boots" => Object.bootsCategory,
                "category_clothing" => Object.clothingCategory,
                "category_cooking" => Object.CookingCategory,
                "category_crafting" => Object.CraftingCategory,
                "category_egg" => Object.EggCategory,
                "category_equipment" => Object.equipmentCategory,
                "category_fertilizer" => Object.fertilizerCategory,
                "category_fish" => Object.FishCategory,
                "category_flowers" => Object.flowersCategory,
                "category_fruits" => Object.FruitsCategory,
                "category_furniture" => Object.furnitureCategory,
                "category_gem" => Object.GemCategory,
                "category_greens" => Object.GreensCategory,
                "category_hat" => Object.hatCategory,
                "category_ingredients" => Object.ingredientsCategory,
                "category_junk" => Object.junkCategory,
                "category_litter" => Object.litterCategory,
                "category_meat" => Object.meatCategory,
                "category_milk" => Object.MilkCategory,
                "category_minerals" => Object.mineralsCategory,
                "category_monster_loot" => Object.monsterLootCategory,
                "category_ring" => Object.ringCategory,
                "category_seeds" => Object.SeedsCategory,
                "category_sell_at_fish_shop" => Object.sellAtFishShopCategory,
                "category_syrup" => Object.syrupCategory,
                "category_tackle" => Object.tackleCategory,
                "category_tool" => Object.toolCategory,
                "category_vegetable" => Object.VegetableCategory,
                "category_weapon" => Object.weaponCategory,
                "category_sell_at_pierres" => Object.sellAtPierres,
                "category_sell_at_pierres_and_marnies" => Object.sellAtPierresAndMarnies,
                "category_metal_resources" => Object.metalResources,
                "category_building_resources" => Object.buildingResources,
                "category_trinket" => Object.trinketCategory,
                _ => null
            };

            if (category.HasValue)
            {
                string displayName = Object.GetCategoryDisplayName(category.Value);
                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    parsed = displayName;
                    return true;
                }
            }

            parsed = null;
            return false;
        }

        /// <summary>Parse a context tag which refers to an item ID, if applicable.</summary>
        /// <param name="tag">The context tag.</param>
        /// <param name="parsed">The human-readable form.</param>
        /// <returns>Returns whether it was parsed successfully.</returns>
        private static bool TryParseItemId(string tag, [NotNullWhen(true)] out string? parsed)
        {
            if (MachineDataHelper.TryGetUniqueItemFromContextTag(tag, out ParsedItemData? data))
            {
                string? name = data.DisplayName;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    parsed = name;
                    return true;
                }
            }

            parsed = null;
            return false;
        }

        /// <summary>Parse a context tag which refers to a preserved item ID, if applicable.</summary>
        /// <param name="tag">The context tag.</param>
        /// <param name="parsed">The human-readable form.</param>
        /// <returns>Returns whether it was parsed successfully.</returns>
        private static bool TryParsePreservedItemId(string tag, [NotNullWhen(true)] out string? parsed)
        {
            if (tag.StartsWith("preserve_sheet_index_"))
            {
                ParsedItemData? data = ItemRegistry.GetData(tag[21..]);
                string? name = data?.DisplayName;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    parsed = I18n.ContextTag_PreservedItem(name);
                    return true;
                }
            }

            parsed = null;
            return false;
        }

        /// <summary>Parse a hardcoded context tag, if applicable.</summary>
        /// <param name="tag">The context tag.</param>
        /// <param name="parsed">The human-readable form.</param>
        /// <returns>Returns whether it was parsed successfully.</returns>
        private static bool TryParseSpecial(string tag, [NotNullWhen(true)] out string? parsed)
        {
            parsed = tag switch
            {
                "bone_item" => I18n.ContextTag_Bone(),
                "egg_item" => I18n.ContextTag_Egg(),
                "large_egg_item" => I18n.ContextTag_LargeEgg(),
                _ => null
            };
            return parsed != null;
        }
    }
}
