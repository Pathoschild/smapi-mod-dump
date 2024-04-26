/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Interfaces;

/// <summary>Represents a cached object.</summary>
/// <typeparam name="T">The cached object type.</typeparam>
public interface ICachedObject<out T>
{
    /// <summary>Gets the value of the cached object.</summary>
    T Value { get; }
}

/// <summary>Represents a cached object.</summary>
public interface ICachedObject { }