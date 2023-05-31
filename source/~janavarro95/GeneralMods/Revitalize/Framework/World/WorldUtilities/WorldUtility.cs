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
using Omegasis.Revitalize.Framework.World.Debris;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Farming;
using StardewValley;
using StardewValley.Objects;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{
    public static class WorldUtility
    {

        public static void InitializeGameWorld()
        {
            CleanUpGameWorldOnLoad();
        }


        //TODO: REDO???? THIS CODE TO SEE IF THERE IS DUPLICATE ENTRIES FOR OBJECTS PLACED INTO THE WORLD????

        /// <summary>
        /// Cleans up an object's references on loading a game save. Necessary due to the hack that CustomObjects are considered furniture and StardewValley.Objects...
        /// </summary>
        public static void CleanUpGameWorldOnLoad()
        {

            foreach (GameLocation location in Game1.locations)
            {
                List<CustomObject> objectsToCleanUp = new List<CustomObject>();
                List<CustomObject> furnitureToCleanUp = new List<CustomObject>();
                foreach (StardewValley.Object obj in location.objects.Values)
                {

                    if (obj is CustomObject)
                    {
                        CustomObject customObj = (CustomObject)obj;

                        if (location.objects.ContainsKey(customObj.TileLocation))
                        {
                            //RevitalizeModCore.log("Clean up from loading: {0}", true, customObj.basicItemInformation.id);

                            objectsToCleanUp.Add(customObj);


                            //customObj.removeFromGameWorld(customObj.TileLocation, location);

                            //Furniture f = GetFurnitureEquivalentPieceAtLocation(location, customObj);
                            //location.furniture.Remove(f);
                        }
                    }
                }
                foreach (Furniture f in location.furniture)
                {
                    if (f is CustomObject)
                    {
                        furnitureToCleanUp.Add((f as CustomObject));
                    }
                }

                foreach (CustomObject obj in objectsToCleanUp)
                {
                    location.objects.Remove(obj.TileLocation);
                    //location.objects.Add(obj.TileLocation,obj);
                }
                foreach (CustomObject obj in furnitureToCleanUp)
                {
                    location.furniture.Remove(obj);
                    //location.furniture.Add(obj);
                }

                foreach (CustomObject obj in furnitureToCleanUp)
                {
                    obj.reAddToGameWorld(obj.TileLocation, location);
                }


                //READD FURNITURE AND CUSTOM OBJECTS

                /*



                foreach (CustomObject obj in objectsToCleanUp)
                {
                    Furniture f = GetFurnitureEquivalentPieceAtLocation(location, obj);
                    bool furnitureContains = f != null;
                    if (location.objects.ContainsKey(obj.TileLocation) == false && furnitureContains == false)
                    {

                        //RevitalizeModCore.log("Add object back to game world since it doesn't exist in object or furniture list: {0}", false, obj.basicItemInformation.id);

                        obj.reAddToGameWorld(obj.TileLocation, location);
                        continue;
                    }

                    else if (location.objects.ContainsKey(obj.TileLocation) == false && furnitureContains == true)
                    {
                        //RevitalizeModCore.log("Add object back to game world since it doesn't exist in object list but it does exist in the furniture list: {0}", false, obj.basicItemInformation.id);
                        location.furniture.Remove(f);
                        obj.reAddToGameWorld(obj.TileLocation, location);
                        continue;

                    }

                    else if (location.objects.ContainsKey(obj.TileLocation) == true && furnitureContains == false)
                    {
                        //RevitalizeModCore.log("Object was NOT removed, but DOES NOT exist in the furniture's database: {0}", true, obj.basicItemInformation.id);
                    }
                    else
                    {
                        //RevitalizeModCore.log("Unsure what should happen here", true, obj.basicItemInformation.id);
                        StardewValley.Object overlappedObject = location.objects[obj.TileLocation];
                        if (overlappedObject is CustomObject)
                        {
                            CustomObject overlappedCustomObject = (CustomObject)overlappedObject;
                            if (overlappedCustomObject.basicItemInformation.id.Equals(obj.basicItemInformation.id))
                            {

                                //RevitalizeModCore.log("Purge due to duplication: {0}", true, obj.basicItemInformation.id);
                                continue;
                            }
                            else
                            {
                                obj.reAddToGameWorld(obj.TileLocation, location);
                            }
                        }

                    }
                }
                */
            }

        }


        public static Furniture GetFurnitureEquivalentPieceAtLocation(this GameLocation environment, CustomObject obj)
        {
            foreach (Furniture f in environment.furniture)
            {
                if (f is CustomObject)
                {

                    CustomObject customObject = (CustomObject)f;

                    if (customObject.TileLocation.Equals(obj.TileLocation) && customObject.basicItemInformation.id.Equals(obj.basicItemInformation.id))
                    {
                        return f;
                    }

                }
            }
            return null;
        }

        public static void RemoveFurnitureAtTileLocation(this GameLocation environment, Vector2 TileLocation)
        {
            List<Furniture> furnitureToRemove = new List<Furniture>();
            foreach (Furniture f in environment.furniture)
            {
                if (f is CustomObject)
                {

                    CustomObject customObject = (CustomObject)f;

                    if (customObject.TileLocation.Equals(TileLocation))
                    {
                        furnitureToRemove.Add(f);
                    }

                }
            }

            foreach (Furniture f in furnitureToRemove)
            {
                environment.furniture.Remove(f);
            }
        }

        public static void RemoveFurnitureIntersectingTileLocation(GameLocation environment, Vector2 TileLocation)
        {
            List<Furniture> furnitureToRemove = new List<Furniture>();
            foreach (Furniture f in environment.furniture)
            {
                if (f is CustomObject)
                {

                    CustomObject customObject = (CustomObject)f;

                    bool contains = customObject.boundingBox.Value.Contains(TileLocation * 64);

                    if (contains)
                    {
                        furnitureToRemove.Add(f);
                    }

                }
            }

            foreach (Furniture f in furnitureToRemove)
            {
                environment.furniture.Remove(f);
            }
        }

        /// <summary>
        /// Creates <see cref="CustomObjectDebris"/> at the given location.
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="item"></param>
        /// <param name="OriginTile"></param>
        public static void CreateItemDebrisAtTileLocation(this GameLocation Location, Item item, Vector2 OriginTile)
        {
            Location.debris.Add(new CustomObjectDebris(item, OriginTile * 64));
        }

        /// <summary>
        /// Creates <see cref="CustomObjectDebris"/> at the given location.
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="item"></param>
        /// <param name="OriginTile"></param>
        /// <param name="DestinationTile"></param>
        public static void CreateItemDebrisAtTileLocation(this GameLocation Location, Item item, Vector2 OriginTile, Vector2 DestinationTile)
        {
            Location.debris.Add(new CustomObjectDebris(item, OriginTile * 64, DestinationTile * 64));
        }

        /// <summary>
        /// Checks to see if minecarts have been unlocked for the world.
        /// </summary>
        /// <returns></returns>
        public static bool AreMinecartsUnlocked()
        {
            if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of minerals donated to the museum.
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfMineralsDonatedToMuseum()
        {
            return NumberOfMuseumItemsOfType("Minerals");
        }

        public static int NumberOfMuseumItemsOfType(string type)
        {
            int num = 0;
            foreach (KeyValuePair<Vector2, int> pair in Game1.netWorldState.Value.MuseumPieces.Pairs)
            {
                if (Game1.objectInformation[pair.Value].Split('/')[3].Contains(type))
                {
                    num++;
                }
            }

            return num;
        }

    }
}
