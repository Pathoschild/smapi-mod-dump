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
using StardewModdingAPI;
using StardewValley.Tools;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class SlingshotCanAutoFirePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SlingshotCanAutoFirePatch()
		{
			Original = typeof(Slingshot).MethodNamed(nameof(Slingshot.CanAutoFire));
			Prefix = new HarmonyMethod(GetType(), nameof(SlingshotCanAutoFirePrefix));
		}

		#region harmony patches

		/// <summary>Patch to allow auto-fire during Desperado super mode.</summary>
		[HarmonyPrefix]
		private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
		{
			try
			{
				var who = __instance.getLastFarmerToUse();
				if (ModEntry.IsSuperModeActive && ModEntry.SuperModeIndex == Util.Professions.IndexOf("Desperado"))
					__result = true;
				else
					__result = false;
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}