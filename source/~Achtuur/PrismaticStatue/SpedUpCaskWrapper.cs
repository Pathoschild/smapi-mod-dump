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
using SObjectCask = StardewValley.Objects.Cask;

namespace PrismaticStatue;

internal class SpedUpCaskWrapper : GenericSpedUpMachineWrapper
{
    private const int DayMinutes = 1600;

    /// <summary>
    /// <see cref="GenericSpedUpMachineWrapper.entity"/> cast to a <see cref="SObjectCask"/> for easier coding
    /// </summary>
    SObjectCask cask_entity;

    float actualDaysToMature;
    float actualAgingRate;
    public SpedUpCaskWrapper(IMachine machine, int n_statues) : base(machine, n_statues)
    {
    }

    /// <inheritdoc/>
    public override void Initialise()
    {
        if (this.entity is null)
            return;

        this.cask_entity = this.entity as SObjectCask;
        this.spedUp = false;
        this.previousState = this.automateMachine.GetState();
        this.SetActualTime();
    }
    /// <inheritdoc/>
    public override void OnDayStarted()
    {
        if (this.actualDaysToMature > 0)
        {
            this.actualDaysToMature -= 1;
        }
        this.previousState = this.automateMachine.GetState();
    }
    /// <inheritdoc/>
    public override void OnTimeChanged()
    {
    }

    protected override void SetActualTime()
    {
        this.actualAgingRate = this.cask_entity.agingRate.Value;
    }

    /// <inheritdoc/>
    protected override bool ShouldDoSpeedup()
    {
        if (this.automateMachine.GetState() != MachineState.Processing || this.n_statues < 1)
            return false;

        if (this.previousState != MachineState.Processing || // wasn't processing before, but is processing now
            this.actualAgingRate == this.cask_entity.agingRate.Value || // isn't sped up
            this.actualAgingRate == -1) // speed has been restored
            return true;

        return false;
    }

    /// <inheritdoc/>
    public override void SpeedUp()
    {
        if (this.automateMachine.GetState() != MachineState.Processing || this.actualAgingRate <= 0)
            return;

        this.spedUp = true;
        this.cask_entity.agingRate.Value = this.actualAgingRate / GetSpeedUpFactor(n_statues);
        AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"{automateMachine.MachineTypeID} at {automateMachine.Location} ({automateMachine.TileArea.X}, {automateMachine.TileArea.Y}) was sped up:\t{this.actualAgingRate} -> {this.cask_entity.agingRate.Value}\t({Formatter.FormatNStatues(this.n_statues)})");
    }

    /// <inheritdoc/>
    public override void RestoreSpeed()
    {
        //this.cask_entity.daysToMature.Value = this.actualDaysToMature;
        this.cask_entity.agingRate.Value = this.actualAgingRate;
        this.spedUp = false;
        this.actualAgingRate = -1;
    }
}
