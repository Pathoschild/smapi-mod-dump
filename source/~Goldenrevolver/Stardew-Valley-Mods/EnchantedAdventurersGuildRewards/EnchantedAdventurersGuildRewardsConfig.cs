/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace EnchantedAdventurersGuildRewards
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddParagraph(IManifest mod, Func<string> text);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }

    public class EnchantedAdventurersGuildRewardsConfig
    {
        public bool EnableTemplarsBladeCorruption { get; set; } = true;

        public int DarkSwordCorruptionKills { get; set; } = 10;

        public int HolyBladeCorruptionKills { get; set; } = 10;

        public bool ShowCorruptionProgressMessages { get; set; } = true;

        public bool ShowCorruptionFinishMessage { get; set; } = false;

        public bool SkeletonMaskBoneThrowSynergy { get; set; } = true;

        public float BoneProjectileDamageMultiplier { get; set; } = 1f;

        public bool SomeHatsGrantDefense { get; set; } = true;

        public bool EnableBugKillerInsectHead { get; set; } = true;

        public bool RevertInsectHeadBuff { get; set; } = false;

        public bool RenameEnglishDarkSwordToDarkBlade { get; set; } = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Would break existing references")]
        public bool EnableDebugSwordRecipes => false;

        public static void VerifyConfigValues(EnchantedAdventurersGuildRewardsConfig config, EnchantedAdventurersGuildRewards mod)
        {
            bool invalidConfig = false;

            if (config.DarkSwordCorruptionKills < 1)
            {
                config.DarkSwordCorruptionKills = 1;
                invalidConfig = true;
            }

            if (config.HolyBladeCorruptionKills < 1)
            {
                config.HolyBladeCorruptionKills = 1;
                invalidConfig = true;
            }

            if (config.BoneProjectileDamageMultiplier < 0f)
            {
                config.BoneProjectileDamageMultiplier = 0f;
                invalidConfig = true;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void InvalidateCache(EnchantedAdventurersGuildRewards mod)
        {
            try
            {
                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/CraftingRecipes");

                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Weapons");

                mod.Helper.GameContent.InvalidateCacheAndLocalized("Data/Hats");

                mod.Helper.GameContent.InvalidateCacheAndLocalized("Strings/Weapons");
            }
            catch (Exception e)
            {
                mod.DebugLog($"Exception when trying to invalidate cache on config change {e}");
            }
        }

        public static void SetUpModConfigMenu(EnchantedAdventurersGuildRewardsConfig config, EnchantedAdventurersGuildRewards mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: delegate
                {
                    config = new EnchantedAdventurersGuildRewardsConfig();
                    InvalidateCache(mod);
                    mod.AddOrDeleteRecipes();
                    Patcher.FixWeaponStats();
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                    InvalidateCache(mod);
                    mod.AddOrDeleteRecipes();
                    Patcher.FixWeaponStats();
                }
            );

            api.AddSectionTitle(manifest, () => "Templar's Blade Corruption");

            api.AddBoolOption(manifest, () => config.EnableTemplarsBladeCorruption, (bool val) => config.EnableTemplarsBladeCorruption = val,
                () => "Enable Corruption");
            api.AddNumberOption(manifest, () => config.DarkSwordCorruptionKills, (int val) => config.DarkSwordCorruptionKills = val,
                () => "Dark Sword Corruption Kills", null, 1);
            api.AddNumberOption(manifest, () => config.HolyBladeCorruptionKills, (int val) => config.HolyBladeCorruptionKills = val,
                () => "Holy Blade Corruption Kills", null, 1);
            api.AddBoolOption(manifest, () => config.ShowCorruptionProgressMessages, (bool val) => config.ShowCorruptionProgressMessages = val,
                () => "Show Progress Messages");
            api.AddBoolOption(manifest, () => config.ShowCorruptionFinishMessage, (bool val) => config.ShowCorruptionFinishMessage = val,
                () => "Show Finish Message");

            api.AddSectionTitle(manifest, () => "Bone Throwing");

            api.AddBoolOption(manifest, () => config.SkeletonMaskBoneThrowSynergy, (bool val) => config.SkeletonMaskBoneThrowSynergy = val,
                () => "Skeleton Mask Synergy");
            api.AddNumberOption(manifest, () => config.BoneProjectileDamageMultiplier, (float val) => config.BoneProjectileDamageMultiplier = val,
                () => "Projectile Damage Multiplier", null, 0f);

            api.AddSectionTitle(manifest, () => "Misc");

            api.AddBoolOption(manifest, () => config.SomeHatsGrantDefense, (bool val) => config.SomeHatsGrantDefense = val,
                () => "Some Hats Grant Defense", () => "Specifically, the hard hat, squire's helmet and knight's helmet");
            api.AddBoolOption(manifest, () => config.EnableBugKillerInsectHead, (bool val) => config.EnableBugKillerInsectHead = val,
                () => "Bug Killer Insect Head", () => "Existing enchantments do not get removed by disabling this");
            api.AddBoolOption(manifest, () => config.RevertInsectHeadBuff, (bool val) => config.RevertInsectHeadBuff = val,
                () => "Revert Insect Head Buff", () => "Option to revert the 1.6 base damage buff to the Insect Head (20-30 -> 10-20 damage)");
            api.AddBoolOption(manifest, () => config.RenameEnglishDarkSwordToDarkBlade, (bool val) => config.RenameEnglishDarkSwordToDarkBlade = val,
                () => "Rename English Dark Sword\nTo Dark Blade", () => "Renames the Dark Sword to Dark Blade when playing in English");
        }
    }
}