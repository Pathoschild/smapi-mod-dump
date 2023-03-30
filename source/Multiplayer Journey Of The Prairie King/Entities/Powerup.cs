/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiPlayerPrairie;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;
using System.Collections.Generic;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Entities
{
    public class Powerup
	{
		public GameMultiplayerPrairieKing gameInstance;

		public long id;

		public POWERUP_TYPE which;

		public Point position;

		int duration;

		public float yOffset;

		public bool queuedForDeletion = false;

		public Powerup(GameMultiplayerPrairieKing gameInstance, POWERUP_TYPE which, Point position, int duration = 7500)
		{
			this.gameInstance = gameInstance;

			this.which = which;
			this.position = position;
			this.duration = duration;

			this.id = gameInstance.modInstance.Helper.Multiplayer.GetNewID();
		}

		public void Tick(GameTime time)
		{
			//Push Powerup away from border tiles
			Rectangle r = new(0, 0, 16, 16);
            HashSet<Vector2> borderTiles = new(StardewValley.Utility.getBorderOfThisRectangle(r));

			Vector2 tile_position = new((position.X + TileSize / 2) / TileSize, (position.Y + TileSize / 2) / TileSize);
			Vector2 corner_7 = new(position.X / TileSize, position.Y / TileSize);
			Vector2 corner_6 = new((position.X + TileSize) / TileSize, position.Y / TileSize);
			Vector2 corner_5 = new(position.X / TileSize, position.Y / TileSize);
			Vector2 corner_4 = new(position.X / TileSize, (position.Y + 64) / TileSize);
			if (borderTiles.Contains(tile_position) || borderTiles.Contains(corner_7) || borderTiles.Contains(corner_6) || borderTiles.Contains(corner_5) || borderTiles.Contains(corner_4))
			{
				Point push_direction = default;
				if (Math.Abs(tile_position.X - 8f) > Math.Abs(tile_position.Y - 8f))
				{
					push_direction.X = Math.Sign(tile_position.X - 8f);
				}
				else
				{
					push_direction.Y = Math.Sign(tile_position.Y - 8f);
				}
				position.X -= push_direction.X;
				position.Y -= push_direction.Y;
			}


			//Despawn powerup after set time
			duration -= time.ElapsedGameTime.Milliseconds;
			if (duration < 0) queuedForDeletion = true;

		}

		public void Draw(SpriteBatch b)
		{
			if (duration > 2000 || duration / 200 % 2 == 0)
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y + yOffset), new Rectangle(272 + (int)which * 16, 1808, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
			}
		}
	}
}
