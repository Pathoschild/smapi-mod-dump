/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Ponds.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class PondSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PondSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    public override bool IsEnabled => Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var buildings = Game1.getFarm().buildings;
        for (var i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            if (building is not FishPond pond || pond.isUnderConstruction() ||
                pond.fishType.Value is not (160 or 899) ||
                pond.Read<int>(DataKeys.FamilyLivingHere) is not ({ } familyCount and > 0))
            {
                continue;
            }

            var data = pond.GetFishPondData();
            {
                if (data is null)
                {
                    break;
                }
            }

            var mates = Math.Min(pond.FishCount - familyCount, familyCount);
            data.SpawnTime = 12 / mates;
        }
    }
}
