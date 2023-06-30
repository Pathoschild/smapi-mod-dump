/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;

namespace PrismaticStatue;

/// <summary>A machine that turns iron bars into gold bars.</summary>
public class SpeedupStatue : IMachine
{

    public static readonly string TypeId = "Achtuur/SpeedupStatue";

    /*********
    ** Fields
    *********/
    /// <summary>The underlying entity.</summary>
    private readonly SObject Entity;


    /*********
    ** Accessors
    *********/
    /// <summary>The location which contains the machine.</summary>
    public GameLocation Location { get; }

    /// <summary>The tile area covered by the machine.</summary>
    public Rectangle TileArea { get; }


    /// <summary>A unique ID for the machine type.</summary>
    /// <remarks>
    /// This value should be identical for two machines if they have the exact same behavior and input logic. 
    /// For example, if one machine in a group can't process input due to missing items, 
    /// Automate will skip any other empty machines of that type in the same group since it assumes they need the same inputs.
    /// </remarks>
    string IMachine.MachineTypeID { get; } = TypeId;


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="entity">The underlying entity.</param>
    /// <param name="location">The location which contains the machine.</param>
    /// <param name="tile">The tile covered by the machine.</param>
    public SpeedupStatue(SObject entity, GameLocation location, in Vector2 tile)
    {
        this.Entity = entity;
        this.Location = location;
        this.TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
    }

    /// <summary>Get the machine's processing state.</summary>
    public MachineState GetState()
    {
        if (this.Entity.heldObject.Value == null)
            return MachineState.Empty;

        return this.Entity.readyForHarvest.Value
            ? MachineState.Done
            : MachineState.Processing;
    }

    /// <summary>Get the output item.</summary>
    public ITrackedStack GetOutput()
    {
        return new TrackedItem(this.Entity.heldObject.Value, onEmpty: item =>
        {
            this.Entity.heldObject.Value = null;
            this.Entity.readyForHarvest.Value = false;
        });
    }

    /// <summary>Provide input to the machine.</summary>
    /// <param name="input">The available items.</param>
    /// <returns>Returns whether the machine started processing an item.</returns>
    public bool SetInput(IStorage input)
    {
        // Does not need input ingredients
        return true;
    }
}