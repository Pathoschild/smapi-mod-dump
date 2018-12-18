using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardustCore.Interfaces;
using StardustCore.Objects.Tools;
using StardustCore.Objects.Tools.SerializationInformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StardustCore.Serialization
{

    /// <summary>
    /// TODO: Find a way to serialize objects and tools.
    /// </summary>


   public class SerializationManager
    {
        public string objectsInWorldPath;
        public string playerInventoryPath;
        public string serializerTrashPath;
        public string storageContainerPath;


        public  Dictionary<string, SerializerDataNode> acceptedTypes = new Dictionary<string, SerializerDataNode>();
        public  List<IItemSerializeable> trackedObjectList = new List<IItemSerializeable>();



        public SerializationManager(string PlayerInventoryPath,string SerializerTrashPath,string ObjectsInWorldPath,string StorageContainerPath)
        {
            objectsInWorldPath = ObjectsInWorldPath;
            playerInventoryPath = PlayerInventoryPath;
            serializerTrashPath = SerializerTrashPath;
            storageContainerPath = StorageContainerPath;
            verifyAllDirectoriesExist();
        }

        private void verifyAllDirectoriesExist()
        {
            if (!Directory.Exists(playerInventoryPath)) Directory.CreateDirectory(playerInventoryPath);
            if (!Directory.Exists(serializerTrashPath)) Directory.CreateDirectory(serializerTrashPath);
            if (!Directory.Exists(objectsInWorldPath)) Directory.CreateDirectory(objectsInWorldPath);
            if (!Directory.Exists(storageContainerPath)) Directory.CreateDirectory(storageContainerPath);
        }

        public void cleanUpInventory()
        {

            return;

            ProcessDirectoryForDeletion(playerInventoryPath);

            //ProcessDirectoryForDeletion(SerializerTrashPath);

            List<Item> removalList = new List<Item>();
            foreach (Item d in Game1.player.Items)
            {
                try
                {
                    if (d == null)
                    {
                        //Log.AsyncG("WTF");
                        continue;
                    }
                    // Log.AsyncC(d.GetType());
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }

                string s = Convert.ToString((d.GetType()));

                if (acceptedTypes.ContainsKey(s))
                {
                    SerializerDataNode t;

                    bool works = acceptedTypes.TryGetValue(s, out t);
                    if (works == true)
                    {
                        t.serialize.Invoke(d);
                        removalList.Add(d);
                    }
                }
            }
            foreach (var i in removalList)
            {
                Game1.player.removeItemFromInventory(i);
            }

            if (Game1.IsMasterGame)
            {
                foreach (Farmer f in Game1.getAllFarmhands())
                {
                    List<Item> farmHandCleaner = new List<Item>();
                    foreach (Item i in f.Items)
                    {
                        if (i == null) continue;
                        string s = Convert.ToString((i.GetType()));

                        if (acceptedTypes.ContainsKey(s))
                        {
                            SerializerDataNode t;

                            bool works = acceptedTypes.TryGetValue(s, out t);
                            if (works == true)
                            {
                                farmHandCleaner.Add(i);
                            }
                        }


                    }
                    foreach(Item i in farmHandCleaner)
                    {
                        f.removeItemFromInventory(i);
                    }
                }
            }
            removalList.Clear();
        }
        
        /// <summary>
        /// Removes custom objects from the world and saves them to a file.
        /// </summary>
        public void cleanUpWorld()
        {
            return;

            try
            {
                ProcessDirectoryForDeletion(objectsInWorldPath);
            }
            catch(Exception e)
            {
                ModCore.ModMonitor.Log(e.ToString());
            }
            List<IItemSerializeable> removalList = new List<IItemSerializeable>();
            int countProcessed = 0;
            List<Item> idk = new List<Item>();

            List<GameLocation> allLocations = new List<GameLocation>();
            foreach (GameLocation location in Game1.locations)
            {
                allLocations.Add(location);
            }
            foreach(Building b in Game1.getFarm().buildings)
            {
                allLocations.Add(b.indoors);
            }

            foreach(GameLocation loc in allLocations)
            {
                foreach(var layer in loc.objects)
                {
                    foreach(var pair in layer)
                    {
                        if (removalList.Contains((pair.Value as CoreObject))) continue;
                        try
                        {
                            if (pair.Value == null)
                            {
                                //Log.AsyncG("WTF");
                                continue;
                            }
                            // Log.AsyncC(d.GetType());
                        }
                        catch (Exception e)
                        {
                            //ModCore.ModMonitor.Log(e.ToString());
                        }
                        string s = Convert.ToString((pair.Value.GetType()));

                        if (acceptedTypes.ContainsKey(s))
                        {
                            // Log.AsyncM("Object is of accepted type: " + s);
                            SerializerDataNode t;

                            bool works = acceptedTypes.TryGetValue(s, out t);
                            if (works == true)
                            {
                                countProcessed++;
                                if ((pair.Value as CoreObject).useXML == false)
                                {
                                    // Log.AsyncY("Saving the object");
                                    //Removes the object from the world and saves it to a file.
                                    t.worldObj.Invoke((pair.Value as CoreObject));
                                }
                                else
                                {
                                    idk.Add((pair.Value as CoreObject));
                                }
                                //  Log.AsyncC("Progress on saving objects: " + countProcessed + "/" + Lists.trackedObjectList.Count);
                                removalList.Add((pair.Value as CoreObject));
                            }
                        }
                    }
                }
            }

            foreach (CoreObject d in trackedObjectList)
            {

                if (removalList.Contains(d)) continue;
                try
                {
                    if (d == null)
                    {
                        //Log.AsyncG("WTF");
                        continue;
                    }
                    // Log.AsyncC(d.GetType());
                }
                catch (Exception e)
                {
                    //ModCore.ModMonitor.Log(e.ToString());
                }
                string s = Convert.ToString((d.GetType()));

                if (acceptedTypes.ContainsKey(s))
                {
                   // Log.AsyncM("Object is of accepted type: " + s);
                    SerializerDataNode t;

                    bool works = acceptedTypes.TryGetValue(s, out t);
                    if (works == true)
                    {
                        countProcessed++;
                        if (d.useXML == false)
                        {
                           // Log.AsyncY("Saving the object");
                           //Removes the object from the world and saves it to a file.
                            t.worldObj.Invoke(d);
                        }
                        else
                        {
                            idk.Add(d);
                        }
                      //  Log.AsyncC("Progress on saving objects: " + countProcessed + "/" + Lists.trackedObjectList.Count);
                        removalList.Add(d);
                    }
                }
            }
            foreach (var i in removalList)
            {
                if (i.getCustomType() == typeof(CoreObject))
                {
                    (i as CoreObject).thisLocation.removeObject((i as CoreObject).TileLocation, false);
                }
            }
            foreach (var v in idk)
            {
                string s = Convert.ToString((v.GetType()));

                if (acceptedTypes.ContainsKey(s))
                {
                    SerializerDataNode t;

                    bool works = acceptedTypes.TryGetValue(s, out t);
                    if (works == true)
                    {
                        countProcessed++;
                        //If the item is a core object I can validate that it is in the world and not in an inventory.
                        if ((v is CoreObject))
                        {
                            if ((v as CoreObject).useXML == true)
                            {
                                t.worldObj.Invoke(v as CoreObject);
                            }
                            //Log.AsyncG("Progress on saving objects: " + countProcessed + "/" + Lists.trackedObjectList.Count);
                            removalList.Add(v as CoreObject);
                        }
                    }
                }
            }

            removalList.Clear();
           // Log.AsyncM("Revitalize: Done cleaning world for saving.");

        }

        /// <summary>
        /// Clean all of the storage containers in the game from custom objects.
        /// </summary>
        public void cleanUpStorageContainers()
        {
            return;
            ProcessDirectoryForDeletion(storageContainerPath);

            List<Item> removalList = new List<Item>();
            foreach (GameLocation loc in Game1.locations)
            {
                int i = loc.objects.Pairs.Count();
                int j = 0;
                foreach (KeyValuePair<Vector2, StardewValley.Object> obj in loc.objects.Pairs)
                {
                    j++;
                    //ModCore.ModMonitor.Log("Parsing location " + loc.Name + " : object number" + j + "/" + i + " : object name: " + obj.Value.name);
                    
                    //If the object is a chest get the items from it.
                    if (obj.Value is StardewValley.Objects.Chest) {
                        int k = (obj.Value as StardewValley.Objects.Chest).items.Count;
                        int l = 0;
                    foreach (var item in (obj.Value as StardewValley.Objects.Chest).items)
                        {
                            l++;
                            //ModCore.ModMonitor.Log("Parsing Chest at : " + loc.Name + " X: " + obj.Key.X + " Y: " + obj.Key.Y + " : object number: " + l + "/" + k + "object name: " + item.Name);
                            if (item is IItemSerializeable) removalList.Add(item);
                        }

                    foreach(var v in removalList)
                        {
                            (obj.Value as StardewValley.Objects.Chest).items.Remove(v);

                            SerializerDataNode t;
                            if (acceptedTypes.ContainsKey((v as IItemSerializeable).GetSerializationName()))
                            {
                                acceptedTypes.TryGetValue((v as IItemSerializeable).GetSerializationName(), out t);
                                string s = Path.Combine(loc.Name, "Chest," + Convert.ToString((int)obj.Key.X) + "," + Convert.ToString((int)obj.Key.Y));
                                string s2 = Path.Combine(ModCore.SerializationManager.storageContainerPath, s);
                                if (!Directory.Exists(s)) Directory.CreateDirectory(s2);
                                t.serializeToContainer.Invoke(v, s2);
                            }
                        }
                        removalList.Clear();
                      }
                }
            }
            if (Game1.getFarm() == null) return;
            if (Game1.getFarm().buildings == null) return;
            //Look through all farm buildings for custom items.
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building == null) continue;
                if (String.IsNullOrEmpty(building.nameOfIndoors)) continue;
                GameLocation loc =Game1.getLocationFromName(building.nameOfIndoors,true);
              //ModCore.ModMonitor.Log("Cleaning up farm building: "+loc.uniqueName.Value);
                int i = loc.objects.Pairs.Count();
                int j = 0;
                foreach (KeyValuePair<Vector2, StardewValley.Object> obj in loc.objects.Pairs) 
                {
                    j++;
                    //ModCore.ModMonitor.Log("Parsing location " + loc.Name + " : object number" + j + "/" + i + " : object name: " + obj.Value.name);
                    //Look through all chests in all farm buildings.
                    if (obj.Value is StardewValley.Objects.Chest)
                    {
                        int k = (obj.Value as StardewValley.Objects.Chest).items.Count;
                        int l = 0;
                        foreach (var item in (obj.Value as StardewValley.Objects.Chest).items)
                        {
                            l++;
                            //ModCore.ModMonitor.Log("Parsing Chest at : " + loc.Name + " X: " + obj.Key.X + " Y: " + obj.Key.Y + " : object number: " + l + "/" + k + "object name: " + item.Name);
                            if (item is IItemSerializeable) removalList.Add(item);
                        }
                        foreach(var v in removalList)
                        {
                            (obj.Value as StardewValley.Objects.Chest).items.Remove(v);

                            SerializerDataNode t;
                            if(acceptedTypes.ContainsKey((v as IItemSerializeable).GetSerializationName())){
                                acceptedTypes.TryGetValue((v as IItemSerializeable).GetSerializationName(), out t);
                                string s = Path.Combine(building.nameOfIndoors, "Chest,"+Convert.ToString( (int)obj.Key.X)+","+Convert.ToString((int)obj.Key.Y));
                                string s2 = Path.Combine(ModCore.SerializationManager.storageContainerPath, s);
                                if (!Directory.Exists(s)) Directory.CreateDirectory(s2);
                                t.serializeToContainer.Invoke(v, s2);
                            }
                        }
                        removalList.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Reloads all modded objects added by this mod back to the game in proper locations.
        /// </summary>
        /// <param name="thingsToAddBackIn"></param>
        public void restoreAllModObjects(List<IItemSerializeable> thingsToAddBackIn, bool onlyInventory=false)
        {
            return;
            processDirectoryForDeserialization(playerInventoryPath,thingsToAddBackIn);
            if (onlyInventory) return;

           // Log.AsyncG("Done deserializing player inventory.");
            try
            {
                trackedObjectList.Clear(); //clear whatever mod objects I'm tracking
                processDirectoryForDeserialization(objectsInWorldPath,thingsToAddBackIn); //restore whatever I'm tracking here when I replace the object back into the world. This also works when loading up the game, not just when saving/loading
                processDirectoryForDeserializationIntoContainer(storageContainerPath, thingsToAddBackIn);
            }
            catch (Exception e)
            {
                ModCore.ModMonitor.Log(e.ToString());
            }
        }


        public void ProcessDirectoryForDeletion(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                File.Delete(fileName);
                // File.Delete(fileName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectoryForDeletion(subdirectory);

        }


        public void serializeXML<T>(Item I)
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var newWriter = new StringWriter();
            using (var writer = XmlWriter.Create(newWriter))
            {
                xmlSerializer.Serialize(writer,I);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="thingsToAddBackIn">Typically this would be the trackedObjectList.</param>
        public void processDirectoryForDeserialization(string pathToFile,List<IItemSerializeable> thingsToAddBackIn)
        {
            //StardustCore.ModCore.ModMonitor.Log("Look through dir: " + pathToFile);
            string[] fileEntries = Directory.GetFiles(pathToFile);
        //    Log.AsyncC(pathToFile);

            foreach(var fileName in fileEntries)
            {
                ProcessFileForCleanUp(fileName,thingsToAddBackIn);
              //  Log.AsyncG(fileName);
            }

            string[] subDirectories = Directory.GetDirectories(pathToFile);
            foreach(var folder in subDirectories)
            {
                processDirectoryForDeserialization(folder,thingsToAddBackIn);
            }

        }

        public void processDirectoryForDeserializationIntoContainer(string pathToFile, List<IItemSerializeable> thingsToAddBackIn)
        {
            string[] fileEntries = Directory.GetFiles(pathToFile);
            //    Log.AsyncC(pathToFile);

            foreach (var fileName in fileEntries)
            {
                ProcessFileForCleanUpIntoContainer(fileName, thingsToAddBackIn);
                //  Log.AsyncG(fileName);
            }

            string[] subDirectories = Directory.GetDirectories(pathToFile);
            foreach (var folder in subDirectories)
            {
                processDirectoryForDeserializationIntoContainer(folder, thingsToAddBackIn);
            }

        }

        public void ProcessFileForCleanUp(string path, List<IItemSerializeable> thingsToAddBackIn)
        {

            try
            {
                string type = "";
                int count = 0;
                while (type == "" || type==null)
                {
                    if (count == 0)
                    {
                        //THE ERROR LIES HERE AS IT THINKS IT CAN TRY TO BE A CORE OBJECT WHEN IT IS NOT!!!!
                        CoreObject core_obj = StardustCore.ModCore.ModHelper.ReadJsonFile<CoreObject>(path); //FIND A WAY TO FIX THIS!!!!
                        type = (core_obj as CoreObject).serializationName;
                        //ModCore.ModMonitor.Log("UMM THIS CAN't BE RIGHT 1" + type);
                    }

                    if (count == 1)
                    {
                        //THIS NEEDS TO BE SOMETHING GENERIC!!!
                        SerializedObjectBase core_obj = StardustCore.ModCore.ModHelper.ReadJsonFile<SerializedObjectBase>(path);
                        type = (core_obj as SerializedObjectBase).SerializationName;
                        //ModCore.ModMonitor.Log("UMM THIS CAN't BE RIGHT 2" + type);
                    }

                    if (count == 2)
                    {
                        ModCore.ModMonitor.Log("A valid type could not be found for the file: "+path);
                        return;
                    }

                    count++;
                }

                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    //  Log.AsyncY(pair.Key);
                    if (pair.Key == type)
                    {
                        try
                        {
                            //parse from Json Style
                            //   Log.AsyncR("1");
                            var cObj = pair.Value.parse.Invoke(path);
                            if (cObj is CoreObject)
                            {
                                (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);

                                if ((cObj as CoreObject).thisLocation == null)
                                {
                                    Game1.player.addItemToInventory(cObj);
                                    // Log.AsyncY("ADDED ITEM TO INVENTORY");
                                    return;
                                }
                                else
                                {
                                    try
                                    {
                                        (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation, (StardewValley.Object)cObj);
                                        thingsToAddBackIn.Add(cObj as CoreObject);
                                    }
                                    catch(Exception err)
                                    {
                                        //throw new Exception(err.ToString());
                                        return;
                                    }
                                    //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
                                }
                            }
                            else
                            {
                                Game1.player.addItemToInventory(cObj);
                            }
                        }
                        catch (Exception e)
                        {
                            ModCore.ModMonitor.Log(e.ToString());
                            // Log.AsyncO(e);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
                //Tool t = StardustCore.ModCore.ModHelper.ReadJsonFile<Tool>(path);
            }
            
        }


        /*
        /// <summary>
        /// Process an item from a file back into it's original storage container.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="thingsToAddBackIn"></param>
        public void ProcessFileForCleanUpIntoContainer(string path, List<IItemSerializeable> thingsToAddBackIn)
        {

            //Log.AsyncC(path);
            string newLine = Environment.NewLine;

            string[] chestArray= path.Split(new string[] { "/"}, StringSplitOptions.None);
            string[] chestArray2= path.Split(new string[] { "\\" }, StringSplitOptions.None);
            /*
            foreach (var v in chestArray)
            {
                Log.AsyncC("PART OF PATH "+v);
            }
            foreach (var v in chestArray2)
            {
                Log.AsyncC("PART OF PATH2 " + v);
            }
            
            if (chestArray2.Length > chestArray.Length) chestArray = chestArray2;

            GameLocation loc = Game1.getLocationFromName(chestArray[chestArray.Length - 3]);
            string[] chest = chestArray[chestArray.Length - 2].Split(',');
            StardewValley.Object chestObject;
            bool f = loc.objects.TryGetValue(new Microsoft.Xna.Framework.Vector2( Convert.ToInt32(chest[1]),Convert.ToInt32(chest[2])),out chestObject);
            if (f == true)
            {
                ModCore.ModMonitor.Log("YAY");
            }
            else
            {
                ModCore.ModMonitor.Log("BOO");
            }

            string[] ehh = File.ReadAllLines(path);
            Item cObj;
            string a;
            string[] b;
            string s = "";
            // Log.AsyncC(path);
            //  Log.AsyncC(data);
            SerializedObjectBase obj = StardustCore.ModCore.ModHelper.ReadJsonFile<SerializedObjectBase>(path);
            try
            {
                    //   Log.AsyncC(obj.thisType);

                    a = obj.SerializationName;
                    ModCore.ModMonitor.Log(":THIS IS MY TYPE!!!:" + a);
                    b = a.Split(',');
                    s = b.ElementAt(0);
                //   Log.AsyncC(s);
            }
            catch (Exception e)
            {
                ModCore.ModMonitor.Log(e.ToString());
                
                //USE XML STYLE DESERIALIZING
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    var word = ParseXMLType(path);
                    if (pair.Key == word.ToString())
                    {
                        cObj = pair.Value.parse.Invoke(path);
                        if (cObj is CoreObject)
                        {
                            (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);
                            (cObj as CoreObject).resetTexture();
                            if ((cObj as CoreObject).thisLocation == null)
                            {
                                // Game1.player.addItemToInventory(cObj);
                                try
                                {

                                    Utilities.addItemToOtherInventory((chestObject as StardewValley.Objects.Chest).items, (cObj as CoreObject));
                                }
                                catch (Exception err)
                                {
                                    ModCore.ModMonitor.Log(err.ToString(), LogLevel.Error);
                                }
                                // Log.AsyncY("ADDED ITEM TO INVENTORY");
                                return;
                            }
                            else
                            {
                                (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation, (StardewValley.Object)cObj);
                                thingsToAddBackIn.Add((cObj as CoreObject));
                                //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
                            }
                        }
                        else
                        {
                           
                                try
                                {
                                    Utilities.addItemToOtherInventory((chestObject as StardewValley.Objects.Chest).items, cObj);
                                }
                                catch (Exception err)
                                {
                                    ModCore.ModMonitor.Log(err.ToString(), LogLevel.Error);
                                }
                            
                        }
                    }
                }

                // Log.AsyncG("attempting to parse from path and value of s is " + s);
            }

            // var cObj = parseBagOfHolding(path); //pair.Value.parse.Invoke(path);
            //  cObj.TextureSheet = Game1.content.Load<Texture2D>(Path.Combine("Revitalize", "CropsNSeeds", "Graphics", "seeds"));
            /*
            cObj.thisLocation = Game1.getLocationFromName(cObj.locationsName);
            if (cObj.thisLocation == null)
            {
                Game1.player.addItemToInventory(cObj);
                return;
            }
            else
            {
                cObj.thisLocation.objects.Add(cObj.tileLocation, cObj);
                Lists.trackedObjectList.Add(cObj);
                //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
            }
            

            //USE JSON STYLE DESERIALIZNG
            if (acceptedTypes.ContainsKey(s))
            {
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    //  Log.AsyncY(pair.Key);
                    if (pair.Key == s)
                    {
                        try
                        {
                            //parse from Json Style
                            //   Log.AsyncR("1");
                            cObj = pair.Value.parse.Invoke(path);
                            if (cObj is CoreObject)
                            {
                                (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);
                                if ((cObj as CoreObject).thisLocation == null)
                                {
                                    try
                                    {
                                        Utilities.addItemToOtherInventory((chestObject as StardewValley.Objects.Chest).items, (cObj as CoreObject));

                                        foreach (var v in (chestObject as StardewValley.Objects.Chest).items)
                                        {
                                            ModCore.ModMonitor.Log(v.Name);
                                        }

                                    }
                                    catch (Exception err)
                                    {
                                        ModCore.ModMonitor.Log(err.ToString(), LogLevel.Error);
                                    }
                                    // Log.AsyncY("ADDED ITEM TO INVENTORY");
                                    return;
                                }
                                else
                                {
                                    (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation, (StardewValley.Object)cObj);
                                    thingsToAddBackIn.Add((cObj as CoreObject));
                                    //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
                                }
                            }
                            else 
                            {
                                try
                                {
                                    Utilities.addItemToOtherInventory((chestObject as StardewValley.Objects.Chest).items, cObj);
                                }
                                catch (Exception err)
                                {
                                    ModCore.ModMonitor.Log(err.ToString(), LogLevel.Error);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            ModCore.ModMonitor.Log(e.ToString());
                            // Log.AsyncO(e);
                        }
                    }
                }
            }
            else
            {
                ModCore.ModMonitor.Log("Error parsing unknown object type: " + s, LogLevel.Error);
            }

        }
    */

            public void ProcessFileForCleanUpIntoContainer(string path, List<IItemSerializeable> thingsToAddBackIn)
            {



            //Log.AsyncC(path);
            string newLine = Environment.NewLine;

            string[] chestArray = path.Split(new string[] { "/" }, StringSplitOptions.None);
            string[] chestArray2 = path.Split(new string[] { "\\" }, StringSplitOptions.None);
            /*
            foreach (var v in chestArray)
            {
                Log.AsyncC("PART OF PATH "+v);
            }
            foreach (var v in chestArray2)
            {
                Log.AsyncC("PART OF PATH2 " + v);
            }
            */
            if (chestArray2.Length > chestArray.Length) chestArray = chestArray2;

            GameLocation loc = Game1.getLocationFromName(chestArray[chestArray.Length - 3]);
            string[] chest = chestArray[chestArray.Length - 2].Split(',');
        StardewValley.Object chestObject;
        bool f = loc.objects.TryGetValue(new Microsoft.Xna.Framework.Vector2(Convert.ToInt32(chest[1]), Convert.ToInt32(chest[2])), out chestObject);

            try
            {
                string type = "";
                int count = 0;
                while (type == "" || type == null)
                {
                    if (count == 0)
                    {
                        //THE ERROR LIES HERE AS IT THINKS IT CAN TRY TO BE A CORE OBJECT WHEN IT IS NOT!!!!
                        CoreObject core_obj = StardustCore.ModCore.ModHelper.ReadJsonFile<CoreObject>(path); //FIND A WAY TO FIX THIS!!!!
                        type = (core_obj as CoreObject).serializationName;
                        //ModCore.ModMonitor.Log("UMM THIS CAN't BE RIGHT 1" + type);
                    }

                    if (count == 1)
                    {
                        //THIS NEEDS TO BE SOMETHING GENERIC!!!
                        SerializedObjectBase core_obj = StardustCore.ModCore.ModHelper.ReadJsonFile<SerializedObjectBase>(path);
                        type = (core_obj as SerializedObjectBase).SerializationName;
                        //ModCore.ModMonitor.Log("UMM THIS CAN't BE RIGHT 2" + type);
                    }

                    if (count == 2)
                    {
                        ModCore.ModMonitor.Log("A valid type could not be found for the file: " + path);
                        return;
                    }

                    count++;
                }

                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    //  Log.AsyncY(pair.Key);
                    if (pair.Key == type)
                    {
                        try
                        {
                            //parse from Json Style
                            //   Log.AsyncR("1");
                            var cObj = pair.Value.parse.Invoke(path);
                            if (cObj is CoreObject)
                            {
                                (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);

                                if ((cObj as CoreObject).thisLocation == null)
                                {
                                    Utilities.addItemToOtherInventory((chestObject as Chest).items, cObj);
                                    // Log.AsyncY("ADDED ITEM TO INVENTORY");
                                    return;
                                }
                                else
                                {
                                    (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation, (StardewValley.Object)cObj);
                                    thingsToAddBackIn.Add(cObj as CoreObject);
                                    //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
                                }
                            }
                            else
                            {
                                Utilities.addItemToOtherInventory((chestObject as Chest).items, cObj);
                            }
                        }
                        catch (Exception e)
                        {
                            ModCore.ModMonitor.Log(e.ToString());
                            // Log.AsyncO(e);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ModCore.ModMonitor.Log(err.ToString());
                //Tool t = StardustCore.ModCore.ModHelper.ReadJsonFile<Tool>(path);
            }

        }




/// <summary>
/// ???
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
public string ParseXMLType(string path)
        {
            string[] s = File.ReadAllLines(path);
            string returnString = "";
            foreach (string v in s)
            {
                //   Log.AsyncC(v);
                if (v.Contains("serializationName"))
                {
                    returnString = v.Remove(0, 12);
                    returnString = returnString.Remove(returnString.Length - 11, 11);
                }

            }
            return returnString;
        }

        /// <summary>
        /// Parse rectangles.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Microsoft.Xna.Framework.Rectangle parseRectFromJson(string s)
        {
            s = s.Replace('{', ' ');
            s = s.Replace('}', ' ');
            s = s.Replace('^', ' ');
            s = s.Replace(':', ' ');
            string[] parsed = s.Split(' ');
            foreach (var v in parsed)
            {
                //Log.AsyncY(v);
            }
            return new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(parsed[2]), Convert.ToInt32(parsed[4]), Convert.ToInt32(parsed[6]), Convert.ToInt32(parsed[8]));
        }

        /// <summary>
        /// Remove all objects that there are a copy of this thing?
        /// </summary>
        /// <param name="c"></param>
        public void removeObjectWithCopy(CoreObject c)
        {
            foreach(var v in StardustCore.ModCore.SerializationManager.trackedObjectList)
            {
                if (v.getCustomType() == typeof(CoreObject))
                {
                    if (c.TileLocation == (v as CoreObject).TileLocation && c.thisLocation == (v as CoreObject).thisLocation)
                    {
                        StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a list of default supported types added by Stardust Core.
        /// </summary>
        public void initializeDefaultSuportedTypes()
        {
            initializeSupportedToolTypes();
            initializeSupportedObjectTypes();
        }

        private void initializeSupportedObjectTypes()
        {
            this.acceptedTypes.Add(typeof(CoreObject).ToString(), new SerializerDataNode(CoreObject.Serialize, CoreObject.Deserialize, new SerializerDataNode.WorldParsingFunction(CoreObject.Serialize), new SerializerDataNode.SerializingToContainerFunction(CoreObject.SerializeToContainer)));
        }

        /// <summary>
        /// Initializes supported tools made by Stardust Core.
        /// </summary>
        private void initializeSupportedToolTypes()
        {
            this.acceptedTypes.Add(typeof(ExtendedAxe).ToString(), new SerializerDataNode(ExtendedAxe.Serialize, ExtendedAxe.Deserialize, null, new SerializerDataNode.SerializingToContainerFunction(ExtendedAxe.SerializeToContainer)));
        }

        public static string getValidSavePathIfDuplicatesExist(Item I, string path, int number)
        {
            String savePath = path;
            String fileName = I.Name + number + ".json";
            String resultPath = Path.Combine(savePath, fileName);
            return resultPath;
        }
    }
}
