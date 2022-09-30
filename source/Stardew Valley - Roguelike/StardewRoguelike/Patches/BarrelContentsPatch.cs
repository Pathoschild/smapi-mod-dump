/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StardewRoguelike.Patches
{
    internal class BarrelContentsPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(BreakableContainer), "releaseContents");

        public static bool Prefix(BreakableContainer __instance, GameLocation location)
        {
			int x = (int)__instance.TileLocation.X;
			int y = (int)__instance.TileLocation.Y;
			if (location is MineShaft mine)
			{
				if (mine.isContainerPlatform(x, y))
					mine.updateMineLevelData(0, -1);

                var (itemId, quantity) = Roguelike.GetBarrelDrops(mine);
                if (itemId > 0 && quantity > 0)
                    Game1.createMultipleObjectDebris(itemId, x, y, quantity, location);
            }

			return false;
        }
    }
}
