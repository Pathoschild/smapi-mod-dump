using System.ComponentModel;
using System.Threading;
using StardewValley;

namespace FarmAutomation.Common
{
    public class SoundHelper
    {
        private static readonly object Lock = new object();
        public static void MuteTemporary(int milliseconds)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (s, a) =>
            {
                lock (Lock)
                {
                    var originalVolume = Game1.options.soundVolumeLevel;
                    if (originalVolume.CompareTo(0) == 0)
                    {
                        return;
                    }
                    Game1.soundCategory.SetVolume(0);
                    Thread.Sleep((int)a.Argument);
                    Game1.soundCategory.SetVolume(originalVolume);
                }
            };
            worker.RunWorkerAsync(milliseconds);

        }
    }
}
