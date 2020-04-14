using MachineAugmentors.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MachineAugmentors
{
    [XmlRoot(ElementName = "MachineConfig", Namespace = "")]
    public class MachineConfig
    {
        [XmlArray("Machines")]
        [XmlArrayItem("Machine")]
        public AugmentableMachine[] Machines { get; set; }

        public MachineConfig()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            this.Machines = new AugmentableMachine[]
            {
                //new AugmentableMachine("Hadi.JASoda", "Syrup Maker", 379, true, false) // Sample
            };
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

    [XmlRoot(ElementName = "Machine", Namespace = "")]
    public class AugmentableMachine
    {
        /// <summary>The "UniqueID" of the mod's manifest that this machine belongs to.</summary>
        [XmlElement("RequiredModUniqueId")]
        public string RequiredModUniqueId { get; set; }
        /// <summary>The internal name of the item (Not the <see cref="StardewValley.Object.DisplayName"/>).</summary>
        [XmlElement("Name")]
        public string Name { get; set; }
        /// <summary><see cref="StardewValley.Item.ParentSheetIndex"/></summary>
        [XmlElement("Id")]
        public int Id { get; set; }

        /// <summary>True if this machine produces output items that can have different <see cref="StardewValley.Object.Quality"/> values.<para/>
        /// For example, this would be true for Mayonnaise machines, but false for Furnaces</summary>
        [XmlElement("HasQualityProducts")]
        public bool HasQualityProducts { get; set; }

        /// <summary>True if the machine requires an input item for each cycle of processing.<para/>This would be false for things like Bee Hives or Tappers, and also for Crystalarium  
        /// (since it only needs an initial input, and then will continue producing forever)<para/>True for things like Furnaces.</summary>
        [XmlElement("RequiresInput")]
        public bool RequiresInput { get; set; }

        [XmlArray("AttachableAugmentors")]
        [XmlArrayItem("AugmentorType")]
        public string[] AttachableAugmentors { get; set; }

        public AugmentableMachine()
        {
            InitializeDefaults();
        }

        public AugmentableMachine(string RequiredModUniqueId, string Name, int Id, bool RequiresInput, bool HasQualityProducts, params AugmentorType[] AttachableTypes)
        {
            InitializeDefaults();
            this.RequiredModUniqueId = RequiredModUniqueId;
            this.Name = Name;
            this.Id = Id;
            this.RequiresInput = RequiresInput;
            this.HasQualityProducts = HasQualityProducts;
            if (!AttachableAugmentors.Any())
                this.AttachableAugmentors = Enum.GetValues(typeof(AugmentorType)).Cast<AugmentorType>().Select(x => x.ToString()).ToArray();
            else
                this.AttachableAugmentors = AttachableAugmentors;
        }

        private void InitializeDefaults()
        {
            this.RequiredModUniqueId = "";
            this.Name = "";
            this.Id = MachineInfo.InvalidId;
            this.RequiresInput = true;
            this.HasQualityProducts = false;
            this.AttachableAugmentors = Enum.GetValues(typeof(AugmentorType)).Cast<AugmentorType>().Select(x => x.ToString()).ToArray();
        }

        internal MachineInfo ToMachineInfo()
        {
            List<AugmentorType> Types = new List<AugmentorType>();
            foreach (string TypeName in AttachableAugmentors)
            {
                if (Enum.TryParse(TypeName, out AugmentorType Type))
                    Types.Add(Type);
            }
            return new MachineInfo(Name, Id, HasQualityProducts, RequiresInput, Types.ToArray());
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
