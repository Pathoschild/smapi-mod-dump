/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone
{
    public static class Utility
    {
        internal static List<char> spaceEquivalents = new List<char>
        {
            '|',
            '_',
            '[',
            ']',
            '(',
            ')',
            '{',
            '}',
            ',',
            '.'
        };

        public static string cleanup(string dirty)
        {
            string outString = "";
            Logger.Log("Given " + dirty);
            dirty = dirty.Replace("\n", " ");
            dirty = dirty.Replace("\\n", " ");
            Logger.Log("Line breaks cleaned as " + dirty);
            for (int index = 0; index < dirty.Length; index++)
            {
                if ((dirty[index] == ' ' && outString.EndsWith(" ")))
                    continue;
                if (spaceEquivalents.Contains(dirty[index]))
                {
                    outString += (outString.EndsWith(" ") ? "" : " ");
                    continue;
                }
                outString += dirty[index];
            }
            Logger.Log("Returning " + outString);
            return outString.Trim(' ');
        }
    }
}
