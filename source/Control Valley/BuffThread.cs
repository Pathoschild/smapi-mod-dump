/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tesla1889tv/ControlValleyMod
**
*************************************************/

using System.Globalization;
using System.Threading;
using StardewValley;

namespace ControlValley
{
    public class BuffThread
    {
        private readonly Buff buff;
        private readonly int duration;

        public BuffThread(int buff, int duration)
        {
            this.buff = new Buff(buff);
            this.duration = duration;
        }

        public void Run()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            buff.addBuff();
            Thread.Sleep(duration);
            buff.removeBuff();
        }
    }
}
