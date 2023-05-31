/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects;
using StardewValley;
using StardewValley.Objects;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{
    public static class ObjectUtilities
    {
        /// <summary>
        /// Gets the objects that are at adjacent tile positions (and potentially including this one!).
        /// </summary>
        /// <param name="location"></param>
        /// <param name="StartingPosition"></param>
        /// <param name="IncludeStartingTilePosition"></param>
        /// <returns></returns>
        public static Dictionary<Vector2,StardewValley.Object> GetAllAdjacentObjectsOrNull(GameLocation location, Vector2 StartingPosition)
        {
            Dictionary<Vector2, StardewValley.Object> adjacentObjects = new Dictionary<Vector2, StardewValley.Object>();

            //Can return null if no object at position.
            for (int x = (int)StartingPosition.X - 1; x <= (int)StartingPosition.X + 1; x++)
            {
                for (int y = (int)StartingPosition.Y - 1; y <= (int)StartingPosition.Y + 1; y++)
                {

                    StardewValley.Object obj = location.getObjectAtTile(x, y);
                    Vector2 tilePosition = new Vector2(x, y);
                    adjacentObjects.Add(tilePosition, obj);

                }
            }
            return adjacentObjects;

        }

        public static Dictionary<Vector2, StardewValley.Object> GetAllConnectedObjectsStartingAtTilePosition(GameLocation location,Vector2 TilePosition)
        {

            HashSet<Vector2> exploredTiles = new HashSet<Vector2>();
            Dictionary<Vector2, StardewValley.Object> connectedObjects = new Dictionary<Vector2, StardewValley.Object>();

            //get starting object,
            //get list of connected adjacent objects.

            Queue<Vector2> tilesToExplore = new Queue<Vector2>();
            tilesToExplore.Enqueue(TilePosition);

            while (tilesToExplore.Count > 0)
            {
                Vector2 tileToExplore = tilesToExplore.Dequeue();

                if (exploredTiles.Contains(tileToExplore))
                {
                    continue;
                }

                Dictionary<Vector2, StardewValley.Object> objects = GetAllAdjacentObjectsOrNull(location, tileToExplore);
                foreach(KeyValuePair<Vector2,StardewValley.Object> tileToObject in objects)
                {
                    if (tileToObject.Value == null)
                    {
                        continue;
                    }
                    if (exploredTiles.Contains(tileToExplore))
                    {
                        continue;
                    }
                    if (connectedObjects.ContainsKey(tileToObject.Key)){
                        continue;
                    }

                    connectedObjects.Add(tileToObject.Key,tileToObject.Value);
                    tilesToExplore.Enqueue(tileToObject.Key);

                }
                exploredTiles.Add(tileToExplore);

            }
            return connectedObjects;


        }

        /// <summary>
        /// Adds an item to a chest. Returns true if the item was added, or false if it was not added.
        /// </summary>
        /// <param name="chest">The chest to add the item to.</param>
        /// <param name="item">The item to add.</param>
        /// <returns></returns>
        public static bool addItemToChest(Chest chest, Item item)
        {

            for(int i = 0; i < chest.GetActualCapacity(); i++)
            {

                if (i >= chest.items.Count())
                {
                    chest.addItem(item);
                    return true;
                }


                Item chestItemSlot = chest.items[i];
                if (chestItemSlot == null)
                {
                    chest.items[i] = item;
                    return true;
                }

                if (item.canStackWith(chestItemSlot))
                {
                    chestItemSlot.Stack += item.Stack;
                    return true;
                }

            }

            return false;

        }

        /// <summary>
        /// Reduces a given item in a chest and cleans up the chest as necessary.
        /// </summary>
        /// <param name="chest">The chest to modify.</param>
        /// <param name="itemIndex">The index of the item in the chest.</param>
        /// <returns>True if the operation was suscessful. False if there was no item to modify.</returns>
        public static bool ReduceChestItemStackByAmountAndRemoveIfNecessary(Chest chest, int itemIndex, int AmountToSubtract)
        {
            if(itemIndex>=chest.GetActualCapacity() || itemIndex>= chest.items.Count)
            {
                return false;
            }

            Item item = chest.items[itemIndex];
            if (item == null)
            {
                return false;
            }

            item.Stack-=AmountToSubtract;

            if (item.Stack <= 0)
            {
                chest.items[itemIndex] = null;
                chest.clearNulls();
            }
            return true;


        }

        /// <summary>
        /// Gets the type of footstep the player is making.
        /// </summary>
        /// <returns></returns>
        public static Enums.FloorType GetFloorType(CustomObject customObject)
        {
            if (customObject.getCurrentLocation() == null) return Enums.FloorType.NULL;
            GameLocation location = customObject.getCurrentLocation();
            if (location.IsOutdoors || location.Name.ToLower().Contains("mine") || location.Name.ToLower().Contains("cave") || location.IsGreenhouse)
            {
                Vector2 tileLocationOfObject = customObject.TileLocation;
                string stepType = location.doesTileHaveProperty((int)tileLocationOfObject.X, (int)tileLocationOfObject.Y, "Type", "Buildings");
                if (stepType == null || stepType.Length < 1)
                {
                    stepType = location.doesTileHaveProperty((int)tileLocationOfObject.X, (int)tileLocationOfObject.Y, "Type", "Back");
                }
                switch (stepType)
                {
                    case "Dirt":
                        return Enums.FloorType.SandOrDirt;
                    case "Stone":
                        return Enums.FloorType.Stone;
                    case "Grass":
                        return Enums.FloorType.Grass;
                    case "Wood":
                        return Enums.FloorType.Wood;
                    default: return Enums.FloorType.Default;

                }
            }
            else
            {
                return Enums.FloorType.Default;
            }
        }

        /// <summary>
        /// Gets a Stardew Valley object from the predefined enum.
        /// </summary>
        /// <param name="sdvObject"></param>
        /// <param name="InitialStack"></param>
        /// <returns></returns>
        public static StardewValley.Object getStardewObjectFromEnum(Enums.SDVObject sdvObject, int InitialStack = 1)
        {
            return new StardewValley.Object((int)sdvObject, InitialStack);
        }

        public static bool IsObjectHoldingItem(StardewValley.Object obj)
        {
            if (obj.heldObject.Value != null) return true;
            else return false;
        }

        /// <summary>
        /// Checks to see if the given object is a SDV vanilla furnace.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsObjectFurnace(StardewValley.Object obj)
        {
            if (obj.ParentSheetIndex == 13 && obj.bigCraftable.Value && obj.Category == -9 && obj.Name == "Furnace")
                return true;
            else return false;
        }

        /// <summary>
        /// Gets a dimension offset depending on the size of the object passed in.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Vector2 GetDimensionOffsetFromItem(StardewValley.Object obj)
        {
            if (TypeUtilities.IsSameType(typeof(StardewValley.Object), obj.GetType()))
                return new Vector2(64f, 64f);

            return new Vector2(64f, 64f);
        }

        /// <summary>
        /// Gets the height of an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetHeightOffsetFromItem(StardewValley.Object obj)
        {
            return GetDimensionOffsetFromItem(obj).Y;
        }

        /// <summary>
        /// Gets the width of an item.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetWidthOffsetFromItem(StardewValley.Object obj)
        {
            return GetDimensionOffsetFromItem(obj).X;
        }

    }
}
