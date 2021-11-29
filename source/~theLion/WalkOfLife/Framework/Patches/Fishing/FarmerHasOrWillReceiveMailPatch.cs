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
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class FarmerHasOrWillReceiveMailPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal FarmerHasOrWillReceiveMailPatch()
		{
			Original = RequireMethod<Farmer>(nameof(Farmer.hasOrWillReceiveMail));
			Prefix = new(GetType(), nameof(FarmerHasOrWillReceiveMailPrefix));
		}

		#region harmony patches

		/// <summary>Patch to allow receiving multiple letters from the FRS and the SWA.</summary>
		[HarmonyPrefix]
		private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
		{
			try
			{
				if (id != $"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice")
					return true; // run original logic

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