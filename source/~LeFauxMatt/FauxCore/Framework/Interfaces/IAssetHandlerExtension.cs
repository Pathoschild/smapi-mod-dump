/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Interfaces;

using StardewMods.FauxCore.Common.Interfaces.Assets;
using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;

/// <summary>Extended Asset Handler methods for FauxCore.</summary>
public interface IAssetHandlerExtension : IAssetHandler
{
    /// <summary>Adds a button asset for the icon.</summary>
    /// <param name="icon">The icon.</param>
    public void AddAsset(IIcon icon);
}