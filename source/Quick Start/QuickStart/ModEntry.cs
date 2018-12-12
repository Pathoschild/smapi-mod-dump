namespace QuickStart
{
	using System;
	using System.Collections.Generic;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;

	/// <summary>
	/// This is the entry class for this mod.
	/// </summary>
	public class ModEntry : Mod
	{
		public IModHelper modHelper;
		public ModOptions options;
		public List<Item> startingItems;
		public const int FarmSkill = 0;
		public const int FishSkill = 1;
		public const int ForagingSkill = 2;
		public const int MiningSkill = 3;
		public const int CombatSkill = 4;

		/// <summary>
		/// This is the entry method for this mod.
		/// </summary>
		/// <param name="helper"></param>
		public override void Entry(IModHelper helper)
		{
			this.Monitor.Log("Loading Quick Start", LogLevel.Info);
			this.modHelper = helper;
			SaveEvents.BeforeCreate += this.SaveEvents_BeforeCreate;
			this.options = this.modHelper.ReadConfig<ModOptions>();
			this.SetupStartingItems();
		}

		private void SaveEvents_BeforeCreate(object sender, EventArgs e)
		{
			foreach (Item item in this.startingItems)
			{
				this.Monitor.Log("adding item to starting inventory: " + item.DisplayName, LogLevel.Info);

				Game1.player.addItemToInventory(item);
			}

			// Sets the character's health and energy.
			this.SetStats();

			// Sets the characters inventory size, money and other misc things.
			this.SetMisc();

			// Set's the character's skill levels
			this.SetCharacterLevels();
		}

		private void SetupStartingItems()
		{
			this.startingItems = new List<Item>();
			this.ValidateOptions();

			if (this.options.IncludeCharcoal)
			{
				this.startingItems.Add(new StardewValley.Object(StardewValley.Object.coal, this.options.CharcoalCount));
			}

			if (this.options.IncludeFiber)
			{
				// Item 771 is fiber.
				// This was taken from: https://stardewvalleywiki.com/Modding:Object_data
				this.startingItems.Add(new StardewValley.Object(771, this.options.FiberCount));
			}

			if (this.options.IncludeStone)
			{
				this.startingItems.Add(new StardewValley.Object(StardewValley.Object.stone, this.options.StoneCount));
			}

			if (this.options.IncludeWood)
			{
				this.startingItems.Add(new StardewValley.Object(StardewValley.Object.wood, this.options.WoodCount));
			}

			if (this.options.IncludeMixedSeeds)
			{
				// Item 770 is mixed seeds.
				this.startingItems.Add(new StardewValley.Object(770, this.options.MixedSeedsCount));
			}

			if (this.options.IncludeBonusChest)
			{
				this.startingItems.Add(new StardewValley.Objects.Chest(true));
			}

			if (this.options.IncludeClay)
			{
				this.startingItems.Add(new StardewValley.Object(330, this.options.ClayCount));
			}
		}

		private void SetStats()
		{
			if (this.options.SetMaxEnergy)
			{
				this.Monitor.Log("Updating Maximum energy to 400");
				Game1.player.MaxStamina = 400;
				Game1.player.Stamina = 400;
			}

			if (this.options.SetMaxHealth)
			{
				this.Monitor.Log("Updating Maximum health to 125");
				Game1.player.maxHealth = 125;
				Game1.player.health = 125;
			}
		}

		private void SetMisc()
		{
			if (this.options.IncludeFirstBackpackUpgrade)
			{
				this.Monitor.Log("Updating maximum items to match first backpack upgrade");
				Game1.player.increaseBackpackSize(12);
			}

			if (this.options.IncludeBonusMoney)
			{
				this.Monitor.Log("Adding 250 gold to starting money", LogLevel.Info);
				Game1.player.Money = Game1.player.Money + 250;
			}

			if (this.options.GiveCopperWateringCan)
			{
				var wateringCan = Game1.player.getToolFromName("Watering Can") as StardewValley.Tools.WateringCan;

				if (wateringCan != null)
				{
					wateringCan.UpgradeLevel = 1;
					wateringCan.waterCanMax = 55;
					wateringCan.WaterLeft = 55;
				}
			}
		}

		private void SetCharacterLevels()
		{
			if (this.options.SetLevel1FishingLevel)
			{
				this.Monitor.Log("Setting player fishing level to 1");
				Game1.player.FishingLevel = 1;
				Game1.player.experiencePoints[ModEntry.FishSkill] = 101;
			}

			if (this.options.SetLevel1CombatLevel)
			{
				this.Monitor.Log("Setting player combat level to 1");
				Game1.player.CombatLevel = 1;
				Game1.player.experiencePoints[ModEntry.CombatSkill] = 101;
			}

			if (this.options.SetLevel1ForagingLevel)
			{
				this.Monitor.Log("Setting player foraging level to 1");
				Game1.player.ForagingLevel = 1;
				Game1.player.experiencePoints[ModEntry.ForagingSkill] = 101;
			}

			if (this.options.SetLevel1HarvestingLevel)
			{
				this.Monitor.Log("Setting player harvesting level to 1");
				Game1.player.FarmingLevel = 1;
				Game1.player.experiencePoints[ModEntry.FarmSkill] = 101;
			}

			if (this.options.SetLevel1Mininglevel)
			{
				this.Monitor.Log("Setting player mining level to 1");
				Game1.player.MiningLevel = 1;
				Game1.player.experiencePoints[ModEntry.MiningSkill] = 101;
			}
		}

		private void ValidateOptions()
		{
			bool updateConfigFile = false;

			if (this.options.CharcoalCount < 1 || this.options.CharcoalCount > 400)
			{
				updateConfigFile = true;
				this.options.CharcoalCount = ModOptions.DefaultCharcoal;
			}

			if (this.options.FiberCount < 1 || this.options.FiberCount > 400)
			{
				updateConfigFile = true;
				this.options.FiberCount = ModOptions.DefaultFiber;
			}

			if (this.options.StoneCount < 1 || this.options.StoneCount > 400)
			{
				updateConfigFile = true;
				this.options.StoneCount = ModOptions.DefaultStone;
			}

			if (this.options.WoodCount < 1 || this.options.WoodCount > 400)
			{
				updateConfigFile = true;
				this.options.WoodCount = ModOptions.DefaultWood;
			}

			if (this.options.MixedSeedsCount < 1 || this.options.MixedSeedsCount > 400)
			{
				updateConfigFile = true;
				this.options.MixedSeedsCount = ModOptions.DefaultMixedSeeds;
			}

			if (this.options.ClayCount < 1 || this.options.ClayCount > 400)
			{
				updateConfigFile = true;
				this.options.ClayCount = ModOptions.DefaultClay;
			}

			if (updateConfigFile)
			{
				// The configuration file has invalid option data. Re-write it so we don't do this next time.
				this.modHelper.WriteConfig(this.options);
			}
		}
	}
}