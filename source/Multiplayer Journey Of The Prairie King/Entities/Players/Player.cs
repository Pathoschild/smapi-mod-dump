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
using MultiPlayerPrairie;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Entities
{
    public class Player : BasePlayer
	{  
        int shotTimer;
		const int shootingDelay = 300;
        const int powerupDuration = 10000;
        public const float playerSpeed = 3f;

        public Player(GameMultiplayerPrairieKing game) : base(game)
        {
			ammoLevel = 0;
        }

        public bool PickupPowerup(Powerup c)
        {
            switch (c.type)
            {
                case POWERUP_TYPE.HEART:
                    UsePowerup(POWERUP_TYPE.HEART);
                    break;
                case POWERUP_TYPE.SKULL:
                    UsePowerup(POWERUP_TYPE.SKULL);
                    break;
                case POWERUP_TYPE.LOG:
                    UsePowerup(POWERUP_TYPE.LOG);
                    break;
                case POWERUP_TYPE.COIN:
                    UsePowerup(POWERUP_TYPE.COIN);
                    break;
                case POWERUP_TYPE.NICKEL:
                    UsePowerup(POWERUP_TYPE.NICKEL);
                    break;
                case POWERUP_TYPE.LIFE:
                    UsePowerup(POWERUP_TYPE.LIFE);
                    break;
                default:
                    {
                        if (heldItem == null)
                        {
                            heldItem = c;
                            Game1.playSound("cowboy_powerup");
                            break;
                        }
                        Powerup tmp = heldItem;
                        heldItem = c;
                        gameInstance.noPickUpBox.Location = c.position;
                        tmp.position = c.position;
                        gameInstance.powerups.Add(tmp);
                        Game1.playSound("cowboy_powerup");
                        return true;
                    }
            }
            return true;
        }

        public override void UsePowerup(POWERUP_TYPE which)
        {
			base.UsePowerup(which);
            //If not visual only (aka sync call for player 2), send network message
            PK_UsePowerup mUsePowerup = new()
            {
                playerId = gameInstance.modInstance.playerID.Value,
                type = (int)which
            };
            gameInstance.modInstance.SyncMessage(mUsePowerup);


            if (gameInstance.activePowerups.ContainsKey(which))
            {
                gameInstance.activePowerups[which] = powerupDuration + 2000;
                return;
            }

            switch (which)
            {
                case POWERUP_TYPE.SHERRIFF:
                    UsePowerup(POWERUP_TYPE.SHOTGUN);
                    UsePowerup(POWERUP_TYPE.RAPIDFIRE);
                    UsePowerup(POWERUP_TYPE.SPEED);
                    for (int j = 0; j < gameInstance.activePowerups.Count; j++)
                    {
                        gameInstance.activePowerups[gameInstance.activePowerups.ElementAt(j).Key] *= 2;
                    }
                    break;
                case POWERUP_TYPE.TELEPORT:

                    Point teleportSpot = Point.Zero;
                    int tries = 0;
                    while ((Math.Abs(teleportSpot.X - position.X) < 8f || Math.Abs(teleportSpot.Y - position.Y) < 8f || gameInstance.map.IsCollidingWithMap(teleportSpot) || gameInstance.map.IsCollidingWithMonster(new Rectangle(teleportSpot.X, teleportSpot.Y, TileSize, TileSize), gameInstance)) && tries < 10)
                    {
                        teleportSpot = new Point(Game1.random.Next(TileSize, 16 * TileSize - TileSize), Game1.random.Next(TileSize, 16 * TileSize - TileSize));
                        tries++;
                    }

                    gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 120f, 5, 0, position + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true));
                    gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 120f, 5, 0, new Vector2(teleportSpot.X, teleportSpot.Y) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true));
                    gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 120f, 5, 0, new Vector2(teleportSpot.X - TileSize / 2, teleportSpot.Y) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
                    {
                        delayBeforeAnimationStart = 200
                    });
                    gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 120f, 5, 0, new Vector2(teleportSpot.X + TileSize / 2, teleportSpot.Y) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
                    {
                        delayBeforeAnimationStart = 400
                    });

                    gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 120f, 5, 0, new Vector2(teleportSpot.X, teleportSpot.Y - TileSize / 2) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
                    {
                        delayBeforeAnimationStart = 600
                    });
                    gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 120f, 5, 0, new Vector2(teleportSpot.X, teleportSpot.Y + TileSize / 2) + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
                    {
                        delayBeforeAnimationStart = 800
                    });

                    if (tries < 10)
                    {
                        position = new Vector2(teleportSpot.X, teleportSpot.Y);

                        //NET Player Move
                        gameInstance.NETmovePlayer(position);

                        SetInvincible(4000);
                        Game1.playSound("cowboy_powerup");
                    }
                    break;

                case POWERUP_TYPE.NUKE:
                    if (!gameInstance.shootoutLevel)
                    {
                        foreach (Enemy e in gameInstance.monsters)
                        {
                            PK_EnemyKilled mEnemyKilled = new()
                            {
                                id = e.id
                            };
                            gameInstance.modInstance.SyncMessage(mEnemyKilled);
                        }

                        gameInstance.monsters.Clear();
                    }
                    else
                    {
                        foreach (Enemy c in gameInstance.monsters)
                        {
                            c.TakeDamage(30);
                            gameInstance.NETspawnBullet(true, c.position.Center, 2, 1);
                        }
                    }
                    break;

                case POWERUP_TYPE.SPREAD:
                case POWERUP_TYPE.RAPIDFIRE:
                case POWERUP_TYPE.SHOTGUN:
                    Game1.playSound("cowboy_gunload");
                    gameInstance.activePowerups.Add(which, powerupDuration + 2000);
                    break;

                default:
                    gameInstance.activePowerups.Add(which, powerupDuration);
                    Game1.playSound("cowboy_powerup");
                    break;
            }

            if (gameInstance.newGamePlus > 0 && gameInstance.activePowerups.ContainsKey(which))
            {
                gameInstance.activePowerups[which] /= 2;
            }
        }


        public void ProcessInputs(Dictionary<GameKeys, int> _buttonHeldFrames)
        {
			if (_buttonHeldFrames[GameKeys.MoveUp] > 0)
			{
				AddMovementDirection(0);
			}
			else if (movementDirections.Contains(0))
			{
				movementDirections.Remove(0);
			}

			if (_buttonHeldFrames[GameKeys.MoveDown] > 0)
			{
				AddMovementDirection(2);
			}
			else if (movementDirections.Contains(2))
			{
				movementDirections.Remove(2);
			}

			if (_buttonHeldFrames[GameKeys.MoveLeft] > 0)
			{
				AddMovementDirection(3);
			}
			else if (movementDirections.Contains(3))
			{
				movementDirections.Remove(3);
			}

			if (_buttonHeldFrames[GameKeys.MoveRight] > 0)
			{
				AddMovementDirection(1);
			}
			else if (movementDirections.Contains(1))
			{
				movementDirections.Remove(1);
			}

			if (_buttonHeldFrames[GameKeys.ShootUp] > 0)
			{
				AddShootingDirection(0);
			}
			else if (shootingDirections.Contains(0))
			{
				shootingDirections.Remove(0);
			}

			if (_buttonHeldFrames[GameKeys.ShootDown] > 0)
			{
				AddShootingDirection(2);
			}
			else if (shootingDirections.Contains(2))
			{
				shootingDirections.Remove(2);
			}

			if (_buttonHeldFrames[GameKeys.ShootLeft] > 0)
			{
				AddShootingDirection(3);
			}
			else if (shootingDirections.Contains(3))
			{
				shootingDirections.Remove(3);
			}

			if (_buttonHeldFrames[GameKeys.ShootRight] > 0)
			{
				AddShootingDirection(1);
			}
			else if (shootingDirections.Contains(1))
			{
				shootingDirections.Remove(1);
			}
		}

		public override void Tick(GameTime time)
        {
			base.Tick(time);
			//Shotting
			if (shotTimer > 0)
			{
				shotTimer -= time.ElapsedGameTime.Milliseconds;
			}
			if (deathTimer <= 0f && shootingDirections.Count > 0 && shotTimer <= 0 && !IsHoldingItem())
			{
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.SPREAD))
				{
					SpawnBullets(new int[1], position);
					SpawnBullets(new int[1] {1}, position);
					SpawnBullets(new int[1] {2}, position);
					SpawnBullets(new int[1] {3}, position);
					SpawnBullets(new int[2] {0,1}, position);
					SpawnBullets(new int[2] {1,2}, position);
					SpawnBullets(new int[2] {2,3}, position);
					SpawnBullets(new int[2] {3,0}, position);
				}
				else if (shootingDirections.Count == 1 || shootingDirections.Last() == (shootingDirections[shootingDirections.Count - 2] + 2) % 4)
				{
					SpawnBullets(new int[1]
					{
							(shootingDirections.Count == 2 && shootingDirections.Last() == (shootingDirections[shootingDirections.Count - 2] + 2) % 4) ? shootingDirections[1] : shootingDirections[0]
					}, position);
				}
				else
				{
					SpawnBullets(shootingDirections.ToArray(), position);
				}
				Game1.playSound("Cowboy_gunshot");
				shotTimer = shootingDelay;
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.RAPIDFIRE))
				{
					shotTimer /= 4;
				}
				for (int i3 = 0; i3 < fireSpeedLevel; i3++)
				{
					shotTimer = shotTimer * 3 / 4;
				}
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.SHOTGUN))
				{
					shotTimer = shotTimer * 3 / 2;
				}
				shotTimer = Math.Max(shotTimer, 20);
			}

			//Movement
			if (deathTimer <= 0f && movementDirections.Count > 0 && !gameInstance.scrollingMap && gameInstance.zombieModeTimer < 8200)
			{
				int effectiveDirections = movementDirections.Count;
				if (effectiveDirections >= 2 && movementDirections.Last() == (movementDirections.ElementAt(movementDirections.Count - 2) + 2) % 4)
				{
					effectiveDirections = 1;
				}
				float speed = GetMovementSpeed(playerSpeed, effectiveDirections);

				//Run faster if COFFEE
				if (gameInstance.activePowerups.Keys.Contains(POWERUP_TYPE.SPEED))
				{
					speed *= 1.5f;
				}
				//Run faster if the zombie mode is actuve
				if (gameInstance.zombieModeTimer > 0)
				{
					speed *= 1.5f;
				}
				//Run faster for each shoe level
				for (int i = 0; i < runSpeedLevel; i++)
				{
					speed *= 1.25f;
				}

				for (int i = Math.Max(0, movementDirections.Count - 2); i < movementDirections.Count; i++)
				{
					if (i != 0 || movementDirections.Count < 2 || movementDirections.Last() != (movementDirections[movementDirections.Count - 2] + 2) % 4)
					{
						Vector2 newPlayerPosition = position;
						switch (movementDirections[i])
						{
							case 0:
								newPlayerPosition.Y -= speed;
								break;
							case 3:
								newPlayerPosition.X -= speed;
								break;
							case 2:
								newPlayerPosition.Y += speed;
								break;
							case 1:
								newPlayerPosition.X += speed;
								break;
						}
						Rectangle newPlayerBox = new((int)newPlayerPosition.X + TileSize / 4, (int)newPlayerPosition.Y + TileSize / 4, TileSize / 2, TileSize / 2);
						//Stop players from colliding
						if (!gameInstance.map.IsCollidingWithMap(newPlayerBox) && (!gameInstance.merchantBox.Intersects(newPlayerBox) || gameInstance.merchantBox.Intersects(boundingBox)))
						{
							position = newPlayerPosition;
						}
					}
				}

				//Reset the players bounding box
				boundingBox.X = (int)position.X + TileSize / 4;
				boundingBox.Y = (int)position.Y + TileSize / 4;
				boundingBox.Width = TileSize / 2;
				boundingBox.Height = TileSize / 2;

				//????
				motionAnimationTimer += time.ElapsedGameTime.Milliseconds;
				motionAnimationTimer %= 400;

				//Pick up powerups
				for (int i = gameInstance.powerups.Count - 1; i >= 0; i--)
				{
					Powerup powerup = gameInstance.powerups[i];
					Rectangle powerupRect = new(powerup.position.X, gameInstance.powerups[i].position.Y, TileSize, TileSize);

					if (boundingBox.Intersects(powerupRect) && !boundingBox.Intersects(gameInstance.noPickUpBox))
					{
                        //NET Pickup Powerup
                        PK_PowerupPickup message = new()
                        {
                            id = powerup.id,
                            which = (int)powerup.type
                        };
                        gameInstance.modInstance.SyncMessage(message);

						if (heldItem != null)
						{
							UsePowerup(powerup.type);
							gameInstance.powerups.RemoveAt(i);
						}
						else if (PickupPowerup(powerup))
						{
							gameInstance.powerups.RemoveAt(i);
						}
					}
				}

				//What
				if (!boundingBox.Intersects(gameInstance.noPickUpBox))
				{
					gameInstance.noPickUpBox.Location = new Point(0, 0);
				}

				//What again
				if (!gameInstance.shoppingCarpetNoPickup.Intersects(boundingBox))
				{
					gameInstance.shoppingCarpetNoPickup.X = -1000;
				}
			}

			//NET Player Move
			gameInstance.NETmovePlayer(position);

		}

		public void SpawnBullets(int[] directions, Vector2 spawn)
		{
			Point bulletSpawn = new((int)spawn.X + 24, (int)spawn.Y + 24 - 6);
			int speed = (int)GetMovementSpeed(8f, 2);
			if (directions.Length == 1)
			{
				int playerShootingDirection = directions[0];
				switch (playerShootingDirection)
				{
					case 0:
						bulletSpawn.Y -= 22;
						break;
					case 1:
						bulletSpawn.X += 16;
						bulletSpawn.Y -= 6;
						break;
					case 2:
						bulletSpawn.Y += 10;
						break;
					case 3:
						bulletSpawn.X -= 16;
						bulletSpawn.Y -= 6;
						break;
				}

				gameInstance.NETspawnBullet(true, bulletSpawn, playerShootingDirection, bulletDamage);
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.SHOTGUN) || spreadPistol)
				{
					switch (playerShootingDirection)
					{
						case 0:
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(-2, -8), bulletDamage);
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(2, -8), bulletDamage);
							break;
						case 1:
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(8, -2), bulletDamage);
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(8, 2), bulletDamage);
							break;
						case 2:
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(-2, 8), bulletDamage);
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(2, 8), bulletDamage);
							break;
						case 3:
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(-8, -2), bulletDamage);
							gameInstance.NETspawnBullet(true, new Point(bulletSpawn.X, bulletSpawn.Y), new Point(-8, 2), bulletDamage);
							break;
					}
				}
			}
			else if (directions.Contains(0) && directions.Contains(1))
			{
				bulletSpawn.X += TileSize / 2;
				bulletSpawn.Y -= TileSize / 2;
				gameInstance.NETspawnBullet(true, bulletSpawn, new Point(speed, -speed), bulletDamage);
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.SHOTGUN) || spreadPistol)
				{
					int modifier8 = -2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(speed + modifier8, -speed + modifier8), bulletDamage);
					modifier8 = 2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(speed + modifier8, -speed + modifier8), bulletDamage);
				}
			}
			else if (directions.Contains(0) && directions.Contains(3))
			{
				bulletSpawn.X -= TileSize / 2;
				bulletSpawn.Y -= TileSize / 2;
				gameInstance.NETspawnBullet(true, bulletSpawn, new Point(-speed, -speed), bulletDamage);
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.SHOTGUN) || spreadPistol)
				{
					int modifier6 = -2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(-speed - modifier6, -speed + modifier6), bulletDamage);
					modifier6 = 2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(-speed - modifier6, -speed + modifier6), bulletDamage);
				}
			}
			else if (directions.Contains(2) && directions.Contains(1))
			{
				bulletSpawn.X += TileSize / 2;
				bulletSpawn.Y += TileSize / 4;
				gameInstance.NETspawnBullet(true, bulletSpawn, new Point(speed, speed), bulletDamage);
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.SHOTGUN) || spreadPistol)
				{
					int modifier4 = -2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(speed - modifier4, speed + modifier4), bulletDamage);
					modifier4 = 2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(speed - modifier4, speed + modifier4), bulletDamage);
				}
			}
			else if (directions.Contains(2) && directions.Contains(3))
			{
				bulletSpawn.X -= TileSize / 2;
				bulletSpawn.Y += TileSize / 4;

				gameInstance.NETspawnBullet(true, bulletSpawn, new Point(-speed, speed), bulletDamage);
				if (gameInstance.activePowerups.ContainsKey(POWERUP_TYPE.SHOTGUN) || spreadPistol)
				{
					int modifier2 = -2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(-speed + modifier2, speed + modifier2), bulletDamage);
					modifier2 = 2;
					gameInstance.NETspawnBullet(true, bulletSpawn, new Point(-speed + modifier2, speed + modifier2), bulletDamage);
				}
			}
		}


		public override void Die()
        {
			base.Die();
			SetInvincible(5000);
			gameInstance.NETmovePlayer(position);
		}
	}
}
