using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vocalization.Framework
{
    public class AudioCues
    {
        //Translation_speaker_file_key
        public static char Seperator = '_';

        public static Dictionary<string, SortedDictionary<string, VoiceAudioOptions>> DictionaryReferences = new Dictionary<string, SortedDictionary<string, VoiceAudioOptions>>();

        public static string generateKey(LanguageName language, string SpeakerName, string fileName, string dialogueKey)
        {
            return Vocalization.config.translationInfo.getTranslationName(language) + Seperator + SpeakerName + Seperator + fileName + Seperator + dialogueKey;
        }

        public static SortedDictionary<string, VoiceAudioOptions> getWavFileReferences(LanguageName language)
        {
            return DictionaryReferences[Vocalization.config.translationInfo.getTranslationName(language)];
        }

        public static void initialize()
        {
            Directory.CreateDirectory(Path.Combine(Vocalization.ModHelper.DirectoryPath, "AudioCues"));
            loadAudioCues();

        }

        public static void addWavReference(string key, VoiceAudioOptions cue)
        {
            try
            {
                string translation = key.Split(Seperator).ElementAt(0);
                DictionaryReferences.TryGetValue(translation, out SortedDictionary<string, VoiceAudioOptions> value);

                value.Add(key, cue);
            }
            catch { }
        }

        public static void loadAudioCues()
        {
            foreach (LanguageName language in Vocalization.config.translationInfo.LanguageNames)
            {
                var loaded = Vocalization.ModHelper.Data.ReadJsonFile<SortedDictionary<string, VoiceAudioOptions>>($"AudioCues/AudioCues{Seperator}{language}.json")
                    ?? new SortedDictionary<string, VoiceAudioOptions>();
                DictionaryReferences.Add(Vocalization.config.translationInfo.getTranslationName(language), loaded);
            }
        }

        public static void saveAudioCues()
        {
            foreach (var v in DictionaryReferences)
                Vocalization.ModHelper.Data.WriteJsonFile($"AudioCues/AudioCues{Seperator}{v.Key}.json", v.Value);
        }
    }
}
