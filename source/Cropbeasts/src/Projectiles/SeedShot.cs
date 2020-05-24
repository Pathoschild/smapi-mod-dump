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
