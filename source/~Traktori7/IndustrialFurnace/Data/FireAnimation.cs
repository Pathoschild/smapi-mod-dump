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
	public class FireAnimation
	{
		public bool UseCustomSprite { get; set; } = false;
		public uint SpawnFrequency { get; set; } = 500;
		public float SpawnChance { get; set; } = 0.2f;
		public int SpawnXOffset { get; set; } = 64;
		public int SpawnYOffset { get; set; } = 18;
		public int SpawnXRandomOffset { get; set; } = 16;
		public int SpawnYRandomOffset { get; set; } = 4;
		public int SpriteSizeX { get; set; } = 64;
		public int SpriteSizeY { get; set; } = 64;
		public int AnimationSpeed { get; set; } = 100;
		public int AnimationLength { get; set; } = 4;
		public float SoundEffectChance { get; set; } = 0.05f;
		public int LightSourceXOffset { get; set; } = 96;
		public int LightSourceYOffset { get; set; } = 96;
		public float LightSourceScaleMultiplier { get; set; } = 1.5f;
	}
}
