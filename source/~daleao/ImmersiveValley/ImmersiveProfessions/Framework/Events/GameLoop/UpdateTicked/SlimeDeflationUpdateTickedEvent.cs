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

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var undeflated = ModEntry.PlayerState.PipedSlimes.Where(b => b.ReadDataAs<double>("PipeTimer") <= 0)
            .ToArray();
        foreach (var piped in undeflated)
            piped.Deflate();

        if (!ModEntry.PlayerState.PipedSlimes.Any()) this.Disable();
    }
}