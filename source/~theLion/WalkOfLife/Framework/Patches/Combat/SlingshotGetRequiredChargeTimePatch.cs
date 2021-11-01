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
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Tools;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class SlingshotGetRequiredChargeTimePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SlingshotGetRequiredChargeTimePatch()
		{
			Original = typeof(Slingshot).MethodNamed(nameof(Slingshot.GetRequiredChargeTime));
			Postfix = new(GetType(), nameof(SlingshotGetRequiredChargeTimePostfix));
		}

		#region harmony patches

		/// <summary>Patch to reduce slingshot charge time for Desperado.</summary>
		[HarmonyPrefix]
		private static void SlingshotGetRequiredChargeTimePostfix(ref float __result)
		{
			try
			{
				if (ModEntry.SuperModeIndex != Util.Professions.IndexOf("Desperado")) return;

				__result *= Util.Professions.GetCooldownOrChargeTimeReduction();
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}