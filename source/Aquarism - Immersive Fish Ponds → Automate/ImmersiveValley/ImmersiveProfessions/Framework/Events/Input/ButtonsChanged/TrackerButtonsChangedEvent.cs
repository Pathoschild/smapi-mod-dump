/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using Common.Events;
using GameLoop;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal TrackerButtonsChangedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.ModKey.JustPressed())
            Manager.Enable<PointerUpdateTickedEvent>();
        else if (ModEntry.Config.ModKey.GetState() == SButtonState.Released &&
                 !ModEntry.State.ProspectorHunt.Value.IsActive && !ModEntry.State.ScavengerHunt.Value.IsActive)
            Manager.Disable<PointerUpdateTickedEvent>();
    }
}