/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace AnimalsDie
{
    using StardewModdingAPI;
    using System;
    using System.Text.RegularExpressions;

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class AnimalsDieConfig
    {
        public bool DeathByOldAge { get; set; } = true;

        public bool DeathByStarvation { get; set; } = true;

        public bool DeathByDehydrationWithAnimalsNeedWaterMod { get; set; } = true;

        public bool DeathByIllness { get; set; } = true;

        public int DaysToDieDueToStarvation { get; set; } = 5;

        public int DaysToDieDueToDehydrationWithAnimalsNeedWaterMod { get; set; } = 3;

        public int IllnessScoreToDie { get; set; } = 7;

        public bool IllnessMessages { get; set; } = true;

        public int MinAgeCow { get; set; } = 14;

        public int MaxAgeCow { get; set; } = 16;

        public int MinAgeChicken { get; set; } = 5;

        public int MaxAgeChicken { get; set; } = 7;

        public int MinAgeDuck { get; set; } = 8;

        public int MaxAgeDuck { get; set; } = 12;

        public int MinAgeRabbit { get; set; } = 8;

        public int MaxAgeRabbit { get; set; } = 12;

        public int MinAgeGoat { get; set; } = 8;

        public int MaxAgeGoat { get; set; } = 12;

        public int MinAgeSheep { get; set; } = 10;

        public int MaxAgeSheep { get; set; } = 12;

        public int MinAgePig { get; set; } = 15;

        public int MaxAgePig { get; set; } = 20;

        public int MinAgeOstrich { get; set; } = 40;

        public int MaxAgeOstrich { get; set; } = 45;

        public int MinAgeDinosaur { get; set; } = 100;

        public int MaxAgeDinosaur { get; set; } = 100;

        public static void VerifyConfigValues(AnimalsDieConfig config, AnimalsDie mod)
        {
            bool invalidConfig = false;

            foreach (var prop in typeof(AnimalsDieConfig).GetProperties())
            {
                if (prop.Name.StartsWith("Min"))
                {
                    if (prop.PropertyType == typeof(int))
                    {
                        int minValue = (int)prop.GetValue(config);

                        if (minValue < 0)
                        {
                            invalidConfig = true;
                            prop.SetValue(config, 0);
                        }

                        var maxProp = typeof(AnimalsDieConfig).GetProperty("Max" + prop.Name[3..]);

                        if ((int)maxProp.GetValue(config) < minValue)
                        {
                            invalidConfig = true;
                            maxProp.SetValue(config, minValue);
                        }
                    }
                }

                if (prop.Name == nameof(config.DaysToDieDueToStarvation)
                    || prop.Name == nameof(config.DaysToDieDueToDehydrationWithAnimalsNeedWaterMod)
                    || prop.Name == nameof(config.IllnessScoreToDie))
                {
                    int value = (int)prop.GetValue(config);

                    if (value < 1)
                    {
                        invalidConfig = true;
                        prop.SetValue(config, 1);
                    }
                }
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void SetUpModConfigMenu(AnimalsDieConfig config, AnimalsDie mod, bool isWaterModInstalled)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(manifest, () => config = new AnimalsDieConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            api.AddSectionTitle(manifest, () => "Features", null);

            foreach (var prop in typeof(AnimalsDieConfig).GetProperties())
            {
                if (prop.PropertyType == typeof(bool))
                {
                    string betterName = Regex.Replace(prop.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                    if (prop.Name == nameof(config.DeathByDehydrationWithAnimalsNeedWaterMod))
                    {
                        if (!isWaterModInstalled)
                        {
                            continue;
                        }
                        else
                        {
                            betterName = Regex.Replace(prop.Name.Remove(prop.Name.Length - "WithAnimalsNeedWaterMod".Length), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                        }
                    }

                    api.AddBoolOption(manifest, () => (bool)prop.GetValue(config), (bool b) => prop.SetValue(config, b), () => betterName);
                }
                else
                {
                    if (prop.Name == nameof(config.DaysToDieDueToStarvation) || prop.Name == nameof(config.IllnessScoreToDie))
                    {
                        string betterName = Regex.Replace(prop.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

                        api.AddNumberOption(manifest, () => (int)prop.GetValue(config), (int b) => prop.SetValue(config, b), () => betterName);
                    }

                    if (prop.Name == nameof(config.DaysToDieDueToDehydrationWithAnimalsNeedWaterMod))
                    {
                        string betterName = string.Empty;

                        if (!isWaterModInstalled)
                        {
                            continue;
                        }
                        else
                        {
                            betterName = Regex.Replace(prop.Name.Remove(prop.Name.Length - "WithAnimalsNeedWaterMod".Length), "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
                        }

                        api.AddNumberOption(manifest, () => (int)prop.GetValue(config), (int b) => prop.SetValue(config, b), () => betterName);
                    }
                }
            }

            api.AddSectionTitle(manifest, () => "Ages (in years)", null);

            foreach (var prop in typeof(AnimalsDieConfig).GetProperties())
            {
                if (prop.Name.StartsWith("Min"))
                {
                    if (prop.PropertyType == typeof(int))
                    {
                        string betterName = Regex.Replace(prop.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

                        api.AddNumberOption(manifest, () => (int)prop.GetValue(config), (int i) => prop.SetValue(config, i),
                            () => betterName, () => "must at least 0 and smaller or equal to maximum age, otherwise it's reset");

                        var maxProp = typeof(AnimalsDieConfig).GetProperty("Max" + prop.Name[3..]);

                        betterName = Regex.Replace(maxProp.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

                        api.AddNumberOption(manifest, () => (int)maxProp.GetValue(config), (int i) => maxProp.SetValue(config, i),
                            () => betterName, () => "must be larger or equal to the minimum age, otherwise it's reset");
                    }
                }
            }
        }
    }
}