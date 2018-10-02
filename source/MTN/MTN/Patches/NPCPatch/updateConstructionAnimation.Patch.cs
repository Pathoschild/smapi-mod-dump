using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.NPCPatch
{
    //[HarmonyPatch(typeof(NPC))]
    //[HarmonyPatch("updateConstructionAnimation")]
    class updateConstructionAnimationPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int i;
            var codes = new List<CodeInstruction>(instructions);

            for (i = 13; i < 31; i++)
            {
                codes[i].opcode = OpCodes.Nop;
            }


            return codes.AsEnumerable();
        }

        public static void Postfix(NPC __instance)
        {
            bool isFestivalDay = Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
            if (Game1.IsMasterGame && __instance.Name == "Robin" && !isFestivalDay)
            {
                if (Game1.player.daysUntilHouseUpgrade > 0)
                {
                    Vector2 wp = new Vector2((Memory.isCustomFarmLoaded) ? Memory.loadedFarm.farmHousePorchX() + 4 : 68f, (Memory.isCustomFarmLoaded) ? Memory.loadedFarm.farmHousePorchY() : 14f);
                    Game1.warpCharacter(__instance, "Farm", wp);
                    Traverse.Create(__instance).Field("isPlayingRobinHammerAnimation").SetValue(false);
                    Traverse.Create(__instance).Field("shouldPlayRobinHammerAnimation").Field("Value").SetValue(true);
                    return;
                }
            }
        }
    }
}
