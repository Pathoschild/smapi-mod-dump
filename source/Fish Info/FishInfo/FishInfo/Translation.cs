/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/FishInfo
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FishInfo
{
    static class Translation
    {
        private static ITranslationHelper i18n = ModEntry.helper.Translation;
        private static Dictionary<string, string> LocationPairs = new Dictionary<string, string>()
        {
            { "railroad","Strings\\StringsFromCSFiles:MapPage.cs.11109" },
            { "town","Strings\\StringsFromCSFiles:MapPage.cs.11190" },
            { "mountain","Strings\\StringsFromCSFiles:MapPage.cs.11177" },
            { "backwoods","Strings\\StringsFromCSFiles:MapPage.cs.11065" },
            { "busstop","Strings\\StringsFromCSFiles:MapPage.cs.11066" },
            { "desert","Strings\\StringsFromCSFiles:MapPage.cs.11062" },
            { "undergroundmine","Strings\\StringsFromCSFiles:MapPage.cs.11098" },
            { "beach","Strings\\StringsFromCSFiles:MapPage.cs.11174" },
            { "woods","Strings\\StringsFromCSFiles:MapPage.cs.11114" },
            { "sewer","Strings\\StringsFromCSFiles:MapPage.cs.11089" },
            { "farm","Strings\\StringsFromCSFiles:NPC.cs.4485" }
        };

        internal static string GetString(string key)
        {
            return i18n.Get(key);
        }
        internal static string GetString(string key, object tokens)
        {
            return i18n.Get(key, tokens);
        }

        internal static string BaseGameTranslation(string key)
        {
            return Game1.content.LoadString(key);
        }

        internal static string GetString(Season s)
        {
            string toReturn = "";
            if (s == Season.All_seasons)
            {
                return GetString("season.all");
            }

            if (s.HasFlag(Season.Spring))
            {
                toReturn += BaseGameTranslation("Strings\\StringsFromCSFiles:Utility.cs.5680") + ", ";
            }
            if (s.HasFlag(Season.Summer))
            {
                toReturn += BaseGameTranslation("Strings\\StringsFromCSFiles:Utility.cs.5681") + ", ";
            }
            if (s.HasFlag(Season.Fall))
            {
                toReturn += BaseGameTranslation("Strings\\StringsFromCSFiles:Utility.cs.5682") + ", ";
            }
            if (s.HasFlag(Season.Winter))
            {
                toReturn += BaseGameTranslation("Strings\\StringsFromCSFiles:Utility.cs.5683") + ", ";
            }

            if(toReturn == "")
            {
                return GetString("season.all");
            }

            return toReturn.Substring(0, toReturn.LastIndexOf(','));
        }

        internal static string GetString(Weather w)
        {
            string toReturn = "";

            if (w.HasFlag(Weather.Sun))
            {
                toReturn += GetString("weather.sun") + ", ";
            }
            if (w.HasFlag(Weather.Rain))
            {
                toReturn += GetString("weather.rain") + ", ";
            }

            if(toReturn == "")
            {
                toReturn += GetString("weather.none") + ", ";
            }

            return toReturn.Substring(0, toReturn.LastIndexOf(','));
        }

        internal static string GetLocationName(string Location)
        {
            string data = "";
            try
            {
                data = BaseGameTranslation(LocationPairs.First(x => x.Key.Equals(Location, System.StringComparison.OrdinalIgnoreCase)).Value);
            }
            catch
            {
                string text = GetString($"location.{Location.ToLower()}".Trim());

                data = text.Contains("no translation") ?
                    Regex.Replace(char.ToUpper(Location[0]) + Location.Substring(1), "([A-Z0-9]+)", " $1").Trim() :
                    text;
            }

            return data;
        }

    }
    

}
