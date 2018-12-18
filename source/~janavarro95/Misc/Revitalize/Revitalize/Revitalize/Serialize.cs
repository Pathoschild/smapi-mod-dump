using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Revitalize.Magic.Alchemy;
using Revitalize.Magic.Alchemy.Objects;
using Revitalize.Objects;
using Revitalize.Objects.Machines;
using Revitalize.Persistence;
using Revitalize.Resources;
using Revitalize.Resources.DataNodes;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Revitalize
{
    class Serialize
    {
        public static string InvPath;

        public static string PlayerDataPath;
        public static string DataDirectoryPath;

        public static string objectsInWorldPath;

        public static string TrackedTerrainDataPath;

        public static string SerializerTrashPath;

        public static string SettingsPath;

        public static void cleanUpWorld()
        {
            ProcessDirectoryForDeletion(objectsInWorldPath);
            List<CoreObject> removalList = new List<CoreObject>();
            int countProcessed = 0;
            List<Item> idk = new List<Item>();
            foreach (CoreObject d in Lists.trackedObjectList)
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

                }
                string s = Convert.ToString((d.GetType()));
              
                if (Dictionaries.acceptedTypes.ContainsKey(s))
                {
                    SerializerDataNode t;

                    bool works = Dictionaries.acceptedTypes.TryGetValue(s, out t);
                    if (works == true)
                    {
                        countProcessed++;
                        if (d.useXML == false)
                        {
                            t.worldObj.Invoke(d);
                        }
                        else
                        {
                            idk.Add(d);
                        }
                        //Log.AsyncC("Progress on saving objects: " + countProcessed + "/" + Lists.trackedObjectList.Count);
                        removalList.Add(d);
                    }
                }
            }
            foreach (var i in removalList)
            {
                i.thisLocation.removeObject(i.tileLocation, false);
            }
            foreach(var v in idk)
            {
                string s = Convert.ToString((v.GetType()));

                if (Dictionaries.acceptedTypes.ContainsKey(s))
                {
                    SerializerDataNode t;

                    bool works = Dictionaries.acceptedTypes.TryGetValue(s, out t);
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
            //Log.AsyncM("Revitalize: Done cleaning world for saving.");

        }


        public static void cleanUpInventory()
        {
            ProcessDirectoryForDeletion(InvPath);

            ProcessDirectoryForDeletion(SerializerTrashPath);

            List<Item> removalList = new List<Item>();
            foreach (Item d in Game1.player.items)
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

                }
                string s = Convert.ToString((d.GetType()));




                if (Dictionaries.acceptedTypes.ContainsKey(s))
                {
                    SerializerDataNode t;

                    bool works = Dictionaries.acceptedTypes.TryGetValue(s, out t);
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

            //Log.AsyncM("Revitalize: Done cleaning inventory!");
        }

        /// <summary>
        /// Restore all of the serialized mod objects from the mod's player's data folders to either the player's inventory or the game world depending where the object was located when it was serialized.
        /// </summary>
        public static void restoreAllModObjects()
        {

            //   Log.AsyncG(InvPath);
            ProcessDirectoryForCleanUp(InvPath);
            try
            {
                Lists.trackedObjectList.Clear(); //clear whatever mod objects I'm tracking
                ProcessDirectoryForCleanUp(objectsInWorldPath); //restore whatever I'm tracking here when I replace the object back into the world. This also works when loading up the game, not just when saving/loading
            }
            catch (Exception e)
            {
                SetUp.createDirectories();
            }
        }

        /// <summary>
        /// Cleans up the mod files by deleting possible old files for objects that may/may not exist. Aka delete everything before serializing everything again.
        /// </summary>
        /// <param name="targetDirectory"></param>
        public static void ProcessDirectoryForDeletion(string targetDirectory)
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

        public static void ProcessDirectoryForCleanUp(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFileForCleanUp(fileName);
                // File.Delete(fileName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectoryForCleanUp(subdirectory);

        }
        
        public static string ParseXMLType(string path)
        {
            string[] s = File.ReadAllLines(path);
            string returnString="";
            foreach(string v in s)
            {
             //   Log.AsyncC(v);
                if (v.Contains("thisType"))
                {                  
                  returnString=  v.Remove(0, 12);                  
                 returnString=   returnString.Remove(returnString.Length - 11, 11);
                }

            }
            return returnString;
        }


        // Insert logic for processing found files here.
        public static void ProcessFileForCleanUp(string path)
        {
            string[] ehh = File.ReadAllLines(path);
            string data = ehh[0];
            Revitalize.CoreObject cObj;
            string a;
            string[] b;
            string s="";
            //Log.AsyncC(path);
            //  Log.AsyncC(data);
            try
            {
                dynamic obj = JObject.Parse(data);


                //   Log.AsyncC(obj.thisType);

                 a = obj.thisType;
                b = a.Split(',');
                s = b.ElementAt(0);
                //   Log.AsyncC(s);
            }
            catch(Exception e)
            {


                foreach (KeyValuePair<string, SerializerDataNode> pair in Dictionaries.acceptedTypes)
                {
                   var word = ParseXMLType(path);
                    if (pair.Key == word.ToString() )
                    {
                        cObj = pair.Value.parse.Invoke(path);
                        cObj.thisLocation = Game1.getLocationFromName(cObj.locationsName);
                        cObj.resetTexture();
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
                    }
                }
                        
                //Log.AsyncG("attempting to parse from path and value of s is " + s);
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
            if (Dictionaries.acceptedTypes.ContainsKey(s))
            {
                //  Log.AsyncC("FUUUUU");
                foreach (KeyValuePair<string, SerializerDataNode> pair in Dictionaries.acceptedTypes)
                {
                    if (pair.Key == s)
                    {
                        try
                        {
                            //parse from Json Style
                                cObj = pair.Value.parse.Invoke(data);
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
                        }
                        catch (Exception e)
                        {
                        
                        }
                    }
                }
            }   

        }

        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
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

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                //Log.AsyncC(filePath);
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();

                JsonSerializerSettings settings = new JsonSerializerSettings();
                // settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.TypeNameHandling = TypeNameHandling.Auto;
                //settings.Formatting = Formatting.Indented;
                return JsonConvert.DeserializeObject<T>(fileContents, settings);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        public static void WriteToXMLFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to serialize.
           
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            
            TextWriter writer = new StreamWriter(filePath);
            serializer.Serialize(writer, objectToWrite);
            writer.Close();

            // XElement school = doc.Element("School");
            // school.Add(new XAttribute("ID", "ID_Value"));

        }

        public static bool WriteToXMLFileSafetyCheck(string filePath, Item objectToWrite, bool append = false)
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to serialize.
            int j = 0;
            foreach (var v in Lists.serializerTypes)
            {
                j++;
             //   Log.AsyncG("HELLO");
                try
                {
                    XmlSerializer serializer =
                    new XmlSerializer(v);
                    
                    TextWriter writer = new StreamWriter(Path.Combine(SerializerTrashPath,objectToWrite.Name+j));
                        serializer.Serialize(writer, objectToWrite);
  
                    writer.Close();
                    File.Delete(Path.Combine(SerializerTrashPath, objectToWrite.Name + j));

                 //   Log.AsyncC("return results");
                    return true;
                }
                catch (Exception e)
                {
                    
                //    Log.AsyncR(e);
                    continue;
                }
            }
            ////Log.AsyncC("CHECK RESULTS " + false);
            return false;
            // XElement school = doc.Element("School");
            // school.Add(new XAttribute("ID", "ID_Value"));
        }

        public static T ReadFromXMLFile<T>(string filePath) where T : new()
        {
            // Create an instance of the XmlSerializer class;
            // specify the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            /* If the XML document has been altered with unknown 
            nodes or attributes, handle them with the 
            UnknownNode and UnknownAttribute events.*/
            /*
            serializer.UnknownNode += new
            XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new
            XmlAttributeEventHandler(serializer_UnknownAttribute);
            */
            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filePath, FileMode.Open);
            // Declare an object variable of the type to be deserialized.
          
            /* Use the Deserialize method to restore the object's state with
            data from the XML document. */
           return (T)serializer.Deserialize(fs);

        }

        private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            //Log.AsyncR(e);
        }

        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            //Log.AsyncR(e);
        }

        public static void serializeMapSwapData(MapSwapData d)
        {
            if (File.Exists(Path.Combine(PlayerDataPath, "ChosenMap" + ".json")))
            {

                File.Delete(Path.Combine(PlayerDataPath, "ChosenMap" + ".json"));
            }
            Serialize.WriteToJsonFile(Path.Combine(PlayerDataPath, "ChosenMap" + ".json"), (MapSwapData)d);
        }

        public static MapSwapData parseMapSwapData()
        {
            try
            {
                return Serialize.ReadFromJsonFile<MapSwapData>(Path.Combine(PlayerDataPath, "ChosenMap" + ".json"));
            }
            catch (Exception e)
            {
                // Log.AsyncR(e);
                return null;
            }
        }

        public static Item parseItemFromJson(string data)
        {

            dynamic obj = JObject.Parse(data);
            string a = obj.thisType;
            string[] b = a.Split(',');
            string s = b.ElementAt(0);

            if (Dictionaries.acceptedTypes.ContainsKey(s))
            {
                foreach (KeyValuePair<string, SerializerDataNode> pair in Dictionaries.acceptedTypes)
                {
                    if (pair.Key == s)
                    {
                        var cObj = pair.Value.parse.Invoke(data);
                        return cObj;
                    }
                }
            }


            return null;
            // return cObj;

        }

        public static Decoration parseDecoration(string data)
        {

            dynamic obj = JObject.Parse(data);


            //  Log.AsyncC(obj.thisType);


            Decoration d = new Decoration(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;
            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;


            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();


            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            d.thisLocation = obj.thisLocation;
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;

            try
            {
                return d;
            }
            catch (Exception e)
            {
                // Log.AsyncM(e);
                return null;
            }




        }
        public static void serializeDecoration(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (Decoration)d);
        }
        public static void serializeDecorationFromWorld(CoreObject d)
        {
            Serialize.WriteToJsonFile(Path.Combine(objectsInWorldPath, d.thisLocation.name, d.Name + ".json"), (Decoration)d);
        }


        public static Spawner parseSpawner(string data)
        {
            dynamic obj = JObject.Parse(data);

            Spawner d = new Spawner(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;
            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;


            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();


            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            d.thisLocation = obj.thisLocation;
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;
            try
            {
                return d;
            }
            catch (Exception e)
            {
                // Log.AsyncM(e);
                return null;
            }
        }

        public static void serializeSpawner(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (Spawner)d);
        }
        public static void serializeSpawnerFromWorld(CoreObject d)
        {
            Serialize.WriteToJsonFile(Path.Combine(objectsInWorldPath, d.thisLocation.name, d.Name + ".json"), (Spawner)d);
        }


        public static GiftPackage parseGiftPackage(string data)
        {
            dynamic obj = JObject.Parse(data);

            GiftPackage d = new GiftPackage(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;
            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;


            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();


            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            d.thisLocation = obj.thisLocation;
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;

            d.drawColor = obj.drawColor;

            try
            {
                return d;
            }
            catch (Exception e)
            {
                //  Log.AsyncM(e);
                return null;
            }
        }
        public static void serializeGiftPackage(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (GiftPackage)d);
        }

        public static ExtraSeeds parseExtraSeeds(string data)
        {

            dynamic obj = JObject.Parse(data);

            ExtraSeeds d = new ExtraSeeds(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;
            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;


            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();


            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            d.thisLocation = obj.thisLocation;
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;
            try
            {
                return d;
            }
            catch (Exception e)
            {
                //   Log.AsyncM(e);
                return null;
            }
        }
        public static void serializeExtraSeeds(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (ExtraSeeds)d);
        }

        public static Spell parseSpell(string data)
        {

            dynamic obj = JObject.Parse(data);
            Spell d = new Spell();
            d.spellIndex = obj.spellIndex;
            Spell spell = new Spell();
            bool b = Dictionaries.spellList.TryGetValue(d.spellIndex, out spell);

            Spell k = (Spell)spell.getOne();
            if (b == true) return k;
            else return null;

        }
        public static void serializeSpell(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (Spell)d);
        }


        public static Light parseLight(string data)
        {

            dynamic obj = JObject.Parse(data);

            Light d = new Light(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;

            //  Log.AsyncC(t);

            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;
            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();
            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            // d.thisLocation = Game1.getLocationFromName(loc);
            // d.thisLocation = obj.thisLocation;
            //Log.AsyncC(d.thisLocation);
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;

            d.drawColor = obj.drawColor;

            try
            {
                return d;
            }
            catch (Exception e)
            {
                //Log.AsyncM(e);
                return null;
            }

        }
        public static void serializeLight(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (Light)d);
        }
        public static void serializeLightFromWorld(CoreObject d)
        {
            Serialize.WriteToJsonFile(Path.Combine(objectsInWorldPath, d.thisLocation.name, d.Name + ".json"), (Light)d);
        }

        public static SpriteFontObject parseSpriteFontObject(string data)
        {

            dynamic obj = JObject.Parse(data);

            SpriteFontObject d = new SpriteFontObject(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;

            //  Log.AsyncC(t);

            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;
            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();
            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            // d.thisLocation = Game1.getLocationFromName(loc);
            // d.thisLocation = obj.thisLocation;
            //Log.AsyncC(d.thisLocation);
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;

            d.drawColor = obj.drawColor;

            try
            {
                return d;
            }
            catch (Exception e)
            {
                //Log.AsyncM(e);
                return null;
            }

        }
        public static void serializeSpriteFontObject(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (SpriteFontObject)d);
        }
        public static void serializeSpriteFontObjectFromWorld(CoreObject d)
        {
            Serialize.WriteToJsonFile(Path.Combine(objectsInWorldPath, d.thisLocation.name, d.Name + ".json"), (SpriteFontObject)d);
        }

        public static BagofHolding parseBagOfHolding(string data)
        {
            BagofHolding h= ReadFromXMLFile<BagofHolding>(data);

            for (int i = 0; i <= h.allInventoriesCapacities.Count - 1; i++)
            {
                h.allInventories[i].Capacity = h.allInventoriesCapacities[i];
            }
            h.inventory = h.allInventories[h.inventoryIndex];

            return h;


            dynamic obj = JObject.Parse(data);

            // Log.AsyncC(data);

             // Log.AsyncC(obj.thisType);


            BagofHolding d = new BagofHolding();

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;

            //  Log.AsyncC(t);

            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;

            JArray allInvCapacities = obj.allInventoriesCapacities;
            d.allInventoriesCapacities = allInvCapacities.ToObject<List<int>>();

            JArray allInv = obj.allInventories;
            d.allInventories = new List<List<Item>>();

            foreach (var v in allInv)
            {
                JArray currentObj = v.ToObject<JArray>();


                List<Item> l = parseInventoryList(currentObj);
                d.allInventories.Add(l);

            }

            //  d.allInventories = allInv.ToObject<List<List<Item>>>();
            d.inventoryIndex = obj.inventoryIndex;

            d.inventory = d.allInventories[d.inventoryIndex];

            for (int i = 0; i <= d.allInventoriesCapacities.Count - 1; i++)
            {
                d.allInventories[i].Capacity = d.allInventoriesCapacities[i];
            }

            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            // d.thisLocation = Game1.getLocationFromName(loc);
            // d.thisLocation = obj.thisLocation;
            ////Log.AsyncC(d.thisLocation);
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;

            d.drawColor = obj.drawColor;

            try
            {
                return d;
            }
            catch (Exception e)
            {
                //Log.AsyncM(e);
                return null;
            }
        }
        public static void serializeBagOfHolding(Item d)
        {
            WriteToXMLFile<BagofHolding>(Path.Combine(InvPath, d.Name + ".xml"), (BagofHolding)d);
            // Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (BagofHolding)d);
        }

        public static Quarry parseQuarry(string data)
        {
            dynamic obj = JObject.Parse(data);
            Quarry d = new Quarry(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;

            //  Log.AsyncC(t);

            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;
            JArray array = obj.inventory;
            d.inventory = array.ToObject<List<Item>>();
            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            d.thisLocation = obj.thisLocation;
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;

            d.ResourceName = obj.ResourceName;
            d.dataNode = obj.dataNode;
            d.locationsName = obj.locationsName;
            try
            {
                return d;
            }
            catch (Exception e)
            {
                //  Log.AsyncM(e);
                return null;
            }
        }
        public static void serializeQuarry(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (Quarry)d);
        }
        public static void serializeQuarryFromWorld(CoreObject d)
        {
            Serialize.WriteToJsonFile(Path.Combine(objectsInWorldPath, d.thisLocation.name, d.Name + ".json"), (Quarry)d);
        }

        public static shopObject parseShopObject(string data)
        {

            dynamic obj = JObject.Parse(data);
            shopObject d = new shopObject(false);

            d.price = obj.price;
            d.Decoration_type = obj.Decoration_type;
            d.rotations = obj.rotations;
            d.currentRotation = obj.currentRotation;
            string s1 = Convert.ToString(obj.sourceRect);
            d.sourceRect = Util.parseRectFromJson(s1);
            string s2 = Convert.ToString(obj.defaultSourceRect);
            d.defaultSourceRect = Util.parseRectFromJson(s2);
            string s3 = Convert.ToString(obj.defaultBoundingBox);
            d.defaultBoundingBox = Util.parseRectFromJson(s3);
            d.description = obj.description;
            d.flipped = obj.flipped;
            d.flaggedForPickUp = obj.flaggedForPickUp;
            d.tileLocation = obj.tileLocation;
            d.parentSheetIndex = obj.parentSheetIndex;
            d.owner = obj.owner;
            d.name = obj.name;
            d.type = obj.type;
            d.canBeSetDown = obj.canBeSetDown;
            d.canBeGrabbed = obj.canBeGrabbed;
            d.isHoedirt = obj.isHoedirt;
            d.isSpawnedObject = obj.isSpawnedObject;
            d.questItem = obj.questItem;
            d.isOn = obj.isOn;
            d.fragility = obj.fragility;
            d.edibility = obj.edibility;
            d.stack = obj.stack;
            d.quality = obj.quality;
            d.bigCraftable = obj.bigCraftable;
            d.setOutdoors = obj.setOutdoors;
            d.setIndoors = obj.setIndoors;
            d.readyForHarvest = obj.readyForHarvest;
            d.showNextIndex = obj.showNextIndex;
            d.hasBeenPickedUpByFarmer = obj.hasBeenPickedUpByFarmer;
            d.isRecipe = obj.isRecipe;
            d.isLamp = obj.isLamp;
            d.heldObject = obj.heldObject;
            d.minutesUntilReady = obj.minutesUntilReady;
            string s4 = Convert.ToString(obj.boundingBox);
            d.boundingBox = Util.parseRectFromJson(s4);
            d.scale = obj.scale;
            d.lightSource = obj.lightSource;
            d.shakeTimer = obj.shakeTimer;
            d.internalSound = obj.internalSound;
            d.specialVariable = obj.specialVariable;
            d.category = obj.category;
            d.specialItem = obj.specialItem;
            d.hasBeenInInventory = obj.hasBeenInInventory;
            string t = obj.texturePath;
            d.TextureSheet = Game1.content.Load<Texture2D>(t);
            d.texturePath = t;
            JArray array = obj.inventory;
            //  d.inventory = array.ToObject<List<Item>>();
            d.inventory = parseInventoryList(array);


            //  Log.AsyncC(array);

            d.inventoryMaxSize = obj.inventoryMaxSize;
            d.itemReadyForHarvest = obj.itemReadyForHarvest;
            d.lightsOn = obj.lightsOn;
            d.thisLocation = obj.thisLocation;
            d.lightColor = obj.lightColor;
            d.thisType = obj.thisType;
            d.removable = obj.removable;
            d.locationsName = obj.locationsName;
            try
            {
                return d;
            }
            catch (Exception e)
            {
                //Log.AsyncM(e);
                return null;
            }
        }
        public static void serializeShopObject(Item d)
        {
            Serialize.WriteToJsonFile(Path.Combine(InvPath, d.Name + ".json"), (shopObject)d);
        }
        public static void serializeShopObjectFromWorld(CoreObject d)
        {
            Serialize.WriteToJsonFile(Path.Combine(objectsInWorldPath, d.thisLocation.name, d.Name + ".json"), (shopObject)d);
        }

        public static void parseTrackedTerrainDataNodeList(string data)
        {
            if (File.Exists(data))
            {
                Lists.trackedTerrainFeaturesDummyList = ReadFromJsonFile<List<TrackedTerrainDummyDataNode>>(data);
                foreach (var v in Lists.trackedTerrainFeaturesDummyList)
                {
                    GameLocation location = Game1.getLocationFromName(v.location);
                    Vector2 position = v.position;

                    TerrainFeature t;
                    bool ehh = location.terrainFeatures.TryGetValue(position, out t);

                    if (t == null)
                    {
                        //  Log.AsyncC("BOOOOO");
                    }

                    Lists.trackedTerrainFeatures.Add(new TrackedTerrainDataNode(location, (HoeDirt)t, position));
                    // Log.AsyncG("YAY");
                }
                Lists.trackedTerrainFeaturesDummyList.Clear();
            }
        }
        public static void serializeTrackedTerrainDataNodeList(List<TrackedTerrainDataNode> list)
        {
            Lists.trackedTerrainFeaturesDummyList.Clear();
            List<TrackedTerrainDataNode> removalList = new List<TrackedTerrainDataNode>();
            foreach (var v in Lists.trackedTerrainFeatures)
            {
                if ((v.terrainFeature as HoeDirt).crop == null)
                {
                    removalList.Add(v);
                    // Log.AsyncR("WHY REMOVE???");
                    continue;
                }

            }

            foreach (var v in removalList)
            {
                Lists.trackedTerrainFeatures.Remove(v);
            }
            removalList.Clear();

            foreach (var v in list)
            {
                Lists.trackedTerrainFeaturesDummyList.Add(new TrackedTerrainDummyDataNode(v.location.name, v.position));
            }


            if (File.Exists(Path.Combine(PlayerDataPath, "TrackedTerrainFeaturesList.json")))
            {
                File.Delete(Path.Combine(PlayerDataPath, "TrackedTerrainFeaturesList.json"));
            }
            if (Lists.trackedTerrainFeaturesDummyList.Count == 0)
            {
                File.Delete(Path.Combine(PlayerDataPath, "TrackedTerrainFeaturesList.json"));
                return;
            }

            WriteToJsonFile<List<TrackedTerrainDummyDataNode>>(Path.Combine(PlayerDataPath, "TrackedTerrainFeaturesList.json"), Lists.trackedTerrainFeaturesDummyList);
        }

        public static List<Item> parseInventoryList(JArray array)
        {

            if (array == null)
            {
                // Log.AsyncC("WTF");
                return new List<Item>();

            }

            List<Item> inventory = new List<Item>();

            foreach (var v in array)
            {
                string data = v.ToString();
                if (data != null)
                {
                    //  Log.AsyncM(data);
                    inventory.Add(parseItemFromJson(data));
                }
            }



            return inventory;

        }

        public abstract class JsonCreationConverter<T> : JsonConverter
        {
            /// <summary>
            /// Create an instance of objectType, based properties in the JSON object
            /// </summary>
            /// <param name="objectType">type of object expected</param>
            /// <param name="jObject">
            /// contents of JSON object that will be deserialized
            /// </param>
            /// <returns></returns>
            protected abstract T Create(Type objectType, JObject jObject);

            public override bool CanConvert(Type objectType)
            {
                return typeof(T).IsAssignableFrom(objectType);
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override object ReadJson(JsonReader reader,
                                            Type objectType,
                                             object existingValue,
                                             JsonSerializer serializer)
            {
                // Load JObject from stream
                JObject jObject = JObject.Load(reader);

                // Create target object based on JObject
                T target = Create(objectType, jObject);

                // Populate the object properties
                serializer.Populate(jObject.CreateReader(), target);

                return target;
            }



        }

        public class RectangleConverter : JsonCreationConverter<Microsoft.Xna.Framework.Rectangle>
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.NullValueHandling = NullValueHandling.Ignore;

               
               // throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.NullValueHandling = NullValueHandling.Ignore;
                ////Log.AsyncG("DESERIALIZE THE WORLD");
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            protected override Rectangle Create(Type objectType, JObject jObject)
            {
                /*
                if (FieldExists("Skill", jObject))
                {
                    return new Artist();
                }
                else if (FieldExists("Department", jObject))
                {
                    return new Employee();
                }
                else
                {
                    return new Person();
                }
                */

                return new Rectangle(0,0,0,0);
            }

            private bool FieldExists(string fieldName, JObject jObject)
            {
                return jObject[fieldName] != null;
            }

           
        }

        public static T deserializeObjectWithRectangles<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                ////Log.AsyncC(filePath);
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();

                JsonSerializerSettings settings = new JsonSerializerSettings();
                // settings.TypeNameHandling = TypeNameHandling.Auto;
                //settings.Formatting = Formatting.Indented;
                return JsonConvert.DeserializeObject<T>(fileContents,new RectangleConverter());
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
