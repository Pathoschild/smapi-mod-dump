/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Threading;
using CoreBoy.gpu;

namespace CoreBoy.gui
{
    public class BitmapDisplay : IDisplay
    {
        public static readonly int DisplayWidth = 160;
        public static readonly int DisplayHeight = 144;

        public static readonly ushort[] Colors =
        {
            0b1101111111111001,
            0b1000011011100111,
            0b0110101111010001,
            0b0010100011000001,
        };

        private readonly ushort[] Pixels;
        public bool Enabled { get; set; }
        private bool DoRefresh;
        private int Index;

        public event FrameProducedEventHandler OnFrameProduced;

        private readonly object LockObject = new object();

        public BitmapDisplay()
        {
            this.Pixels = new ushort[DisplayWidth * DisplayHeight];
        }

        public void PutDmgPixel(int color)
        {
            this.Pixels[this.Index++] = Colors[color];
        }

        /// <summary>
        /// Gameboy Color pixels are 15 bit RGB values, ie
        /// -rrrrrgggggbbbbb
        /// Convert this to Bgra5551 Surface Format
        /// TODO: see if I can get the emulator emitting this format natively
        /// bbbbbgggggrrrrra
        /// </summary>
        /// <param name="gbcRgb"></param>
        public void PutColorPixel(int gbcRgb)
        {
            this.Pixels[this.Index++] = (ushort)((gbcRgb >> 9) | (gbcRgb << 11) | ((gbcRgb << 1) & 0x7c0) | 0b1);
        }

        public void RequestRefresh() => this.SetRefreshFlag(true);

        public void WaitForRefresh()
        {
            while (this.DoRefresh)
            {
                Thread.Sleep(1);
            }
        }

        public void Run(CancellationToken token)
        {
            this.SetRefreshFlag(false);

            this.Enabled = true;

            while (!token.IsCancellationRequested)
            {
                if (!this.DoRefresh)
                {
                    Thread.Sleep(1);
                    continue;
                }

                this.RefreshScreen();

                this.SetRefreshFlag(false);
            }
        }

        private void RefreshScreen()
        {
            OnFrameProduced?.Invoke(this, this.Pixels);

            this.Index = 0;
        }

        private void SetRefreshFlag(bool flag)
        {
            lock (this.LockObject)
            {
                this.DoRefresh = flag;
            }
        }
    }

}
