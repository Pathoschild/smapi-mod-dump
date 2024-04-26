/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Common.SpaceUI;

public class Table : Container
{
    private readonly List<Element[]> rows = new();

    private Vector2 sizeImpl;

    private const int RowPadding = 16;
    private int rowHeightImpl;
    private bool fixedRowHeight;
    private int contentHeight;
    
    public Vector2 Size
    {
        get => sizeImpl;
        set
        {
            sizeImpl = new Vector2(value.X, (int)(value.Y / RowHeight) * RowHeight);
            UpdateScrollbar();
        }
    }

    public int RowHeight
    {
        get => rowHeightImpl;
        set
        {
            rowHeightImpl = value + RowPadding;
            UpdateScrollbar();
        }
    }

    public int RowCount => rows.Count;

    public Scrollbar Scrollbar { get; }

    /// <inheritdoc />
    public override int Width => (int)Size.X;

    /// <inheritdoc />
    public override int Height => (int)Size.Y;


    /*********
     ** Public methods
     *********/
    public Table(bool fixedRowHeight = true)
    {
        this.fixedRowHeight = fixedRowHeight;
        UpdateChildren = false; // table will update children itself
        Scrollbar = new Scrollbar
        {
            LocalPosition = new Vector2(0, 0)
        };
        AddChild(Scrollbar);
    }

    public void AddRow(Element[] elements)
    {
        rows.Add(elements);
        int maxElementHeight = 0;
        foreach (var child in elements)
        {
            AddChild(child);
            maxElementHeight = Math.Max(maxElementHeight, child.Height);
        }
        contentHeight += fixedRowHeight ? RowHeight : maxElementHeight + RowPadding;
        UpdateScrollbar();
    }

    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);
        if (IsHidden(isOffScreen))
            return;

        int topPx = 0;
        foreach (var row in rows)
        {
            int maxElementHeight = 0;
            foreach (var element in row)
            {
                element.LocalPosition = new Vector2(element.LocalPosition.X, topPx - Scrollbar.TopRow * RowHeight);
                bool isChildOffScreen = isOffScreen || IsElementOffScreen(element);

                if (!isChildOffScreen || element is Label) // Labels must update anyway to get rid of hovertext on scrollwheel
                    element.Update(isOffScreen: isChildOffScreen);
                maxElementHeight = Math.Max(maxElementHeight, element.Height);
            }
            topPx += fixedRowHeight ? RowHeight : maxElementHeight + RowPadding;
        }

        if (topPx != contentHeight) {
            contentHeight = topPx;
            Scrollbar.Rows = PxToRow(contentHeight);
        }

        Scrollbar.Update();
    }

    public void ForceUpdateEvenHidden(bool isOffScreen = false)
    {
        int topPx = 0;
        foreach (var row in rows)
        {
            int maxElementHeight = 0;
            foreach (var element in row)
            {
                element.LocalPosition = new Vector2(element.LocalPosition.X, topPx - Scrollbar.ScrollPercent * rows.Count * RowHeight);
                bool isChildOffScreen = isOffScreen || IsElementOffScreen(element);

                element.Update(isOffScreen: isChildOffScreen);
                maxElementHeight = Math.Max(maxElementHeight, element.Height);
            }
            topPx += fixedRowHeight ? RowHeight : maxElementHeight + RowPadding;
        }
        contentHeight = topPx;
        Scrollbar.Update(isOffScreen);
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        // calculate draw area
        var backgroundArea = new Rectangle((int)Position.X - 32, (int)Position.Y - 32, (int)Size.X + 64, (int)Size.Y + 64);
        int contentPadding = 12;
        var contentArea = new Rectangle(backgroundArea.X + contentPadding, backgroundArea.Y + contentPadding, backgroundArea.Width - contentPadding * 2, backgroundArea.Height - contentPadding * 2);

        // draw background
        IClickableMenu.drawTextureBox(b, backgroundArea.X, backgroundArea.Y, backgroundArea.Width, backgroundArea.Height, Color.White);
        b.Draw(Game1.menuTexture, contentArea, new Rectangle(64, 128, 64, 64), Color.White); // Smoother gradient for the content area.

        // draw table contents
        // This uses a scissor rectangle to clip content taller than one row that might be
        // drawn past the bottom of the UI, like images or complex options.
        Element? renderLast = null;
        InScissorRectangle(b, contentArea, contentBatch =>
        {
            foreach (var row in rows)
            {
                foreach (var element in row)
                {
                    if (IsElementOffScreen(element))
                        continue;
                    if (element == RenderLast) {
                        renderLast = element;
                        continue;
                    }
                    element.Draw(contentBatch);
                }
            }
        });
        renderLast?.Draw(b);

        Scrollbar.Draw(b);
    }


    /*********
     ** Private methods
     *********/
    /// <summary>Get whether a child element is outside the table's current display area.</summary>
    /// <param name="element">The child element to check.</param>
    private bool IsElementOffScreen(Element element)
    {
        return
            element.Position.Y + element.Height < Position.Y
            || element.Position.Y > Position.Y + Size.Y;
    }

    private void UpdateScrollbar()
    {
        Scrollbar.LocalPosition = new Vector2(Size.X + 48, Scrollbar.LocalPosition.Y);
        Scrollbar.RequestHeight = (int)Size.Y;
        Scrollbar.Rows = PxToRow(contentHeight);
        Scrollbar.FrameSize = (int)(Size.Y / RowHeight);
    }

    private void InScissorRectangle(SpriteBatch spriteBatch, Rectangle area, Action<SpriteBatch> draw)
    {
        // render the current sprite batch to the screen
        spriteBatch.End();

        // start temporary sprite batch
        using SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice);
        GraphicsDevice device = Game1.graphics.GraphicsDevice;
        Rectangle prevScissorRectangle = device.ScissorRectangle;

        // render in scissor rectangle
        try
        {
            device.ScissorRectangle = area;
            contentBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);

            draw(contentBatch);

            contentBatch.End();
        }
        finally
        {
            device.ScissorRectangle = prevScissorRectangle;
        }

        // resume previous sprite batch
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
    }

    private int PxToRow(int px)
    {
        return (px + RowHeight - 1) / RowHeight;
    }
}