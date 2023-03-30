/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Taxes.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var farm = Game1.getFarm();
        if (!Game1.player.IsMainPlayer || farm.Read(DataKeys.UsableTiles, -1) > 0)
        {
            return;
        }

        var usableTiles = 0;
        for (var i = 0; i < farm.Map.DisplayHeight; i++)
        {
            for (var j = 0; j < farm.Map.DisplayWidth; j++)
            {
                if (farm.doesTileHaveProperty(i, j, "Diggable", "Back") is not null)
                {
                    usableTiles++;
                }
            }
        }

        farm.Write(DataKeys.UsableTiles, usableTiles.ToString());
        Log.D($"[Taxes]: Counted {usableTiles} usable tiles in {farm.Name}.");
    }
}
