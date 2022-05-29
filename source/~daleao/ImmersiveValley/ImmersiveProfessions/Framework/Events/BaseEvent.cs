/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events;

#region using directives

using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Base implementation for an event wrapper allowing dynamic enabling / disabling.</summary>
internal abstract class BaseEvent : IEvent
{
    protected readonly PerScreen<bool> enabled = new();

    /// <inheritdoc />
    public bool IsEnabled => enabled.Value;

    /// <inheritdoc />
    public bool IsEnabledForScreen(int screenId)
    {
        return enabled.GetValueForScreen(screenId);
    }

    /// <inheritdoc />
    public void Enable()
    {
        enabled.Value = true;
    }

    /// <inheritdoc />
    public void Disable()
    {
        enabled.Value = false;
    }
}