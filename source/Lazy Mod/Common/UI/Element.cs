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

namespace Common.UI;

public abstract class Element
{
    public abstract Vector2 LocalPosition { get; set; }
    public Vector2 Position => LocalPosition + (Parent?.Position ?? Vector2.Zero);
    public abstract int Width { get; }
    public abstract int Height { get; }
    private Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);
    
    public Container? Parent;

    public bool Hover;
    public Action<SpriteBatch>? OnHover = null;

    public Func<bool>? CheckHidden = null;

    public virtual void Update()
    {
        var isHidden = IsHidden();
        if (isHidden)
        {
            Hover = false;
            return;
        }
        
        var mousePosition = Game1.getMousePosition();
        Hover = Bounds.Contains(mousePosition);
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        OnHover?.Invoke(spriteBatch);
    }

    public bool IsHidden()
    {
        return CheckHidden is not null && CheckHidden.Invoke();
    }
}