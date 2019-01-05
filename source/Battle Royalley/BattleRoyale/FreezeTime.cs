using StardewValley;

namespace BattleRoyale
{
	class FreezeTime : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Game1), "shouldTimePass");

		public static bool TimeFrozen { get; set; } = false;
		
		public static bool Postfix(bool __result) => TimeFrozen ? false : __result;
	}
}
