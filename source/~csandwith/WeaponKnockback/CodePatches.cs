/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeaponKnockback
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.damageMonster), new Type[] {typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) })]
        public class GameLocation_damageMonster_Patch
        {
            public static void Prefix(ref float knockBackModifier, Farmer who)
            {
                if (!Config.ModEnabled)
                    return;

            }
        }
        
        [HarmonyPatch(typeof(Monster), nameof(Monster.Slipperiness))]
        [HarmonyPatch(MethodType.Getter)]
        public class Monster_Slipperiness_Patch
        {
            public static void Postfix(Monster __instance, ref int __result)
            {
                if (!Config.ModEnabled)
                    return;
                var x = __result;
            }
        }
        

    }
}