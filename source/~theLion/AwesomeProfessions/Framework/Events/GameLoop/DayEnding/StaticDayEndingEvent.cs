/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

[UsedImplicitly]
internal class StaticDayEndingEvent : DayEndingEvent
{
    /// <inheritdoc />
    public override void OnDayEnding(object sender, DayEndingEventArgs e)
    {
        ModState.UsedDogStatueToday = false;
    }
}