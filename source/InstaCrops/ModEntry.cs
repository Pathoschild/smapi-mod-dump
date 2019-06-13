namespace InstaCrops
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Microsoft.Xna.Framework;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;
	using StardewValley.TerrainFeatures;

	/// <summary>
	/// This class is the main entry point of the mod.
	/// </summary>
	public class ModEntry : Mod
	{
		private IModHelper modHelper;
		private ModConfig config;
		private Random randomvalue;

		/// <summary>
		/// This is the entry method for this mod.
		/// </summary>
		/// <param name="helper"></param>
		public override void Entry(IModHelper helper)
		{
			this.Monitor.Log("Loading InstaCrops", LogLevel.Info);
			this.modHelper = helper;
			this.modHelper.Events.GameLoop.DayEnding += this.GameLoop_DayStarted;
			this.config = this.modHelper.ReadConfig<ModConfig>();
			this.config.ValidateConfigOptions();
			this.randomvalue = new Random();
		}

		/// <summary>
		/// Raised after the game begins a new day (including when the player loads a save).
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void GameLoop_DayStarted(object sender, DayEndingEventArgs e)
		{
			var currentChance = this.UpdateChanceForTodaysLuck();

			foreach (var location in Game1.locations.Where(x => x.IsFarm || x.IsGreenhouse))
			{
				foreach (var pair in location.terrainFeatures.Pairs)
				{
					if (pair.Value is HoeDirt)
					{
						var dirt = pair.Value as HoeDirt;

						// A state of 1 for dirt means it's watered.
						if (dirt.crop != null && !dirt.crop.dead.Value && dirt.state == 1)
						{
							var randomChance = this.config.UseRandomChance ? this.randomvalue.Next(0, 100) : 100;

							if (randomChance <= currentChance)
							{
								dirt.crop.growCompletely();
								// this.Monitor.Log($"Instantly growing crop! Current Chance: {currentChance}, Random Chance: {randomChance}");
							}
						}
					}
				}
			}
		}

		private int UpdateChanceForTodaysLuck()
		{
			if (!this.config.UseRandomChance)
			{
				return 100;
			}

			var currentChance = this.config.ChanceForGrowth;

			if (Game1.dailyLuck < -0.04)
			{
				currentChance -= 10;
			}
			else if (Game1.dailyLuck < 0)
			{
				// Do nothing, this is normal luck.
			}
			else if (Game1.dailyLuck <= 0.04)
			{
				// Kindof lucky today, add a 5% chance.
				currentChance += 5;
			}
			else
			{
				currentChance += 10;
			}

			if (currentChance < 0)
			{
				currentChance = 0;
			}

			if (currentChance > 100)
			{
				currentChance = 100;
			}

			return currentChance;
		}
	}
}
