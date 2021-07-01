/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;

namespace Cropbeasts.Projectiles
{
	public class Sandblast : BeastProjectile
	{
		private static readonly Rectangle[] SourceRects = new Rectangle[]
		{
			new Rectangle (173, 465, 16, 16),
			new Rectangle (189, 465, 16, 16),
			new Rectangle (189, 481, 16, 16),
		};

		private readonly NetRectangle sourceRect = new ();

		public Sandblast ()
		{
			NetFields.AddFields (sourceRect);
		}

		public Sandblast (int damage)
		{
			NetFields.AddFields (sourceRect);

			damageToFarmer.Value = damage;

			currentTileSheetIndex.Value = 135; // sorta looks like sand
			spriteFromObjectSheet.Value = true;
			sourceRect.Value = SourceRects[Game1.random.Next (0, SourceRects.Length)];

			firingSound.Value = "shadowDie";
			collisionSound.Value = "sandyStep";

			ignoreTravelGracePeriod.Value = true;
			maxTravelDistance.Value = 384;
		}

		public override void behaviorOnCollisionWithPlayer
			(GameLocation location, Farmer player)
		{
			if (Game1.random.Next (10) >= player.immunity)
			{
				SandblastDebuff.Duration += (int) (SandblastDebuff.AddIncrement *
					(1f - Math.Pow (travelDistance / maxTravelDistance, 2f)));
				base.behaviorOnCollisionWithPlayer (location, player);
			}
		}

		public override void draw (SpriteBatch b)
		{
			b.Draw (Game1.mouseCursors, Game1.GlobalToLocal (Game1.viewport,
				position + new Vector2 (32f, 32f)), sourceRect.Value, Color.White,
				rotation, new Vector2 (8f, 8f), 4f, SpriteEffects.None,
				(position.Y + 96f) / 10000f);
		}

		protected override void explode (GameLocation _location)
		{ }
	}
}
