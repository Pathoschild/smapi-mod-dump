using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocalization.Framework
{
    public class AudioCues
    {
        //Translation_speaker_file_key
        public static char Seperator = '_';

        public static Dictionary<string, SortedDictionary<string, VoiceAudioOptions>> DictionaryReferences = new Dictionary<string, SortedDictionary<string, VoiceAudioOptions>>();

        public static string generateKey(string translationPath, string SpeakerName, string fileName, string dialogueKey)
        {
            return Vocalization.config.translationInfo.getTranslationNameFromPath(translationPath) + Seperator + SpeakerName + Seperator + fileName + Seperator + dialogueKey;
        }

        public static SortedDictionary<string,VoiceAudioOptions> getWavFileReferences(string translation)
        {
            return DictionaryReferences[Vocalization.config.translationInfo.getTranslationNameFromPath(translation)];
        }

        public static void initialize()
        {
            if (!Directory.Exists(Path.Combine(Vocalization.ModHelper.DirectoryPath, "AudioCues"))){
                Directory.CreateDirectory(Path.Combine(Vocalization.ModHelper.DirectoryPath, "AudioCues"));
            }
            loadAudioCues();

        }

        public static void addWavReference(string key, VoiceAudioOptions cue)
        {
            try
            {
                string translation = key.Split(Seperator).ElementAt(0);
                DictionaryReferences.TryGetValue(translation,out SortedDictionary<string,VoiceAudioOptions> value);

                value.Add(key, cue);
            }
            catch(Exception err)
            {

            }
        }

        public static void loadAudioCues()
        {
                foreach (var v in Vocalization.config.translationInfo.translations)
                {
                    var loaded = Vocalization.ModHelper.ReadJsonFile<SortedDictionary<string, VoiceAudioOptions>>(Path.Combine(Vocalization.ModHelper.DirectoryPath, "AudioCues","AudioCues" + Seperator + v + ".json"));
                    if (loaded == null) loaded = new SortedDictionary<string, VoiceAudioOptions>();
                    DictionaryReferences.Add(v,loaded);
                }
        }

        public static void saveAudioCues()
        {
            foreach (var v in DictionaryReferences)
            {
                Vocalization.ModHelper.WriteJsonFile<SortedDictionary<string, VoiceAudioOptions>>(Path.Combine(Vocalization.ModHelper.DirectoryPath, "AudioCues", "AudioCues" + Seperator + v.Key + ".json"),v.Value);
            }
        }

    }
}
