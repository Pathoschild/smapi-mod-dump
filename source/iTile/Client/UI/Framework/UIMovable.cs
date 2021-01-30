/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using iTile.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace iTile.Client.UI.Framework
{
    public abstract class UIMovable : UIElement
    {
        public bool useMouseLeftToMove;
        public bool canBeMoved = true;
        public bool moveMode;
        private Point oldMousePos;
        private Point newMousePos;
        private Point delta;

        public UIMovable(string name, Rectangle transform, UIElement parent = null) : base(name, transform, parent)
        {
            processClicks = true;
        }

        protected override void Update()
        {
            base.Update();
            if (moveMode)
            {
                Move();
            }
            else
            {
                oldMousePos = Math.NegativePoint();
            }
        }

        private void Move()
        {
            newMousePos = Game1.getMousePosition();
            if (oldMousePos.X == -1 || oldMousePos.Y == -1)
            {
                oldMousePos = newMousePos;
                return;
            }
            delta = Math.GetDelta(oldMousePos, newMousePos);
            oldMousePos = newMousePos;
            transform.X += delta.X;
            transform.Y += delta.Y;
        }

        protected override void OnButtonPressed(ButtonPressedEventArgs e)
        {
            base.OnButtonPressed(e);
            if (GetPressed(e.Button))
                moveMode = true;
        }

        protected override void OnButtonReleased(ButtonReleasedEventArgs e)
        {
            base.OnButtonReleased(e);
            if (RightButtonUsed(e.Button))
                moveMode = false;
        }

        private bool GetPressed(SButton button)
        {
            return show && canBeMoved && MouseHovered() && RightButtonUsed(button);
        }

        private bool RightButtonUsed(SButton button)
        {
            return button == (useMouseLeftToMove ? SButton.MouseLeft : SButton.MouseRight);
        }
    }
}
