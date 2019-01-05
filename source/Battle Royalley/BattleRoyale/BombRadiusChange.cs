using StardewValley;

namespace BattleRoyale
{
	class BombRadiusChange : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameLocation), "explode");

		public static void Prefix(ref int radius)
		{
			radius -= 1;
		}
	}
}
