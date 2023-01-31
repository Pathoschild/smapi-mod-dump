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

using Microsoft.Xna.Framework;

using Pathoschild.Stardew.Automate;

using StardewValley.TerrainFeatures;


namespace TapGiantCropsAutomateBridge.Framework;

/// <summary>
/// Tracks a tapped giant crop.
/// </summary>
public class TappedGiantCrop : IMachine
{
    private GiantCrop crop;
    private SObject tapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TappedGiantCrop"/> class.
    /// </summary>
    /// <param name="crop">The giant crop to tap.</param>
    /// <param name="tapper">The tapper instance.</param>
    /// <param name="location">The map the tapper is on.</param>
    /// <param name="tileArea">The area that should be covered for automate.</param>
    public TappedGiantCrop(GiantCrop crop, SObject tapper, GameLocation location, Rectangle tileArea)
    {
        this.crop = crop;
        this.tapper = tapper;
        this.Location = location;
        this.TileArea = tileArea;
    }

    public string MachineTypeID => "atravita/TappedGiantCrop";

    public GameLocation Location { get; init; }

    public Rectangle TileArea { get; init; }

    public ITrackedStack? GetOutput()
    {
        AutomateBridge.ModMonitor.DebugOnlyLog($"Automated requested output of giant crop at {this.Location} - {this.crop.tile}");
        return new TrackedItem(
            this.tapper.heldObject.Value,
            onEmpty: (_) =>
            {
                this.tapper.heldObject.Value = null;
                this.tapper.readyForHarvest.Value = false;
            });
    }

    public MachineState GetState()
    {
        AutomateBridge.ModMonitor.DebugOnlyLog($"Automate requested state of giant crop at {this.Location} - {this.crop.tile} - {this.tapper.heldObject.Value?.ParentSheetIndex ?? -1}");
        if (this.tapper.heldObject.Value is null)
        {
            return MachineState.Empty;
        }
        return this.tapper.readyForHarvest.Value ? MachineState.Done : MachineState.Processing;
    }

    /// <inheritdoc />
    public bool SetInput(IStorage input) => false; // Always false, never takes input.
}
