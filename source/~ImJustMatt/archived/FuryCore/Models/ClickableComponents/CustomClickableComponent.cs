/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.FuryCore.Models.ClickableComponents;

using Common.Enums;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.FuryCore.Enums;
using StardewMods.FuryCore.Interfaces.ClickableComponents;
using StardewValley.Menus;

/// <inheritdoc />
public class CustomClickableComponent : IClickableComponent
{
    private int? _myId;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CustomClickableComponent" /> class.
    /// </summary>
    /// <param name="component">The clickable texture component.</param>
    /// <param name="area">The area of the screen to orient the component to.</param>
    /// <param name="layer">Whether the component will be drawn above or below the menu.</param>
    public CustomClickableComponent(ClickableTextureComponent component, ComponentArea area = ComponentArea.Custom, ComponentLayer layer = ComponentLayer.Above)
        : this(area)
    {
        this.Component = component;
        this.Layer = layer;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CustomClickableComponent" /> class.
    /// </summary>
    /// <param name="area">The area of the screen to orient the component to.</param>
    protected CustomClickableComponent(ComponentArea area)
    {
        this.Area = area;
        this.ComponentType = ComponentType.Custom;
    }

    /// <inheritdoc />
    public ComponentArea Area { get; }

    /// <inheritdoc />
    public ClickableTextureComponent Component { get; protected init; }

    /// <inheritdoc />
    public ComponentType ComponentType { get; }

    /// <inheritdoc />
    public virtual string HoverText
    {
        get => this.Component?.hoverText;
    }

    /// <inheritdoc />
    public int Id
    {
        get => this._myId ??= this.Component.myID = CustomClickableComponent.ComponentId++;
    }

    /// <inheritdoc />
    public bool IsVisible
    {
        get => this.Component.visible;
        set => this.Component.visible = value;
    }

    /// <inheritdoc />
    public ComponentLayer Layer { get; }

    /// <inheritdoc />
    public string Name
    {
        get => this.Component.name;
    }

    /// <inheritdoc />
    public virtual int X
    {
        get => this.Component.bounds.X;
        set => this.Component.bounds.X = value;
    }

    /// <inheritdoc />
    public virtual int Y
    {
        get => this.Component.bounds.Y;
        set => this.Component.bounds.Y = value;
    }

    private static int ComponentId { get; set; } = 69420;

    /// <inheritdoc />
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (this.IsVisible)
        {
            this.Component?.draw(spriteBatch);
        }
    }

    /// <inheritdoc />
    public virtual void TryHover(int x, int y, float maxScaleIncrease = 0.1f)
    {
        if (this.IsVisible)
        {
            this.Component?.tryHover(x, y, maxScaleIncrease);
        }
    }
}