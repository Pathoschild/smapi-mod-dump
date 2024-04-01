/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace DecidedlyShared.Ui;

public class UiElement
{
    internal string elementName;
    internal Rectangle bounds;
    internal Rectangle sourceRect;
    internal Texture2D texture;
    internal Color textureTint;
    internal int topEdgeSize, bottomEdgeSize, leftEdgeSize, rightEdgeSize;
    internal int scale;
    internal UiElement? parent;
    internal bool drawBox;
    internal DrawableType drawableType;
    internal Action? leftClickCallback;
    internal Action? rightClickCallback;
    internal bool drawShadow;
    internal Logger logger;

    public bool DrawBox
    {
        get { return this.drawBox; }
    }

    public Vector2 TopLeftCorner
    {
        get => new Vector2(this.bounds.Left, this.bounds.Top);
    }

    public Vector2 BottomLeftCorner
    {
        get => new Vector2(this.bounds.Left, this.bounds.Bottom);
    }

    public Vector2 TopRightCorner
    {
        get => new Vector2(this.bounds.Right, this.bounds.Top);
    }

    public Vector2 BottomRightCorner
    {
        get => new Vector2(this.bounds.Right, this.bounds.Bottom);
    }

    /// <summary>
    /// Sets a new width for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int Width
    {
        get => this.bounds.Width;
        set
        {
            this.bounds = new Rectangle(
                this.bounds.X,
                this.bounds.Y,
                value,
                this.bounds.Height);


        }
    }

    /// <summary>
    /// Sets a new height for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int Height
    {
        get => this.bounds.Height;
        set
        {
            this.bounds = new Rectangle(
                this.bounds.X,
                this.bounds.Y,
                this.bounds.Width,
                value);


        }
    }

    /// <summary>
    /// Sets a new X co-ordinate for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int X
    {
        get => this.bounds.X;
        set
        {
            this.bounds = new Rectangle(
                value,
                this.bounds.Y,
                this.bounds.Width,
                this.bounds.Height);


        }
    }

    /// <summary>
    /// Sets a new Y co-ordinate for this <see cref="UiElement"/>'s bounds.
    /// </summary>
    public int Y
    {
        get => this.bounds.Y;
        set
        {
            this.bounds = new Rectangle(
                this.bounds.X,
                value,
                this.bounds.Width,
                this.bounds.Height);


        }
    }

    public UiElement(string name, Rectangle bounds, Logger logger, DrawableType type = DrawableType.Texture, Texture2D texture = null, Rectangle? sourceRect = null,
        Color? color = null, bool drawShadow = false,
        int topEdgeSize = 16, int bottomEdgeSize = 12, int leftEdgeSize = 12, int rightEdgeSize = 16, int scale = 4)
    {
        this.elementName = name;
        this.texture = texture;
        this.bounds = bounds;
        if (sourceRect.HasValue) this.sourceRect = sourceRect.Value;
        this.drawableType = type;
        this.scale = scale;
        this.drawShadow = drawShadow;
        this.logger = logger;

        if (sourceRect.HasValue)
        {
            this.bounds.Width = sourceRect.Value.Width;
            this.bounds.Height = sourceRect.Value.Height;
        }

        if (this.drawableType == DrawableType.SlicedBox && this.texture == null)
        {
            // In this situation, we need sane defaults.
            this.texture = Game1.menuTexture;
            this.sourceRect = new Rectangle(0, 256, 60, 60);
        }

        if (color.HasValue)
            this.textureTint = color.Value;
        else
            this.textureTint = Color.White;

        this.topEdgeSize = topEdgeSize;
        this.bottomEdgeSize = bottomEdgeSize;
        this.leftEdgeSize = leftEdgeSize;
        this.rightEdgeSize = rightEdgeSize;

        this.bounds = new Rectangle()
        {
            X = this.bounds.X,
            Y = this.bounds.Y,
            Width = this.bounds.Width * scale,
            Height = this.bounds.Height * scale
        };
    }

    public void RegisterLeftClickCallback(Action left)
    {
        this.leftClickCallback = left;
    }

    public void RegisterRightClickCallback(Action right)
    {
        this.rightClickCallback = right;
    }

    internal void UpdatePosition(int xPos, int yPos)
    {
        this.BoundsChanged();
    }

    internal void UpdateSize(int width, int height)
    {
        this.BoundsChanged();
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (this.drawableType == DrawableType.Texture)
        {
            if (this.texture is null)
            {
                return;
            }

            if (this.drawShadow)
                spriteBatch.Draw(this.texture, new Rectangle(this.bounds.X + 4, this.bounds.Y + 4, this.bounds.Width, this.bounds.Height), this.sourceRect, this.textureTint);

            spriteBatch.Draw(this.texture, this.bounds, this.sourceRect, this.textureTint);
        }
        else if (this.drawableType == DrawableType.SlicedBox)
        {
            Utils.DrawBox(
                spriteBatch,
                this.texture,
                this.sourceRect,
                this.bounds,
                this.topEdgeSize,
                this.leftEdgeSize,
                this.rightEdgeSize,
                this.bottomEdgeSize
                );
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Color colourTint)
    {
        if (this.drawableType == DrawableType.Texture)
        {
            if (this.drawShadow)
                spriteBatch.Draw(this.texture, new Rectangle(this.bounds.X + 4, this.bounds.Y + 4, this.bounds.Width, this.bounds.Height), this.sourceRect, Color.Black);

            spriteBatch.Draw(this.texture, this.bounds, this.sourceRect, colourTint);
        }
        else if (this.drawableType == DrawableType.SlicedBox)
        {
            Utils.DrawBox(
                spriteBatch,
                this.texture,
                this.sourceRect,
                this.bounds,
                this.topEdgeSize,
                this.leftEdgeSize,
                this.rightEdgeSize,
                this.bottomEdgeSize
                );
        }
    }

    internal void BoundsChanged()
    {
    }

    public virtual void ReceiveLeftClick(int x, int y)
    {
        if (this.leftClickCallback == null)
            return;

        this.leftClickCallback.Invoke();
    }

    public virtual void ReceiveRightClick(int x, int y)
    {
        if (this.rightClickCallback == null)
            return;

        this.rightClickCallback.Invoke();
    }

    public virtual void ReceiveScrollWheel(int direction)
    {

    }

    public void SetParent(UiElement parent)
    {
        this.parent = parent;
    }

    internal virtual void OrganiseChildren()
    {

    }
}
