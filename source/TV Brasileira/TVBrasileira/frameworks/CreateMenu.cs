/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JhonnieRandler/TVBrasileira
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TVBrasileira.frameworks
{
    public class CreateMenu
    {
        private readonly IModHelper _helper;
        private readonly IManifest _modManifest;
        private readonly IMonitor _monitor;
        private  ModConfig _config;
        
        private readonly List<string> _patchedAssets = new()
        {
            "LooseSprites/Cursors",
            "LooseSprites/Cursors2",
            "Strings/StringsFromCSFiles",
            "Data/TV/TipChannel"
        };
        
        public CreateMenu(IModHelper helper, IManifest modManifest, IMonitor monitor) {
            _monitor = monitor;
            _helper = helper;
            _config = helper.ReadConfig<ModConfig>();
            _modManifest = modManifest;
            _helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenuApi =
                _helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenuApi is null)
            {
                _monitor.Log(I18n.DisabledGmcm(), LogLevel.Info);
                return;
            }

            configMenuApi.Register(
                mod: _modManifest,
                reset: () => _config = new ModConfig(),
                save: CommitConfig,
                titleScreenOnly: false
            );
            
            configMenuApi.AddBoolOption(
                mod: _modManifest,
                name: () => I18n.TitleEdnaldo(),
                tooltip: () => I18n.TooltipEdnaldo(),
                getValue: () => _config.EdnaldoPereiraToggle,
                setValue: value => _config.EdnaldoPereiraToggle = value
            );
            
            configMenuApi.AddBoolOption(
                mod: _modManifest,
                name: () => I18n.TitlePalmirinha(),
                tooltip: () => I18n.TooltipPalmirinha(),
                getValue: () => _config.PalmirinhaToggle,
                setValue: value => _config.PalmirinhaToggle = value
            );
            
            configMenuApi.AddBoolOption(
                mod: _modManifest,
                name: () => I18n.TitleGloboRural(),
                tooltip: () => I18n.TooltipGloboRural(),
                getValue: () => _config.GloboRuralToggle,
                setValue: value => _config.GloboRuralToggle = value
            );
        }
        
        private void CommitConfig()
        {
            _helper.WriteConfig(_config);

            string currentLocale = _helper.GameContent.CurrentLocale != "" ? 
                "." + _helper.GameContent.CurrentLocale : "";
            
            foreach (var path in _patchedAssets)
            {
                _helper.GameContent.InvalidateCache(path);
                _helper.GameContent.InvalidateCache(path + currentLocale);
            }
        }
    }
}