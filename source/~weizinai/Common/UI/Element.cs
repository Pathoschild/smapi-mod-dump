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

namespace Common.UI;

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
    protected Vector2 Position => LocalPosition + (Parent?.Position ?? Vector2.Zero);
    public abstract int Width { get; }
    public abstract int Height { get; }
    private Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);
    private bool LeftClick => hover && leftClickGesture;

    public virtual void Update()
    {
        var isHidden = IsHidden();
        if (isHidden)
        {
            hover = false;
            leftClickGesture = false;
            return;
        }

        var mousePosition = Game1.getMousePosition();
        hover = Bounds.Contains(mousePosition);
        leftClickGesture = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed;
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        if (hover)
            OnHover?.Invoke(this, spriteBatch);
        else
            OffHover?.Invoke(this);
    }

    public virtual void ReceiveLeftClick()
    {
        if (IsHidden()) return;
        if (LeftClick) OnLeftClick?.Invoke();
    }

    public bool IsHidden()
    {
        return CheckHidden is not null && CheckHidden.Invoke();
    }
}