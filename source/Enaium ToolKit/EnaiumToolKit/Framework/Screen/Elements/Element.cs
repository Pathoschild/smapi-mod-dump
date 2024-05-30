/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace EnaiumToolKit.Framework.Screen.Elements;

public abstract class Element
{
    public bool Hovered;
    public bool Visibled;
    public bool Enabled;
    public bool Focused;
    public int Width;
    public int Height;
    public string? Title;
    public string? Description;

    public Action? OnLeftClicked = null;
    public Action? OnLeftReleased = null;
    public Action? OnRightClicked = null;

    public const int DefaultWidth = 800;
    public const int DefaultHeight = 75;

    protected Element(string? title, string? description)
    {
        Title = title;
        Description = description;
        Width = DefaultWidth;
        Height = DefaultHeight;
        Hovered = false;
        Visibled = true;
        Enabled = true;
    }

    public virtual void Render(SpriteBatch b, int x, int y)
    {
        Hovered = new Rectangle(x, y, Width, Height).Contains(Game1.getMouseX(), Game1.getMouseY());
    }

    public virtual void MouseLeftClicked(int x, int y)
    {
        OnLeftClicked?.Invoke();
    }

    public virtual void MouseLeftReleased(int x, int y)
    {
        OnLeftReleased?.Invoke();
    }

    public virtual void MouseRightClicked(int x, int y)
    {
        OnRightClicked?.Invoke();
    }

    public virtual void LostFocus(int x, int y)
    {
    }
}