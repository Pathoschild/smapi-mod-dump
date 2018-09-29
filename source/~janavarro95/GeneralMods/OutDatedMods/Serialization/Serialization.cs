using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omegasis.CoreLibraries;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Serialization
{

    /// <summary>
    /// TODO: Find a way to serialize objects and tools.
    /// </summary>


   public class SerializationManager
    {
        public string objectsInWorldPath;
        public string playerInventoryPath;
        public string SerializerTrashPath;

        public Dictionary<string, SerializerDataNode> acceptedTypes = new Dictionary<string, SerializerDataNode>();

        public SerializationManager(string PlayerInventoryPath,string SerializerTrashPath,string ObjectsInWorldPath)
        {
            this.objectsInWorldPath = ObjectsInWorldPath;
            this.playerInventoryPath = PlayerInventoryPath;
            this.SerializerTrashPath = SerializerTrashPath;

            verifyAllDirectoriesExist();
        }

        private void verifyAllDirectoriesExist()
        {
            if (!Directory.Exists(this.playerInventoryPath)) Directory.CreateDirectory(this.playerInventoryPath);
            //if (!Directory.Exists(this.SerializerTrashPath)) Directory.CreateDirectory(this.playerInventoryPath);
            if (!Directory.Exists(this.objectsInWorldPath)) Directory.CreateDirectory(this.playerInventoryPath);
        }

        public void cleanUpInventory()
        {
            ProcessDirectoryForDeletion(playerInventoryPath);

            //ProcessDirectoryForDeletion(SerializerTrashPath);

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

        public void processDirectoryForDeserialization(string pathToXML,List<Item> thingsToAddBackIn)
        {
            string[] fileEntries = Directory.GetFiles(pathToXML);
            foreach(var fileName in fileEntries)
            {
                ProcessFileForCleanUp(fileName,thingsToAddBackIn);
            }

            string[] subDirectories = Directory.GetDirectories(pathToXML);
            foreach(var folder in subDirectories)
            {
                processDirectoryForDeserialization(folder,thingsToAddBackIn);
            }

        }


        public void ProcessFileForCleanUp(string path, List<Item> thingsToAddBackIn)
        {
            string[] ehh = File.ReadAllLines(path);
            string data = ehh[0];
            Item cObj;
            string a;
            string[] b;
            string s = "";
           // Log.AsyncC(path);
            //  Log.AsyncC(data);
            try
            {
                dynamic obj = JObject.Parse(data);


                //   Log.AsyncC(obj.thisType);

                a = obj.serializationName;
                b = a.Split(',');
                s = b.ElementAt(0);
                //   Log.AsyncC(s);
            }
            catch (Exception e)
            {

                //USE XML STYLE DESERIALIZING
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    var word = ParseXMLType(path);
                    if (pair.Key == word.ToString())
                    {
                        cObj = pair.Value.parse.Invoke(path);
                        (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);
                        (cObj as CoreObject).resetTexture();
                        if ((cObj as CoreObject).thisLocation == null)
                        {
                            Game1.player.addItemToInventory(cObj);
                            return;
                        }
                        else
                        {
                            (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).tileLocation, (StardewValley.Object)cObj);
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
                //  Log.AsyncC("FUUUUU");
                foreach (KeyValuePair<string, SerializerDataNode> pair in acceptedTypes)
                {
                    if (pair.Key == s)
                    {
                        try
                        {
                            //parse from Json Style
                            cObj = (CoreObject)pair.Value.parse.Invoke(data);
                            (cObj as CoreObject).thisLocation = Game1.getLocationFromName((cObj as CoreObject).locationsName);

                            if ((cObj as CoreObject).thisLocation == null)
                            {
                                Game1.player.addItemToInventory(cObj);
                                return;
                            }
                            else
                            {
                                (cObj as CoreObject).thisLocation.objects.Add((cObj as CoreObject).tileLocation,(StardewValley.Object)cObj);
                                thingsToAddBackIn.Add(cObj);
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

    }
}
