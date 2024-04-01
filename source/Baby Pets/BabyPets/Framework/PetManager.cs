/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DelphinWave/BabyPets
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;

namespace BabyPets.Framework
{
    internal class PetManager
    {

        /// <summary>
        /// Gets all pets currently on Farm or in FarmHouses
        /// </summary>
        /// <returns>List of Pet objects</returns>
        internal static  List<Pet> GetAllPets()
        {
            List<Pet> pets = new List<Pet>();
            foreach (NPC npc in Game1.getFarm().characters)
            {
                if (npc is Pet)
                {
                    pets.Add(npc as Pet);
                }
            }
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                foreach (NPC npc in Utility.getHomeOfFarmer(farmer).characters)
                {
                    if (npc is Pet)
                    {
                        pets.Add(npc as Pet);
                    }
                }
            }
            return pets;
        }

        internal static List<Pet> InitializeBdayModData(int daysSinceStart)
        {
            var pets = GetAllPets();
            foreach (Pet pet in pets)
            {
                //ModEntry.SMonitor.Log($"{pet.Name}", LogLevel.Info);
                //ModEntry.SMonitor.Log($"DaysSinceStart {daysSinceStart}", LogLevel.Info);
                //ModEntry.SMonitor.Log($"TimesPet {pet.timesPet.Value}", LogLevel.Info);

                if (!pet.modData.ContainsKey(ModEntry.MOD_DATA_BDAY))
                {
                    int bday = daysSinceStart - pet.timesPet.Value;
                    if (bday < 0) bday = 0;
                    pet.modData[ModEntry.MOD_DATA_BDAY] = bday.ToString();
                }
            }

            return pets;
        }


    }
}
