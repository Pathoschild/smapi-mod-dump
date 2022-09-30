/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HaulinOats/StardewMods
**
*************************************************/

using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace DynamicCrops
{
    public class Helpers
    {
        public Helpers()
        {
        }

        public static int GetRandomIntegerInRange(int min, int max)
        {
            return new Random().Next(min, max + 1);
        }

        public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.IndexOf(Find);
            if (Place == -1) return Source;
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int Place = Source.LastIndexOf(Find);
            if (Place == -1) return Source;
            string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            return result;
        }

        public static string Capitalize(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}

