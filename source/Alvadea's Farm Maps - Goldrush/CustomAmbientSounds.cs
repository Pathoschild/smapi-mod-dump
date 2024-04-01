/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jaksha6472/MiningShack
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace AlvadeasMiningShack
{
    internal class CustomAmbientSounds
    {
        private static Dictionary<Vector2, int> sounds = new Dictionary<Vector2, int>();
        private static float[] shortestDistanceForCue;
        private static int farthestSoundDistance = 1000;
        public const float doNotPlay = 9999999f;
        private static ICue waterfall;
        public const int sound_fly = 0;
        public const int sound_frog = 1;

        static CustomAmbientSounds()
        {
            if (Game1.soundBank != null)
            {
                if (waterfall == null)
                {
                    waterfall = Game1.soundBank.GetCue("Alvadea.Goldrush_Waterfall");
                    waterfall.Play();
                    waterfall.Pause();
                }
            }
            shortestDistanceForCue = new float[1];
        }

        public static void update()
        {
            if (sounds.Count == 0)
                return;

            for (int index = 0; index < shortestDistanceForCue.Length; ++index)
                shortestDistanceForCue[index] = 9999999f;

            Vector2 standingPosition = Game1.player.getStandingPosition();

            foreach (KeyValuePair<Vector2, int> sound in sounds)
            {
                float num = Vector2.Distance(sound.Key, standingPosition);
                if (shortestDistanceForCue[sound.Value] > num)
                    shortestDistanceForCue[sound.Value] = num;
            }
            for (int index = 0; index < shortestDistanceForCue.Length; ++index)
            {
                if (shortestDistanceForCue[index] <= farthestSoundDistance)
                {
                    float num = Math.Min(1f, (float)(1.0 - shortestDistanceForCue[index] / (double)farthestSoundDistance));
                    switch (index)
                    {
                        case 0:
                            if (waterfall != null)
                            {
                                waterfall.Volume = Math.Min(num, Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel));
                                waterfall.Resume();
                                continue;
                            }
                            continue;
                        default:
                            continue;
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            if (waterfall != null)
                            {
                                waterfall.Pause();
                                continue;
                            }
                            continue;
                        default:
                            continue;
                    }
                }
            }
        }

        public static void addCustomSound(Vector2 tileLocation, int whichSound)
        {
            if (sounds.ContainsKey(tileLocation * 64f) && sounds[tileLocation * 64f] == whichSound)
                return;
            if (sounds.ContainsKey(tileLocation * 64f) && sounds[tileLocation * 64f] != whichSound)
            {
                removeCustomSound(tileLocation);
            }
            sounds.Add(tileLocation * 64f, whichSound);
        }

        public static void removeCustomSound(Vector2 tileLocation)
        {
            if (!sounds.ContainsKey(tileLocation * 64f))
                return;
            switch (sounds[tileLocation * 64f])
            {
                case 0:
                    if (waterfall != null)
                    {
                        waterfall.Pause();
                        break;
                    }
                    break;
            }
            sounds.Remove(tileLocation * 64f);
        }

    }
}