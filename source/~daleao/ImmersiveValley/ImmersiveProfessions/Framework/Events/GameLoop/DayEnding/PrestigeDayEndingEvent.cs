/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeDayEndingEvent : DayEndingEvent
{
    private static Queue<ISkill> _ToReset => ModEntry.PlayerState.SkillsToReset;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PrestigeDayEndingEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        while (_ToReset.Count > 0)
        {
            var toReset = _ToReset.Dequeue();
            switch (toReset)
            {
                case Skill skill:
                    Game1.player.ResetSkill(skill);
                    break;
                case CustomSkill customSkill:
                    Game1.player.ResetCustomSkill(customSkill);
                    break;
            }
        }

        Unhook();
    }
}