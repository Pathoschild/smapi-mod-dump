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

/// <summary>Interface for an event wrapper allowing dynamic enabling/disabling of events.</summary>
internal interface IEvent
{
    public bool IsEnabled { get; }

    /// <summary>Enable this event on the current screen.</summary>
    public void Enable();

    /// <summary>Disable this event on the current screen.</summary>
    public void Disable();
}