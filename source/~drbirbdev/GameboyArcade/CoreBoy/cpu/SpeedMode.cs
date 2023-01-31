/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.cpu
{
    public class SpeedMode : IAddressSpace
    {
        private bool _currentSpeed;
        private bool _prepareSpeedSwitch;

        public bool Accepts(int address) => address == 0xff4d;
        public void SetByte(int address, int value) => _prepareSpeedSwitch = (value & 0x01) != 0;
        public int GetByte(int address) => (_currentSpeed ? (1 << 7) : 0) | (_prepareSpeedSwitch ? (1 << 0) : 0) | 0b01111110;

        public bool OnStop()
        {
            if (!_prepareSpeedSwitch) return false;
            
            _currentSpeed = !_currentSpeed;
            _prepareSpeedSwitch = false;
            return true;
        }

        public int GetSpeedMode() => _currentSpeed ? 2 : 1;
    }
}
