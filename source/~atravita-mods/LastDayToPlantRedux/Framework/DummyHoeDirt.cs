/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Diagnostics;

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace LastDayToPlantRedux.Framework;

/// <summary>
/// A subclass of Hoedirt that nixes all the location specific stuff.
/// Also nixes a few other methods.
/// </summary>
[DebuggerDisplay("fertilizer {fertilizer.Value} near water {nearWaterForPaddy.Value}")]
internal class DummyHoeDirt : HoeDirt
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DummyHoeDirt"/> class.
    /// </summary>
    /// <param name="fertilizer">Fertilizer to use.</param>
    public DummyHoeDirt(int fertilizer)
    {
        this.currentLocation = Game1.getFarm();
        this.fertilizer.Value = fertilizer;
    }

    /// <summary>
    /// nop'ed.
    /// </summary>
    /// <param name="environment">location.</param>
    /// <param name="tileLocation">tile.</param>
    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
    }

    /// <summary>
    /// nop'ed.
    /// </summary>
    /// <param name="onLoad">irrelevant.</param>
    /// <returns>always false.</returns>
    public override bool seasonUpdate(bool onLoad)
    {
        return false;
    }

    /// <summary>
    /// Calculates how long it takes for a specific crop to grow with a given farmer.
    /// </summary>
    /// <param name="who">Farmer to check.</param>
    /// <returns>Number of days it takes to grow.</returns>
    internal int? CalculateTimings(Farmer who)
    {
        this.applySpeedIncreases(who);
        int ret = 0;
        for(int i = 0; i < this.crop.phaseDays.Count - 1; i++)
        {
            ret += this.crop.phaseDays[i];
        }
        return ret;
    }
}
