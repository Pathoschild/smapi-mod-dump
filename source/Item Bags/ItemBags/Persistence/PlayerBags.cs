/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ItemBags.Persistence
{
    [XmlRoot(ElementName = "PlayerBags", Namespace = "")]
    public class PlayerBags
    {
        public const string OwnedBagsDataKey = "ownedbags";

        [XmlArray("Bags")]
        [XmlArrayItem("Bag")]
        public BagInstance[] Bags { get; set; }

        public PlayerBags()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            this.Bags = new BagInstance[] { };
        }

        /*private static JsonSerializerSettings JsonSettings { get; } = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace, // avoid issue where default ICollection<T> values are duplicated each time the config is loaded
            Converters = new List<JsonConverter>
            {
                //new SemanticVersionConverter(),
                new StringEnumConverter()
            }
        };*/

        internal static PlayerBags DeserializeFromCurrentSaveFile()
        {
            if (!Context.IsMultiplayer || Context.IsMainPlayer)
            {
                return ItemBagsMod.ModInstance.Helper.Data.ReadSaveData<PlayerBags>(OwnedBagsDataKey);
            }
            else
            {
                /*string FilePath = Path.Combine(Constants.SavesPath, Constants.SaveFolderName, string.Format("{0}.json", OwnedBagsDataKey));
                if (!File.Exists(FilePath))
                    return null;
                else
                    return JsonConvert.DeserializeObject<PlayerBags>(File.ReadAllText(FilePath), JsonSettings);*/
                return null;

                //return ItemBagsMod.ModInstance.Helper.Data.ReadJsonFile<PlayerBags>(FilePath);
            }
        }

        internal void SerializeToCurrentSaveFile()
        {
            if (!Context.IsMultiplayer || Context.IsMainPlayer)
            {
                ItemBagsMod.ModInstance.Helper.Data.WriteSaveData(OwnedBagsDataKey, this);
            }
            else
            {
                /*string FilePath = Path.Combine(Constants.SavesPath, Constants.SaveFolderName, string.Format("{0}.json", OwnedBagsDataKey));
                if (File.Exists(FilePath))
                {
                    JsonConvert.SerializeObject(this, Formatting.Indented, JsonSettings);
                    //ItemBagsMod.ModInstance.Helper.Data.WriteJsonFile(FilePath, this);
                }*/
            }
        }

        /*public const string SettingsFilename = "Mod_ItemBags_Data.xml";
        //public static string SettingsFilePath { get { return Path.Combine(<Path to current player's saves>, SettingsFilename); } }

        public void Serialize(out bool Successful, out Exception SerializationError)
        {
            XMLSerializer.Serialize(this, SettingsFilePath, out Successful, out SerializationError);
        }

        public static PlayerBags Deserialize(out bool Successful, out Exception DeserializationError)
        {
            return XMLSerializer.Deserialize<PlayerBags>(SettingsFilePath, out Successful, out DeserializationError);
        }*/

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }
}
