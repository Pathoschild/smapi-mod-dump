/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace EnchantedAdventurersGuildRewards
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Buffs;
    using StardewValley.Enchantments;
    using StardewValley.Objects;
    using StardewValley.Projectiles;
    using StardewValley.Tools;
    using System;
    using static EnchantedAdventurersGuildRewards;

    internal class Patcher
    {
        private static EnchantedAdventurersGuildRewards mod;

        public static string TemplarsBladeName => Game1.content.LoadString("Strings\\Weapons:TemplarsBlade_Name");
        public static string HolyBladeName => Game1.content.LoadString("Strings\\Weapons:HolyBlade_Name");
        public static string DarkSwordName => Game1.content.LoadString("Strings\\Weapons:DarkSword_Name");

        public static void PatchAll(EnchantedAdventurersGuildRewards mod)
        {
            Patcher.mod = mod;

            var harmony = new Harmony(Patcher.mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Item), nameof(Item.AddEquipmentEffects)),
               postfix: new HarmonyMethod(typeof(Patcher), nameof(AddHatDefense)));

            harmony.Patch(
               original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.animateSpecialMove)),
               prefix: new HarmonyMethod(typeof(Patcher), nameof(ThrowBone)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.fixProblems)),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(FixWeaponStats)));

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "onMonsterKilled"),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(CheckForSwordConversion)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Item), "ValidateUnqualifiedItemId"),
               prefix: new HarmonyMethod(typeof(Patcher), nameof(SuppressUnqualifiedWarning)));
        }

        public static void CheckForSwordConversion(GameLocation __instance, Farmer who)//, Monster monster)
        {
            if (who == null || !who.IsLocalPlayer || __instance is SlimeHutch || !mod.Config.EnableTemplarsBladeCorruption)
            {
                return;
            }

            if (who.CurrentItem is not MeleeWeapon weapon || weapon.QualifiedItemId != $"(W){templarsBladeUQID}")
            {
                return;
            }

            bool isWearingVampRing = who.GetEffectsOfRingMultiplier(vampireRingQUID) > 0;
            bool isWearingArcaneHat = who.hat.Value?.QualifiedItemId == $"(H){arcaneHatQUID}";

            int darkCorruption = GetSwordCorruption(weapon, darkSwordCorruptionKey);
            int holyCorruption = GetSwordCorruption(weapon, holyBladeCorruptionKey);

            if (isWearingVampRing)
            {
                darkCorruption++;
                IncrementSwordCorruption(weapon, darkSwordCorruptionKey);

                if (darkCorruption >= mod.Config.DarkSwordCorruptionKills)
                {
                    CorruptIntoDarkSword(who, weapon);
                    return;
                }

                if ((!isWearingArcaneHat || darkCorruption > holyCorruption) && (darkCorruption % 5) == 1)
                {
                    var desc = mod.Helper.Translation.Get("TemplarsBladeVampiricCorruptionProgress", new { templarsBladeName = TemplarsBladeName });

                    ShowCorruptionProgressInfo(desc);
                }
            }

            if (isWearingArcaneHat)
            {
                holyCorruption++;
                IncrementSwordCorruption(weapon, holyBladeCorruptionKey);

                if (holyCorruption >= mod.Config.HolyBladeCorruptionKills)
                {
                    CorruptIntoHolyBlade(who, weapon);
                    return;
                }

                if ((!isWearingVampRing || holyCorruption > darkCorruption) && (holyCorruption % 5) == 1)
                {
                    var desc = mod.Helper.Translation.Get("TemplarsBladeHolyCorruptionProgress", new { templarsBladeName = TemplarsBladeName });

                    ShowCorruptionProgressInfo(desc);
                }
            }

            if (isWearingVampRing && isWearingArcaneHat && holyCorruption == darkCorruption && (holyCorruption % 5) == 1)
            {
                var desc = mod.Helper.Translation.Get("TemplarsBladeHolyAndVampiricCorruptionProgress", new { templarsBladeName = TemplarsBladeName });

                ShowCorruptionProgressInfo(desc);
            }
        }

        public static void ShowCorruptionProgressInfo(string info)
        {
            if (Context.IsWorldReady && mod.Config.ShowCorruptionProgressMessages) // && Game1.activeClickableMenu == null)
            {
                Game1.showGlobalMessage(info);
            }
        }

        public static void CorruptIntoDarkSword(Farmer who, MeleeWeapon weapon)
        {
            var desc = mod.Helper.Translation.Get("TemplarsBladeCorruptionTransform",
                new { templarsBladeName = TemplarsBladeName, transformSwordName = DarkSwordName });

            CorruptWeapon(who, weapon, darkSwordUQID, new VampiricEnchantment(), desc);
        }

        public static void CorruptIntoHolyBlade(Farmer who, MeleeWeapon weapon)
        {
            var desc = mod.Helper.Translation.Get("TemplarsBladeCorruptionTransform",
                new { templarsBladeName = TemplarsBladeName, transformSwordName = HolyBladeName });

            CorruptWeapon(who, weapon, holyBladeUQID, new CrusaderEnchantment(), desc);
        }

        public static void CorruptWeapon(Farmer who, MeleeWeapon weapon, string corruptedWeaponUQID, BaseWeaponEnchantment enchantment, string transformationMessage)
        {
            who.removeItemFromInventory(weapon);
            var sword = ItemRegistry.Create($"(W){corruptedWeaponUQID}") as MeleeWeapon;
            sword.AddEnchantment(enchantment);
            who.addItemByMenuIfNecessaryElseHoldUp(sword);

            if (Context.IsWorldReady && mod.Config.ShowCorruptionFinishMessage) // && Game1.activeClickableMenu == null)
            {
                Game1.showGlobalMessage(transformationMessage);
            }
        }

        public static int GetSwordCorruption(MeleeWeapon weapon, string key)
        {
            if (!weapon.modData.ContainsKey(key))
            {
                return 0;
            }
            else
            {
                return weapon.modData[key].Length;
            }
        }

        public static void IncrementSwordCorruption(MeleeWeapon weapon, string key)
        {
            if (!weapon.modData.ContainsKey(key))
            {
                weapon.modData[key] = "|";
            }
            else
            {
                weapon.modData[key] += "|";
            }
        }

        public static void FixWeaponStats()
        {
            Utility.ForEachItem(delegate (Item item)
            {
                if (item is not MeleeWeapon weapon)
                {
                    return true;
                }

                if (weapon.QualifiedItemId == $"(W){insectHeadUQID}")
                {
                    weapon.minDamage.Value = mod.Config.RevertInsectHeadBuff ? 10 : 20;
                    weapon.maxDamage.Value = mod.Config.RevertInsectHeadBuff ? 20 : 30;
                }

                MaybeAddMissingEnchantments(mod.Config, weapon);

                return true;
            });
        }

        public static void ThrowBone(MeleeWeapon __instance, Farmer who)
        {
            if (!mod.Config.SkeletonMaskBoneThrowSynergy)
            {
                return;
            }

            if (__instance.QualifiedItemId != $"(W){boneSwordUQID}" || Game1.fadeToBlack || MeleeWeapon.defenseCooldown > 0)
            {
                return;
            }

            if (who.hat.Value?.QualifiedItemId != $"(H){skeletonMaskUQID}")
            {
                return;
            }

            var facingVector = Vector2.Zero;
            float facingBasedYOffset = 0f;

            switch (who.FacingDirection)
            {
                case Game1.left:
                    facingVector = new Vector2(-1, 0);
                    facingBasedYOffset = -20f;
                    break;

                case Game1.right:
                    facingVector = new Vector2(1, 0);
                    facingBasedYOffset = -20f;
                    break;

                case Game1.up:
                    facingVector = new Vector2(0, -1);
                    facingBasedYOffset = -36f;
                    break;

                case Game1.down:
                    facingVector = new Vector2(0, 1);
                    facingBasedYOffset = -36f;
                    break;
            }

            Vector2 velocity = Utility.getVelocityTowardPoint(who.Tile, who.Tile + facingVector, 12);

            int baseDamage = Game1.random.Next(__instance.minDamage.Value, __instance.maxDamage.Value + 1);
            int damage = (int)Math.Ceiling((float)baseDamage * Math.Clamp(mod.Config.BoneProjectileDamageMultiplier, 0f, 10f));

            who.currentLocation.projectiles.Add(new BasicProjectile(damage, 4, 0, 0, (float)Math.PI / 16f, velocity.X, velocity.Y, new Vector2(who.Position.X, who.Position.Y + facingBasedYOffset), "skeletonHit", "skeletonStep", null, explode: false, damagesMonsters: true, who.currentLocation, who));
        }

        public static void AddHatDefense(Item __instance, BuffEffects effects)
        {
            if (!mod.Config.SomeHatsGrantDefense)
            {
                return;
            }

            if (__instance is not Hat hat)
            {
                return;
            }

            switch (hat.QualifiedItemId)
            {
                case $"(H){hardHatUQID}":
                case $"(H){squiresHelmUQID}":
                    if (effects.Defense.Value == 0f)
                    {
                        effects.Defense.Value += 1f;
                    }
                    break;

                case $"(H){knightsHelmUQID}":
                    if (effects.Defense.Value == 0f)
                    {
                        effects.Defense.Value += 2f;
                    }
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0057:Use range operator", Justification = "Should stay the same as orig method")]
        private static bool SuppressUnqualifiedWarning(Item __instance, string id, ref string __result)
        {
            if (!mod.Config.EnableDebugSwordRecipes)
            {
                return true;
            }

            if (!(id is $"(W){darkSwordUQID}" or $"(W){holyBladeUQID}"))
            {
                return true;
            }

            if (ItemRegistry.IsQualifiedItemId(id))
            {
                string qualifier = __instance.TypeDefinitionId;

                if (id.StartsWith(qualifier))
                {
                    id = id.Substring(qualifier.Length).TrimStart();
                }
            }

            __result = id;
            return false;
        }
    }
}