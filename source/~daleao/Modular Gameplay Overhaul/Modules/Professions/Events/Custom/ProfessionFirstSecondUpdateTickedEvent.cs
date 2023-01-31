/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Custom;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProfessionFirstSecondUpdateTickedEvent : FirstSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionFirstSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionFirstSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnFirstSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        this.Manager.Enable<LateLoadOneSecondUpdateTickedEvent>();
    }
}
