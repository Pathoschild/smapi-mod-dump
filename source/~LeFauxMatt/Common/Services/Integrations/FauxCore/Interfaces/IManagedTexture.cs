/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.FauxCore;

using Microsoft.Xna.Framework.Graphics;

/// <summary>Represents a texture managed by <see cref="IThemeHelper" />.</summary>
public interface IManagedTexture
{
    /// <summary>Gets the asset name of the managed texture.</summary>
    public IAssetName Name { get; }

    /// <summary>Gets the current texture for the managed asset.</summary>
    public Texture2D Value { get; }
}