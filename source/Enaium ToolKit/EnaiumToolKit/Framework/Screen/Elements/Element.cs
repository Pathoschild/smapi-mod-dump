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

namespace EnaiumToolKit.Framework.Screen.Elements;

public abstract class Element
{
    public bool Hovered;
    public bool Visibled;
    public bool Enabled;
    public int Width;
    public int Height;
    public string Title;
    public string Description;
        
    public Action OnLeftClicked = () => { };
    public Action OnLeftReleased = () => { };
    public Action OnRightClicked = () => { };

    public Element(string title, string description)
    {
        Title = title;
        Description = description;
        Width = 800;
        Height = 75;
        Hovered = false;
        Visibled = true;
        Enabled = true;
    }
        
    public abstract void Render(SpriteBatch b, int x, int y);
        
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
}