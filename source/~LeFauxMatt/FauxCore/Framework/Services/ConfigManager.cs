/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Services;

using StardewModdingAPI.Events;
using StardewMods.FauxCore.Common.Enums;
using StardewMods.FauxCore.Common.Interfaces;
using StardewMods.FauxCore.Common.Services;
using StardewMods.FauxCore.Common.Services.Integrations.ContentPatcher;
using StardewMods.FauxCore.Common.Services.Integrations.GenericModConfigMenu;
using StardewMods.FauxCore.Framework.Interfaces;
using StardewMods.FauxCore.Framework.Models;

/// <summary>Handles the config menu.</summary>
internal sealed class ConfigManager : ConfigManager<DefaultConfig>, IModConfig
{
    private readonly GenericModConfigMenuIntegration genericModConfigMenuIntegration;

    /// <summary>Initializes a new instance of the <see cref="ConfigManager" /> class.</summary>
    /// <param name="contentPatcherIntegration">Dependency for Content Patcher integration.</param>
    /// <param name="dataHelper">Dependency used for storing and retrieving data.</param>
    /// <param name="eventManager">Dependency used for managing events.</param>
    /// <param name="genericModConfigMenuIntegration">Dependency for Generic Mod Config Menu integration.</param>
    /// <param name="modHelper">Dependency for events, input, and content.</param>
    public ConfigManager(
        ContentPatcherIntegration contentPatcherIntegration,
        IDataHelper dataHelper,
        IEventManager eventManager,
        GenericModConfigMenuIntegration genericModConfigMenuIntegration,
        IModHelper modHelper)
        : base(contentPatcherIntegration, dataHelper, eventManager, modHelper)
    {
        this.genericModConfigMenuIntegration = genericModConfigMenuIntegration;

        eventManager.Subscribe<GameLaunchedEventArgs>(this.OnGameLaunched);
    }

    /// <inheritdoc />
#if DEBUG
    public SimpleLogLevel LogLevel => SimpleLogLevel.More;
#else
    public SimpleLogLevel LogLevel => this.Config.LogLevel;
#endif

    private void OnGameLaunched(GameLaunchedEventArgs e)
    {
        if (this.genericModConfigMenuIntegration.IsLoaded)
        {
            this.SetupModConfigMenu();
        }
    }

    private void SetupModConfigMenu()
    {
        if (!this.genericModConfigMenuIntegration.IsLoaded)
        {
            return;
        }

        var gmcm = this.genericModConfigMenuIntegration.Api;
        var config = this.GetNew();

        // Register mod configuration
        this.genericModConfigMenuIntegration.Register(this.Reset, () => this.Save(config));

        // general options
        gmcm.AddSectionTitle(Mod.Manifest, I18n.Config_Section_General_Title, I18n.Config_Section_General_Description);

        gmcm.AddTextOption(
            Mod.Manifest,
            () => config.LogLevel.ToStringFast(),
            value => config.LogLevel = SimpleLogLevelExtensions.TryParse(value, out var logLevel)
                ? logLevel
                : SimpleLogLevel.Less,
            I18n.Config_LogLevel_Title,
            I18n.Config_LogLevel_Description,
            SimpleLogLevelExtensions.GetNames(),
            LocalizedTextManager.TryFormat);
    }
}