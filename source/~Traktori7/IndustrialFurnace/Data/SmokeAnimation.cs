/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

namespace IndustrialFurnace.Data
{
	public class SmokeAnimation
	{
		public bool UseCustomSprite { get; set; } = false;
		public uint SpawnFrequency { get; set; } = 500;
		public int SpawnXOffset { get; set; } = 68;
		public int SpawnYOffset { get; set; } = -64;
		public int SpriteSizeX { get; set; } = 10;
		public int SpriteSizeY { get; set; } = 10;
		public float SmokeScale { get; set; } = 2;
		public float SmokeScaleChange { get; set; } = 0.02f;
	}
}
