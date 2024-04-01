/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A Harmony patch that conditionally disables <see cref="Monster.getExtraDropItems"/> based on a monster spawn setting.</summary>
        public class HarmonyPatch_ToggleExtraLoot
        {
            /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
            /// <param name="harmony">This mod's Harmony instance.</param>
            public static void ApplyPatch(Harmony harmony)
            {
                try
                {
                    HashSet<Type> typesToPatch = new HashSet<Type>(); //a set of types to patch

                    foreach (Type type in Utility.GetAllSubclassTypes(typeof(Monster))) //for each existing monster type (including Monster itself)
                    {
                        if (AccessTools.Method(type, nameof(Monster.getExtraDropItems)) is MethodInfo info) //if this type has an extra loot method
                            typesToPatch.Add(info.DeclaringType); //add that method's declaring type to the set (if it hasn't already been added)
                    }

                    Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_ToggleExtraLoot)}\": postfixing {typesToPatch.Count} implementations of SDV method \"Monster.getExtraDropitems()\".", LogLevel.Trace);
                    foreach (Type type in typesToPatch) //for each type that implements a unique version of the target method
                    {
                        try
                        {
                            Utility.Monitor.VerboseLog($"* Postfixing SDV method \"{type.Name}.getExtraDropitems()\".");
                            harmony.Patch(
                                original: AccessTools.Method(type, nameof(Monster.getExtraDropItems)),
                                postfix: new HarmonyMethod(typeof(HarmonyPatch_ToggleExtraLoot), nameof(getExtraDropItems_Postfix))
                            );
                        }
                        catch (Exception ex)
                        {
                            Utility.Monitor.VerboseLog($"* Encountered an error while patching {type.Name}. That type will be skipped. Full error message: {ex.ToString()}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_ToggleExtraLoot)}\" failed to apply. Custom monsters with \"extra loot\" disabled might still drop extra items. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }

            /// <summary>Removes all items from the result if the monster's "extra loot" setting is false.</summary>
            /// <param name="__instance">The monster generating extra items.</param>
            /// <param name="__result">The list of extra items.</param>
            public static void getExtraDropItems_Postfix(Monster __instance, ref List<Item> __result)
            {
                try
                {
                    if (__instance.modData.TryGetValue(Utility.ModDataKeys.ExtraLoot, out string data) && data.StartsWith("f", StringComparison.OrdinalIgnoreCase)) //if this monster's "extra loot" setting is false
                    {
                        __result.Clear(); //remove all extra items from the result
                    }
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_ToggleExtraLoot)}\" has encountered an error. Custom monsters with \"extra loot\" disabled might still drop extra items. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }
        }
    }
}
