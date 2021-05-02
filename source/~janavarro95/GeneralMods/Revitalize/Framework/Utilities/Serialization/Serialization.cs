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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.Furniture;
using Revitalize.Framework.Utilities.Serialization.ContractResolvers;
using Revitalize.Framework.Utilities.Serialization.Converters;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Utilities
{
    /// <summary>
    /// Handles serialization of all objects in existence.
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// The actual json serializer.
        /// </summary>
        private JsonSerializer serializer;

        /// <summary>
        /// All files to be cleaned up after loading.
        /// </summary>
        private Dictionary<string, List<string>> filesToDelete = new Dictionary<string, List<string>>();

        /// <summary>
        /// The items to remove for deletion.
        /// </summary>
        private List<Item> itemsToRemove = new List<Item>();

        /// <summary>
        /// The settings used by the seralizer
        /// </summary>
        private JsonSerializerSettings settings;


        public static NetFieldConverter NetFieldConverter;
        /// <summary>
        /// Constructor.
        /// </summary>
        public Serializer()
        {
            this.serializer = new JsonSerializer();
            this.serializer.Formatting = Formatting.Indented;
            this.serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this.serializer.NullValueHandling = NullValueHandling.Include;

            this.serializer.ContractResolver = new NetFieldContract();
            NetFieldConverter = new NetFieldConverter();

            this.addConverter(new Framework.Utilities.Serialization.Converters.RectangleConverter());
            this.addConverter(new Framework.Utilities.Serialization.Converters.Texture2DConverter());
            this.addConverter(new Framework.Utilities.Serialization.Converters.ItemCoverter());
            this.addConverter(NetFieldConverter);
            //this.addConverter(new Framework.Utilities.Serialization.Converters.CustomObjectDataConverter());
            //this.addConverter(new Framework.Utilities.Serialization.Converters.NetFieldConverter());
            //this.addConverter(new Framework.Utilities.Serialization.Converters.Vector2Converter());

            //this.gatherAllFilesForCleanup();

            this.settings = new JsonSerializerSettings();
            foreach (JsonConverter converter in this.serializer.Converters)
            {
                this.settings.Converters.Add(converter);
            }
            this.settings.Formatting = Formatting.Indented;
            this.settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this.settings.NullValueHandling = NullValueHandling.Include;
            this.settings.ContractResolver = new NetFieldContract();
        }

        /// <summary>
        /// Process all the save data for objects to be deleted by this mod.
        /// </summary>
        private void gatherAllFilesForCleanup()
        {
            if (!Directory.Exists(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"))) Directory.CreateDirectory(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            this.filesToDelete.Clear();
            string[] directories = Directory.GetDirectories(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            foreach (string playerData in directories)
            {
                string objectPath = Path.Combine(playerData, "SavedObjectInformation");
                string[] objectFiles = Directory.GetFiles(objectPath);
                foreach (string file in objectFiles)
                {
                    string playerName = new DirectoryInfo(objectPath).Parent.Name;
                    if (this.filesToDelete.ContainsKey(playerName))
                    {
                        this.filesToDelete[playerName].Add(file);
                        //Revitalize.ModCore.log("Added File: " + file);
                    }
                    else
                    {
                        this.filesToDelete.Add(playerName, new List<string>());
                        //Revitalize.ModCore.log("Added Player Key: " + playerName);
                        this.filesToDelete[playerName].Add(file);
                        //Revitalize.ModCore.log("Added File: " + file);
                    }

                }
            }
        }

        private void deleteFilesBeforeSave()
        {
            if (!Directory.Exists(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"))) Directory.CreateDirectory(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            this.filesToDelete.Clear();
            string[] directories = Directory.GetDirectories(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            foreach (string playerData in directories)
            {
                string objectPath = Path.Combine(playerData, "SavedObjectInformation");
                string[] objectFiles = Directory.GetFiles(objectPath);
                foreach (string file in objectFiles)
                {
                    string playerName = new DirectoryInfo(objectPath).Parent.Name;
                    if (playerName != this.getUniqueCharacterString()) return;
                    else
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        /// <summary>
        /// Called after load to deal with internal file cleanUp
        /// </summary>
        public void afterLoad()
        {
            ModCore.log("WHAT");
            this.deleteAllUnusedFiles();
            //this.removeNullObjects();
            this.restoreModObjects();
        }

        /// <summary>
        /// Restore mod objects to inventories and world after load.
        /// </summary>
        public void restoreModObjects()
        {
            //ModCore.log("Restore all mod objects!");
            //Replace all items in the world.
            List<CustomObject> objsToRestore = new List<CustomObject>();
            foreach (var v in ModCore.ObjectGroups)
            {
                foreach (var obj in v.Value.objects.Values)
                {
                    //(obj as CustomObject).replaceAfterLoad();
                    objsToRestore.Add(obj as CustomObject);
                }
            }
            foreach(CustomObject o in objsToRestore)
            {
                (o as CustomObject).replaceAfterLoad();
            }

            //Replace all held items or items in inventories.
            foreach (GameLocation loc in LocationUtilities.GetAllLocations())
            {
                //ModCore.log("Looking at location: " + loc);
                foreach (StardewValley.Object c in loc.Objects.Values)
                {
                    if (c is Chest)
                    {
                        List<Item> toRemove = new List<Item>();
                        List<Item> toAdd = new List<Item>();
                        foreach (Item o in (c as Chest).items)
                        {
                            if (o == null) continue;
                            if (o is Chest && o.Name != "Chest")
                            {
                                Item I = this.GetItemFromChestName(o.Name);
                                ModCore.log("Found a custom item in a chest!");
                                toAdd.Add(I);
                                toRemove.Add(o);
                            }
                        }
                        foreach (Item i in toRemove)
                        {
                            (c as Chest).items.Remove(i);
                        }
                        foreach (Item I in toAdd)
                        {
                            (c as Chest).items.Add(I);
                        }
                    }
                    else if(c is Chest && c.Name != "Chest")
                    {
                        loc.objects[c.TileLocation] = (StardewValley.Object)this.GetItemFromChestName(c.Name);
                        ModCore.log("Found a custom item that is a chest!");
                    }
                    else if (c is CustomObject)
                    {
                        if ((c as CustomObject).info.inventory == null) continue;
                        List<Item> toRemove = new List<Item>();
                        List<Item> toAdd = new List<Item>();
                        foreach (Item o in (c as CustomObject).info.inventory.items)
                        {
                            if (o == null) continue;
                            if (o is Chest && o.Name != "Chest")
                            {
                                Item I = this.GetItemFromChestName(o.Name);
                                toAdd.Add(I);
                                toRemove.Add(o);
                            }
                        }
                        foreach (Item i in toRemove)
                        {
                            (c as Chest).items.Remove(i);
                        }
                        foreach (Item I in toAdd)
                        {
                            (c as Chest).items.Add(I);
                        }
                        if (c.heldObject.Value != null)
                        {
                            if (c.heldObject.Value is Chest && c.heldObject.Value.Name != "Chest")
                            {
                                ModCore.log("Found a custom object as a held object!");
                                Item I = this.GetItemFromChestName(c.heldObject.Value.Name);
                                c.heldObject.Value = (StardewValley.Object)I;
                            }
                        }
                    }
                    else if (c is StardewValley.Object)
                    {
                        if (c.heldObject.Value != null)
                        {
                            if (c.heldObject.Value is Chest && c.heldObject.Value.Name != "Chest")
                            {
                                ModCore.log("Found a custom object as a held object!");
                                Item I = this.GetItemFromChestName(c.heldObject.Value.Name);
                                c.heldObject.Value = (StardewValley.Object)I;
                            }
                        }
                    }
                }
            }
            List<Item> toAdd2 = new List<Item>();
            List<Item> toRemove2 = new List<Item>();
            foreach (Item I in Game1.player.Items)
            {
                if (I == null) continue;
                else
                {
                    if (I is Chest && I.Name != "Chest")
                    {
                        Item ret = this.GetItemFromChestName(I.Name);
                        toAdd2.Add(ret);
                        toRemove2.Add(I);
                    }

                }
            }
            foreach (Item i in toRemove2)
            {
                Game1.player.Items.Remove(i);
            }
            foreach (Item I in toAdd2)
            {
                Game1.player.addItemToInventory(I);
            }
        }


        /// <summary>
        /// Gets an Item recreated from PYTK's chest replacement objects.
        /// </summary>
        /// <param name="JsonName"></param>
        /// <returns></returns>
        public Item GetItemFromChestName(string JsonName)
        {
            //ModCore.log("Found a custom object in a chest!");
            string jsonString = JsonName;
            //ModCore.log(JsonName);
            string dataSplit= jsonString.Split(new string[] { "<" }, StringSplitOptions.None)[1];
            string backUpGUID = dataSplit.Split('|')[0];
            string[] guidArr = jsonString.Split(new string[] { "|" }, StringSplitOptions.None);

            foreach(string s in guidArr)
            {
                //ModCore.log(s);
            }

            string guidName = guidArr[guidArr.Length - 1];
            guidName = guidName.Substring(5);

            try
            {
                Guid g = Guid.Parse(guidName);
            }
            catch (Exception err)
            {
                Guid d = Guid.Parse(backUpGUID);
                guidName = backUpGUID;
            }
            //ModCore.log("THE GUID IS:"+ guidName);
            
            //ModCore.log(jsonString);
            string type = jsonString.Split('|')[2];
            Item I = (Item)ModCore.Serializer.DeserializeGUID(guidName, Type.GetType(type));

            if (I is MultiTiledObject)
            {
                (I as MultiTiledObject).recreate();
            }
            return I;
        }

        public Item DeserializeFromFarmhandInventory(string JsonName)
        {
            //ModCore.log("Found a custom object in a chest!");
            string jsonString = JsonName;
            //ModCore.log(JsonName);
            string dataSplit = jsonString.Split(new string[] { "<" }, StringSplitOptions.None)[2];
            string backUpGUID = dataSplit.Split('|')[0];
            string[] guidArr = jsonString.Split(new string[] { "|" }, StringSplitOptions.None);

            string infoStr = jsonString.Split(new string[] { "<" }, StringSplitOptions.None)[0];
            string guidStr= jsonString.Split(new string[] { "<" }, StringSplitOptions.None)[1];

            CustomObjectData pyTkData = ModCore.Serializer.DeserializeFromJSONString<CustomObjectData>(dataSplit);
            Type t = Type.GetType(pyTkData.type);
            string id = pyTkData.id;
            //Need Item info

            string guidName = backUpGUID;
            string infoSplit = infoStr.Split('|')[3];
            infoSplit = infoSplit.Substring(3);
            BasicItemInformation info = ModCore.Serializer.DeserializeFromJSONString<BasicItemInformation>(infoSplit);

            CustomObject clone = (CustomObject)ModCore.ObjectManager.getItemByIDAndType(id, t);
            if (clone != null)
            {
                clone.info = info;
                ModCore.log("Guid is????:"+guidStr);
                clone.guid = Guid.Parse(guidStr);
                return clone;
            }
            return null;
        }

        public void returnToTitle()
        {
            //this.gatherAllFilesForCleanup();
        }



        [Obsolete]
        private void removeNullObjects()
        {
            List<Item> removalList = new List<Item>();
            foreach (Item I in Game1.player.Items)
            {
                if (I == null) continue;
                if (I.DisplayName.Contains("Revitalize.Framework") && (I is Chest))
                {
                    removalList.Add(I);
                }

            }
            foreach (Item I in removalList)
            {
                Game1.player.Items.Remove(I);
            }
        }

        /// <summary>
        /// Removes the file from all files that will be deleted.
        /// </summary>
        /// <param name="playerDirectory"></param>
        /// <param name="fileName"></param>
        private void removeFileFromDeletion(string playerDirectory, string fileName)
        {
            if (this.filesToDelete.ContainsKey(playerDirectory))
            {
                //Revitalize.ModCore.log("Removing from deletion: " + fileName);
                this.filesToDelete[playerDirectory].Remove(fileName);
            }
            else
            {
                //Revitalize.ModCore.log("Found key: " + playerDirectory);
                //Revitalize.ModCore.log("Found file: " + fileName);
            }
        }

        /// <summary>
        /// Deletes unused object data.
        /// </summary>
        private void deleteAllUnusedFiles()
        {
            foreach (KeyValuePair<string, List<string>> pair in this.filesToDelete)
            {
                foreach (string file in pair.Value)
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Adds a new converter to the json serializer.
        /// </summary>
        /// <param name="converter">The type of json converter to add to the Serializer.</param>
        public void addConverter(JsonConverter converter)
        {
            this.serializer.Converters.Add(converter);
        }


        /// <summary>
        /// Deserializes an object from a .json file.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="p">The path to the file.</param>
        /// <returns>An object of specified type T.</returns>
        public T Deserialize<T>(string p)
        {
            using (StreamReader sw = new StreamReader(p))
            using (JsonReader reader = new JsonTextReader(sw))
            {

                var obj = this.serializer.Deserialize<T>(reader);
                return obj;
            }
        }

        /// <summary>
        /// Deserializes an object from a .json file.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="p">The path to the file.</param>
        /// <returns>An object of specified type T.</returns>
        public object Deserialize(string p, Type T)
        {
            using (StreamReader sw = new StreamReader(p))
            using (JsonReader reader = new JsonTextReader(sw))
            {
                object obj = this.serializer.Deserialize(reader, T);
                return obj;
            }
        }

        /// <summary>
        /// Serializes an object to a .json file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="o"></param>
        public void Serialize(string path, object o)
        {
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                this.serializer.Serialize(writer, o);
            }
        }

        /// <summary>
        /// Serialize a data structure into an file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public void SerializeGUID(string fileName, object obj)
        {
            string path = Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData", Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID, "SavedObjectInformation", fileName + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            this.Serialize(path, obj);
        }

        /// <summary>
        /// Deserialze a file into it's proper data structure.
        /// </summary>
        /// <typeparam name="T">The type of data structure to deserialze to.</typeparam>
        /// <param name="fileName">The name of the file to deserialize from.</param>
        /// <returns>A data structure object deserialize from a json string in a file.</returns>
        public object DeserializeGUID(string fileName, Type T)
        {
            string path = Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData", Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID, "SavedObjectInformation", fileName + ".json");
            this.removeFileFromDeletion((Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID), path);
            return this.Deserialize(path, T);
        }

        /// <summary>
        /// Deserialze a file into it's proper data structure.
        /// </summary>
        /// <typeparam name="T">The type of data structure to deserialze to.</typeparam>
        /// <param name="fileName">The name of the file to deserialize from.</param>
        /// <returns>A data structure object deserialize from a json string in a file.</returns>
        public T DeserializeGUID<T>(string fileName)
        {
            string path = Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData", Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID, "SavedObjectInformation", fileName + ".json");
            //this.removeFileFromDeletion((Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID),path);
            if (File.Exists(path))
            {
                //ModCore.log("Deseralizing file:" + path);
                return this.Deserialize<T>(path);
            }
            else
            {
                throw new Exception("Can't deserialize file. Default returned. " + path);
            }
        }

        /// <summary>
        /// Converts objects to json form.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string ToJSONString(object o)
        {
            return JsonConvert.SerializeObject(o, this.settings);
        }

        /// <summary>
        /// Converts from json form to objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public T DeserializeFromJSONString<T>(string info)
        {
            return JsonConvert.DeserializeObject<T>(info, this.settings);
        }

        /// <summary>
        /// Converts from json form to objects.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public object DeserializeFromJSONString(string info, Type T)
        {
            return JsonConvert.DeserializeObject(info, T, this.settings);
        }


        /// <summary>
        /// Deserailizes a content file for the mod.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public T DeserializeContentFile<T>(string pathToFile)
        {
            if (File.Exists(pathToFile))
            {

                return this.Deserialize<T>(pathToFile);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Serializes a content file if it doesn't already exist. If it does exist this does nothing as to not override the content file.
        /// </summary>
        /// <param name="fileName">The name to name the file. So a file named MyFile would be a MyFile.json</param>
        /// <param name="obj">The actual to serialize.</param>
        /// <param name="extensionFolder">The sub folder path inside of the Content folder for this mod.</param>
        public void SerializeContentFile(string fileName, object obj, string extensionFolder)
        {
            string path = Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "Content", extensionFolder, fileName + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (File.Exists(path)) return;
            this.Serialize(path, obj);
        }

        /// <summary>
        /// Deletes all .json saved objects before saving.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sender"></param>
        public void DayEnding_CleanUpFilesForDeletion(object o, StardewModdingAPI.Events.DayEndingEventArgs sender)
        {
            //ModCore.log("Day ending now delete files!");
            this.deleteFilesBeforeSave();
        }

        /// <summary>
        /// Gets the unique character path string.
        /// </summary>
        /// <returns></returns>
        public string getUniqueCharacterString()
        {
            return Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/2742276/how-do-i-check-if-a-type-is-a-subtype-or-the-type-of-an-object
        /// </summary>
        /// <param name="potentialBase"></param>
        /// <param name="potentialDescendant"></param>
        /// <returns></returns>
        public bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        public bool IsSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase);
        }
    }
}
