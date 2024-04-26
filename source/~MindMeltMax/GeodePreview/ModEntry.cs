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

namespace GeodePreview
{
    internal class ModEntry : Mod
    {
        internal static Config Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Config>();

            helper.Events.GameLoop.GameLaunched += (_, _) =>
            {
                Patches.Patch(ModManifest.UniqueID);

                registerForGMCM();
            };
        }

        private void registerForGMCM()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (gmcm is null)
                return;

            gmcm.Register(ModManifest, () => Config = new(), () => Helper.WriteConfig(Config));

            gmcm.AddBoolOption(ModManifest, () => Config.ShowAlways, (x) => Config.ShowAlways = x, () => "Show Always", () => "Show the treasure item whether in the geode menu or not");

            gmcm.AddBoolOption(ModManifest, () => Config.ShowStack, (x) => Config.ShowStack = x, () => "Show Stack", () => "Whether or not to display the stack size of the treasure item");

            gmcm.AddNumberOption(ModManifest, () => Config.Offset, (x) => Config.Offset = x <= 0 ? 1 : x, () => "Offset", () => "The number of geodes to look ahead");
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
    }
}
