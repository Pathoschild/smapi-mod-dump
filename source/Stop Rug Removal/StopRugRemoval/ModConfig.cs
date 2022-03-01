/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace StopRugRemoval;

/// <summary>
/// Configuration class for this mod.
/// </summary>
public class ModConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether whether or not the entire mod is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
#if DEBUG
    /// <summary>
    /// Gets or sets a value indicating whether whether or not I should be able to place rugs outside.
    /// </summary>
    public bool CanPlaceRugsOutside { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether whether or not I should be able to place rugs under things.
    /// </summary>
    public bool CanPlaceRugsUnder { get; set; } = true;

#endif

    /// <summary>
    /// Gets or sets a value indicating whether whether or not to prevent the removal of items from a table.
    /// </summary>
    public bool PreventRemovalFromTable { get; set; } = true;

    /// <summary>
    /// Gets or sets keybind to use to remove an item from a table.
    /// </summary>
    public KeybindList FurniturePlacementKey { get; set; } = KeybindList.Parse("LeftShift + Z");
}
