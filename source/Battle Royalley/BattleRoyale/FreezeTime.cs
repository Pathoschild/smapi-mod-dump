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

namespace BattleRoyale
{
	class FreezeTime : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Game1), "shouldTimePass");

		public static bool TimeFrozen { get; set; } = false;
		
		public static bool Postfix(bool __result) => TimeFrozen ? false : __result;
	}
}
