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
using System.Diagnostics;
using System.IO;
using CoreBoy.cpu;

namespace CoreBoy.serial
{
    public class SerialPort : IAddressSpace
    {
        private readonly SerialEndpoint _serialEndpoint;
        private readonly InterruptManager _interruptManager;
        private readonly SpeedMode _speedMode;
        private int _sb;
        private int _sc;
        private bool _transferInProgress;
        private int _divider;

        public SerialPort(InterruptManager interruptManager, SerialEndpoint serialEndpoint, SpeedMode speedMode)
        {
            this._interruptManager = interruptManager;
            this._serialEndpoint = serialEndpoint;
            this._speedMode = speedMode;
        }

        public void Tick()
        {
            if (!this._transferInProgress)
            {
                return;
            }

            if (++this._divider >= Gameboy.TicksPerSec / 8192 / this._speedMode.GetSpeedMode())
            {
                this._transferInProgress = false;
                try
                {
                    this._sb = this._serialEndpoint.transfer(this._sb);
                }
                catch (IOException e)
                {
                    Debug.WriteLine($"Can't transfer byte {e}");
                    this._sb = 0xff;
                }

                this._interruptManager.RequestInterrupt(InterruptManager.InterruptType.Serial);
            }
        }

        public bool Accepts(int address)
        {
            return address == 0xff01 || address == 0xff02;
        }

        public void SetByte(int address, int value)
        {
            if (address == 0xff01)
            {
                this._sb = value;
            }
            else if (address == 0xff02)
            {
                this._sc = value;
                if ((this._sc & (1 << 7)) != 0)
                {
                    this.StartTransfer();
                }
            }
        }

        public int GetByte(int address)
        {
            if (address == 0xff01)
            {
                return this._sb;
            }
            else if (address == 0xff02)
            {
                return this._sc | 0b01111110;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private void StartTransfer()
        {
            this._transferInProgress = true;
            this._divider = 0;
        }
    }
}
