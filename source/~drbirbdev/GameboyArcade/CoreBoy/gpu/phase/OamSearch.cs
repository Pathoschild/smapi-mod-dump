/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using CoreBoy.memory;

namespace CoreBoy.gpu.phase
{
    public class OamSearch : IGpuPhase
    {
        private enum State
        {
            ReadingY,
            ReadingX
        }

        public sealed class SpritePosition
        {

            private readonly int _x;
            private readonly int _y;
            private readonly int _address;

            public SpritePosition(int x, int y, int address)
            {
                this._x = x;
                this._y = y;
                this._address = address;
            }

            public int GetX()
            {
                return this._x;
            }

            public int GetY()
            {
                return this._y;
            }

            public int GetAddress()
            {
                return this._address;
            }
        }

        private readonly IAddressSpace _oemRam;
        private readonly MemoryRegisters _registers;
        private readonly SpritePosition[] _sprites;
        private readonly Lcdc _lcdc;
        private int _spritePosIndex;
        private State _state;
        private int _spriteY;
        private int _spriteX;
        private int _i;

        public OamSearch(IAddressSpace oemRam, Lcdc lcdc, MemoryRegisters registers)
        {
            this._oemRam = oemRam;
            this._registers = registers;
            this._lcdc = lcdc;
            this._sprites = new SpritePosition[10];
        }

        public OamSearch Start()
        {
            this._spritePosIndex = 0;
            this._state = State.ReadingY;
            this._spriteY = 0;
            this._spriteX = 0;
            this._i = 0;
            for (var j = 0; j < this._sprites.Length; j++)
            {
                this._sprites[j] = null;
            }

            return this;
        }


        public bool Tick()
        {
            var spriteAddress = 0xfe00 + (4 * this._i);
            switch (this._state)
            {
                case State.ReadingY:
                    this._spriteY = this._oemRam.GetByte(spriteAddress);
                    this._state = State.ReadingX;
                    break;

                case State.ReadingX:
                    this._spriteX = this._oemRam.GetByte(spriteAddress + 1);
                    if (this._spritePosIndex < this._sprites.Length && Between(this._spriteY, this._registers.Get(GpuRegister.Ly) + 16,
                            this._spriteY + this._lcdc.GetSpriteHeight()))
                    {
                        this._sprites[this._spritePosIndex++] = new SpritePosition(this._spriteX, this._spriteY, spriteAddress);
                    }

                    this._i++;
                    this._state = State.ReadingY;
                    break;
            }

            return this._i < 40;
        }

        public SpritePosition[] GetSprites()
        {
            return this._sprites;
        }

        private static bool Between(int from, int x, int to)
        {
            return from <= x && x < to;
        }
    }
}