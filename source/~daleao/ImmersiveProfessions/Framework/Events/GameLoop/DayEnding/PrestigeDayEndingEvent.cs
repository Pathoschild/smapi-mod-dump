/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using Extensions;

#endregion using directives

internal class PrestigeDayEndingEvent : DayEndingEvent
{
    public PerScreen<Queue<SkillType>> SkillsToReset { get; } = new(() => new());

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        while (SkillsToReset.Value.Any()) Game1.player.ResetSkill(SkillsToReset.Value.Dequeue());
        Disable();
    }
}