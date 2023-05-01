/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CameraPan.HarmonyPatches;

namespace CameraPan.Framework;

/// <inheritdoc />
public sealed class API : ICameraAPI
{
    private string callerUniqueID;

    /// <summary>
    /// Initializes a new instance of the <see cref="API"/> class.
    /// </summary>
    /// <param name="callerUniqueID">The unique ID of the caller.</param>
    public API(string callerUniqueID) => this.callerUniqueID = callerUniqueID;

    /// <inheritdoc />
    public bool IsEnabled => ModEntry.IsEnabled && ViewportAdjustmentPatches.Behavior.HasFlagFast(CameraBehavior.Offset);

    /// <inheritdoc />
    public CameraBehavior Behavior => ViewportAdjustmentPatches.Behavior;

    /// <inheritdoc />
    public ToggleBehavior ToggleBehavior => ModEntry.Config.ToggleBehavior;

    /// <inheritdoc />
    public void Disable()
    {
        ModEntry.ModMonitor.Log($"{this.callerUniqueID} disabling panning.");
        ModEntry.IsEnabled = false;
    }

    /// <inheritdoc />
    public void Enable()
    {
        ModEntry.ModMonitor.Log($"{this.callerUniqueID} enabling panning.");
        ModEntry.IsEnabled = true;
    }

    /// <inheritdoc />
    public void HardReset()
    {
        ModEntry.ModMonitor.Log($"{this.callerUniqueID} hard resetting.");
        ModEntry.Reset();
    }

    /// <inheritdoc />
    public void Reset()
    {
        ModEntry.ModMonitor.Log($"{this.callerUniqueID} resetting.");
        ModEntry.Reset();
        ModEntry.SnapOnNextTick = true;
    }
}
