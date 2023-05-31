/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace DecidedlyShared.Ui;

public class HBoxElement : ContainerElement
{
    private bool resizeToFitElements;
    private int maximumHeight;
    private Orientation orientation;
    private Alignment alignment;
    private int childSpacing;

    public HBoxElement(string name, Rectangle bounds, Logger logger, DrawableType type = DrawableType.Texture, Texture2D? texture = null, Rectangle? sourceRect = null,
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
        int tallestChild = 0;
        int totalWidth;
        int cumulativeChildWidth = 0;
        int totalHeight;

        // Figure out our tallest child element, and total width.
        foreach (UiElement child in this.childElements)
        {
            if (child.bounds.Height > tallestChild)
                tallestChild = child.bounds.Height;

            cumulativeChildWidth += child.bounds.Width + this.childSpacing;
        }

        totalHeight = tallestChild + this.topEdgeSize + this.bottomEdgeSize;
        totalWidth = cumulativeChildWidth + this.leftEdgeSize + this.rightEdgeSize - this.childSpacing;

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

        int previousChildRight = 0;

        // Now, we want to lay out our children horizontally.
        foreach (UiElement child in this.childElements)
        {
            if (previousChildRight == 0)
            {
                child.bounds.X = this.bounds.X + this.leftEdgeSize;
            }
            else
            {
                child.bounds.X = previousChildRight + this.childSpacing;
            }

            previousChildRight = child.bounds.Right;
        }

        // Then, for now, just vertically centre them.
        foreach (UiElement child in this.childElements)
        {
            child.bounds.Y = this.bounds.Y + this.bounds.Height / 2 - child.bounds.Height / 2;
        }
    }
}
