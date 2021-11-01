/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace CommonHarmony
{
    using System;
    using System.Collections.Generic;
    using HarmonyLib;

    internal class PatternPatch
    {
        private readonly IList<Action<LinkedList<CodeInstruction>>> _patches = new List<Action<LinkedList<CodeInstruction>>>();
        private readonly Queue<int> _patternIndex = new();

        private readonly List<CodeInstruction> _patterns = new();
        private int _endIndex;
        private int _index;
        private int _loop;
        private int _startIndex;

        /// <summary></summary>
        public int Skipped { get; private set; }

        /// <summary></summary>
        public bool Loop
        {
            get => this._loop == -1 || --this._loop > 0;
        }

        /// <summary></summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public PatternPatch Find(IEnumerable<CodeInstruction> pattern)
        {
            this._patterns.AddRange(pattern);
            this._patternIndex.Enqueue(this._patterns.Count);
            return this;
        }

        /// <summary></summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public PatternPatch Find(CodeInstruction pattern)
        {
            this._patterns.Add(pattern);
            this._patternIndex.Enqueue(this._patterns.Count);
            return this;
        }

        /// <summary></summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public PatternPatch Patch(Action<LinkedList<CodeInstruction>> patch)
        {
            this._patches.Add(patch);
            return this;
        }

        /// <summary></summary>
        /// <param name="skip"></param>
        /// <returns></returns>
        public PatternPatch Skip(int skip)
        {
            this.Skipped = skip;
            return this;
        }

        /// <summary></summary>
        /// <param name="loop"></param>
        /// <returns></returns>
        public PatternPatch Repeat(int loop)
        {
            this._loop = loop;
            return this;
        }

        /// <summary></summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public bool Matches(CodeInstruction instruction)
        {
            // Initialize end index
            if (this._startIndex == this._endIndex)
            {
                this._endIndex = this._patternIndex.Dequeue();
            }

            // Reset on loop
            if (this._index == this._endIndex)
            {
                this._index = this._startIndex;
            }

            // Opcode not matching
            if (!this._patterns[this._index].opcode.Equals(instruction.opcode))
            {
                this._index = this._startIndex;
                return false;
            }

            // Operand not matching
            if (this._patterns[this._index].operand is not null && !this._patterns[this._index].operand.Equals(instruction.operand))
            {
                this._index = this._startIndex;
                return false;
            }

            // Incomplete pattern search
            if (++this._index != this._endIndex)
            {
                return false;
            }

            // Complete pattern search
            if (this._patternIndex.Count <= 0)
            {
                return true;
            }

            // Incomplete pattern search
            this._startIndex = this._endIndex;
            return false;
        }

        /// <summary>Applies patches to the code stack at current index.</summary>
        /// <param name="rawStack"></param>
        public void Patches(LinkedList<CodeInstruction> rawStack)
        {
            foreach (var patch in this._patches)
            {
                patch?.Invoke(rawStack);
            }
        }
    }
}