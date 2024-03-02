/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using CoreBoy.gpu.phase;
using CoreBoy.memory;
using static CoreBoy.cpu.BitUtils;
using static CoreBoy.gpu.GpuRegister;

namespace CoreBoy.gpu
{
    public class Fetcher
    {
        private enum State
        {
            ReadTileId,
            ReadData1,
            ReadData2,
            Push,
            ReadSpriteTileId,
            ReadSpriteFlags,
            ReadSpriteData1,
            ReadSpriteData2,
            PushSprite
        }

        private static readonly int[] EmptyPixelLine = new int[8];

        private static readonly List<State> SpritesInProgress = new List<State> {
            State.ReadSpriteTileId,
            State.ReadSpriteFlags,
            State.ReadSpriteData1,
            State.ReadSpriteData2,
            State.PushSprite
        };

        private readonly IPixelFifo _fifo;
        private readonly IAddressSpace _videoRam0;
        private readonly IAddressSpace _videoRam1;
        private readonly IAddressSpace _oemRam;
        private readonly MemoryRegisters _r;
        private readonly Lcdc _lcdc;
        private readonly bool _gbc;

        private readonly int[] _pixelLine = new int[8];
        private State _state;
        private bool _fetchingDisabled;

        private int _mapAddress;
        private int _xOffset;

        private int _tileDataAddress;
        private bool _tileIdSigned;
        private int _tileLine;
        private int _tileId;
        private TileAttributes _tileAttributes;
        private int _tileData1;
        private int _tileData2;

        private int _spriteTileLine;
        private OamSearch.SpritePosition _sprite;
        private TileAttributes _spriteAttributes;
        private int _spriteOffset;
        private int _spriteOamIndex;

        private int _divider = 2;

        public Fetcher(IPixelFifo fifo, IAddressSpace videoRam0, IAddressSpace videoRam1, IAddressSpace oemRam, Lcdc lcdc,
            MemoryRegisters registers, bool gbc)
        {
            this._gbc = gbc;
            this._fifo = fifo;
            this._videoRam0 = videoRam0;
            this._videoRam1 = videoRam1;
            this._oemRam = oemRam;
            this._r = registers;
            this._lcdc = lcdc;
        }

        public void Init()
        {
            this._state = State.ReadTileId;
            this._tileId = 0;
            this._tileData1 = 0;
            this._tileData2 = 0;
            this._divider = 2;
            this._fetchingDisabled = false;
        }

        public void StartFetching(int mapAddress, int tileDataAddress, int xOffset, bool tileIdSigned, int tileLine)
        {
            this._mapAddress = mapAddress;
            this._tileDataAddress = tileDataAddress;
            this._xOffset = xOffset;
            this._tileIdSigned = tileIdSigned;
            this._tileLine = tileLine;
            this._fifo.Clear();

            this._state = State.ReadTileId;
            this._tileId = 0;
            this._tileData1 = 0;
            this._tileData2 = 0;
            this._divider = 2;
        }

        public void FetchingDisabled()
        {
            this._fetchingDisabled = true;
        }

        public void AddSprite(OamSearch.SpritePosition sprite, int offset, int oamIndex)
        {
            this._sprite = sprite;
            this._state = State.ReadSpriteTileId;
            this._spriteTileLine = this._r.Get(Ly) + 16 - sprite.GetY();
            this._spriteOffset = offset;
            this._spriteOamIndex = oamIndex;
        }

        public void Tick()
        {
            if (this._fetchingDisabled && this._state == State.ReadTileId)
            {
                if (this._fifo.GetLength() <= 8)
                {
                    this._fifo.Enqueue8Pixels(EmptyPixelLine, this._tileAttributes);
                }

                return;
            }

            if (--this._divider == 0)
            {
                this._divider = 2;
            }
            else
            {
                return;
            }

        stateSwitch:

            switch (this._state)
            {
                case State.ReadTileId:
                    this._tileId = this._videoRam0.GetByte(this._mapAddress + this._xOffset);

                    this._tileAttributes = this._gbc
                            ? TileAttributes.ValueOf(this._videoRam1.GetByte(this._mapAddress + this._xOffset))
                            : TileAttributes.Empty;

                    this._state = State.ReadData1;
                    break;

                case State.ReadData1:
                    this._tileData1 = this.GetTileData(this._tileId, this._tileLine, 0, this._tileDataAddress, this._tileIdSigned, this._tileAttributes, 8);
                    this._state = State.ReadData2;
                    break;

                case State.ReadData2:
                    this._tileData2 = this.GetTileData(this._tileId, this._tileLine, 1, this._tileDataAddress, this._tileIdSigned, this._tileAttributes, 8);
                    this._state = State.Push;
                    goto stateSwitch; // Sorry mum

                case State.Push:
                    if (this._fifo.GetLength() <= 8)
                    {
                        this._fifo.Enqueue8Pixels(this.Zip(this._tileData1, this._tileData2, this._tileAttributes.IsXFlip()), this._tileAttributes);
                        this._xOffset = (this._xOffset + 1) % 0x20;
                        this._state = State.ReadTileId;
                    }

                    break;

                case State.ReadSpriteTileId:
                    this._tileId = this._oemRam.GetByte(this._sprite.GetAddress() + 2);
                    this._state = State.ReadSpriteFlags;
                    break;

                case State.ReadSpriteFlags:
                    this._spriteAttributes = TileAttributes.ValueOf(this._oemRam.GetByte(this._sprite.GetAddress() + 3));
                    this._state = State.ReadSpriteData1;
                    break;

                case State.ReadSpriteData1:
                    if (this._lcdc.GetSpriteHeight() == 16)
                    {
                        this._tileId &= 0xfe;
                    }

                    this._tileData1 = this.GetTileData(this._tileId, this._spriteTileLine, 0, 0x8000, false, this._spriteAttributes,
                        this._lcdc.GetSpriteHeight());
                    this._state = State.ReadSpriteData2;
                    break;

                case State.ReadSpriteData2:
                    this._tileData2 = this.GetTileData(this._tileId, this._spriteTileLine, 1, 0x8000, false, this._spriteAttributes,
                        this._lcdc.GetSpriteHeight());
                    this._state = State.PushSprite;
                    break;

                case State.PushSprite:
                    this._fifo.SetOverlay(this.Zip(this._tileData1, this._tileData2, this._spriteAttributes.IsXFlip()), this._spriteOffset,
                        this._spriteAttributes, this._spriteOamIndex);
                    this._state = State.ReadTileId;
                    break;
            }
        }

        private int GetTileData(int tileId, int line, int byteNumber, int tileDataAddress, bool signed,
            TileAttributes attr, int tileHeight)
        {
            int effectiveLine = attr.IsYFlip() ? tileHeight - 1 - line : line;
            int tileAddress = signed ? tileDataAddress + (ToSigned(tileId) * 0x10) : tileDataAddress + (tileId * 0x10);

            var videoRam = (attr.GetBank() == 0 || !this._gbc) ? this._videoRam0 : this._videoRam1;
            return videoRam.GetByte(tileAddress + (effectiveLine * 2) + byteNumber);
        }

        public bool SpriteInProgress()
        {
            return SpritesInProgress.Contains(this._state);
        }

        public int[] Zip(int data1, int data2, bool reverse)
        {
            return Zip(data1, data2, reverse, this._pixelLine);
        }

        public static int[] Zip(int data1, int data2, bool reverse, int[] pixelLine)
        {
            for (int i = 7; i >= 0; i--)
            {
                int mask = 1 << i;
                int p = (2 * ((data2 & mask) == 0 ? 0 : 1)) + ((data1 & mask) == 0 ? 0 : 1);
                if (reverse)
                {
                    pixelLine[i] = p;
                }
                else
                {
                    pixelLine[7 - i] = p;
                }
            }

            return pixelLine;
        }
    }
}