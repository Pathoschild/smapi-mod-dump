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

using Common.Events;
using Extensions;
using StardewModdingAPI.Events;
using System.Linq;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SlimeDeflationUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var undeflated = GreenSlime_Piped.Values.Select(pair => pair.Key).ToArray();
        if (undeflated.Length == 0)
        {
            Disable();
            return;
        }

        foreach (var piped in undeflated) piped.Deflate();
    }
}