/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using SAML.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Events
{
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    public class MouseButtonEventArgs(int x, int y, bool playSound = false) : EventArgs
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
        /// Whether or not the request was made with muted sounds
        /// </summary>
        public bool PlaySound { get; } = playSound;

        public bool IsOverElement(Element element, bool includeMargin = false, bool includePadding = false) => element.GetBounds(includeMargin, includePadding).Contains(X, Y);
    }
}
