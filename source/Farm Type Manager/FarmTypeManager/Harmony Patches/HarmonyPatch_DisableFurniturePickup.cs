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
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A Harmony patch that conditionally prevents players picking up specific furniture instances.</summary>
        public class HarmonyPatch_DisableFurniturePickup
        {
            /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
            /// <param name="harmony">This mod's Harmony instance.</param>
            public static void ApplyPatch(Harmony harmony)
            {
                try
                {
                    HashSet<Type> typesToPatch = new HashSet<Type>(); //a set of types to patch

                    foreach (Type type in Utility.GetAllSubclassTypes(typeof(Furniture))) //for each existing furniture type (including Furniture itself)
                    {
                        if (AccessTools.Method(type, nameof(Furniture.canBeRemoved)) is MethodInfo info) //if this type has a removal check method
                            typesToPatch.Add(info.DeclaringType); //add that method's declaring type to the set (if it hasn't already been added)
                    }

                    Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_DisableFurniturePickup)}\": postfixing {typesToPatch.Count} implementations of SDV method \"Furniture.canBeRemoved(Farmer)\".", LogLevel.Trace);
                    foreach (Type type in typesToPatch) //for each type that implements a unique version of the target method
                    {
                        try
                        {
                            Utility.Monitor.VerboseLog($"* Postfixing SDV method \"{type.Name}.canBeRemoved(Farmer)\".");
                            harmony.Patch(
                                original: AccessTools.Method(type, nameof(Furniture.canBeRemoved)),
                                postfix: new HarmonyMethod(typeof(HarmonyPatch_DisableFurniturePickup), nameof(canBeRemoved_Postfix))
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
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_DisableFurniturePickup)}\" failed to apply. Spawned furniture might ignore the \"CanBePickedUp\" setting. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }

            /// <summary>Prevents furniture being removed if its "can be picked up" setting is false.</summary>
            /// <param name="__instance">The furniture being checked for removal.</param>
            /// <param name="__result">True if the furniture instance can be removed.</param>
            public static void canBeRemoved_Postfix(Furniture __instance, ref bool __result)
            {
                try
                {
                    if (__result && __instance.modData.TryGetValue(Utility.ModDataKeys.CanBePickedUp, out string data) && data.StartsWith("f", StringComparison.OrdinalIgnoreCase)) //if this furniture is normally allowed to be removed, but its "can be picked up" setting is false
                    {
                        __result = false; //prevent removal
                    }
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_DisableFurniturePickup)}\" has encountered an error. Spawned furniture might ignore the \"CanBePickedUp\" setting. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }
        }
    }
}
