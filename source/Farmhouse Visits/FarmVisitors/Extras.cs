/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/FarmhouseVisits
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmVisitors
{
    /* Extras *
     * extra methods that are important, yet don't fit into specific categories (e.g titles, assetloading, booleans) */
    internal class Extras
    {
        /* GMCM related */
        public static string ExtrasTL()
        {
            string result = ModEntry.Help.Translation.Get("config.Extras");
            return result;
        }
        public static string BlacklistTL()
        {
            string result = ModEntry.Help.Translation.Get("config.Blacklist.name");
            return result;
        }
        public static string BlacklistTTP()
        {
            string result = ModEntry.Help.Translation.Get("config.Blacklist.description");
            return result;
        }
        internal static string VisitConfiguration()
        {
            string result = ModEntry.Help.Translation.Get("config.VisitConfiguration");
            return result;
        }
        internal static string DebugTL()
        {
            string result = ModEntry.Help.Translation.Get("config.Debug.name");
            return result;
        }

        /* Related to custom visits*/
        internal static void AssetRequest(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("mistyspring.farmhousevisits/Schedules", true))
            {
                e.LoadFrom(
                () => new Dictionary<string, ScheduleData>(),
                AssetLoadPriority.Medium
            );
            }
        }
        internal static bool IsScheduleValid(KeyValuePair<string, ScheduleData> pair)
        {
            if(pair.Value.From is 600 || pair.Value.From is 0)
            {
                ModEntry.Mon.Log(ModEntry.Help.Translation.Get("CantBe600"), LogLevel.Error);
                return false;
            }
            if(pair.Value.To is 2600)
            {
                ModEntry.Mon.Log(ModEntry.Help.Translation.Get("CantBe2600"), LogLevel.Error);
                return false;
            }
            if(!ModEntry.NameAndLevel.Keys.Contains(pair.Key))
            {
                ModEntry.Mon.Log(ModEntry.Help.Translation.Get("NotInSave"), LogLevel.Error);
                return false;
            }
            if(pair.Value.From > pair.Value.To && pair.Value.To is not 0)
            {
                ModEntry.Mon.Log(ModEntry.Help.Translation.Get("FromHigherThanTo"), LogLevel.Error);
                return false;
            }
            return true;
        }

        /* in the future, see if using these
         * instead of using .Any()  
         * (as of now, they make the whole mod fail, though).
         */
        internal static bool AnyInList(List<string> list)
        {
            try
            {
                return list.Count != 0;
            }
            catch
            {
                return false;
            }
        }
        internal static bool AnySchedule()
        {
            try
            {
                return ModEntry.SchedulesParsed.Count != 0;
            }
            catch
            {
                return false;
            }
        }
        internal static bool AnySchedule(Dictionary<string, ScheduleData> sch)
        {
            try
            {
                return sch.Count != 0;
            }
            catch
            {
                return false;
            }
        }
    }

    /* For vanilla NPCS *
     * Gets dialogue for when ur in-laws visit */
    internal class Vanillas
    {
        public static bool InLawOfSpouse(string who)
        {
            string of;

            of = who switch
            {
                "Caroline" => "Abigail",
                "Pierre" => "Abigail",

                "Demetrius" => "Maru&Seb",
                "Robin" => "Maru&Seb",

                "Emily" => "Haley", //emily is inlaw of haley
                "Haley" => "Emily", //and haley of emily.

                "Evelyn" => "Alex",
                "George" => "Alex",

                "Pam" => "Penny",

                "Jodi" => "Sam",
                "Kent" => "Sam",

                "Marnie" => "Shane",

                _ => "none",
            };

            foreach(string spouse in ModEntry.MarriedNPCs)
            {
                if (spouse.Equals("Maru") || spouse.Equals("Sebastian"))
                {
                    if (of.Equals("Maru&Seb"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (spouse.Equals(of))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            //if none applied
            return false;
        }

        public static string GetInLawDialogue(string who)
        {
            int choice = Game1.random.Next(0,11);
            string result;

            if (choice >= 5)
            {
                int ran = Game1.random.Next(1, 4);
                result = who switch
                {
                    //for abigail
                    "Caroline" => ModEntry.Help.Translation.Get($"InLaw.Abigail.{ran}"),
                    "Pierre" => ModEntry.Help.Translation.Get($"InLaw.Abigail.{ran}"),

                    //for alex
                    "Evelyn" => ModEntry.Help.Translation.Get($"InLaw.Alex.{ran}"),
                    "George" => ModEntry.Help.Translation.Get($"InLaw.Alex.{ran}"),

                    //for haley and emily
                    "Emily" => ModEntry.Help.Translation.Get($"InLaw.Haley.{ran}"),
                    "Haley" => ModEntry.Help.Translation.Get($"InLaw.Emily.{ran}"),

                    //for maru
                    "Demetrius" => ModEntry.Help.Translation.Get($"InLaw.Maru.{ran}"),

                    //for penny
                    "Pam" => ModEntry.Help.Translation.Get($"InLaw.Penny.{ran}"),

                    //for sam
                    "Jodi" => ModEntry.Help.Translation.Get($"InLaw.Sam.{ran}"),
                    "Kent" => ModEntry.Help.Translation.Get($"InLaw.Sam.{ran}"),

                    //for sebastian
                    "Robin" => ModEntry.Help.Translation.Get($"InLaw.Sebastian.{ran}"),

                    //for shane
                    "Marnie" => ModEntry.Help.Translation.Get($"InLaw.Shane.{ran}"),

                    _ => null,
                };
            }
            else
            {
                int ran = Game1.random.Next(1, 16);

                string notParsed = ModEntry.Help.Translation.Get($"InLaw.Generic.{ran}");
                string spousename = GetSpouseName(who);

                result = string.Format(notParsed, spousename);
            }

            return result;
        }

        public static string GetSpouseName (string who)
        {
            string RelatedTo = who switch
            {
                "Caroline" => "Abigail",
                "Pierre" => "Abigail",

                "Demetrius" => "Maru&Seb",
                "Robin" => "Maru&Seb",

                "Emily" => "Haley", //emily is inlaw of haley
                "Haley" => "Emily", //and haley of emily.

                "Evelyn" => "Alex",
                "George" => "Alex",

                "Pam" => "Penny",

                "Jodi" => "Sam",
                "Kent" => "Sam",

                "Marnie" => "Shane",

                _ => "none",
            };

            foreach(string spouse in ModEntry.MarriedNPCs)
            {
                if (spouse.Equals("Maru") && RelatedTo.Equals("Maru&Seb"))
                {
                    return "Maru";
                }
                else if(spouse.Equals("Sebastian") && RelatedTo.Equals("Maru&Seb"))
                {
                    return "Sebastian";
                }
                else if(spouse.Equals(RelatedTo))
                {
                    return spouse;
                }
            }

            throw new ArgumentException("Character must be related to a vanilla spouse.", nameof(who));
        }

        internal static string AskAboutKids(Farmer player)
        {
            var ran = Game1.random.Next(1, 6);

            var kids = player.getChildren();
            string result;
            
            if(kids.Count is 1)
            {
                var notformatted = ModEntry.Help.Translation.Get($"ask.singlechild.{ran}");
                result = String.Format(notformatted, kids[0].Name);
            }
            else
            {
                var notformatted = ModEntry.Help.Translation.Get($"ask.multiplechild.{ran}");
                result = String.Format(notformatted, kids[0].Name, kids[1].Name);
            }

            return result;
        }
    }

    /* For modded NPC data *
     * (e.g get characters theyre family of) */
    internal class Moddeds
    {
        //get all relatives for NPC
        internal static List<string> GetInlawOf(Dictionary<string, string> reference, string who)
        {
            if(!reference.Keys.Contains(who))
            {
                ModEntry.Mon.Log("NPC not in dictionary!");
                return null;
            }

            List<string> result = new();

            //set new char array
            char[] ToSplitWith = new char[1];
            ToSplitWith[0] = '/';

            //get relationships field
            var CloseOnes = SpanSplit.GetNthChunk(reference[who], ToSplitWith, 9);
            if (CloseOnes.IsEmpty)
                return null;

            //pass to string, split by space
            string CloseString = CloseOnes.ToString();
            var CloseArray = CloseString.Split(' ');

            //foreach. if word isnt alias, add
            foreach (var word in CloseArray)
            {
                if (!word.Contains('\''))
                {
                    result.Add(word);
                }
            }

            return result;
        }
        //get name of spouse the NPC is related to
        public static string GetRelativeName(string who)
        {
            if (!ModEntry.InLaws.Any())
            {
                return null;
            }

            if (!ModEntry.InLaws.Keys.Contains(who))
            {
                return null;
            }
            if(ModEntry.InLaws[who] is null)
            {
                return null;
            }

            foreach(string spousename in ModEntry.MarriedNPCs)
            {
                if (ModEntry.InLaws[who].Contains(spousename))
                {
                    return spousename;
                }
            }

            return null;
        }
        //get raw dialogue (needs formatting)
        public static string GetDialogueRaw()
        {
            int ran = Game1.random.Next(0, 16);
            string Raw = ModEntry.Help.Translation.Get($"InLaw.Generic.{ran}");

            //string result = string.Format(Raw, GetSpouseName(who));
            
            return Raw;
        }
        public static bool IsVanillaInLaw(string who)
        {
            bool result = who switch
            {
                "Caroline" => true,
                "Pierre" => true,
                "Demetrius" => true,
                "Robin" => true,
                "Emily" => true,
                "Haley" => true,
                "Evelyn" => true,
                "George" => true,
                "Pam" => true,
                "Jodi" => true,
                "Kent" => true,
                "Marnie" => true,

                _ => false,
            };

            return result;
        }
    }

    public static class SpanSplit
    {
        //code below taken from atravita !

        /// <summary>
        /// Faster replacement for str.Split()[index];.
        /// </summary>
        /// <param name="str">String to search in.</param>
        /// <param name="deliminators">deliminator to use.</param>
        /// <param name="index">index of the chunk to get.</param>
        /// <returns>a readonlyspan char with the chunk, or an empty readonlyspan for failure.</returns>
        /// <remarks>Inspired by the lovely Wren.</remarks>
        public static ReadOnlySpan<char> GetNthChunk(this string str, char[] deliminators, int index = 0)
        {
            if (index < 0)
            {
                throw new ArgumentException();
            }

            int start = 0;
            int ind = 0;
            while (index-- >= 0)
            {
                ind = str.IndexOfAny(deliminators, start);
                if (ind == -1)
                {
                    // since we've previously decremented
                    // index, check against -1;
                    // this means we're done.
                    if (index == -1)
                    {
                        return str.AsSpan()[start..];
                    }

                    // else, we've run out of entries
                    // and return an empty span to mark as failure.
                    return ReadOnlySpan<char>.Empty;
                }

                if (index > -1)
                {
                    start = ind + 1;
                }
            }
            return str.AsSpan()[start..ind];
        }
    }
}

