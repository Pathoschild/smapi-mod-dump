/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using System;

namespace EscasModdingPlugins.ModInteractions
{
    public static class GMCM
    {
        /// <summary>Initializes this mod's GMCM menu if available.</summary>
        /// <param name="helper">The helper instance to use for API access.</param>
        /// <param name="monitor">The monitor instance to use for console/log messages.</param>
        /// <param name="manifest">This mod's manifest for use by GMCM.</param>
        public static void Initialize(IModHelper helper, IMonitor monitor, IManifest manifest)
        {
            try
            {
                var api = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                if (api == null)
                {
                    if (monitor.IsVerbose)
                        monitor.Log($"GMCM does not seem to be available. Skipping menu setup.", LogLevel.Trace);
                    return;
                }

                // register mod
                api.Register(
                    mod: manifest,
                    reset: () => ModConfig.Instance = new ModConfig(),
                    save: () => helper.WriteConfig(ModConfig.Instance)
                );

                api.AddBoolOption
                (
                    mod: manifest,
                    name: () => helper.Translation.Get("Config.PlaceBedsAnywhere.Name"),
                    tooltip: () => helper.Translation.Get("Config.PlaceBedsAnywhere.Description"),
                    getValue: () => ModConfig.Instance.AllowBedPlacementEverywhere,
                    setValue: value => ModConfig.Instance.AllowBedPlacementEverywhere = value
                );

                api.AddBoolOption
                (
                    mod: manifest,
                    name: () => helper.Translation.Get("Config.PlaceMiniFridgesAnywhere.Name"),
                    tooltip: () => helper.Translation.Get("Config.PlaceMiniFridgesAnywhere.Description"),
                    getValue: () => ModConfig.Instance.AllowMiniFridgesEverywhere,
                    setValue: value => ModConfig.Instance.AllowMiniFridgesEverywhere = value
                );

                api.AddBoolOption
                (
                    mod: manifest,
                    name: () => helper.Translation.Get("Config.PassOutSafelyEverywhere.Name"),
                    tooltip: () => helper.Translation.Get("Config.PassOutSafelyEverywhere.Description"),
                    getValue: () => ModConfig.Instance.PassOutSafelyEverywhere,
                    setValue: value => ModConfig.Instance.PassOutSafelyEverywhere = value
                );

                if (monitor.IsVerbose)
                    monitor.Log($"GMCM menu setup complete.", LogLevel.Trace);
            }
            catch (Exception ex)
            {
                monitor.Log($"Error while setting up in-game menu with Generic Mod Config Menu (GMCM). The menu might be missing or incomplete. Full error message: \n{ex}", LogLevel.Warn);
            }
        }
    }
}
