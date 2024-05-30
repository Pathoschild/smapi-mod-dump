/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace FurnitureFramework
{
	static class Transpiler
	{

		static public bool are_equal(CodeInstruction a, CodeInstruction b, bool debug = false)
		{
			if (a.IsStloc() && b.IsStloc())
			{
				return true;
			}
			
			if (a.opcode != b.opcode) return false;

			if (a.opcode == OpCodes.Callvirt)
			{
				if (a.operand is MethodInfo mi_a && b.operand is MethodInfo mi_b)
				{
					return mi_a.Name == mi_b.Name;
				}
			}

			// do not check operands for these OpCodes
			if (
				a.opcode == OpCodes.Beq_S || a.opcode == OpCodes.Br_S ||
				a.opcode == OpCodes.Ldloca || a.opcode == OpCodes.Ldloca_S
			)
			{
				return true;
			}

			return Equals(a.operand, b.operand);
		}

		static public List<int> find_start_indices(
			IEnumerable<CodeInstruction> original,
			IEnumerable<CodeInstruction> to_find,
			bool debug = false
		)
		{
			List<int> indices = new();
 
			for (int i = 0; i < 1 + original.Count() - to_find.Count(); i++)
			{
				bool seq_matches = true;
				int j;
				for (j = 0; j < to_find.Count(); j++, i++)
				{
					CodeInstruction orig = original.ElementAt(i);
					CodeInstruction to_f = to_find.ElementAt(j);

					if (debug)
					{
						ModEntry.log($"original element at {i} :");
						ModEntry.log($"\topcode : {orig.opcode}");
						ModEntry.log($"\toperand : {orig.operand}");
						ModEntry.log($"to_find element at {j} :");
						ModEntry.log($"\topcode : {to_f.opcode}");
						ModEntry.log($"\toperand : {to_f.operand}");
					}

					if (!are_equal(orig, to_f, debug))
					{
						if (debug) ModEntry.log("Restart match");
						seq_matches = false;
						break;
					}
					else if (debug) ModEntry.log("Matching!");
				}
				if (seq_matches)
				{
					indices.Add(i-j);
					if (debug) ModEntry.log("Full Match found!", StardewModdingAPI.LogLevel.Info);
				}
			}

			return indices;
		}

		static public IEnumerable<CodeInstruction> replace_instructions(
			IEnumerable<CodeInstruction> instructions,
			List<CodeInstruction> to_replace,
			List<CodeInstruction> to_write,
			int match_limit = 1,
			bool debug = false
		)
		{
			List<int> start_indices = find_start_indices(instructions, to_replace, debug);

			if (start_indices.Count > match_limit)
			{
				start_indices.RemoveRange(match_limit, start_indices.Count - match_limit);
			}

			if (debug)
				ModEntry.log($"Transpiler found {start_indices.Count} instances to replace");

			int k = 0;

			List<CodeInstruction> new_inst = new();

			if (debug)
				ModEntry.log($"Starting to replace instructions");

			for (int i = 0; i < instructions.Count(); i++)
			{
				if (k < start_indices.Count && i == start_indices[k])
				{
					if (debug)
						ModEntry.log($"Replacing a set of instructions at index {i}");
					k++;
					i += to_replace.Count - 1;
					foreach (CodeInstruction instruction in to_write)
					{
						if (debug)
							ModEntry.log($"\t{instruction}");
						new_inst.Add(instruction);
					}
				}

				else
				{
					new_inst.Add(instructions.ElementAt(i));
				}
			}

			if (debug)
				ModEntry.log($"Finished replacing instructions");

			return new_inst;
		}
	}
}