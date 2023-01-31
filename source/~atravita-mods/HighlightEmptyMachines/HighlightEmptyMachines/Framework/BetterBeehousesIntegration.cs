/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using AtraBase.Toolkit.Extensions;

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.Utils.Extensions;

using StardewModdingAPI.Utilities;

namespace HighlightEmptyMachines.Framework;

/// <summary>
/// Handles the beehives.
/// </summary>
internal static class BeehouseHandler
{
    private static IBetterBeehousesAPI? api;

    /// <summary>
    /// Gets the current beehouse status.
    /// </summary>
    internal static PerScreen<MachineStatus> Status { get; } = new(() => MachineStatus.Disabled);

    /// <summary>
    /// Tries to grab the PFM api.
    /// </summary>
    /// <param name="modRegistry">ModRegistry.</param>
    /// <returns>True if API grabbed, false otherwise.</returns>
    internal static bool TryGetAPI(IModRegistry modRegistry)
        => new IntegrationHelper(ModEntry.ModMonitor, ModEntry.TranslationHelper, modRegistry)
            .TryGetAPI("tlitookilakin.BetterBeehouses", "1.2.6", out api);

    /// <summary>
    /// Updates the status of beehives for the current location.
    /// </summary>
    /// <param name="location">The game location to update to.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal static void UpdateStatus(GameLocation? location)
    {
        if (location is null)
        {
            return;
        }

        if (!ModEntry.Config.VanillaMachines.SetDefault(VanillaMachinesEnum.BeeHouse, true))
        {
            Status.Value = MachineStatus.Disabled;
            return;
        }

        if (api is null)
        {
            Status.Value = (location.IsOutdoors && Game1.GetSeasonForLocation(location) != "winter") ? MachineStatus.Enabled : MachineStatus.Invalid;
        }
        else
        {
            Status.Value = api.GetEnabledHere(location, Game1.GetSeasonForLocation(location) == "winter") ? MachineStatus.Enabled : MachineStatus.Invalid;
        }

        ModEntry.ModMonitor.DebugOnlyLog($"Current status of beehives is {Status.Value}");
    }
}
