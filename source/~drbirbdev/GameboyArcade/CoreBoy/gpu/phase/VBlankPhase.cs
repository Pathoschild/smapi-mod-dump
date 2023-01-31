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
    public class VBlankPhase : IGpuPhase
    {
        private int _ticks;

        public VBlankPhase Start()
        {
            _ticks = 0;
            return this;
        }

        public bool Tick()
        {
            return ++_ticks < 456;
        }
    }
}