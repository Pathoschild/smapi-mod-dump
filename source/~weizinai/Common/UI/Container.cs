/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace Common.UI;

public abstract class Container : Element
{
    public readonly List<Element> Children = new();

    public void AddChild(params Element[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent?.RemoveChild(element);
            Children.Add(element);
            element.Parent = this;
        }
    }

    private void RemoveChild(Element element)
    {
        if (element.Parent != this) throw new ArgumentException("Element must be a child of this container.");
        Children.Remove(element);
        element.Parent = null;
    }

    public override void Update()
    {
        base.Update();
        foreach (var element in Children) element.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        foreach (var element in Children) element.Draw(spriteBatch);
    }

    public override void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        foreach (var element in Children) element.PerformHoverAction(spriteBatch);
    }

    public override void ReceiveLeftClick()
    {
        if (IsHidden()) return;
        foreach (var element in Children) element.ReceiveLeftClick();
    }
}