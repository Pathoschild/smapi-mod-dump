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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Events
{
    public delegate void KeyboardEventHandler(object sender, KeyboardEventArgs e);

    public class KeyboardEventArgs(Keys key, KeyboardState state) : EventArgs
    {
        /// <summary>
        /// The key that was pressed
        /// </summary>
        public Keys Key { get; } = key;

        /// <summary>
        /// The state of the keyboard at the time the event was fired
        /// </summary>
        public KeyboardState KeyboardState { get; } = state;

        /// <summary>
        /// Whether or not either shift key was pressed when the event was fired
        /// </summary>
        public bool IsShiftDown => KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);

        /// <summary>
        /// Whether or not either control key was pressed when the event was fired
        /// </summary>
        public bool IsCtrlDown => KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);

        /// <summary>
        /// Whether or not capslock was toggled when the event was fired
        /// </summary>
        public bool IsCapsDown => KeyboardState.CapsLock;

        /// <summary>
        /// Whether or not numlock was toggled when the event was fired
        /// </summary>
        public bool IsNumLockDown => KeyboardState.NumLock;

        public bool IsArrowKey => Key == Keys.Left || Key == Keys.Right || Key == Keys.Up || Key == Keys.Down || (!IsNumLockDown && (Key == Keys.NumPad4 || Key == Keys.NumPad6 || Key == Keys.NumPad8 || Key == Keys.NumPad2));

        public KeyboardEventArgs(Keys key) : this(key, Game1.GetKeyboardState()) { }
    }
}
