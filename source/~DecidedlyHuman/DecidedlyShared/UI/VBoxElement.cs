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
using StardewValley;

namespace DecidedlyShared.Ui;

public class VBoxElement : ContainerElement
{
    private bool resizeToFitElements;
    private int maximumHeight;
    private Orientation orientation;
    private Alignment alignment;
    private int childSpacing;

    public VBoxElement(string name, Rectangle bounds, Logger logger, DrawableType type = DrawableType.Texture, Texture2D? texture = null, Rectangle? sourceRect = null,
        Color? color = null,
        int topEdgeSize = 4, int bottomEdgeSize = 4, int leftEdgeSize = 4, int rightEdgeSize = 4, int childSpacing = 4)
        : base(name, bounds, logger, type, texture, sourceRect, color,
            topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize)
    {
        this.childSpacing = childSpacing;
        this.bounds = bounds;
    }

    internal override void OrganiseChildren()
    {
        int widestChild = 0;
        int totalWidth;
        int cumulativeChildHeight = 0;
        int totalHeight;

        // Figure out our widest child element, and total height.
        foreach (UiElement child in this.childElements)
        {
            if (child.bounds.Width > widestChild)
                widestChild = child.bounds.Width;

            cumulativeChildHeight += child.bounds.Height + this.childSpacing;
        }

        totalWidth = widestChild + this.leftEdgeSize + this.rightEdgeSize;
        totalHeight = cumulativeChildHeight + this.topEdgeSize + this.bottomEdgeSize - this.childSpacing;

        // if (this.bounds.Width < totalWidth)
        this.bounds.Width = totalWidth;

        // if (this.bounds.Height < totalHeight)
        this.bounds.Height = totalHeight;

        int widthModulo = this.bounds.Width % 4;
        int heightModulo = this.bounds.Height % 4;

        this.bounds.Width -= widthModulo * 4;
        this.bounds.Height -= heightModulo * 4;

        Vector2 centrePosition = Utility.getTopLeftPositionForCenteringOnScreen(this.Width, this.Height);
        this.X = (int)centrePosition.X;
        this.Y = (int)centrePosition.Y;

        int previousChildBottom = 0;

        // Now, we want to lay out our children vertically.
        foreach (UiElement child in this.childElements)
        {
            if (previousChildBottom == 0)
            {
                child.bounds.Y = this.bounds.Y + this.topEdgeSize;
            }
            else
            {
                child.bounds.Y = previousChildBottom + this.childSpacing;
            }

            previousChildBottom = child.bounds.Bottom;
        }

        // Then, for now, just horizontally centre them.
        foreach (UiElement child in this.childElements)
        {
            child.bounds.X = this.bounds.X + this.bounds.Width / 2 - child.bounds.Width / 2;
        }
    }
}
