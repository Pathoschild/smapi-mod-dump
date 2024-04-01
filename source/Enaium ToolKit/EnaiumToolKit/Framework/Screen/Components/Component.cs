/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace EnaiumToolKit.Framework.Screen.Components;

public abstract class Component
{
    public bool Hovered;
    public bool Visibled;
    public bool Enabled;

    public int X;
    public int Y;
    public int Width;
    public int Height;
    public string Title;
    public string Description;

    public Action OnLeftClicked = () => { };
    public Action OnLeftReleased = () => { };
    public Action OnRightClicked = () => { };

    public Component(string title, string description, int x, int y, int width, int height)
    {
        Title = title;
        Description = description;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Hovered = false;
        Visibled = true;
        Enabled = true;
    }

    public abstract void Render(SpriteBatch b);

    public virtual void MouseLeftClicked(int x, int y)
    {
        OnLeftClicked.Invoke();
    }

    public virtual void MouseLeftReleased(int x, int y)
    {
        OnLeftReleased.Invoke();
    }

    public virtual void MouseRightClicked(int x, int y)
    {
        OnRightClicked.Invoke();
    }

    public virtual void MouseScrollWheelAction(int direction)
    {
    }
}