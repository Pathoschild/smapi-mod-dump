/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GeodeCrusherMachineSetInputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GeodeCrusherMachineSetInputPatch()
		{
			Original = AccessTools.Method(
				"Pathoschild.Stardew.Automate.Framework.Machines.Objects.GeodeCrusherMachine:SetInput");
			Postfix = new(GetType(), nameof(GeodeCrusherMachineSetInputPostfix));
		}

		#region harmony patches

		/// <summary>Patch to apply Gemologist effects to automated Geode Crusher.</summary>
		[HarmonyPostfix]
		private static void GeodeCrusherMachineSetInputPostfix(object __instance)
		{
			if (__instance is null) return;

			var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
			if (machine?.heldObject.Value is null) return;

			var who = Game1.getFarmer(machine.owner.Value);
			if (!who.HasProfession("Gemologist") || !machine.heldObject.Value.IsForagedMineral() &&
				!machine.heldObject.Value.IsGemOrMineral()) return;

			machine.heldObject.Value.Quality = Util.Professions.GetGemologistMineralQuality();
			if (who.IsLocalPlayer) ModEntry.Data.IncrementField<uint>("MineralsCollected");
		}

		#endregion harmony patches
	}
}