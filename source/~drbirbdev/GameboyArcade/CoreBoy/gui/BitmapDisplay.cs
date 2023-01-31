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

        private readonly ushort[] _pixels;
        public bool Enabled { get; set; }
        private bool _doRefresh;
        private int _i;

        public event FrameProducedEventHandler OnFrameProduced;

        private readonly object _lockObject = new object();

        public BitmapDisplay()
        {
            _pixels = new ushort[DisplayWidth * DisplayHeight];
        }

        public void PutDmgPixel(int color)
        {
            _pixels[_i++] = Colors[color];
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
            _pixels[_i++] = (ushort)((gbcRgb >> 9) | (gbcRgb << 11) | (gbcRgb << 1 & 0x7c0) | 0b1);
        }

        public void RequestRefresh() => SetRefreshFlag(true);

        public void WaitForRefresh()
        {
            while (_doRefresh)
            {
                Thread.Sleep(1);
            }
        }
        
        public void Run(CancellationToken token)
        {
            SetRefreshFlag(false);
            
            Enabled = true;
            
            while (!token.IsCancellationRequested)
            {
                if (!_doRefresh)
                {
                    Thread.Sleep(1);
                    continue;
                }

                RefreshScreen();

                SetRefreshFlag(false);
            }
        }

        private void RefreshScreen()
        {
            OnFrameProduced?.Invoke(this, _pixels);

            _i = 0;
        }

        private void SetRefreshFlag(bool flag)
        {
            lock (_lockObject)
            {
                _doRefresh = flag;
            }
        }
    }

}
