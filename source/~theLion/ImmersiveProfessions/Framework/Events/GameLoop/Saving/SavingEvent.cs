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

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class SavingEvent : BaseEvent
{
    /// <summary>Raised before the game writes data to save file.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnSaving(object sender, SavingEventArgs e)
    {
        if (enabled.Value) OnSavingImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaving" />
    protected abstract void OnSavingImpl(object sender, SavingEventArgs e);
}