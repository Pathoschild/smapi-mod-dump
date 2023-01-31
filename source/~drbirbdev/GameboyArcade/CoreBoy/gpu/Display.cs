/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using CoreBoy.gui;

namespace CoreBoy.gpu
{
    public delegate void FrameProducedEventHandler(object sender, ushort[] frameData);

    public interface IDisplay : IRunnable
    {
        bool Enabled { get; set; }

        event FrameProducedEventHandler OnFrameProduced;

        void PutDmgPixel(int color);
        void PutColorPixel(int gbcRgb);
        void RequestRefresh();
        void WaitForRefresh();
    }
}
