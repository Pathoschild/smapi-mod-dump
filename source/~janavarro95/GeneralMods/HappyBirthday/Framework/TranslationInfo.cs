using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.HappyBirthday.Framework
{
    /// <summary>
    /// A class which deals with handling different translations for Vocalization should other voice teams ever wish to voice act for that language.
    /// </summary>
    public class TranslationInfo
    {
        /// <summary>
        /// The list of all supported translations by this mod.
        /// </summary>
        public List<string> translations;

        /// <summary>
        /// The current translation mode for the mod, so that it knows what files to load at the beginning of the game.
        /// </summary>
        public string currentTranslation;

        /// <summary>
        /// Holds the info for what translation has what file extension.
        /// </summary>
        public Dictionary<string, string> translationFileInfo;


        public Dictionary<string, LocalizedContentManager.LanguageCode> translationCodes;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public TranslationInfo()
        {
            translations = new List<string>();

            translationFileInfo = new Dictionary<string, string>();
            translationCodes = new Dictionary<string, LocalizedContentManager.LanguageCode>();
            translations.Add("English");
            translations.Add("Spanish");
            translations.Add("Chinese");
            translations.Add("Japanese");
            translations.Add("Russian");
            translations.Add("German");
            translations.Add("Brazillian Portuguese");

            currentTranslation = "English";

            translationFileInfo.Add("English", ".json");
            translationFileInfo.Add("Spanish", ".es-ES.json");
            translationFileInfo.Add("Chinese", ".zh-CN.json");
            translationFileInfo.Add("Japanese", ".ja-JP.json");
            translationFileInfo.Add("Russian", ".ru-RU.json");
            translationFileInfo.Add("German", ".de-DE.json");
            translationFileInfo.Add("Brazillian Portuguese", ".pt-BR.json");


            translationCodes.Add("English", LocalizedContentManager.LanguageCode.en);
            translationCodes.Add("Spanish", LocalizedContentManager.LanguageCode.es);
            translationCodes.Add("Chinese", LocalizedContentManager.LanguageCode.zh);
            translationCodes.Add("Japanese", LocalizedContentManager.LanguageCode.ja);
            translationCodes.Add("Russian", LocalizedContentManager.LanguageCode.ru);
            translationCodes.Add("German", LocalizedContentManager.LanguageCode.de);
            translationCodes.Add("Brazillian Portuguese", LocalizedContentManager.LanguageCode.pt);

        }

        public string getTranslationNameFromPath(string fullPath)
        {
            return Path.GetFileName(fullPath);
        }


        public void changeLocalizedContentManagerFromTranslation(string translation)
        {
            string tra = getTranslationNameFromPath(translation);
            bool f = translationCodes.TryGetValue(tra, out LocalizedContentManager.LanguageCode code);
            if (f == false) LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.en;
            else LocalizedContentManager.CurrentLanguageCode = code;
            return;
        }

        public void resetLocalizationCode()
        {
            LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.en;
        }

        /// <summary>
        /// Gets the proper file extension for the current translation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string getFileExtentionForTranslation(string path)
        {
            /*
            bool f = translationFileInfo.TryGetValue(translation, out string value);
            if (f == false) return ".json";
            else return value;
            */
            string translation = Path.GetFileName(path);
            try
            {
                return translationFileInfo[translation];
            }
            catch (Exception err)
            {
                HappyBirthday.ModMonitor.Log("WTF SOMETHING IS WRONG!", StardewModdingAPI.LogLevel.Warn);
                //Vocalization.ModMonitor.Log(err.ToString());
                //Vocalization.ModMonitor.Log("Attempted to get translation: " + translation);
                return ".json";
            }
        }

        /// <summary>
        /// Gets the proper json for Buildings (aka Blueprints) from the data folder.
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public string getBuildingjsonForTranslation(string translation)
        {
            string buildings = "Blueprints";
            return buildings + getFileExtentionForTranslation(translation);
        }

        /// <summary>
        /// Gets the proper json file for the name passed in. Combines the file name with it's proper translation extension.
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <param name="translation"></param>
        /// <returns></returns>
        public string getjsonForTranslation(string jsonFileName, string translation)
        {
            return jsonFileName + getFileExtentionForTranslation(translation);
        }



        /// <summary>
        /// Loads an json file from StardewValley/Content
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <param name="key"></param>
        /// <param name="translation"></param>
        /// <returns></returns>
        public string LoadjsonFile(string jsonFileName, string key, string translation)
        {
            string json = jsonFileName + getFileExtentionForTranslation(translation);
            Dictionary<string, string> loadedDict = Game1.content.Load<Dictionary<string, string>>(json);

            string loaded;
            bool f = loadedDict.TryGetValue(key, out loaded);
            if (f == false)
            {
                //Vocalization.ModMonitor.Log("Big issue: Key not found in file:" + json + " " + key);
                return "";
            }
            else return loaded;
        }

        /// <summary>
        /// Loads a string dictionary from a json file.
        /// </summary>
        /// <param name="jsonFileName"></param>
        /// <param name="translation"></param>
        /// <returns></returns>
        public Dictionary<string,string> LoadJsonFileDictionary(string jsonFileName, string translation)
        {
            string json = jsonFileName + getFileExtentionForTranslation(translation);
            Dictionary<string, string> loadedDict = Game1.content.Load<Dictionary<string, string>>(json);

            return loadedDict;
        }

        public virtual string LoadString(string path, string translation, object sub1, object sub2, object sub3)
        {
            string format = this.LoadString(path, translation);
            try
            {
                return string.Format(format, sub1, sub2, sub3);
            }
            catch (Exception ex)
            {
            }

            return format;
        }

        public virtual string LoadString(string path, string translation, object sub1, object sub2)
        {
            string format = this.LoadString(path, translation);
            try
            {
                return string.Format(format, sub1, sub2);
            }
            catch (Exception ex)
            {
            }

            return format;
        }

        public virtual string LoadString(string path, string translation, object sub1)
        {
            string format = this.LoadString(path, translation);
            try
            {
                return string.Format(format, sub1);
            }
            catch (Exception ex)
            {
            }

            return format;
        }

        public virtual string LoadString(string path, string translation)
        {
            string assetName;
            string key;
            this.parseStringPath(path, out assetName, out key);

            return LoadjsonFile(assetName, key, translation);
        }

        public virtual string LoadString(string path, string translation, params object[] substitutions)
        {
            string format = this.LoadString(path, translation);
            if (substitutions.Length != 0)
            {
                try
                {
                    return string.Format(format, substitutions);
                }
                catch (Exception ex)
                {
                }
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
