/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/FreezeTimeMultiplayer
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;

namespace WeirdSounds
{
    internal static class WeirdSoundsLibrary
    {
        private static readonly Dictionary<string, List<string>> Library = [];
        private static readonly Dictionary<string, long> LibraryTimer = [];
        private static readonly Dictionary<string, int> LibraryIndex = [];
        private static readonly Dictionary<string, int> LibraryTimerDelay = [];
        private const char SubSoundSeparator = '_';
        internal static readonly Random Random = new();

        public static void Load(IMod modInstance)
        {
            var assetsDir = Path.Combine(modInstance.Helper.DirectoryPath, "assets");
            var soundNames = Directory.GetFiles(assetsDir).Where(fileName => fileName.EndsWith(".wav")).Select(Path.GetFileName).ToArray();
            foreach (var soundFileName in soundNames) {// a_1.wav
                if (soundFileName is null) {
                    continue;
                }
                var soundFileNameWithoutExt = Path.GetFileNameWithoutExtension(soundFileName);//a_1
                var soundPrefix = soundFileNameWithoutExt.LastIndexOf(SubSoundSeparator) != -1 ? soundFileNameWithoutExt[..soundFileNameWithoutExt.LastIndexOf(SubSoundSeparator)] : soundFileNameWithoutExt;//a
                var soundUniqueFileNameWithoutExt = string.Join('.', [modInstance.ModManifest.UniqueID, soundFileNameWithoutExt]);//modId.a_1
                var sound = SoundEffect.FromStream(new FileStream(Path.Combine(assetsDir, soundFileName), FileMode.Open));
                var cue = new CueDefinition
                {
                    name = soundUniqueFileNameWithoutExt,
                    instanceLimit = 1,
                    limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest
                };
                cue.SetSound(sound, Game1.audioEngine.GetCategoryIndex("Sound"));
                Game1.soundBank.AddCue(cue);
                if (Library.TryGetValue(soundPrefix, out var value)) {
                    value.Add(soundUniqueFileNameWithoutExt);
                } else {
                    Library.Add(soundPrefix, [soundUniqueFileNameWithoutExt]);
                    LibraryIndex.Add(soundPrefix, -1);
                    LibraryTimer.Add(soundPrefix, 0);
                    LibraryTimerDelay.Add(soundPrefix, -1);
                }
            }
            if (LibraryTimerDelay.ContainsKey("tool")) {
                LibraryTimerDelay["tool"] = 5;
            }
            if (LibraryTimerDelay.ContainsKey("cluck")) {
                LibraryTimerDelay["cluck"] = -2;
            }
            if (LibraryTimerDelay.ContainsKey("machine")) {
                LibraryTimerDelay["machine"] = 60;
            }
        }

        internal static string GetCueName(string key)
        {
            if (!Library.ContainsKey(key)) {
                return "Coin";
            }
            switch (LibraryTimerDelay[key]) {
                case > -1: {
                    if (Game1.gameModeTicks - LibraryTimer[key] > LibraryTimerDelay[key] * 60) {
                        LibraryIndex[key] = 0;
                    } else {
                        if (LibraryIndex[key] + 1 >= Library[key].Count) {
                            LibraryIndex[key] = 0;
                        } else {
                            LibraryIndex[key]++;
                        }
                    }
                    LibraryTimer[key] = Game1.gameModeTicks;
                    break;
                }
                case -1 when LibraryIndex[key] + 1 >= Library[key].Count:
                    LibraryIndex[key] = 0;
                    break;
                case -1:
                    LibraryIndex[key]++;
                    break;
                case -2:
                    LibraryIndex[key] = Random.Next(Library[key].Count);
                    break;
            }
            return Library[key][LibraryIndex[key]];
        }
    } 
}