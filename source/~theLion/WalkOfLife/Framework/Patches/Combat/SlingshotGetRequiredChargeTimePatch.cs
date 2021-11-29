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
using JetBrains.Annotations;
using StardewValley.Tools;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class SlingshotGetRequiredChargeTimePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SlingshotGetRequiredChargeTimePatch()
		{
			Original = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
			Postfix = new(GetType(), nameof(SlingshotGetRequiredChargeTimePostfix));
		}

		#region harmony patches

		/// <summary>Patch to reduce slingshot charge time for Desperado.</summary>
		[HarmonyPostfix]
		private static void SlingshotGetRequiredChargeTimePostfix(ref float __result)
		{
			if (ModState.SuperModeIndex != Utility.Professions.IndexOf("Desperado")) return;
			__result *= Utility.Professions.GetCooldownOrChargeTimeReduction();
		}

		#endregion harmony patches
	}
}