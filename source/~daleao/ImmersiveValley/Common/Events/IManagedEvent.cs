/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Events;

/// <summary>Interface for an event wrapper allowing dynamic hooking / unhooking.</summary>
public interface IManagedEvent
{
    /// <summary>Whether this event is hooked.</summary>
    bool IsHooked { get; }

    /// <summary>Whether this event is hooked for a specific splitscreen player.</summary>
    /// <param name="screenID">The player's screen id.</param>
    bool IsHookedForScreen(int screenID);

    /// <summary>Hook this event on the current screen.</summary>
    void Hook();

    /// <summary>Unhook this event on the current screen.</summary>
    void Unhook();
}