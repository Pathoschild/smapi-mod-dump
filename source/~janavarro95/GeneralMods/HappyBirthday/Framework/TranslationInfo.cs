/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework
{
    /// <summary>A class which deals with handling different translations for Vocalization should other voice teams ever wish to voice act for that language.</summary>
    public class TranslationInfo
    {

        public enum LanguageName
        {
            English,
            Spanish,
            Chinese,
            Japanese,
            Russian,
            German,
            Portuguese,
            Italian,
            French,
            Korean,
            Turkish,
            Hungarian
        }

        public enum FileType
        {
            XNB,
            JSON
        }
        /*********
        ** Accessors
        *********/
        /// <summary>The language names supported by this mod.</summary>
        public LanguageName[] LanguageNames { get; } = (from LanguageName language in Enum.GetValues(typeof(LanguageName)) select language).ToArray();

        /// <summary>The current translation mode for the mod, so that it knows what files to load at the beginning of the game.</summary>
        public LanguageName CurrentTranslation { get; set; } = LanguageName.English;

        /// <summary>Holds the info for what translation has what file extension.</summary>
        public Dictionary<LanguageName, string> TranslationFileExtensions { get; } = new Dictionary<LanguageName, string>();

        public Dictionary<LanguageName, LocalizedContentManager.LanguageCode> TranslationCodes { get; } = new Dictionary<LanguageName, LocalizedContentManager.LanguageCode>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public TranslationInfo()
        {
            this.TranslationFileExtensions.Add(LanguageName.English, "en-US");
            this.TranslationFileExtensions.Add(LanguageName.Spanish, "es-ES");
            this.TranslationFileExtensions.Add(LanguageName.Chinese, "zh-CN");
            this.TranslationFileExtensions.Add(LanguageName.Japanese, "ja-JP");
            this.TranslationFileExtensions.Add(LanguageName.Russian, "ru-RU");
            this.TranslationFileExtensions.Add(LanguageName.German, "de-DE");
            this.TranslationFileExtensions.Add(LanguageName.Portuguese, "pt-BR");
            //1.3 languages.
            this.TranslationFileExtensions.Add(LanguageName.Italian, "it-IT");
            this.TranslationFileExtensions.Add(LanguageName.French, "fr-FR");
            this.TranslationFileExtensions.Add(LanguageName.Hungarian, "hu-HU");
            this.TranslationFileExtensions.Add(LanguageName.Turkish, "tr-TR");
            this.TranslationFileExtensions.Add(LanguageName.Korean, "ko-KR");


            this.TranslationCodes.Add(LanguageName.English, LocalizedContentManager.LanguageCode.en);
            this.TranslationCodes.Add(LanguageName.Spanish, LocalizedContentManager.LanguageCode.es);
            this.TranslationCodes.Add(LanguageName.Chinese, LocalizedContentManager.LanguageCode.zh);
            this.TranslationCodes.Add(LanguageName.Japanese, LocalizedContentManager.LanguageCode.ja);
            this.TranslationCodes.Add(LanguageName.Russian, LocalizedContentManager.LanguageCode.ru);
            this.TranslationCodes.Add(LanguageName.German, LocalizedContentManager.LanguageCode.de);
            this.TranslationCodes.Add(LanguageName.Portuguese, LocalizedContentManager.LanguageCode.pt);
            //1.3 languages
            this.TranslationCodes.Add(LanguageName.Italian, LocalizedContentManager.LanguageCode.it);
            this.TranslationCodes.Add(LanguageName.French, LocalizedContentManager.LanguageCode.fr);
            this.TranslationCodes.Add(LanguageName.Hungarian, LocalizedContentManager.LanguageCode.hu);
            this.TranslationCodes.Add(LanguageName.Turkish, LocalizedContentManager.LanguageCode.tr);
            this.TranslationCodes.Add(LanguageName.Korean, LocalizedContentManager.LanguageCode.ko);
        }
        /// <summary>
        /// Gets the current SDV translation code.
        /// </summary>
        /// <returns></returns>
        public LocalizedContentManager.LanguageCode getCurrrentLanguageCode()
        {
            return this.TranslationCodes[this.CurrentTranslation];
        }

        public void setTranslationFromLanguageCode(LocalizedContentManager.LanguageCode code)
        {
            foreach (var v in this.TranslationCodes)
            {
                if (v.Value.Equals(code))
                {
                    this.CurrentTranslation = v.Key;
                    HappyBirthday.ModHelper.WriteConfig<ModConfig>(HappyBirthday.Config);
                    return;
                }
            }
        }

        /// <summary>Gets the proper file extension for the current translation.</summary>
        /// <param name="language">The translation language name.</param>
        public string getFileExtentionForTranslation(LanguageName language, FileType File)
        {
            try
            {
                if (language == LanguageName.English)
                {
                    return this.getFileExtensionForFileType(File);
                }
                return "."+this.TranslationFileExtensions[language] + this.getFileExtensionForFileType(File);
            }
            catch (Exception err)
            {

                Omegasis.HappyBirthday.HappyBirthday.ModMonitor.Log(err.ToString());
                Omegasis.HappyBirthday.HappyBirthday.ModMonitor.Log($"Attempted to get translation: {language}");
                return ".xnb";
            }
        }
        public string getFileExtensionForFileType(FileType Type)
        {
            if (Type == FileType.JSON)
            {
                return ".json";
            }
            else
            {
                return ".xnb";
            }
        }


        public string getFileExtentionForDirectory(LanguageName language)
        {
            try
            {
                string s = this.TranslationFileExtensions[language];
                return s;
            }
            catch (Exception err)
            {

                Omegasis.HappyBirthday.HappyBirthday.ModMonitor.Log(err.ToString());
                Omegasis.HappyBirthday.HappyBirthday.ModMonitor.Log($"Attempted to get translation: {language}");
                return "";
            }
        }


        /// <summary>
        /// Gets the json file for the translation.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string getJSONForTranslation(string FileName, LanguageName language)
        {
            if (language != LanguageName.English)
            {
                return FileName + this.getFileExtentionForTranslation(language, FileType.JSON);
            }
            else
            {
                return FileName + this.getFileExtentionForTranslation(language, FileType.JSON);
            }
        }

        /// <summary>Loads an XNB file from StardewValley/Content</summary>
        public string LoadStringFromXNBFile(string xnbFileName, string key, LanguageName language)
        {
            string xnb = xnbFileName + this.getFileExtentionForTranslation(language, FileType.XNB);
            Dictionary<string, string> loadedDict = Game1.content.Load<Dictionary<string, string>>(xnb);

            if (!loadedDict.TryGetValue(key, out string loaded))
            {
                Omegasis.HappyBirthday.HappyBirthday.ModMonitor.Log("Big issue: Key not found in file:" + xnb + " " + key);
                return "";
            }
            return loaded;
        }

        public virtual string LoadString(string path, LanguageName language)
        {
            this.parseStringPath(path, out string assetName, out string key);

            return this.LoadStringFromXNBFile(assetName, key, language);
        }

        private void parseStringPath(string path, out string assetName, out string key)
        {
            int length = path.IndexOf(':');
            assetName = path.Substring(0, length);
            key = path.Substring(length + 1, path.Length - length - 1);
        }

        /// <summary>
        /// Gets a translated string from the the dictionary with the proper translation; Returns an empty string if this fails somehow.
        /// </summary>
        /// <param name="Language"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string getTranslatedString(LocalizedContentManager.LanguageCode Language, string Key)
        {
            try
            {
                return HappyBirthday.Instance.messages.translatedStrings[Language][Key];

            }
            catch (Exception err)
            {
                return "";
            }
        }

        public string getTranslatedString(string Key)
        {
            if (string.IsNullOrEmpty(Key)) return "";
            if (Key.Equals("Birthday"))
            {
                string s = Game1.content.LoadString("Strings\\UI:Profile_Birthday");
                return s;
            }
            if (Key.Equals("Spring") || Key.Equals("spring"))
            {
                string file = Path.Combine("Strings", "StringsFromCSFiles");
                return HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(file, "Utility.cs.5680", HappyBirthday.Config.translationInfo.CurrentTranslation);
            }
            if (Key.Equals("Summer") || Key.Equals("summer"))
            {
                string file = Path.Combine("Strings", "StringsFromCSFiles");
                return HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(file, "Utility.cs.5681", HappyBirthday.Config.translationInfo.CurrentTranslation);
            }
            if (Key.Equals("Fall") || Key.Equals("fall"))
            {
                string file = Path.Combine("Strings", "StringsFromCSFiles");
                return HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(file, "Utility.cs.5682", HappyBirthday.Config.translationInfo.CurrentTranslation);
            }
            if (Key.Equals("Winter") || Key.Equals("winter"))
            {
                string file = Path.Combine("Strings", "StringsFromCSFiles");
                return HappyBirthday.Config.translationInfo.LoadStringFromXNBFile(file, "Utility.cs.5683", HappyBirthday.Config.translationInfo.CurrentTranslation);
            }
            try
            {
                return HappyBirthday.Instance.messages.translatedStrings[this.getCurrrentLanguageCode()][Key];

            }
            catch (Exception err)
            {
                return "";
            }
        }
    }
}
