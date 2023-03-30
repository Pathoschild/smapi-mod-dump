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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Entities
{
    public class Enemy
	{
		protected GameMultiplayerPrairieKing gameInstance;

		public long id;

		const double lootChance = 0.05;

		const double coinChance = 0.05;

		public const int MonsterAnimationDelay = 500;

		public int health;

		public MONSTER_TYPE type;

		protected int speed;

		float movementAnimationTimer;

		public Rectangle position;

		int movementDirection;

		bool movedLastTurn;

		bool oppositeMotionGuy;

		public bool invisible;

		bool special;

		bool uninterested;

		public bool flyer;

		public Color tint = Color.White;

		public Color flashColor = Color.Red;

		public float flashColorTimer;

		public int ticksSinceLastMovement;

		public Vector2 acceleration;

		public Point targetPosition;



		public Enemy(GameMultiplayerPrairieKing game, MONSTER_TYPE which, Point position)
		{
			this.gameInstance = game;


			type = which;
			this.position = new Rectangle(position.X, position.Y, TileSize, TileSize);
			switch (type)
			{
				case MONSTER_TYPE.orc:
					speed = 2;
					health = 1;
					uninterested = (Game1.random.NextDouble() < 0.25);
					if (uninterested)
					{
						targetPosition = new Point(Game1.random.Next(2, 14) * TileSize, Game1.random.Next(2, 14) * TileSize);
					}
					break;
				case MONSTER_TYPE.ogre:
					speed = 1;
					health = 3;
					break;
				case MONSTER_TYPE.mushroom:
					speed = 3;
					health = 2;
					break;
				case MONSTER_TYPE.ghost:
					speed = 2;
					health = 1;
					flyer = true;
					break;
				case MONSTER_TYPE.mummy:
					health = 6;
					speed = 1;
					uninterested = (Game1.random.NextDouble() < 0.25);
					if (uninterested)
					{
						targetPosition = new Point(Game1.random.Next(2, 14) * TileSize, Game1.random.Next(2, 14) * TileSize);
					}
					break;
				case MONSTER_TYPE.devil:
					health = 3;
					speed = 3;
					flyer = true;
					break;
				case MONSTER_TYPE.spikey:
					{
						speed = 3;
						health = 2;
						int tries = 0;
						do
						{
							targetPosition = new Point(Game1.random.Next(2, 14) * TileSize, Game1.random.Next(2, 14) * TileSize);
							tries++;
						}
						while (gameInstance.map.IsCollidingWithMap(targetPosition) && tries < 10);

						if(gameInstance.isHost)
                        {
							PK_SpikeyNewTarget message = new();
							message.target = targetPosition;
							message.id = id;
							gameInstance.modInstance.Helper.Multiplayer.SendMessage(message, "PK_SpikeyNewTarget");
						}
						break;
					}
				case MONSTER_TYPE.outlaw:
					health = (gameInstance.world == 0) ? 50 : 100;
					break;
			}
			oppositeMotionGuy = (Game1.random.NextDouble() < 0.5);

			//Add bonus health in newGamePlus
			if (gameInstance.newGamePlus > 0)
			{
				health += gameInstance.newGamePlus * 2;
			}

			//Add a health multiplier for each additional player, depending on difficulty
			if(gameInstance.difficulty == DIFFICULTY.NORMAL)
            {
				float newHealth = health + health * (0.6f * gameInstance.playerList.Count - 1);
                health = (int)Math.Ceiling(newHealth);
			}
			else if(gameInstance.difficulty == DIFFICULTY.HARD)
            {
                float newHealth = health + health * (1f * gameInstance.playerList.Count - 1);
                health = (int)Math.Ceiling(newHealth);
			}
			
			//NET spawn enemy
			if (gameInstance.isHost)
			{
				id = gameInstance.modInstance.Helper.Multiplayer.GetNewID();

                PK_EnemySpawn message = new()
                {
                    id = id,
                    which = (int)which,
                    position = position
                };
                game.modInstance.Helper.Multiplayer.SendMessage(message, "PK_EnemySpawn");
			}
		}

		public virtual void Draw(SpriteBatch b)
		{
			if (type == MONSTER_TYPE.spikey && special)
			{
				if (flashColorTimer > 0f)
				{
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(480, 1696, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				}
				else
				{
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(576, 1712, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				}
			}
			else if (!invisible)
			{
				if (flashColorTimer > 0f)
				{
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(352 + (int)type * 16, 1696, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				}
				else
				{
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + new Vector2(position.X, position.Y), new Rectangle(352 + ((int)type * 2 + ((movementAnimationTimer < 250f) ? 1 : 0)) * 16, 1712, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				}
				if (gameInstance.monsterConfusionTimer > 0)
				{
					b.DrawString(Game1.smallFont, "?", topLeftScreenCoordinate + new Vector2((position.X + TileSize / 2) - Game1.smallFont.MeasureString("?").X / 2f, position.Y - TileSize / 2), new Color(88, 29, 43), 0f, Vector2.Zero, 1f, SpriteEffects.None, position.Y / 10000f);
					b.DrawString(Game1.smallFont, "?", topLeftScreenCoordinate + new Vector2((position.X + TileSize / 2) - Game1.smallFont.MeasureString("?").X / 2f + 1f, position.Y - TileSize / 2), new Color(88, 29, 43), 0f, Vector2.Zero, 1f, SpriteEffects.None, position.Y / 10000f);
					b.DrawString(Game1.smallFont, "?", topLeftScreenCoordinate + new Vector2((position.X + TileSize / 2) - Game1.smallFont.MeasureString("?").X / 2f - 1f, position.Y - TileSize / 2), new Color(88, 29, 43), 0f, Vector2.Zero, 1f, SpriteEffects.None, position.Y / 10000f);
				}
			}
		}

		public virtual bool TakeDamage(int damage)
		{
			health -= damage;
			health = Math.Max(0, health);
			if (health <= 0)
			{
				OnDeath();
				return true;
			}
			Game1.playSound("cowboy_monsterhit");
			flashColor = Color.Red;
			flashColorTimer = 100f;
			return false;
		}

		public virtual POWERUP_TYPE GetLootDrop()
		{
			//Half chances to drop something second playthrough
			if (gameInstance.newGamePlus == 1 && Game1.random.NextDouble() < 0.5)
			{
				return POWERUP_TYPE.LOG;
			}

			//Dont drop anything from transformed spikeys
			if (type == MONSTER_TYPE.spikey && special)
			{
				return POWERUP_TYPE.LOG;
			}

			//Chances
			if (Game1.random.NextDouble() < coinChance)
			{
				if (type != MONSTER_TYPE.orc && Game1.random.NextDouble() < 0.1)
				{
					return POWERUP_TYPE.NICKEL;
				}
				if (Game1.random.NextDouble() < 0.01)
				{
					return POWERUP_TYPE.NICKEL;
				}
				return POWERUP_TYPE.COIN;
			}
			if (Game1.random.NextDouble() < lootChance)
			{
				if (Game1.random.NextDouble() < 0.15)
				{
					POWERUP_TYPE t = (POWERUP_TYPE)Game1.random.Next(6, 8);
					if (gameInstance.newGamePlus > 0 && (t == POWERUP_TYPE.ZOMBIE || t == POWERUP_TYPE.LIFE) && Game1.random.NextDouble() < 0.4)
					{
						t = POWERUP_TYPE.LOG;
					}
					return t;
				}
				if (Game1.random.NextDouble() < 0.07)
				{
					return POWERUP_TYPE.SHERRIFF;
				}

				POWERUP_TYPE loot = (POWERUP_TYPE)Game1.random.Next(2, 10);
				if (loot == POWERUP_TYPE.ZOMBIE && Game1.random.NextDouble() < 0.4)
				{
					loot = (POWERUP_TYPE)Game1.random.Next(2, 10);
				}

				if (gameInstance.newGamePlus > 0 && (loot == POWERUP_TYPE.ZOMBIE || loot == POWERUP_TYPE.LIFE) && Game1.random.NextDouble() < 0.4)
				{
					loot = POWERUP_TYPE.LOG;
				}

				return loot;
			}

			return POWERUP_TYPE.LOG;
		}

		public virtual bool Move(Vector2 playerPosition, GameTime time)
		{
			movementAnimationTimer -= time.ElapsedGameTime.Milliseconds;
			if (movementAnimationTimer <= 0f)
			{
				movementAnimationTimer = Math.Max(100, MonsterAnimationDelay - speed * 50);
			}
			if (flashColorTimer > 0f)
			{
				flashColorTimer -= time.ElapsedGameTime.Milliseconds;
				return false;
			}
			if (gameInstance.monsterConfusionTimer > 0)
			{
				return false;
			}
			ticksSinceLastMovement++;
			switch (type)
			{
				case MONSTER_TYPE.orc:
				case MONSTER_TYPE.ogre:
				case MONSTER_TYPE.mummy:
				case MONSTER_TYPE.mushroom:
				case MONSTER_TYPE.spikey:
					{     
						if (type == MONSTER_TYPE.spikey)
						{
							if (special || invisible)
							{
								break;
							}
							if (ticksSinceLastMovement > 20)
							{
								int tries2 = 0;
								do
								{
									targetPosition = new Point(Game1.random.Next(2, 14) * TileSize, Game1.random.Next(2, 14) * TileSize);
									tries2++;
								}
								while (gameInstance.map.IsCollidingWithMap(targetPosition) && tries2 < 5);

								if(gameInstance.isHost)
                                {
									PK_SpikeyNewTarget message = new();
									message.id = id;
									message.target = targetPosition;
									gameInstance.modInstance.Helper.Multiplayer.SendMessage(message, "PK_SpikeyNewTarget");
								} 
							}
						}
						else if (ticksSinceLastMovement > 20)
						{
							int tries = 0;
							do
							{
								oppositeMotionGuy = !oppositeMotionGuy;
								targetPosition = new Point(Game1.random.Next(position.X - TileSize * 2, position.X + TileSize * 2), Game1.random.Next(position.Y - TileSize * 2, position.Y + TileSize * 2));
								tries++;
							}
							while (gameInstance.map.IsCollidingWithMap(targetPosition) && tries < 5);
						}

						Vector2 target2;
						if (targetPosition.Equals(Point.Zero)) target2 = playerPosition;
						else target2 = new Vector2(targetPosition.X, targetPosition.Y);
		
						if (gameInstance.gopherRunning)
						{
							target2 = new Vector2(gameInstance.gopherBox.X, gameInstance.gopherBox.Y);
						}
						if (Game1.random.NextDouble() < 0.001)
						{
							oppositeMotionGuy = !oppositeMotionGuy;
						}
						if ((type == MONSTER_TYPE.spikey && !oppositeMotionGuy) || Math.Abs(target2.X - position.X) > Math.Abs(target2.Y - position.Y))
						{
							if (target2.X + speed < position.X && (movedLastTurn || movementDirection != 3))
							{
								movementDirection = 3;
							}
							else if (target2.X > (position.X + speed) && (movedLastTurn || movementDirection != 1))
							{
								movementDirection = 1;
							}
							else if (target2.Y > (position.Y + speed) && (movedLastTurn || movementDirection != 2))
							{
								movementDirection = 2;
							}
							else if (target2.Y + speed < position.Y && (movedLastTurn || movementDirection != 0))
							{
								movementDirection = 0;
							}
						}
						else if (target2.Y > (position.Y + speed) && (movedLastTurn || movementDirection != 2))
						{
							movementDirection = 2;
						}
						else if (target2.Y + speed < position.Y && (movedLastTurn || movementDirection != 0))
						{
							movementDirection = 0;
						}
						else if (target2.X + speed < position.X && (movedLastTurn || movementDirection != 3))
						{
							movementDirection = 3;
						}
						else if (target2.X > (position.X + speed) && (movedLastTurn || movementDirection != 1))
						{
							movementDirection = 1;
						}
						movedLastTurn = false;
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
						if (gameInstance.zombieModeTimer > 0)
						{
							attemptedPosition.X = position.X - (attemptedPosition.X - position.X);
							attemptedPosition.Y = position.Y - (attemptedPosition.Y - position.Y);
						}

						//Ogers stomp spikeys
						if (type == MONSTER_TYPE.ogre)
						{
							for (int i = gameInstance.monsters.Count - 1; i >= 0; i--)
							{
								if (gameInstance.monsters[i].type == MONSTER_TYPE.spikey && gameInstance.monsters[i].special && gameInstance.monsters[i].position.Intersects(attemptedPosition))
								{
                                    //Net EnemyKilled
                                    PK_EnemyKilled message = new()
                                    {
                                        id = gameInstance.monsters[i].id
                                    };
                                    gameInstance.modInstance.Helper.Multiplayer.SendMessage(message, "PK_EnemyKilled");

									gameInstance.AddGuts(gameInstance.monsters[i].position.Location, gameInstance.monsters[i].type);
									Game1.playSound("Cowboy_monsterDie");
									gameInstance.monsters.RemoveAt(i);
								}
							}
						}
						if (gameInstance.map.IsCollidingWithMapForMonsters(attemptedPosition) || gameInstance.map.IsCollidingWithMonster(attemptedPosition, this) || gameInstance.player.deathTimer > 0f)
						{
							break;
						}
						ticksSinceLastMovement = 0;
						position = attemptedPosition;
						movedLastTurn = true;
						if (!position.Contains((int)target2.X + TileSize / 2, (int)target2.Y + TileSize / 2))
						{
							break;
						}
						targetPosition = Point.Zero;
						if ((type == MONSTER_TYPE.orc || type == MONSTER_TYPE.mummy) && uninterested)
						{
							targetPosition = new Point(Game1.random.Next(2, 14) * TileSize, Game1.random.Next(2, 14) * TileSize);
							if (Game1.random.NextDouble() < 0.5)
							{
								uninterested = false;
								targetPosition = Point.Zero;
							}
						}
						if (type == MONSTER_TYPE.spikey && !invisible)
						{
							if(gameInstance.isHost)
                            {
								SpikeyStartTransform();
								PK_SpikeyTransform message = new();
								message.id = id;
								gameInstance.modInstance.Helper.Multiplayer.SendMessage(message, "PK_SpikeyTransform");
							}
						}
						break;
					}
				case MONSTER_TYPE.ghost:
				case MONSTER_TYPE.devil:
					{
						if (ticksSinceLastMovement > 20)
						{
							int tries3 = 0;
							do
							{
								oppositeMotionGuy = !oppositeMotionGuy;
								targetPosition = new Point(Game1.random.Next(position.X - TileSize * 2, position.X + TileSize * 2), Game1.random.Next(position.Y - TileSize * 2, position.Y + TileSize * 2));
								tries3++;
							}
							while (gameInstance.map.IsCollidingWithMap(targetPosition) && tries3 < 5);
						}
						_ = targetPosition;
						Vector2 target2 = (!targetPosition.Equals(Point.Zero)) ? new Vector2(targetPosition.X, targetPosition.Y) : playerPosition;
                        Vector2 targetToFly = StardewValley.Utility.getVelocityTowardPoint(position.Location, target2 + new Vector2(TileSize / 2, TileSize / 2), speed);
						float accelerationMultiplyer = (targetToFly.X != 0f && targetToFly.Y != 0f) ? 1.5f : 1f;
						if (targetToFly.X > acceleration.X)
						{
							acceleration.X += 0.1f * accelerationMultiplyer;
						}
						if (targetToFly.X < acceleration.X)
						{
							acceleration.X -= 0.1f * accelerationMultiplyer;
						}
						if (targetToFly.Y > acceleration.Y)
						{
							acceleration.Y += 0.1f * accelerationMultiplyer;
						}
						if (targetToFly.Y < acceleration.Y)
						{
							acceleration.Y -= 0.1f * accelerationMultiplyer;
						}
						if (!gameInstance.map.IsCollidingWithMonster(new Rectangle(position.X + (int)Math.Ceiling(acceleration.X), position.Y + (int)Math.Ceiling(acceleration.Y), TileSize, TileSize), this) && gameInstance.player.deathTimer <= 0f)
						{
							ticksSinceLastMovement = 0;
							position.X += (int)Math.Ceiling(acceleration.X);
							position.Y += (int)Math.Ceiling(acceleration.Y);
							if (position.Contains((int)target2.X + TileSize / 2, (int)target2.Y + TileSize / 2))
							{
								targetPosition = Point.Zero;
							}
						}
						break;
					}
			}
			return false;
		}

		public void SpikeyStartTransform()
        {
			gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(352, 1728, 16, 16), 60f, 3, 0, new Vector2(position.X, position.Y) + topLeftScreenCoordinate, flicker: false, flipped: false, (float)position.Y / 10000f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
			{
				endFunction = (int specialParam) =>
				{
					invisible = false;
					health += 5;
					special = true;
				}
			});
			invisible = true;
		}

		public virtual void OnDeath()
		{
			if(gameInstance.isHost)
            {
				//Spawn Pickup if host
				POWERUP_TYPE loot = GetLootDrop();

				if (loot != POWERUP_TYPE.LOG && gameInstance.currentLevel != 12)
				{
					gameInstance.NETspawnPowerup(loot, position.Location);
				}
			}

		}
	}
}
