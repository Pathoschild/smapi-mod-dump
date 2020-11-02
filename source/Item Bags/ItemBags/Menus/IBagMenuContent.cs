/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags.Menus
{
    public interface IBagMenuContent : IGamepadControllable
    {
        ItemBagMenu IBM { get; }
        ItemBag Bag { get; }
        Item HoveredItem { get; }
        void UpdateHoveredItem(CursorMovedEventArgs e);

        int Padding { get; }
        Rectangle RelativeBounds { get; }
        Rectangle Bounds { get; }
        Point TopLeftScreenPosition { get; }
        void SetTopLeft(Point Point);
        void InitializeLayout(int ResizeIteration);
        bool CanResize { get; }

        void OnClose();

        void OnMouseMoved(CursorMovedEventArgs e);
        void OnMouseButtonPressed(ButtonPressedEventArgs e);
        void OnMouseButtonReleased(ButtonReleasedEventArgs e);
        void Update(UpdateTickedEventArgs e);

        void Draw(SpriteBatch b);
        void DrawToolTips(SpriteBatch b);
    }
}
