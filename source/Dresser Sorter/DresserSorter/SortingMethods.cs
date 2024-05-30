/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/DresserSorter
**
*************************************************/

using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Drawing;

namespace DresserSorter
{
    internal static class SortingMethods
    {
        internal static ModConfig config;
        internal static IMonitor monitor;
        internal static IModHelper modHelper;

        public static bool SortOverride(ref int __result, ref Item a, ref Item b)
        {
            try
            {
                if (a.Category != b.Category)
                    __result = a.Category.CompareTo(b.Category);
                // case of Clothing
                else if (a is Clothing c1 && b is Clothing c2)
                    __result = ClothCompare(c1, c2);
                // case of Boots
                else if (a is Boots b1 && b is Boots b2)
                    __result = BootsCompare(b1, b2);
                // case of Ring
                else if (a is Ring r1 && b is Ring r2)
                    __result = RingCompare(r1, r2);

                // Additional Categories
                if (__result == 0 && config.SortAdditionalCategories)
                {
                    // case of Tool
                    if (a is Tool t1 && b is Tool t2)
                        __result = ToolCompare(t1, t2);
                    // case of Object and Furniture
                    else if (a is StardewValley.Object o1 && b is StardewValley.Object o2)
                        __result = ObjectCompare(o1, o2);
                }

                // else case, sort by id or displayname
                if (__result == 0)
                    __result = IdAndDisplayNameCompare(a, b);
                return false;
            }
            catch (Exception e)
            {
                monitor.Log("Comparison of two items failed. Stack trace is below:", LogLevel.Error);
                monitor.Log(e.StackTrace ?? "Oh, No stack trace! I don't think I can fix this error. Sorry.", LogLevel.Error);
                return true;
            }
        }

        private static int ClothCompare(Clothing a, Clothing b)
        {
            // cloth type compare
            if (a.clothesType.Value != b.clothesType.Value)
                return a.clothesType.Value.CompareTo(b.clothesType.Value);
            // if cloth type is same, compare display name or id
            else
            {
                // id and display name compare
                int result = IdAndDisplayNameCompare(a, b);

                // if id is same too and use color sort, color compare
                if (result == 0 && config.Cloth_ColorSortByHue) // if two clothes have same status
                {
                    // xna color to System.Drawing.Color
                    a.clothesColor.Value.Deconstruct(out byte r1, out byte g1, out byte b1, out byte a1);
                    b.clothesColor.Value.Deconstruct(out byte r2, out byte g2, out byte b2, out byte a2);

                    System.Drawing.Color color1 = Color.FromArgb(a1, r1, g1, b1);
                    System.Drawing.Color color2 = Color.FromArgb(a2, r2, g2, b2);

                    result = color1.GetHue().CompareTo(color2.GetHue());
                }

                return result;
            }
        }

        private static int BootsCompare(Boots a, Boots b)
        {
            // id compare
            int result = a.ItemId.CompareTo(b.ItemId);

            // if id is same, compare with display name
            if (result == 0)
                result = a.DisplayName.CompareTo(b.DisplayName);

            // stat compare
            if (result == 0 && config.VeryTidySort_ByStats)
            {
                result = a.defenseBonus.Value.CompareTo(b.defenseBonus.Value) * -1; // higher defense is prior
                if (result == 0)
                    result = a.immunityBonus.Value.CompareTo(b.immunityBonus.Value) * -1; // higher defense is prior
            }
            return result;
        }

        private static int RingCompare(Ring a, Ring b)
        {
            // if Ring_CombinedRingIsPrior is true, combined rings are prior
            if (config.Ring_CombinedRingIsPrior)
            {
                if (a is CombinedRing && b is not CombinedRing)
                    return -1;
                else if (a is not CombinedRing && b is CombinedRing)
                    return 1;
            }

            Ring ringToCompareA = a;
            Ring ringToCompareB = b;
            NetList<Ring, NetRef<Ring>>? ringListA = null;
            NetList<Ring, NetRef<Ring>>? ringListB = null;

            // if a ring is combined ring, extract the array of combined rings
            if (a is CombinedRing combinedRingA)
            {
                ringListA = combinedRingA.combinedRings;
                ringToCompareA = ringListA[0];
            }
            if (b is CombinedRing combinedRingB)
            {
                ringListB = combinedRingB.combinedRings;
                ringToCompareB = ringListB[0];
            }

            // id and display name compare
            int result = IdAndDisplayNameCompare(ringToCompareA, ringToCompareB);

            // if ids are same and subrings are not null, compare subrings
            // why compare even if not using very tidy sort? because combined rings are drawed with both rings' appearnace
            if (result == 0 && (ringListA != null && ringListB != null))
            {
                for (var i = 1; i < ringListA.Count; i++)
                {
                    // if ring a is combined may than b, a is prior
                    if (ringListB.Count < i + 1)
                    {
                        result = -1;
                        break;
                    }

                    // id and display name compare
                    result = IdAndDisplayNameCompare(ringListA[i], ringListB[i]);

                    // if both rings are not same state, break loop
                    if (result != 0)
                        break;
                }
            }

            return result;
        }

        private static int ToolCompare(Tool a, Tool b)
        {
            /* not working
            // slingshot is later: right in front of melee weapon
            if (a is Slingshot && b is not Slingshot)
                return 1;
            */

            // if a and b are MeleeWeapon, call melee weapon compare method
            //else
            if (a is MeleeWeapon mw1 && b is MeleeWeapon mw2)
                return MeleeWeaponCompare(mw1, mw2);

            // tool type compare
            int result = a.GetType().ToString().CompareTo(b.GetType().ToString());
            // if type is same, compare upgrade level
            if (result == 0)
                result = a.UpgradeLevel.CompareTo(b.UpgradeLevel) * -1; // higher is prior
            // if upgrade level is same, compare display name
            if (result == 0)
                result = a.DisplayName.CompareTo(b.DisplayName);
            // if display name is same, compare item id
            if (result == 0)
                result = a.ItemId.CompareTo(b.ItemId);
            // if the id is same too and using very tidy sort, compare enchantment
            if (result == 0 && config.VeryTidySort_ByStats)
                result = EnchantmentsCompare(a, b);
            // if enchantments are same too, decide as same status; Because tools with attachments are not expected to be storable
            return result;
        }

        private static int MeleeWeaponCompare(MeleeWeapon a, MeleeWeapon b)
        {
            // compare weapon type
            int result = a.type.Value.CompareTo(b.type.Value);

            // if weapon type is same, compare drawn id before actual id or display name
            if (result == 0)
                result = a.GetDrawnItemId().CompareTo(b.GetDrawnItemId());
            // compare display name
            if (result == 0)
                result = a.DisplayName.CompareTo(b.DisplayName);
            // if display name is same, compare item id
            if (result == 0)
                result = a.ItemId.CompareTo(b.ItemId);

            // if id is same, compare level
            if (result == 0)
                result = a.getItemLevel().CompareTo(b.getItemLevel()) * -1;

            // very tidy sort: compare all stats
            if (result == 0 && config.VeryTidySort_ByStats)
            {
                // compare stats
                result = a.minDamage.Value.CompareTo(b.minDamage.Value) * -1;
                if (result == 0)
                    result = a.maxDamage.Value.CompareTo(b.maxDamage.Value) * -1;
                if (result == 0)
                    result = a.speed.Value.CompareTo(b.speed.Value) * -1;
                if (result == 0)
                    result = a.addedDefense.Value.CompareTo(a.addedDefense.Value) * -1;
                if (result == 0)
                    result = a.critChance.Value.CompareTo(b.critChance.Value) * -1;
                if (result == 0)
                    result = a.critMultiplier.Value.CompareTo(b.critMultiplier.Value) * -1;
                if (result == 0)
                    result = a.knockback.Value.CompareTo(b.CompareTo(b.knockback.Value)) * -1;
                // compare enchantments
                if (result == 0)
                    result = EnchantmentsCompare(a, b);

            }
            return result;
        }

        private static int ObjectCompare(StardewValley.Object a, StardewValley.Object b)
        {
            if (a is Furniture f1 && b is Furniture f2)
                return FurnitureCompare(f1, f2);
            else if (a is Trinket t1 && b is Trinket t2)
                return TrinketCompare(t1, t2);

            int result = 0;
            // compare display name, unless item's category is resource or need to sort resources
            if ((a.Category != StardewValley.Object.metalResources && a.Category != StardewValley.Object.buildingResources) || !config.Object_NotSortResourceByDisplayName)
                result = a.DisplayName.CompareTo(b.DisplayName);
            // compare id
            if (result == 0)
                result = a.ItemId.CompareTo(b.ItemId);
            // if both are same, compare quality
            if (result == 0)
                result = a.Quality.CompareTo(b.Quality) * -1; // better quality is prior
            // if quality is same, compare stacks
            if (result == 0)
                result = a.Stack.CompareTo(b.Stack) * -1; // high stack is prior

            // special cases
            if (result == 0)
            {
                // case of tackles
                if (a.Category == StardewValley.Object.tackleCategory) // and b is not tackle too
                    result = a.uses.Value.CompareTo(b.uses.Value);
                // both have parent item, but display name, quality, and stacks are same
                else if (a.preservedParentSheetIndex.Value != null && b.preservedParentSheetIndex.Value != null)
                    result = a.preservedParentSheetIndex.Value.CompareTo(b.preservedParentSheetIndex.Value);
            }
            return result;
        }

        private static int TrinketCompare(Trinket a, Trinket b)
        {
            // trinket type compare
            int result = a.GetTrinketData().TrinketEffectClass.CompareTo(b.GetTrinketData().TrinketEffectClass);
            // name and id compare
            if (result == 0)
            {
                result = a.DisplayName.CompareTo(b.DisplayName);
                if (result == 0)
                    result = a.ItemId.CompareTo(b.ItemId);
            }

            // very tidy sort: sort by trinket stats
            // separate by trinket effects
            if (result == 0 && config.VeryTidySort_ByStats)
            {
                TrinketEffect effectA = a.GetEffect();
                TrinketEffect effectB = b.GetEffect();

                // fairy: compare heal delay
                if (effectA is FairyBoxTrinketEffect fbt1 && effectB is FairyBoxTrinketEffect fbt2)
                {
                    float delay1 = modHelper.Reflection.GetField<float>(fbt1, "healDelay").GetValue();
                    float delay2 = modHelper.Reflection.GetField<float>(fbt2, "healDelay").GetValue();
                    result = delay1.CompareTo(delay2); // lower value is prior
                }
                // frog: comapre variant
                else if (effectA is CompanionTrinketEffect cte1 && effectB is CompanionTrinketEffect cte2)
                    result = cte1.variant.CompareTo(cte2.variant);
                // ice rod: comapre projectile delay then freeze time
                else if (effectA is IceOrbTrinketEffect iote1 && effectB is IceOrbTrinketEffect iote2)
                {
                    // compare delay
                    float delay1 = modHelper.Reflection.GetField<float>(iote1, "projectileDelayMS").GetValue();
                    float delay2 = modHelper.Reflection.GetField<float>(iote2, "projectileDelayMS").GetValue();
                    result = delay1.CompareTo(delay2); // lower value is prior

                    // compare freeze time
                    if (result == 0)
                    {
                        float freezeTime1 = modHelper.Reflection.GetField<float>(iote1, "freezeTime").GetValue();
                        float freezeTime2 = modHelper.Reflection.GetField<float>(iote2, "freezeTime").GetValue();
                        result = freezeTime1.CompareTo(freezeTime2) * -1; // higher value is prior
                    }
                }
                // magic quiver: compare delay, min dmg then max dmg
                else if (effectA is MagicQuiverTrinketEffect mqte1 && effectB is MagicQuiverTrinketEffect mqte2)
                {
                    // compare delay
                    float delay1 = modHelper.Reflection.GetField<float>(mqte1, "projectileDelayMS").GetValue();
                    float delay2 = modHelper.Reflection.GetField<float>(mqte2, "projectileDelayMS").GetValue();
                    result = delay1.CompareTo(delay2); // lower value is prior

                    // compare min damage
                    if (result == 0)
                    {
                        float freezeTime1 = modHelper.Reflection.GetField<float>(mqte1, "mindamage").GetValue();
                        float freezeTime2 = modHelper.Reflection.GetField<float>(mqte2, "mindamage").GetValue();
                        result = freezeTime1.CompareTo(freezeTime2) * -1; // higher value is prior
                    }

                    // compare max damage
                    if (result == 0)
                    {
                        float freezeTime1 = modHelper.Reflection.GetField<float>(mqte1, "maxdamage").GetValue();
                        float freezeTime2 = modHelper.Reflection.GetField<float>(mqte2, "maxdamage").GetValue();
                        result = freezeTime1.CompareTo(freezeTime2) * -1; // higher value is prior
                    }
                }
                // else case
                else
                    result = effectA.general_stat_1.CompareTo(effectB.general_stat_1) * -1; // // higher value is prior
            }
            return result;
        }

        private static int FurnitureCompare(Furniture a, Furniture b)
        {
            // compare type, if need to compare furniture type
            int result = 0;
            if (!config.Furniture_NotCompareFurnitureType)
                result = a.furniture_type.Value.CompareTo(b.furniture_type.Value);
            // id and display name compare
            if (result == 0)
                result = IdAndDisplayNameCompare(a, b);

            return result;
        }

        private static int EnchantmentsCompare(Tool a, Tool b)
        {
            int result = 0;
            for (var i = 0; i < a.enchantments.Count; i++)
            {
                // if a have more enchantment than b, a is prior
                if (b.enchantments.Count < i + 1)
                {
                    result = -1;
                    break;
                }

                BaseEnchantment eA = a.enchantments[i];
                BaseEnchantment eB = b.enchantments[i];

                // compare enchantment display name
                result = eA.GetDisplayName().CompareTo(eB.GetDisplayName());

                // if display name is same, compare level
                if (result == 0)
                    result = eA.Level.CompareTo(eB.Level) * -1; // high level is prior

                // if both enchants are not same state, break loop
                if (result != 0)
                    break;
            }
            return result;
        }

        private static int IdAndDisplayNameCompare(Item a, Item b)
        {
            // compare display name
            int result = a.DisplayName.CompareTo(b.DisplayName);

            // if display name is same, compare item id
            if (result == 0)
                result = a.ItemId.CompareTo(b.ItemId);

            return result;
        }
    }
}