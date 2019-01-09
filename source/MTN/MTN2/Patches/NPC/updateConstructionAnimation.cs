using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.NPCPatches
{
    public class updateConstructionAnimationPatch
    {
        private static CustomFarmManager farmManager;

        public updateConstructionAnimationPatch(CustomFarmManager farmManager) {
            updateConstructionAnimationPatch.farmManager = farmManager;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 13; i < 31; i++) {
                codes[i].opcode = OpCodes.Nop;
            }
            return codes.AsEnumerable();
        }

        public static void Postfix(NPC __instance) {
            bool isFestivalDay = Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
            if (Game1.IsMasterGame && __instance.Name == "Robin" && !isFestivalDay) {
                if (Game1.player.daysUntilHouseUpgrade > 0) {
                    Vector2 wp = new Vector2((!farmManager.Canon) ? farmManager.FarmHousePorch.X + 4 : 68f, (!farmManager.Canon) ? farmManager.FarmHousePorch.Y : 14f);
                    Game1.warpCharacter(__instance, "Farm", wp);
                    Traverse.Create(__instance).Field("isPlayingRobinHammerAnimation").SetValue(false);
                    Traverse.Create(__instance).Field("shouldPlayRobinHammerAnimation").Field("Value").SetValue(true);
                    return;
                }
            }
        }
    }
}
