/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using FelixDev.StardewMods.FeTK.Framework.Helpers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Data.Parsers
{
    /// <summary>
    /// Provides an API to parse a colored string.
    /// 
    /// To color a string, use the <color> tag:
    /// <color=COLOR_VALUE>Some text</color> <-- Displays "Some text" in the color specified by [COLOR_VALUE].
    /// 
    /// Color values can be specified using the following color representations:
    ///     (1) The hexadecimal format. A hexadecimal color is specified with: #RRGGBB, 
    ///     where the RR (red), GG (green) and BB (blue) hexadecimal integers specify the components of the color. 
    ///     All values must be between 00 (lowest value) and FF (highest value) and the values are case-insensitive.
    ///     (2) A HTML color name. See https://htmlcolorcodes.com/color-names/ for a list of all valid color names.
    ///     Names are case-insensitive.
    /// 
    /// If we want to color the above text "Some text" in red, we thus can write the text as follows:
    /// <color=#FF0000>Some text</color> -or- <color=Red>Some text</color>
    /// 
    /// Each string can contain zero or more <color></color> tag pairs. If a string contains no such pair, the 
    /// specified default text color will be used. You can have multiple <color> tags side-by-side and 
    /// you can even have <color> tags inside of other <color> tags.
    /// 
    /// A valid <color> syntax thus is defined as follows (instead of a color value in the format'#[A-Fa-f0-9]{6}' a HTML color name
    /// can also be used):
    /// ...<color=#[A-Fa-f0-9]{6}>...</color>...
    /// where ... = [Optional text and <color></color> start/end tags]
    /// 
    /// Examples:
    ///  - "Tomorrow we will all meet at the <color=#FF0000">Town Center</color> to celebrate the end of harvest season!"
    ///  ---> Displays "Town Center" in red, the remaining text in the default text color.
    ///  
    /// - "Some <color=#FF0000>red</color>, some <color=Green>green</color> and some <color=#0000FF>blue</color>."
    /// ---> Displays "red" in red, "green" in green, "blue" in blue and the remaining text in the default text color.
    /// 
    /// - "<color=#000000>Some small <color=#C47902>light source</color> surrounded by darkness.</color>"
    /// ---> Displays "light source" in a orange and the remaining text in black.
    /// 
    /// Note that if there is an incorrect <color> tag syntax, the entire string color parsing fails.
    /// </summary>
    internal class StringColorParser
    {
        /// <summary>Provides access to the <see cref="IMonitor"/> API provided by SMAPI.</summary>
        private static readonly IMonitor Monitor = ToolkitMod._Monitor;

        /// <summary>
        /// Create a new instance of the <see cref="StringColorParser"/> class.
        /// </summary>
        /// <param name="defaultColor">The default text color to use in case color-parsing errors.</param>
        public StringColorParser(Color defaultColor)
        {
            DefaultColor = defaultColor;
        }

        /// <summary>The default text color.</summary>
        public Color DefaultColor { get; set; }

        /// <summary>
        /// Parses a string to determine which sections of the string are to be displayed in what color.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="textColorMappings">
        /// When this method returns, contains a collection of <seealso cref="TextColorInfo"/> objects 
        /// or <c>null</c> if the specified <paramref name="input"/> couldn't be parsed. This parameter is passed 
        /// uninitialized; any value originally supplied in result will be overwritten.
        /// </param>
        /// <returns><c>true</c> if the specified <paramref name="input"/> was parsed successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="input"/>is <c>null</c>.</exception>
        /// <remarks>If the specified <paramref name="input"/> contains no <color> tags, the <see cref="DefaultColor"/> will be used.</remarks>
        public bool TryParse(string input, out List<TextColorInfo> textColorMappings)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool couldParse = DoParse(0, 0, input, input, DefaultColor, DefaultColor, out ParseResultData parseResultData);
            if (!couldParse)
            {
                textColorMappings = null;
                return false;
            }

            textColorMappings = parseResultData.TextColorData;
            return true;
        }

        /// <summary>
        /// Parses a string to determine which sections of the string are to be displayed in what color.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="defaultColor">The default color for the string.</param>
        /// <param name="textColorData">
        /// When this method returns, contains a collection of <seealso cref="TextColorInfo"/> objects 
        /// or <c>null</c> if the specified <paramref name="input"/> couldn't be parsed. This parameter is passed 
        /// uninitialized; any value originally supplied in result will be overwritten.
        /// </param>
        /// <returns><c>true</c> if the specified <paramref name="input"/> was parsed successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="input"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The static <see cref="StringColorParser.Parse(string, Color)"/> method is equivalent to constructing a 
        /// <seealso cref="StringColorParser"/> object with the <seealso cref="StringColorParser(Color)"/> constructor 
        /// and calling the instance <seealso cref="TryParse(string, out List{TextColorInfo})(string)"/> method. 
        /// </remarks>
        public static bool TryParse(string input, Color defaultColor, out List<TextColorInfo> textColorData)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            bool couldParse = DoParse(0, 0, input, input, defaultColor, defaultColor, out ParseResultData parseResultData);
            if (!couldParse)
            {
                textColorData = null;
                return false;
            }

            textColorData = parseResultData.TextColorData;
            return true;
        }

        /// <summary>
        /// Parse a string and split it up into a collection of text -> color mappings. 
        /// </summary>
        /// <param name="level">The current number of opened color tags which have yet to be closed with a closing color tag during the parsing process.</param>
        /// <param name="currentIndexInOriginalString">The current index into the original string.</param>
        /// <param name="input">The original input string parsing was started on.</param>
        /// <param name="unhandledInputPart">The sub-string of the specified <paramref name="input"/> which has yet to be parsed.</param>
        /// <param name="defaultColor">The default text color to use if there is a color-parsing error.</param>
        /// <param name="levelColor">The text color to apply to characters in the current level.</param>
        /// <param name="parseResultData">The parsed data or <c>null</c> if the string couldn't be parsed.</param>
        /// <returns><c>true</c> if the specified string <paramref name="unhandledInputPart"/> was successfully parsed; otherwise, <c>false</c>.</returns>
        private static bool DoParse(int level, int currentIndexInOriginalString, string input, string unhandledInputPart, Color defaultColor, Color levelColor, out ParseResultData parseResultData)
        {
            // Immediately return if the string to parse is empty.
            if (unhandledInputPart.Equals(""))
            {
                parseResultData = new ParseResultData(
                    new List<TextColorInfo>() { new TextColorInfo("", levelColor) }, 
                    currentIndexInOriginalString
                    );

                return true;
            }

            // Get the different color start tags and color end tags in the current unparsed (sub-)string.
            var colorTagOpeningMatches = Regex.Matches(unhandledInputPart, "<color=(?<color>[^>]*)>");
            var colorTagClosingMatches = Regex.Matches(unhandledInputPart, "</color>");

            // In case the current string contains no color start tags any longer, the number of remaining color end tags has to match 
            // the number of currently pending color start tags. A pending color start tag (<color=#...>) is a tag which was alraedy 
            // reached by the parser, however, the parser hasn't reached its corresponding color end tag yet (</color>). The amount of pending 
            // start tags is specified by "level": A value of 'n' means 'n' pending color start tags.
            if (colorTagOpeningMatches.Count == 0 && colorTagClosingMatches.Count == level)
            {
                // If there is exactly one pending color start tag and there is exactly one remaining color end tag,
                // we return the text data the pair of tags is enclosing, the color for the data and report success.
                if (level > 0)
                {
                    var colorTagClosingStartIndex = colorTagClosingMatches[0].Index;
                    parseResultData = new ParseResultData(
                        new List<TextColorInfo>() { new TextColorInfo(unhandledInputPart.Substring(0, colorTagClosingStartIndex), levelColor) },
                        currentIndexInOriginalString + colorTagClosingStartIndex + colorTagClosingMatches[0].Length
                        );

                    return true;
                }

                // If there are no pending color start tags and no remaining color end tags, we return the text data
                // and report success.
                else if (level == 0)
                {
                    parseResultData = new ParseResultData(
                        new List<TextColorInfo>() { new TextColorInfo(unhandledInputPart, levelColor) },
                        currentIndexInOriginalString + unhandledInputPart.Length
                        );

                    return true;
                }
                else
                {
                    // Reaching this code part means there was a negative number of pending color start tags, 
                    // something which simply cannot happen without some major event (such as memory corruption)
                    // having taken place.
                    
                    parseResultData = null;
                    Monitor.Log("StringColorParser.DoParse(): Catastrophic failure! Number of pending color start tags cannot be negative!", LogLevel.Error);

                    return false;
                }
            }

            // The current string needs to contain the same amount of color end tags as the amount of color start tags of 
            // this string + the amount of pending color start tags (which were met by the parser previously when 
            // walking along the original input). If that is not the case, we have one of two different scenarios:
            //  - 1: There is at least one color start tag in the original input which does not have a matching color end tag
            //  - 2: There is at least one color end tag in the original input which does not have a matching color start tag
            //
            // In both cases, we return and report failure.
            else if (colorTagClosingMatches.Count != level + colorTagOpeningMatches.Count)
            {
                parseResultData = null;
                Monitor.Log($"StringColorParser.DoParse(): Mismatch between the number of color start tags and color end tags in string \"{unhandledInputPart}\"!", 
                    LogLevel.Error);

                return false;
            }

            // The current string contains at least one more color start tag which the parser has yet to meet -> set up recursion
            else if (colorTagOpeningMatches.Count > 0)
            {
                // The number of color starting tags matches the number of closing tags in the string:

                // We now need to check if the start/end tags follow the correct syntax, that is, for every color end tag 
                // there is an individuel color start tag. Suppose we have an input string like this: 
                //      </color>Some Text<color=#012345>
                // with the parser currently parsing the substring (same as input string, first parsing step):
                //      </color>Some Text<color=#012345>
                // The above input string would still be valid at this point in the code flow but is actually invalid.
                //
                // A second case, which is actually valid, is the following:
                // Suppose we have an input string like this:
                //      <color=#FF0000>Some text in red</color><color=#0000FF>and blue.</color>
                // with the parser currently parsing the substring:
                //      Some text in red</color><color=#0000FF>and blue.</color>
                //
                // Both cases differ in the number of pending color start tags the parser has already met on his parsing process.
                // As we can see by looking at the examples, this is an important info, as it determines which case is 
                // valid (second example) and which is not (first example). Fortunately, at any given time, the parser knows exactly
                // how many pending color start tags there are in the current parser step (as mentioned above, that number is hold 
                // by the "level" argument).
                //
                // In the first case, the parser sees that there is a color end tag but since there are no pending color start tags.
                // This signals invalid syntax (at least one color end tag without a matching color start tag) and we report failure.
                //
                // In the second case, when only looking at the substring, the parser immediately sees a mismatch between the amount of 
                // color start tags and color end tags (one more end tag than start tag). However, the parser is aware of exactly one 
                // pending color start tag and thus the number of start tags and end tags actually matches. The parser uses the first color 
                // end tag to close the pending color start tag; it returns the enclosed string ("Some text in red"), its color (#FF0000 = red) 
                // and report success.
                if (colorTagClosingMatches[0].Index < colorTagOpeningMatches[0].Index)
                {
                    if (level > 0)
                    {
                        // Case of second example above: Return data and report success
                        var colorTagClosingStartIndex = colorTagClosingMatches[0].Index;
                        parseResultData = new ParseResultData(
                            new List<TextColorInfo>() { new TextColorInfo(unhandledInputPart.Substring(0, colorTagClosingStartIndex), levelColor) },
                            currentIndexInOriginalString + colorTagClosingStartIndex + colorTagClosingMatches[0].Length
                            );

                        return true;
                    }

                    // Case of first example above: Report failure
                    parseResultData = null;
                    Monitor.Log($"StringColorParser.DoParse(): There is at least one color end tag without a corresponding color close tag in string \"{unhandledInputPart}\"!",
                        LogLevel.Error);

                    return false;
                }

                // At this point in the code, we are (potentially) looking at nested color start tags (thus multiple pending
                // start tags). Example:
                //      <color=#FF0000>Some text in red <color=#0000FF>and blue.</color></color>
                // as input string, with the parser currently parsing the following substring:
                //      Some text in red <color=#0000FF>and blue.</color></color>
                //
                // In such cases, the parser needs to take any text data before the inner color start tag (<color=#0000FF>)
                // and maps it to the outer pending color start tag (<color=#FF0000>). The parser then proceeds to parse
                // the inner color start tags before it continues to parse the still pending outer start tag.


                // At this point, the parser knows there is a color start tag and proceeds to parse the inner data of that tag.
                // All data prior to that start tag, if any, in the current unparsed input substring will be mapped to 
                // a) the outer pending start tag or b) gets assigned the default text color (there is an implicit enclosing pair
                // of <color></color> around every input string specifying as color the given default text color).

                var colorTagOpeningMatch = colorTagOpeningMatches[0];
                parseResultData = new ParseResultData(
                    new List<TextColorInfo>() { new TextColorInfo(unhandledInputPart.Substring(0, colorTagOpeningMatch.Index), levelColor) },
                    currentIndexInOriginalString + colorTagOpeningMatch.Index + colorTagOpeningMatch.Length
                    );

                // Get the text color for the block of text enclosed by the current color start tag. Use the specified
                // default text color if no valid color was specified.
                Color textColor;
                string sColor = colorTagOpeningMatch.Groups["color"].Value;
                if (!ColorHelper.TryGetColorFromString(sColor, out Color? color))
                {                    
                    textColor = defaultColor;
                    Monitor.Log($"StringColorParser: The specified color value \"{sColor}\" is not a valid color! " +
                        $"Using the specified default text color \"{defaultColor}\" instead.");
                }
                else
                {                    
                    textColor = color.Value;
                }


                // Parse the inner data of color start tag. The inner data can also contain color tags so we recursively set up the 
                // parsing process.
                bool couldParse = DoParse(
                    level + 1,
                    currentIndexInOriginalString + colorTagOpeningMatch.Index + colorTagOpeningMatch.Length,
                    input,
                    input.Substring(currentIndexInOriginalString + colorTagOpeningMatch.Index + colorTagOpeningMatch.Length),
                    defaultColor,
                    textColor,
                    out ParseResultData nestedParseResultData
                    );

                // If there was an error parsing the inner data of the color start tag: Report failure.
                if (!couldParse)
                {
                    parseResultData = null;
                    return false;
                }

                // The inner data of start tag was parsed successully: Add its resulting parsed data (a text -> color mapping)
                // to the list of already parsed data for the original input.
                parseResultData.TextColorData.AddRange(nestedParseResultData.TextColorData);
                parseResultData.StartIndexOfNextUnhandledStringPart = nestedParseResultData.StartIndexOfNextUnhandledStringPart;

                // If the complete string has been parsed -> Return the complete parsed data and report success.
                // To visualize: See the following input string:
                //      <color=#FF0000>Some red text.</color>
                //
                // The inner data ("Some red text.") has been parsed successfully and the color start tag has been parsed (it is 
                // no longer pending). The parser would then proceed to parse any remaining data following this color tag, however,
                // in this case, we already reached the end of the string. Thus the parser is finished, returns the parsed data and 
                // reports success.
                if (parseResultData.StartIndexOfNextUnhandledStringPart >= input.Length)
                {
                    return true;
                }

                // This time there is still data following a successfully parsed <color></color> pair.
                // The parser now proceeds to parse this remaining data. To visualize: See the following input string:
                //      <color=#FF0000>Some red text.</color> Some more text. <color=#0000FF>And even more text!</color>
                //
                // After successfully parsing "<color=#FF0000>Some red text.</color>", the parser now needs to parse the 
                // remaining string, which contains another <color></color> pair.
                // (The above example is also valid for pending color start tags and their inner data (which can easily contain
                // multiple side-by-side <color></color> tags.)
                couldParse = DoParse(
                    level,
                    parseResultData.StartIndexOfNextUnhandledStringPart,
                    input,
                    input.Substring(parseResultData.StartIndexOfNextUnhandledStringPart),
                    defaultColor,
                    levelColor,
                    out nestedParseResultData
                    );

                // If the parser failed to parse the remaining input data on this "level": Report failure.
                if (!couldParse)
                {
                    parseResultData = null;
                    return false;
                }

                // The remaining input data for the pending start tag was parsed successully: Add its resulting parsed data 
                // (a text -> color mapping) to the list of already parsed data for the original input and report success.
                parseResultData.TextColorData.AddRange(nestedParseResultData.TextColorData);
                parseResultData.StartIndexOfNextUnhandledStringPart = nestedParseResultData.StartIndexOfNextUnhandledStringPart;

                return true;
            }
            else
            {
                // Reaching this code part means there was a negative number of color start tags, 
                // something which simply cannot happen without some major event (such as memory corruption)
                // having taken place.
                
                parseResultData = null;
                Monitor.Log("StringColorParser.DoParse(): Catastrophic failure! Number of color start tags cannot be negative!", LogLevel.Error);

                return false;
            }
        }

        /// <summary>
        /// Encapsulates the resulting data of a text color parsing run.
        /// </summary>
        private class ParseResultData
        {
            /// <summary>
            /// Create a new instance of the <see cref="ParseResultData"/> class.
            /// </summary>
            /// <param name="textColorData">The text -> color mapping results for a parsing (sub-) process.</param>
            /// <param name="startIndexOfNextUnhandledStringPart">The first position in the string, on which the parsing process has been started, which has not been parsed yet.</param>
            public ParseResultData(List<TextColorInfo> textColorData, int startIndexOfNextUnhandledStringPart)
            {
                TextColorData = textColorData;
                StartIndexOfNextUnhandledStringPart = startIndexOfNextUnhandledStringPart;
            }

            /// <summary>
            /// A collection of <see cref="TextColorInfo"/> objects for the current parsing process.
            /// </summary>
            public List<TextColorInfo> TextColorData { get; }

            /// <summary>
            /// The first position in the string, on which the parsing process has been started, which has not been parsed yet.
            /// </summary>
            public int StartIndexOfNextUnhandledStringPart { get; set; }
        }
    }
}
