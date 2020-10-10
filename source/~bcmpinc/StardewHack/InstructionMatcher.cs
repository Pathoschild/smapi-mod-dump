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
using Harmony;

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
    
    internal class IM_CodeInstruction : InstructionMatcher
    {
        readonly CodeInstruction query;
        public IM_CodeInstruction(CodeInstruction q) {
            query = q;
        }

        public override bool match(CodeInstruction instruction) {
            // Exact match.
            if (query == instruction) return true;
            
            // Check Opcode
            if (!instruction.opcode.Equals(query.opcode)) return false;
            
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
        public IM_OpCode(OpCode q) {
            query = q;
        }

        public override bool match(CodeInstruction instruction) {
            return instruction.opcode.Equals(query);
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
