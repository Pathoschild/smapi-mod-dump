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
using StardewValley;
using StardewValley.Locations;

namespace BuildableLocationsFramework.Patches
{
    [HarmonyPatch(typeof(GameLocation), "carpenters")]
    public static class GameLocationCarpentersPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler( ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns )
        {
            Log.info( "Transpiling " + original );
            List<CodeInstruction> ret = new List<CodeInstruction>();

            foreach ( var insn in insns )
            {
                if ( insn.opcode == OpCodes.Callvirt && insn.operand is MethodInfo info )
                {
                    if ( info.DeclaringType == typeof( BuildableGameLocation ) && info.Name == "isThereABuildingUnderConstruction" )
                    {
                        Log.debug( "Found isThereABuildingUnderConstruction, replacing..." );
                        var newInsn = new CodeInstruction( OpCodes.Call, typeof( GameLocationCarpentersPatch ).GetMethod( nameof( IsAnyBuildingUnderConstruction ) ) );
                        ret.Add( newInsn );
                        continue;
                    }
                }
                ret.Add( insn );
            }

            return ret;
        }

        public static bool IsAnyBuildingUnderConstruction( Farm originalParam )
        {
            foreach ( var loc in Mod.GetAllLocations() )
            {
                if ( loc is BuildableGameLocation bgl )
                {
                    if ( bgl.isThereABuildingUnderConstruction() )
                        return true;
                }
            }

            return false;
        }
    }
}
