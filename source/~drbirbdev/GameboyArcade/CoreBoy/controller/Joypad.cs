/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Concurrent;
using CoreBoy.cpu;

namespace CoreBoy.controller
{
    public class Joypad : IAddressSpace
    {
        private readonly ConcurrentDictionary<Button, Button> _buttons = new ConcurrentDictionary<Button, Button>();
        private int _p1;

        public Joypad(InterruptManager interruptManager, IController controller)
        {
            controller.SetButtonListener(new JoyPadButtonListener(interruptManager, _buttons));
        }

        public bool Accepts(int address)
        {
            return address == 0xff00;
        }


        public void SetByte(int address, int value)
        {
            _p1 = value & 0b00110000;
        }

        public int GetByte(int address)
        {
            var result = _p1 | 0b11001111;
            foreach (var b in _buttons.Keys)
            {
                if ((b.Line & _p1) == 0)
                {
                    result &= 0xff & ~b.Mask;
                }
            }

            return result;
        }
    }
}