/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

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
