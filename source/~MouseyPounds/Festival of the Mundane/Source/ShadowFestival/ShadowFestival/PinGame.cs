using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Minigames;
using SVObject = StardewValley.Object;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShadowFestival
{
    public class PinGame : IMinigame
    {
        private float PlayerPosition = 0.5f;
        private int CurrentStage = -1;
        private int TotalStages = 6;
        private double GameLengthSeconds = 20.0;
        private double StartTime = 0;
        private double LastUpdate = 0;
        private string CurrentDist = "";
        private int CurrentDirection = 0;

		public float nextHint = 0.0F;

		protected int _lastCurrentDirection = 0;
		public float nextRiffRaff = 0.0F;

		public float currentStepTimer = 0.0F;

		public float gameStartTimer;
		public int gameStartCount;

		protected List<Callout> _callouts;

		protected float _gameEndTimer = 0.0F;
		protected bool _endGame;

		// This should be cleared/nulled out at the end of every day.
		public static HashSet<int> claimedPrizes = new HashSet<int>();

        public PinGame()
        {
            Game1.player.CanMove = false;
            Game1.player.FacingDirection = 0;
			_callouts = new List<Callout>();
			nextRiffRaff = 0.0F;

			StartGame();
		}

        public void StartGame()
        {
            // Make sure we have to move a fair amount
            while (PlayerPosition > 0.35f && PlayerPosition < 0.65f)
            {
                PlayerPosition = (float)Game1.random.NextDouble();
            }
            StartTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;

            Game1.player.canOnlyWalk = false;
            Game1.player.setRunning(false, true);
            Game1.player.canOnlyWalk = true;
            UpdatePlayer();
            UpdateHint();
            CurrentStage = 0;
			nextHint = 0.5F;

			gameStartTimer = 1.0F;
			gameStartCount = 4;
		}

        public void EndGame()
        {
			// Only grant prizes if the game was actually finished.
			if (CurrentStage >= TotalStages)
			{
				float dist = Math.Abs(PlayerPosition - 0.5f);

				if (dist < 0.005 && ClaimPrizeOrPass(373, "discoverMineral")) // Golden Pumpkin
				{

				}
				else if (dist < 0.05 && ClaimPrizeOrPass(305, "newArtifact")) // Void Egg
				{

				}
				else if (dist < 0.1 && ClaimPrizeOrPass(203, "newArtifact")) // Strange Bun
				{

				}
				else if (dist < 0.25 && ClaimPrizeOrPass(78, "hoeHit")) // Cave Carrot
				{

				}
				else // Rotten plant
				{
					ClaimPrizeOrPass(747, "slimeHit");
					claimedPrizes.Remove(747); // Unlimited stock of rotten plants.
				}

				if (ModEntry.setGoblinNosePosition != null)
				{
					ModEntry.setGoblinNosePosition(Game1.player.Position);
				}
			}

            UpdatePlayer();
            this.unload();
            //Game1.currentMinigame = null;
            Game1.player.forceCanMove();
            Game1.player.canOnlyWalk = false;
        }

		public bool ClaimPrizeOrPass(int item_index, string sound)
		{
			if (claimedPrizes == null)
			{
				claimedPrizes = new HashSet<int>();
			}

			// This prize was already claimed.
			if (claimedPrizes.Contains(item_index))
			{
				return false;
			}

			Game1.player.addItemByMenuIfNecessary(new SVObject(item_index, 1));
			Game1.playSound(sound);

			claimedPrizes.Add(item_index);

			return true;
		}

        public void UpdatePlayer()
        {
            if (gameStartCount > 0)
            {
                return;
            }

			float game_progress = (float)((Game1.currentGameTime.TotalGameTime.TotalSeconds - StartTime) / GameLengthSeconds);

			float X = (Math.Min(26.0F, Math.Max(22.0F, MathHelper.Lerp(22.0F, 26.0F, PlayerPosition)))) * Game1.tileSize;

            float Y = (Math.Min(21.0F, Math.Max(20.0F, MathHelper.Lerp(21.0F, 20.0F, game_progress)))) * Game1.tileSize;

			Game1.player.Position = new Vector2(X, Y);
        }

        public void UpdateHint()
        {
			if (CurrentStage >= 0 && CurrentStage <= TotalStages)
			{
				Game1.playSound("shadowpeep");

				// Originally this was going to use a Lerp, but I want a bit more flexibility here in order to have
				// different sized ranges and some randomness to the text.
				string direction = (PlayerPosition > 0.5f) ? "left" : "right";
				string key = "NoDistance";
				float dist = Math.Abs(PlayerPosition - 0.5f);
				if (dist < 0.005)
				{
					key = "OnTarget";
				}
				else if (dist < 0.1)
				{
					key = $"Close.{Game1.random.Next(3)}";
				}
				else if (dist < 0.25)
				{
					key = $"Medium.{Game1.random.Next(3)}";
				}
				else
				{
					key = $"Far.{Game1.random.Next(3)}";
				}

				Callout callout = new Callout();

				callout.calloutText = ModEntry.Instance.Helper.Translation.Get($"PinGame.Hint.{key}", new { direction = direction }); ;

				_callouts.Add(callout);
			}

			// CurrentDist is a horizontal large unit ("feet") during the game and is then converted to a
			// small unit ("inches") after the game. Small unit is 1/100 of the large unit, but it we
			// stick with Imperial units since the game refers to inches in other areas like fish size.
			if (CurrentStage <= TotalStages)
            {
                CurrentDist = ModEntry.Instance.Helper.Translation.Get($"PinGame.Distance", new { d = 1 + TotalStages - CurrentStage });
            }
            LastUpdate = Game1.currentGameTime.TotalGameTime.TotalSeconds;
        }

        public bool tick(GameTime time)
        {
			if (_gameEndTimer > 0)
			{
				_gameEndTimer -= (float)time.ElapsedGameTime.TotalSeconds;

				if (_gameEndTimer <= 0)
				{
					_endGame = true;
				}
			}

			UpdatePlayer();

			if (_endGame)
			{
				_endGame = false;
				EndGame();
				return true;
			}

			if (gameStartCount > 0)
			{
				gameStartTimer -= (float)time.ElapsedGameTime.TotalSeconds;

				if (gameStartTimer <= 0)
				{
					gameStartTimer = 1.0F;
					gameStartCount--;

					if (gameStartCount == 0)
					{
						Game1.playSound("whistle");
					}
					else
					{
						var cue = Game1.soundBank.GetCue("clam_tone");
						cue.SetVariable("Pitch", 1200 - 100 * gameStartCount);
						cue.Play();
					}
				}

				return false;
			}

            if (CurrentStage < 0)
            {
                return false;
            }

			if (CurrentStage > 0 && CurrentStage <= TotalStages)
			{
				if (nextHint <= 0.0F)
				{
					UpdateHint();
					nextHint = 2.75F;
				}
				else
				{
					nextHint -= (float)time.ElapsedGameTime.TotalSeconds;
				}
			}

            if (Game1.currentGameTime.TotalGameTime.TotalSeconds > StartTime + GameLengthSeconds * ((double)CurrentStage / (double)TotalStages) )
            {
                CurrentStage++;
                if (CurrentStage == TotalStages + 1)
                {
					_callouts.Clear();
                    Game1.playSound("axe");

					_gameEndTimer = 4.0F;

					// Show the results.
					CurrentDist = ModEntry.Instance.Helper.Translation.Get($"PinGame.End.0",
						new { d = $"{100f * Math.Abs(PlayerPosition - 0.50f):0.#}" });
				}
            }

			if (CurrentStage > 0 && CurrentStage <= TotalStages)
			if (nextRiffRaff <= 0.0F)
			{
				nextRiffRaff = 0.3F + (float)Game1.random.NextDouble() * 0.25F;

				Callout callout = new Callout();

				callout.calloutText = ModEntry.Instance.Helper.Translation.Get($"PinGame.RiffRaff." + Game1.random.Next(11));

				callout.lifeTime = 1.25F;

				_callouts.Add(callout);
			}
			else
			{
				nextRiffRaff -= (float)time.ElapsedGameTime.TotalSeconds;
			}

			for (int i = 0; i < _callouts.Count; i++)
			{
				Callout callout = _callouts[i];

				callout.Update(time);

				// If this callout has expired, remove it.
				if (callout.age >= callout.lifeTime)
				{
					_callouts.RemoveAt(i);
					i--;
				}
			}

			// We want some variability in player movement. Remember that full range of positions is (0,1) with 0.5 right on target
			if (CurrentDirection != 0)
            {
				float MoveAmount = 0.0025f;
                if (CurrentDirection == -1)
                {
                    PlayerPosition = (float)Math.Max(0, PlayerPosition - MoveAmount);
                }
                else if (CurrentDirection == 1)
                {
                    PlayerPosition = (float)Math.Min(1, PlayerPosition + MoveAmount);
                }
			}

			// Hacky, but move the callouts so we can get a sense of spatial movement.
			foreach (Callout callout in _callouts)
			{
				callout.drawPosition.X += CurrentDirection * -1.0F;
				callout.drawPosition.Y += 0.3F;
			}

			if (CurrentStage > 0 && CurrentStage <= TotalStages)
			{
				float step_size = 1.0F;

				currentStepTimer += (float)time.ElapsedGameTime.TotalSeconds;

				if (CurrentDirection != 0)
				{
					step_size = 0.25F;
				}

				if (currentStepTimer >= step_size || (_lastCurrentDirection != CurrentDirection && CurrentDirection != 0)) // If we should take a step or we've changed from non-moving to moving.
				{
					Game1.playSound("stoneStep");
					currentStepTimer = 0.0F;
				}


			}

			_lastCurrentDirection = CurrentDirection;

			return false;
        }

        public bool overrideFreeMouseMovement()
        {
            return true;
        }

        public bool doMainGameUpdates()
        {
            return false;
        }

        public void receiveLeftClick(int x, int y, bool playSound = true)
        {
			if (CurrentStage >= TotalStages)
			{
				_endGame = true;
			}
		}

        public void receiveRightClick(int x, int y, bool playSound = true)
        {

        }

        public void leftClickHeld(int x, int y)
        {

        }

        public void releaseLeftClick(int x, int y)
        {

        }

        public void releaseRightClick(int x, int y)
        {

        }

        public void receiveKeyPress(Keys k)
        {
            if (k.Equals(Keys.Escape))
            {
				_endGame = true;
                return;
            }

            if (CurrentStage >= 0)
            {
                if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, k))
                {
                    CurrentDirection = -1;
                }
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, k))
                {
                    CurrentDirection = 1;
                }
            }

        }

        public void receiveKeyRelease(Keys k)
        {
            if (CurrentDirection == -1 && Game1.options.doesInputListContain(Game1.options.moveLeftButton, k))
            {
                CurrentDirection = 0;
            }
            else if (CurrentDirection == 1 && Game1.options.doesInputListContain(Game1.options.moveRightButton, k))
            {
                CurrentDirection = 0;
            }

        }

        public void draw(SpriteBatch b)
        {
            b.Begin();
            
            if (gameStartCount > 0)
            {
                Game1.drawWithBorder(ModEntry.Instance.Helper.Translation.Get("PinGame.Instructions.0"), Game1.textColor, Color.LimeGreen, new Vector2(50, 50));
                Game1.drawWithBorder(ModEntry.Instance.Helper.Translation.Get("PinGame.Instructions.1"), Game1.textColor, Color.LimeGreen, new Vector2(50, 100));
                //Game1.drawWithBorder(ModEntry.Instance.Helper.Translation.Get("PinGame.Instructions.2"), Game1.textColor, Color.LimeGreen, new Vector2(50, 150));
            }
            else if (CurrentStage > TotalStages)
            {
                Game1.drawWithBorder(CurrentDist, Game1.textColor, Color.LimeGreen, new Vector2(50, 50));
                //Game1.drawWithBorder(ModEntry.Instance.Helper.Translation.Get("PinGame.End.1"), Game1.textColor, Color.LimeGreen, new Vector2(50, 100));
            }

			foreach (Callout callout in _callouts)
			{
				callout.Draw(b);
			}

			/*
            Game1.drawWithBorder($"Debug: PlayerPosition = {PlayerPosition}", Game1.textColor, Color.Pink, new Vector2(50, 400));
            Game1.drawWithBorder($"Debug: Stage = {CurrentStage}/{TotalStages}", Game1.textColor, Color.Pink, new Vector2(50, 450));
            Game1.drawWithBorder($"Debug: StartTime = {StartTime}", Game1.textColor, Color.Pink, new Vector2(50, 500));
            Game1.drawWithBorder($"Debug: GameTime = {Game1.currentGameTime.TotalGameTime.TotalSeconds}", Game1.textColor, Color.Pink, new Vector2(50, 550));
            */

			Game1.player.FarmerSprite.draw(b, new Vector2(100, 100), 0.5F);

			b.End();
		}

        public void changeScreenSize()
        {

        }

        public void unload()
        {
        }

        public void receiveEventPoke(int data)
        {
            throw new NotImplementedException();
        }

        public string minigameId()
        {
            return "ShadwFestival.PinGame";
        }

        public bool forceQuit()
        {
            return true;
        }

        public class Callout
		{
			public float age = 0.0F;
			public float lifeTime = 2.5F;
			public float fadeTime = 0.25F;
			public string calloutText;
			public Vector2 position;
			public float shakeTimer;

			public Vector2 velocity;

			public Vector2 drawPosition;

            public Callout()
			{
				drawPosition.X = (float)Game1.random.NextDouble() * Game1.viewport.Width;
				drawPosition.Y = (float)Game1.random.NextDouble() * Game1.viewport.Height;

				velocity.X = (float)Game1.random.NextDouble();
				velocity.Y = (float)Game1.random.NextDouble();

				if (velocity.LengthSquared() > 0)
				{
					velocity.Normalize();
					velocity *= (float)Game1.random.NextDouble() * 0.5F;
				}

				shakeTimer = 0.5F;
			}

			public void Draw(SpriteBatch b)
			{
				float alpha = Math.Max(Math.Min(1, (lifeTime - age) / fadeTime), 0);

				float width = StardewValley.BellsAndWhistles.SpriteText.getWidthOfString(calloutText) + 32;
				float height = StardewValley.BellsAndWhistles.SpriteText.getHeightOfString(calloutText) + 32;

				// Keep the callouts on-screen.
				if (drawPosition.X < width / 2)
				{
					drawPosition.X = width / 2;
				}

				if (drawPosition.X > Game1.viewport.Width - width / 2)
				{
					drawPosition.X = Game1.viewport.Width - width / 2;
				}

				if (drawPosition.Y < height / 2)
				{
					drawPosition.Y = height / 2;
				}

				if (drawPosition.Y > Game1.viewport.Height - height / 2)
				{
					drawPosition.Y = Game1.viewport.Height - height / 2;
				}

				Vector2 shake_amount = Vector2.Zero;

				if (shakeTimer > 0)
				{
					shake_amount.X = Game1.random.Next(-1, 2) * 2;
					shake_amount.Y = Game1.random.Next(-1, 2) * 2;
				}

				StardewValley.BellsAndWhistles.SpriteText.drawStringWithScrollCenteredAt(b, calloutText, (int)(drawPosition.X + shake_amount.X), (int)(drawPosition.Y + shake_amount.Y), "", alpha, -1, StardewValley.BellsAndWhistles.SpriteText.scrollStyle_speechBubble, 0.88F, false);
			}
			
			public void Update(GameTime time)
			{
				age += (float)time.ElapsedGameTime.TotalSeconds;

				if (shakeTimer > 0)
				{
					shakeTimer -= (float)time.ElapsedGameTime.TotalSeconds;
				}

				drawPosition += velocity;
			}

		}

    }
}
