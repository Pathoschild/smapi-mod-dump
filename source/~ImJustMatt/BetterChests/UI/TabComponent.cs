/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.UI;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.FuryCore.Enums;
using StardewMods.FuryCore.Models.ClickableComponents;
using StardewValley;
using StardewValley.Menus;

/// <inheritdoc />
internal class TabComponent : CustomClickableComponent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TabComponent" /> class.
    /// </summary>
    /// <param name="component">The texture that is drawn for the tab.</param>
    /// <param name="tags">The context tags that determine what items are shown for this tab.</param>
    public TabComponent(ClickableTextureComponent component, IList<string> tags)
        : base(component, ComponentArea.Bottom, ComponentLayer.Below)
    {
        this.Tags = tags;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this tab is currently selected.
    /// </summary>
    public bool Selected { get; set; }

    /// <summary>
    ///     Gets this tab's list of context tags for filtering displayed items.
    /// </summary>
    public IList<string> Tags { get; }

    /// <inheritdoc />
    public override int Y { get; set; }

    /// <inheritdoc />
    public override void Draw(SpriteBatch spriteBatch)
    {
        var color = this.Selected ? Color.White : Color.Gray;
        this.Component.bounds.Y = this.Y + (this.Selected ? Game1.pixelZoom : 0) - 16;
        this.Component.draw(spriteBatch, color, 0.86f + this.Component.bounds.Y / 20000f);
    }

    /// <inheritdoc />
    public override void TryHover(int x, int y, float maxScaleIncrease = 0.1f)
    {
        // Do Nothing
    }
}