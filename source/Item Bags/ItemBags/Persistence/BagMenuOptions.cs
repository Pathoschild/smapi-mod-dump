/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Menus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ItemBags.Persistence
{
    [XmlRoot(ElementName = "MenuOptions", Namespace = "")]
    public class BagMenuOptions
    {
        [XmlRoot(ElementName = "GroupedLayoutOptions", Namespace = "")]
        public class GroupedLayout
        {
            [XmlElement("GroupsPerRow")]
            public int GroupsPerRow { get; set; }
            [XmlElement("ShowValueColumn")]
            public bool ShowValueColumn { get; set; }
            [XmlElement("SlotSize")]
            public int SlotSize { get; set; }

            public GroupedLayout()
            {
                InitializeDefaults();
            }

            public GroupedLayout GetCopy()
            {
                return new GroupedLayout()
                {
                    GroupsPerRow = this.GroupsPerRow,
                    ShowValueColumn = this.ShowValueColumn,
                    SlotSize = this.SlotSize
                };
            }

            private void InitializeDefaults()
            {
                this.GroupsPerRow = 3;
                this.ShowValueColumn = true;
                this.SlotSize = BagInventoryMenu.DefaultInventoryIconSize;
            }

            [OnSerializing]
            private void OnSerializing(StreamingContext sc) { }

            [OnSerialized]
            private void OnSerialized(StreamingContext sc) { }

            [OnDeserializing]
            private void OnDeserializing(StreamingContext sc)
            {
                InitializeDefaults();
            }

            [OnDeserialized]
            private void OnDeserialized(StreamingContext sc) { }
        }

        [XmlRoot(ElementName = "UngroupedLayoutOptions", Namespace = "")]
        public class UngroupedLayout
        {
            [XmlElement("Columns")]
            public int Columns { get; set; }
            [XmlArray("LineBreakIndices")]
            [XmlArrayItem("Index")]
            public int[] LineBreakIndices { get; set; }
            [XmlArray("LineBreakHeights")]
            [XmlArrayItem("Height")]
            public int[] LineBreakHeights { get; set; }
            [XmlElement("SlotSize")]
            public int SlotSize { get; set; }

            public UngroupedLayout()
            {
                InitializeDefaults();
            }

            public UngroupedLayout GetCopy()
            {
                return new UngroupedLayout()
                {
                    Columns = this.Columns,
                    LineBreakIndices = this.LineBreakIndices.ToArray(),
                    LineBreakHeights = this.LineBreakHeights.ToArray(),
                    SlotSize = this.SlotSize
                };
            }

            private void InitializeDefaults()
            {
                this.Columns = 12;
                this.LineBreakIndices = new int[] { };
                this.LineBreakHeights = new int[] { };
                this.SlotSize = BagInventoryMenu.DefaultInventoryIconSize;
            }

            [OnSerializing]
            private void OnSerializing(StreamingContext sc) { }

            [OnSerialized]
            private void OnSerialized(StreamingContext sc) { }

            [OnDeserializing]
            private void OnDeserializing(StreamingContext sc)
            {
                InitializeDefaults();
            }

            [OnDeserialized]
            private void OnDeserialized(StreamingContext sc) { }
        }

        [XmlElement("GroupByQuality")]
        public bool GroupByQuality { get; set; }
        [XmlElement("InventoryColumns")]
        public int InventoryColumns { get; set; }
        [XmlElement("InventorySlotSize")]
        public int InventorySlotSize { get; set; }
        [XmlElement("GroupedLayoutOptions")]
        public GroupedLayout GroupedLayoutOptions { get; set; }
        [XmlElement("UngroupedLayoutOptions")]
        public UngroupedLayout UngroupedLayoutOptions { get; set; }

        public BagMenuOptions()
        {
            InitializeDefaults();
        }

        public BagMenuOptions GetCopy()
        {
            return new BagMenuOptions()
            {
                GroupByQuality = this.GroupByQuality,
                InventoryColumns = this.InventoryColumns,
                InventorySlotSize = this.InventorySlotSize,
                GroupedLayoutOptions = this.GroupedLayoutOptions.GetCopy(),
                UngroupedLayoutOptions = this.UngroupedLayoutOptions.GetCopy()
            };
        }

        private void InitializeDefaults()
        {
            this.GroupByQuality = true;
            this.InventoryColumns = 12;
            this.InventorySlotSize = BagInventoryMenu.DefaultInventoryIconSize;

            this.GroupedLayoutOptions = new GroupedLayout();
            this.UngroupedLayoutOptions = new UngroupedLayout();
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
