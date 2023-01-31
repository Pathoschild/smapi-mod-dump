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

namespace CoreBoy.gpu
{
    public class NullDisplay : IDisplay
    {
        public bool Enabled { get; set; }
        public event FrameProducedEventHandler OnFrameProduced;

        public void PutDmgPixel(int color)
        {
        }

        public void PutColorPixel(int gbcRgb)
        {
        }

        public void RequestRefresh()
        {
        }

        public void WaitForRefresh()
        {
        }

        public void Run(CancellationToken token)
        {
        }
    }
}