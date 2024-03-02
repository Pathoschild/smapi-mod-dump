/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using SAML.Elements;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Events
{
    public delegate void GamePadButtonEventHandler(object sender, GamePadButtonEventArgs e);

    public class GamePadButtonEventArgs(int x, int y, Buttons button) : EventArgs
    {
        /// <summary>
        /// The horizontal position of the cursor at the time the event was fired
        /// </summary>
        public int X { get; } = x;

        /// <summary>
        /// The vertical position of the cursor at the time the event was fired
        /// </summary>
        public int Y { get; } = y;

        /// <summary>
        /// The button that was pressed
        /// </summary>
        public Buttons Button { get; } = button;

        public GamePadButtonEventArgs(Buttons button) : this(Game1.getMouseX(), Game1.getMouseY(), button) { }

        public bool IsOverElement(Element element, bool includeMargin = false, bool includePadding = false) => element.GetBounds(includeMargin, includePadding).Contains(X, Y);
    }
}
