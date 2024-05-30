/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/namelessto/EnchantedGalaxyWeapons
**
*************************************************/

using StardewModdingAPI;

namespace EnchantedGalaxyWeapons
{
    internal class ModMenu
    {
        public static void BuildMenu(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            mainPage(helper, manifest, configMenu);
            configMenu.AddPageLink(mod: manifest,
                pageId: "enchantments",
                () => helper.Translation.Get("menu.enchantments-title")
            );
            configMenu.AddPageLink(mod: manifest,
                pageId: "weapons",
                () => helper.Translation.Get("menu.weapons-title")
            );
            enchantmentsPage(helper, manifest, configMenu);
            weaponsPage(helper, manifest, configMenu);
        }

        private static void mainPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.main-title")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-daily-limit"),
                tooltip: () => helper.Translation.Get("menu.main-daily-limit-tooltip") +
                               helper.Translation.Get("menu.default", new { defaultValue = helper.Translation.Get("menu.true") }),
                getValue: () => ModEntry.Config.HaveDailySpawnLimit,
                setValue: value => ModEntry.Config.HaveDailySpawnLimit = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-daily-limit-amount"),
                tooltip: () => helper.Translation.Get("menu.main-daily-limit-amount-tooltip-p1") +
                               helper.Translation.Get("menu.main-daily-limit-amount-tooltip-p2") +
                               helper.Translation.Get("menu.default", new { defaultValue = 5 }),
                getValue: () => ModEntry.Config.DailySpawnLimit,
                setValue: value => ModEntry.Config.DailySpawnLimit = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-global-chance"),
                tooltip: () => helper.Translation.Get("menu.main-global-chance-tooltip-p1") +
                               helper.Translation.Get("menu.main-global-chance-tooltip-p2") +
                               helper.Translation.Get("menu.default", new { defaultValue = helper.Translation.Get("menu.false") }),
                getValue: () => ModEntry.Config.HaveGlobalChance, setValue: value => ModEntry.Config.HaveGlobalChance = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-base-chance"),
                tooltip: () => helper.Translation.Get("menu.default", new { defaultValue = "60%" }),
                getValue: () => ModEntry.Config.BaseSpawnChance,
                setValue: value => ModEntry.Config.BaseSpawnChance = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.05f
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-increment-chance"),
                tooltip: () => helper.Translation.Get("menu.main-increment-chance-tooltip-p1") +
                               helper.Translation.Get("menu.main-increment-chance-tooltip-p2") +
                               helper.Translation.Get("menu.default", new { defaultValue = "5%" }),
                getValue: () => ModEntry.Config.IncreaseSpawnChanceStep,
                setValue: value => ModEntry.Config.IncreaseSpawnChanceStep = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.05f
            );

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.main-dangerous-chance")
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-additional-attempts"),
                tooltip: () => helper.Translation.Get("menu.main-additional-attempts-tooltip-p1") +
                               helper.Translation.Get("menu.main-additional-attempts-tooltip-p2") +
                               helper.Translation.Get("menu.default", new { defaultValue = 0 }),
                getValue: () => ModEntry.Config.AdditionalTriesToSpawn,
                setValue: value => ModEntry.Config.AdditionalTriesToSpawn = value,
                min: 0,
                max: 50
            );


            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-more-barrels"),
                tooltip: () => helper.Translation.Get("menu.main-more-barrels-tooltip-p1") +
                               helper.Translation.Get("menu.main-more-barrels-tooltip-p2") +
                               helper.Translation.Get("menu.default", new { defaultValue = helper.Translation.Get("menu.false") }),
                getValue: () => ModEntry.Config.AllowMoreThanOne,
                setValue: value => ModEntry.Config.AllowMoreThanOne = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.main-barrels-amount"),
                tooltip: () => helper.Translation.Get("menu.main-barrels-amount-tooltip") +
                               helper.Translation.Get("menu.default", new { defaultValue = 0 }),
                getValue: () => ModEntry.Config.AdditionalBarrels,
                setValue: value => ModEntry.Config.AdditionalBarrels = value,
                min: 0,
                max: 20
            );
        }

        private static void enchantmentsPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(mod: manifest, pageId: "enchantments", () => helper.Translation.Get("menu.enchantments"));

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.basic-options")
            );


            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-guarantee-regular"),
                tooltip: () => helper.Translation.Get("menu.enchantments-guarantee-regular-tooltip") +
                               helper.Translation.Get("menu.default", new { defaultValue = helper.Translation.Get("menu.false") }),
                getValue: () => ModEntry.Config.ForceHaveEnchantment,
                setValue: value => ModEntry.Config.ForceHaveEnchantment = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-guarantee-innate"),
                tooltip: () => helper.Translation.Get("menu.enchantments-guarantee-innate-tooltip") +
                               helper.Translation.Get("menu.default", new { defaultValue = helper.Translation.Get("menu.false") }),
                getValue: () => ModEntry.Config.ForceInnateEnchantment,
                setValue: value => ModEntry.Config.ForceInnateEnchantment = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-regular-chance"),
                tooltip: () => helper.Translation.Get("menu.enchantments-regular-chance-tooltip") +
                               helper.Translation.Get("menu.default", new { defaultValue = "15%" }),
                getValue: () => ModEntry.Config.ChanceForEnchantment,
                setValue: value => ModEntry.Config.ChanceForEnchantment = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.01f
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-innate-chance"),
                tooltip: () => helper.Translation.Get("menu.enchantments-regular-chance-tooltip") +
                               helper.Translation.Get("menu.default", new { defaultValue = "50%" }),
                getValue: () => ModEntry.Config.ChanceForInnate,
                setValue: value => ModEntry.Config.ChanceForInnate = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.01f
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.enchantments-regular-options")
            );

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.enchantments-select")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-artful"),
                getValue: () => ModEntry.Config.AllowArtful,
                setValue: value => ModEntry.Config.AllowArtful = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-bug-killer"),
                getValue: () => ModEntry.Config.AllowBugKiller,
                setValue: value => ModEntry.Config.AllowBugKiller = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-crusader"),
                getValue: () => ModEntry.Config.AllowCrusader,
                setValue: value => ModEntry.Config.AllowCrusader = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-haymaker"),
                getValue: () => ModEntry.Config.AllowHaymaker,
                setValue: value => ModEntry.Config.AllowHaymaker = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-vampiric"),
                getValue: () => ModEntry.Config.AllowVampiric,
                setValue: value => ModEntry.Config.AllowVampiric = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.enchantments-innate-options")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-innate-keep"),
                tooltip: () => helper.Translation.Get("menu.enchantments-innate-keep-tooltip"),
                getValue: () => ModEntry.Config.KeepVanilla,
                setValue: value => ModEntry.Config.KeepVanilla = value
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-min-innate"),
                getValue: () => ModEntry.Config.MinInnateEnchantments,
                setValue: value => ModEntry.Config.MinInnateEnchantments = value,
                min: 0,
                max: 8
            );

            configMenu.AddNumberOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-max-innate"),
                getValue: () => ModEntry.Config.MaxInnateEnchantments,
                setValue: value => ModEntry.Config.MaxInnateEnchantments = value,
                min: 0,
                max: 8
            );

            configMenu.AddParagraph(
                mod: manifest,
                text: () => helper.Translation.Get("menu.enchantments-innate-limit-p1") +
                            helper.Translation.Get("menu.enchantments-innate-limit-p2")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-slime-gatherer"),
                getValue: () => ModEntry.Config.AllowSlimeGatherer,
                setValue: value => ModEntry.Config.AllowSlimeGatherer = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-slime-slayer"),
                getValue: () => ModEntry.Config.AllowSlimeSlayer,
                setValue: value => ModEntry.Config.AllowSlimeSlayer = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-attack"),
                getValue: () => ModEntry.Config.AllowAttack,
                setValue: value => ModEntry.Config.AllowAttack = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-speed"),
                getValue: () => ModEntry.Config.AllowSpeed,
                setValue: value => ModEntry.Config.AllowSpeed = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-defense"),
                getValue: () => ModEntry.Config.AllowDefense,
                setValue: value => ModEntry.Config.AllowDefense = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-weight"),
                getValue: () => ModEntry.Config.AllowWeight,
                setValue: value => ModEntry.Config.AllowWeight = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-crit-chance"),
                getValue: () => ModEntry.Config.AllowCritChance,
                setValue: value => ModEntry.Config.AllowCritChance = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.enchantments-crit-power"),
                getValue: () => ModEntry.Config.AllowCritPow,
                setValue: value => ModEntry.Config.AllowCritPow = value
            );
        }

        private static void weaponsPage(IModHelper helper, IManifest manifest, IGenericModConfigMenuApi configMenu)
        {
            configMenu.AddPage(
                mod: manifest,
                pageId: "weapons",
                () => helper.Translation.Get("menu.weapons")
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.basic-options")
            );

            configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => helper.Translation.Get("menu.weapons-skip-galaxy"),
                    tooltip: () => helper.Translation.Get("menu.weapons-skip-galaxy-tooltip") +
                                   helper.Translation.Get("menu.default", new { defaultValue = helper.Translation.Get("menu.false") }),
                    getValue: () => ModEntry.Config.SkipGalaxyCheck, setValue: value => ModEntry.Config.SkipGalaxyCheck = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.weapons-skip-infinity"),
                tooltip: () => helper.Translation.Get("menu.weapons-skip-infinity-tooltip") +
                                   helper.Translation.Get("menu.default", new { defaultValue = helper.Translation.Get("menu.false") }),
                getValue: () => ModEntry.Config.SkipInfinityCheck, setValue: value => ModEntry.Config.SkipInfinityCheck = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => helper.Translation.Get("menu.weapons-allow")
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.weapons-galaxy-sword"),
                getValue: () => ModEntry.Config.AllowGalSword,
                setValue: value => ModEntry.Config.AllowGalSword = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.weapons-galaxy-dagger"),
                getValue: () => ModEntry.Config.AllowGalDagger,
                setValue: value => ModEntry.Config.AllowGalDagger = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.weapons-galaxy-hammer"),
                getValue: () => ModEntry.Config.AllowGalHammer,
                setValue: value => ModEntry.Config.AllowGalHammer = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.weapons-infinity-sword"),
                getValue: () => ModEntry.Config.AllowInfSword,
                setValue: value => ModEntry.Config.AllowInfSword = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.weapons-infinity-dagger"),
                getValue: () => ModEntry.Config.AllowInfDagger,
                setValue: value => ModEntry.Config.AllowInfDagger = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => helper.Translation.Get("menu.weapons-infinity-hammer"),
                getValue: () => ModEntry.Config.AllowInfHammer,
                setValue: value => ModEntry.Config.AllowInfHammer = value
            );
        }
    }
}
