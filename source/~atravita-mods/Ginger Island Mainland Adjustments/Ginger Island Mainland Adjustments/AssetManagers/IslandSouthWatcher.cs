/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

namespace GingerIslandMainlandAdjustments.AssetManagers;

/// <summary>
/// Watches IslandSouth's resort parrot.
/// </summary>
internal sealed class IslandSouthWatcher
{
    private readonly IGameContentHelper contentHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="IslandSouthWatcher"/> class.
    /// </summary>
    /// <param name="contentHelper">SMAPI's game content helper.</param>
    public IslandSouthWatcher(IGameContentHelper contentHelper) => this.contentHelper = contentHelper;

    /// <summary>
    /// Called when the resort is fixed.
    /// </summary>
    internal void OnResortFixed()
    {
        Globals.ModMonitor.DebugOnlyLog("Resort fixed! Invalidating.", LogLevel.Info);

        try
        {
            foreach (string name in AssetEditor.CharacterDialogues)
            {
                this.contentHelper.InvalidateCacheAndLocalized(AssetEditor.Dialogue + name);
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"Unexpected error refreshing NPC dialogue:\n\n{ex}", LogLevel.Error);
        }
    }
}
