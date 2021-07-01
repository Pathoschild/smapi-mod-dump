/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

namespace StardewMods
{
	internal class ModConfig
	{
		public int BarIconMode { get; set; } = 0;
		public string Comment_BarIconMode { get; set; } = "Above BarIconMode values: 0= Horizontal Icons, 1= Vertical Icons, 2= Vertical Icons + Text, 3= Off";
		public int BarTopLeftLocationX { get; set; } = 20;
		public int BarTopLeftLocationY { get; set; } = 20;
		public float BarScale { get; set; } = 1.0f;
		public int BarMaxIcons { get; set; } = 100;
		public int BarMaxIconsPerRow { get; set; } = 20;
		public int BarBackgroundMode { get; set; } = 0;
		public string Comment_BarBackgroundMode { get; set; } = "Above BarBackgroundMode values: 0= Circles (behind each icon), 1= Rectangle (behind everything), 2= Off";
		public bool BarShowBaitAndTackleInfo { get; set; } = true;
		public bool BarShowPercentages { get; set; } = true;
		public int BarSortMode { get; set; } = 0;
		public string Comment_BarSortMode { get; set; } = "Above BarSortMode values: 0= Sort Icons by Name (text mode only), 1= Sort icons by catch chance (Extra Check Frequency based), 2= Off";
		public int BarExtraCheckFrequency { get; set; } = 100;
		public int BarScanRadius { get; set; } = 20;
		public bool BarCrabPotEnabled { get; set; } = true;
		public bool UncaughtFishAreDark { get; set; } = true;
		public int MinigamePreviewMode { get; set; } = 0;
		public string Comment_MinigamePreviewMode { get; set; } = "0= Copies multiple layers to look better. 1= Looks worse, but might perform/work better. 2= Only outlines it in the Bar (BarEnabled needed), 3= Off.";

	}
}
