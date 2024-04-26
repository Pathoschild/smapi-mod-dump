/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace SkinnyAnimals; 

internal partial class Mod {
    
    public class FarmAnimal_farmerPushing_Patch {
        private static bool isEnabled() {
            if (!Config.Enabled) return false;
            if (Config.PushSpeedMultiplier == 1 && !Config.IgnoreCollision) return false;
            
            return true;
        }
        public static void Postfix(FarmAnimal __instance) {
            if (!isEnabled()) return;
            if (__instance.pushAccumulator > 60) return;

            if (Config.IgnoreCollision) __instance.pushAccumulator = 61;
            else __instance.pushAccumulator += Config.PushSpeedMultiplier-1;
        }
    }
}