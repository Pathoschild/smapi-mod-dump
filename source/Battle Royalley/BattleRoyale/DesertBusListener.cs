using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;

namespace BattleRoyale
{
	class DesertBusListener : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Desert), "playerReachedBusDoor");
		
		public static bool Prefix(Desert __instance)
		{
			Game1.activeClickableMenu = new DialogueBox("Head to the right of the road");
			return false;

			/*Console.WriteLine("Exit desert by bus");
			Game1.warpFarmer("Backwoods", 25, 30, false);
			return false;*/
		}
	}
}
