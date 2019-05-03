using System;
using System.Text.RegularExpressions;

namespace SilentOak.QualityProducts.Extensions
{
    public static class StringExtensions
    {
        public static string SplitCamelCase(this string str, string join = " ")
        {
            /// From https://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters
            string[] words = Regex.Split(str, @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
            return string.Join(join, words);
        }
    }
}
