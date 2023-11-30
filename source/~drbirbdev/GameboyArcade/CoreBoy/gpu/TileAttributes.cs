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
    public class TileAttributes
    {
        public static TileAttributes Empty { get; }
        private static readonly TileAttributes[] Attributes;
        private readonly int _value;

        static TileAttributes()
        {
            Attributes = new TileAttributes[256];

            for (var i = 0; i < 256; i++)
            {
                Attributes[i] = new TileAttributes(i);
            }

            Empty = Attributes[0];
        }

        private TileAttributes(int value) => this._value = value;
        public static TileAttributes ValueOf(int value) => Attributes[value];
        public bool IsPriority() => (this._value & (1 << 7)) != 0;
        public bool IsYFlip() => (this._value & (1 << 6)) != 0;
        public bool IsXFlip() => (this._value & (1 << 5)) != 0;
        public GpuRegister GetDmgPalette() => (this._value & (1 << 4)) == 0 ? GpuRegister.Obp0 : GpuRegister.Obp1;
        public int GetBank() => (this._value & (1 << 3)) == 0 ? 0 : 1;
        public int GetColorPaletteIndex() => this._value & 0x07;
    }
}