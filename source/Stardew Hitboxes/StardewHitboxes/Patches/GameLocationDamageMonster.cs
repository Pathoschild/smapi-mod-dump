/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewHitboxes
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace StardewHitboxes.Patches
{
    [HarmonyPatch(typeof(GameLocation), "damageMonster", new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) })]
    internal class GameLocationDamageMonster
    {
        public static void Postfix(Rectangle areaOfEffect)
        {
            ModEntry.RenderWeaponAOE(areaOfEffect);
        }
    }
}
