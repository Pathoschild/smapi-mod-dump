/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;
using Microsoft.Xna.Framework;

namespace ConvenientChests.CategorizeChests.Interface.Widgets
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