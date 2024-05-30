/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CropWitherDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class CropWitherDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMainPlayer && Config.CropWitherChance > 0f;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        foreach (var location in Game1.locations)
        {
            foreach (var dirt in location.terrainFeatures.Values.OfType<HoeDirt>())
            {
                if (dirt.crop is null || dirt.crop.dead.Value || dirt.state.Value == 1)
                {
                    Data.Write(dirt, DataKeys.DaysSinceWatered, null);
                }
                else if (dirt.crop is { dead.Value: false, forageCrop.Value: false } crop && dirt.state.Value == 0)
                {
                    if (crop.indexOfHarvest.Value == QualifiedObjectIds.Fiber)
                    {
                        continue;
                    }

                    var daysSinceWatered = Data.ReadAs<int>(dirt, DataKeys.DaysSinceWatered);
                    if (Game1.random.NextDouble() < Config.CropWitherChance * (daysSinceWatered - 1))
                    {
                        dirt.crop.Kill();
                    }
                    else
                    {
                        Data.Increment(dirt, DataKeys.DaysSinceWatered);
                    }
                }
            }
        }
    }
}
