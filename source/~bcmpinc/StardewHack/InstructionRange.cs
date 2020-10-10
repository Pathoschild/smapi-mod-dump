/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using StardewModdingAPI;

namespace StardewHack
{
    /// <summary>
    /// A helper class for manipulating short sequences of instructions. 
    /// Note: The range becomes invalid whenever the list of instructions is changed from outside this class.
    /// </summary>
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

        /// <summary>
        /// Implementation of the instruction find methods. 
        /// Parameter step can be negative.
        /// The instruction at start won't match when searching backwards. 
        /// </summary>
        public InstructionRange(List<CodeInstruction> insts, InstructionMatcher[] contains, int start=0, int step=1) {
            int count = insts.Count - contains.Length + 1;
            if (step<0) start -= contains.Length;
            int best_match = 0;
            string best_text = "(none)";
            for (int i=start; 0<=i && i<count; i+=step) {
                for (int j=0; j<contains.Length; j++) {
                    var query = contains[j];
                    var inst = insts[i+j];
                    if (best_match < j) {
                        best_match = j;
                        best_text = inst.ToString();
                    }
                    if (query != null && !query.match(inst)) {
                        goto NO_MATCH;
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
                throw new IndexOutOfRangeException("Could not find instruction: '"+contains[0]+"'");
            } 
            System.Text.StringBuilder msg = new System.Text.StringBuilder($"Could not find instruction sequence (failed to match line {best_match}: \"{best_text}\"):");
            foreach (Object obj in contains) {
                msg.Append("\n  " + obj);
            }
            throw new IndexOutOfRangeException(msg.ToString());
        }

        /// <summary>
        /// An empty InstructionRange pointing to the start of this range.
        /// </summary>
        public InstructionRange Start { get { return new InstructionRange(insts, start, 0); } }

        /// <summary>
        /// An empty InstructionRange pointing to the end of this range.
        /// </summary>
        public InstructionRange End   { get { return new InstructionRange(insts, start + length, 0); } }
        
        /// <summary>
        /// Returns the requested sub-range.
        /// </summary>
        public InstructionRange SubRange(int start, int length) {
            return new InstructionRange(insts, this.start + start, length);
        }

        /// <summary>
        /// Moves all jump labels for 'from' to 'to'./// </summary>
        public void ReplaceJump(int @from, CodeInstruction to) {
            var f = insts[start + @from];
            if (f == to) return;
            to.labels.AddRange(f.labels);
            f.labels.Clear();
        }

        /// <summary>
        /// Inserts the specified list of instructions before this range.
        /// </summary>
        public void Prepend(params CodeInstruction[] new_insts) {
            insts.InsertRange(start, new_insts);
            length += new_insts.Length;
        }

        /// <summary>
        /// Inserts the specified list of instructions at the given position.
        /// </summary>
        public void Insert(int i, params CodeInstruction[] new_insts) {
            insts.InsertRange(start + i, new_insts);
            length += new_insts.Length;
        }

        /// <summary>
        /// Inserts the specified list of instructions after this range.
        /// </summary>
        public void Append(params CodeInstruction[] new_insts) {
            insts.InsertRange(start + length, new_insts);
            length += new_insts.Length;
        }

        /// <summary>
        /// Removes all instructions contained within this range. 
        /// Automatically fixes jumps to the start of this code range to point to the first instruction after this code range.
        /// </summary>
        public void Remove() {
            ReplaceJump(0, insts[start + length]);
            insts.RemoveRange(start, length);
            length = 0;
        }

        /// <summary>
        /// Access elements relative to the start of this range.
        /// </summary>
        public CodeInstruction this[int index] {
            get { return insts[start+index]; }
            set { insts[start+index] = value; }
        }

        /// <summary>
        /// Find the first occurance of the given sequence of instructions that follows this range.
        /// See InstructionHelpers.Find() for how the matching is performed.
        /// </summary>
        public InstructionRange FindNext(params InstructionMatcher[] contains) {
            return new InstructionRange(insts, contains, start+length);
        }

        /// <summary>
        /// Find the first occurance of the given sequence of instructions that precedes this range.
        /// See InstructionHelpers.Find() for how the matching is performed.
        /// </summary>
        public InstructionRange FindPrevious(params InstructionMatcher[] contains) {
            return new InstructionRange(insts, contains, start, -1);
        }

        /// <summary>
        /// Extend the range up-to and including the specified instructions.
        /// </summary>
        public void Extend(params InstructionMatcher[] contains) {
            Extend(FindNext(contains));
        }
        public void Extend(InstructionRange ext) {
            length = ext.start + ext.length - start;
        }

        /// <summary>
        /// Extend the range backwards to include the specified instructions.
        /// </summary>
        public void ExtendBackwards(params InstructionMatcher[] contains) {
            ExtendBackwards(FindPrevious(contains));
        }
        public void ExtendBackwards(InstructionRange ext) {
            length = start + length - ext.start;
            start = ext.start;
        }
        

        /// <summary>
        /// Replaces the instructions within this range with the specified new instructions.
        /// Automatically fixes jumps to the start of this code range to point to the start of the new instructions.
        /// </summary>
        public void Replace(params CodeInstruction[] new_insts) {
            ReplaceJump(0, new_insts[0]);
            if (length == new_insts.Length) {
                for (int i=0; i<length; i++) {
                    insts[start+i] = new_insts[i];
                }
            } else {
                insts.RemoveRange(start, length);
                insts.InsertRange(start, new_insts);
                length = new_insts.Length;
            }
        }

        /// <summary>
        /// Get the jump target of the branch instruction at index i. 
        /// The resulting InstructionRange will have length 0. 
        /// Use Extend or ExtendBackwards to give it a size.
        /// The length being 0, ExtendBackwards won't include the instruction being pointed at.
        /// </summary>
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

        /// <summary>
        /// Writes the instruction range to console.
        /// </summary>
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
                } else if (inst.operand is LocalBuilder) {
                    var lb = inst.operand as LocalBuilder;
                    res = $"{inst.opcode} [{lb.LocalIndex}] ({lb.LocalType}) - {lb.GetHashCode()}";
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

