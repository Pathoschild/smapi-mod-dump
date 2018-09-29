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

			if (this.options.IncludeBonusMoney)
			{
				this.Monitor.Log("Adding 250 gold to starting money", LogLevel.Info);
				Game1.player.Money = Game1.player.Money + 250;
			}
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