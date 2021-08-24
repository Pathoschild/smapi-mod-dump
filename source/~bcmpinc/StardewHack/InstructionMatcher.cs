/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System;
using System.Reflection.Emit;
using HarmonyLib;

namespace StardewHack
{
    abstract public class InstructionMatcher
    {
        public abstract bool match(CodeInstruction instruction);
        public abstract override string ToString();

        public static implicit operator InstructionMatcher(CodeInstruction query) => new IM_CodeInstruction(query);
        public static implicit operator InstructionMatcher(OpCode query) => new IM_OpCode(query);
        
        public static InstructionMatcher AnyOf(params InstructionMatcher[] query) => new IM_AnyOf(query);
    }

    internal static class FuzzyOpcodeTable {
        static OpCode[] table;
        
        static void init() {
            table = new OpCode[256];

            add_pair(OpCodes.Bge,     OpCodes.Bge_S);
            add_pair(OpCodes.Bge_Un,  OpCodes.Bge_Un_S);
            add_pair(OpCodes.Bgt,     OpCodes.Bgt_S);
            add_pair(OpCodes.Bgt_Un,  OpCodes.Bgt_Un_S);
            add_pair(OpCodes.Ble,     OpCodes.Ble_S);
            add_pair(OpCodes.Ble_Un,  OpCodes.Ble_Un_S);
            add_pair(OpCodes.Blt,     OpCodes.Blt_S);
            add_pair(OpCodes.Blt_Un,  OpCodes.Blt_Un_S);
            add_pair(OpCodes.Beq,     OpCodes.Beq_S);
            add_pair(OpCodes.Bne_Un,  OpCodes.Bne_Un_S);

            add_pair(OpCodes.Br,      OpCodes.Br_S);
            add_pair(OpCodes.Brfalse, OpCodes.Brfalse_S);
            add_pair(OpCodes.Brtrue,  OpCodes.Brtrue_S);

            add_pair(OpCodes.Call,    OpCodes.Callvirt);
        }

        static void add_pair(OpCode a, OpCode b) {
            if (table[a.Value].Size > 0) throw new ArgumentException("OpCode " + a.Name + " is already paired.");
            if (table[b.Value].Size > 0) throw new ArgumentException("OpCode " + b.Name + " is already paired.");
            table[a.Value] = b;
            table[b.Value] = a;
        }

        public static OpCode alt(OpCode v) {
            if (v.Value < 0 || v.Value >= 256) return v;
            if (table == null) init();
            var r = table[v.Value];
            if (r.Size == 0) return v;
            return r;
        }

    };


    internal class IM_CodeInstruction : InstructionMatcher
    {
        readonly CodeInstruction query;
        readonly OpCode alt;

        public IM_CodeInstruction(CodeInstruction q) {
            query = q;
            alt = FuzzyOpcodeTable.alt(q.opcode);
        }

        public override bool match(CodeInstruction instruction) {
            // Exact match.
            if (query == instruction) return true;

            // Check Opcode
            if (!query.opcode.Equals(instruction.opcode) && !alt.Equals(instruction.opcode)) return false;
            
            // Check Operand
            if (instruction.operand == null) {
                return query.operand == null;
            } 
            if (query.operand==null) return false;
            if (instruction.operand.Equals(query.operand)) return true;
            
            // In case the operand is an integer, but their boxing types don't match.
            try {
                if (Convert.ToInt64(instruction.operand) != Convert.ToInt64(query.operand)) return false;
                return true;
            } catch {
                return false;
            }
        }

        public override string ToString() {
            return query.ToString();
        }
    }

    // Matches based on OpCode
    internal class IM_OpCode : InstructionMatcher
    {
        readonly OpCode query;
        readonly OpCode alt;

        public IM_OpCode(OpCode q) {
            query = q;
            alt = FuzzyOpcodeTable.alt(q);
        }

        public override bool match(CodeInstruction instruction) {
            return query.Equals(instruction.opcode) || alt.Equals(instruction.opcode);
        }

        public override string ToString() {
            return query.ToString();
        }
    }

    // Matches if any of the specified opcodes match.
    internal class IM_AnyOf : InstructionMatcher
    {
        readonly InstructionMatcher[] query;
        public IM_AnyOf(InstructionMatcher[] q) {
            query = q;
        }

        public override bool match(CodeInstruction instruction) {
            foreach(var q in query) {
                if (q.match(instruction)) return true;
            }
            return false;
        }

        public override string ToString() {
            return "<" + String.Join<InstructionMatcher>(" | ", query) + ">";
        }
    }
}
