/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.FindAnything;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>Represents a result returned from a search.</summary>
public interface ISearchResult
{
    /// <summary>Gets the name of the matched entity.</summary>
    string EntityName { get; }

    /// <summary>Gets the texture to use for the result.</summary>
    Texture2D Texture { get; }

    /// <summary>Gets the source rectangle to use for the result.</summary>
    Rectangle SourceRect { get; }

    /// <summary>Gets the position of the result.</summary>
    Vector2 Position { get; }
}