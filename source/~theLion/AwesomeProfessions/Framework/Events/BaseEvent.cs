/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace TheLion.Stardew.Professions.Framework.Events;

/// <summary>Base implementation for dynamic events.</summary>
internal abstract class BaseEvent : IEvent
{
    /// <inheritdoc />
    public abstract void Hook();

    /// <inheritdoc />
    public abstract void Unhook();
}