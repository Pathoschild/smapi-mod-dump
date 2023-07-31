/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Utility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AchtuurCore.Patches;


public enum ILCodeInsertMethod
{
    /// <summary>
    /// Inserts IL code before the first match.
    /// </summary>
    InsertBeforeStart,
    /// <summary>
    /// Insert IL code in between the first and second opcodes of the matching sequence
    /// </summary>
    InsertAtStart,
    /// <summary>
    /// Insert IL code in between second to last and last opcodes of matching sequence
    /// </summary>
    InsertOneBeforeEnd,
    /// <summary>
    /// Insert IL code after matching sequence
    /// </summary>
    InsertAtEnd,

}

public enum ILCodeReplaceMethod
{
    /// <summary>
    /// Replace every instruction from sequence
    /// </summary>
    ReplaceAll,

    /// <summary>
    /// Replace every instruction from start of sequence, only keeping the last one in the sequence
    /// </summary>
    ReplaceAllExceptEnd,

    /// <summary>
    /// Replace only the instruction 1 before the last in the match sequence
    /// </summary>
    ReplaceBeforeEnd,

    /// <summary>
    /// Start replacing starting with the last instruction in the sequence match
    /// </summary>
    ReplaceEnd
}


/// <summary>
/// Generic patcher that can be implemented by other patches. Derived classes should implement 1 method that serves as the patch.
/// </summary>
public abstract class GenericPatcher
{
    /// <summary>
    /// Apply the patch the derived method implements using <see cref="Harmony.Patch"/>
    /// </summary>
    /// <param name="harmony">Harmony instance</param>
    /// <param name="monitor">Monitor instance provided by <see cref="StardewModdingAPI.IMonitor"/>, used for logging if necessary</param>
    public abstract void Patch(Harmony harmony);

    /// <summary>
    /// Get method that will be patched as <see cref="MethodInfo"/> using <see cref="System.Reflection"/>.
    /// </summary>
    /// <typeparam name="MethodClass">Class that contains the method that will be patched</typeparam>
    /// <param name="methodname">Name of the original method that will be patched</param>
    /// <returns></returns>
    public MethodInfo GetOriginalMethod<MethodClass>(string methodname)
    {
        return AccessTools.Method(typeof(MethodClass), methodname);
    }

    public MethodInfo GetOriginalMethod(Type classtype, string methodname)
    {
        return AccessTools.Method(classtype, methodname);
    }


    /// <summary>
    /// Get <see cref="HarmonyMethod"/> from this patcher' patch method
    /// </summary>
    /// <param name="methodName">Name of the method that implements the patch</param>
    /// <param name="priority">Priority of returned <see cref="HarmonyMethod"/>, use <see cref="HarmonyPriority"/> to set priorities.</param>
    /// <param name="before"><inheritdoc cref="HarmonyMethod.before"/></param>
    /// <param name="after"><inheritdoc cref="HarmonyMethod.after"/></param>
    /// <returns></returns>
    public HarmonyMethod GetHarmonyMethod(string name, int? priority = null, string before = null, string after = null, bool? debug = null)
    {
        HarmonyMethod method = new HarmonyMethod(AccessTools.Method(this.GetType(), name));

        if (priority is not null)
            method.priority = (int)priority;

        if (before is not null)
            method.before = new[] { before };

        if (after is not null)
            method.after = new[] { after };

        if (debug is not null)
            method.debug = (bool)debug;

        return method;
    }


    public static void MatchAndInsertILCode(List<CodeInstruction> method_il, List<CodeInstruction> match_sequence, List<CodeInstruction> instr_replace, ILCodeInsertMethod InsertMethod = ILCodeInsertMethod.InsertAtStart)
    {
        // Get range that corresponds to bobber_add_sequence in original code
        Range? sequence_index = GenericPatcher.FindILSequence(method_il, match_sequence);

        if (sequence_index.HasValue)
        {
            Range r = sequence_index.Value;
            int i = GetInsertMethodIndex(r, InsertMethod);

            if (i != -1)
            {
                // Insert multiply code before add instruction
                // effectively means we change the code to `this.bobberBarSpeed += gravity * mul_value`
                GenericPatcher.InsertILCode(method_il, instr_replace, i);
            }
        }
    }

    public static void MatchAndReplaceILCode(List<CodeInstruction> method_il, List<CodeInstruction> match_sequence, List<CodeInstruction> repl_sequence, ILCodeReplaceMethod replaceMethod)
    {
        // Get range that corresponds to bobber_add_sequence in original code
        Range? sequence_index = GenericPatcher.FindILSequence(method_il, match_sequence);

        if (sequence_index.HasValue)
        {
            Range r = sequence_index.Value;
            Range repl_range = GetReplaceMethodIndex(r, replaceMethod);

            if (repl_range.Start.Value != -1)
            {
                // Insert multiply code before add instruction
                // effectively means we change the code to `this.bobberBarSpeed += gravity * mul_value`
                GenericPatcher.ReplaceILCode(method_il, repl_sequence, repl_range);
            }
        }
    }

    public static int GetInsertMethodIndex(Range r, ILCodeInsertMethod insertMethod)
    {
        switch (insertMethod)
        {
            case ILCodeInsertMethod.InsertBeforeStart:
                return r.Start.Value;

            case ILCodeInsertMethod.InsertAtStart:
                return r.Start.Value + 1;

            case ILCodeInsertMethod.InsertOneBeforeEnd:
                return r.End.Value;

            case ILCodeInsertMethod.InsertAtEnd:
                return r.End.Value + 1;

            default: return -1;
        }
    }

    public static Range GetReplaceMethodIndex(Range r, ILCodeReplaceMethod replaceMethod)
    {
        switch (replaceMethod)
        {
            case ILCodeReplaceMethod.ReplaceAll:
                return r;
            case ILCodeReplaceMethod.ReplaceAllExceptEnd:
                return new Range(r.Start.Value, r.End.Value - 1);
            case ILCodeReplaceMethod.ReplaceBeforeEnd:
                return new Range(r.End.Value - 1, r.End.Value - 1);
            case ILCodeReplaceMethod.ReplaceEnd:
                return new Range(r.End.Value, r.End.Value);
            default: return new Range(-1, -1);
        }
    }

    /// <summary>
    /// Searches for sequence of instructions given by <paramref name="instr_sequence"/> in <paramref name="method_il"/>.
    /// </summary>
    /// <param name="method_il">Method IL instructions</param>
    /// <param name="instr_sequence">Instruction sequence to look for</param>
    /// <returns>Range of instructions that match sequence, start and ending index both inclusive. returns null if sequence not found</returns>
    public static Range? FindILSequence(List<CodeInstruction> method_il, List<CodeInstruction> instr_sequence)
    {
        if (method_il.Count <= 0 || instr_sequence.Count <= 0)
            return null;

        var enumerated_sequence = instr_sequence.Select((instr, i) => new { Instr = instr, Index = i });
        for (int i = 0; i < method_il.Count; i++)
        {

            if (i == 830)
            {
                int x = 5;
            }

            // Check if at this i there is a 
            if (enumerated_sequence.All(enum_instr => CodeInstructionMatches(method_il[i + enum_instr.Index], enum_instr.Instr)))
            {
                return new Range(i, i + instr_sequence.Count - 1);
            }
        }
        return null;
    }

    /// <summary>
    /// Insert <paramref name="insert_instr"/> in <paramref name="method_il"/>, starting at index <paramref name="index"/>
    /// </summary>
    /// <param name="method_il">IL code to insert instructions into, passed by reference</param>
    /// <param name="insert_instr">instructions to insert</param>
    /// <param name="index">Index in <paramref name="method_il"/> to put <paramref name="insert_instr"/></param>
    public static void InsertILCode(List<CodeInstruction> method_il, List<CodeInstruction> insert_instr, int index)
    {
        // Return if invalid input
        if (method_il.Count < index || insert_instr.Count <= 0)
            return;

        // Transfer labels
        if (method_il[index].labels.Count > 0)
        {
            insert_instr[0].labels = new List<Label>(method_il[index].labels);
            method_il[index].labels.Clear();
        }

        method_il.InsertRange(index, insert_instr);
    }

    public static void ReplaceILCode(List<CodeInstruction> method_il, List<CodeInstruction> repl_instr, int index)
    {
        //replace single instruction and insert rest
        ReplaceILCode(method_il, repl_instr, new Range(index, index));
    }

    /// <summary>
    /// Replace instructions in <paramref name="method_il"/> at <paramref name="repl_range"/> (START & END INCLUSIVE) with <paramref name="repl_instr"/>
    /// </summary>
    /// <param name="method_il"></param>
    /// <param name="repl_instr"></param>
    /// <param name="repl_range"></param>
    public static void ReplaceILCode(List<CodeInstruction> method_il, List<CodeInstruction> repl_instr, Range repl_range)
    {
        // Return if invalid input
        if (method_il.Count < repl_range.Start.Value || repl_instr.Count <= 0)
            return;

        int j = 0;
        // These instructions will REPLACE instructions in method_il
        for (int i = repl_range.Start.Value; i <= repl_range.End.Value; i++)
        {
            // Transfer labels
            if (method_il[i].labels.Count > 0)
            {
                repl_instr[j].labels = new List<Label>(method_il[i].labels);
                //method_il[index].labels.Clear();
            }

            method_il[i] = repl_instr[j++];
        }

        // Insert rest of instructions in repl_range after replaced part
        int range_len = repl_range.End.Value - repl_range.Start.Value + 1;
        method_il.InsertRange(repl_range.End.Value + 1, repl_instr.Skip(range_len));
    }

    private static bool CodeInstructionMatches(CodeInstruction lhs, CodeInstruction rhs)
    {
        return lhs.opcode == rhs.opcode && OperandsMatch(lhs.operand, rhs.operand);
    }

    private static bool OperandsMatch(object lhs, object rhs)
    {
        // if either operand is null, dont do comparison by returning true
        if (lhs == null || rhs == null)
            return true;

        if (TypeChecker.isType<float>(lhs) && TypeChecker.isType<float>(rhs))
            return Math.Abs((float)lhs - (float)rhs) < 0.00000001f;

        else if (TypeChecker.isType<double>(lhs) && TypeChecker.isType<double>(rhs))
            return Math.Abs((double)lhs - (double)rhs) < 0.00000001;

        else
            return lhs == rhs;
    }

}
