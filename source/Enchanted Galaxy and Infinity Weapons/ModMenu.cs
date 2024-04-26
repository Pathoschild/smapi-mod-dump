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
        private static IManifest manifest;
        private static IGenericModConfigMenuApi configMenu;
        public static void BuildMenu(IManifest manifestVar, IGenericModConfigMenuApi configMenuVar)
        {
            manifest = manifestVar;
            configMenu = configMenuVar;

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "Main Options"
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Daily Limit",
                tooltip: () => "Whether to limit the number of barrels that can spawn per day. Disable to ignore the limit.\nDefault: true",
                getValue: () => ModEntry.Config.HaveDailySpawnLimit,
                setValue: value => ModEntry.Config.HaveDailySpawnLimit = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Daily Spawn Limit",
                tooltip: () =>
                    "Maximum number of barrels that can spawn per day. Increase with luck.\n" +
                    "Decrease when breaking a barrel.\n" +
                    "Default: 5, ignored if the limit is disabled.",
                getValue: () => ModEntry.Config.DailySpawnLimit,
                setValue: value => ModEntry.Config.DailySpawnLimit = value
            );

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Allow Global Drop Chance",
                tooltip: () => "If enabled, the drop chance will be the same at any level in the mines/skull cavern.\nInstead of increasing chance at higher levels as it normally would.\nDefault: false",
                getValue: () => ModEntry.Config.HaveGlobalChance, setValue: value => ModEntry.Config.HaveGlobalChance = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Base Drop Chance",
                tooltip: () => "Default: 60%",
                getValue: () => ModEntry.Config.BaseSpawnChance,
                setValue: value => ModEntry.Config.BaseSpawnChance = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.05f
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Drop Chance Increment",
                tooltip: () =>
                    "The value is added to base drop chance after every 20 floors up to 120.\n" +
                    "Ignored if global chance is on.\n" +
                    "Default: 5%",
                getValue: () => ModEntry.Config.IncreaseSpawnChanceStep,
                setValue: value => ModEntry.Config.IncreaseSpawnChanceStep = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.05f
            );
            configMenu.AddParagraph(
                mod: manifest,
                text: () => "The dangerous variants of the mines and skull cavern add add 10%"
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Additional Spawn Attempts",
                tooltip: () => "Increase this value to have better chance to spawn a crate per floor.\nDoesn't work with the more barrels.\nDefault: 0",
                getValue: () => ModEntry.Config.AdditionalTriesToSpawn,
                setValue: value => ModEntry.Config.AdditionalTriesToSpawn = value,
                min: 0,
                max: 50
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Allow More Barrels",
                tooltip: () => "Allow for more than 1 barrel per floor.\nIf value is larger than daily limit will stop spawning after passing it.\nDefault: false",
                getValue: () => ModEntry.Config.AllowMoreThanOne, setValue: value => ModEntry.Config.AllowMoreThanOne = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Additional Barrels Amount",
                tooltip: () => "Increase this value to have more barrels per floor.\nDefault: 0",
                getValue: () => ModEntry.Config.AdditionalBarrels, setValue: value => ModEntry.Config.AdditionalBarrels = value,
                min: 0,
                max: 20
            );
            configMenu.AddPageLink(mod: manifest, pageId: "enchantments", () => "Enchantments Page");
            configMenu.AddPageLink(mod: manifest, pageId: "weapons", () => "Weapons Page");
            enchantmentsPage();
            weaponsPage();
        }

        private static void enchantmentsPage()
        {
            configMenu.AddPage(mod: manifest, pageId: "enchantments", () => "Enchantments");
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "Basic Options"
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Always Innate Enchantment",
                tooltip: () => "Make sure all weapons drop will have innate enchantment.\nDefault: false",
                getValue: () => ModEntry.Config.ForceInnateEnchantment,
                setValue: value => ModEntry.Config.ForceInnateEnchantment = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Always Regular Enchantment",
                tooltip: () => "Make sure all weapons drop will have regular enchantment.\nDefault: false",
                getValue: () => ModEntry.Config.ForceHaveEnchantment,
                setValue: value => ModEntry.Config.ForceHaveEnchantment = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Regular Enchantment Chance",
                tooltip: () => "The chance a weapon will get a regular enchantment.\nDefault: 15%",
                getValue: () => ModEntry.Config.ChanceForEnchantment, 
                setValue: value => ModEntry.Config.ChanceForEnchantment = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.01f
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Innate Enchantment Chance",
                tooltip: () => "The chance a weapon will get an innate enchantment.\nDefault: 50%",
                getValue: () => ModEntry.Config.ChanceForInnate,
                setValue: value => ModEntry.Config.ChanceForInnate = value,
                min: 0f,
                max: 1f,
                formatValue: value => $"{Math.Round(value * 100)}%",
                interval: 0.01f
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "Regular Enchantments Options"
            );
            configMenu.AddParagraph(mod: manifest, text: () => "Select enchantment that could appear");
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Artful",
                getValue: () => ModEntry.Config.AllowArtful,
                setValue: value => ModEntry.Config.AllowArtful = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Bug Killer",
                getValue: () => ModEntry.Config.AllowBugKiller,
                setValue: value => ModEntry.Config.AllowBugKiller = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Crusader",
                getValue: () => ModEntry.Config.AllowCrusader,
                setValue: value => ModEntry.Config.AllowCrusader = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Haymaker",
                getValue: () => ModEntry.Config.AllowHaymaker,
                setValue: value => ModEntry.Config.AllowHaymaker = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Vampiric",
                getValue: () => ModEntry.Config.AllowVampiric,
                setValue: value => ModEntry.Config.AllowVampiric = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "Innate Enchantments Options"
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Keep as is",
                tooltip: () => "While this is selected the options below doesn't do anything",
                getValue: () => ModEntry.Config.KeepVanilla,
                setValue: value => ModEntry.Config.KeepVanilla = value
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Min Innate Enchantments",
                getValue: () => ModEntry.Config.MinInnateEnchantments,
                setValue: value => ModEntry.Config.MinInnateEnchantments = value,
                min: 0,
                max: 8
            );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => "Max Innate Enchantments",
                getValue: () => ModEntry.Config.MaxInnateEnchantments,
                setValue: value => ModEntry.Config.MaxInnateEnchantments = value,
                min: 0,
                max: 8
            );
            configMenu.AddParagraph(mod: manifest, text: () => "Increasing the minimum does guarantee it will be enchanted.\n" + "If max is lower the min the cap will be the max.");

            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Slime Gatherer",
                getValue: () => ModEntry.Config.AllowSlimeGatherer,
                setValue: value => ModEntry.Config.AllowSlimeGatherer = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Slime Slayer",
                getValue: () => ModEntry.Config.AllowSlimeSlayer,
                setValue: value => ModEntry.Config.AllowSlimeSlayer = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Attack",
                getValue: () => ModEntry.Config.AllowAttack,
                setValue: value => ModEntry.Config.AllowAttack = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Speed",
                getValue: () => ModEntry.Config.AllowSpeed,
                setValue: value => ModEntry.Config.AllowSpeed = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Defense",
                getValue: () => ModEntry.Config.AllowDefense,
                setValue: value => ModEntry.Config.AllowDefense = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Weight",
                getValue: () => ModEntry.Config.AllowWeight,
                setValue: value => ModEntry.Config.AllowWeight = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Crit. Chance",
                getValue: () => ModEntry.Config.AllowCritChance,
                setValue: value => ModEntry.Config.AllowCritChance = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Crit. Power",
                getValue: () => ModEntry.Config.AllowCritPow,
                setValue: value => ModEntry.Config.AllowCritPow = value
            );
        }

        private static void weaponsPage()
        {
            configMenu.AddPage(mod: manifest, pageId: "weapons", () => "Weapons");

            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "Basic Options"
            );
            configMenu.AddBoolOption(
                    mod: manifest,
                    name: () => "Skip Galaxy Check",
                    tooltip: () => "Allow galaxy weapons to drop even without getting the galaxy sword before.\nDefault: false",
                    getValue: () => ModEntry.Config.SkipGalaxyCheck, setValue: value => ModEntry.Config.SkipGalaxyCheck = value
                );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Skip Infinity Check",
                tooltip: () => "Allow infinity weapons to drop even without getting an infinity weapon before.\nDefault: false",
                getValue: () => ModEntry.Config.SkipInfinityCheck, setValue: value => ModEntry.Config.SkipInfinityCheck = value
            );
            configMenu.AddSectionTitle(
                mod: manifest,
                text: () => "Weapons that can be obtained"
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Galaxy Sword",
                getValue: () => ModEntry.Config.AllowGalSword,
                setValue: value => ModEntry.Config.AllowGalSword = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Galaxy Dagger",
                getValue: () => ModEntry.Config.AllowGalDagger,
                setValue: value => ModEntry.Config.AllowGalDagger = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Galaxy Hammer",
                getValue: () => ModEntry.Config.AllowGalHammer,
                setValue: value => ModEntry.Config.AllowGalHammer = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Infinity Sword",
                getValue: () => ModEntry.Config.AllowInfSword,
                setValue: value => ModEntry.Config.AllowInfSword = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Infinity Dagger",
                getValue: () => ModEntry.Config.AllowInfDagger,
                setValue: value => ModEntry.Config.AllowInfDagger = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "Infinity Hammer",
                getValue: () => ModEntry.Config.AllowInfHammer,
                setValue: value => ModEntry.Config.AllowInfHammer = value
            );
        }
    }
}
