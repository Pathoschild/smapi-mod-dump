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
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectDayUpdatePatch()
		{
			Original = typeof(SObject).MethodNamed(nameof(SObject.DayUpdate));
			Postfix = new HarmonyMethod(GetType(), nameof(ObjectDayUpdatePostfix));
		}

		#region harmony patches

		/// <summary>Patch to add quality to Ecologist Mushroom Boxes.</summary>
		[HarmonyPostfix]
		private static void ObjectDayUpdatePostfix(SObject __instance)
		{
			try
			{
				if (!__instance.bigCraftable.Value || __instance.ParentSheetIndex != 128 || __instance.heldObject.Value == null || !Game1.MasterPlayer.HasProfession("Ecologist"))
					return;

				__instance.heldObject.Value.Quality = Util.Professions.GetEcologistForageQuality();
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}