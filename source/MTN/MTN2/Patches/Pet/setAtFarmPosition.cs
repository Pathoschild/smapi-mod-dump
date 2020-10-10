/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.PetPatches
{
    public class setAtFarmPositionPatch
    {
        private static ICustomManager customManager;

        public setAtFarmPositionPatch(ICustomManager customManager) {
            setAtFarmPositionPatch.customManager = customManager;
        }

        public static bool Prefix(Pet __instance) {
            return (customManager.Canon) ? true : false;
        }

        public static void Postfix(Pet __instance) {
            if (customManager.Canon) return;

            if (!Game1.isRaining) {
                __instance.faceDirection(2);
                Game1.warpCharacter(__instance, "Farm", new Vector2(customManager.PetWaterBowl.X, customManager.PetWaterBowl.Y - 1));
                __instance.position.X -= 64f;
            }
            return;
        }
    }
}
