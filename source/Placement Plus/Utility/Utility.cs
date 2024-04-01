/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

namespace PlacementPlus.Utility
{
    public static class Utility {
        // Bi-directional mapping between flooring types and their respective item's qualified item id.
        // Note that the flooring itself would instead use ItemRegistry.type_floorpaper (FL).
        public static readonly BiDictionary<string> FlooringInfoMap = new() {
            { Flooring.wood,                "(O)328" },
            { Flooring.stone,               "(O)329" },
            { Flooring.ghost,               "(O)331" },
            { Flooring.iceTile,             "(O)333" },
            { Flooring.straw,               "(O)401" },
            { Flooring.gravel,              "(O)407" },
            { Flooring.boardwalk,           "(O)405" },
            { Flooring.colored_cobblestone, "(O)409" },
            { Flooring.cobblestone,         "(O)411" },
            { Flooring.steppingStone,       "(O)415" },
            { Flooring.brick,               "(O)293" },
            { Flooring.plankFlooring,       "(O)840" },
            { Flooring.townFlooring,        "(O)841" },
        };

        // Qualified IDs for fence items.
        public static class FenceType {
            public const string WoodFence     = "(O)322";
            public const string StoneFence    = "(O)323";
            public const string IronFence     = "(O)324";
            public const string Gate          = "(O)325";
            public const string HardwoodFence = "(O)298";
        }

        private static readonly HashSet<string> FenceTypes = new() {
            FenceType.WoodFence, FenceType.StoneFence, FenceType.IronFence, FenceType.Gate, FenceType.HardwoodFence,
        };



        /// <summary>
        /// Returns <c>true</c> if flooring exists as a <see cref="TerrainFeature">TerrainFeature</see> at the specified
        /// tile; otherwise, <c>false</c>.
        /// </summary>
        public static bool DoesTileHaveFlooring(
            NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures, Vector2 tilePos)
        {
            return terrainFeatures.ContainsKey(tilePos) && terrainFeatures[tilePos] is Flooring;
        }



        /// <summary> Returns <c>true</c> if <c>item</c> is flooring; otherwise, <c>false</c>. </summary>
        public static bool IsItemFlooring(Item item) =>
            item != null && FlooringInfoMap.ContainsKey(item.QualifiedItemId);
        

        
        /// <summary> Returns <c>true</c> if <c>item</c> is the target flooring; otherwise, <c>false</c>. </summary>
        public static bool IsItemTargetFlooring(Item item, Flooring flooring) =>
            item != null && item.QualifiedItemId == FlooringInfoMap[flooring.whichFloor.Value];



        /// <summary> Returns <c>true</c> if <c>item</c> is a fence gate; otherwise, <c>false</c>. </summary>
        // We either cast the item as Fence and check if it's a gate (for Objects), or compare the item's
        // ParentSheetIndex with the index for gates (for Items).
        public static bool IsItemGate(Item item) => item != null &&
            ((item as Fence)?.isGate.Value ?? item.QualifiedItemId == FenceType.Gate);



        /// <summary> Returns <c>true</c> if <c>item</c> is a fence; otherwise, <c>false</c>. </summary>
        public static bool IsItemFence(Item item) =>
            item != null && (FenceTypes.Contains(item.QualifiedItemId) || item is Fence);
        
        
        
        /// <summary> Returns <c>true</c> if <c>item</c> is the target fence; otherwise, <c>false</c>. </summary>
        public static bool IsItemTargetFence(Item item, Fence fence) =>
            item.Name == fence.Name && !fence.isGate.Value;

        
        
        /// <summary>
        /// Returns <c>true</c> if the main farmhouse's mailbox lies on the tile; otherwise, <c>false</c>.
        /// </summary>
        public static bool DoesTileHaveMainMailbox(Farm farm, Vector2 tilePos) =>
            new Rectangle(tilePos.ToPoint(), new Point(1, 2)).Contains(farm.GetMainMailboxPosition());
        
        
        
        /// <summary>
        /// Returns <c>true</c> if a building interactable (i.e. mailbox, door, or shipping bin) lies on the tile;
        /// otherwise, <c>false</c>.
        /// </summary>
        public static bool IsTileOnBuildingInteractable(Farm farm, Vector2 tilePos)
        {
            var _ = ""; // Throwaway variable for Building.doesTileHaveProperty reference argument.
            var tile = tilePos.ToPoint();
            var building = farm.getBuildingAt(tilePos);
            // If there is no building at the tile, check if the tile intersects the main mailbox.
            if (building == null) return DoesTileHaveMainMailbox(farm, tilePos);

            // Check for cabin mailboxes/doors/shipping bins.
            var tileHasMailbox = building.doesTileHaveProperty(tile.X, tile.Y, "Mailbox", "Buildings", ref _);
            var tileHasDoor = building.getPointForHumanDoor() == tile || building.getRectForAnimalDoor().Contains(64 * tilePos);
            var tileHasShippingBin = building is ShippingBin sb && sb.occupiesTile(tilePos);

            return tileHasMailbox || tileHasDoor || tileHasShippingBin;
        }



        /// <summary>
        /// Returns <c>true</c> if the tile border of a farm building lies on the tile; otherwise, <c>false</c>.
        /// </summary>
        public static bool IsTileOnBuildingEdge(Farm farm, Vector2 tilePos)
        {
            var building = farm.getBuildingAt(tilePos);
            // If there is no building at the tile, check if the tile intersects the edge of the main farmhouse.
            building ??= farm.GetMainFarmHouse();
            
            // Check if the tile intersects the edge of the building.
            var buildingRect = new Rectangle(building.tileX.Value, building.tileY.Value, building.tilesWide.Value, building.tilesHigh.Value);
            return IntersectsRectEdge(buildingRect, tilePos);

            static bool IntersectsRectEdge(Rectangle rect, Vector2 tilePos)
            {
                var innerRect = rect; innerRect.Inflate(-1, -1);
                return rect.Contains(tilePos) && !innerRect.Contains(tilePos);
            }
        }
    }
}
