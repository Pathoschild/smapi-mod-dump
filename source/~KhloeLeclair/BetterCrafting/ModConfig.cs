/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

using Leclair.Stardew.BetterCrafting.Models;

using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.BetterCrafting {

	public enum MaxQuality {
		Disabled,
		None,
		Silver,
		Gold,
		Iridium
	};

	public enum ButtonAction {
		None,
		Craft,
		BulkCraft,
		Favorite
	};

	public enum TTWhen {
		Never,
		ForController,
		Always
	};

	public class ModConfig {

		public string Theme { get; set; } = "Automatic";

		public bool ReplaceCooking { get; set; } = true;
		public bool ReplaceCrafting { get; set; } = true;

		public bool UseCategories { get; set; } = true;

		public bool ShowSettingsButton { get; set; } = true;

		// Quality
		public MaxQuality MaxQuality { get; set; } = MaxQuality.Iridium;
		public bool LowQualityFirst { get; set; } = true;

		// Bindings
		public TTWhen ShowKeybindTooltip { get; set; } = TTWhen.ForController;

		public KeybindList SuppressBC { get; set; } = KeybindList.Parse("LeftShift");

		public KeybindList FavoriteKey { get; set; } = KeybindList.Parse("F, ControllerBack");
		public KeybindList BulkCraftKey { get; set; } = KeybindList.Parse("None");
		public KeybindList SearchKey { get; set; } = KeybindList.Parse("F3");

		// Actions
		public ButtonAction LeftClick { get; set; } = ButtonAction.Craft;
		public ButtonAction RightClick { get; set; } = ButtonAction.BulkCraft;


		// Standard Crafting
		public bool UseUniformGrid { get; set; } = false;
		public bool SortBigLast { get; set; } = false;
		public bool CraftingAlphabetic { get; set; } = false;

		// Cooking
		public SeasoningMode UseSeasoning { get; set; } = SeasoningMode.Enabled;
		public bool HideUnknown { get; set; } = false;
		public bool CookingAlphabetic { get; set; } = false;


		// Better Workbench
		public bool UseDiscovery { get; set; } = true;
		public int MaxInventories { get; set; } = 15;
		public int MaxDistance { get; set; } = 20;
		public int MaxCheckedTiles { get; set; } = 500;

		public bool UseDiagonalConnections { get; set; } = true;

		public CaseInsensitiveHashSet ValidConnectors { get; set; } = new();

	}
}
