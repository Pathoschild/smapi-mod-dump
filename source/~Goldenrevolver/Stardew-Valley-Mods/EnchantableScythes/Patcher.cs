/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace EnchantableScythesConfig
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;
    using StardewValley.Constants;
    using StardewValley.Enchantments;
    using StardewValley.Extensions;
    using StardewValley.Tools;
    using System;
    using System.Collections.Generic;

    public class Patcher
    {
        private static EnchantableScythes mod;

        // TODO check what iridium scythe does on release

        public static void PatchAll(EnchantableScythes scytheFixes)
        {
            mod = scytheFixes;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.fixProblems)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(RespawnGoldenScythes)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.drawTooltip)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(AddEnchantmentTooltipToScythe)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(BaseWeaponEnchantment), nameof(BaseWeaponEnchantment.CanApplyTo)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(CanApplyEnchantmentToScythe)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(BaseEnchantment), nameof(BaseEnchantment.GetEnchantmentFromItem)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(GetEnchantmentFromItem_Post)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(BaseEnchantment), nameof(BaseEnchantment.GetAvailableEnchantmentsForItem)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(AddHaymakerOnlyWorkaround)));

                harmony.Patch(
                    original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.Forge)),
                    postfix: new HarmonyMethod(typeof(Patcher), nameof(Forge_Post)));
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        public static void AddHaymakerOnlyWorkaround(Tool item, ref List<BaseEnchantment> __result)
        {
            if (item is MeleeWeapon meleeWeapon && meleeWeapon.isScythe() && mod.Config.ScythesCanOnlyGetHaymaker && __result.Count == 0)
            {
                __result = new List<BaseEnchantment>();

                List<BaseEnchantment> enchantments = BaseEnchantment.GetAvailableEnchantments();

                foreach (BaseEnchantment enchantment2 in enchantments)
                {
                    if (enchantment2.CanApplyTo(item))
                    {
                        __result.Add(enchantment2);
                    }
                }
            }
        }

        public static void AddEnchantmentTooltipToScythe(MeleeWeapon __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha)
        {
            if (!mod.Config.EnchantableScythes || !__instance.isScythe())
            {
                return;
            }

            foreach (BaseEnchantment enchantment in __instance.enchantments)
            {
                if (enchantment.ShouldBeDisplayed())
                {
                    Utility.drawWithShadow(spriteBatch, Game1.mouseCursors2, new Vector2((float)(x + 16 + 4), (float)(y + 16 + 4)), new Rectangle(127, 35, 10, 10), Color.White, 0f, Vector2.Zero, 4f, false, 1f, -1, -1, 0.35f);
                    Utility.drawTextWithShadow(spriteBatch, BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(), font, new Vector2((float)(x + 16 + 52), (float)(y + 16 + 12)), new Color(120, 0, 210) * 0.9f * alpha, 1f, -1f, -1, -1, 1f, 3);
                    y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
                }
            }
        }

        public static void Forge_Post(MeleeWeapon __instance, Item item, bool count_towards_stats, ref bool __result)
        {
            if (!mod.Config.EnchantableScythes || !__instance.isScythe())
            {
                return;
            }

            if (item is MeleeWeapon other_weapon && other_weapon.type == __instance.type)
            {
                __instance.appearance.Value = other_weapon.QualifiedItemId;
                __result = true;
                return;
            }

            BaseEnchantment enchantment = BaseEnchantment.GetEnchantmentFromItem(__instance, item);
            if (enchantment != null && __instance.AddEnchantment(enchantment))
            {
                // deleted diamond case

                if (count_towards_stats && !enchantment.IsForge())
                {
                    __instance.previousEnchantments.Insert(0, enchantment.GetName());
                    while (__instance.previousEnchantments.Count > 2)
                    {
                        __instance.previousEnchantments.RemoveAt(__instance.previousEnchantments.Count - 1);
                    }
                    Game1.stats.Increment(StatKeys.TimesEnchanted, 1);
                }

                __result = true;
                return;
            }

            __result = false;
        }

        public static void GetEnchantmentFromItem_Post(Item base_item, Item item, ref BaseEnchantment __result)
        {
            if (!mod.Config.EnchantableScythes)
            {
                return;
            }

            if (base_item is not MeleeWeapon meleeWeapon || !meleeWeapon.isScythe())
            {
                return;
            }

            switch (item?.QualifiedItemId)
            {
                // actual enchantments
                case "(O)74":
                    __result = Utility.CreateRandom(Game1.stats.Get(StatKeys.TimesEnchanted), Game1.uniqueIDForThisGame, 0.0, 0.0, 0.0).ChooseFrom(BaseEnchantment.GetAvailableEnchantmentsForItem(base_item as Tool));
                    return;

                // weapon forging
                case "(O)60":
                    __result = new EmeraldEnchantment();
                    return;

                case "(O)62":
                    __result = new AquamarineEnchantment();
                    return;

                case "(O)64":
                    __result = new RubyEnchantment();
                    return;

                case "(O)66":
                    __result = new AmethystEnchantment();
                    return;

                case "(O)68":
                    __result = new TopazEnchantment();
                    return;

                case "(O)70":
                    __result = new JadeEnchantment();
                    return;

                // deleted diamond case
                default:
                    __result = null;
                    return;
            }
        }

        public static void CanApplyEnchantmentToScythe(BaseWeaponEnchantment __instance, Item item, ref bool __result)
        {
            if (!mod.Config.EnchantableScythes)
            {
                return;
            }

            if (item is MeleeWeapon meleeWeapon)
            {
                if (meleeWeapon.isScythe())
                {
                    if (__instance is not DiamondEnchantment && __instance is not ArtfulEnchantment)
                    {
                        if (!mod.Config.ScythesCanOnlyGetHaymaker || __instance is HaymakerEnchantment || __instance.IsForge())
                        {
                            __result = true;
                        }
                    }
                }
                else
                {
                    if (mod.Config.OtherWeaponsCannotGetHaymakerAnymore && __instance is HaymakerEnchantment)
                    {
                        __result = false;
                    }
                }
            }
        }

        public static void RespawnGoldenScythes()
        {
            if (!mod.Config.GoldenScytheRespawns)
            {
                return;
            }

            var farmers = Game1.getAllFarmers();

            int missingGoldenScythes = 0;
            foreach (Farmer who in farmers)
            {
                if (who.mailReceived.Contains("gotGoldenScythe"))
                {
                    missingGoldenScythes++;
                }
            }

            foreach (Farmer who in farmers)
            {
                foreach (var item in who.Items)
                {
                    if (item is MeleeWeapon meleeWeapon && meleeWeapon.QualifiedItemId == "(W)" + MeleeWeapon.goldenScytheId)
                    {
                        missingGoldenScythes--;
                    }
                }
            }

            if (missingGoldenScythes > 0)
            {
                Utility.iterateChestsAndStorage(delegate (Item item)
                {
                    if (item is MeleeWeapon meleeWeapon && meleeWeapon.QualifiedItemId == "(W)" + MeleeWeapon.goldenScytheId)
                    {
                        missingGoldenScythes--;
                    }
                });

                foreach (Farmer who in farmers)
                {
                    if (missingGoldenScythes > 0 && who.mailReceived.Contains("gotGoldenScythe"))
                    {
                        who.mailReceived.Remove("gotGoldenScythe");
                        missingGoldenScythes--;
                    }
                }
            }
        }
    }
}