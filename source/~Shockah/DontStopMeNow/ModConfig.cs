/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.DontStopMeNow
{
	internal class ModConfig
	{
		public bool SlowMove { get; set; } = true;
		public bool MoveWhileSwingingTools { get; set; } = false;
		public bool MoveWhileSwingingMeleeWeapons { get; set; } = true;
		public bool MoveWhileSpecial { get; set; } = true;
		public bool MoveWhileAimingSlingshot { get; set; } = true;
		public bool MoveWhileChargingTools { get; set; } = false;
		public bool FixToolFacing { get; set; } = true;
		public bool FixMeleeWeaponFacing { get; set; } = true;
		public bool FixChargingToolFacing { get; set; } = true;
		public bool FixFishingRodFacing { get; set; } = true;
		public bool FixFacingOnMouse { get; set; } = true;
		public bool FixFacingOnController { get; set; } = false;
	}
}