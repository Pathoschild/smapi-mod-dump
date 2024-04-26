/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Interfaces;

/// <summary>Represents a lock that is currently being held.</summary>
internal interface IServiceLock
{
    /// <summary>Release the lock.</summary>
    public void Release();
}