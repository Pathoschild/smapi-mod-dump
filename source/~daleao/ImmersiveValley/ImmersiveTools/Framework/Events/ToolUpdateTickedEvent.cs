/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Framework.Events;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => ModEntry.Shockwave.Value is not null;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolUpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.Config.TicksBetweenWaves > 0 && !e.IsMultipleOf(ModEntry.Config.TicksBetweenWaves)) return;

        ModEntry.Shockwave.Value!.Update(Game1.currentGameTime.TotalGameTime.TotalMilliseconds);
    }
}