/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace PortableBridges.TerrainFeatures
{
    public class BridgeObject : Fence
    {
        // public readonly NetInt whichFloor = new NetInt();
        // public readonly NetInt whichView = new NetInt();
        // public readonly NetBool isPathway = new NetBool();
        // public readonly NetBool isSteppingStone = new NetBool();
        // public readonly NetBool drawContouredShadow = new NetBool();
        // public readonly NetBool cornerDecoratedBorders = new NetBool();

        public BridgeObject()
        {
            this.Type = "TODO: REPLACE ME";
        }

        public override void actionOnPlayerEntry()
        {
            Game1.player.ignoreCollisions = true;
        }
    }
}
