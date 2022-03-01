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

namespace Leclair.Stardew.SeeMeRollin {
	public class ModConfig {

		public KeybindList UseKey { get; set; } = KeybindList.Parse("Space, LeftStick");

		public bool ShowBuff { get; set; } = true;

		public int SpeedModifier { get; set; } = 6;

		// Speed Falloff
		public FalloffMode FalloffMode { get; set; } = FalloffMode.Slight;


		// Animations
		public bool EnableAnimation { get; set; } = true;


		// Allowances
		public bool AllowWhenSwimming { get; set; } = true;
		public bool AllowWhenSlowed { get; set; } = false;
	}

	public enum FalloffMode {
		None,
		TillZero,
		Slight
	};
}
