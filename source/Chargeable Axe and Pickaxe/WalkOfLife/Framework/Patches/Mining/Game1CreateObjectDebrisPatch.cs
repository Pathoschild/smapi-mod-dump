/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class Game1CreateObjectDebrisPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal Game1CreateObjectDebrisPatch() { }

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new Type[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) }),
				prefix: new HarmonyMethod(GetType(), nameof(Game1CreateObjectDebrisPrefix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Gemologist mineral quality.</summary>
		protected static bool Game1CreateObjectDebrisPrefix(int objectIndex, int xTile, int yTile, long whichPlayer, GameLocation location)
		{
			Farmer who = Game1.getFarmer(whichPlayer);
			if (Utility.SpecificPlayerHasProfession("gemologist", who) && Utility.IsMineral(objectIndex))
			{
				location.debris.Add(new Debris(objectIndex, new Vector2(xTile * 64 + 32, yTile * 64 + 32), who.getStandingPosition())
				{
					itemQuality = Utility.GetGemologistMineralQuality()
				});

				++Data.MineralsCollected;
				return false; // don't run original logic
			}

			return true; // run original logic
		}
		#endregion harmony patches
	}
}
