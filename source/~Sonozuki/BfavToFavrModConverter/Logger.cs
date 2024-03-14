/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using System;

namespace BfavToFavrModConverter
{
    /// <summary>A wrapper around <see cref="Console"/> for easier logging with colour.</summary>
    public static class Logger
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Writes the given text to the console output.</summary>
        /// <param name="text">The text to display.</param>
        /// <param name="colour">The colour to display the text in.</param>
        public static void WriteLine(string text, ConsoleColor colour = ConsoleColor.White)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
