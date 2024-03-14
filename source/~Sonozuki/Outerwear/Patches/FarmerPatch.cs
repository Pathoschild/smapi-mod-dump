/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Outerwear.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Farmer"/> class.</summary>
    internal class FarmerPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The transpiler for the <see cref="Farmer()"/> and <see cref="Farmer(FarmerSprite, Vector2, int, string, List{Item}, bool)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to change the length of the <see cref="Farmer.appliedBuffs"/> length to store the custom buffs.</remarks>
        internal static IEnumerable<CodeInstruction> ConstructorTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var patchApplied = false;
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                if (patchApplied)
                {
                    yield return instruction;
                    continue;
                }

                // check if this instruction is responsible for setting the length of appliedBuffs
                var nextNextInstruction = instructions.ElementAt(i + 2);
                if (instruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(instruction.operand) == 12
                    && nextNextInstruction.opcode == OpCodes.Stfld && nextNextInstruction.operand == typeof(Farmer).GetField("appliedBuffs", BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    instruction.operand = ((int[])Enum.GetValues(typeof(CustomBuff))).Max() + 1;
                    patchApplied = true;
                }

                yield return instruction;
            }
        }

        /// <summary>The post fix for the <see cref="Farmer.addBuffAttributes(int[])"/> method.</summary>
        /// <param name="buffAttributes">The buff attributes to apply to the farmer.</param>
        /// <param name="__instance">The current <see cref="Farmer"/> instance being patched.</param>
        /// <remarks>This is used to apply the custom buffs.</remarks>
        internal static void AddBuffAttributesPostFix(int[] buffAttributes, Farmer __instance)
        {
            __instance.addedCombatLevel.Value += buffAttributes[(int)CustomBuff.CombatSkill];
            __instance.critChanceModifier += buffAttributes[(int)CustomBuff.CriticalChance] / 100f;
            __instance.critPowerModifier += buffAttributes[(int)CustomBuff.CriticalPower] / 100f;
            __instance.maxHealth += buffAttributes[(int)CustomBuff.MaxHealth];
        }

        /// <summary>The post fix for the <see cref="Farmer.removeBuffAttributes(int[])"/> method.</summary>
        /// <param name="buffAttributes">The buff attributes to remove from the farmer.</param>
        /// <param name="__instance">The current <see cref="Farmer"/> instance being patched.</param>
        /// <remarks>This is used to remove the custom buffs.</remarks>
        internal static void RemoveBuffAttributesPostFix(int[] buffAttributes, Farmer __instance) => RemoveCustomBuffs(buffAttributes, __instance);

        /// <summary>The prefix for the <see cref="Farmer.ClearBuffs()"/> method.</summary>
        /// <param name="__instance">The current <see cref="Farmer"/> instance being patched.</param>
        /// <remarks>This is used to remove the custom buffs.</remarks>
        internal static void ClearBuffsPrefix(Farmer __instance)
        {
            // get the applied buffs to remove the custom ones
            var appliedBuffs = new int[((int[])Enum.GetValues(typeof(CustomBuff))).Max() + 1];
            var netAppliedBuffs = (NetArray<int, NetInt>)typeof(Farmer).GetField("appliedBuffs", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            netAppliedBuffs.CopyTo(appliedBuffs, 0);

            // the base game ClearBuffs methods doesn't reset these
            {
                if (appliedBuffs[Buff.maxStamina] != 0)
                {
                    __instance.MaxStamina = Math.Max(0, __instance.MaxStamina - appliedBuffs[Buff.maxStamina]);
                    __instance.Stamina = Math.Min(__instance.Stamina, __instance.MaxStamina);
                }

                if (appliedBuffs[Buff.magneticRadius] != 0)
                    __instance.MagneticRadius = Math.Max(0, __instance.MagneticRadius - appliedBuffs[Buff.magneticRadius]);

                if (appliedBuffs[Buff.defense] != 0)
                    __instance.resilience = Math.Max(0, __instance.resilience - appliedBuffs[Buff.defense]);
            }

            RemoveCustomBuffs(appliedBuffs, __instance);

            // for some reason the game doesn't do this, which results in the custom buffs becoming stacked up which can cause the buff removal to take away too much
            for (int i = 0; i < netAppliedBuffs.Count; i++)
                netAppliedBuffs[i] = 0;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Removes the custom buffs from a player.</summary>
        /// <param name="buffAttributes">The buffs to remove.</param>
        /// <param name="player">The player who should have the buffs removed from.</param>
        private static void RemoveCustomBuffs(int[] buffAttributes, Farmer player)
        {
            if (buffAttributes[(int)CustomBuff.CombatSkill] != 0)
                player.addedCombatLevel.Value = Math.Max(0, player.addedCombatLevel - buffAttributes[(int)CustomBuff.CombatSkill]);

            if (buffAttributes[(int)CustomBuff.CriticalChance] != 0)
                player.critChanceModifier = Math.Max(0, player.critChanceModifier - buffAttributes[(int)CustomBuff.CriticalChance] / 100f);

            if (buffAttributes[(int)CustomBuff.CriticalPower] != 0)
                player.critPowerModifier = Math.Max(0, player.critPowerModifier - buffAttributes[(int)CustomBuff.CriticalPower] / 100f);

            if (buffAttributes[(int)CustomBuff.MaxHealth] != 0)
            {
                player.maxHealth = Math.Max(0, player.maxHealth - buffAttributes[(int)CustomBuff.MaxHealth]);
                player.health = Math.Min(player.health, player.maxHealth);
            }
        }
    }
}
