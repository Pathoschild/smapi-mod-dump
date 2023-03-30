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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MultiPlayerPrairie;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Entities.Enemies
{
    enum OUTLAW_PHASE
	{
		TALKING = -1,
		HIDING = 0,
		DARTOUTANDSHOOT = 1,
		RUNANDGUN = 2,
		RUNGUNANDPANT = 3,
		SHOOTING = 4
	}
	public class Outlaw : Enemy
	{

		OUTLAW_PHASE phase;

		int phaseCountdown;

		int shootTimer;

		int phaseInternalTimer;

		int phaseInternalCounter;

		bool dartLeft;

		readonly int fullHealth;

		Point homePosition;

		public Outlaw(GameMultiplayerPrairieKing game, Point position)
			: base(game, MONSTER_TYPE.outlaw, position)
		{
			homePosition = position;
			fullHealth = health;
			phaseCountdown = 4000;
			phase = OUTLAW_PHASE.TALKING;
		}

		public override void Draw(SpriteBatch b)
		{
			b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y + 16 * TileSize + 3, (int)(16f * TileSize * ((float)health / (float)fullHealth)), TileSize / 3), new Color(188, 51, 74));
			if (flashColorTimer > 0f)
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(496, 1696, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				return;
			}
			int num = (int)phase;
			if ((uint)(num - -1) <= 1u)
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(560 + ((phaseCountdown / 250 % 2 == 0) ? 16 : 0), 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				if (phase == OUTLAW_PHASE.TALKING && phaseCountdown > 1000)
				{
					b.Draw(
						Game1.mouseCursors,
						topLeftScreenCoordinate + new Vector2(position.X - TileSize / 2, position.Y - TileSize * 2),
						new Rectangle(576 + ((gameInstance.currentLevel > 5) ? 32 : 0), 1792, 32, 32),
						Color.White,
						0f,
						Vector2.Zero,
						3f,
						SpriteEffects.None,
						position.Y / 10000f + 0.001f
					);
				}
			}
			else if (phase == OUTLAW_PHASE.RUNGUNANDPANT && phaseInternalCounter == 2)
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(560 + ((phaseCountdown / 250 % 2 == 0) ? 16 : 0), 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
			}
			else
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(592 + ((phaseCountdown / 80 % 2 == 0) ? 16 : 0), 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
			}
		}

		public override bool Move(Vector2 playerPosition, GameTime time)
		{
			if (flashColorTimer > 0f)
			{
				flashColorTimer -= time.ElapsedGameTime.Milliseconds;
			}
			phaseCountdown -= time.ElapsedGameTime.Milliseconds;
			if (position.X > 17 * TileSize || position.X < -TileSize)
			{
				position.X = 16 * TileSize / 2;
			}
			switch (phase)
			{
				case OUTLAW_PHASE.TALKING:
				case OUTLAW_PHASE.HIDING:
					if (phaseCountdown >= 0)
					{
						break;
					}
					phase = (OUTLAW_PHASE)Game1.random.Next(1, 5);
					dartLeft = (playerPosition.X < position.X);
					if (playerPosition.X > (7 * TileSize) && playerPosition.X < (9 * TileSize))
					{
						if (Game1.random.NextDouble() < 0.66 || phase == OUTLAW_PHASE.RUNANDGUN)
						{
							phase = OUTLAW_PHASE.SHOOTING;
						}
					}
					else if (phase == OUTLAW_PHASE.SHOOTING)
					{
						phase = OUTLAW_PHASE.RUNGUNANDPANT;
					}
					phaseInternalCounter = 0;
					phaseInternalTimer = 0;
					break;
				case OUTLAW_PHASE.SHOOTING:
					{
						int motion4 = dartLeft ? (-3) : 3;
						if (phaseInternalCounter == 0 && (playerPosition.X <= (7 * TileSize) || playerPosition.X >= (9 * TileSize)))
						{
							phaseInternalCounter = 1;
							phaseInternalTimer = Game1.random.Next(500, 1500);
							break;
						}
						if (Math.Abs(position.Location.X - homePosition.X + TileSize / 2) < TileSize * 7 + 12 && phaseInternalCounter == 0)
						{
							position.X += motion4;
							break;
						}
						if (phaseInternalCounter == 2)
						{
							motion4 = (dartLeft ? (-4) : 4);
							position.X -= motion4;
							if (Math.Abs(position.X - homePosition.X) < 4)
							{
								position.X = homePosition.X;
								phase = 0;
								phaseCountdown = Game1.random.Next(1000, 2000);
							}
							break;
						}
						if (phaseInternalCounter == 0)
						{
							phaseInternalCounter++;
							phaseInternalTimer = Game1.random.Next(1000, 2000);
						}
						phaseInternalTimer -= time.ElapsedGameTime.Milliseconds;
						shootTimer -= time.ElapsedGameTime.Milliseconds;
						if (shootTimer < 0)
						{
                            Vector2 trajectory = StardewValley.Utility.getVelocityTowardPoint(new Point(position.X + TileSize / 2, position.Y), playerPosition + new Vector2(TileSize / 2, TileSize / 2), 8f);
							if (gameInstance.isHost) gameInstance.NETspawnBullet(false, new Point(position.X + TileSize / 2, position.Y - TileSize / 2), new Point((int)trajectory.X, (int)trajectory.Y), 1);
							shootTimer = 120;
							Game1.playSound("Cowboy_gunshot");
						}
						if (phaseInternalTimer <= 0)
						{
							phaseInternalCounter++;
						}
						break;
					}
				case OUTLAW_PHASE.DARTOUTANDSHOOT:
					{
						int motion4 = dartLeft ? (-3) : 3;
						if (Math.Abs(position.Location.X - homePosition.X + TileSize / 2) < TileSize * 2 + 12 && phaseInternalCounter == 0)
						{
							position.X += motion4;
							if (position.X > 256)
							{
								phaseInternalCounter = 2;
							}
							break;
						}
						if (phaseInternalCounter == 2)
						{
							position.X -= motion4;
							if (Math.Abs(position.X - homePosition.X) < 4)
							{
								position.X = homePosition.X;
								phase = 0;
								phaseCountdown = Game1.random.Next(1000, 2000);
							}
							break;
						}
						if (phaseInternalCounter == 0)
						{
							phaseInternalCounter++;
							phaseInternalTimer = Game1.random.Next(1000, 2000);
						}
						phaseInternalTimer -= time.ElapsedGameTime.Milliseconds;
						shootTimer -= time.ElapsedGameTime.Milliseconds;
						if (shootTimer < 0)
						{
							if (gameInstance.isHost) gameInstance.NETspawnBullet(false, new Point(position.X + TileSize / 2, position.Y - TileSize / 2), new Point(Game1.random.Next(-2, 3), -8), 1);
							shootTimer = 150;
							Game1.playSound("Cowboy_gunshot");
						}
						if (phaseInternalTimer <= 0)
						{
							phaseInternalCounter++;
						}
						break;
					}
				case OUTLAW_PHASE.RUNANDGUN:
					if (phaseInternalCounter == 2)
					{
						if (position.X < homePosition.X)
						{
							position.X += 4;
						}
						else
						{
							position.X -= 4;
						}
						if (Math.Abs(position.X - homePosition.X) < 5)
						{
							position.X = homePosition.X;
							phase = 0;
							phaseCountdown = Game1.random.Next(1000, 2000);
						}
						return false;
					}
					if (phaseInternalCounter == 0)
					{
						phaseInternalCounter++;
						phaseInternalTimer = Game1.random.Next(4000, 7000);
					}
					phaseInternalTimer -= time.ElapsedGameTime.Milliseconds;
					if (position.X > playerPosition.X && position.X - playerPosition.X > 3f)
					{
						position.X -= 2;
					}
					else if (position.X < playerPosition.X && playerPosition.X - position.X > 3f)
					{
						position.X += 2;
					}
					shootTimer -= time.ElapsedGameTime.Milliseconds;
					if (shootTimer < 0)
					{
						if (gameInstance.isHost) gameInstance.NETspawnBullet(false, new Point(position.X + TileSize / 2, position.Y - TileSize / 2), new Point(Game1.random.Next(-1, 2), -8), 1);
						shootTimer = 250;
						if (fullHealth > 50)
						{
							shootTimer -= 50;
						}
						if (Game1.random.NextDouble() < 0.2)
						{
							shootTimer = 150;
						}
						Game1.playSound("Cowboy_gunshot");
					}
					if (phaseInternalTimer <= 0)
					{
						phaseInternalCounter++;
					}
					break;
				case OUTLAW_PHASE.RUNGUNANDPANT:
					{
						if (phaseInternalCounter == 0)
						{
							phaseInternalCounter++;
							phaseInternalTimer = Game1.random.Next(3000, 6500);
							break;
						}
						if (phaseInternalCounter == 2)
						{
							phaseInternalTimer -= time.ElapsedGameTime.Milliseconds;
							if (phaseInternalTimer <= 0)
							{
								phaseInternalCounter++;
							}
							break;
						}
						if (phaseInternalCounter == 3)
						{
							if (position.X < homePosition.X)
							{
								position.X += 4;
							}
							else
							{
								position.X -= 4;
							}
							if (Math.Abs(position.X - homePosition.X) < 5)
							{
								position.X = homePosition.X;
								phase = 0;
								phaseCountdown = Game1.random.Next(1000, 2000);
							}
							break;
						}
						int motion4 = dartLeft ? (-3) : 3;
						position.X += motion4;
						if (position.X < TileSize || position.X > 15 * TileSize)
						{
							dartLeft = !dartLeft;
						}
						shootTimer -= time.ElapsedGameTime.Milliseconds;
						if (shootTimer < 0)
						{
							if (gameInstance.isHost) gameInstance.NETspawnBullet(false, new Point(position.X + TileSize / 2, position.Y - TileSize / 2), new Point(Game1.random.Next(-1, 2), -8), 1);
							shootTimer = 250;
							if (fullHealth > 50)
							{
								shootTimer -= 50;
							}
							if (Game1.random.NextDouble() < 0.2)
							{
								shootTimer = 150;
							}
							Game1.playSound("Cowboy_gunshot");
						}
						phaseInternalTimer -= time.ElapsedGameTime.Milliseconds;
						if (phaseInternalTimer <= 0)
						{
							if (phase == OUTLAW_PHASE.RUNANDGUN)
							{
								phaseInternalCounter = 3;
								break;
							}
							phaseInternalTimer = 3000;
							phaseInternalCounter++;
						}
						break;
					}
			}
			return false;
		}

		public override POWERUP_TYPE GetLootDrop()
		{
			return POWERUP_TYPE.LIFE;
		}

		public override void OnDeath()
		{
			base.OnDeath();

			if (gameInstance.isHost)
			{
				gameInstance.NETspawnPowerup((gameInstance.world == 0) ? POWERUP_TYPE.LOG : POWERUP_TYPE.SKULL, new Point(8 * TileSize, 10 * TileSize), 9999999);
			}

			if (outlawSong != null && outlawSong.IsPlaying)
			{
				outlawSong.Stop(AudioStopOptions.Immediate);
			}
			gameInstance.map[8, 8] = MAP_TILE.BRIDGE;
			gameInstance.screenFlash = 200;
			for (int i = 0; i < 15; i++)
			{
				gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, new Vector2(position.X + Game1.random.Next(-TileSize, TileSize), position.Y + Game1.random.Next(-TileSize, TileSize)) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
				{
					delayBeforeAnimationStart = i * 75
				});
			}
		}

		public override bool TakeDamage(int damage)
		{
			if (Math.Abs(position.X - homePosition.X) < 5)
			{
				return false;
			}
			health -= damage;
			if (health < 0)
			{
				OnDeath();
				return true;
			}
			flashColorTimer = 150f;
			Game1.playSound("cowboy_monsterhit");
			return false;
		}
	}
}
