/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Events;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class WarriorWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WarriorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WarriorWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (RingsModule.State.WarriorKillCount <= 0)
        {
            this.Disable();
            return;
        }

        if (e.NewLocation.IsDungeon() || e.NewLocation.HasMonsters())
        {
            return;
        }

        RingsModule.State.WarriorKillCount = 0;
        this.Disable();
    }
}
