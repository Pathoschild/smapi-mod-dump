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
using Microsoft.Xna.Framework.Media;
using MultiPlayerPrairie;
using MultiplayerPrairieKing.Components;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using static MultiPlayerPrairie.GameMultiplayerPrairieKing;

namespace MultiplayerPrairieKing.Entities
{
    public class BasePlayer
    {
		protected GameMultiplayerPrairieKing gameInstance;

		public Vector2 position;
		public Rectangle boundingBox;

		public List<int> movementDirections = new();
		public List<int> shootingDirections = new();

		public int motionAnimationTimer;
		public float deathTimer;

		protected float footstepSoundTimer = 200f;
		protected int invincibleTimer;

		protected int holdItemTimer;
		protected ITEM_TYPE itemToHold = ITEM_TYPE.NONE;

		public static Texture2D texture;
		public Vector2 textureBase;

		public BasePlayer(GameMultiplayerPrairieKing gameInstance)
		{
			this.gameInstance = gameInstance;
		}

        public virtual void UsePowerup(POWERUP_TYPE which)
        {
            switch (which)
            {
                case POWERUP_TYPE.HEART:
                    foreach (BasePlayer p in gameInstance.playerList.Values)
                    {
                        p.HoldItem(ITEM_TYPE.FINISHED_GAME, 4000);
                    }

                    Game1.playSound("Cowboy_Secret");
                    gameInstance.endCutscene = true;
                    gameInstance.cutscene.endCutsceneTimer = 4000;
                    gameInstance.world = 0;
                    if (!Game1.player.hasOrWillReceiveMail("Beat_PK"))
                    {
                        Game1.addMailForTomorrow("Beat_PK");
                    }
                    break;

                case POWERUP_TYPE.SKULL:
                    gameInstance.StartGopherTrain(ITEM_TYPE.SKULL);
                    break;

                case POWERUP_TYPE.LOG:
                    gameInstance.StartGopherTrain(ITEM_TYPE.LOG);
                    break;

                case POWERUP_TYPE.ZOMBIE:
                    if (overworldSong != null && overworldSong.IsPlaying)
                    {
                        overworldSong.Stop(AudioStopOptions.Immediate);
                    }
                    if (zombieSong != null && zombieSong.IsPlaying)
                    {
                        zombieSong.Stop(AudioStopOptions.Immediate);
                        zombieSong = null;
                    }
                    zombieSong = Game1.soundBank.GetCue("Cowboy_undead");
                    zombieSong.Play();
                    gameInstance.motionPause = 1800;
                    gameInstance.zombieModeTimer = 10000;
                    break;

                case POWERUP_TYPE.TELEPORT:
                    gameInstance.monsterConfusionTimer = 4000;
                    gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 120f, 5, 0, position + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true));
                    break;

                case POWERUP_TYPE.LIFE:
                    gameInstance.lives++;
                    Game1.playSound("cowboy_powerup");
                    break;

                case POWERUP_TYPE.NUKE:
                    Game1.playSound("cowboy_explosion");
                    //Spawn the little explosion things yk yes
                    for (int i = 0; i < 30; i++)
                    {
                        gameInstance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(464, 1792, 16, 16), 80f, 5, 0, new Vector2(Game1.random.Next(1, 16), Game1.random.Next(1, 16)) * TileSize + topLeftScreenCoordinate + new Vector2(TileSize / 2, TileSize / 2), flicker: false, flipped: false, 1f, 0f, Color.White, 3f, 0f, 0f, 0f, local: true)
                        {
                            delayBeforeAnimationStart = Game1.random.Next(800)
                        });
                    }

                    if (!gameInstance.shootoutLevel)
                    {
                        foreach (Enemy e in gameInstance.monsters)
                        {
                            gameInstance.AddGuts(e.position.Location, e.type);
                        }
                    }
                    break;

                case POWERUP_TYPE.COIN:
                    gameInstance.Coins++;
                    Game1.playSound("Pickup_Coin15");
                    break;

                case POWERUP_TYPE.NICKEL:
                    gameInstance.Coins += 5;
                    Game1.playSound("Pickup_Coin15");
                    Game1.playSound("Pickup_Coin15");
                    break;

                default:
                    break;
            }
        }


        protected void AddMovementDirection(int direction)
		{
			if (!gameInstance.gopherTrain && !movementDirections.Contains(direction))
			{
				if (movementDirections.Count == 1)
				{
					_ = (movementDirections.ElementAt(0) + 2) % 4;
				}
				movementDirections.Add(direction);
			}
		}

		protected void AddShootingDirection(int direction)
		{
			if (!shootingDirections.Contains(direction))
			{
				shootingDirections.Add(direction);
			}
		}

		public bool IsHoldingItem()
		{
			return holdItemTimer > 0;
		}

		public ITEM_TYPE GetHeldItem()
		{
			return itemToHold;
		}

		public void HoldItem(ITEM_TYPE item, int duration = 4000)
		{
			if (item != ITEM_TYPE.NONE)
			{
				itemToHold = item;
			}
			holdItemTimer = duration;
		}

		public void SetInvincible(int duration)
		{
			invincibleTimer = duration;
		}

		public bool IsInvincible()
		{
			return invincibleTimer > 0;
		}

		public virtual void Tick(GameTime time)
        {
			if (invincibleTimer > 0)
			{
				invincibleTimer -= time.ElapsedGameTime.Milliseconds;
			}
			if (deathTimer > 0)
			{
				deathTimer -= time.ElapsedGameTime.Milliseconds;
			}
			if (holdItemTimer > 0)
			{
				holdItemTimer -= time.ElapsedGameTime.Milliseconds;
			}

			if (movementDirections.Count > 0)
			{
				footstepSoundTimer -= time.ElapsedGameTime.Milliseconds;
				if (footstepSoundTimer <= 0)
				{
					Game1.playSound("Cowboy_Footstep");
					footstepSoundTimer = 200;
				}
			}
		}

		public void Draw(SpriteBatch b)
		{
			if (deathTimer <= 0f && (invincibleTimer <= 0 || invincibleTimer / 100 % 2 == 0))
			{
				if (holdItemTimer > 0)
				{
					b.Draw(texture, topLeftScreenCoordinate + position + new Vector2(0f, -TileSize / 4), new Rectangle((int)textureBase.X + 48, (int)textureBase.Y + 16, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
					b.Draw(Game1.mouseCursors, topLeftScreenCoordinate + position + new Vector2(0f, -TileSize * 2 / 3) + new Vector2(0f, -TileSize / 4), new Rectangle(320 + (int)itemToHold * 16, 1776, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.002f);
				}
				else if (gameInstance.zombieModeTimer > 0)
				{
					b.Draw(texture, topLeftScreenCoordinate + position + new Vector2(0f, -TileSize / 4), new Rectangle((int)textureBase.X + ((gameInstance.zombieModeTimer / 50 % 2 == 0) ? 16 : 0), (int)textureBase.Y + 32, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				}
				else if (movementDirections.Count == 0 && shootingDirections.Count == 0)
				{
					b.Draw(texture, topLeftScreenCoordinate + position + new Vector2(0f, -TileSize / 4), new Rectangle((int)textureBase.X + 32, (int)textureBase.Y + 16, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f);
				}
				else
				{
					int facingDirection = (shootingDirections.Count == 0) ? movementDirections.ElementAt(0) : shootingDirections.Last();
					b.Draw(texture, topLeftScreenCoordinate + position + new Vector2(0f, -TileSize / 4) + new Vector2(4f, 13f) * 3f, new Rectangle((int)textureBase.X + 19, (int)textureBase.Y + 16 + motionAnimationTimer / 100 * 3, 10, 3), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.001f + 0.001f);
					b.Draw(texture, topLeftScreenCoordinate + position + new Vector2(3f, -TileSize / 4), new Rectangle((int)textureBase.X + facingDirection * 16, (int)textureBase.Y, 16, 16), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, position.Y / 10000f + 0.002f + 0.001f);
				}
			}
		}

        protected float GetMovementSpeed(float speed, int directions)
        {
            float movementSpeed = speed;
            if (directions > 1)
            {
                movementSpeed = Math.Max(1, (int)Math.Sqrt(2f * (movementSpeed * movementSpeed)) / 2);
            }
            return movementSpeed;
        }


        public virtual void Die()
        {
			deathTimer = 3000f;

			//Reset the player position (Different depending on boss or non boss level)
			if (gameInstance.shootoutLevel)
			{
				position = new Vector2(8 * TileSize, 3 * TileSize);
				Game1.playSound("Cowboy_monsterDie");
			}
			else
			{
				position = new Vector2(8 * TileSize - TileSize, 8 * TileSize);

				boundingBox.X = (int)position.X + TileSize / 4;
				boundingBox.Y = (int)position.Y + TileSize / 4;
				boundingBox.Width = TileSize / 2;
				boundingBox.Height = TileSize / 2;

				Game1.playSound("cowboy_dead");
			}
		}
    }
}
