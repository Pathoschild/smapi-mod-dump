/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SpaceCore.Overrides
{
    public class HoeDirtWinterFix
    {
        public static void DestroyCropReplacement( Vector2 tileLocation, bool showAnimation, GameLocation location )
        {
            // We don't want it to ever do anything.
            // Crops wither out of season anyways.
        }

        public static IEnumerable<CodeInstruction> Transpiler( ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns )
        {
            // TODO: Learn how to use ILGenerator
            
            var newInsns = new List<CodeInstruction>();
            foreach ( var insn in insns )
            {
                if ( insn.opcode == OpCodes.Call && ( insn.operand as MethodInfo ).Name == "destroyCrop" )
                {
                    // Replace with our call. We do this instead of nop to clear the stack entries
                    // Because I'm too lazy to figure out the rest properly.
                    insn.operand = AccessTools.Method( typeof( HoeDirtWinterFix ), nameof( HoeDirtWinterFix.DestroyCropReplacement ) );
                }

                newInsns.Add( insn );
            }

            return newInsns;
        }
    }
}
