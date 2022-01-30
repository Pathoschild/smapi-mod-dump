/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using Microsoft.Xna.Framework.Audio;
    using StardewValley;
    using StardewValley.Characters;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class SoundModule
    {
        private const string HorseSnickerSound = "goldenrevolver_horseSnicker";
        private const string HorseSnortSound = "goldenrevolver_horseSnort";

        public static void PlayHorseEatSound(Horse horse, HorseConfig config)
        {
            PlayHorseSound(HorseSnortSound, horse, config);
        }

        public static void PlayHorsePettingSound(Horse horse, HorseConfig config)
        {
            int number = Game1.random.Next(2);
            PlayHorseSound(HorseSnickerSound + number, horse, config);
        }

        public static void SetupSounds(HorseOverhaul mod)
        {
            var soundEffects = new List<Tuple<string, string>>
            {
                new Tuple<string, string>(HorseSnickerSound + "0", "horse-neigh-shortened.wav"),
                new Tuple<string, string>(HorseSnickerSound + "1", "horse-small-whinny.wav"),
                new Tuple<string, string>(HorseSnortSound, "horse-snort-3.wav")
            };

            foreach (var item in soundEffects)
            {
                // Creating cue and setting its name, which will be the name of the audio to play when using sound functions
                var myCueDefinition = new CueDefinition
                {
                    name = item.Item1
                };

                //// myCueDefinition.instanceLimit = 1;
                //// myCueDefinition.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;

                // Get the audio file and add it to a SoundEffect
                SoundEffect audio;
                string filePathCombined = Path.Combine(mod.Helper.DirectoryPath, "assets", "sounds", item.Item2);
                using (var stream = new FileStream(filePathCombined, FileMode.Open))
                {
                    audio = SoundEffect.FromStream(stream);
                }

                // Setting the sound effect to the new cue
                myCueDefinition.SetSound(audio, Game1.audioEngine.GetCategoryIndex("Sound"), false);

                // Adding the cue to the sound bank
                Game1.soundBank.AddCue(myCueDefinition);
            }
        }

        private static void PlayHorseSound(string soundName, Horse horse, HorseConfig config)
        {
            if (horse?.currentLocation != null && !Game1.options.muteAnimalSounds && !config.DisableHorseSounds)
            {
                horse.currentLocation.playSoundAt(soundName, horse.getTileLocation());
            }
        }
    }
}