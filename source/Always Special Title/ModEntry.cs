/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DespairScent/AlwaysSpecialTitle
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;
using System.Reflection.Emit;
using System.Reflection;

namespace DespairScent.AlwaysSpecialTitle
{
    internal sealed class ModEntry : Mod
    {

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.receiveLeftClick)),
               transpiler: new HarmonyMethod(typeof(ModEntry), nameof(SpecialTitlePatch)));
        }

        public static IEnumerable<CodeInstruction> SpecialTitlePatch(IEnumerable<CodeInstruction> instructions)
        {
            var foundRandomMethod = false;

            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && (MethodInfo)codes[i].operand == AccessTools.Method("System.Random:NextDouble"))
                {
                    foundRandomMethod = true;
                }
                else
                {
                    if (foundRandomMethod && codes[i].opcode == OpCodes.Ldc_R8 && (double)codes[i].operand == 0.02)
                    {
                        codes[i].operand = 1.0;
                    }
                    foundRandomMethod = false;
                }
            }

            return codes.AsEnumerable();
        }

    }
}
