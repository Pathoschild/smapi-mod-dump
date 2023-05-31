/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Weapons;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class SpecialItemHandicapNpcListChangedEvent : NpcListChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpecialItemHandicapNpcListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpecialItemHandicapNpcListChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => WeaponsModule.Config.EnableRebalance;

    /// <inheritdoc />
    protected override void OnNpcListChangedImpl(object? sender, NpcListChangedEventArgs e)
    {
        if (e.Location is not MineShaft)
        {
            return;
        }

        var monsters = e.Added.OfType<Monster>().ToArray();
        if (monsters.Any(m => m.hasSpecialItem.Value))
        {
            WeaponsModule.State.MonsterDropAccumulator = 0.0;
            return;
        }

        foreach (var monster in e.Added.OfType<Monster>())
        {
            if (Game1.random.NextDouble() < WeaponsModule.State.MonsterDropAccumulator)
            {
                monster.hasSpecialItem.Value = true;
                Game1.player.Write(DataKeys.MonsterDropAccumulator, "0.0");
            }
            else
            {
                WeaponsModule.State.MonsterDropAccumulator += 0.00025;
            }
        }
    }
}
