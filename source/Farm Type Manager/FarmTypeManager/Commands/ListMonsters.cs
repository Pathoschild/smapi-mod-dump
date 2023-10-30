/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Console command. Outputs a list of loaded custom monster types and the assemblies providing them.</summary>
        private void ListMonsters(string command, string[] args)
        {
            if (!Context.IsGameLaunched) { return; } //if the game hasn't fully launched yet, ignore this command

            Utility.Monitor.Log($"-----", LogLevel.Info);
            Utility.Monitor.Log($"Standard monster names", LogLevel.Info);
            Utility.Monitor.Log($"-----", LogLevel.Info);
            Utility.Monitor.Log($"", LogLevel.Info);
            foreach (string monster in GetFTMMonsterNames()) //for each monster name from FTM's basic list
            {
                Utility.Monitor.Log($"{monster}", LogLevel.Info);
            }
            string previousAssembly = null; //the most recently displayed assembly name
            List<Type> monsters = Utility.GetAllSubclassTypes(typeof(Monster)); //a list of every available monster type (including those from SDV and this mod)
            foreach (Type monster in monsters) //for each available monster type
            {
                string currentAssembly = monster.Assembly.GetName().Name; //get this monster's simple assembly name
                if (currentAssembly == "Stardew Valley" || currentAssembly == "StardewValley" || //if this monster is from SDV
                    currentAssembly == "FarmTypeManager" || //or from this mod
                    currentAssembly == "MonstersTheFramework") //or from MTF
                {
                    continue; //skip to the next monster
                }

                if (previousAssembly != currentAssembly) //if this monster is from a different assembly than the previously displayed one
                {
                    Utility.Monitor.Log($"", LogLevel.Info);
                    Utility.Monitor.Log($"-----", LogLevel.Info);
                    Utility.Monitor.Log($"Mod: {currentAssembly}", LogLevel.Info); //display the assembly name
                    Utility.Monitor.Log($"-----", LogLevel.Info);
                    Utility.Monitor.Log($"", LogLevel.Info);
                    previousAssembly = currentAssembly;
                }

                Utility.Monitor.Log($"{monster.FullName}", LogLevel.Info); //display this monster's name (i.e. the name compatible with this mod's MonsterName setting)
            }

            IEnumerable<string> monstersFromMTF = Utility.MonstersTheFrameworkAPI?.GetAllMonsterTypes(); //try to get MTF's available monster IDs

            if (monstersFromMTF != null) //if MTF's monster IDs are accessible
            {
                Utility.Monitor.Log($"", LogLevel.Info);
                Utility.Monitor.Log($"-----", LogLevel.Info);
                Utility.Monitor.Log($"Mod: MonstersTheFramework", LogLevel.Info);
                Utility.Monitor.Log($"-----", LogLevel.Info);
                Utility.Monitor.Log($"", LogLevel.Info);

                foreach (string id in monstersFromMTF)
                {
                    Utility.Monitor.Log($"{id}", LogLevel.Info); //display this ID
                }
            }
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
                    "magma sprite",
                    "magma sparker",
                    "big green slime",
                    "big blue slime",
                    "big red slime",
                    "big purple slime",
                    "blue squid",
                    "bug",
                    "armored bug",
                    "pepper rex",
                    "duggy",
                    "magma duggy",
                    "dust sprite",
                    "dwarvish sentry",
                    "ghost",
                    "carbon ghost",
                    "putrid ghost",
                    "green slime",
                    "blue slime",
                    "red slime",
                    "purple slime",
                    "tiger slime",
                    "prismatic slime",
                    "cave grub",
                    "cave fly",
                    "mutant grub",
                    "mutant fly",
                    "metal head",
                    "hot head",
                    "lava lurk",
                    "leaper",
                    "mummy",
                    "rock crab",
                    "lava crab",
                    "iridium crab",
                    "stick bug",
                    "false magma cap",
                    "rock golem",
                    "stone golem",
                    "wilderness golem",
                    "serpent",
                    "royal serpent",
                    "shadow brute",
                    "shadow shaman",
                    "shadow sniper",
                    "skeleton",
                    "skeleton mage",
                    "spiker",
                    "squid kid"
                }
            );

            return monsterNames;
        }
    }
}
