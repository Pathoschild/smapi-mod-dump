using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
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
        public  List<CoreObject> trackedObjectList = new List<CoreObject>();



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
            removalList.Clear();
        }

        public void cleanUpWorld()
        {
            try
            {
                ProcessDirectoryForDeletion(objectsInWorldPath);
            }
            catch(Exception e)
            {
                ModCore.ModMonitor.Log(e.ToString());
            }
            List<CoreObject> removalList = new List<CoreObject>();
            int countProcessed = 0;
            List<Item> idk = new List<Item>();
            foreach (CoreObject d in trackedObjectList)
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
                catch (Exception e)
                {
                    ModCore.ModMonitor.Log(e.ToString());
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
                i.thisLocation.removeObject(i.TileLocation, false);
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
                        if ((v as CoreObject).useXML == true)
                        {
                            t.worldObj.Invoke(v as CoreObject);
                        }
                        //Log.AsyncG("Progress on saving objects: " + countProcessed + "/" + Lists.trackedObjectList.Count);
                        removalList.Add(v as CoreObject);
                    }
                }
            }

            removalList.Clear();
           // Log.AsyncM("Revitalize: Done cleaning world for saving.");

        }

        public void cleanUpStorageContainers()
        {
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
                    if (obj.Value is StardewValley.Objects.Chest) {
                        int k = (obj.Value as StardewValley.Objects.Chest).items.Count;
                        int l = 0;
                    foreach (var item in (obj.Value as StardewValley.Objects.Chest).items)
                        {
                            l++;
                            //ModCore.ModMonitor.Log("Parsing Chest at : " + loc.Name + " X: " + obj.Key.X + " Y: " + obj.Key.Y + " : object number: " + l + "/" + k + "object name: " + item.Name);
                            if (item is CoreObject) removalList.Add(item);
                        }

                    foreach(var v in removalList)
                        {
                            (obj.Value as StardewValley.Objects.Chest).items.Remove(v);

                            SerializerDataNode t;
                            if (acceptedTypes.ContainsKey((v as CoreObject).serializationName))
                            {
                                acceptedTypes.TryGetValue((v as CoreObject).serializationName, out t);
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

            foreach (Building building in Game1.getFarm().buildings)
            {
               
                GameLocation loc =Game1.getLocationFromName(building.nameOfIndoors,true);
              ModCore.ModMonitor.Log("Cleaning up farm building: "+loc.uniqueName.Value);
                int i = loc.objects.Pairs.Count();
                int j = 0;
                foreach (KeyValuePair<Vector2, StardewValley.Object> obj in loc.objects.Pairs) 
                {
                    j++;
                    ModCore.ModMonitor.Log("Parsing location " + loc.Name + " : object number" + j + "/" + i + " : object name: " + obj.Value.name);
                    if (obj.Value is StardewValley.Objects.Chest)
                    {
                        int k = (obj.Value as StardewValley.Objects.Chest).items.Count;
                        int l = 0;
                        foreach (var item in (obj.Value as StardewValley.Objects.Chest).items)
                        {
                            l++;
                            ModCore.ModMonitor.Log("Parsing Chest at : " + loc.Name + " X: " + obj.Key.X + " Y: " + obj.Key.Y + " : object number: " + l + "/" + k + "object name: " + item.Name);
                            if (item is CoreObject) removalList.Add(item);
                        }
                        foreach(var v in removalList)
                        {
                            (obj.Value as StardewValley.Objects.Chest).items.Remove(v);

                            SerializerDataNode t;
                            if(acceptedTypes.ContainsKey((v as CoreObject).serializationName)){
                                acceptedTypes.TryGetValue((v as CoreObject).serializationName, out t);
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
        public void restoreAllModObjects(List<CoreObject> thingsToAddBackIn)
        {
            processDirectoryForDeserialization(playerInventoryPath,thingsToAddBackIn);
            

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


        public void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                //settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.TypeNameHandling = TypeNameHandling.Auto;
                //  settings.Formatting = Formatting.Indented;
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, settings);
                int i = 0;
                string s = filePath;
                while (File.Exists(s) == true)
                {
                    s = filePath;
                    s = (s + Convert.ToString(i));
                    i++;
                }
                filePath = s;

                writer = new StreamWriter(filePath, append);

                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
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
        public void processDirectoryForDeserialization(string pathToFile,List<CoreObject> thingsToAddBackIn)
        {
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

        public void processDirectoryForDeserializationIntoContainer(string pathToFile, List<CoreObject> thingsToAddBackIn)
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


        public void ProcessFileForCleanUp(string path, List<CoreObject> thingsToAddBackIn)
        {

            //Log.AsyncC(path);

            string[] ehh = File.ReadAllLines(path);
            string data = ehh[0];
            CoreObject cObj;
            string a;
            string[] b;
            string s = "";
           // Log.AsyncC(path);
            //  Log.AsyncC(data);
            try
            {
                JObject obj2 = JObject.Parse(data);
                CoreObject obj = (CoreObject)obj2.ToObject(typeof(CoreObject));


                //   Log.AsyncC(obj.thisType);

                a = obj.serializationName;
                b = a.Split(',');
                s = b.ElementAt(0);
             //   Log.AsyncC(s);
            }
            catch (Exception e)
            {
                e.ToString(); //Get rid of that warning because I'll do other things.
                //USE XML STYLE DESERIALIZING
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    var word = ParseXMLType(path);
                    if (pair.Key == word.ToString())
                    {
                        cObj =(CoreObject) pair.Value.parse.Invoke(path);
                        (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);
                        (cObj as CoreObject).resetTexture();
                        if ((cObj as CoreObject).thisLocation == null)
                        {
                            Game1.player.addItemToInventory(cObj);
                           // Log.AsyncY("ADDED ITEM TO INVENTORY");
                            return;
                        }
                        else
                        {
                            (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation, (StardewValley.Object)cObj);
                          thingsToAddBackIn.Add(cObj);
                            //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
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
            */

            //USE JSON STYLE DESERIALIZNG
            if (acceptedTypes.ContainsKey(s))
            {
                  //Log.AsyncC("FUUUUU");
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                  //  Log.AsyncY(pair.Key);
                    if (pair.Key == s)
                    {
                        try
                        {
                            //parse from Json Style
                         //   Log.AsyncR("1");
                            cObj = (CoreObject)pair.Value.parse.Invoke(data);
                            (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);

                            if ((cObj as CoreObject).thisLocation == null)
                            {
                                Game1.player.addItemToInventory(cObj);
                               // Log.AsyncY("ADDED ITEM TO INVENTORY");
                                return;
                            }
                            else
                            {
                                (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation,(StardewValley.Object)cObj);
                                thingsToAddBackIn.Add(cObj);
                                //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
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



        public void ProcessFileForCleanUpIntoContainer(string path, List<CoreObject> thingsToAddBackIn)
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
            */
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
            string data = ehh[0];
            CoreObject cObj;
            string a;
            string[] b;
            string s = "";
            // Log.AsyncC(path);
            //  Log.AsyncC(data);
            try
            {
                JObject obj2 = JObject.Parse(data);
                CoreObject obj =(CoreObject)obj2.ToObject(typeof(CoreObject));

                //   Log.AsyncC(obj.thisType);

                a = obj.serializationName;
                b = a.Split(',');
                s = b.ElementAt(0);
                //   Log.AsyncC(s);
            }
            catch (Exception e)
            {
                e.ToString();
                //USE XML STYLE DESERIALIZING
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    var word = ParseXMLType(path);
                    if (pair.Key == word.ToString())
                    {
                        cObj = (CoreObject)pair.Value.parse.Invoke(path);
                        (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);
                        (cObj as CoreObject).resetTexture();
                        if ((cObj as CoreObject).thisLocation == null)
                        {
                           // Game1.player.addItemToInventory(cObj);
                            try
                            {
                                
                                Utilities.addItemToOtherInventory((chestObject as StardewValley.Objects.Chest).items, cObj);
                            }
                            catch(Exception err)
                            {
                                ModCore.ModMonitor.Log(err.ToString(), LogLevel.Error);
                            }
                            // Log.AsyncY("ADDED ITEM TO INVENTORY");
                            return;
                        }
                        else
                        {
                            (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation, (StardewValley.Object)cObj);
                            thingsToAddBackIn.Add(cObj);
                            //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
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
            */

            //USE JSON STYLE DESERIALIZNG
            if (acceptedTypes.ContainsKey(s))
            {
                ModCore.ModMonitor.Log("parse???");
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    //  Log.AsyncY(pair.Key);
                    if (pair.Key == s)
                    {
                        try
                        {
                            //parse from Json Style
                            //   Log.AsyncR("1");
                            cObj = (CoreObject)pair.Value.parse.Invoke(data);
                            (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);
                            ModCore.ModMonitor.Log("closer");
                            if ((cObj as CoreObject).thisLocation == null)
                            {
                                try
                                {
                                    ModCore.ModMonitor.Log("HERE PRETTY MUCH!!!");
                                    Utilities.addItemToOtherInventory((chestObject as StardewValley.Objects.Chest).items, cObj);

                                    foreach(var v in (chestObject as StardewValley.Objects.Chest).items)
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
                                ModCore.ModMonitor.Log("WHY HERE????");
                                (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).TileLocation, (StardewValley.Object)cObj);
                                thingsToAddBackIn.Add(cObj);
                                //Util.placementAction(cObj, cObj.thisLocation,(int)cObj.tileLocation.X,(int) cObj.tileLocation.Y,null,false);
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
                if(c.TileLocation==v.TileLocation && c.thisLocation == v.thisLocation)
                {
                    StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(v);
                }
            }
        }

    }
}
