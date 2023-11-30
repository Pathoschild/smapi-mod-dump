/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;

namespace CoreBoy.sound
{
    public class PolynomialCounter
    {
        private int _i;
        private int _shiftedDivisor;

        public void SetNr43(int value)
        {
            var clockShift = value >> 4;

            var divisor = (value & 0b111) switch
            {
                0 => 8,
                1 => 16,
                2 => 32,
                3 => 48,
                4 => 64,
                5 => 80,
                6 => 96,
                7 => 112,
                _ => throw new InvalidOperationException()
            };

            this._shiftedDivisor = divisor << clockShift;
            this._i = 1;
        }

        public bool Tick()
        {
            if (--this._i == 0)
            {
                this._i = this._shiftedDivisor;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}