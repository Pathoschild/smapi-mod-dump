/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FindAnything.Framework.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.Common.Services.Integrations.FindAnything;

/// <inheritdoc />
internal sealed class SearchResult(
    string fullTerm,
    Texture2D texture,
    Rectangle sourceRect,
    Vector2 position) : ISearchResult
{
    /// <inheritdoc />
    public string EntityName { get; } = fullTerm;

    /// <inheritdoc />
    public Texture2D Texture { get; } = texture;

    /// <inheritdoc />
    public Rectangle SourceRect { get; } = sourceRect;

    /// <inheritdoc />
    public Vector2 Position { get; } = position;
}