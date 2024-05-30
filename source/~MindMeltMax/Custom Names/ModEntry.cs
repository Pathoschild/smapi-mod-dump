/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

global using Object = StardewValley.Object;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CustomNames
{
    internal class ModEntry : Mod
    {
        internal static string ModDataKey => $"{IHelper.ModRegistry.ModID}.DisplayName";

        internal static IModHelper IHelper;
        internal static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IConfig = Helper.ReadConfig<Config>();
            Helper.Events.GameLoop.GameLaunched += onGameLaunch;
            /*
             * Plan:
             *  1. Patch DisplayName for Object, Tool, Ring, and maybe more. \\Easy
             *  2. Patch Forge menu and inject a textbox to allow renaming \\Kill me
             *  3. Patch in some functionality for extra flair \\Meh //Never mind, just kill me again (fucking toolbar)
             */
        }

        private void onGameLaunch(object? sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(ModManifest.UniqueID);
            registerForGMCM();
        }

        private void registerForGMCM()
        {
            var api = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (api is null)
                return;

            api.Register(ModManifest, () => IConfig = new(), () => Helper.WriteConfig(IConfig));

            api.AddNumberOption(ModManifest, () => IConfig.CostToName, (v) => IConfig.CostToName = v, () => Helper.Translation.Get("Config.CostToName.Name"), () => Helper.Translation.Get("Config.CostToName.Description"));

            api.AddBoolOption(ModManifest, () => IConfig.UnforgeClearsName, (v) => IConfig.UnforgeClearsName = v, () => Helper.Translation.Get("Config.UnforgeClearsName.Name"), () => Helper.Translation.Get("Config.UnforgeClearsName.Description"));

            api.AddBoolOption(ModManifest, () => IConfig.ShowCompanionName, (v) => IConfig.ShowCompanionName = v, () => Helper.Translation.Get("Config.ShowCompanionName.Name"), () => Helper.Translation.Get("Config.ShowCompanionName.Description"));

            api.AddBoolOption(ModManifest, () => IConfig.ShowToolbarName, (v) => IConfig.ShowToolbarName = v, () => Helper.Translation.Get("Config.ShowToolbarName.Name"), () => Helper.Translation.Get("Config.ShowToolbarName.Description"));
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
    }
}
