/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

namespace Cropbeasts.Projectiles
{
	public class SeedShot : BeastProjectile
	{
		public SeedShot ()
		{}

		public SeedShot (int damage, int seedIndex)
		{
			damageToFarmer.Value = damage;

			currentTileSheetIndex.Value = 114; // Ancient Seed
			spriteFromObjectSheet.Value = true;

			collisionSound.Value = "seeds";

			parryCatchIndex.Value = seedIndex;
			parryCatchChance.Value = 0.08;
		}
	}
}
