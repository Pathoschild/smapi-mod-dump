/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Shockah.CommonModCode.IL
{
	public sealed class TranspileWorker
	{
		private readonly IList<CodeInstruction> Instructions;
		private int Index;
		public int Length { get; private set; }

		public int StartIndex => Index;
		public int EndIndex => Index + Length;

		private TranspileWorker(IList<CodeInstruction> instructions, int index, int length)
		{
			this.Instructions = instructions;
			this.Index = index;
			this.Length = length;
		}

		public static TranspileWorker? FindLabel(IList<CodeInstruction> instructions, Label label, int startIndex = 0, int? endIndex = null)
			=> FindInstructions(instructions, new Func<CodeInstruction, bool>[] { i => i.labels.Contains(label) }, startIndex, endIndex);

		public static TranspileWorker? FindInstructions(IList<CodeInstruction> instructions, IList<Func<CodeInstruction, bool>> instructionsToFind, int startIndex = 0, int? endIndex = null, int occurence = 1)
		{
			var maxIndex = (endIndex ?? instructions.Count) - instructionsToFind.Count;
			for (int index = startIndex; index < maxIndex; index++)
			{
				for (int toFindIndex = 0; toFindIndex < instructionsToFind.Count; toFindIndex++)
				{
					if (!instructionsToFind[toFindIndex](instructions[index + toFindIndex]))
						goto continueOuter;
				}

				if (--occurence == 0)
					return new TranspileWorker(instructions, index, instructionsToFind.Count);
				continueOuter:;
			}
			return null;
		}

		public static TranspileWorker? FindInstructionsBackwards(IList<CodeInstruction> instructions, IList<Func<CodeInstruction, bool>> instructionsToFind, int? endIndex = null, int? startIndex = null, int occurence = 1)
		{
			var minIndex = (startIndex ?? 0) + instructionsToFind.Count - 1;
			var intEndIndex = endIndex ?? instructions.Count - 1;
			for (int index = intEndIndex; index >= minIndex; index--)
			{
				for (int toFindIndex = instructionsToFind.Count - 1; toFindIndex >= 0; toFindIndex--)
				{
					if (!instructionsToFind[toFindIndex](instructions[index + toFindIndex]))
						goto continueOuter;
				}

				if (--occurence == 0)
					return new TranspileWorker(instructions, index, instructionsToFind.Count);
				continueOuter:;
			}
			return null;
		}

		public CodeInstruction this[int index]
		{
			get => Instructions[index + this.Index];
			set => Instructions[index + this.Index] = value;
		}

		public IEnumerable<CodeInstruction> GetWorkerInstructions()
		{
			for (int i = 0; i < this.Length; i++)
				yield return this.Instructions[this.Index + i];
		}

		public TranspileWorker Clear()
		{
			for (int i = 0; i < this.Length; i++)
				this.Instructions.RemoveAt(this.Index);
			this.Length = 0;
			return this;
		}

		public TranspileWorker Replace(IEnumerable<CodeInstruction> newInstructions)
		{
			Clear();
			foreach (var instruction in newInstructions)
				this.Instructions.Insert(this.Index + this.Length++, instruction);
			return this;
		}

		public TranspileWorker Insert(int relativeIndex, IEnumerable<CodeInstruction> newInstructions, bool moveFirstInstructionLabels = true)
		{
			var firstInstructionLabels = new List<Label>(this.Instructions[this.Index].labels);
			if (moveFirstInstructionLabels && relativeIndex == 0)
				this.Instructions[this.Index].labels.Clear();
			int extraLength = 0;
			foreach (var instruction in newInstructions)
				this.Instructions.Insert(this.Index + relativeIndex + extraLength++, instruction);
			this.Length += extraLength;
			if (moveFirstInstructionLabels && relativeIndex == 0)
				foreach (var label in firstInstructionLabels)
					this.Instructions[this.Index + relativeIndex].labels.Add(label);
			return this;
		}

		public TranspileWorker Prefix(IEnumerable<CodeInstruction> newInstructions, bool moveFirstInstructionLabels = true)
			=> this.Insert(0, newInstructions, moveFirstInstructionLabels);

		public TranspileWorker Postfix(IEnumerable<CodeInstruction> newInstructions)
			=> this.Insert(this.Length, newInstructions, false);
	}
}
