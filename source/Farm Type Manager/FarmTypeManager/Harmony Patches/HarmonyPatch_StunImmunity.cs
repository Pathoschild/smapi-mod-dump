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
using StardewValley.Projectiles;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A Harmony patch that prevents certain monsters being stunned, e.g. being frozen by the Ice Rod trinket.</summary>
        public class HarmonyPatch_StunImmunity
        {
            /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
            /// <param name="harmony">This mod's Harmony instance.</param>
            public static void ApplyPatch(Harmony harmony)
            {
                try
                {
                    //apply Harmony patches
                    Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_StunImmunity)}\": prefixing SDV method \"DebuffingProjectile.behaviorOnCollisionWithMonster\".", LogLevel.Trace);
                    harmony.Patch(
                        original: AccessTools.Method(typeof(DebuffingProjectile), nameof(DebuffingProjectile.behaviorOnCollisionWithMonster)),
                        prefix: new HarmonyMethod(typeof(HarmonyPatch_StunImmunity), nameof(behaviorOnCollisionWithMonster_Prefix))
                    );
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_StunImmunity)}\" failed to apply. Modded monsters might lose their immunity to certain attacks. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }

            /// <summary>Causes nothing to happen if an Ice Rod's projectile touches a monster with the "stun immunity" mod data flag.</summary>
            /// <param name="__instance">The debuffing projectile.</param>
            /// <param name="n">The NPC touched by a debuffing projectile.</param>
            /// <returns>If false, the original method will be skipped.</returns>
            public static bool behaviorOnCollisionWithMonster_Prefix(DebuffingProjectile __instance, NPC n)
            {
                try
                {
                    if
                    (
                        __instance.damagesMonsters.Value && __instance.debuff.Value.Equals("frozen", StringComparison.Ordinal) && //if this is an ice rod projectile
                        n is Monster m && m.modData.TryGetValue(Utility.ModDataKeys.StunImmunity, out string value) && value.StartsWith("t", StringComparison.OrdinalIgnoreCase) //and if this monster has the "stun immunity" setting enabled
                    )
                    {
                        return false; //skip the original method (do nothing when an ice rod projectile touches this monster, as if the monster weren't there)
                    }

                    return true; //do nothing

                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_StunImmunity)}\" has encountered an error. Modded monsters might lose their immunity to certain attacks. Full error message: \n{ex.ToString()}", LogLevel.Error);
                    return true; //call the original method
                }
            }
        }
    }
}
