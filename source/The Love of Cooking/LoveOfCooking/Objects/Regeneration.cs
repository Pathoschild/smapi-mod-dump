/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoveOfCooking
{
	public class Regeneration
	{
		protected struct Regen
		{
			public float HP;
			public float EP;

			public float Total => this.HP + this.EP;

			public Regen(float hp, float ep)
			{
				this.HP = hp;
				this.EP = ep;
			}

			public static implicit operator Regen(int operand)
			{
				return new Regen { HP = operand, EP = operand };
			}

			public static implicit operator Regen(float operand)
			{
				return new Regen { HP = operand, EP = operand };
			}

			public static Regen operator +(Regen lhs, Regen rhs)
			{
				return new Regen { HP = lhs.HP + rhs.HP, EP = lhs.EP + rhs.EP };
			}

			public static Regen operator -(Regen lhs, Regen rhs)
			{
				return new Regen { HP = lhs.HP - rhs.HP, EP = lhs.EP - rhs.EP };
			}

			public static Regen operator *(Regen lhs, Regen rhs)
			{
				return new Regen { HP = lhs.HP * rhs.HP, EP = lhs.EP * rhs.EP };
			}

			public static Regen operator /(Regen lhs, Regen rhs)
			{
				return new Regen { HP = lhs.HP / rhs.HP, EP = lhs.EP / rhs.EP };
			}

			public static Regen operator ++(Regen operand)
			{
				return new Regen { HP = operand.HP++, EP = operand.EP++ };
			}

			public static Regen operator --(Regen operand)
			{
				return new Regen { HP = operand.HP--, EP = operand.EP-- };
			}

			public override string ToString()
			{
				return $"H{HP} E{EP}";
			}
		}

		// Active variables

		/// <summary> Whether the player health status bar was visible on the previous game tick. </summary>
		protected bool IsHealthBarVisible;
		/// <summary> Instance of the last edible object consumed by the player. </summary>
		protected StardewValley.Object LastFoodEaten;
		/// <summary> Player status bar values as of the previous game tick. </summary>
		protected Regen PlayerValue;
		/// <summary> Current food regeneration values waiting to be added to player status bars. </summary>
		protected Regen RemainingValue;
		/// <summary> Total regeneration amount to be added to player status bars since last emptied. </summary>
		protected Regen InitialValue;
		/// <summary> Current incremental tick count for regenerating player status bars. </summary>
		protected Regen TicksCurrent;
		/// <summary> Ticks required for player status bars before regeneration is applied to either. </summary>
		protected Regen TicksRequired;

		// Config variables

		/// <summary> Modifier for base rate of regeneration. </summary>
		protected int BaseRate;
		/// <summary> Modifier for final rate of regeneration. </summary>
		protected float FinalRate;
		/// <summary> Modifier for rate of HP regeneration. </summary>
		protected float HealthRate;
		/// <summary> Modifier for rate of EP regeneration. </summary>
		protected float EnergyRate;
		/// <summary> Modifier for base rate of regeneration considering all current skill modifiers affecting food regeneration. </summary>
		protected float SkillModifierRate;

		// Debug variables

		/// <summary> Queue of the most recent 5 tick counts for regenerating player status bars. </summary>
		protected readonly Queue<Regen> TickerHistory = new();
		
		/// <summary>
		/// Register listeners/handlers for game and SMAPI event hooks.
		/// </summary>
		public void RegisterEvents(IModHelper helper)
		{
			helper.Events.GameLoop.UpdateTicked += this.Update;
			if (ModEntry.Config.ShowFoodRegenBar)
			{
				helper.Events.Display.RenderingHud += this.Draw;
				helper.Events.Display.Rendered += this.AfterDraw;
			}
			else
			{
				helper.Events.Display.RenderingHud -= this.Draw;
				helper.Events.Display.Rendered -= this.AfterDraw;
			}
		}

		/// <summary>
		/// Update cached config values from definitions data file.
		/// </summary>
		public void UpdateDefinitions()
		{
			// Calculate food regeneration rate from skill levels
			const int maxLevel = 10;
			float[] scalingCurrent = new float[ModEntry.ItemDefinitions["RegenSkillModifiers"].Count];
			float[] scalingMax = new float[scalingCurrent.Length];
			for (int i = 0; i < ModEntry.ItemDefinitions["RegenSkillModifiers"].Count; ++i)
			{
				string[] split = ModEntry.ItemDefinitions["RegenSkillModifiers"][i].Split(':');
				string name = split[0];
				bool isDefined = Enum.TryParse(name, out ModEntry.SkillIndex skillIndex);
				int level = isDefined
					? Game1.player.GetSkillLevel((int)Enum.Parse(typeof(ModEntry.SkillIndex), name))
					: SpaceCore.Skills.GetSkill(name) is not null
						? Game1.player.GetCustomSkillLevel(name)
						: -1;
				float value = float.Parse(split[1]);
				if (level < 0)
					continue;
				scalingCurrent[i] = level * value;
				scalingMax[i] = maxLevel * value;
			}

			// Set values
			this.SkillModifierRate = scalingCurrent.Sum() / scalingMax.Sum();
			this.BaseRate = int.Parse(ModEntry.ItemDefinitions["RegenBaseRate"][0]);
			this.HealthRate = float.Parse(ModEntry.ItemDefinitions["RegenHealthRate"][0]);
			this.EnergyRate = float.Parse(ModEntry.ItemDefinitions["RegenEnergyRate"][0]);
			this.FinalRate = float.Parse(ModEntry.ItemDefinitions["RegenFinalRate"][0]);
		}

		/// <summary>
		/// Adds HP and EP values for a given food to the remaining regeneration amount.
		/// </summary>
		/// <param name="food">Food item to be used for HP and EP regeneration.</param>
		public void Eat(StardewValley.Object food)
		{
			this.LastFoodEaten = food.getOne() as StardewValley.Object;

			// Regenerate health/energy over time:
			int foodHealth = food.healthRecoveredOnConsumption();
			int foodStamina = food.staminaRecoveredOnConsumption();

			// Add new regen values to current amount
			this.RemainingValue.HP += foodHealth;
			this.RemainingValue.EP += foodStamina;

			// Reset total regeneration values to start from current remaining amount
			this.InitialValue = this.RemainingValue;

			// Revert player values to before having eaten food
			this.RevertPlayer();
		}

		/// <summary>
		/// Add HP and EP values to the remaining regeneration amount.
		/// Negative values will deduct from remaining regeneration.
		/// </summary>
		/// <param name="hp">HP value to add.</param>
		/// <param name="ep">EP value to add.</param>
		public void Add(int hp, int ep)
		{
			this.RemainingValue += new Regen { HP = hp, EP = ep };
		}

		/// <summary>
		/// Revert player status bar values to their state on the previous tick.
		/// </summary>
		public void RevertPlayer()
		{
			Game1.player.health = (int)Math.Ceiling(this.PlayerValue.HP);
			Game1.player.Stamina = this.PlayerValue.EP;
		}

		/// <summary>
		/// Update HP and EP regeneration values for the player on the current screen on the current tick.
		/// </summary>
		protected void Update(object sender, UpdateTickedEventArgs e)
		{
			// Track player HP/EP to use in reverting instant food healing
			if (Game1.player is not null && Context.IsWorldReady)
			{
				this.PlayerValue = new Regen { HP = Game1.player.health, EP = Game1.player.Stamina };
			}

			// Do not regenerate if game is paused
			if ((!Game1.IsMultiplayer && (!Game1.game1.IsActive || Game1.eventUp)) || (Game1.activeClickableMenu is not null && !Game1.shouldTimePass()))
			{
				return;
			}

			// Check to regenerate HP/EP for player over time
			if (this.RemainingValue.HP < 1 && this.RemainingValue.EP < 1)
			{
				this.InitialValue = 0;
				return;
			}

			// End HP/EP regeneration on player KO
			if (Game1.player.health < 1)
			{
				this.PlayerValue = 1;
				return;
			}

			// Fetch all components for the rate of HP/EP regeneration
			int cookingLevel = ModEntry.CookingSkillApi.GetLevel();
			Regen panicMultiplier = new Regen
			{
				HP = Game1.player.health / Game1.player.maxHealth,
				EP = Game1.player.Stamina / Game1.player.MaxStamina
			};
			float foodMultiplier = Utils.GetFoodRegenRate(this.LastFoodEaten);

			// Calculate regeneration for both status bars
			float calculateRate(float value, float mult) => (float)Math.Floor(Math.Max(36 - cookingLevel * 1.75f, value * mult) / this.FinalRate);
			Regen rate = this.BaseRate - (this.BaseRate * this.SkillModifierRate * foodMultiplier * 100);
			rate.HP = (int)(calculateRate(value: rate.HP, mult: panicMultiplier.HP) / this.HealthRate);
			rate.EP = (int)(calculateRate(value: rate.EP, mult: panicMultiplier.EP) / this.EnergyRate);
			this.TicksRequired = rate;
			bool diff = false;

			// Regenerate player HP/EP when tick count is greater than required tick rate
			if (this.RemainingValue.HP > 0 && this.TicksCurrent.HP < this.TicksRequired.HP)
			{
				++this.TicksCurrent.HP;
			}
			else if (this.TicksCurrent.HP >= rate.HP)
			{
				this.TicksCurrent.HP = 0;
				if (this.RemainingValue.HP > 0)
				{
					if (Game1.player.health < Game1.player.maxHealth)
						++Game1.player.health;
					--this.RemainingValue.HP;
					diff = true;
				}
			}
			if (this.RemainingValue.EP > 0 && this.TicksCurrent.EP < this.TicksRequired.EP)
			{
				++this.TicksCurrent.EP;
			}
			else if (this.TicksCurrent.EP >= rate.EP)
			{
				this.TicksCurrent.EP = 0;
				if (this.RemainingValue.EP > 0)
				{
					if (Game1.player.Stamina < Game1.player.MaxStamina)
						++Game1.player.Stamina;
					--this.RemainingValue.EP;
					diff = true;
				}
			}
			if (diff)
			{
				this.TickerHistory.Enqueue(this.TicksCurrent);
				if (this.TickerHistory.Count > 5)
					this.TickerHistory.Dequeue();
			}
		}

		/// <summary>
		/// Draw status bar for food HP and EP regeneration to the HUD on the current screen.
		/// </summary>
		[EventPriority(EventPriority.Low)]
		protected void Draw(object sender, RenderingHudEventArgs e)
		{
			if (!Context.IsWorldReady || Game1.farmEvent is not null || this.RemainingValue.Total <= 0)
			{
				return;
			}

			Rectangle viewport = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

			const int heightFromBottom = 4 * Game1.pixelZoom;
			const int otherBarWidth = 12 * Game1.pixelZoom;
			const int otherBarSpacing = 1 * Game1.pixelZoom;

			Point barIconOffset = new Point(x: -1, y: 1);

			int otherBarCount = this.IsHealthBarVisible ? 2 : 1;
			int width = AssetManager.RegenBarArea.Width * Game1.pixelZoom;
			int height = AssetManager.RegenBarArea.Height * Game1.pixelZoom;

			int sourceWidth = AssetManager.RegenBarArea.Width;
			int sourceHeight = AssetManager.RegenBarArea.Height;
			Vector2 regenBarOrigin = new Vector2(
				x: viewport.Right - (sourceWidth * Game1.pixelZoom / 2) - (otherBarWidth * (1 + otherBarCount)),
				y: viewport.Bottom - heightFromBottom - (sourceHeight * Game1.pixelZoom));

			// Regen bar sprites
			{
				// region of cursors spritesheet asset to find base game regen bar sprite
				Rectangle originalArea = new Rectangle(256, 408, 12, 56);
				// starting region of original region to read data from
				Rectangle sourceArea = new Rectangle(
					x: originalArea.X,
					y: originalArea.Y,
					width: sourceWidth / 2,
					height: sourceHeight / 2);
				// starting target region on screen to draw to
				Rectangle destArea = new Rectangle(
					x: (int)regenBarOrigin.X,
					y: (int)regenBarOrigin.Y,
					width: sourceArea.Width * Game1.pixelZoom,
					height: sourceArea.Height * Game1.pixelZoom);

				Point[] sourceOffsets = new Point[]
				{
					Point.Zero,
					new Point(originalArea.Width - sourceArea.Width, 0),
					new Point(0, originalArea.Height - sourceArea.Height),
					new Point(originalArea.Width - sourceArea.Width, originalArea.Height - sourceArea.Height)
				};
				Point[] destOffsets = new Point[]
				{
					Point.Zero,
					new Point(destArea.Width, 0),
					new Point(0, destArea.Height),
					new Point(destArea.Width, destArea.Height)
				};
				for (int i = 0; i < 4; ++i)
				{
					Rectangle newSourceArea = sourceArea;
					newSourceArea.X += sourceOffsets[i].X;
					newSourceArea.Y += sourceOffsets[i].Y;
					Rectangle newDestArea = destArea;
					newDestArea.X += destOffsets[i].X;
					newDestArea.Y += destOffsets[i].Y;

					e.SpriteBatch.Draw(
						texture: Game1.mouseCursors,
						sourceRectangle: newSourceArea,
						destinationRectangle: newDestArea,
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						effects: SpriteEffects.None,
						layerDepth: 1f);
				}
				// cooking skill icon
				e.SpriteBatch.Draw(
					texture: ModEntry.SpriteSheet,
					sourceRectangle: AssetManager.CookingSkillIconArea,
					position: new Vector2(
						x: destArea.X - (barIconOffset.X * Game1.pixelZoom),
						y: destArea.Y - (barIconOffset.Y * Game1.pixelZoom)),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: Game1.pixelZoom,
					effects: SpriteEffects.None,
					layerDepth: 1f);
			}

			// Regen bar fill colour
			{
				Point borderWidth = new Point(
					x: 3 * Game1.pixelZoom,
					y: 3 * Game1.pixelZoom);
				float fillColourHeightRatio = (float)this.RemainingValue.Total / this.InitialValue.Total;
				int xOffset = borderWidth.X;
				int yOffset = barIconOffset.Y + (AssetManager.CookingSkillIconArea.Height * Game1.pixelZoom);
				width -= (xOffset + borderWidth.X);
				height -= (yOffset + borderWidth.Y);

				// Draw background
				Vector2 fillColourOrigin = new Vector2(
					x: regenBarOrigin.X + xOffset,
					y: regenBarOrigin.Y + yOffset + height + (1 * Game1.pixelZoom) - (int)(height * fillColourHeightRatio));
				if (Game1.isOutdoorMapSmallerThanViewport())
				{
					fillColourOrigin.X = Math.Min(fillColourOrigin.X, -Game1.viewport.X + (Game1.currentLocation.Map.Layers[0].LayerWidth * Game1.tileSize));
				}
				e.SpriteBatch.Draw(
					texture: ModEntry.SpriteSheet,
					position: fillColourOrigin,
					sourceRectangle: AssetManager.RegenBarArea,
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: Game1.pixelZoom,
					effects: SpriteEffects.None,
					layerDepth: 1f);

				// Draw fill colour
				Color colour = Utility.getRedToGreenLerpColor(0.5f);
				Rectangle destArea = new Rectangle(
					x: (int)fillColourOrigin.X,
					y: (int)fillColourOrigin.Y,
					width: width,
					height: (int)(height * fillColourHeightRatio));
				// fill colour body
				e.SpriteBatch.Draw(
					texture: Game1.staminaRect,
					destinationRectangle: destArea,
					sourceRectangle: Game1.staminaRect.Bounds,
					color: colour,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// fill colour top border
				destArea.Height = 1 * Game1.pixelZoom;
				colour.R = (byte)Math.Max(0, colour.R - 50);
				colour.G = (byte)Math.Max(0, colour.G - 50);
				e.SpriteBatch.Draw(
					texture: Game1.staminaRect,
					destinationRectangle: destArea,
					sourceRectangle: Game1.staminaRect.Bounds,
					color: colour,
					rotation: 0f,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: 1f);

				// Draw value
				if (Game1.getOldMouseX() >= fillColourOrigin.X
					&& Game1.getOldMouseY() >= regenBarOrigin.Y
					&& Game1.getOldMouseX() < fillColourOrigin.X + width)
				{
					SpriteFont font = Game1.smallFont;
					string text = $"H +{Math.Max(0, this.RemainingValue.HP)}\nE +{Math.Max(0, this.RemainingValue.EP)}";
					Vector2 position = regenBarOrigin + new Vector2(
						x: (-4 * Game1.pixelZoom) - font.MeasureString("H +000").X - otherBarSpacing,
						y: 0);
					e.SpriteBatch.DrawString(
						spriteFont: font,
						text: text,
						position: position,
						color: Color.White);
				}
			}

			// Draw debug info if enabled
			if (ModEntry.Config.DebugMode)
			{
				string[] debugLines;
				Vector2 linePosition;
				Vector2 blockPosition;
				Vector2 margin = new Vector2(
					x: 16,
					y: 224 + (int)((Game1.player.MaxStamina - 270) * 0.625f));

				// Labels
				debugLines = new[] {
					new string('\n', this.TickerHistory.Count),
					"TICKS",
					"RATE",
					"REGEN"
				};
				linePosition = blockPosition = new Vector2(
					x: viewport.Right - margin.X,
					y: viewport.Bottom - margin.Y);
				foreach (string debugLine in debugLines.Reverse())
				{
					Vector2 textSize = Game1.smallFont.MeasureString(debugLine);
					linePosition.X = blockPosition.X - textSize.X;
					linePosition.Y -= textSize.Y;
					Utility.drawTextWithColoredShadow(
						b: e.SpriteBatch,
						text: debugLine,
						font: Game1.smallFont,
						position: linePosition,
						color: Color.White,
						shadowColor: Color.DarkSlateBlue);
				}

				// Values
				debugLines = this.TickerHistory
					.Select(regen => regen)
					.Concat(new[]{
						this.TicksCurrent,
						this.TicksRequired,
						this.RemainingValue})
					.Select(regen => regen.ToString())
					.ToArray();
				blockPosition.X -= 220;
				linePosition = blockPosition;
				foreach (string debugLine in debugLines.Reverse())
				{
					Vector2 textSize = Game1.smallFont.MeasureString(debugLine);
					linePosition.Y -= textSize.Y;
					Utility.drawTextWithColoredShadow(
						b: e.SpriteBatch,
						text: debugLine,
						font: Game1.smallFont,
						position: linePosition,
						color: Color.White,
						shadowColor: Color.DarkSlateBlue);
				}
			}
		}

		/// <summary>
		/// Update regeneration state after HUD is drawn to the current screen.
		/// </summary>
		protected void AfterDraw(object sender, RenderedEventArgs e)
		{
			this.IsHealthBarVisible = Game1.showingHealthBar;
		}
	}
}
