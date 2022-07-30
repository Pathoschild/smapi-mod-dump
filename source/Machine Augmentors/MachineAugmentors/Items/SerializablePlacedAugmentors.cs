/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MachineAugmentors.Items
{
    [XmlRoot(ElementName = "PlacedAugmentors", Namespace = "")]
    public class SerializablePlacedAugmentors
    {
        [XmlArray("AugmentedLocations")]
        [XmlArrayItem("AugmentedLocation")]
        public SerializableAugmentedLocation[] Locations { get; set; }

        public SerializablePlacedAugmentors()
        {
            InitializeDefaults();
        }

        public SerializablePlacedAugmentors(PlacedAugmentorsManager CopyFrom)
        {
            InitializeDefaults();
            if (CopyFrom != null)
            {
                this.Locations = CopyFrom.Locations.Select(x => new SerializableAugmentedLocation(x.Value)).ToArray();
            }
        }

        private void InitializeDefaults()
        {
            this.Locations = new SerializableAugmentedLocation[] { };
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    [XmlRoot(ElementName = "AugmentedLocation", Namespace = "")]
    public class SerializableAugmentedLocation
    {
        [JsonProperty("UniqueLocationName")]
        [XmlElement("UniqueLocationName")]
        public string UniqueLocationName { get; set; }

        [XmlArray("AugmentedTile")]
        [XmlArrayItem("AugmentedTile")]
        public SerializableAugmentedTile[] Tiles { get; set; }

        public SerializableAugmentedLocation()
        {
            InitializeDefaults();
        }

        public SerializableAugmentedLocation(AugmentedLocation CopyFrom)
        {
            InitializeDefaults();
            if (CopyFrom != null)
            {
                this.UniqueLocationName = CopyFrom.UniqueLocationName;
                this.Tiles = CopyFrom.Tiles.Select(x => new SerializableAugmentedTile(x.Value)).ToArray();
            }
        }

        private void InitializeDefaults()
        {
            this.UniqueLocationName = null;
            this.Tiles = new SerializableAugmentedTile[] { };
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext sc) { }
        [OnSerialized]
        private void OnSerialized(StreamingContext sc) { }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc) { InitializeDefaults(); }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc) { }
    }

    [XmlRoot(ElementName = "AugmentedTile", Namespace = "")]
    public class SerializableAugmentedTile
    {
        [JsonProperty("XPosition")]
        [XmlElement("XPosition")]
        public int TileXPosition { get; set; }

        [JsonProperty("YPosition")]
        [XmlElement("YPosition")]
        public int TileYPosition { get; set; }

        [XmlArray("AugmentorTypes")]
        [XmlArrayItem("Type")]
        public AugmentorType[] AugmentorTypes { get; set; }

        [XmlArray("AugmentorQuantities")]
        [XmlArrayItem("Quantity")]
        public int[] AugmentorQuantities { get; set; }

        public SerializableAugmentedTile()
        {
            InitializeDefaults();
        }

        public SerializableAugmentedTile(AugmentedTile CopyFrom)
        {
            InitializeDefaults();

            if (CopyFrom != null)
            {
                this.TileXPosition = CopyFrom.Position.X;
                this.TileYPosition = CopyFrom.Position.Y;

                List<AugmentorType> TempTypes = new List<AugmentorType>();
                List<int> TempQuantities = new List<int>();
                foreach (KeyValuePair<AugmentorType, int> KVP in CopyFrom.Quantities)
                {
                    AugmentorType Type = KVP.Key;
                    int Quantity = KVP.Value;
                    if (Quantity > 0)
                    {
                        TempTypes.Add(Type);
                        TempQuantities.Add(Quantity);
                    }
                }

                this.AugmentorTypes = TempTypes.ToArray();
                this.AugmentorQuantities = TempQuantities.ToArray();
            }
        }

        private void InitializeDefaults()
        {
            this.TileXPosition = -1;
            this.TileYPosition = -1;
            this.AugmentorTypes = new AugmentorType[] { };
            this.AugmentorQuantities = new int[] { };
        }

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
