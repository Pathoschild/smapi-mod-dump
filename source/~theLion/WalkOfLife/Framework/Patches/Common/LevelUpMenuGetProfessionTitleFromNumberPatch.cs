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
using StardewValley.Menus;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuGetProfessionTitleFromNumberPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetProfessionTitleFromNumberPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(nameof(LevelUpMenu.getProfessionTitleFromNumber));
			Prefix = new HarmonyMethod(GetType(), nameof(LevelUpMenuGetProfessionTitleFromNumberPrefix));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession names.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
		{
			try
			{
				if (!Util.Professions.IndexByName.Contains(whichProfession)) return true; // run original logic

				__result = ModEntry.ModHelper.Translation.Get(Util.Professions.NameOf(whichProfession) + ".name." + (Game1.player.IsMale ? "male" : "female"));
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