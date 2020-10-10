/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/NeatAdditions
**
*************************************************/

namespace NeatAdditions
{
	class ModConfig
	{
		//Not Harmony
		public bool EnableFlooringAndWallpaperPreview { get; set; } = true;

		//Harmony
		public bool EnableFishingBobControlForGamepads { get; set; } = true;
		public bool EnableFullyCycleToolbar { get; set; } = false;
		public bool CornerFlashGlitch { get; set; } = true;
	}
}
