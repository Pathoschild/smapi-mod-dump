/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System.Collections.Generic;

namespace StardewDruid.Map
{
    static class VillagerData
    {

        public static List<string> VillagerIndex(string residentType)
        {

            List<string> villagerList = new();

            Dictionary<string, List<string>> villagerIndex = new()
            {
                ["mountain"] = new(){
                    "Sebastian", "Sam",
                    "Maru", "Abigail",
                    "Robin", "Demetrius", "Linus", "Dwarf",
                },
                ["town"] = new(){

                    "Alex", "Elliott", "Harvey",
                    "Emily", "Penny",
                    "Caroline", "Clint", "Evelyn", "George", "Gus", "Jodi", "Kent", "Lewis", "Pam", "Pierre", "Vincent",
                },
                ["forest"] = new(){

                    "Shane",
                    "Leah", "Haley",
                    "Marnie", "Jas", "Krobus", "Wizard", "Willy",
                }
            };

            if (villagerIndex.ContainsKey(residentType))
            {

                villagerList = villagerIndex[residentType];

            }

            return villagerList;

        }

        public static Dictionary<string, int> Affinity()
        {
            Dictionary<string, int> affinities = new()
            {

                // Skeptical / Worldly
                ["Demetrius"] = 0,
                ["Pam"] = 0,
                ["Penny"] = 0,
                ["Kent"] = 0,
                ["Harvey"] = 0,
                ["Maru"] = 0,
                ["Robin"] = 0,

                // Ignorant / Don't know
                ["Jas"] = 1,
                ["Alex"] = 1,
                ["Vincent"] = 1,
                ["Sam"] = 1,
                ["Haley"] = 1,
                ["Marnie"] = 1,
                
                // Indifferent / Unconcerned
                ["Shane"] = 2,
                ["Jodi"] = 2,
                ["Gus"] = 2,
                ["Lewis"] = 2,
                ["Pierre"] = 2,
                ["George"] = 2,

                // Enthusiastic / Inspired
                ["Leah"] = 3,
                ["Emily"] = 3,
                ["Elliott"] = 3,
                ["Abigail"] = 3,
                ["Sebastian"] = 3,
                ["Caroline"] = 3,

                // Matter of Fact / Concerned
                ["Evelyn"] = 4,
                ["Clint"] = 4,
                ["Dwarf"] = 4,
                ["Marlon"] = 4,
                ["Willy"] = 4,
                ["Mateo"] = 4,

                // Esoteric Knowledge
                ["Krobus"] = 5,
                ["Linus"] = 5,
                ["Wizard"] = 5,

            };

            return affinities;

        }

    }

}
