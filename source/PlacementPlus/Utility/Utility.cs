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

using Object = StardewValley.Object;

namespace PlacementPlus.Utility
{
    public static class Utility
    {
        // enum mapping to flooring types--the order represented matters! 
        public enum FlooringType 
        { 
            Wood_floor, Stone_floor, Weathered_floor, Crystal_floor, Straw_floor, Gravel_floor, Rustic_plank_floor, 
            Crystal_path, Cobblestone_path, Stepping_stone_path, Brick_floor, Wood_path, Stone_walkway_floor,
        }
        // Bi-directional mapping between flooring types and their respective item's ParentSheetIndex.
        public static readonly BiDictionary<int> FlooringInfoMap = new() {
            { (int) FlooringType.Wood_floor,          328 },
            { (int) FlooringType.Stone_floor,         329 },
            { (int) FlooringType.Weathered_floor,     331 },
            { (int) FlooringType.Crystal_floor,       333 },
            { (int) FlooringType.Straw_floor,         401 },
            { (int) FlooringType.Gravel_floor,        407 },
            { (int) FlooringType.Rustic_plank_floor,  405 },
            { (int) FlooringType.Crystal_path,        409 },
            { (int) FlooringType.Cobblestone_path,    411 },
            { (int) FlooringType.Stepping_stone_path, 415 },
            { (int) FlooringType.Brick_floor,         293 },
            { (int) FlooringType.Wood_path,           840 },
            { (int) FlooringType.Stone_walkway_floor, 841 },
        };

        // enum mapping to fence types--the order represented matters! 
        public enum FenceType { Wood_fence = 1, Stone_fence, Iron_fence, Gate, Hardwood_fence, }
        // Bi-directional mapping between fence types and their respective item's ParentSheetIndex.
        public static readonly BiDictionary<int> FenceInfoMap = new()
        {
            { (int) FenceType.Wood_fence,     322 },
            { (int) FenceType.Stone_fence,    323 },
            { (int) FenceType.Iron_fence,     324 },
            { (int) FenceType.Gate,           325 },
            { (int) FenceType.Hardwood_fence, 298 },
        };
        
        // enum mapping to chest types.
        public enum ChestType { Chest, Stone_chest, }
        // Bi-directional mapping between chest types and their respective item's ParentSheetIndex.
        public static readonly BiDictionary<int> ChestInfoMap = new()
        {
            { (int) ChestType.Chest,       130 },
            { (int) ChestType.Stone_chest, 232 },
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
        // ? Does the furniture category only cover flooring?
        public static bool IsItemFlooring(Item item) =>
            item.Category == Object.furnitureCategory;
        

        
        
        /// <summary> Returns <c>true</c> if <c>item</c> is the target flooring; otherwise, <c>false</c>. </summary>
        public static bool IsItemTargetFlooring(Item item, Flooring flooring) =>
            item.ParentSheetIndex == FlooringInfoMap[flooring.whichFloor.Value];
        


        /// <summary> Returns <c>true</c> if <c>item</c> is a chest; otherwise, <c>false</c>. </summary>
        public static bool IsItemChest(Item item) =>
            item != null && ChestInfoMap.ContainsKey(item.ParentSheetIndex);



        /// <summary> Returns <c>true</c> if <c>item</c> is a fence gate; otherwise, <c>false</c>. </summary>
        // We either cast the item as Fence and check if it's a gate (for Objects), or compare the item's
        // ParentSheetIndex with the index for gates (for Items).
        public static bool IsItemGate(Item item)
        {
            return item != null && 
                   ((item as Fence)?.isGate.Value ?? item.ParentSheetIndex == FenceInfoMap[(int)FenceType.Gate]);
        }



        /// <summary> Returns <c>true</c> if <c>item</c> is a fence; otherwise, <c>false</c>. </summary>
        public static bool IsItemFence(Item item) =>
            item != null && (FenceInfoMap.ContainsKey(item.ParentSheetIndex) || item is Fence);
        
        
        
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
            var tileHasDoor = building.getPointForHumanDoor() == tile || 
                              building.getRectForAnimalDoor().Contains(64 * tilePos);
            var tileHasShippingBin = building is ShippingBin sb && sb.occupiesTile(tilePos);

            return tileHasMailbox || tileHasDoor || tileHasShippingBin;
        }



        /// <summary>
        /// Returns <c>true</c> if the tile border of a farm building lies on the tile; otherwise, <c>false</c>.
        /// </summary>
        public static bool IsTileOnBuildingEdge(Farm farm, Vector2 tilePos)
        {
            static bool IntersectsRectEdge(Rectangle rect, Vector2 tilePos)
            {
                var innerRect = rect; innerRect.Inflate(-1, -1);
                return rect.Contains(tilePos) && !innerRect.Contains(tilePos);
            }
            

            var building = farm.getBuildingAt(tilePos);
            
            // If there is no building at the tile, check if the tile intersects the edge of the main farmhouse.
            if (building == null) return IntersectsRectEdge(farm.GetHouseRect(), tilePos);
            
            // Check if the tile intersects the edge of the building.
            var buildingRect = new Rectangle(
                building.tileX.Value, building.tileY.Value, building.tilesWide.Value, building.tilesHigh.Value);
            return IntersectsRectEdge(buildingRect, tilePos);
        }
    }
}