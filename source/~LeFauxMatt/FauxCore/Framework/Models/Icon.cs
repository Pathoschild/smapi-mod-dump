/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FauxCore.Framework.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;
using StardewValley.Menus;

/// <inheritdoc />
internal sealed class Icon : IIcon
{
    private readonly Func<IIcon, IconStyle, int, int, float, string?, string?, ClickableTextureComponent> getComponent;
    private readonly Func<IIcon, IconStyle, Texture2D> getTexture;

    /// <summary>Initializes a new instance of the <see cref="Icon" /> class.</summary>
    /// <param name="getTexture">A function that returns the button texture.</param>
    /// <param name="getComponent">A function that return a new button.</param>
    public Icon(
        Func<IIcon, IconStyle, Texture2D> getTexture,
        Func<IIcon, IconStyle, int, int, float, string?, string?, ClickableTextureComponent> getComponent)
    {
        this.getTexture = getTexture;
        this.getComponent = getComponent;
    }

    /// <inheritdoc />
    public string UniqueId => $"{this.Source}/{this.Id}";

    /// <inheritdoc />
    public Rectangle Area { get; set; } = Rectangle.Empty;

    /// <inheritdoc />
    public string Id { get; set; } = string.Empty;

    /// <inheritdoc />
    public string Path { get; set; } = string.Empty;

    /// <inheritdoc />
    public string Source { get; set; } = string.Empty;

    /// <inheritdoc />
    public ClickableTextureComponent Component(
        IconStyle style,
        int x = 0,
        int y = 0,
        float scale = Game1.pixelZoom,
        string? name = null,
        string? hoverText = null) =>
        this.getComponent(this, style, x, y, scale, name, hoverText);

    /// <inheritdoc />
    public Texture2D Texture(IconStyle style) => this.getTexture(this, style);
}