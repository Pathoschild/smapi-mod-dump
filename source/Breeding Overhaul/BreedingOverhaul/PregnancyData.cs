/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StarAmy/BreedingOverhaul
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;


namespace BreedingOverhaul
{
    public class PregnancyData
    {
        public Dictionary<string, string> PreganancyItems;
        public Dictionary<string, List<string>> Offspring;

        public PregnancyData()
        {
            PreganancyItems = new Dictionary<string, string>();
            Offspring = new Dictionary<string, List<string>>();

        }

        public bool MatchingPregnancyItem(FarmAnimal animal, SObject o)
        {
            if (animal == null || o == null)
            {
                return false;
            }
            else if (!PreganancyItems.ContainsKey(animal.displayType))
            {
                ModEntry.MyMonitor.Log($"Dont know anything about {animal.displayType}", LogLevel.Trace);
                return false;
            }
            return PreganancyItems.ContainsKey(animal.displayType) && PreganancyItems[animal.displayType].Equals(o.Name);
        }

        public string GetPregnancyItemName(FarmAnimal animal)
        {
            if (PreganancyItems.ContainsKey(animal.displayType))
            {
                return PreganancyItems[animal.displayType];
            }
            ModEntry.MyMonitor.Log($"Dont know anything about {animal.displayType}", LogLevel.Trace);
            return "";
        }

        public string GetRandomOffspringType(FarmAnimal animal)
        {
            ModEntry.MyMonitor.Log($"GetRandomOffspringType, Parent is {animal.Name} type {animal.type.Value} display type {animal.displayType}", LogLevel.Trace);
            string ret = "";
            if (animal == null)
            {
                return "";
            }

            if(ModEntry.pregnancyData.Offspring.ContainsKey(animal.type.Value)) {
                List<string> animals = ModEntry.pregnancyData.Offspring[animal.type.Value];
                ret = animals[new Random().Next(animals.Count)];
                ModEntry.MyMonitor.Log($"Parent is {animal.Name} type {animal.type.Value}, offspring type is {ret}", LogLevel.Trace);
            }
            return ret;
        }
    }
}
