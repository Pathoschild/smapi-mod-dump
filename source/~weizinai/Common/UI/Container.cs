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

namespace weizinai.StardewValleyMod.Common.UI;

public abstract class Container : Element
{
    public readonly List<Element> Children = new();

    public void AddChild(params Element[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent?.RemoveChild(element);
            this.Children.Add(element);
            element.Parent = this;
        }
    }

    private void RemoveChild(Element element)
    {
        if (element.Parent != this) throw new ArgumentException("Element must be a child of this container.");
        this.Children.Remove(element);
        element.Parent = null;
    }

    public override void Update()
    {
        base.Update();
        foreach (var element in this.Children) element.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (this.IsHidden()) return;
        foreach (var element in this.Children) element.Draw(spriteBatch);
    }

    public override void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (this.IsHidden()) return;
        foreach (var element in this.Children) element.PerformHoverAction(spriteBatch);
    }

    public override void ReceiveLeftClick()
    {
        if (this.IsHidden()) return;
        foreach (var element in this.Children) element.ReceiveLeftClick();
    }
}