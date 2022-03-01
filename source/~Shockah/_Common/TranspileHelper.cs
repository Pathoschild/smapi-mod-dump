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
using System.Reflection.Emit;

namespace Shockah.CommonModCode
{
	public static class TranspileHelper
	{
		public static bool IsLdcI4(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Ldc_I4 ||
				instruction.opcode == OpCodes.Ldc_I4_S ||
				instruction.opcode == OpCodes.Ldc_I4_0 ||
				instruction.opcode == OpCodes.Ldc_I4_1 ||
				instruction.opcode == OpCodes.Ldc_I4_2 ||
				instruction.opcode == OpCodes.Ldc_I4_3 ||
				instruction.opcode == OpCodes.Ldc_I4_4 ||
				instruction.opcode == OpCodes.Ldc_I4_5 ||
				instruction.opcode == OpCodes.Ldc_I4_6 ||
				instruction.opcode == OpCodes.Ldc_I4_7 ||
				instruction.opcode == OpCodes.Ldc_I4_8 ||
				instruction.opcode == OpCodes.Ldc_I4_M1;
		}

		public static bool IsLdcI4(this CodeInstruction instruction, int value)
		{
			switch (value)
			{
				case 0:
					if (instruction.opcode == OpCodes.Ldc_I4_0)
						return true;
					break;
				case 1:
					if (instruction.opcode == OpCodes.Ldc_I4_1)
						return true;
					break;
				case 2:
					if (instruction.opcode == OpCodes.Ldc_I4_2)
						return true;
					break;
				case 3:
					if (instruction.opcode == OpCodes.Ldc_I4_3)
						return true;
					break;
				case 4:
					if (instruction.opcode == OpCodes.Ldc_I4_4)
						return true;
					break;
				case 5:
					if (instruction.opcode == OpCodes.Ldc_I4_5)
						return true;
					break;
				case 6:
					if (instruction.opcode == OpCodes.Ldc_I4_6)
						return true;
					break;
				case 7:
					if (instruction.opcode == OpCodes.Ldc_I4_7)
						return true;
					break;
				case 8:
					if (instruction.opcode == OpCodes.Ldc_I4_8)
						return true;
					break;
				case -1:
					if (instruction.opcode == OpCodes.Ldc_I4_M1)
						return true;
					break;
			}
			return (instruction.opcode == OpCodes.Ldc_I4 && (int)instruction.operand == value) ||
				(value < 256 && instruction.opcode == OpCodes.Ldc_I4_S && ((instruction.operand is int @int && @int == value) || (instruction.operand is sbyte @byte && @byte == value)));
		}

		public static int? GetLdcI4Value(this CodeInstruction instruction)
		{
			if (instruction.opcode == OpCodes.Ldc_I4_0)
				return 0;
			else if (instruction.opcode == OpCodes.Ldc_I4_1)
				return 1;
			else if (instruction.opcode == OpCodes.Ldc_I4_2)
				return 2;
			else if (instruction.opcode == OpCodes.Ldc_I4_3)
				return 3;
			else if (instruction.opcode == OpCodes.Ldc_I4_4)
				return 4;
			else if (instruction.opcode == OpCodes.Ldc_I4_5)
				return 5;
			else if (instruction.opcode == OpCodes.Ldc_I4_6)
				return 6;
			else if (instruction.opcode == OpCodes.Ldc_I4_7)
				return 7;
			else if (instruction.opcode == OpCodes.Ldc_I4_8)
				return 8;
			else if (instruction.opcode == OpCodes.Ldc_I4_M1)
				return -1;
			else if (instruction.opcode == OpCodes.Ldc_I4)
				return (int)instruction.operand;
			else if (instruction.opcode == OpCodes.Ldc_I4_S)
				return (sbyte)instruction.operand;
			else
				return null;
		}

		public static int? GetLdargIndex(this CodeInstruction instruction)
		{
			if (instruction.opcode == OpCodes.Ldarg_0)
				return 0;
			else if (instruction.opcode == OpCodes.Ldarg_1)
				return 1;
			else if (instruction.opcode == OpCodes.Ldarg_2)
				return 2;
			else if (instruction.opcode == OpCodes.Ldarg_3)
				return 3;
			else if (instruction.opcode == OpCodes.Ldarg)
				return (int)instruction.operand;
			else if (instruction.opcode == OpCodes.Ldarg_S)
				return (sbyte)instruction.operand;
			else
				return null;
		}

		public static bool IsBrtrue(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Brtrue || instruction.opcode == OpCodes.Brtrue_S;
		}

		public static bool IsBrfalse(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Brfalse || instruction.opcode == OpCodes.Brfalse_S;
		}

		public static bool IsBle(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Ble || instruction.opcode == OpCodes.Ble_S;
		}

		public static bool IsBneUn(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Bne_Un || instruction.opcode == OpCodes.Bne_Un_S;
		}

		public static bool IsBge(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Bge || instruction.opcode == OpCodes.Bge_S;
		}

		public static bool IsBgeUn(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Bge_Un || instruction.opcode == OpCodes.Bge_Un_S;
		}

		public static bool IsBlt(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Blt || instruction.opcode == OpCodes.Blt_S;
		}

		public static bool IsBltUn(this CodeInstruction instruction)
		{
			return instruction.opcode == OpCodes.Blt_Un || instruction.opcode == OpCodes.Blt_Un_S;
		}
	}
}
