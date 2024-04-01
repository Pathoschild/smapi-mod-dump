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
using System;

namespace UnbreakableTackles
{
    public class ModEntry : Mod
    {
        public static IModHelper IHelper;
        public static IMonitor IMonitor;
        public static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            IConfig = helper.ReadConfig<Config>();

            helper.Events.GameLoop.GameLaunched += onGameLaunch;
        }

        private void onGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            Patches.Patch(IHelper.ModRegistry.ModID);
            registerForGMCM();
        }

        private void registerForGMCM()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (gmcm is null)
                return;

            gmcm.Register(ModManifest, () => IConfig = new(), () => IHelper.WriteConfig(IConfig));

            gmcm.AddBoolOption(ModManifest, () => IConfig.consumeBait, (x) => IConfig.consumeBait = x, () => "Consume Bait", () => "Whether or not bait should be consumed");
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }
}
