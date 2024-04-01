/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/coop-cursor
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Coop_Cursor
{
    public class MouseHook
    {
        private MouseState xnaState = default(MouseState);

        private POINT cursorPos = new POINT();
        private int scrollWheel = 0;
        private ButtonState leftButton = ButtonState.Released;
        private ButtonState middleButton = ButtonState.Released;
        private ButtonState rightButton = ButtonState.Released;
        private ButtonState xButton1 = ButtonState.Released;
        private ButtonState xButton2 = ButtonState.Released;

        public void Initialize()
        {
            ModEntry._Helper.Events.GameLoop.UpdateTicking += updateState;
        }

        private void updateState(object sender, UpdateTickingEventArgs e)
        {
            xnaState = Mouse.GetState();

            leftButton = GetKeyState((int)VK.LBUTTON) == 0 ? ButtonState.Released : ButtonState.Pressed;
            middleButton = GetKeyState((int)VK.MBUTTON) == 0 ? ButtonState.Released : ButtonState.Pressed;
            rightButton = GetKeyState((int)VK.RBUTTON) == 0 ? ButtonState.Released : ButtonState.Pressed;
            xButton1 = GetKeyState((int)VK.XBUTTON1) == 0 ? ButtonState.Released : ButtonState.Pressed;
            xButton2 = GetKeyState((int)VK.XBUTTON2) == 0 ? ButtonState.Released : ButtonState.Pressed;

        }

        public MouseState getState()
        {
            return new MouseState(xnaState.X, xnaState.Y, xnaState.ScrollWheelValue, leftButton, middleButton, rightButton, xButton1, xButton2);
        }

        private enum VK
        {
            LBUTTON = 0x01,
            RBUTTON = 0x02,
            MBUTTON = 0x04,
            XBUTTON1 = 0x05,
            XBUTTON2 = 0x06,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int VirtualKeyPressed);
        [DllImport("user32.dll")]
        static extern short GetKeyState(int VirtualKeyPressed);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
}

