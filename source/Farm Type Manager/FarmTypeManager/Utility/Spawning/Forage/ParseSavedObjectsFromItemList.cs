/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Parses a "raw" list of Items into a new list of SavedObjects, excluding any that aren't successfully parsed.</summary>
            /// <param name="rawObjects">A list of objects, each describing a specific "kind" of Item.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A list of SavedObjects representing each successfully parsed item.</returns>
            public static List<SavedObject> ParseSavedObjectsFromItemList(IEnumerable<object> rawItems, string areaID = "")
            {
                List<SavedObject> SavedObjects = new List<SavedObject>();

                foreach (object raw in rawItems) //for each object in the raw list
                {
                    if (raw is long rawLong) //if this is the ID of a StardewValley.Object
                    {
                        SavedObject saved = null;
                        try
                        {
                            int objectID = Convert.ToInt32(rawLong); //parse the number into a 32-bit integer
                            saved = CreateSavedObject(objectID, areaID); //use the parsed ID to create a saved object
                        }
                        catch //if parsing caused an exception
                        {
                            Monitor.Log($"An area's item list contains a number that could not be parsed correctly.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                            Monitor.Log($"This may be caused by a formatting error in the item list. The affected item will be skipped.", LogLevel.Info);
                        }

                        if (saved != null) //if parsing was successful
                        {
                            SavedObjects.Add(saved); //add this to the list
                        }
                    }
                    else if (raw is string rawString) //if this is a string
                    {
                        SavedObject saved = null;

                        if (int.TryParse(rawString, out int objectID)) //if this string is the ID of a StardewValley.Object
                        {
                            saved = CreateSavedObject(objectID, areaID); //use the parsed ID to create a saved object
                        }
                        else //if this string is the name of a StardewValley.Object
                        {
                            saved = CreateSavedObject(rawString, areaID); //use the string to create a saved object
                        }

                        if (saved != null) //if parsing was successful
                        {
                            SavedObjects.Add(saved); //add this to the list
                        }

                    }
                    else if (raw is JObject rawObj) //if this is a ConfigItem
                    {
                        SavedObject saved = null;
                        ConfigItem item = null;
                        try
                        {
                            item = rawObj.ToObject<ConfigItem>(); //attempt to parse this into a ConfigItem
                            saved = CreateSavedObject(item, areaID); //use the item to create a saved object
                        }
                        catch
                        {
                            Monitor.Log($"An area's item list contains a complex item that could not be parsed properly.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                            Monitor.Log($"This may be caused by a formatting error in the item list. The affected item will be skipped.", LogLevel.Info);
                        }
                        
                        if (saved != null) //if parsing was successful
                        {
                            SavedObjects.Add(saved); //add this to the list
                        }
                    }
                    else //the object doesn't match any known types
                    {
                        Monitor.Log($"An area's item list contains an unrecognized item format.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                        Monitor.Log($"This may be caused by a formatting error in the item list. The affected item will be skipped.", LogLevel.Info);
                    }
                }

                return SavedObjects;
            }

            /// <summary>Uses an integer to create a saved object respresenting a StardewValley.Object.</summary>
            /// <param name="objectID">The object's ID, also known as index or parentSheetIndex.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A saved object representing the designated StardewValley.Object. Null if creation failed.</returns>
            private static SavedObject CreateSavedObject(int objectID, string areaID = "")
            {
                IDictionary<int, string> objDictionary = GetItemDictionary("object"); //get currently loaded object data
                if (objDictionary.ContainsKey(objectID)) //if data exists for this object ID
                {
                    SavedObject saved = new SavedObject() //generate a saved object with the appropriate type and ID
                    {
                        Type = SavedObject.ObjectType.Object,
                        ID = objectID
                    };
                    Monitor.VerboseLog($"Parsed integer object ID: {objectID}");
                    return saved;
                }
                else //if no data exists for this object ID
                {
                    Monitor.Log($"An area's item list contains an object ID that did not match any loaded objects.", LogLevel.Info);
                    Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                    Monitor.Log($"Object ID: \"{objectID}\"", LogLevel.Info);
                    Monitor.Log($"This may be caused by an error in the item list or a modded object that wasn't loaded. The affected object will be skipped.", LogLevel.Info);
                    return null;
                }
            }

            /// <summary>Uses a string to create a saved object representing a StardewValley.Object.</summary>
            /// <param name="objectName">The name of the object.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A saved object representing the designated StardewValley.Object. Null if creation failed.</returns>
            private static SavedObject CreateSavedObject(string objectName, string areaID = "")
            {
                int? objectID = GetItemID("object", objectName); //get an object ID for this name

                if (objectID.HasValue) //if a matching object ID was found
                {
                    SavedObject saved = new SavedObject() //generate a saved object with the appropriate type, ID, and name
                    {
                        Type = SavedObject.ObjectType.Object,
                        Name = objectName,
                        ID = objectID.Value
                    };
                    Monitor.VerboseLog($"Parsed \"{objectName}\" into object ID: {objectID}");
                    return saved;
                }
                else //if no matching object ID was found
                {
                    Monitor.Log($"An area's item list contains an object name that did not match any loaded objects.", LogLevel.Info);
                    Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                    Monitor.Log($"Object name: \"{objectName}\"", LogLevel.Info);
                    Monitor.Log($"This may be caused by an error in the item list or a modded object that wasn't loaded. The affected object will be skipped.", LogLevel.Info);
                    return null;
                }
            }

            /// <summary>Uses a ConfigItem to create a saved object representing an item.</summary>
            /// <param name="item">The ConfigItem class describing the item.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A saved object representing the designated item. Null if creation failed.</returns>
            private static SavedObject CreateSavedObject(ConfigItem item, string areaID = "")
            {
                switch (item.Type)
                {
                    case SavedObject.ObjectType.Object:
                    case SavedObject.ObjectType.Item:
                    case SavedObject.ObjectType.Container:
                        //these are valid item types
                        break;
                    default:
                        Monitor.Log($"An area's item list contains a complex item with a type that is not recognized.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                        Monitor.Log($"Item type: \"{item.Type}\"", LogLevel.Info);
                        Monitor.Log($"This is likely due to a design error in the mod's code. Please report this to the mod's developer. The affected item will be skipped.", LogLevel.Info);
                        return null;
                }

                if (item.Contents != null) //if this item has contents
                {
                    for (int x = item.Contents.Count - 1; x >= 0; x--) //for each of the contents
                    {
                        List<SavedObject> contentSave = ParseSavedObjectsFromItemList(new object[] { item.Contents[x] }, areaID); //attempt to parse this into a saved object
                        if (contentSave.Count <= 0) //if parsing failed
                        {
                            item.Contents.RemoveAt(x); //remove this from the contents list
                        }
                    }
                }

                if (item.Type == SavedObject.ObjectType.Container) //if this is a container
                {
                    //containers have no name or ID to validate, so don't involve them

                    SavedObject saved = new SavedObject() //generate a saved object with these settings
                    {
                        Type = item.Type,
                        ConfigItem = item
                    };
                    Monitor.VerboseLog($"Parsed \"{item.Category}\" as a container type.");
                    return saved;
                }

                string savedName = item.Category + ":" + item.Name;

                int? itemID = GetItemID(item.Category, item.Name); //get an item ID for the category and name
                if (itemID.HasValue) //if a matching item ID was found
                {
                    SavedObject saved = new SavedObject() //generate a saved object with these settings
                    {
                        Type = item.Type,
                        Name = savedName,
                        ID = itemID.Value,
                        ConfigItem = item
                    };
                    Monitor.VerboseLog($"Parsed \"{item.Category}\": \"{item.Name}\" into item ID: {itemID}");
                    return saved;
                }
                else //if no matching item ID was found
                {
                    Monitor.Log($"An area's item list contains a complex item definition that did not match any loaded items.", LogLevel.Info);
                    Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                    Monitor.Log($"Item name: \"{savedName}\"", LogLevel.Info);
                    Monitor.Log($"This may be caused by an error in the item list or a modded item that wasn't loaded. The affected item will be skipped.", LogLevel.Info);
                    return null;
                }
            }
        }
    }
}