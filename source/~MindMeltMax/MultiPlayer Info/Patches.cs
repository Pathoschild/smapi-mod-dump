/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MPInfo 
{
    internal class Patches 
    {
        private static Config Config => ModEntry.Config;

        public static void Apply(string uniqueId) 
        {
            var harmony = new Harmony(uniqueId);
            harmony.Patch(
                // 0: Link into Game1.drawHUD() since that is where health and stamina is drawn
                original: AccessTools.Method(typeof(Game1), "drawHUD"),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(Transpile_Game1_drawHUD))
            );
        }

        private static int Replacement_Right() 
        {
            var r = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Right;
            // 5: If hiding bars, double the "Right" value to push drawing off the screen
            return Config.Enabled && Config.HideHealthBars ? r + r : r;
        }

        public static IEnumerable<CodeInstruction> Transpile_Game1_drawHUD(IEnumerable<CodeInstruction> instructions) 
        {
            var replacement = AccessTools.Method(typeof(Patches), nameof(Replacement_Right));
            var found = 0;
            foreach (var code in instructions) 
            {
                // 1: Find the first reference to calling Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Right
                if (found == 0 && code.opcode == OpCodes.Ldsfld && code.operand.ToString() == "Microsoft.Xna.Framework.GraphicsDeviceManager graphics")
                {
                    found++;
                    // 2: Call the replacement method instead
                    yield return new CodeInstruction(OpCodes.Call, replacement);
                }
                else if (found > 0 && found < 7) // 3: Skip the next 6 instructions which would normally finish calling "Right"
                    found++;
                else // 4: All other instructions are unchanged
                    yield return code;
                
            }
        }
    }
}
