/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using Microsoft.Xna.Framework;

namespace ChestFeatureSet.Framework.CFSChest
{
    public class CFSChest
    {
        public Vector2 ChestTileLocation { get; }
        public string LocationName { get; }

        public CFSChest(Vector2 chestTileLocation, string locationName)
        {
            this.ChestTileLocation = chestTileLocation;
            this.LocationName = locationName;
        }
    }

    public class SaveCFSChest
    {
        public readonly IEnumerable<CFSChest> Chests;

        public SaveCFSChest(IEnumerable<CFSChest> chests)
        {
            this.Chests = chests;
        }
    }
}
