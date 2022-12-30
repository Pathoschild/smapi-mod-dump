/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace AeroCore.Patches
{
	[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.DayUpdate))]
	public class PersistSpawned
	{
		public const string Flag = "tlitookilakin.aerocore.persist";
		public static void SetPersist(SObject what, bool persist = true)
		{
			if (persist)
				what.modData[Flag] = "T";
			else
				what.modData.Remove(Flag);
		}

		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> PersistPatch(IEnumerable<CodeInstruction> codes, ILGenerator gen)
			=> patcher.Run(codes, gen);

		private static readonly ILHelper patcher = new ILHelper(ModEntry.monitor, "spawnable persistance")
			.SkipTo(new CodeInstruction(OpCodes.Ldfld, typeof(SObject).FieldNamed(nameof(SObject.isSpawnedObject))))
			.Skip(2)
			.Transform(addCheck)
			.Remove(1)
			.Finish();

		private static IList<CodeInstruction> addCheck(ILHelper.ILEnumerator cursor)
			=> new[]
			{
				cursor.Current,
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldloc_S, 18),
				new(OpCodes.Call, typeof(PersistSpawned).MethodNamed(nameof(check))),
				new(OpCodes.Brtrue_S, cursor.Current.operand)
			};

		private static bool check(GameLocation loc, int index)
			=> loc.Objects.Pairs.ElementAt(index).Value.modData.ContainsKey(Flag);
	}
}
