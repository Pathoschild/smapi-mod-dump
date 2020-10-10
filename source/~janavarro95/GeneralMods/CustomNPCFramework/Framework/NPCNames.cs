/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using CustomNPCFramework.Framework.Enums;
using StardewValley;

namespace CustomNPCFramework.Framework
{
    /// <summary>Used as a class to hold all of the possible NPC names.</summary>
    public class NpcNames
    {
        /// <summary>Holds all of the NPC male names.</summary>
        public static List<string> maleNames = new List<string>
        {
            "Freddy",
            "Josh",
            "Ash"
        };

        /// <summary>Holds all of the NPC female names.</summary>
        public static List<string> femaleNames = new List<string>
        {
            "Rebecca",
            "Sierra",
            "Lisa"
        };

        /// <summary>Holds all of the NPC gender non-binary names.</summary>
        public static List<string> otherGenderNames = new List<string>
        {
            "Jayden",
            "Ryanne",
            "Skylar"
        };

        /// <summary>Get a gender appropriate name from the pool of NPC names.</summary>
        public static string getRandomNpcName(Genders gender)
        {
            if (gender == Genders.female)
            {
                int rand = Game1.random.Next(0, femaleNames.Count - 1);
                return femaleNames.ElementAt(rand);
            }

            if (gender == Genders.male)
            {
                int rand = Game1.random.Next(0, maleNames.Count - 1);
                return maleNames.ElementAt(rand);
            }

            if (gender == Genders.other)
            {
                int rand = Game1.random.Next(0, otherGenderNames.Count - 1);
                return otherGenderNames.ElementAt(rand);
            }

            return "";
        }
    }
}
