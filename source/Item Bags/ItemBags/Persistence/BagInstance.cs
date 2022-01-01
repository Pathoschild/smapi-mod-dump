/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Helpers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ItemBags.Persistence
{
    [XmlRoot(ElementName = "BagInstance", Namespace = "")]
    public class BagInstance
    {
        [XmlElement("InstanceId")]
        public int InstanceId { get; set; }
        [XmlElement("TypeId")]
        public string TypeId { get; set; }
        [XmlElement("Size")]
        public ContainerSize Size { get; set; }
        [XmlElement("Autofill")]
        public bool Autofill { get; set; }

#region Rucksack Properties
        [XmlElement("AutofillPriority")]
        public AutofillPriority AutofillPriority { get; set; }
        [XmlElement("SortProperty")]
        public SortingProperty SortProperty { get; set; }
        [XmlElement("SortOrder")]
        public SortingOrder SortOrder { get; set; }
        #endregion Rucksack Properties

#region Omni Bag Properties
        [XmlArray("NestedBags")]
        [XmlArrayItem("Bag")]
        public BagInstance[] NestedBags { get; set; }
        #endregion Omni Bag Properties

#region Standard Bag Properties
        [XmlElement("ExcludedAutofillItems")]
        public List<KeyValuePair<string, HashSet<ObjectQuality>>> ExcludedAutofillItems { get; set; }
#endregion Standard Bag Properties

        [XmlArray("Contents")]
        [XmlArrayItem("Item")]
        public BagItem[] Contents { get; set; }

        [XmlElement("IsCustomIcon")]
        public bool IsCustomIcon { get; set; }
        [XmlElement("OverriddenIcon")]
        public Rectangle OverriddenIcon { get; set; }

        public BagInstance()
        {
            InitializeDefaults();
        }

        public BagInstance(int Id, ItemBag Bag)
        {
            InitializeDefaults();
            this.InstanceId = Id;

            if (Bag is BoundedBag BoundedBag)
            {
                if (BoundedBag is BundleBag BundleBag)
                {
                    this.TypeId = BundleBag.BundleBagTypeId;
                }
                else
                {
                    this.TypeId = BoundedBag.TypeInfo.Id;
                    this.ExcludedAutofillItems = BoundedBag.ExcludedAutofillItems.Select(x => new KeyValuePair<string, HashSet<ObjectQuality>>(x.Key, x.Value)).ToList();
                }
                this.Autofill = BoundedBag.Autofill;
            }
            else if (Bag is Rucksack Rucksack)
            {
                this.TypeId = Rucksack.RucksackTypeId;
                this.Autofill = Rucksack.Autofill;
                this.AutofillPriority = Rucksack.AutofillPriority;
                this.SortProperty = Rucksack.SortProperty;
                this.SortOrder = Rucksack.SortOrder;
            }
            else if (Bag is OmniBag OmniBag)
            {
                this.TypeId = OmniBag.OmniBagTypeId;
                this.NestedBags = OmniBag.NestedBags.Select(x => new BagInstance(-1, x)).ToArray();
            }
            else
            {
                throw new NotImplementedException(string.Format("Logic for encoding Bag Type '{0}' is not implemented", Bag.GetType().ToString()));
            }

            this.Size = Bag.Size;
            if (Bag.Contents != null)
            {
                this.Contents = Bag.Contents.Where(x => x != null).Select(x => new BagItem(x)).ToArray();
            }

            if (!Bag.IsUsingCustomIcon)
            {
                this.IsCustomIcon = false;
                this.OverriddenIcon = new Rectangle();
            }
            else
            {
                this.IsCustomIcon = true;
                this.OverriddenIcon = Bag.CustomIconTexturePosition.Value;
            }
        }

        internal bool TryDecode(out ItemBag Decoded)
        {
            //  Handle BundleBags
            if (this.TypeId == BundleBag.BundleBagTypeId)
            {
                Decoded = new BundleBag(this);
                return true;
            }
            //  Handle Rucksacks
            else if (this.TypeId == Rucksack.RucksackTypeId)
            {
                Decoded = new Rucksack(this);
                return true;
            }
            //  Handle OmniBags
            else if (this.TypeId == OmniBag.OmniBagTypeId)
            {
                Decoded = new OmniBag(this);
                return true;
            }
            //  Handle all other types of Bags
            else if (ItemBagsMod.BagConfig.IndexedBagTypes.TryGetValue(this.TypeId, out BagType BagType))
            {
                BagSizeConfig SizeConfig = BagType.SizeSettings.FirstOrDefault(x => x.Size == this.Size);
                if (SizeConfig != null)
                {
                    Decoded = new BoundedBag(BagType, this);
                    return true;
                }
                else
                {
                    string Warning = string.Format("Warning - BagType with Id = {0} was found, but it does not contain any settings for Size={1}. Did you manually edit your {2} json file? The saved bag cannot be loaded without the corresponding settings for this size!",
                        this.TypeId, this.Size.ToString(), ItemBagsMod.BagConfigDataKey);
                    ItemBagsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);
                    Decoded = null;
                    return false;
                }
            }
            else
            {
                string Warning = string.Format("Warning - no BagType with Id = {0} was found. Did you manually edit your {1} json file or delete a .json file from 'Modded Bags' folder? The saved bag cannot be loaded without a corresponding type!",
                    this.TypeId, ItemBagsMod.BagConfigDataKey);
                ItemBagsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);
                Decoded = null;
                return false;
            }
        }

        private void InitializeDefaults()
        {
            this.InstanceId = -1;
            this.TypeId = Guid.Empty.ToString();
            this.Size = ContainerSize.Small;
            this.Autofill = false;
            this.Contents = new BagItem[] { };
            this.IsCustomIcon = false;
            this.OverriddenIcon = new Rectangle();
            this.AutofillPriority = AutofillPriority.Low;
            this.SortProperty = SortingProperty.Similarity;
            this.SortOrder = SortingOrder.Ascending;
            this.NestedBags = new BagInstance[] { };
            this.ExcludedAutofillItems = new List<KeyValuePair<string, HashSet<ObjectQuality>>>();
        }

        #region PyTK CustomElementHandler
        private const string PyTKSaveDataKey = "BagInstanceXmlString";
        private const string PyTKEqualsSignEncoding = "~~~";

        public Dictionary<string, string> ToPyTKAdditionalSaveData()
        {
            Dictionary<string, string> SaveData = new Dictionary<string, string>();

            if (XMLSerializer.TrySerializeToString(this, out string DataString, out Exception Error))
            {
                string CompatibleDataString = DataString.Replace("=", PyTKEqualsSignEncoding); // PyTK Mod doesn't like it when the Value contains '=' characters (the string will be truncated in ISaveElement.rebuild), so replace = with something else
                SaveData.Add(PyTKSaveDataKey, CompatibleDataString);
            }

            return SaveData;
        }

        public static BagInstance FromPyTKAdditionalSaveData(Dictionary<string, string> PyTKData)
        {
            if (PyTKData != null && PyTKData.TryGetValue(PyTKSaveDataKey, out string DataString))
            {
                if (XMLSerializer.TryDeserializeFromString(DataString.Replace(PyTKEqualsSignEncoding, "="), out BagInstance Data, out Exception Error))
                {
                    return Data;
                }
            }

            return null;
        }

        public static bool TryDeserializePyTKData(Item item, out BagInstance result)
        {
            string name = item.Name;

            //  Create a regex that will look for strings that start with PyTK serialization prefixes such as "PyTK|Item|ItemBags.Bags.BoundedBag,  ItemBags|BagInstanceXmlString="
            string namespacePrefix = @$"{nameof(ItemBags)}\.{nameof(Bags)}\."; // "ItemBags.Bags."
            List<string> classNames = new List<string>() { nameof(BoundedBag), nameof(BundleBag), nameof(Rucksack), nameof(OmniBag) };
            string classNamesPattern = $"({string.Join(@"|", classNames)})"; // "(BoundedBag|BundleBag|Rucksack|OmniBag)"
            string pattern = @$"^PyTK\|Item\|{namespacePrefix}{classNamesPattern},  ItemBags\|BagInstanceXmlString=";
            Regex prefix = new Regex(pattern);

            if (prefix.IsMatch(name))
            {
                string escapedXMLString = name.Replace(prefix.Match(name).Value, "");
                if (XMLSerializer.TryDeserializeFromString(escapedXMLString.Replace(PyTKEqualsSignEncoding, "="), out BagInstance Data, out Exception Error))
                {
                    result = Data;
                    return true;
                }
            }

            result = null;
            return false;
        }
        #endregion PyTK CustomElementHandler

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
