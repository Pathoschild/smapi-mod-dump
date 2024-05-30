/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System.Reflection.Emit;
using System.Reflection;

namespace ItemExtensions.Patches;

internal class FishPondPatches
{

#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif

    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(TrainPatches)}\": transpiling SDV method \"FishPond.JumpFish\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(FishPond), nameof(FishPond.JumpFish)),
            transpiler: new HarmonyMethod(typeof(FishPondPatches), nameof(Transpiler_JumpFish))
        );

        Log($"Applying Harmony patch \"{nameof(TrainPatches)}\": transpiling SDV method \"FishPond.doAction\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(FishPond), nameof(FishPond.doAction)),
            transpiler: new HarmonyMethod(typeof(FishPondPatches), nameof(Transpiler_doAction))
        );
    }

    /// <summary>
    /// Transpiles JumpFish to allow no jumping.
    /// </summary>
    /// <param name="instructions">Original instructions</param>
    /// <param name="il">IL</param>
    /// <returns>The code (either original or transpiled).</returns>
    private static IEnumerable<CodeInstruction> Transpiler_JumpFish(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        //new one
        var codes = new List<CodeInstruction>(instructions);
        var instructionsToInsert = new List<CodeInstruction>();

        //find the code that chooses silhouette- aka if there ARE fish to jump
        CodeInstruction chooseSilhouette = null;
        for (var i = 0; i < codes.Count - 1; i++)
        {
            if (codes[i].opcode != OpCodes.Ldsfld)
                continue;

            if (codes[i + 1].opcode != OpCodes.Ldarg_0)
                continue;

            chooseSilhouette = codes[i];
            break;
        }

        if (chooseSilhouette is null)
        {
            Log("chooseSilhouette wasn't found.");
            return codes.AsEnumerable();
        }

        //add label for brfalse
        var brfalseLabel = il.DefineLabel();
        chooseSilhouette.labels ??= new List<Label>();
        chooseSilhouette.labels.Add(brfalseLabel);

        /* if (MustSkip(this))
         * {
         *      return false;
         * }
         */

        //arguments
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0)); //this
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FishPondPatches), nameof(MustSkip))));

        //tell where to go if false
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse, brfalseLabel));

        //if true: return false
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Ret));

        Log($"codes count: {codes.Count}, insert count: {instructionsToInsert.Count}");
        Log("Inserting method");
        codes.InsertRange(6, instructionsToInsert);

        return codes.AsEnumerable();
    }

    /// <summary>
    /// Checks fish pond's fish.
    /// </summary>
    /// <param name="pond"></param>
    /// <returns>Whether the jumping should be skipped.</returns>
    internal static bool MustSkip(FishPond pond)
    {
        var fish = pond.GetFishObject();

        if (fish is null)
            return false;

        if (Game1.objectData.TryGetValue(fish.ItemId, out var objData) == false)
            return false;

        if (objData.CustomFields is null || objData.CustomFields?.Any() == false) 
            return false;

        if (objData.CustomFields.ContainsKey(Additions.ModKeys.DisableJumping))
        {
            pond._fishSilhouettes.Clear();
            pond._jumpingFish.Clear();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Transpiles doAction to allow non-fish in ponds.
    /// </summary>
    /// <param name="instructions">Original instructions</param>
    /// <param name="il">IL</param>
    /// <returns>The code (either original or transpiled).</returns>
    private static IEnumerable<CodeInstruction> Transpiler_doAction(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        //new one
        var codes = new List<CodeInstruction>(instructions);
        var instructionsToInsert = new List<CodeInstruction>();

        var index = codes.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo { Name: "get_Category" }) + 2; //+2, for "ldc.i4.s -4" & "beq.s"
#if DEBUG
        Log($"index: {index}", LogLevel.Info);
#endif

        CodeInstruction redirectHere = null;
        for (var i = 0; i < codes.Count - 1; i++)
        {
            if (codes[i].opcode != OpCodes.Ldarg_0)
                continue;

            if (codes[i + 1].opcode != OpCodes.Ldfld)
                continue;

            if (codes[i + 2].opcode != OpCodes.Callvirt)
                continue;

            if (codes[i + 3].opcode != OpCodes.Brfalse)
                continue;

            redirectHere = codes[i];
            break;
        }

        if (redirectHere is null)
        {
            Log("Code to redirect to wasn't found.");
            return codes.AsEnumerable();
        }

        //add label for brtrue
        var brTrueLabel = il.DefineLabel();
        redirectHere.labels ??= new List<Label>();
        redirectHere.labels.Add(brTrueLabel);

        if (index <= -1)
            return codes.AsEnumerable();

        /* if (who.ActiveObject != null && (AllowInPond() || who.ActiveObject.Category == -4 || who.ActiveObject.QualifiedItemId == "(O)393" || who.ActiveObject.QualifiedItemId == "(O)397"))
         * {
         *      //rest of normal code
         * }
         */

        //call my code w/ prev args
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FishPondPatches), nameof(AllowInPond))));

        //tell where to go if false
        instructionsToInsert.Add(new CodeInstruction(OpCodes.Brtrue, brTrueLabel));

        Log($"codes count: {codes.Count}, insert count: {instructionsToInsert.Count}");
        Log("Inserting method");
        codes.InsertRange(index + 1, instructionsToInsert);

        return codes.AsEnumerable();
    }

    /// <summary>
    /// Checks item data when adding to ponds.
    /// </summary>
    /// <returns>Whether the item can be force-added, regardless of category.</returns>
    private static bool AllowInPond()
    {
        var obj = Game1.player.ActiveObject;

        if (obj is null)
            return false;

        if (obj.Category == -4)
            return true;

        if (Game1.objectData.TryGetValue(obj.ItemId, out var objData) == false)
            return false;

        if (objData.CustomFields is null || objData.CustomFields?.Any() == false)
            return false;

        if (objData.CustomFields.ContainsKey(Additions.ModKeys.AllowFishPond))
        {
            return true;
        }

        return false;
    }
}
