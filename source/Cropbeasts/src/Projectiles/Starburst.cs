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
	public class Starburst : BeastProjectile
	{
		public Starburst ()
		{ }

		public Starburst (int damage)
		{
			damageToFarmer.Value = damage;

			currentTileSheetIndex.Value = 768; // Solar Essence
			spriteFromObjectSheet.Value = true;

			bouncesLeft.Value = 1;
			tailLength.Value = 8;

			firingSound.Value = "debuffSpell";
			collisionSound.Value = "debuffHit";

			shouldExplode.Value = true;

			parryCatchChance.Value = 0.2;
		}
	}
}
