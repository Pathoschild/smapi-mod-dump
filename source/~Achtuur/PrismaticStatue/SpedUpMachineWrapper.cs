/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Pathoschild.Stardew.Automate;
using PrismaticStatue.Utility;
namespace PrismaticStatue;

public class SpedUpMachineWrapper : GenericSpedUpMachineWrapper
{

    /// <summary>
    /// Initial MinutesUntilReady when this wrapper was constructed
    /// </summary>
    //int intialMinutesUntilReady;

    /// <summary>
    /// Current MinutesUntilReady as if there was no speedup, decremented by 10 every ten minutes
    /// </summary>
    int actualMinutesUntilReady;

    public SpedUpMachineWrapper(IMachine machine, int n_statues) : base(machine, n_statues)
    {
    }

    /// <inheritdoc/>
    public override void Initialise()
    {
        if (this.entity is null)
            return;

        this.spedUp = false;
        this.SetActualTime();
        this.previousState = this.automateMachine.GetState();
    }

    /// <inheritdoc/>
    public override void OnTimeChanged()
    {
        if (this.actualMinutesUntilReady > 0)
        {
            this.actualMinutesUntilReady -= 10;
        }
        this.previousState = this.automateMachine.GetState();
    }

    /// <inheritdoc/>
    public override void OnDayStarted()
    {
    }

    /// <inheritdoc/>
    protected override bool ShouldDoSpeedup()
    {
        if (this.automateMachine.GetState() != MachineState.Processing || this.n_statues < 1)
            return false;

        if (this.previousState != MachineState.Processing || // wasn't processing before, but is processing now
            this.actualMinutesUntilReady == this.entity.MinutesUntilReady || // isn't sped up
            this.actualMinutesUntilReady == -1) // speed has been restored
            return true;

        return false;
    }

    protected override void SetActualTime()
    {
        this.actualMinutesUntilReady = this.entity.MinutesUntilReady;
    }

    /// <inheritdoc/>
    public override void SpeedUp()
    {
        // Don't speedup if there is nothing to speedup
        if (this.automateMachine.GetState() != MachineState.Processing || this.actualMinutesUntilReady <= 0)
            return;

        this.spedUp = true;
        this.entity.MinutesUntilReady = SpeedUpFunction(this.actualMinutesUntilReady, this.n_statues);
        AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"{automateMachine.MachineTypeID} at {automateMachine.Location} ({automateMachine.TileArea.X}, {automateMachine.TileArea.Y}) was sped up:\t{Formatter.FormatMinutes(this.actualMinutesUntilReady)} -> {Formatter.FormatMinutes(this.entity.MinutesUntilReady)}\t({Formatter.FormatNStatues(this.n_statues)})");
    }

    /// <inheritdoc/>
    public override void RestoreSpeed()
    {
        this.spedUp = false;

        AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"{automateMachine.MachineTypeID} at {automateMachine.Location} ({automateMachine.TileArea.X}, {automateMachine.TileArea.Y}) speed restored:\t{Formatter.FormatMinutes(this.actualMinutesUntilReady)} -> {Formatter.FormatMinutes(this.entity.MinutesUntilReady)}");
        this.entity.MinutesUntilReady = this.actualMinutesUntilReady;

        // Set these values to -1 to know that in the next update the machine's speed was restored, as opposed to just finished/not processing
        this.actualMinutesUntilReady = -1;
    }
}
