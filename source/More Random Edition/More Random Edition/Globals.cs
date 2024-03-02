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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Randomizer
{
    /// <summary>
    /// Used for any global access - USE SPARINGLY
    /// </summary>
    public class Globals
	{
		public static ModEntry ModRef { get; set; }
		public static ModConfig Config { get; set; }
		public static SaveLoadRNG RNG { get; set; }
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
		/// Gets a random boolean value
		/// </summary>
		/// <returns />
		public static bool RNGGetNextBoolean()
		{
			return RNG.Next(0, 2) == 0;
		}

		/// <summary>
		/// Gets a random boolean value
		/// </summary>
		/// <param name="percentage">The percentage of the boolean being true - 10 would be 10%, etc.</param>
		/// <param name="rng">The Random object to use - defaults to the global one</param>
		/// <returns />
		public static bool RNGGetNextBoolean(int percentage, Random rng = null)
		{
			var rngToUse = rng ?? RNG;
			if (percentage < 0 || percentage > 100) ConsoleWarn("Percentage is invalid (less than 0 or greater than 100)");
			return rngToUse.Next(0, 100) < percentage;
		}

        /// <summary>
        /// Gets a random integer value + or - the given percentage (rounds up)
        /// ex) value of 10 with percentage of 50 returns a value between 5 and 15
        /// </summary>
        /// <param name="value">The base value</param>
        /// <param name="percentage">The percentage of the base value to use</param>
        /// <param name="rng">The Random object to use - defaults to the global one</param>
        /// <returns>The random value retrieved</returns>
        public static int RNGGetIntWithinPercentage(int value, int percentage, Random rng = null)
		{
            var rngToUse = rng ?? RNG;
            var difference = (int)Math.Ceiling(value * ((double)percentage / 100));
			return new Range(value - difference, value + difference).GetRandomValue(rngToUse);
		}

        /// <summary>
        /// Gets a random value out of the given list
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="rng">The Random object to use - defaults to the global one</param>
        /// <returns />
        public static T RNGGetRandomValueFromList<T>(List<T> list, Random rng = null)
		{
            var rngToUse = rng ?? RNG;

            if (list == null || list.Count == 0)
			{
				ConsoleError("Attempted to get a random value out of an empty list!");
				return default;
			}

			return list[rngToUse.Next(list.Count)];
		}

        /// <summary>
        /// Gets a random value out of the given list and removes it
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="rng">The Random object to use - defaults to the global one</param>
        /// <returns />
        public static T RNGGetAndRemoveRandomValueFromList<T>(List<T> list, Random rng = null)
		{
            var rngToUse = rng ?? RNG;

            if (list == null || list.Count == 0)
			{
				ConsoleError("Attempted to get a random value out of an empty list!");
				return default;
			}
			int selectedIndex = rngToUse.Next(list.Count);
			T selectedValue = list[selectedIndex];
			list.RemoveAt(selectedIndex);
			return selectedValue;
		}

        /// <summary>
        /// Gets a random set of values form a list
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="inputList">The list</param>
        /// <param name="numberOfvalues">The number of values to return</param>
        /// <param name="rng">The Random object to use - defaults to the global one</param>
		/// <param name="forceNumberOfValuesRNGCalls">
		/// Forces this to advance the RNG even if number of values is less than the list length
		/// This is for situations where different settings result in different lengths of lists
		/// </param>
        /// <returns>
        /// The randomly chosen values - might be less than the number of values if the list doesn't contain that many
        /// </returns>
        public static List<T> RNGGetRandomValuesFromList<T>(
			List<T> inputList, 
			int numberOfvalues, 
			Random rng = null,
			bool forceNumberOfValuesRNGCalls = false)
		{
            var rngToUse = rng ?? RNG;

            List<T> listToChooseFrom = new(inputList); // Don't modify the original list
			List<T> randomValues = new();
			if (listToChooseFrom == null || listToChooseFrom.Count == 0)
			{
				ConsoleError("Attempted to get random values out of an empty list!");
				return randomValues;
			}

			int numberOfIterations = Math.Min(numberOfvalues, listToChooseFrom.Count);
			int i;
			for (i = 0; i < numberOfIterations; i++)
			{
				randomValues.Add(RNGGetAndRemoveRandomValueFromList(listToChooseFrom, rngToUse));
			}

			// If we're forcing RNG to advance, we must call it for each item that's left
			if (forceNumberOfValuesRNGCalls)
			{
                for (; i < inputList.Count - 1; i++)
                {
                    rngToUse.Next();
                }
            }

			return randomValues;
		}

        /// <summary>
        /// Gets an RNG value based on the seed and the ingame day
		/// Essentially, this is a seed that changes once a week (every Monday)
		/// Seeded on the given string, the farm name, and the days played
        /// </summary>
		/// <param name="seed">The seed to use</param>
        /// <returns>The Random object</returns>
        public static Random GetWeeklyRNG(string seed)
        {
			int time = Game1.Date.TotalDays / 7;
            byte[] seedvar = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value + seed));
            int fullSeed = BitConverter.ToInt32(seedvar, 0) + time;

            return new Random(fullSeed);
        }

        /// <summary>
        /// Gets an RNG value based on the seed and the ingame day
        /// Essentially, this is a seed that changes once every day
        /// Seeded on the given string, the farm name, and the days played
        /// <param name="seed">The seed to use</param>
        /// </summary>
        /// <returns>The Random object</returns>
        public static Random GetDailyRNG(string seed)
        {
			int time = Game1.Date.TotalDays;
            byte[] seedvar = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value + seed));
            int fullSeed = BitConverter.ToInt32(seedvar, 0) + time;

            return new Random(fullSeed);
        }

        /// <summary>
        /// Gets an RNG value based on the farm namd and the given seed
        /// </summary>
        /// <param name="seed">The seed to use</param>
        /// <returns>The Random object</returns>
        public static Random GetFarmRNG(string seed)
		{
            byte[] seedvar = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value + seed));
            int fullSeed = BitConverter.ToInt32(seedvar, 0);

            return new Random(fullSeed);
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
			if (word.StartsWith("a") || word.StartsWith("e") || word.StartsWith("i") || word.StartsWith("o") || word.StartsWith("u"))
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

		/// <summary>
		/// Replace one method with another
		/// Credit goes here: https://stackoverflow.com/questions/7299097/dynamically-replace-the-contents-of-a-c-sharp-method
		/// NOTE: THIS CODE IS UNSAFE, USE WITH CAUTION
		/// </summary>
		/// <param name="methodToReplace">The method to replace</param>
		/// <param name="methodToInject">The method to replace it with</param>
		public static void RepointMethod(MethodInfo methodToReplace, MethodInfo methodToInject)
		{
			if (methodToReplace == null || methodToInject == null)
			{
				return;
			}

			unsafe
			{
				if (IntPtr.Size == 4) // Checks whether we're running on a 32-bit or 64-bit architecture
				{
					int* addressToUse = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
					int* addressToReplace = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
					*addressToReplace = *addressToUse;
				}
				else
				{
					long* addressTouse = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
					long* addressToReplace = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
					*addressToReplace = *addressTouse;
				}
			}
		}
	}
}
