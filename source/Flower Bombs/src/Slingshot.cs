/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Projectiles;
using System.Collections.Generic;
using System.Linq;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		internal static void FixProjectile (BasicProjectile projectile)
		{
			// Only handle projectiles that are Flower Bombs.
			if (Helper.Reflection.GetField<NetInt> (projectile,
					"currentTileSheetIndex").GetValue ().Value != EmptyID)
				return;

			// Make an appropriate sound for dirt hitting dirt/grass.
			var collisionSound = Helper.Reflection.GetField<NetString> (projectile,
				"collisionSound").GetValue ();
			if (collisionSound.Value == "dirtyHit")
				return;
			collisionSound.Value = "dirtyHit";

			// Upon hitting, try to place the Flower Bomb nearby.
			Helper.Reflection.GetField<BasicProjectile.onCollisionBehavior> (projectile,
				"collisionBehavior").SetValue (OnProjectileCollision);
		}

		private static void OnProjectileCollision (GameLocation location,
			int xPosition, int yPosition, Character _who)
		{
			// If a tile near the collision point is suitable, place the Flower
			// Bomb on that tile.
			FlowerBomb bomb = new (EmptyData);
			List<Vector2> tiles = Utility.recursiveFindOpenTiles (location,
				new Vector2 (xPosition / 64, yPosition / 64), maxIterations: 2);
			Vector2 tile = tiles.FirstOrDefault ((tile) =>
				IsSuitableTile (location, tile, bomb));
			if (tile != Vector2.Zero)
			{
				bomb.TileLocation = tile;
				location.objects[tile] = bomb;
			}
		}
	}
}
