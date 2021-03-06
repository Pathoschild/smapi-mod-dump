/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Harmony; // el diavolo
using System;

namespace LoveOfCooking.Core.HarmonyPatches
{
	public static class HarmonyPatches
	{
		public static string Id => ModEntry.Instance.Helper.ModRegistry.ModID;

		public static void Patch()
		{
			var harmony = HarmonyInstance.Create(Id);
			try
			{
				BushPatches.Patch(harmony);
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
			try
			{
				CommunityCentrePatches.Patch(harmony);
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
			try
			{
				CraftingPagePatches.Patch(harmony);
			}
			catch (Exception ex)
			{
				Log.E("" + ex);
			}
		}
	}
}
