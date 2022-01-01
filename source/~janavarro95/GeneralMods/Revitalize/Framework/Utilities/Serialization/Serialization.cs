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
using Revitalize;
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
        /// The settings used by the seralizer
        /// </summary>
        private JsonSerializerSettings settings;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Serializer()
        {
            this.serializer = new JsonSerializer();
            this.serializer.Formatting = Formatting.Indented;
            this.serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this.serializer.NullValueHandling = NullValueHandling.Include;
            this.serializer.TypeNameHandling = TypeNameHandling.All;

            this.addConverter(new Framework.Utilities.Serialization.Converters.RectangleConverter());
            this.addConverter(new Framework.Utilities.Serialization.Converters.Texture2DConverter());

            this.settings = new JsonSerializerSettings();
            foreach (JsonConverter converter in this.serializer.Converters)
            {
                this.settings.Converters.Add(converter);
            }
            this.settings.Formatting = Formatting.Indented;
            this.settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this.settings.NullValueHandling = NullValueHandling.Include;
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
            string path = Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", extensionFolder, fileName + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (File.Exists(path)) return;
            this.Serialize(path, obj);
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
