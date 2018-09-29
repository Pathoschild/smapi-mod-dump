using Microsoft.Xna.Framework;
using System;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A simple clickable widget.
    /// </summary>
    public abstract class Button : Widget
    {
        public event Action OnPress;

        public override bool ReceiveLeftClick(Point point)
        {
            OnPress?.Invoke();
            return true;
        }
    }
}