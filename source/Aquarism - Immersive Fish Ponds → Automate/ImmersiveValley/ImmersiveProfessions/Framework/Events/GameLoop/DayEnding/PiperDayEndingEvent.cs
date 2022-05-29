/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using System;
using JetBrains.Annotations;
using StardewValley;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class PiperDayEndingEvent : DayEndingEvent
{
    private static readonly int _which = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Profession.Piper;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        Game1.buffsDisplay.removeOtherBuff(_which);
        Array.Clear(ModEntry.PlayerState.AppliedPiperBuffs, 0, 12);
        this.Disable();
    }
}