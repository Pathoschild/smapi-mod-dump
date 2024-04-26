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

namespace Common.SpaceUI;

public abstract class Container : Element
{
    private readonly IList<Element> childrenImpl = new List<Element>();

    /// <summary>Whether to update <see cref="Children"/> when <see cref="Update"/> is called.</summary>
    protected bool UpdateChildren { get; set; } = true;
    
    private Element? renderLast;

    public Element? RenderLast
    {
        get => renderLast;
        set
        {
            renderLast = value;
            if (Parent is not null)
            {
                if (value is null) Parent.RenderLast = Parent.RenderLast == this ? null : this;
            }
        }
    }

    public Element[] Children => childrenImpl.ToArray();
    
    public void AddChild(Element element)
    {
        element.Parent?.RemoveChild(element);
        childrenImpl.Add(element);
        element.Parent = this;
    }

    public void RemoveChild(Element element)
    {
        if (element.Parent != this)
            throw new ArgumentException("Element must be a child of this container.");
        childrenImpl.Remove(element);
        element.Parent = null;
    }
    
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);
        if (UpdateChildren)
        {
            foreach (var element in childrenImpl)
                element.Update(isOffScreen);
        }
    }
    
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        foreach (var child in childrenImpl)
        {
            if (child == RenderLast)
                continue;
            child.Draw(b);
        }

        RenderLast?.Draw(b);
    }
}
