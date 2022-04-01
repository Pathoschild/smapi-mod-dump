/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AngeloC3/StardewMods
**
*************************************************/

using System.Runtime.InteropServices;

namespace StardewMediaKeys
{
    internal class KeyClicker
    {
        // Fields related to keyboard events
        private const int KEYEVENTF_EXTENTEDKEY = 0;
        private const int KEYEVENTF_KEYUP = 0;
        /// <summary>Hexidecimal value for the Next Track key</summary>
        public byte NEXT = 0xB0;
        /// <summary>Hexidecimal value for Play/Pause key key</summary>
        public byte PPKEY = 0xB3;
        /// <summary>Hexidecimal value for Previous Track key</summary>
        public byte PREV = 0xB1;

        public KeyClicker() { }

        /// <summary>Simulates the input of a specified key.</summary>
        /// <param name="key">The hexadecimal value of the key to be pressed.</param>
        public void keyClick(byte key)
        {
            keybd_event(key, KEYEVENTF_KEYUP, KEYEVENTF_EXTENTEDKEY, System.IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        /// <summary>Event that simulates a key input with specifications </summary>
        private static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, System.IntPtr extraInfo);
    }
}
