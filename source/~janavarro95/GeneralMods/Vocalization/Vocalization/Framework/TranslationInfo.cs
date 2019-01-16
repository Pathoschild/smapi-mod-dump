using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace Vocalization.Framework
{
    /// <summary>A class which deals with handling different translations for Vocalization should other voice teams ever wish to voice act for that language.</summary>
    public class TranslationInfo
    {
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
            this.TranslationFileExtensions.Add(LanguageName.English, ".xnb");
            this.TranslationFileExtensions.Add(LanguageName.Spanish, ".es-ES.xnb");
            this.TranslationFileExtensions.Add(LanguageName.Chinese, ".zh-CN.xnb");
            this.TranslationFileExtensions.Add(LanguageName.Japanese, ".ja-JP.xnb");
            this.TranslationFileExtensions.Add(LanguageName.Russian, ".ru-RU.xnb");
            this.TranslationFileExtensions.Add(LanguageName.German, ".de-DE.xnb");
            this.TranslationFileExtensions.Add(LanguageName.Portuguese, ".pt-BR.xnb");

            this.TranslationCodes.Add(LanguageName.English, LocalizedContentManager.LanguageCode.en);
            this.TranslationCodes.Add(LanguageName.Spanish, LocalizedContentManager.LanguageCode.es);
            this.TranslationCodes.Add(LanguageName.Chinese, LocalizedContentManager.LanguageCode.zh);
            this.TranslationCodes.Add(LanguageName.Japanese, LocalizedContentManager.LanguageCode.ja);
            this.TranslationCodes.Add(LanguageName.Russian, LocalizedContentManager.LanguageCode.ru);
            this.TranslationCodes.Add(LanguageName.German, LocalizedContentManager.LanguageCode.de);
            this.TranslationCodes.Add(LanguageName.Portuguese, LocalizedContentManager.LanguageCode.pt);
        }

        /// <summary>Get the language name from a string.</summary>
        /// <param name="language">The language name.</param>
        public string getTranslationName(LanguageName language)
        {
            return language.ToString();
        }

        public void changeLocalizedContentManagerFromTranslation(LanguageName language)
        {
            LocalizedContentManager.CurrentLanguageCode = !this.TranslationCodes.TryGetValue(language, out LocalizedContentManager.LanguageCode code)
                ? LocalizedContentManager.LanguageCode.en
                : code;
        }

        public void resetLocalizationCode()
        {
            LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.en;
        }

        /// <summary>Gets the proper file extension for the current translation.</summary>
        /// <param name="language">The translation language name.</param>
        public string getFileExtentionForTranslation(LanguageName language)
        {
            try
            {
                return this.TranslationFileExtensions[language];
            }
            catch (Exception err)
            {

                Vocalization.ModMonitor.Log(err.ToString());
                Vocalization.ModMonitor.Log($"Attempted to get translation: {language}");
                return ".xnb";
            }
        }

        /// <summary>Gets the proper XNB for Buildings (aka Blueprints) from the data folder.</summary>
        public string getBuildingXNBForTranslation(LanguageName language)
        {
            string buildings = "Blueprints";
            return buildings + this.getFileExtentionForTranslation(language);
        }

        /// <summary>Gets the proper XNB file for the name passed in. Combines the file name with it's proper translation extension.</summary>
        public string getXNBForTranslation(string xnbFileName, LanguageName language)
        {
            return xnbFileName + this.getFileExtentionForTranslation(language);
        }

        /// <summary>Loads an XNB file from StardewValley/Content</summary>
        public string LoadXNBFile(string xnbFileName, string key, LanguageName language)
        {
            string xnb = xnbFileName + this.getFileExtentionForTranslation(language);
            Dictionary<string, string> loadedDict = Game1.content.Load<Dictionary<string, string>>(xnb);

            if (!loadedDict.TryGetValue(key, out string loaded))
            {
                Vocalization.ModMonitor.Log("Big issue: Key not found in file:" + xnb + " " + key);
                return "";
            }
            return loaded;
        }

        public virtual string LoadString(string path, LanguageName language, object sub1, object sub2, object sub3)
        {
            string format = this.LoadString(path, language);
            try
            {
                return string.Format(format, sub1, sub2, sub3);
            }
            catch { }

            return format;
        }

        public virtual string LoadString(string path, LanguageName language, object sub1, object sub2)
        {
            string format = this.LoadString(path, language);
            try
            {
                return string.Format(format, sub1, sub2);
            }
            catch { }

            return format;
        }

        public virtual string LoadString(string path, LanguageName language, object sub1)
        {
            string format = this.LoadString(path, language);
            try
            {
                return string.Format(format, sub1);
            }
            catch { }

            return format;
        }

        public virtual string LoadString(string path, LanguageName language)
        {
            this.parseStringPath(path, out string assetName, out string key);

            return this.LoadXNBFile(assetName, key, language);
        }

        public virtual string LoadString(string path, LanguageName language, params object[] substitutions)
        {
            string format = this.LoadString(path, language);
            if (substitutions.Length != 0)
            {
                try
                {
                    return string.Format(format, substitutions);
                }
                catch { }
            }
            return format;
        }

        private void parseStringPath(string path, out string assetName, out string key)
        {
            int length = path.IndexOf(':');
            assetName = path.Substring(0, length);
            key = path.Substring(length + 1, path.Length - length - 1);
        }
    }
}
