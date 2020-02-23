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
