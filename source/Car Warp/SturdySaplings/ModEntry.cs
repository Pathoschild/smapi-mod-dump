/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Tools;

namespace SturdySaplings;

public class ModEntry : Mod
{
    private static IMonitor Logger;
    private static List<CodeInstruction> ilCode;
    private Harmony harmony;

    public override void Entry(IModHelper helper)
    {
        Logger = Monitor;
        harmony = new Harmony(ModManifest.UniqueID);
        PerformHarmonyPatches();

        helper.ConsoleCommands.Add("sophie.ss.dump_il",
            "Dump modified IL code for StardewValley.TerrainFeatures.Tree:performToolAction",
            (_, _) => { LogTrace(string.Join("\n", ilCode.Select(x => x.ToString()))); });
    }

    private void PerformHarmonyPatches()
    {
        Tree_performToolAction_Patch();
    }

    private void Tree_performToolAction_Patch()
    {
        MethodInfo performToolAction = AccessTools.Method("StardewValley.TerrainFeatures.Tree:performToolAction");

        if (performToolAction is null)
        {
            LogError(
                "Could not locate MethodInfo for StardewValley.TerrainFeatures.Tree:performToolAction. Aborting patch.");
        }

        try
        {
            harmony.Patch(
                performToolAction,
                transpiler: new HarmonyMethod(typeof(ModEntry), "Tree_performToolAction_Transpiler")
            );

            LogTrace($"Successfully patched {performToolAction.FullDescription()}");
        }
        catch (Exception e)
        {
            LogError($"Encountered exception while attempting to patch {performToolAction.FullDescription()}: {e}");
        }
    }

    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<CodeInstruction> Tree_performToolAction_Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionsList = instructions.ToList();

        bool found = false;
        int index;

        // find sequence we need:
        // ldarg_1 followed immediately by isinst MeleeWeapon
        for (index = 0; index < instructionsList.Count - 1; index++)
        {
            if (instructionsList[index].opcode == OpCodes.Ldarg_1 &&
                instructionsList[index + 1].Is(OpCodes.Isinst, typeof(MeleeWeapon)))
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            LogError(
                "Could not locate instruction sequence:\n\tldarg_1\n\tisinst StardewValley.Tools.MeleeWeapon\nin StardewValley.TerrainFeatures.Tree::performToolAction.\nAborting transpiler.");
            return instructions;
        }

        // sequence found - excise from list
        instructionsList.RemoveRange(index, 2);

        // change brfalse.s after to unconditional jump
        instructionsList[index] = new CodeInstruction(OpCodes.Br, instructionsList[index].operand);

        // store modified il code in case we need to dump it
        ilCode = instructionsList;

        return instructionsList.AsEnumerable();
    }

    public static void LogTrace(string message)
    {
        Logger.Log(message);
    }

    public static void LogError(string message)
    {
        Logger.Log(message, LogLevel.Error);
    }
}
