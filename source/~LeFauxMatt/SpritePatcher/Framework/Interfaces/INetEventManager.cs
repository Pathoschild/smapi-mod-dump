/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Interfaces;

/// <summary>Represents a service for managing net field events.</summary>
public interface INetEventManager
{
    /// <summary>Subscribes a target object to a field change event.</summary>
    /// <param name="target">The object that will be subscribed.</param>
    /// <param name="source">The field with the event.</param>
    /// <param name="eventName">The name of the event.</param>
    public void Subscribe(ISprite target, object source, string eventName);
}