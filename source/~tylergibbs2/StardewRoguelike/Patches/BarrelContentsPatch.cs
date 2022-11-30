/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

namespace StardewRoguelike.Patches
{
    internal class BarrelContentsPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(BreakableContainer), "releaseContents");

        public static bool Prefix(BreakableContainer __instance, GameLocation location, Farmer who)
        {
			int x = (int)__instance.TileLocation.X;
			int y = (int)__instance.TileLocation.Y;
			if (location is MineShaft mine)
			{
				if (mine.isContainerPlatform(x, y))
					mine.updateMineLevelData(0, -1);

                var (itemId, quantity) = Roguelike.GetBarrelDrops(mine);
                if (itemId == 1000)
                {
                    Item toDrop = new FishingRod();
                    location.debris.Add(new Debris(toDrop, new Vector2(x * 64 + 32, y * 64 + 32), Game1.player.getStandingPosition()));
                }
                else if (itemId > 0 && quantity > 0)
                    Game1.createMultipleObjectDebris(itemId, x, y, quantity, location);
            }

            if (who == Game1.player && who.get_FarmerActiveHatQuest() is not null)
                who.get_FarmerActiveHatQuest()!.BarrelsDestroyed++;

			return false;
        }
    }
}
