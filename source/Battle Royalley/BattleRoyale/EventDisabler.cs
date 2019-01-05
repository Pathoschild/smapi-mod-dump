using StardewValley;

namespace BattleRoyale
{
    class EventDisabler : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameLocation), "checkForEvents");

		public static bool Prefix()
		{
			return false;// !ModEntry.game.IsGameInProgress;
		}
	}
}
