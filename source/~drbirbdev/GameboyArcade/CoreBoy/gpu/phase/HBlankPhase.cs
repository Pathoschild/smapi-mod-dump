/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.gpu.phase
{
    public class HBlankPhase : IGpuPhase
    {

        private int _ticks;

        public HBlankPhase Start(int ticksInLine)
        {
            _ticks = ticksInLine;
            return this;
        }

        public bool Tick()
        {
            _ticks++;
            return _ticks < 456;
        }

    }
}