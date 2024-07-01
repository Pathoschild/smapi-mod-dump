/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.Input.ButtonsChanged;

#region using directives

using DaLion.Professions.Framework.UI;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TrackerButtonsChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class TrackerButtonsChangedEvent(EventManager? manager = null)
    : ButtonsChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (Config.ModKey.JustPressed())
        {
            HudPointer.Instance.ShouldBob = true;
        }
        else if (Config.ModKey.GetState() == SButtonState.Released && State.ProspectorHunt?.IsActive != true && State.ScavengerHunt?.IsActive != true)
        {
            HudPointer.Instance.ShouldBob = false;
        }
    }
}
