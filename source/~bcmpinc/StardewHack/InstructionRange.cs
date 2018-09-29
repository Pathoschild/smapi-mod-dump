using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using StardewModdingAPI;

namespace StardewHack
{
    /** A helper class for manipulating short sequences of instructions. 
     * Note: The range becomes invalid whenever the list of instructions is changed from outside this class.
     */
    public class InstructionRange {
        readonly List<CodeInstruction> insts;
        public int start;
        public int length;

        public InstructionRange(List<CodeInstruction> insts) {
            this.insts = insts;
            this.start = 0;
            this.length = insts.Count;
        }

        public InstructionRange(List<CodeInstruction> insts, int start, int length) {
            this.insts = insts;
            this.start = start;
            this.length = length;
        }

        /** Implementation of the instruction find methods. 
         * Parameter step can be negative.
         * The instruction at start won't match when searching backwards. 
         */
        public InstructionRange(List<CodeInstruction> insts, Object[] contains, int start=0, int step=1) {
            int count = insts.Count - contains.Length + 1;
            if (step<0) start -= contains.Length;
            int best_match = 0;
            for (int i=start; 0<=i && i<count; i+=step) {
                for (int j=0; j<contains.Length; j++) {
                    if (best_match<j) best_match=j;
                    var query = contains[j];
                    var inst = insts[i+j];
                    if (query == null) {
                        // No query
                    } else if (query == inst) {
                        // Exact match.
                    } else if (query is String) {
                        if (!inst.ToString().Contains(query as String)) goto NO_MATCH; 
                    } else if (query is MemberInfo) {
                        if (!query.Equals(inst.operand)) goto NO_MATCH;
                    } else if (query is OpCode) {
                        if (!inst.opcode.Equals(query)) goto NO_MATCH;
                    } else if (query is CodeInstruction) {
                        CodeInstruction qin = query as CodeInstruction;
                        if (!inst.opcode.Equals(qin.opcode)) goto NO_MATCH;
                        if (!inst.operand.Equals(qin.operand)) goto NO_MATCH;
                    } else {
                        throw new ArgumentException("Unsupported type "+query.GetType()+" for argument "+(j+1)+": " + query);
                    }
                }
                //Hack.Log($"Found match at {i} of length {contains.Length}:");
                this.insts = insts;
                this.start = i;
                this.length = contains.Length;
                //Print();
                return;

                NO_MATCH:;
            }
            // No match found, throw a descriptive Exception.
            if (contains.Length == 1) {
                throw new System.IndexOutOfRangeException("Could not find instruction: '"+contains[0]+"'");
            } else {
                System.Text.StringBuilder msg = new System.Text.StringBuilder("Could not find instruction sequence (failed to match line "+best_match+"):");
                foreach (Object obj in contains) {
                    msg.Append("\n  " + obj);
                }
                throw new System.IndexOutOfRangeException(msg.ToString());
            }
        }

        /** An empty InstructionRange pointing to the start of this range. */
        public InstructionRange Start { get { return new InstructionRange(insts, start, 0); } }

        /** An empty InstructionRange pointing to the end of this range. */
        public InstructionRange End   { get { return new InstructionRange(insts, start + length, 0); } }

        /** Moves all jump labels for 'from' to 'to'. */
        public void ReplaceJump(int @from, CodeInstruction to) {
            var f = insts[start + @from];
            if (f == to) return;
            to.labels.AddRange(f.labels);
            f.labels.Clear();
        }

        /** Inserts the specified list of instructions before this range. */
        public void Prepend(params CodeInstruction[] new_insts) {
            insts.InsertRange(start, new_insts);
            length += new_insts.Length;
        }

        /** Inserts the specified list of instructions at the given position. */
        public void Insert(int i, params CodeInstruction[] new_insts) {
            insts.InsertRange(start + i, new_insts);
            length += new_insts.Length;
        }

        /** Inserts the specified list of instructions after this range. */
        public void Append(params CodeInstruction[] new_insts) {
            insts.InsertRange(start + length, new_insts);
            length += new_insts.Length;
        }

        /** Removes all instructions contained within this range. */
        public void Remove() {
            insts.RemoveRange(start, length);
            length = 0;
        }

        /** Access elements relative to the start of this range. */
        public CodeInstruction this[int index] {
            get { return insts[start+index]; }
            set { insts[start+index] = value; }
        }

        /** Find the first occurance of the given sequence of instructions that follows this range.
         * See InstructionHelpers.Find() for how the matching is performed.
         */
        public InstructionRange FindNext(params Object[] contains) {
            return new InstructionRange(insts, contains, start+length);
        }

        /** Find the first occurance of the given sequence of instructions that precedes this range.
         * See InstructionHelpers.Find() for how the matching is performed.
         */
        public InstructionRange FindPrevious(params Object[] contains) {
            return new InstructionRange(insts, contains, start, -1);
        }

        /** Extend the range up-to and including the specified instructions. */
        public void Extend(params Object[] contains) {
            var ext = FindNext(contains);
            length = ext.start + ext.length - start;
        }

        /** Extend the range by searching backwards to include the specified instructions. */
        public void ExtendBackwards(params Object[] contains) {
            var ext = FindPrevious(contains);
            length = start + length - ext.start;
            start = ext.start;
        }

        /** Replaces the instructions within this range with the specified new instructions. */
        public void Replace(params CodeInstruction[] new_insts) {
            if (length == new_insts.Length) {
                for (int i=0; i<length; i++) {
                    insts[start+i] = new_insts[i];
                }
            } else {
                Remove();
                Append(new_insts);
                length = new_insts.Length;
            }
        }

        /** Get the jump target of the branch instruction at index i. 
         * The resulting InstructionRange will have length 0. 
         * Use Extend or ExtendBackwards to give it a size.
         * The length being 0, ExtendBackwards won't include the instruction being pointed at.
         */
        public InstructionRange Follow(int i) {
            var op = insts[start+i].operand;
            if (op == null || !(op is Label)) {
                throw new Exception("Expected a branch instruction, got: "+insts[start+i]);
            }
            Label lbl = (Label)op;
            int pos = insts.FindIndex(inst => inst.labels.Contains(lbl));
            if (pos < 0) throw new Exception($"Label not found: LBL_{lbl.GetHashCode()}");
            var res = new InstructionRange(insts, pos, 0);
            // Hack.Log($"Jump points to {pos}:");
            return res;
        }

        /** Writes the instruction range to console. */
        public void Print(IMonitor monitor) {
            monitor.Log("-----");
            for (int i=0; i<length; i++) {
                var inst = insts[start+i];

                // Print any labels
                foreach (var lbl in inst.labels) {
                    monitor.Log($"LBL_{lbl.GetHashCode()}:");
                }

                // Print the operation.
                string res;
                if (inst.operand == null) {
                    res = $"{inst.opcode}";
                } else if (inst.operand is Label) {
                    res = $"{inst.opcode} LBL_{inst.operand.GetHashCode()}";
                } else if (inst.operand is string) {
                    res = $"{inst.opcode} \"{inst.operand}\"";
                } else {
                    res = inst.ToString();
                }
                monitor.Log("  " + res);
            }
            monitor.Log("-----");
        }

        public CodeInstruction[] ToArray() {
            CodeInstruction[] array = new CodeInstruction[length];
            for (int index=0; index<length; index++) {
                array[index] = insts[start+index];
            }
            return array;
        }

        public override string ToString () {
            return string.Format ("[InstructionRange[{0}..{1}], length={2}]", start, start+length-1, length);
        }
    }
}

