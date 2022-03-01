/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;

#endregion using directives

internal class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        foreach (var piped in ModEntry.PlayerState.Value.SuperfluidSlimes.Where(p => p.BuffTimer <= 0)) piped.Deflate();

        if (!ModEntry.PlayerState.Value.SuperfluidSlimes.Any()) Disable();
    }
}