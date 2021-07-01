/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley.Objects;

namespace BiggerJunimoChest
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            var harmony = HarmonyInstance.Create( ModManifest.UniqueID );
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Chest), nameof( Chest.GetActualCapacity ))]
    public static class LargerJunimoChestCapacityPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler( ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns )
        {
            // TODO: Learn how to use ILGenerator

            int counter = 0;

            var newInsns = new List<CodeInstruction>();
            foreach ( var insn in insns )
            {
                if ( insn.opcode == OpCodes.Ldc_I4_S && (sbyte) insn.operand == (sbyte) 9 )
                {
                    if ( ++counter == 2 )
                        insn.operand = (sbyte) 36;
                }
                newInsns.Add( insn );
            }

            return newInsns;
        }
    }
}
