/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events;

/// <summary>Interface for an event wrapper allowing dynamic enabling / disabling.</summary>
public interface IEvent
{
    /// <summary>Whether this event is enabled.</summary>
    bool IsEnabled { get; }

    /// <summary>Whether this event is enabled for a specific splitscreen player.</summary>
    /// <param name="screenId">The player's screen id.</param>
    bool IsEnabledForScreen(int screenId);

    /// <summary>Enable this event on the current screen.</summary>
    void Enable();

    /// <summary>Disable this event on the current screen.</summary>
    void Disable();
}