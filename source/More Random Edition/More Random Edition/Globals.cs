using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Randomizer
{
	/// <summary>
	/// Used for any global access - USE SPARINGLY
	/// </summary>
	public class Globals
	{
		public static ModEntry ModRef { get; set; }
		public static ModConfig Config { get; set; }
		public static Random RNG { get; set; }
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
			if (tokens == null)
			{
				return ModRef.Helper.Translation.Get(key);
			}
			return ModRef.Helper.Translation.Get(key, tokens);
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
		/// <returns />
		public static bool RNGGetNextBoolean(int percentage)
		{
			if (percentage < 0 || percentage > 100) ConsoleWarn("Percentage is invalid (less than 0 or greater than 100)");
			return RNG.Next(0, 100) < percentage;
		}

		/// <summary>
		/// Gets a random integer value + or - the given percentage (rounds up)
		/// ex) value of 10 with percentage of 50 returns a value between 5 and 15
		/// </summary>
		/// <param name="value">The base value</param>
		/// <param name="percentage">The percentage of the base value to use</param>
		/// <returns>The random value retrieved</returns>
		public static int RNGGetIntWithinPercentage(int value, int percentage)
		{
			int difference = (int)Math.Ceiling(value * ((double)percentage / 100));
			return new Range(value - difference, value + difference).GetRandomValue();
		}

		/// <summary>
		/// Gets a random value out of the given list
		/// </summary>
		/// <typeparam name="T">The type of the list</typeparam>
		/// <param name="list">The list</param>
		/// <returns />
		public static T RNGGetRandomValueFromList<T>(List<T> list)
		{
			if (list == null || list.Count == 0)
			{
				ConsoleError("Attempted to get a random value out of an empty list!");
				return default(T);
			}

			return list[RNG.Next(list.Count)];
		}

		/// <summary>
		/// Gets a random value out of the given list and removes it
		/// </summary>
		/// <typeparam name="T">The type of the list</typeparam>
		/// <param name="list">The list</param>
		/// <returns />
		public static T RNGGetAndRemoveRandomValueFromList<T>(List<T> list)
		{
			if (list == null || list.Count == 0)
			{
				ConsoleError("Attempted to get a random value out of an empty list!");
				return default(T);
			}
			int selectedIndex = RNG.Next(list.Count);
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
		/// <returns>
		/// The randomly chosen values - might be less than the number of values if the list doesn't contain that many
		/// </returns>
		public static List<T> RNGGetRandomValuesFromList<T>(List<T> inputList, int numberOfvalues)
		{
			List<T> listToChooseFrom = new List<T>(inputList); // Don't modify the original list
			List<T> randomValues = new List<T>();
			if (listToChooseFrom == null || listToChooseFrom.Count == 0)
			{
				ConsoleError("Attempted to get random values out of an empty list!");
				return randomValues;
			}

			int numberOfIterations = Math.Min(numberOfvalues, listToChooseFrom.Count);
			for (int i = 0; i < numberOfIterations; i++)
			{
				randomValues.Add(RNGGetAndRemoveRandomValueFromList(listToChooseFrom));
			}

			return randomValues;
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
			return input.Length < length ? input : input.Substring(0, length);
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
