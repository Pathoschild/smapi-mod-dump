/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using StardewValley.Objects;
using System.Collections.ObjectModel;
using Object = StardewValley.Object;
using ItemBags.Menus;
using ItemBags.Persistence;
using ItemBags.Helpers;
using System.Runtime.Serialization;

namespace ItemBags.Bags
{
    /// <summary>Represents an <see cref="ItemBag"/> that can only store certain, pre-defined Items in it.</summary>
    [XmlType("Mods_BoundedBag")]
    [XmlRoot(ElementName = "BoundedBag", Namespace = "")]
    [KnownType(typeof(BundleBag))]
    [XmlInclude(typeof(BundleBag))]
    public class BoundedBag : ItemBag
    {
        public class AllowedObject
        {
            public string Id { get; }
            public ReadOnlyCollection<ObjectQuality> Qualities { get; }
            public bool HasQualities { get; }
            public bool IsBigCraftable { get; }

            public const int DefaultQuality = 0;

            public AllowedObject(StoreableBagItem Item)
            {
                this.Id = Item.Id;
                this.HasQualities = Item.HasQualities;
                this.IsBigCraftable = Item.IsBigCraftable;

                if (Item.Qualities == null || !HasQualities)
                    this.Qualities = new ReadOnlyCollection<ObjectQuality>(Enum.GetValues(typeof(ObjectQuality)).Cast<ObjectQuality>().ToList());
                else
                    this.Qualities = new ReadOnlyCollection<ObjectQuality>(Item.Qualities);
            }

            internal bool IsValidQuality(Object Item)
            {
                if (!HasQualities)
                    return Item.Quality == DefaultQuality;
                else
                    return Qualities.Any(x => (int)x == Item.Quality);
            }

            public bool IsMatch(Object Item)
            {
                return Item != null && Item.ItemId == this.Id && Item.bigCraftable.Value == this.IsBigCraftable && IsValidQuality(Item);
            }

            public override string ToString()
            {
                return string.Format("Id={0}, BigCraftable={1}, Qualities={2}", Id, IsBigCraftable, string.Join(",", Qualities.Select(x => x.ToString())));
            }
        }

        /// <summary>If true, then when the player picks up an item that can be stored in this bag, it will automatically be stored if there is space for it.<para/>
        /// If multiple <see cref="BoundedBag"/> objects can store the item and have <see cref="Autofill"/>=true, then the item will be stored in the first one we can find that already has a stack of that item in it.</summary>
        [XmlElement("Autofill")]
        public bool Autofill { get; set; }

        /// <summary>Key = the DisplayName of an item that is skipped when autofilling. Value = the Qualities of that item to skip.</summary>
        [XmlIgnore]
        public Dictionary<string, HashSet<ObjectQuality>> ExcludedAutofillItems { get; private set; }

        [XmlArray("ExcludedAutofillItems")]
        [XmlArrayItem("ExcludedAutofillItem")]
        public string[] SerializableExcludedAutofillItems
        {
            get
            {
                if (ExcludedAutofillItems == null)
                    return new string[] { };
                else
                    return ExcludedAutofillItems.Select(x => EncodeExcludedItem(x.Key, x.Value)).ToArray();
            }
            set
            {
                ExcludedAutofillItems = new Dictionary<string, HashSet<ObjectQuality>>();
                if (value != null)
                {
                    foreach (string EncodedData in value)
                    {
                        if (TryDecodeExcludedItem(EncodedData, out string Name, out HashSet<ObjectQuality> Qualities))
                            ExcludedAutofillItems[Name] = Qualities;
                    }
                }
            }
        }

        private static string EncodeExcludedItem(string Name, HashSet<ObjectQuality> Qualities)
        {
            string Delimiter = "||";
            return string.Format("{0}{1}{2}", Name, Delimiter, string.Join(Delimiter, Qualities.Select(x => x.ToString())));
        }

        private static bool TryDecodeExcludedItem(string Data, out string Name, out HashSet<ObjectQuality> Qualities)
        {
            Name = null;
            Qualities = null;

            if (string.IsNullOrEmpty(Data))
                return false;

            string Delimiter = "||";
            string[] Fields = Data.Split(new List<string>() { Delimiter }.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            Name = Fields[0];
            Qualities = new HashSet<ObjectQuality>(Fields.Skip(1).Select(x => (ObjectQuality)Enum.Parse(typeof(ObjectQuality), x)));
            return true;
        }

        public void ToggleItemAutofill(Object Item)
        {
            if (Item != null)
            {
                if (Enum.IsDefined(typeof(ObjectQuality), Item.Quality))
                {
                    ObjectQuality ItemQuality = (ObjectQuality)Item.Quality;
                    if (ExcludedAutofillItems.TryGetValue(Item.DisplayName, out HashSet<ObjectQuality> ExcludedQualities))
                    {
                        if (ExcludedQualities.Contains(ItemQuality))
                        {
                            ExcludedQualities.Remove(ItemQuality);
                            if (!ExcludedQualities.Any())
                                ExcludedAutofillItems.Remove(Item.DisplayName);
                        }
                        else
                            ExcludedQualities.Add(ItemQuality);
                    }
                    else
                    {
                        ExcludedQualities = new HashSet<ObjectQuality>();
                        ExcludedQualities.Add(ItemQuality);
                        ExcludedAutofillItems.Add(Item.DisplayName, ExcludedQualities);
                    }
                }
            }
        }

        /// <summary>Returns true if this Bag isn't capable of storing more Quantity of the given Item (Either because the Item is not valid for this bag, or because maximum capacity has been reached)</summary>
        public override bool IsFull(Object Item)
        {
            if (!IsValidBagObject(Item))
                return true;
            else
            {
                Object BagItem = this.Contents.FirstOrDefault(x => AreItemsEquivalent(x, Item, false, true));
                if (BagItem == null)
                    return false;
                else
                    return BagItem.Stack >= GetMaxStackSize(Item);
            }
        }

        /// <summary>The Objects that can be stored in this bag.</summary>
        [XmlIgnore]
        public ReadOnlyCollection<AllowedObject> AllowedObjects { get; protected set; }

        [XmlIgnore]
        private BagType _TypeInfo { get; set; }
        /// <summary>The type that this Bag instance is using. The <see cref="BagType"/> defines things like the name, description, what kinds of items can be stored etc. 
        /// A <see cref="BagType"/> is unique, but there can be multiple <see cref="BoundedBag"/> instances that are using the same type's metadata.</summary>
        [XmlIgnore]
        public BagType TypeInfo
        {
            get { return _TypeInfo; }
            protected set
            {
                _TypeInfo = value;

                if (TypeInfo == null)
                {
                    SizeInfo = null;
                }
                else
                {
                    DefaultIconTexture = TypeInfo.GetIconTexture();
                    DefaultIconTexturePosition = TypeInfo.IconSourceRect;

                    SizeInfo = TypeInfo.SizeSettings.FirstOrDefault(x => x.Size == this.Size);
                    if (SizeInfo == null) // This should never happen but just in case...
                        SizeInfo = TypeInfo.SizeSettings.First();

                    _MaxStackSize = ItemBagsMod.UserConfig.GetStandardBagCapacity(Size, TypeInfo);

                    BaseName = BagType.GetTranslatedName(TypeInfo);
                    DescriptionAlias = string.Format("{0}\n({1})",
                        BagType.GetTranslatedDescription(TypeInfo),
                        ItemBagsMod.Translate("CapacityDescription", new Dictionary<string, string>() { { "count", MaxStackSize.ToString() } }));

                    if (SizeInfo.Size != Size)
                        this.AllowedObjects = new List<AllowedObject>().AsReadOnly();
                    else
                        this.AllowedObjects = new ReadOnlyCollection<AllowedObject>(SizeInfo.Items.Select(x => new AllowedObject(x)).ToList());
                }
            }
        }

        [XmlElement("BagTypeId")]
        public string TypeId {
            get { return TypeInfo?.Id ?? ""; }
            set { SetTypeId(value); }
        }

        private void SetTypeId(string value)
        {
            if (string.IsNullOrEmpty(value) || ItemBagsMod.BagConfig?.IndexedBagTypes == null)
            {
                TypeInfo = null;
            }
            else
            {
                if (ItemBagsMod.BagConfig.IndexedBagTypes.TryGetValue(value, out BagType Type))
                    TypeInfo = Type;
                else
                    TypeInfo = null;
            }
        }

        [XmlIgnore]
        public BagSizeConfig SizeInfo { get; protected set; }

        [XmlIgnore]
        private int _MaxStackSize { get; set; }
        [XmlIgnore]
        public override int MaxStackSize { get { return _MaxStackSize; } }

        /// <summary>Default parameterless constructor intended for use by XML Serialization. Do not use this constructor to instantiate a bag.</summary>
        public BoundedBag()
            : base(ItemBagsMod.Translate("DefaultBagName"), ItemBagsMod.Translate("DefaultBagDescription"), ContainerSize.Small, null, null, new Vector2(16, 16), 0.5f, 1f)
        {
            this.TypeInfo = ItemBagsMod.BagConfig.BagTypes.First();
            this.Autofill = false;
            this.ExcludedAutofillItems = new Dictionary<string, HashSet<ObjectQuality>>();
        }

        /// <param name="Autofill">If true, then when the player picks up an item that can be stored in this bag, it will automatically be stored if there is space for it.</param>
        public BoundedBag(BagType TypeInfo, ContainerSize Size, bool Autofill)
            : base(BagType.GetTranslatedName(TypeInfo), "", Size, TypeInfo.GetIconTexture(), TypeInfo.IconSourceRect, new Vector2(16, 16), 0.5f, 1f)
        {
            this.TypeInfo = TypeInfo;
            this.Autofill = Autofill;
            this.ExcludedAutofillItems = new Dictionary<string, HashSet<ObjectQuality>>();
            OnSizeChanged += BoundedBag_OnSizeChanged;
        }

        private void BoundedBag_OnSizeChanged(object sender, EventArgs e)
        {
            SetTypeId(this.TypeId); // Force TypeInfo to be re-initialized
        }

        public BoundedBag(BagType TypeInfo, BagInstance SavedData)
            : this(TypeInfo, SavedData.Size, SavedData.Autofill)
        {
            foreach (BagItem Item in SavedData.Contents)
            {
                this.Contents.Add(Item.ToObject());
            }

            if (SavedData.IsCustomIcon)
            {
                this.CustomIconSourceTexture = BagType.SourceTexture.SpringObjects;
                this.CustomIconTexturePosition = SavedData.OverriddenIcon;
            }

            this.ExcludedAutofillItems = new Dictionary<string, HashSet<ObjectQuality>>();
            foreach (var KVP in SavedData.ExcludedAutofillItems)
                this.ExcludedAutofillItems.Add(KVP.Key, KVP.Value);
        }

        /// <summary>Intended to only be used when instantiating a <see cref="BundleBag"/></summary>
        protected BoundedBag(string BaseName, string Description, ContainerSize Size, bool Autofill)
            : base(BaseName, Description, Size, TextureHelpers.JunimoNoteTexture, new Rectangle(0, 244, 16, 16), new Vector2(16, 16), 0.5f, 1f)
        {
            this.Autofill = Autofill;
            this.ExcludedAutofillItems = new Dictionary<string, HashSet<ObjectQuality>>();
        }

        public bool CanAutofillWithItem(Object item)
        {
            if (item == null)
            {
                return false;
            }
            else if (ExcludedAutofillItems.TryGetValue(item.DisplayName, out HashSet<ObjectQuality> ExcludedQualities))
            {
                if (!Enum.IsDefined(typeof(ObjectQuality), item.Quality))
                {
                    return true;
                }
                else
                {
                    ObjectQuality ItemQuality = (ObjectQuality)item.Quality;
                    return !ExcludedQualities.Contains(ItemQuality);
                }
            }
            else
            {
                return true;
            }
        }

        protected override void LoadSettings(BagInstance Data)
        {
            if (Data != null)
            {
                this.Size = Data.Size;
                this.Autofill = Data.Autofill;
                this.ExcludedAutofillItems = new Dictionary<string, HashSet<ObjectQuality>>();
                foreach (var KVP in Data.ExcludedAutofillItems)
                    this.ExcludedAutofillItems.Add(KVP.Key, KVP.Value);

                //  Load the type
                this.TypeInfo = ItemBagsMod.BagConfig.BagTypes.FirstOrDefault(x => x.Id == Data.TypeId);
                if (TypeInfo == null)
                {
                    string Warning = string.Format("Warning - no BagType with Id = {0} was found. Did you manually edit your {1} json file or delete a .json file from 'Modded Bags' folder? The saved bag cannot be properly loaded without a corresponding type!"
                        + " To prevent crashes, this bag will be automatically converted to a default BagType.", Data.TypeId, ItemBagsMod.BagConfigDataKey);
                    ItemBagsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);

                    //  To prevent crashes, convert this bag into a different bag type that exists
                    this.TypeInfo = ItemBagsMod.BagConfig.GetDefaultBoundedBagType();
                }

                //  Load the size configuration
                this.SizeInfo = TypeInfo.SizeSettings.FirstOrDefault(x => x.Size == Size);
                if (SizeInfo == null)
                {
                    string Warning = string.Format("Warning - BagType with Id = {0} does not contain any settings for Size={1}. Did you manually edit your {2} json file?"
                        + " The saved bag cannot be properly loaded without the corresponding settings for this size! To prevent crashes, this bag will be automatically converted to a default size for this BagType.",
                        this.TypeInfo.Id, this.Size.ToString(), ItemBagsMod.BagConfigDataKey);
                    ItemBagsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);

                    this.SizeInfo = TypeInfo.SizeSettings.First();
                }

                _MaxStackSize = ItemBagsMod.UserConfig.GetStandardBagCapacity(Size, TypeInfo);

                this.BaseName = BagType.GetTranslatedName(TypeInfo);
                DescriptionAlias = string.Format("{0}\n({1})",
                    BagType.GetTranslatedDescription(TypeInfo),
                    ItemBagsMod.Translate("CapacityDescription", new Dictionary<string, string>() { { "count", MaxStackSize.ToString() } }));

                if (SizeInfo.Size != Size)
                    this.AllowedObjects = new ReadOnlyCollection<AllowedObject>(new List<AllowedObject>());
                else
                    this.AllowedObjects = new ReadOnlyCollection<AllowedObject>(SizeInfo.Items.Select(x => new AllowedObject(x)).ToList());

                this.Contents.Clear();
                foreach (BagItem Item in Data.Contents)
                {
                    this.Contents.Add(Item.ToObject());
                }

                if (Data.IsCustomIcon)
                {
                    this.CustomIconSourceTexture = BagType.SourceTexture.SpringObjects;
                    this.CustomIconTexturePosition = Data.OverriddenIcon;
                }
                else
                {
                    ResetIcon();
                }
            }
        }

        internal override bool OnJsonAssetsItemIdsFixed(IJsonAssetsAPI API, bool AllowResyncing)
        {
            if (SizeInfo != null)
                this.AllowedObjects = new ReadOnlyCollection<AllowedObject>(SizeInfo.Items.Select(x => new AllowedObject(x)).ToList());
            return ValidateContentsIds(API, AllowResyncing);
        }

        public override void ResetIcon()
        {
            this.DefaultIconTexture = TypeInfo.GetIconTexture();
            this.DefaultIconTexturePosition = TypeInfo.IconSourceRect;
            this.CustomIconSourceTexture = null;
            this.CustomIconTexturePosition = null;
        }

        public override int GetPurchasePrice() { return ItemBagsMod.UserConfig.GetStandardBagPrice(Size, TypeInfo); }
        public override string GetTypeId() { return TypeInfo?.Id; }

        /// <param name="InventorySource">Typically this is <see cref="Game1.player.Items"/> if this menu should display the player's inventory.</param>
        /// <param name="ActualCapacity">The maximum # of items that can be stored in the InventorySource list. Use <see cref="Game1.player.MaxItems"/> if moving to/from the inventory.</param>
        protected override ItemBagMenu CreateMenu(IList<Item> InventorySource, int ActualCapacity)
        {
            try
            {
                ItemBagMenu Menu = new ItemBagMenu(this, InventorySource, ActualCapacity, SizeInfo.MenuOptions);
                Menu.Content = new BoundedBagMenu(Menu, this, SizeInfo.MenuOptions, 12);
                return Menu;
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Unhandled error while creating BoundedBagMenu: {0}", ex.Message), LogLevel.Error);
                return null;
            }
        }

        public override bool IsValidBagObject(Object item)
        {
            if (!base.IsValidBagObject(item))
            {
                return false;
            }
            else
            {
                return AllowedObjects.Any(x => x.IsMatch(item));
            }
        }

        public override void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
        {
            //  If viewing this bag from a Shop, draw a custom tooltip that displays icons for each item the bag is capable of storing
            if (Game1.activeClickableMenu is ShopMenu)
            {
                int SlotSize = 32; // May want to try 48. 64 is probably too big especially for bags that can store a large # of different items
                int NumItems = SizeInfo.Items.Count;
                int Columns = Math.Min(12, NumItems);
                int Rows = NumItems == 0 ? 0 : (NumItems - 1) / Columns + 1;

                int TitleWidth = (int)(font.MeasureString(this.DisplayName).X * 1.5) + 24; // Not sure if this is the correct scale and margin that the game's default rendering of the title bar uses
                int TextWidth = (int)font.MeasureString(overrideText).X + 32; // Do not change this 32, it's the additional margin that the game uses around the description text
                int ItemsMargin = 24;
                int ItemsWidth = SlotSize * Columns + ItemsMargin * 2;
                int RequiredWidth = Math.Max(Math.Max(TextWidth, TitleWidth), ItemsWidth);

                int MarginAfterDescription = 24;
                int RequiredHeight = (int)font.MeasureString(overrideText).Y + MarginAfterDescription + Rows * SlotSize - 8 + (int)font.MeasureString("999").Y + 32;

                DrawHelpers.DrawBox(spriteBatch, new Rectangle(x, y, RequiredWidth, RequiredHeight));

                //  Draw the description text
                if (overrideText != null && !string.IsNullOrEmpty(overrideText.ToString()) && overrideText.ToString() != " ")
                {
                    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
                    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
                    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
                    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)), (Game1.textColor * 0.9f) * alpha);
                    y = y + (int)font.MeasureString(overrideText).Y;
                }

                //  Draw icons for each item that this bag is capable of storing
                y += MarginAfterDescription;
                int RowStartX = x + (RequiredWidth - SlotSize * Columns) / 2;
                int CurrentX = RowStartX;
                int CurrentIndex = 0;
                foreach (StoreableBagItem ItemInfo in SizeInfo.Items)
                {
                    if (CurrentIndex == Columns)
                    {
                        CurrentIndex = 0;
                        CurrentX = RowStartX;
                        y += SlotSize;
                    }

                    Object Item = ItemInfo.IsBigCraftable ? 
                        new Object(Vector2.Zero, ItemInfo.Id, false) : 
                        new Object(ItemInfo.Id, 0, false, -1, 0);

                    Rectangle Destination = new Rectangle(CurrentX, y, SlotSize, SlotSize);
                    spriteBatch.Draw(Game1.menuTexture, Destination, new Rectangle(128, 128, 64, 64), Color.White);
                    DrawHelpers.DrawItem(spriteBatch, Destination, Item, false, false, 1f, 1f, Color.White, Color.White);
                    CurrentX += SlotSize;
                    CurrentIndex++;
                }

                //  Finish filling in the current row with empty slots
                while (CurrentIndex < Columns)
                {
                    Rectangle Destination = new Rectangle(CurrentX, y, SlotSize, SlotSize);
                    spriteBatch.Draw(Game1.menuTexture, Destination, new Rectangle(64, 896, 64, 64), Color.White);
                    CurrentX += SlotSize;
                    CurrentIndex++;
                }

                y += SlotSize - 8;
            }
            else
            {
                base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);
            }
        }

        protected override Item GetOneNew() => new BoundedBag(TypeInfo, Size, Autofill);
        protected override void GetOneCopyFrom(Item source)
        {
            base.GetOneCopyFrom(source);
            if (source is BoundedBag bag)
            {
                TypeInfo = bag.TypeInfo;
                Size = bag.Size;
                Autofill = bag.Autofill;
                ExcludedAutofillItems = bag.ExcludedAutofillItems;
            }
        }
    }
}
