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

	public class ModConfig {

		public bool ReplaceCooking { get; set; } = true;
		public bool ReplaceCrafting { get; set; } = true;

		public bool UseCategories { get; set; } = true;

		public bool ShowSettingsButton { get; set; } = true;

		// Quality
		public MaxQuality MaxQuality { get; set; } = MaxQuality.Iridium;
		public bool LowQualityFirst { get; set; } = true;

		// Bindings
		public KeybindList SuppressBC { get; set; } = KeybindList.Parse("LeftShift");

		// Standard Crafting
		public bool UseUniformGrid { get; set; } = false;
		public bool SortBigLast { get; set; } = false;

		// Cooking
		public SeasoningMode UseSeasoning { get; set; } = SeasoningMode.Enabled;
		public bool HideUnknown { get; set; } = false;


		// Better Workbench
		public bool UseDiscovery { get; set; } = true;
		public int MaxInventories { get; set; } = 15;
		public int MaxDistance { get; set; } = 20;
		public int MaxCheckedTiles { get; set; } = 500;

		public bool UseDiagonalConnections { get; set; } = true;

		public CaseInsensitiveHashSet ValidConnectors { get; set; } = new();

	}
}
