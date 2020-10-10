/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rupak0577/FishDex
**
*************************************************/

using StardewModdingAPI;

namespace FishDex
{
	// From Pathoschild.Stardew.LookupAnything.Framework.ModConfig
	/// <summary>The parsed mod configuration.</summary>
	internal class ModConfig
	{
		/*********
		** Accessors
		*********/
		/// <summary>The amount to scroll long content on each up/down scroll.</summary>
		public int ScrollAmount { get; set; } = 160;

		/// <summary>Whether to show all the fishes.</summary>
		public bool ShowAll { get; set; } = false;

		/// <summary>The control bindings.</summary>
		public ModConfigControls Controls { get; set; } = new ModConfigControls();


		/*********
		** Nested models
		*********/
		/// <summary>A set of control bindings.</summary>
		internal class ModConfigControls
		{
			/// <summary>The control which toggles the menu.</summary>
			public SButton[] ToggleFishMenu { get; set; } = { SButton.G };

			/// <summary>The control which scrolls up long content.</summary>
			public SButton[] ScrollUp { get; set; } = { SButton.Up };

			/// <summary>The control which scrolls down long content.</summary>
			public SButton[] ScrollDown { get; set; } = { SButton.Down };
		}
	}
}
