/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using DeluxeJournal.Task;

namespace DeluxeJournal.Menus.Components
{
    public interface ITaskParameterComponent : IClickableComponentSupplier
    {
        /// <summary>Clickable component for the parameter value input area.</summary>
        ClickableComponent ClickableComponent { get; }

        /// <summary>Backing parameter whose value is to be set and displayed.</summary>
        TaskParameter Parameter { get; set; }

        /// <summary>Parameter display label.</summary>
        string Label { get; set; }

        /// <summary>Recalculate bounds after changes have been made that may affect component size/position.</summary>
        void RecalculateBounds();

        /// <summary>Try to do a hover action given the mouse position.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        void TryHover(int x, int y);

        /// <summary>Receive a mouse click.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        /// <param name="playSound">Allow a sound to be played (if any at all).</param>
        void ReceiveLeftClick(int x, int y, bool playSound = true);

        /// <summary>Draw the parameter value input component.</summary>
        void Draw(SpriteBatch b);
    }
}
