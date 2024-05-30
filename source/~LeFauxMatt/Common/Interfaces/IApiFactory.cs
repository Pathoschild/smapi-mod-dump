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
namespace StardewMods.FauxCore.Common.Interfaces;
#else
namespace StardewMods.Common.Interfaces;
#endif

/// <summary>Factory service for creating Api instances.</summary>
internal interface IApiFactory
{
    /// <summary>Creates a new instance of the Api.</summary>
    /// <param name="modInfo">Dependency used for accessing mod info.</param>
    /// <returns>Returns the api instance.</returns>
    public object CreateApi(IModInfo modInfo);
}