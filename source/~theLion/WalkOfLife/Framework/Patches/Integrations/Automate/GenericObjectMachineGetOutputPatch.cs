/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GenericObjectMachineSetInputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GenericObjectMachineSetInputPatch()
		{
			Postfix = new(GetType(), nameof(GenericObjectMachineGetOutputPostfix));
		}

		/// <inheritdoc />
		public override Dictionary<string, int> Apply(Harmony harmony)
		{
			var stats = new Dictionary<string, int>
			{
				{ "patched", 0},
				{ "failed", 0},
				{ "ignored", 0},
				{ "prefixed", 0},
				{ "postfixed", 0},
				{ "transpiled", 0},
			};

			var targetMethods = TargetMethods().ToList();
			Log($"[Patch]: Found {targetMethods.Count} target methods for {GetType().Name}.", LogLevel.Trace);
			foreach (var method in targetMethods)
				try
				{
					Original = method;
					var results = base.Apply(harmony);
					
					// aggregate patch results to total stats
					foreach (var key in stats.Keys)
						stats[key] += results[key];
				}
				catch
				{
					// ignored
				}

			return stats;
		}

		#region harmony patches

		/// <summary>Patch to apply Artisan effects to automated machines.</summary>
		[HarmonyPostfix]
		private static void GenericObjectMachineGetOutputPostfix(object __instance)
		{
			if (__instance is null) return;

			var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
			if (machine is null || machine.heldObject.Value is null ||
			    !machine.heldObject.Value.IsArtisanGood()) return;

			var who = Game1.getFarmer(machine.owner.Value);
			if (!who.HasProfession("Artisan")) return;

			if (machine.heldObject.Value.Quality < SObject.bestQuality &&
			    new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
				machine.heldObject.Value.Quality += machine.heldObject.Value.Quality == SObject.medQuality ? 2 : 1;

			machine.MinutesUntilReady -= machine.MinutesUntilReady / 10;
		}

		#endregion harmony patches

		#region private methods

		[HarmonyTargetMethods]
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return from type in AccessTools.AllTypes()
				where type.Name.AnyOf(
					"CheesePressMachine",
					"KegMachine",
					"LoomMachine",
					"MayonnaiseMachine",
					"OilMakerMachine",
					"PreservesJarMachine")
				select type.MethodNamed("SetInput");
		}

		#endregion private methods
	}
}