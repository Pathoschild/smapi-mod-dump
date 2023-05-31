/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        BleedAnimation.BleedAnimationByMonster.Clear();
        BurnAnimation.BurnAnimationByMonster.Clear();
        PoisonAnimation.PoisonAnimationByMonster.Clear();
        SlowAnimation.SlowAnimationByMonster.Clear();
        StunAnimation.StunAnimationByMonster.Clear();
    }
}
