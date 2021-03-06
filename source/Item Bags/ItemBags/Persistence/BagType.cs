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
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ItemBags.Persistence
{
    [XmlRoot(ElementName = "BagType", Namespace = "")]
    public class BagType
    {
        [XmlElement("Id")]
        public string Id { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlRoot(ElementName = "SourceTexture", Namespace = "")]
        public enum SourceTexture
        {
            [XmlEnum("Maps\\springobjects")]
            [Description("Maps\\springobjects")]
            SpringObjects,
            [XmlEnum("TileSheets\\Craftables")]
            [Description("TileSheets\\Craftables")]
            Craftables,
            [XmlEnum("TileSheets\\debris")]
            [Description("TileSheets\\debris")]
            Debris,
            [XmlEnum("TileSheets\\tools")]
            [Description("TileSheets\\tools")]
            Tools,
            [XmlEnum("LooseSprites\\Cursors")]
            [Description("LooseSprites\\Cursors")]
            Cursors
        }
        [XmlElement("IconSourceTexture")]
        public SourceTexture IconSourceTexture { get; set; }

        [XmlElement("IconSourceRect")]
        public Rectangle IconSourceRect { get; set; }

        [XmlArray("BagSizes")]
        [XmlArrayItem("BagSizeConfig")]
        public BagSizeConfig[] SizeSettings { get; set; }

        public BagType()
        {
            InitializeDefaults();
        }

        public static string GetTranslatedName(BagType Type)
        {
            string TranslationKey = string.Format("{0}Name", Type.Name.Replace(" ", ""));
            try
            {
                string Translated = ItemBagsMod.Translate(TranslationKey);
                if (!string.IsNullOrEmpty(Translated))
                    return Translated;
                else
                    return Type.Name;
            }
            catch (Exception) { return Type.Name; }
        }

        public static string GetTranslatedDescription(BagType Type)
        {
            string TranslationKey = string.Format("{0}Description", Type.Name.Replace(" ", ""));
            try
            {
                string Translated = ItemBagsMod.Translate(TranslationKey);
                if (!string.IsNullOrEmpty(Translated))
                    return Translated;
                else
                    return Type.Description;
            }
            catch (Exception) { return Type.Description; }
        }

        public void SerializeToXML(string FilePath, out bool Successful, out Exception SerializationError)
        {
            XMLSerializer.Serialize(this, FilePath, out Successful, out SerializationError);
        }

        public static BagType DeserializeFromXML(string FilePath, out bool Successful, out Exception DeserializationError)
        {
            return XMLSerializer.Deserialize<BagType>(FilePath, out Successful, out DeserializationError);
        }

        private void InitializeDefaults()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Name = null;
            this.Description = null;
            this.IconSourceTexture = SourceTexture.SpringObjects;
            this.IconSourceRect = new Rectangle(0, 0, 16, 16);
            this.SizeSettings = new BagSizeConfig[]
            {
                new BagSizeConfig()
                {
                    Size = ContainerSize.Small,
                    Price = BagTypeFactory.DefaultPrices[ContainerSize.Small],
                    Sellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Pierre },
                    Items = new List<StoreableBagItem>()
                },
                new BagSizeConfig()
                {
                    Size = ContainerSize.Medium,
                    Price = BagTypeFactory.DefaultPrices[ContainerSize.Medium],
                    Sellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Pierre },
                    Items = new List<StoreableBagItem>()
                },
                new BagSizeConfig()
                {
                    Size = ContainerSize.Large,
                    Price = BagTypeFactory.DefaultPrices[ContainerSize.Large],
                    Sellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Pierre },
                    Items = new List<StoreableBagItem>()
                },
                new BagSizeConfig()
                {
                    Size = ContainerSize.Giant,
                    Price = BagTypeFactory.DefaultPrices[ContainerSize.Giant],
                    Sellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Pierre },
                    Items = new List<StoreableBagItem>()
                },
                new BagSizeConfig()
                {
                    Size = ContainerSize.Massive,
                    Price = BagTypeFactory.DefaultPrices[ContainerSize.Massive],
                    Sellers = new List<BagSizeConfig.BagShop>() { BagSizeConfig.BagShop.Pierre },
                    Items = new List<StoreableBagItem>()
                }
            };
        }

        internal static Texture2D GetIconTexture(SourceTexture Source)
        {
            if (Source == SourceTexture.SpringObjects)
                return Game1.objectSpriteSheet;
            else if (Source == SourceTexture.Craftables)
                return Game1.bigCraftableSpriteSheet;
            else if (Source == SourceTexture.Debris)
                return Game1.debrisSpriteSheet;
            else if (Source == SourceTexture.Tools)
                return Game1.toolSpriteSheet;
            else if (Source == SourceTexture.Cursors)
                return Game1.mouseCursors;
            else
                return null;
        }

        internal Texture2D GetIconTexture()
        {
            return GetIconTexture(this.IconSourceTexture);
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
