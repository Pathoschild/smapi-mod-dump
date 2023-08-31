/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using System;
using System.Reflection;
using SObject = StardewValley.Object;
namespace PrismaticStatue;

public abstract class GenericSpedUpMachineWrapper
{
    /// <summary>
    /// Underlying <see cref="StardewValley.Object"/> entity. This entity is edited to get the ingame speedup.
    /// </summary>
    protected SObject entity;

    /// <summary>
    /// Underlying Automate machine, this is used to know which machine this is in relation to the Automate() method
    /// </summary>
    protected IMachine automateMachine;

    /// <summary>
    /// Previous state of machine, used to check if machine should be sped up when starting to, or already processing
    /// </summary>
    protected MachineState previousState;

    /// <summary>
    /// Amount of statues in the same group as this machine
    /// </summary>
    protected int n_statues;

    /// <summary>
    /// Whether this wrapper's machine is sped up
    /// </summary>
    protected bool spedUp;

    public GenericSpedUpMachineWrapper(IMachine machine, int n_statues)
    {
        this.automateMachine = machine;
        this.n_statues = n_statues;
        this.entity = GetMachineEntity(this.automateMachine);

        Initialise();
    }

    /// <summary>
    /// Initialise internal values that keep track of the wrapped machine's speedup state
    /// </summary>
    public abstract void Initialise();

    /// <summary>
    /// Event listener for TimeChanged event
    /// </summary>
    public abstract void OnTimeChanged();

    /// <summary>
    /// Event listener for DayStarted event
    /// </summary>
    public abstract void OnDayStarted();

    /// <summary>
    /// Set internal counter that keeps track of the underlying entity's actual time
    /// </summary>
    protected abstract void SetActualTime();

    /// <summary>
    /// Return true if this machine should speed up, when not sped up already.
    /// </summary>
    /// <returns></returns>
    protected abstract bool ShouldDoSpeedup();

    /// <summary>
    /// Perform speed up on internal <see cref="GenericSpedUpMachineWrapper.entity"/>
    /// </summary>
    public abstract void SpeedUp();

    /// <summary>
    /// Restore speed of internal <see cref="GenericSpedUpMachineWrapper.entity"/> to value as if the statue was never there.
    /// </summary>
    public abstract void RestoreSpeed();

    /// <summary>
    /// Update internal speed up state of this wrapper. Is called once per Automate() call
    /// </summary>
    public void UpdateState()
    {
        // If previous state was not processing -> machine began processing and minutesuntilready should be reset
        if (ShouldDoSpeedup())
        {
            // Set initial values for speedup
            this.SetActualTime();

            // Calculate speedup for machine
            this.SpeedUp();

            this.previousState = MachineState.Processing;
        }
        else if (this.automateMachine.GetState() != MachineState.Processing)
        {
            this.spedUp = false;
        }
    }

    /// <summary>
    /// Returns true if internal <see cref="GenericSpedUpMachineWrapper.entity"/> is null
    /// </summary>
    /// <returns></returns>
    public bool isNull()
    {
        return this.entity is null;
    }

    /// <summary>
    /// Returns true if this wrapper holds a sped up machine
    /// </summary>
    /// <returns></returns>
    public bool isSpedUp()
    {
        return this.spedUp;
    }

    /// <summary>
    /// Returns <see cref="GameLocation"/> of this wrapper's machine
    /// </summary>
    /// <returns><see cref="GameLocation"/> of this wrapper's machine, or null if this wrapper does not hold an IMachine</returns>
    public GameLocation GetLocation()
    {
        return (this.automateMachine is not null)
            ? this.automateMachine.Location
            : null;
    }

    /// <summary>
    /// Returns tile position of this wrapper's machine
    /// </summary>
    /// <returns><see cref="Vector2"/> with tile position of this wrapper's IMachine, or null if no IMachine held.</returns>
    public Vector2? GetTile()
    {
        return (this.automateMachine is not null)
            ? new Vector2(this.automateMachine.TileArea.X, this.automateMachine.TileArea.Y)
            : null;
    }

    /// <summary>
    /// Returns true when this machine is on specified tile
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    internal bool IsOnTile(Vector2 tile)
    {
        Vector2? this_tile = this.GetTile();
        return (this_tile is null)
            ? false
            : this_tile == tile;
    }

    /// <summary>
    /// Returns true if this wrapper's machine and <paramref name="other"/> wrapper's machine are the same
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool isSameMachine(GenericSpedUpMachineWrapper other)
    {
        return isSameMachine(other.automateMachine);
    }

    /// <summary>
    /// Returns true if this wrapper's machine and <paramref name="machine"/> are the same
    /// </summary>
    /// <param name="machine"></param>
    /// <returns></returns>
    public bool isSameMachine(IMachine machine)
    {
        return this.automateMachine.MachineTypeID == machine.MachineTypeID && this.automateMachine.TileArea == machine.TileArea;
    }

    /// <summary>
    /// Updates number of statues this wrapper's machine is in contact with. Should only be called if the number of statues changes.
    /// </summary>
    /// <param name="new_n_statues"></param>
    public void OnNStautesChange(int new_n_statues)
    {
        this.n_statues = new_n_statues;

        if (this.n_statues == 0)
        {
            // No statues -> restore speed
            this.RestoreSpeed();
        }
        else
        {
            // Force speedup with new n_statues
            this.SpeedUp();
        }
    }

    /// <summary>
    /// Calculate factor of speedup such that <c>t_fast = t_normal * GetSpeedUpFactor(n_statues)</c>
    /// </summary>
    /// <param name="n_statues"></param>
    /// <returns></returns>
    public static float GetSpeedUpFactor(int n_statues)
    {
        double statues = Math.Min(ModEntry.Instance.Config.MaxStatues, n_statues);

        // Do factor^n_statues
        return (float)Math.Pow(ModEntry.Instance.Config.StatueSpeedupFactor, statues);
    }

    /// <summary>
    /// Calculates new time left based on original time left 
    /// </summary>
    /// <param name="original_time"></param>
    public static int SpeedUpFunction(int minutes_until_ready, int n_statues)
    {
        if (n_statues == 0)
            return minutes_until_ready;

        double minutes_unrounded = minutes_until_ready * GetSpeedUpFactor(n_statues);
        return RoundToNearestTenth((int)minutes_unrounded);
    }

    /// <summary>
    /// Rounds <paramref name="x"/> to nearest tenth, with a minimum of 10
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static int RoundToNearestTenth(int x)
    {
        return Math.Max(10, Math.Max(x - (x % 10), 0));
    }

    public static SObject GetMachineEntity(IMachine machine)
    {
        try
        {
            // Get derived class of GenericObjectMachine, which is derived of BaseMachine<MachineT>
            var BaseMachineDerived = machine.GetType().GetProperty("Machine", BindingFlags.Public | BindingFlags.Instance).GetValue(machine, null);

            string MachineId = BaseMachineDerived.GetType().GetProperty("MachineTypeID", BindingFlags.Public | BindingFlags.Instance).GetValue(BaseMachineDerived) as string;
            if (ModEntry.PFMEnabled && MachineId.Contains("PFM"))
                return GetPFMMachineEntity(machine);

            // Get underlying StardewValley.Object this machine refers to
            SObject MachineEntity = BaseMachineDerived.GetType().GetProperty("Machine", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(BaseMachineDerived, null) as SObject;

            return MachineEntity;
        }
        catch (Exception e)
        {
            AchtuurCore.Logger.TraceLog(
                ModEntry.Instance.Monitor,
                $"Failed to find underlying machine entity for {machine.MachineTypeID} at {machine.Location} ({machine.TileArea.X}, {machine.TileArea.Y})"
            );
            return null;
        }
    }

    public static SObject GetPFMMachineEntity(IMachine machine)
    {
        try
        {
            var BaseMachineDerived = machine.GetType().GetProperty("Machine", BindingFlags.Public | BindingFlags.Instance).GetValue(machine, null);

            // vanilla machine, wrapped using "PfmMachine" property
            var VanillaMachine = BaseMachineDerived.GetType().GetField("PfmMachine", BindingFlags.Public | BindingFlags.Instance);

            if (VanillaMachine is not null)
                BaseMachineDerived = VanillaMachine.GetValue(BaseMachineDerived);

            return BaseMachineDerived.GetType().GetField("_machine", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(BaseMachineDerived) as SObject;
        }
        catch (Exception e)
        {
            AchtuurCore.Logger.TraceLog(
                ModEntry.Instance.Monitor,
                $"(PFM) Failed to find underlying machine entity for {machine.MachineTypeID} at {machine.Location} ({machine.TileArea.X}, {machine.TileArea.Y})"
            );
            return null;
        }
    }
}
