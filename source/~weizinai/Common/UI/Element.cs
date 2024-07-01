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
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace weizinai.StardewValleyMod.Common.UI;

public abstract class Element
{
    public Func<bool>? CheckHidden;

    private bool hover;

    private bool leftClickGesture;
    public Action<Element>? OffHover;
    public Action<Element, SpriteBatch>? OnHover;
    public Action? OnLeftClick;

    public Container? Parent;
    public Vector2 LocalPosition { get; set; }
    protected Vector2 Position => this.LocalPosition + (this.Parent?.Position ?? Vector2.Zero);
    public abstract int Width { get; }
    public abstract int Height { get; }
    private Rectangle Bounds => new((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
    private bool LeftClick => this.hover && this.leftClickGesture;

    public virtual void Update()
    {
        var isHidden = this.IsHidden();
        if (isHidden)
        {
            this.hover = false;
            this.leftClickGesture = false;
            return;
        }

        var mousePosition = Game1.getMousePosition();
        this.hover = this.Bounds.Contains(mousePosition);
        this.leftClickGesture = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed;
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (this.IsHidden()) return;
        if (this.hover)
            this.OnHover?.Invoke(this, spriteBatch);
        else
            this.OffHover?.Invoke(this);
    }

    public virtual void ReceiveLeftClick()
    {
        if (this.IsHidden()) return;
        if (this.LeftClick) this.OnLeftClick?.Invoke();
    }

    public bool IsHidden()
    {
        return this.CheckHidden is not null && this.CheckHidden.Invoke();
    }
}