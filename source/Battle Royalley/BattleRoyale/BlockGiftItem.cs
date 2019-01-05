using StardewValley;

namespace BattleRoyale
{
	class BlockGiftItem : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farmer), "checkAction");

		public static bool Prefix(Farmer who, GameLocation location, Farmer __instance)
		{
			if (Game1.CurrentEvent != null && Game1.CurrentEvent.isSpecificFestival("spring24") && who.dancePartner.Value == null)
				return true;

			if (who.CurrentItem != null && (int)who.CurrentItem.parentSheetIndex == 801 && !__instance.isMarried() && !__instance.isEngaged() && !who.isMarried() && !who.isEngaged())
				return true;

			if (who.ActiveObject != null && who.ActiveObject.canBeGivenAsGift())
				return false;

			return true;
		}
	}
}
