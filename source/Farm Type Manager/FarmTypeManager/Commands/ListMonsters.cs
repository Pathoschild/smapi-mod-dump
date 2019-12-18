using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Console command. Outputs a list of loaded custom monster types and the assemblies providing them.</summary>
        private void ListMonsters(string command, string[] args)
        {
            if (!Context.IsGameLaunched) { return; } //if the game hasn't fully launched yet, ignore this command

            Monitor.Log($"Standard monster names:", LogLevel.Info);
            Monitor.Log($"", LogLevel.Info);
            foreach (string monster in GetFTMMonsterNames()) //for each monster name from FTM's basic list
            {
                Monitor.Log($"{monster}", LogLevel.Info);
            }
            Monitor.Log($"", LogLevel.Info);
            Monitor.Log($"Custom monster names:", LogLevel.Info);
            Monitor.Log($"", LogLevel.Info);
            string previousAssembly = null; //the most recently displayed assembly name
            List<Type> monsters = GetAllSubclassTypes(typeof(Monster)); //a list of every available monster type (including those from SDV and this mod)
            foreach (Type monster in monsters) //for each available monster type
            {
                string currentAssembly = monster.Assembly.GetName().Name; //get this monster's simple assembly name
                if (currentAssembly == "Stardew Valley" || currentAssembly == "FarmTypeManager") //if this monster is from SDV or this mod
                {
                    continue; //skip to the next monster
                }

                if (previousAssembly != currentAssembly) //if this monster is from a different assembly than the previously displayed one
                {
                    Monitor.Log($"-----", LogLevel.Info);
                    Monitor.Log($"Mod: {currentAssembly}", LogLevel.Info); //display the assembly name
                    Monitor.Log($"-----", LogLevel.Info);
                    previousAssembly = currentAssembly;
                }
                
                Monitor.Log($"{monster.FullName}", LogLevel.Info); //display this monster's name (i.e. the name compatible with this mod's MonsterName setting)
            }
        }

        /// <summary>Searches all loaded assemblies for subclasses of the provided class type and returns them in a list.</summary>
        /// <param name="baseClass">The returned type must be derived from this class.</param>
        /// <returns>A list of types derived from baseClass.</returns>
        private static List<Type> GetAllSubclassTypes(Type baseClass)
        {
            List<Type> subclassTypes = new List<Type>();

            if (baseClass == null)
            {
                return subclassTypes;
            }

            bool filterSubclass(Type type) => type.IsSubclassOf(baseClass); //true when a type is derived from the provided base class

            subclassTypes.AddRange
            (
                AppDomain.CurrentDomain.GetAssemblies() //get all assemblies
                .Where
                (
                    assembly => assembly.IsDynamic == false //ignore any dynamic assemblies
                ) 
                .SelectMany(assembly => assembly.GetTypes()) //get all types from each assembly as a single sequence
                .Where(filterSubclass) //ignore any types that are not subclasses of baseClass
            );

            return subclassTypes;
        }

        /// <summary>Produces a premade (non-dynamic) list of names for each monster type from Stardew Valley, as used by this mod's "MonsterName" setting.</summary>
        /// <returns>A list containing the "primary" MonsterName value for each monster from SDV.</returns>
        private static List<string> GetFTMMonsterNames()
        {
            List<string> monsterNames = new List<string>();

            monsterNames.AddRange
            (
                new string[]
                {
                    "bat",
                    "frost bat",
                    "lava bat",
                    "iridium bat",
                    "cursed doll",
                    "haunted skull",
                    "big slime",
                    "big green slime",
                    "big frost jelly",
                    "big red sludge",
                    "big purple sludge",
                    "bug",
                    "armored bug",
                    "pepper rex",
                    "duggy",
                    "dust sprite",
                    "ghost",
                    "carbon ghost",
                    "green slime",
                    "frost jelly",
                    "red sludge",
                    "purple sludge",
                    "cave grub",
                    "cave fly",
                    "mutant grub",
                    "mutant fly",
                    "metal head",
                    "mummy",
                    "rock crab",
                    "lava crab",
                    "iridium crab",
                    "stone golem",
                    "wilderness golem",
                    "serpent",
                    "shadow brute",
                    "shadow shaman",
                    "skeleton",
                    "squid kid"
                }
            );

            return monsterNames;
        }
    }
}