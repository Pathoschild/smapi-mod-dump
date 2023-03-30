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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Entities.Enemies
{
    enum DRACULA_PHASE
    {
		GLOATING_PHASE = -1,
		RANDOM_SHOOT = 0,
		SPREAD_SHOOT = 1,
		SUMMON_DEMON = 2,
		SUMMON_MUMMY = 3
    }

	public class Dracula : Enemy
	{
		DRACULA_PHASE phase;

		int phaseInternalTimer;

		int phaseInternalCounter;

		int shootTimer;

		readonly int fullHealth;

		Point homePosition;

		public Dracula(GameMultiplayerPrairieKing game)
			: base(game, MONSTER_TYPE.dracula, new Point(8 * TileSize, 8 * TileSize))
		{
			homePosition = position.Location;
			position.Y += TileSize * 4;
			health = 350;
			fullHealth = health;
			phase = DRACULA_PHASE.GLOATING_PHASE;
			phaseInternalTimer = 4000;
			speed = 2;
		}

		public override void Draw(SpriteBatch b)
		{
			if (phase != DRACULA_PHASE.GLOATING_PHASE)
			{
				b.Draw(Game1.staminaRect, new Rectangle((int)topLeftScreenCoordinate.X, (int)topLeftScreenCoordinate.Y + 16 * TileSize + 3, (int)((16f * TileSize) * ((float)health / (float)fullHealth)), TileSize / 3), new Color(188, 51, 74));
			}
			if (flashColorTimer > 0f)
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(464, 1696, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f);
				return;
			}
			int num = (int)phase;
			if (num == -1 || (uint)(num - 1) <= 2u)
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(592 + phaseInternalTimer / 100 % 3 * 16, 1760, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f);
				if (phase == DRACULA_PHASE.GLOATING_PHASE)
				{
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, (position.Y + TileSize) + (float)Math.Sin(phaseInternalTimer / 1000f) * 3f), new Rectangle(528, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f);
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X - TileSize / 2, position.Y - TileSize * 2), new Rectangle(608, 1728, 32, 32), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f);
				}
			}
			else
			{
				b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(592 + phaseInternalTimer / 100 % 2 * 16, 1712, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f);
			}
		}

		public override POWERUP_TYPE GetLootDrop()
		{
			return POWERUP_TYPE.LOG;
		}

		public override void OnDeath()
		{
			base.OnDeath();
			//Play death sound
			Game1.playSound("cowboy_explosion");

			//Spawn heart on defeat
			if (gameInstance.isHost)
			{
				gameInstance.NETspawnPowerup(POWERUP_TYPE.HEART, new Point(8 * TileSize, 10 * TileSize), 9999999);
			}

			//???
			gameInstance.noPickUpBox = new Rectangle(8 * TileSize, 10 * TileSize, TileSize, TileSize);

			//Stop Boss song
			if (outlawSong != null && outlawSong.IsPlaying)
			{
				outlawSong.Stop(AudioStopOptions.Immediate);
			}

			//Flash Screen
			gameInstance.screenFlash = 200;

			for (int j = 0; j < 30; j++)
			{
				gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(512, 1696, 16, 16), 70f, 6, 0, new Vector2(position.X + Game1.random.Next(-TileSize, TileSize), position.Y + Game1.random.Next(-TileSize, TileSize)) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
				{
					delayBeforeAnimationStart = j * 75
				});
				if (j % 4 == 0)
				{
					gameInstance.AddGuts(new Point(position.X + Game1.random.Next(-TileSize, TileSize), position.Y + Game1.random.Next(-TileSize, TileSize)), MONSTER_TYPE.dracula);
				}
				if (j % 4 == 0)
				{
					gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, new Vector2(position.X + Game1.random.Next(-TileSize, TileSize), position.Y + Game1.random.Next(-TileSize, TileSize)) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
					{
						delayBeforeAnimationStart = j * 75
					});
				}
				if (j % 3 == 0)
				{
					gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(544, 1728, 16, 16), 100f, 4, 0, new Vector2(position.X + Game1.random.Next(-TileSize, TileSize), position.Y + Game1.random.Next(-TileSize, TileSize)) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
					{
						delayBeforeAnimationStart = j * 75
					});
				}
			}
		}

		public override bool TakeDamage(int damage)
		{
			if (phase == DRACULA_PHASE.GLOATING_PHASE)
			{
				return false;
			}
			health -= damage;
			if (health < 0)
			{
				OnDeath();
				return true;
			}
			flashColorTimer = 100f;
			Game1.playSound("cowboy_monsterhit");
			return false;
		}

		public override bool Move(Vector2 playerPosition, GameTime time)
		{
			if (flashColorTimer > 0f)
			{
				flashColorTimer -= time.ElapsedGameTime.Milliseconds;
			}
			phaseInternalTimer -= time.ElapsedGameTime.Milliseconds;
			switch (phase)
			{
				case DRACULA_PHASE.GLOATING_PHASE:
					if (phaseInternalTimer <= 0)
					{
						phaseInternalCounter = 0;
						if (Game1.soundBank != null)
						{
							outlawSong = Game1.soundBank.GetCue("cowboy_boss");
							outlawSong.Play();
						}
						phase = 0;
					}
					break;
				case DRACULA_PHASE.RANDOM_SHOOT:
					{
						if (phaseInternalCounter == 0)
						{
							phaseInternalCounter++;
							phaseInternalTimer = Game1.random.Next(3000, 7000);
						}
						if (phaseInternalTimer < 0)
						{
							phaseInternalCounter = 0;
							phase = (DRACULA_PHASE)Game1.random.Next(1, 4);
							phaseInternalTimer = 9999;
						}
						Vector2 target = playerPosition;
						if (gameInstance.player.deathTimer > 0f)
						{
							break;
						}
						int movementDirection = -1;
						if (Math.Abs(target.X - position.X) > Math.Abs(target.Y - position.Y))
						{
							if (target.X + speed < position.X)
							{
								movementDirection = 3;
							}
							else if (target.X > (position.X + speed))
							{
								movementDirection = 1;
							}
							else if (target.Y > (position.Y + speed))
							{
								movementDirection = 2;
							}
							else if (target.Y + speed < position.Y)
							{
								movementDirection = 0;
							}
						}
						else if (target.Y > (position.Y + speed))
						{
							movementDirection = 2;
						}
						else if (target.Y + speed < position.Y)
						{
							movementDirection = 0;
						}
						else if (target.X + speed < position.X)
						{
							movementDirection = 3;
						}
						else if (target.X > (position.X + speed))
						{
							movementDirection = 1;
						}
						Rectangle attemptedPosition = position;
						switch (movementDirection)
						{
							case 0:
								attemptedPosition.Y -= speed;
								break;
							case 1:
								attemptedPosition.X += speed;
								break;
							case 2:
								attemptedPosition.Y += speed;
								break;
							case 3:
								attemptedPosition.X -= speed;
								break;
						}
						attemptedPosition.X = position.X - (attemptedPosition.X - position.X);
						attemptedPosition.Y = position.Y - (attemptedPosition.Y - position.Y);
						if (!gameInstance.map.IsCollidingWithMapForMonsters(attemptedPosition) && !gameInstance.map.IsCollidingWithMonster(attemptedPosition, this))
						{
							position = attemptedPosition;
						}
						shootTimer -= time.ElapsedGameTime.Milliseconds;
						if (shootTimer < 0)
						{
                            Vector2 trajectory = StardewValley.Utility.getVelocityTowardPoint(new Point(position.X + TileSize / 2, position.Y), playerPosition + new Vector2(TileSize / 2, TileSize / 2), 8f);
							if (gameInstance.player.movementDirections.Count > 0)
							{
								trajectory = StardewValley.Utility.getTranslatedVector2(trajectory, gameInstance.player.movementDirections.Last(), 3f);
							}
							if (gameInstance.isHost) gameInstance.NETspawnBullet(false, new Point(position.X + TileSize / 2, position.Y + TileSize / 2), new Point((int)trajectory.X, (int)trajectory.Y), 1);
							shootTimer = 250;
							Game1.playSound("Cowboy_gunshot");
						}
						break;
					}
				case DRACULA_PHASE.SUMMON_DEMON:
				case DRACULA_PHASE.SUMMON_MUMMY:
					if (phaseInternalCounter == 0)
					{
						Point oldPosition = position.Location;
						if (position.X > homePosition.X + 6)
						{
							position.X -= 6;
						}
						else if (position.X < homePosition.X - 6)
						{
							position.X += 6;
						}
						if (position.Y > homePosition.Y + 6)
						{
							position.Y -= 6;
						}
						else if (position.Y < homePosition.Y - 6)
						{
							position.Y += 6;
						}
						if (position.Location.Equals(oldPosition))
						{
							phaseInternalCounter++;
							phaseInternalTimer = 1500;
						}
					}
					else if (phaseInternalCounter == 1 && phaseInternalTimer < 0)
					{
						SummonEnemies(new Point(position.X + TileSize / 2, position.Y + TileSize / 2), (MONSTER_TYPE)Game1.random.Next(0, 5));
						if (Game1.random.NextDouble() < 0.4)
						{
							phase = 0;
							phaseInternalCounter = 0;
						}
						else
						{
							phaseInternalTimer = 2000;
						}
					}
					break;
				case DRACULA_PHASE.SPREAD_SHOOT:
					if (phaseInternalCounter == 0)
					{
						Point oldPosition2 = position.Location;
						if (position.X > homePosition.X + 6)
						{
							position.X -= 6;
						}
						else if (position.X < homePosition.X - 6)
						{
							position.X += 6;
						}
						if (position.Y > homePosition.Y + 6)
						{
							position.Y -= 6;
						}
						else if (position.Y < homePosition.Y - 6)
						{
							position.Y += 6;
						}
						if (position.Location.Equals(oldPosition2))
						{
							phaseInternalCounter++;
							phaseInternalTimer = 1500;
						}
					}
					else if (phaseInternalCounter == 1)
					{
						if (phaseInternalTimer < 0)
						{
							phaseInternalCounter++;
							phaseInternalTimer = 2000;
							shootTimer = 200;
							FireSpread(new Point(position.X + TileSize / 2, position.Y + TileSize / 2), 0.0);
						}
					}
					else if (phaseInternalCounter == 2)
					{
						shootTimer -= time.ElapsedGameTime.Milliseconds;
						if (shootTimer < 0)
						{
							FireSpread(new Point(position.X + TileSize / 2, position.Y + TileSize / 2), 0.0);
							shootTimer = 200;
						}
						if (phaseInternalTimer < 0)
						{
							phaseInternalCounter++;
							phaseInternalTimer = 500;
						}
					}
					else if (phaseInternalCounter == 3)
					{
						if (phaseInternalTimer < 0)
						{
							phaseInternalTimer = 2000;
							shootTimer = 200;
							phaseInternalCounter++;
                            Vector2 trajectory3 = StardewValley.Utility.getVelocityTowardPoint(new Point(position.X + TileSize / 2, position.Y), playerPosition + new Vector2(TileSize / 2, TileSize / 2), 8f);
							if (gameInstance.isHost) gameInstance.NETspawnBullet(false, new Point(position.X + TileSize / 2, position.Y + TileSize / 2), new Point((int)trajectory3.X, (int)trajectory3.Y), 1);
							Game1.playSound("Cowboy_gunshot");
						}
					}
					else
					{
						if (phaseInternalCounter != 4)
						{
							break;
						}
						shootTimer -= time.ElapsedGameTime.Milliseconds;
						if (shootTimer < 0)
						{
                            Vector2 trajectory2 = StardewValley.Utility.getVelocityTowardPoint(new Point(position.X + TileSize / 2, position.Y), playerPosition + new Vector2(TileSize / 2, TileSize / 2), 8f);
							trajectory2.X += Game1.random.Next(-1, 2);
							trajectory2.Y += Game1.random.Next(-1, 2);
							if (gameInstance.isHost) gameInstance.NETspawnBullet(false, new Point(position.X + TileSize / 2, position.Y + TileSize / 2), new Point((int)trajectory2.X, (int)trajectory2.Y), 1);
							Game1.playSound("Cowboy_gunshot");
							shootTimer = 200;
						}
						if (phaseInternalTimer < 0)
						{
							if (Game1.random.NextDouble() < 0.4)
							{
								phase = 0;
								phaseInternalCounter = 0;
							}
							else
							{
								phaseInternalTimer = 500;
								phaseInternalCounter = 1;
							}
						}
					}
					break;
			}
			return false;
		}

		public void FireSpread(Point origin, double offsetAngle)
		{
            Vector2[] surroundingTileLocationsArray = StardewValley.Utility.getSurroundingTileLocationsArray(new Vector2(origin.X, origin.Y));
			for (int i = 0; i < surroundingTileLocationsArray.Length; i++)
			{
				Vector2 p = surroundingTileLocationsArray[i];
                Vector2 trajectory = StardewValley.Utility.getVelocityTowardPoint(origin, p, 6f);
				if (offsetAngle > 0.0)
				{
					offsetAngle /= 2.0;
					trajectory.X = (float)(Math.Cos(offsetAngle) * (p.X - origin.X) - Math.Sin(offsetAngle) * (p.Y - origin.Y) + origin.X);
					trajectory.Y = (float)(Math.Sin(offsetAngle) * (p.X - origin.X) + Math.Cos(offsetAngle) * (p.Y - origin.Y) + origin.Y);
					trajectory = StardewValley.Utility.getVelocityTowardPoint(origin, trajectory, 8f);
				}
				if (gameInstance.isHost) gameInstance.NETspawnBullet(false, origin, new Point((int)trajectory.X, (int)trajectory.Y), 1);
			}
			Game1.playSound("Cowboy_gunshot");
		}

		public void SummonEnemies(Point origin, MONSTER_TYPE which)
		{
			if (gameInstance.isHost)
			{
				if (!gameInstance.map.IsCollidingWithMonster(new Rectangle(origin.X - TileSize - TileSize / 2, origin.Y, TileSize, TileSize), null))
				{
					gameInstance.monsters.Add(new Enemy(gameInstance, which, new Point(origin.X - TileSize - TileSize / 2, origin.Y)));
				}
				if (!gameInstance.map.IsCollidingWithMonster(new Rectangle(origin.X + TileSize + TileSize / 2, origin.Y, TileSize, TileSize), null))
				{
					gameInstance.monsters.Add(new Enemy(gameInstance, which, new Point(origin.X + TileSize + TileSize / 2, origin.Y)));
				}
				if (!gameInstance.map.IsCollidingWithMonster(new Rectangle(origin.X, origin.Y + TileSize + TileSize / 2, TileSize, TileSize), null))
				{
					gameInstance.monsters.Add(new Enemy(gameInstance, which, new Point(origin.X, origin.Y + TileSize + TileSize / 2)));
				}
				if (!gameInstance.map.IsCollidingWithMonster(new Rectangle(origin.X, origin.Y - TileSize - TileSize * 3 / 4, TileSize, TileSize), null))
				{
					gameInstance.monsters.Add(new Enemy(gameInstance, which, new Point(origin.X, origin.Y - TileSize - TileSize * 3 / 4)));
				}
			}

			gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, topLeftScreenCoordinate + new Vector2(origin.X - TileSize - TileSize / 2, origin.Y), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
			{
				delayBeforeAnimationStart = Game1.random.Next(800)
			});
			gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, topLeftScreenCoordinate + new Vector2(origin.X + TileSize + TileSize / 2, origin.Y), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
			{
				delayBeforeAnimationStart = Game1.random.Next(800)
			});
			gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, topLeftScreenCoordinate + new Vector2(origin.X, origin.Y - TileSize - TileSize * 3 / 4), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
			{
				delayBeforeAnimationStart = Game1.random.Next(800)
			});
			gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, topLeftScreenCoordinate + new Vector2(origin.X, origin.Y + TileSize + TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
			{
				delayBeforeAnimationStart = Game1.random.Next(800)
			});
			Game1.playSound("Cowboy_monsterDie");
		}
	}

}
