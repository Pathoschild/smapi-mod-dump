/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ShivaGuy/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;

namespace ShivaGuy.Stardew.FarmAnimalChoices.Patch
{
    internal static class FarmAnimalPatch
    {
        private static string animalRequested = "";

        static IMonitor Monitor { get { return ModEntry.Context.Monitor; } }

        private static bool IsNotGeneric(string type)
        {
            return (type.Contains("Chicken") && (type.Contains("White") || type.Contains("Brown") || type.Contains("Blue")))
                || (type.Contains("Cow") && (type.Contains("White") || type.Contains("Brown")));
        }

        private static bool SameSpacies(string type1, string type2)
        {
            return (type1.Contains("Chicken") && type2.Contains("Chicken"))
                || (type1.Contains("Cow") && type2.Contains("Cow"))
                || (type1.Equals(type2));
        }

        public static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Constructor(typeof(FarmAnimal), new Type[] { typeof(string), typeof(long), typeof(long) }),
                prefix: new HarmonyMethod(typeof(FarmAnimalPatch), nameof(ctor_Prefix)),
                postfix: new HarmonyMethod(typeof(FarmAnimalPatch), nameof(ctor_Postfix)));
        }

        public static void ctor_Prefix(string type)
        {
            if (IsNotGeneric(type))
                animalRequested = type;
        }

        public static void ctor_Postfix(FarmAnimal __instance)
        {
            var animalCreated = __instance.type.Value;

            if (!IsNotGeneric(animalCreated))
                return;

            if (SameSpacies(animalCreated, animalRequested) && !animalRequested.Equals(animalCreated))
            {
                Monitor.Log($"Reverting {animalCreated} back to {animalRequested}.");
                __instance.type.Value = animalRequested;
                __instance.reloadData();
            }
        }
    }
}
