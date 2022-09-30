/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Framework;

using System.Collections.Generic;
using System.Linq;
using StardewMods.Common.Integrations.ExpandedStorage;
using StardewMods.Common.Integrations.GenericModConfigMenu;
using StardewMods.ExpandedStorage.Models;

/// <summary>
///     Config helper for Expanded Storage.
/// </summary>
internal sealed class Config
{
#nullable disable
    private static Config Instance;
#nullable enable
    private readonly ModConfig _config;

    private readonly IModHelper _helper;
    private readonly IManifest _manifest;

    private Config(IModHelper helper, IManifest manifest)
    {
        this._helper = helper;
        this._manifest = manifest;
        this._config = this._helper.ReadConfig<ModConfig>();
    }

    private static IGenericModConfigMenuApi GMCM => Integrations.GenericModConfigMenu.API!;

    private static ModConfig ModConfig => Config.Instance._config;

    private static IManifest ModManifest => Config.Instance._manifest;

    /// <summary>
    ///     Gets config data for an Expanded Storage chest type.
    /// </summary>
    /// <param name="id">The id of the config to get.</param>
    /// <returns>Returns storage config data.</returns>
    public static StorageConfig? GetConfig(string id)
    {
        return Config.ModConfig.Config.TryGetValue(id, out var config) ? config : null;
    }

    /// <summary>
    ///     Initializes <see cref="Config" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="manifest">A manifest to describe the mod.</param>
    /// <returns>Returns an instance of the <see cref="Config" /> class.</returns>
    public static Config Init(IModHelper helper, IManifest manifest)
    {
        return Config.Instance ??= new(helper, manifest);
    }

    /// <summary>
    ///     Setup Generic Mod Config Options menu.
    /// </summary>
    /// <param name="storages">The storages to add to the mod config menu.</param>
    public static void SetupConfig(IDictionary<string, ICustomStorage> storages)
    {
        var configStorages = storages.Where(storage => storage.Value.PlayerConfig)
                                     .OrderBy(storage => storage.Value.DisplayName)
                                     .ToArray();

        foreach (var (id, storage) in configStorages)
        {
            if (Config.ModConfig.Config.TryGetValue(id, out var config))
            {
                continue;
            }

            config = new() { BetterChestsData = new() };
            storage.BetterChestsData?.CopyTo(config.BetterChestsData);
            Config.ModConfig.Config.Add(id, config);
        }

        if (!Integrations.GenericModConfigMenu.IsLoaded)
        {
            return;
        }

        Integrations.GenericModConfigMenu.Register(Config.ModManifest, Config.Reset, Config.Save);

        if (!Integrations.BetterChests.IsLoaded)
        {
            return;
        }

        foreach (var (id, storage) in configStorages)
        {
            if (storage.BetterChestsData is null)
            {
                continue;
            }

            Config.GMCM.AddPageLink(Config.ModManifest, id, () => storage.DisplayName, () => storage.Description);
        }

        foreach (var (id, storage) in configStorages)
        {
            if (storage.BetterChestsData is null)
            {
                continue;
            }

            var config = Config.GetConfig(id);
            if (config?.BetterChestsData is null)
            {
                continue;
            }

            Config.GMCM.AddPage(Config.ModManifest, id, () => storage.DisplayName);
            Integrations.BetterChests.API.AddConfigOptions(Config.ModManifest, config.BetterChestsData);
        }
    }

    private static void Reset()
    {
        var config = new ModConfig();
        config.CopyTo(Config.ModConfig);
    }

    private static void Save()
    {
        Config.Instance._helper.WriteConfig(Config.ModConfig);
    }
}