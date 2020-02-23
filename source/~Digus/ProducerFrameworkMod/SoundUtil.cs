using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;

namespace ProducerFrameworkMod
{
    internal class SoundUtil
    {
        internal static void PlaySound(List<string> soundList, GameLocation currentLocation)
        {
            soundList.ForEach(s =>
            {
                try
                {
                    currentLocation.playSound(s, NetAudio.SoundContext.Default);
                }
                catch (Exception)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Error trying to play sound '{s}'.",LogLevel.Debug);
                }
            });
        }

        internal static void PlayDelayedSound(List<Dictionary<string,int>> delayedSoundList, GameLocation currentLocation)
        {
            foreach (Dictionary<string, int> dictionary in delayedSoundList)
            {
                foreach (KeyValuePair<string, int> pair in dictionary)
                {
                    DelayedAction.playSoundAfterDelay(pair.Key, pair.Value, (GameLocation)null, -1);
                }
            }
        }
    }
}
