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
using System.Collections.Generic;
using System.Text;

namespace CoreBoy.gpu
{
    public class ColorPalette : IAddressSpace
    {
        private readonly int IndexAddress;
        private readonly int DataAddress;
        private int Index;
        private bool AutoIncrement;

        private readonly List<List<int>> Palettes;

        public ColorPalette(int offset)
        {
            this.Palettes = new List<List<int>>();
            for (int x = 0; x < 8; x++)
            {
                var row = new List<int>(4);
                for (int y = 0; y < 4; y++)
                {
                    row.Add(0);
                }

                this.Palettes.Add(row);
            }

            this.IndexAddress = offset;
            this.DataAddress = offset + 1;
        }

        public bool Accepts(int address) => address == this.IndexAddress || address == this.DataAddress;

        public void SetByte(int address, int value)
        {
            if (address == this.IndexAddress)
            {
                this.Index = value & 0x3f;
                this.AutoIncrement = (value & (1 << 7)) != 0;
            }
            else if (address == this.DataAddress)
            {
                int color = this.Palettes[this.Index / 8][this.Index % 8 / 2];
                if (this.Index % 2 == 0)
                {
                    color = (color & 0xff00) | value;
                }
                else
                {
                    color = (color & 0x00ff) | (value << 8);
                }
                this.Palettes[this.Index / 8][this.Index % 8 / 2] = color;
                if (this.AutoIncrement)
                {
                    this.Index = (this.Index + 1) & 0x3f;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public int GetByte(int address)
        {
            if (address == this.IndexAddress)
            {
                return this.Index | (this.AutoIncrement ? 0x80 : 0x00) | 0x40;
            }

            if (address != this.DataAddress)
            {
                throw new ArgumentException("Invalid color pallete byte.");
            }

            int color = this.Palettes[this.Index / 8][this.Index % 8 / 2];
            if (this.Index % 2 == 0)
            {
                return color & 0xff;
            }

            return (color >> 8) & 0xff;
        }

        public int[] GetPalette(int index)
        {
            return this.Palettes[index].ToArray();
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                b.Append(i).Append(": ");

                int[] palette = this.GetPalette(i);

                foreach (int c in palette)
                {
                    b.Append($"{c:X4}").Append(' ');
                }

                b[^1] = '\n';
            }

            return b.ToString();
        }

        public void FillWithFf()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.Palettes[i][j] = 0x7fff;
                }
            }
        }
    }
}
