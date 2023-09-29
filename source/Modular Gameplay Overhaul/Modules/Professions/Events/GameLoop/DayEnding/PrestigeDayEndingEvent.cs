/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayEnding;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ProfessionsModule.State.SkillsToReset.Count > 0;

    /// <summary>Gets the current reset queue.</summary>
    private static Queue<ISkill> ToReset => ProfessionsModule.State.SkillsToReset;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        while (ToReset.Count > 0)
        {
            var toReset = ToReset.Dequeue();
            toReset.Reset();
            Log.D($"[Prestige]: {Game1.player.Name}'s {toReset.DisplayName} skill has been reset.");
        }
    }
}
