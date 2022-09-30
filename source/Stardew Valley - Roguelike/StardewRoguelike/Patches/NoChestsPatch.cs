/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley.Locations;

namespace StardewRoguelike.Patches
{
	internal class NoChestsPatch : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "addLevelChests");

		public static bool Prefix()
		{
			return false;
		}
	}
}
