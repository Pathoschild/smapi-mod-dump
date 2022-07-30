/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using EnhancedSlingshots.Framework.Enchantments;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace EnhancedSlingshots.Framework.Patch
{
    [HarmonyPatch(typeof(Projectile))]
    public static class ProjectilePatchs
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Projectile.isColliding))]
        public static void isColliding_Postfix(Projectile __instance, ref bool __result, ref NetCharacterRef ___theOneWhoFiredMe, GameLocation location)
        {
            var who = ___theOneWhoFiredMe.Get(location);
            if (who is Farmer player && player.CurrentTool is Slingshot sling && sling.hasEnchantmentOfType<MagneticEnchantment>())
            {
                var result = location.objects.Pairs.FirstOrDefault(obj => obj.Value.getBoundingBox(obj.Key).Intersects(__instance.getBoundingBox()));
                if (default(KeyValuePair<Vector2, SObject>).Equals(result))
                    return;

                if(ModEntry.Instance.config.MagneticEnchantment_AffectedStones.Contains(result.Value.ParentSheetIndex))
                    __result = true;
            }           
        }

    }
}
