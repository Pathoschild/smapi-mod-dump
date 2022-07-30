/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

#if DEBUG
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace PamTries.HarmonyPatches;

/// <summary>
/// Patches for bus stops.
/// </summary>
[HarmonyPatch(typeof(BusStop))]
internal static class BusStopPatch
{
    [MethodImpl(TKConstants.Hot)]
    private static bool ShouldAllowBus(GameLocation loc)
    {
        Vector2 bustile = new(11f, 10f);
        foreach(NPC npc in loc.getCharacters())
        {
            if (npc.isVillager() && npc.getTileLocation().Equals(bustile))
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Subbing in {npc.Name} as the bus driver.", LogLevel.Info);
                return true;
            }
        }
        return false;
    }

    [HarmonyPatch(nameof(BusStop.answerDialogue), new Type[] { typeof(Response) })]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        /*******************************
         * Want to: remove the check for Pam from
         * if (Game1.player.Money >= (Game1.shippingTax ? 50 : 500) && base.characters.Contains(characterFromName) && characterFromName.getTileLocation().Equals(new Vector2(11f, 10f)))
         * AND replace it with our own check.
         *
         * So delete:
         * ldarg.0
         * ldfld class Netcode.NetCollection`1<class StardewValley.NPC> StardewValley.GameLocation::characters
         * ldloc.0
         * callvirt instance bool class Netcode.NetCollection`1<class StardewValley.NPC>::Contains(!0)
         * brfalse IL_0137
         * IL_0082: ldloc.0
         *  callvirt instance valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Vector2 StardewValley.Character::getTileLocation()
         * stloc.1
         * IL_0089: ldloca.s 1
         * IL_008b: ldc.r4 11
         * IL_0090: ldc.r4 10
         * IL_0095: newobj instance void [MonoGame.Framework]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
         * IL_009a: call instance bool [MonoGame.Framework]Microsoft.Xna.Framework.Vector2::Equals(valuetype [MonoGame.Framework]Microsoft.Xna.Framework.Vector2)
         *
         *
         * TODO: figure out what draws Pam in the bus and remove that too...
        *****************************************/

        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldstr, "Pam"),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Call),
                })
                .FindNext(new CodeInstructionWrapper[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld),
                    new(SpecialCodeInstructionCases.LdLoc),
                    new(OpCodes.Callvirt),
                    new(OpCodes.Brfalse),
                })
                .RemoveIncluding(new CodeInstructionWrapper[]
                {
                    new (OpCodes.Ldc_R4, 11),
                    new (OpCodes.Ldc_R4, 10),
                    new (OpCodes.Newobj),
                    new (OpCodes.Call, typeof(Vector2).GetCachedMethod(nameof(Vector2.Equals), ReflectionCache.FlagTypes.InstanceFlags, new Type[] { typeof(Vector2) })),
                })
                .Insert(new CodeInstruction[]
                {
                    new (OpCodes.Ldarg_0),
                    new (OpCodes.Call, typeof(BusStopPatch).GetCachedMethod(nameof(BusStopPatch.ShouldAllowBus), ReflectionCache.FlagTypes.StaticFlags)),
                });
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed when trying to apply transpiler for BusStop.answerDialogue\n\n{ex}", LogLevel.Error);
            return null;
        }
    }
}

#endif