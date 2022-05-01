/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

namespace GingerIslandMainlandAdjustments.CustomConsoleCommands;

/// <summary>
/// Model to save custom data into the save.
/// </summary>
/// <remarks>Only available for the main player.</remarks>
public sealed class SaveDataModel
{
    /// <summary>
    /// Gets or sets list of NPCS queued for the next day for Ginger Island.
    /// </summary>
    public List<string> NPCsForTomorrow { get; set; } = new();
}