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
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class BobberBarCtorPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal BobberBarCtorPatch()
		{
			Original = typeof(BobberBar).Constructor(new[] { typeof(int), typeof(float), typeof(bool), typeof(int) });
			Postfix = new HarmonyMethod(GetType(), nameof(BobberBarCtorPostfix));
		}

		#region harmony patches

		/// <summary>Patch for Aquarist bonus bobber height.</summary>
		[HarmonyPostfix]
		private static void BobberBarCtorPostfix(ref int ___bobberBarHeight, ref float ___bobberBarPos)
		{
			int bonusBobberHeight = 0;
			try
			{
				if (Game1.player.HasProfession("Aquarist"))
					bonusBobberHeight = Util.Professions.GetAquaristBonusBobberBarHeight();
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return;
			}

			___bobberBarHeight += bonusBobberHeight;
			___bobberBarPos -= bonusBobberHeight;
		}

		#endregion harmony patches
	}
}