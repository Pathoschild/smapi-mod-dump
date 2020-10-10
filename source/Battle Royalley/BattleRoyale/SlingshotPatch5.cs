/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
	class SlingshotPatch5 : Patch
	{
		public static Rectangle GetFarmerBounds(Farmer f)
		{
			var bounds = f.GetBoundingBox();
			int s = 32 * 2;
			bounds.Y -= s;
			bounds.Height += s;
			return bounds;
		}

		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Projectile), "behaviorOnCollision");

		public static bool Prefix()
		{
			return false;
		}

		public static bool Postfix(bool __result, Projectile __instance, GameLocation location)
		{
			bool damagesMonsters = ModEntry.BRGame.ModHelper.Reflection.GetField<NetBool>(__instance, "damagesMonsters").GetValue().Value;

			bool whatToReturn = false;

			foreach (Farmer farmer in location.farmers)
			{
				if ((true || !damagesMonsters) && GetFarmerBounds(farmer).Intersects(__instance.getBoundingBox()))
				{
					__instance.behaviorOnCollisionWithPlayer(location, farmer);
					return whatToReturn;
				}
			}

			foreach (Vector2 item in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(__instance.getBoundingBox()))
			{
				//__result = true;
				whatToReturn = true;

				if (location.terrainFeatures.ContainsKey(item) && !((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)location.terrainFeatures)[item].isPassable(null))
				{
					__instance.behaviorOnCollisionWithTerrainFeature(((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>)location.terrainFeatures)[item], item, location);
					return whatToReturn;
				}
			}
			__instance.behaviorOnCollisionWithOther(location);
			return whatToReturn;
		}
	}
}
