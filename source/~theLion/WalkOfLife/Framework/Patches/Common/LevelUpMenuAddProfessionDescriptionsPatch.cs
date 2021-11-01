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
using System.Collections.Generic;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuAddProfessionDescriptionsPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed("addProfessionDescriptions");
			Prefix = new(GetType(), nameof(LevelUpMenuAddProfessionDescriptionsPrefix));
		}

		#region harmony patches

		/// <summary>Patch to apply modded profession descriptions.</summary>
		[HarmonyPrefix]
		private static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions, string professionName)
		{
			try
			{
				if (!Util.Professions.IndexByName.Contains(professionName)) return true; // run original logic

				descriptions.Add(ModEntry.ModHelper.Translation.Get(professionName + ".name." +
																	(Game1.player.IsMale ? "male" : "female")));
				descriptions.AddRange(ModEntry.ModHelper.Translation.Get(professionName + ".desc").ToString()
					.Split('\n'));
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}