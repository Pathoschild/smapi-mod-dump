/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/SDVCustomChores
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;

namespace LeFauxMatt.HelpfulSpouses
{
    internal class NpcPatches
    {
        public static void MarriageDuties_Prefix()
        {
            try
            {
                // Prevent default chores from occurring
                NPC.hasSomeoneFedTheAnimals = true;
                NPC.hasSomeoneFedThePet = true;
                NPC.hasSomeoneRepairedTheFences = true;
                NPC.hasSomeoneWateredCrops = true;
            }
            catch (Exception ex)
            {
                HelpfulSpousesMod.Instance.Monitor.Log($"Failed in {nameof(MarriageDuties_Prefix)}:\n{ex}", LogLevel.Error);
                throw;
            }
        }
    }
}
