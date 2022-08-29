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

using Common.Events;
using StardewModdingAPI.Events;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperDayEndingEvent : DayEndingEvent
{
    private static readonly int _piperBuffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PiperDayEndingEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        Game1.buffsDisplay.removeOtherBuff(_piperBuffId);
        Array.Clear(ModEntry.State.AppliedPiperBuffs, 0, 12);
        Disable();
    }
}