/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.FarmAnimals;
using System;
using System.Collections.Generic;

namespace WinterPigs
{
    internal class ModEntry : Mod
    {
        internal static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            IConfig = Helper.ReadConfig<Config>();

            Helper.Events.GameLoop.GameLaunched += onGameLaunched;
        }

        private void onGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(Monitor, Helper);
            registerForGMCM();
        }

        private void registerForGMCM()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (gmcm is null)
                return;

            gmcm.Register(ModManifest, () => IConfig = new(), () => Helper.WriteConfig(IConfig));

            gmcm.AddTextOption(ModManifest, () => formatListOption(IConfig.AnimalsStayInside), (v) => IConfig.AnimalsStayInside = formatListOption(v), () => Helper.Translation.Get("Config.AnimalsStayInside.Name"), () => Helper.Translation.Get("Config.AnimalsStayInside.Description"));

            gmcm.AddTextOption(ModManifest, () => formatListOption(IConfig.AnimalsGoOutsideBadWeather), (v) => IConfig.AnimalsGoOutsideBadWeather = formatListOption(v), () => Helper.Translation.Get("Config.AnimalsGoOutsideBadWeather.Name"), () => Helper.Translation.Get("Config.AnimalsGoOutsideBadWeather.Description"));

            gmcm.AddTextOption(ModManifest, () => formatListOption(IConfig.AnimalsGoOutsideHorribleWeather), (v) => IConfig.AnimalsGoOutsideHorribleWeather = formatListOption(v), () => Helper.Translation.Get("Config.AnimalsGoOutsideHorribleWeather.Name"), () => Helper.Translation.Get("Config.AnimalsGoOutsideHorribleWeather.Description"));

            gmcm.AddBoolOption(ModManifest, () => IConfig.DelayedPet, (v) => IConfig.DelayedPet = v, () => Helper.Translation.Get("Config.DelayedPet.Name"), () => Helper.Translation.Get("Config.DelayedPet.Description"));

            gmcm.AddBoolOption(ModManifest, () => IConfig.NoTruffleLimit, (v) => IConfig.NoTruffleLimit = v, () => Helper.Translation.Get("Config.NoTruffleLimit.Name"), () => Helper.Translation.Get("Config.NoTruffleLimit.Description"));

            gmcm.AddNumberOption(ModManifest, () => (float)IConfig.PigAnimalCrackerMultiplier, (v) => IConfig.PigAnimalCrackerMultiplier = (float)v, () => Helper.Translation.Get("Config.PigAnimalCrackerMultiplier.Name"), () => Helper.Translation.Get("Config.PigAnimalCrackerMultiplier.Description"));
        }

        private string formatListOption(List<string> items) => string.Join(",", items).TrimEnd(',');

        private List<string> formatListOption(string value) => new(value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
    }
}
