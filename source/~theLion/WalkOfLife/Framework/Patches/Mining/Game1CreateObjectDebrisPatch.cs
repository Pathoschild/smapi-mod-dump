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
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class Game1CreateObjectDebrisPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal Game1CreateObjectDebrisPatch()
		{
			Original = typeof(Game1).MethodNamed(nameof(Game1.createObjectDebris),
				new[] {typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation)});
			Prefix = new(GetType(), nameof(Game1CreateObjectDebrisPrefix));
		}

		#region harmony patches

		/// <summary>Patch for Gemologist mineral quality and increment counter for mined minerals.</summary>
		[HarmonyPrefix]
		private static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer,
			GameLocation location)
		{
			try
			{
				var who = Game1.getFarmer(whichPlayer);
				if (!who.HasProfession("Gemologist") || !new SObject(objectIndex, 1).IsGemOrMineral())
					return true; // run original logic

				location.debris.Add(new(objectIndex, new(xTile * 64 + 32, yTile * 64 + 32),
					who.getStandingPosition())
				{
					itemQuality = Util.Professions.GetGemologistMineralQuality()
				});

				ModEntry.Data.IncrementField<uint>("MineralsCollected");
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