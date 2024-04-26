/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/justastranger/ArtisanProductsCopyQuality
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Machines;

namespace ArtisanProductsCopyQuality
{
    public class ModEntry : Mod
    {
        internal Config config;
        internal ITranslationHelper i18n => Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            Monitor.Log(i18n.Get("ArtisanProductsCopyQuality.start"), LogLevel.Info);
            Helper.Events.Content.AssetRequested += AssetRequested;
            Helper.Events.GameLoop.GameLaunched += GameLaunched;
        }

        private void GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            config = Helper.ReadConfig<Config>();
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
            {
                var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

                api?.Register(ModManifest, () => config = new Config(), () => Helper.WriteConfig(config), true);
                api?.AddSectionTitle(ModManifest, () => "Targeted Machines");
                api?.AddTextOption(ModManifest, () => string.Join("; ", config.machinesToTarget), (string val) =>
                {
                    config.machinesToTarget = val.Split(";").Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).ToHashSet();
                    Helper.GameContent.InvalidateCache("Data/Machines");
                }, () => i18n.Get("ArtisanProductsCopyQuality.config.TargetedMachines.name"), () => i18n.Get("ArtisanProductsCopyQuality.config.TargetedMachines.description"));
            }
        }

        private void AssetRequested(object? sender, AssetRequestedEventArgs ev)
        {
            if (ev.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
                ev.Edit(EditMachines, AssetEditPriority.Default);
        }

        private void EditMachines(IAssetData asset)
        {
            if (asset.Data is Dictionary<string, MachineData> data)
            {
                foreach (KeyValuePair<string, MachineData> machine in data)
                {
                    if (!config.machinesToTarget.Contains(machine.Key)) continue;
                    machine.Value?.OutputRules.ForEach(rule => {
                        rule?.OutputItem.ForEach(item => {
                            if (item is not null)
                                item.CopyQuality = true;
                        });
                    });

                    Monitor.Log(i18n.Get("ArtisanProductsCopyQuality.patch", new { machine.Key }), LogLevel.Trace);
                }
            }
        }
    }
}
