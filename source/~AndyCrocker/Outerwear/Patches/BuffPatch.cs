/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Outerwear.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="Buff"/> class.</summary>
    internal class BuffPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The post fix for the <see cref="Buff(int)"/>, <see cref="Buff(string, int, string, int)"/>, and <see cref="Buff(int, int, int, int, int, int, int, int, int, int, int, int, int, string, string)"/> constructors.</summary>
        /// <param name="__instance">The current <see cref="Buff"/> instance being patched.</param>
        /// <remarks>This is used to expand <see cref="Buff.buffAttributes"/> to be able to store the custom buffs.</remarks>
        internal static void ConstructorPostFix(Buff __instance)
        {
            var newBuffAttributes = new int[((int[])Enum.GetValues(typeof(CustomBuff))).Max() + 1];
            Array.Copy(__instance.buffAttributes, newBuffAttributes, 12);
            __instance.buffAttributes = newBuffAttributes;
        }

        /// <summary>The post fix for the <see cref="Buff.getClickableComponents()"/> method.</summary>
        /// <param name="__instance">The current <see cref="Buff"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <remarks>This is used to draw the outerwear on the farmer.</remarks>
        internal static void GetClickableComponentsPostFix(Buff __instance, List<ClickableTextureComponent> __result)
        {
            for (int i = 12; i < __instance.buffAttributes.Length; i++)
                if (__instance.buffAttributes[i] != 0)
                    __result.Add(new ClickableTextureComponent("", Rectangle.Empty, null, GetDescription(__instance, (CustomBuff)i, __instance.buffAttributes[i]), Game1.buffsIcons, Game1.getSourceRectForStandardTileSheet(Game1.buffsIcons, i, 16, 16), 4));
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Gets the description of a <see cref="CustomBuff"/>.</summary>
        /// <param name="buff">The buff containing the <paramref name="customBuff"/> whose description to get.</param>
        /// <param name="customBuff">The custom buff to get the description of.</param>
        /// <param name="value">The amount of change the buff has.</param>
        /// <returns>The description of <paramref name="customBuff"/>.</returns>
        private static string GetDescription(Buff buff, CustomBuff customBuff, int value)
        {
            var description = customBuff switch
            {
                CustomBuff.CombatSkill => $"{(value > 0 ? "+" : "-")}{value} Combat",
                CustomBuff.CriticalChance => $"{(value > 0 ? " + " : " - ")}{value} Critical Chance",
                CustomBuff.CriticalPower => $"{(value > 0 ? "+" : "-")}{value} Critical Power",
                CustomBuff.MaxHealth => $"{(value > 0 ? "+" : "-")}{value} Max Health",
                CustomBuff.HealthRegeneration => $"{(value > 0 ? "+" : "-")}{value} Health Regeneration",
                CustomBuff.StaminaRegeneration => $"{(value > 0 ? "+" : "-")}{value} Energy Regeneration",
                _ => "???"
            };

            if (!string.IsNullOrEmpty(buff.source))
                description += $"\n{Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.508")}{buff.displaySource}\n";

            return description;
        }
    }
}
