/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace ProducerFrameworkMod.Utils
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
