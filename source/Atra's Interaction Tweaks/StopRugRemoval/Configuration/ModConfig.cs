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
using StardewValley.Locations;

namespace StopRugRemoval.Configuration;

/// <summary>
/// Configuration class for this mod.
/// </summary>
public class ModConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether or not the entire mod is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not rugs should not be removed from under things.
    /// </summary>
    public bool PreventRugRemoval { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether whether or not I should be able to place rugs outside.
    /// </summary>
    public bool CanPlaceRugsOutside { get; set; } = false;

#if DEBUG
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
    /// Gets or sets a value indicating whether planting on rugs should be allowed.
    /// </summary>
    public bool PreventPlantingOnRugs { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether grass should be placed under objects.
    /// </summary>
    public bool PlaceGrassUnder { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether jukeboxes should be playable everywhere.
    /// </summary>
    public bool JukeboxesEverywhere { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether golden coconuts should be allowed to appear off the island, if you've cracked at least one before.
    /// </summary>
    public bool GoldenCoconutsOffIsland { get; set; } = false;

    /// <summary>
    /// Gets or sets keybind to use to remove an item from a table.
    /// </summary>
    public KeybindList FurniturePlacementKey { get; set; } = KeybindList.Parse("LeftShift + Z");

    /// <summary>
    /// Gets or sets a value indicating whether or not to confirm bomb placement in safe areas.
    /// </summary>
    public ConfirmBombEnum InSafeAreas { get; set; } = ConfirmBombEnum.On;

    /// <summary>
    /// Gets or sets a value indicating whether or not to confirm bomb placement in dangerous areas.
    /// </summary>
    public ConfirmBombEnum InDangerousAreas { get; set; } = ConfirmBombEnum.Off;

    /// <summary>
    /// Gets or sets map to which locations are considered safe.
    /// </summary>
    public Dictionary<string, IsSafeLocationEnum> SafeLocationMap { get; set; } = new();

    /// <summary>
    /// Pre-populates locations.
    /// </summary>
    public void PrePopulateLocations()
    {
        foreach (GameLocation loc in Game1.locations)
        {
            if (loc is SlimeHutch or Town or IslandWest || loc.IsFarm || loc.IsGreenhouse)
            {
                this.SafeLocationMap.TryAdd(loc.NameOrUniqueName, IsSafeLocationEnum.Safe);
            }
            else if (loc is MineShaft or VolcanoDungeon)
            {
                this.SafeLocationMap.TryAdd(loc.NameOrUniqueName, IsSafeLocationEnum.Dangerous);
            }
            else
            {
                this.SafeLocationMap.TryAdd(loc.NameOrUniqueName, IsSafeLocationEnum.Dynamic);
            }
        }
    }
}
