/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace LessFlashy.Patches;

public class GameLocationPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static IModHelper Helper => ModEntry.Help;
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(GameLocationPatches)}\": transpiling SDV method \"GameLocation.explode\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
            transpiler: new HarmonyMethod(typeof(GameLocationPatches), nameof(Transpiler))
        );
    }
    
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        //new one
        var codes = new List<CodeInstruction>(instructions);
        var instructionsToInsert = new List<CodeInstruction>();

        var index = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo { Name: "broadcastSprites"});
        Log($"index: {index}", LogLevel.Info);
        
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 4));
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationPatches), nameof(Change))));

        Log($"codes count: {codes.Count}, insert count: {instructionsToInsert.Count}");
        Log("Inserting method");
        codes.InsertRange(index - 3, instructionsToInsert);
        
        /* print the IL code
         * courtesy of atravita
         *
        StringBuilder sb = new();
        sb.Append("ILHelper for: GameLocation.spawnObjects");
        for (int i = 0; i < codes.Count; i++)
        {
            sb.AppendLine().Append(codes[i]);
            if (index + 3 == i)
            {
                sb.Append("       <---- start of transpiler");
            }
            if (index + 3 + instructionsToInsert.Count == i)
            {
                sb.Append("       <----- end of transpiler");
            }
        }
        Log(sb.ToString(), LogLevel.Info);
        */
        return codes.AsEnumerable();
    }

    private static void Change(List<TemporaryAnimatedSprite> sprites)
    {
        #if DEBUG
        Log("changing");
        #endif
        foreach (var sprite in sprites)
        {
            sprite.flash = false;
            sprite.alpha = 0f;
            sprite.alphaFade = 0f;
        }
    }
}