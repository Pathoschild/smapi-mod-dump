using System;

namespace BFAVToFAVRModConverter
{
    /// <summary>The wrapper around the <see cref="Console"/> for easier logging with colour.</summary>
    public static class Logger
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Write the given text to the console output.</summary>
        /// <param name="text">The text to display.</param>
        /// <param name="colour">The colour to display the text in.</param>
        public static void WriteLine(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
