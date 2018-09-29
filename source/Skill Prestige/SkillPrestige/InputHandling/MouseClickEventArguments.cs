using System;
using Microsoft.Xna.Framework;

namespace SkillPrestige.InputHandling
{
    /// <summary>
    /// Arguments passed for the event of the mouse being clicked.
    /// </summary>
    internal class MouseClickEventArguments : EventArgs
    {
        public Point ClickPoint { get; } 
        public Point ReleasePoint { get; }

        public MouseClickEventArguments(Point clickPoint, Point releasePoint)
        {
            ClickPoint = clickPoint;
            ReleasePoint = releasePoint;
        }
    }
}
