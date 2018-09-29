using System;
using Microsoft.Xna.Framework;

namespace SkillPrestige.InputHandling
{
    /// <summary>
    /// Arguments passed for the event of the mouse being moved.
    /// </summary>
    internal class MouseMoveEventArguments : EventArgs
    {
        public Point LastPoint { get; } 
        public Point CurrentPoint { get; }

        public MouseMoveEventArguments(Point lastPoint, Point currentPoint)
        {
            LastPoint = lastPoint;
            CurrentPoint = currentPoint;
        }
    }
}
