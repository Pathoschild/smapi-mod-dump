/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.gpu
{
    public class Lcdc : IAddressSpace
    {
        private int _value = 0x91;

        public bool IsBgAndWindowDisplay() => (this._value & 0x01) != 0;
        public bool IsObjDisplay() => (this._value & 0x02) != 0;
        public int GetSpriteHeight() => (this._value & 0x04) == 0 ? 8 : 16;
        public int GetBgTileMapDisplay() => (this._value & 0x08) == 0 ? 0x9800 : 0x9c00;
        public int GetBgWindowTileData() => (this._value & 0x10) == 0 ? 0x9000 : 0x8000;
        public bool IsBgWindowTileDataSigned() => (this._value & 0x10) == 0;
        public bool IsWindowDisplay() => (this._value & 0x20) != 0;
        public int GetWindowTileMapDisplay() => (this._value & 0x40) == 0 ? 0x9800 : 0x9c00;
        public bool IsLcdEnabled() => (this._value & 0x80) != 0;
        public bool Accepts(int address) => address == 0xff40;

        public void SetByte(int address, int val) => this._value = val;
        public int GetByte(int address) => this._value;
        public void Set(int val) => this._value = val;
        public int Get() => this._value;
    }
}