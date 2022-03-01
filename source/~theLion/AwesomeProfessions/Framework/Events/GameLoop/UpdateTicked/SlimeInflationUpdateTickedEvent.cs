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

internal class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var uninflatedSlimes = ModEntry.PlayerState.Value.SuperfluidSlimes.Where(p => !p.DoneInflating).ToArray();
        if (!uninflatedSlimes.Any())
        {
            Disable();
            return;
        }

        foreach (var piped in uninflatedSlimes) piped.Inflate();
    }
}