/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Interfaces.Cache;
#else
namespace StardewMods.Common.Interfaces.Cache;
#endif

/// <summary>Represents a cached value.</summary>
internal interface ICachedValue
{
    /// <summary>Gets the original value.</summary>
    string OriginalValue { get; }
}