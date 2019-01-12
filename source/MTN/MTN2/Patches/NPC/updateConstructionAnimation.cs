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
    /// <summary>
    /// REASON FOR PATCHING: To adjust for the relocation of the farmhouse on custom maps.
    /// 
    /// Patches the method NPC.updateConstructionAnimation to adjust the NPC Robin to be
    /// placed and render at the side of the Farm House if it was relocated.
    /// </summary>
    public class updateConstructionAnimationPatch
    {
        private static CustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public updateConstructionAnimationPatch(CustomManager customManager) {
            updateConstructionAnimationPatch.customManager = customManager;
        }

        /// <summary>
        /// Transpiles the CLI to remove operations pertaining to the rendering of the NPC Robin when upgrades to the farm house are
        /// occuring.
        /// </summary>
        /// <param name="instructions">Code Instructions (in CLI)</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);

            // TO-DO: Refactor to remove the code, instead of replacing with Nops.
            for (int i = 13; i < 31; i++) {
                codes[i].opcode = OpCodes.Nop;
            }
            return codes.AsEnumerable();
        }

        /// <summary>
        /// Postfix Method. Occurs after the original method has been executed.
        /// 
        /// Checks to see if the NPC is Robin, a holiday isn't occuring, and if 
        /// the farmhouse is being upgraded. Renders Robin at the proper location
        /// if all is true.
        /// </summary>
        /// <param name="__instance">The instance of NPC that called updateConstructionAnimation.</param>
        public static void Postfix(NPC __instance) {
            bool isFestivalDay = Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason);
            if (Game1.IsMasterGame && __instance.Name == "Robin" && !isFestivalDay) {
                if (Game1.player.daysUntilHouseUpgrade > 0) {
                    Vector2 wp = new Vector2((!customManager.Canon) ? customManager.FarmHousePorch.X + 4 : 68f, (!customManager.Canon) ? customManager.FarmHousePorch.Y : 14f);
                    Game1.warpCharacter(__instance, "Farm", wp);
                    Traverse.Create(__instance).Field("isPlayingRobinHammerAnimation").SetValue(false);
                    Traverse.Create(__instance).Field("shouldPlayRobinHammerAnimation").Field("Value").SetValue(true);
                    return;
                }
            }
        }
    }
}
