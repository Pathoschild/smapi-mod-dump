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
