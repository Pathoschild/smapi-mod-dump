/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
            if(villagerIndex.ContainsKey(residentType))
            {
                
                villagerList = villagerIndex[residentType];

            }

            return villagerList;

        }

    }
}
