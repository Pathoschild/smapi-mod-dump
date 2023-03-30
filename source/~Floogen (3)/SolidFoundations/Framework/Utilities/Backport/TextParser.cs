/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Utilities.Backport
{
    // TODO: When updated to SDV v1.6, this class should be deleted in favor of using StardewValley.TextParser
    internal class TextParser
    {
        public delegate bool ParseDialogueDelegate(string[] tag_split, ref string substitution, Random r);

        protected static Farmer _targetFarmer;

        public static string ParseText(string format, Random r = null, ParseDialogueDelegate handle_additional_tags = null, Farmer target_farmer = null)
        {
            try
            {
                TextParser._targetFarmer = target_farmer;
                if (TextParser._targetFarmer == null)
                {
                    TextParser._targetFarmer = Game1.player;
                }
                if (format == null)
                {
                    return null;
                }
                format = format.Replace("\\n", "\n");
                if (r == null)
                {
                    r = Game1.random;
                }
                for (int i = 0; i < format.Length; i++)
                {
                    if (format[i] == '[')
                    {
                        i = TextParser._ParseTags(ref format, i, r, handle_additional_tags);
                    }
                }
                return format.Replace('\u00a0', ' ');
            }
            finally
            {
                TextParser._targetFarmer = null;
            }
        }

        protected static int _ParseTags(ref string format, int start_index, Random r, ParseDialogueDelegate handle_additional_tags)
        {
            for (int i = start_index + 1; i < format.Length; i++)
            {
                if (format[i] == '[')
                {
                    i = TextParser._ParseTags(ref format, i, r, handle_additional_tags);
                }
                else if (format[i] == ']')
                {
                    string tag = format.Substring(start_index + 1, i - start_index - 1);
                    string replacement = null;
                    if (TextParser._ParseTag(tag, ref replacement, r, handle_additional_tags))
                    {
                        format = format.Remove(start_index, i - start_index + 1);
                        format = format.Insert(start_index, replacement);
                        return start_index + replacement.Length - 1;
                    }
                    return i;
                }
            }
            return format.Length - 1;
        }

        public static string EscapeSpaces(string text)
        {
            return text.Replace(' ', '\u00a0');
        }

        protected static bool _ParseTag(string tag, ref string replacement, Random r, ParseDialogueDelegate handle_additional_tags)
        {
            string[] array = tag.Split(' ');
            if (handle_additional_tags != null && handle_additional_tags(array, ref replacement, r))
            {
                return true;
            }
            if (array[0] == "LocalizedText")
            {
                if (array.Length > 2)
                {
                    object[] array2 = new object[array.Length - 2];
                    for (int i = 2; i < array.Length; i++)
                    {
                        array2[i - 2] = array[i];
                    }
                    replacement = Game1.content.LoadString(array[1], array2);
                }
                else
                {
                    replacement = Game1.content.LoadString(array[1]);
                }
                return true;
            }
            if (array[0] == "LocationName")
            {
                GameLocation locationFromName = Game1.getLocationFromName(array[1]);
                if (locationFromName != null)
                {
                    replacement = locationFromName.NameOrUniqueName;
                    return true;
                }
                return false;
            }
            if (array[0] == "DayOfMonth")
            {
                replacement = Game1.dayOfMonth.ToString();
                return true;
            }
            if (array[0] == "Season")
            {
                replacement = Game1.CurrentSeasonDisplayName;
                return true;
            }
            if (array[0] == "CharacterName")
            {
                NPC characterFromName = Game1.getCharacterFromName(array[1]);
                if (characterFromName != null)
                {
                    replacement = characterFromName.displayName;
                    return true;
                }
                return false;
            }
            if (array[0] == "FarmName")
            {
                replacement = TextParser._targetFarmer.farmName;
                return true;
            }
            if (array[0] == "FarmerUniqueID")
            {
                replacement = TextParser._targetFarmer.UniqueMultiplayerID.ToString();
                return true;
            }
            if (array[0] == "PositiveAdjective")
            {
                replacement = Lexicon.getRandomPositiveAdjectiveForEventOrPerson();
                return true;
            }
            if (array[0] == "ArticleFor")
            {
                replacement = Lexicon.getProperArticleForWord(array[1]);
                return true;
            }
            if (array[0] == "GenderedText")
            {
                if (TextParser._targetFarmer.IsMale)
                {
                    replacement = array[1];
                }
                else
                {
                    replacement = array[2];
                }
                return true;
            }
            if (array[0] == "SpouseGenderedText")
            {
                bool flag = false;
                if (TextParser._targetFarmer.team.GetSpouse(TextParser._targetFarmer.UniqueMultiplayerID).HasValue)
                {
                    if (Game1.getFarmerMaybeOffline(TextParser._targetFarmer.team.GetSpouse(TextParser._targetFarmer.UniqueMultiplayerID).Value).IsMale)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (TextParser._targetFarmer.getSpouse() == null)
                    {
                        return false;
                    }
                    if (TextParser._targetFarmer.getSpouse().Gender == 0)
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    replacement = array[1];
                }
                else
                {
                    replacement = array[2];
                }
                return true;
            }
            if (array[0] == "SpouseFarmerText")
            {
                if (TextParser._targetFarmer.team.GetSpouse(TextParser._targetFarmer.UniqueMultiplayerID).HasValue)
                {
                    replacement = array[1];
                }
                else
                {
                    if (TextParser._targetFarmer.getSpouse() == null)
                    {
                        return false;
                    }
                    replacement = array[2];
                }
                return true;
            }
            if (array[0] == "DataString")
            {
                try
                {
                    Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>(array[1]);
                    string key = string.Join(" ", array, 2, array.Length - 3);
                    if (dictionary.ContainsKey(key))
                    {
                        string text = dictionary[key];
                        int index = int.Parse(array[^1]);
                        replacement = GetStringAtIndex(text.Split('/'), index);
                        if (replacement == null)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else if (array[0] == "EscapedText")
            {
                replacement = string.Join(" ", array.Skip(1).ToArray());
                replacement = TextParser.EscapeSpaces(replacement);
                return true;
            }
            return false;
        }

        public static string GetStringAtIndex(string[] array, int index, string default_value = null, bool ignore_blank = false)
        {
            string text = GetDataAtIndex(array, index, default_value);
            if (ignore_blank && string.IsNullOrWhiteSpace(text))
            {
                text = null;
            }
            return text;
        }

        public static T GetDataAtIndex<T>(T[] array, int index, T default_value = default(T))
        {
            if (array != null && index >= 0 && index < array.Length)
            {
                return array[index];
            }
            return default_value;
        }
    }
}
