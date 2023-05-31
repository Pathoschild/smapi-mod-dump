/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DecidedlyShared.Ui;

public class ContainerElement : UiElement
{
    internal List<UiElement> childElements = new List<UiElement>();
    internal int containerMargin;
    internal MenuBase parentMenu;

    public ContainerElement(string name, Rectangle bounds, Logger logger, DrawableType type = DrawableType.SlicedBox, Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null,
        int topEdgeSize = 16, int bottomEdgeSize = 12, int leftEdgeSize = 12, int rightEdgeSize = 16,
        int containerMargin = 4)
        : base(name, bounds, logger, type, texture, sourceRect, color, false,
            topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        this.bounds = bounds;
        if (color.HasValue)
            this.textureTint = color.Value;
        else
            this.textureTint = Color.White;

        this.containerMargin = containerMargin;
    }

    internal virtual void AddChild(UiElement child)
    {
        if (!this.childElements.Contains(child))
        {
            this.childElements.Add(child);
            child.parent = this;
        }

        this.OrganiseChildren();
    }

    internal virtual void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        foreach (UiElement child in this.childElements)
        {
            child.Draw(spriteBatch);
        }
    }

    public override void ReceiveLeftClick(int x, int y)
    {
        foreach (UiElement child in this.childElements)
        {
            child.ReceiveLeftClick(x, y);
        }

        base.ReceiveLeftClick(x, y);
    }

    public override void ReceiveRightClick(int x, int y)
    {
        foreach (UiElement child in this.childElements)
        {
            child.ReceiveRightClick(x, y);
        }

        base.ReceiveRightClick(x, y);
    }

    public void SetParent(MenuBase parent)
    {
        this.parentMenu = parent;
        this.OrganiseChildren();
    }

    internal override void OrganiseChildren()
    {
        this.parentMenu?.UpdateCloseButton(base.TopRightCorner);
    }
}
