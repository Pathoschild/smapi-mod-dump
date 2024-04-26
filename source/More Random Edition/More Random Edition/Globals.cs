/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.IO;

namespace Randomizer
{
    /// <summary>
    /// Used for any global access - USE SPARINGLY
    /// </summary>
    public class Globals
	{
		public static ModEntry ModRef { get; set; }
		public static ModConfig Config { get; set; }
		public static SpoilerLogger SpoilerLog { get; set; }

		/// <summary>
		/// A shortcut to write traces to the console
		/// </summary>
		/// <param name="input">The input string</param>
		public static void ConsoleTrace(string input)
		{
			ModRef.Monitor.Log(input);
		}

		/// <summary>
		/// A shortcut to write warnings to the console
		/// </summary>
		/// <param name="input"></param>
		public static void ConsoleWarn(string input)
		{
			ModRef.Monitor.Log(input, LogLevel.Warn);
		}

		/// <summary>
		/// A shortcut to write errors to the console
		/// </summary>
		/// <param name="input"></param>
		public static void ConsoleError(string input)
		{
			ModRef.Monitor.Log(input, LogLevel.Error);
		}

		/// <summary>
		/// A shortcut to write to the spoiler log
		/// </summary>
		/// <param name="input">The input</param>
		public static void SpoilerWrite(string input)
		{
			SpoilerLog.BufferLine(input);
		}

		/// <summary>
		/// A shortcut to the translation API
		/// </summary>
		/// <param name="key">The translation key</param>
		/// <param name="tokens">Tokens to replace in the translation</param>
		/// <returns>The retrieved translation</returns>
		public static string GetTranslation(string key, object tokens = null)
		{
			
            return ModRef.Helper.Translation.Get(key, tokens);
		}

        /// <summary>
        /// Gets the English translation of a key
        /// Used when we need this, but are using another locale
        /// </summary>
        /// <param name="key">The translation key</param>
        /// <param name="tokens">Tokens to replace in the translation</param>
        /// <returns>The translation</returns>
        public static string GetEnglishTranslation(string key, object tokens = null)
		{
			var allTranslations = ModRef.Helper.Translation.GetInAllLocales(key);
			return allTranslations["default"].Tokens(tokens);
		}

		/// <summary>
		/// Gets the language-specific file name given the file name and extension
		/// Modified from Stardew's code in LocalizedContentManager
		/// </summary>
		/// <returns></returns>
		public static string GetLocalizedFileName(string fileName, string extension = "")
		{
			// If extension is given, include the period before the extension
			// Otherwise we do want to support getting the name without an extension
            if (extension != "")
			{
				extension = $".{extension}";
			}

            string localeCode = ModRef.Helper.Translation.LocaleEnum switch
            {
                LocalizedContentManager.LanguageCode.ja => ".ja-JP",
                LocalizedContentManager.LanguageCode.ru => ".ru-RU",
                LocalizedContentManager.LanguageCode.zh => ".zh-CN",
                LocalizedContentManager.LanguageCode.pt => ".pt-BR",
                LocalizedContentManager.LanguageCode.es => ".es-ES",
                LocalizedContentManager.LanguageCode.de => ".de-DE",
                LocalizedContentManager.LanguageCode.th => ".th-TH",
                LocalizedContentManager.LanguageCode.fr => ".fr-FR",
                LocalizedContentManager.LanguageCode.ko => ".ko-KR",
                LocalizedContentManager.LanguageCode.it => ".it-IT",
                LocalizedContentManager.LanguageCode.tr => ".tr-TR",
                LocalizedContentManager.LanguageCode.hu => ".hu-HU",
                _ => "",
            };
            return $"{fileName}{localeCode}{extension}";
        }

		/// <summary>
		/// Gets the file path given the path from the mod directory
		/// </summary>
		/// <param name="pathFromModFolder">The path to the file from the mod folder</param>
		/// <returns></returns>
		public static string GetFilePath(string pathFromModFolder)
		{
			return Path.Combine(ModRef.Helper.DirectoryPath, pathFromModFolder);
		}

        /// <summary>
        /// Returns "a" or "an" based on if word begins with vowel
        /// </summary>
        public static string GetArticle(string word)
		{
			if (string.IsNullOrEmpty(word))
			{
				return string.Empty;
			}

			word = word.ToLower();
			if (word.StartsWith("a") || 
				word.StartsWith("e") || 
				word.StartsWith("i") || 
				word.StartsWith("o") || 
				word.StartsWith("u"))
				return "an";
			else
				return "a";
		}

		/// <summary>
		/// Returns the start of the given string, up to the given length
		/// </summary>
		/// <param name="input">The string</param>
		/// <param name="length">The length</param>
		/// <returns>The requested length, truncated appropriately</returns>
		public static string GetStringStart(string input, int length)
		{
			return input.Length < length ? input : input[..length];
		}
	}
}
