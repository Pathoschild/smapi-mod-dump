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
using System.ComponentModel;
using Object = StardewValley.Object;
using StardewValley.Objects;
using ItemBags.Menus;
using ItemBags.Helpers;
using System.Xml.Serialization;
using ItemBags.Persistence;
using System.Runtime.Serialization;
using Netcode;

namespace ItemBags.Bags
{
    [XmlRoot(ElementName = "ContainerSize", Namespace = "")]
    public enum ContainerSize
    {
        [XmlEnum("Small")]
        [Description("Small")]
        Small = 0,
        [XmlEnum("Medium")]
        [Description("Medium")]
        Medium = 1,
        [XmlEnum("Large")]
        [Description("Large")]
        Large = 2,
        [XmlEnum("Giant")]
        [Description("Giant")]
        Giant = 3,
        [XmlEnum("Massive")]
        [Description("Massive")]
        Massive = 4
    }

    [XmlRoot(ElementName = "Quality", Namespace = "")]
    public enum ObjectQuality
    {
        [XmlEnum("Regular")]
        [Description("Regular")]
        Regular = 0,
        [XmlEnum("Silver")]
        [Description("Silver")]
        Silver = 1,
        [XmlEnum("Gold")]
        [Description("Gold")]
        Gold = 2,
        [XmlEnum("Iridium")]
        [Description("Iridium")]
        Iridium = 4 // Might not be correct. According the the game code, if (StardewValley.Object.Quality > 3) then it draws iridium icon. So maybe it's possible for multiple qualities to map to iridium? Not sure why they didn't just use (StardewValley.Object.Quality == 3)
    }

    [XmlRoot(ElementName = "ItemBag", Namespace = "")]
    [KnownType(typeof(BoundedBag))]
    [KnownType(typeof(BundleBag))]
    [KnownType(typeof(Rucksack))]
    [KnownType(typeof(OmniBag))]
    [XmlInclude(typeof(BoundedBag))]
    [XmlInclude(typeof(BundleBag))]
    [XmlInclude(typeof(Rucksack))]
    [XmlInclude(typeof(OmniBag))]
    public abstract class ItemBag : GenericTool
    {
        /// <summary>The default color tints to use when rendering the bag icon in your inventory/toolbar.</summary>
        protected static readonly Dictionary<ContainerSize, Color> BaseColorMasks = new Dictionary<ContainerSize, Color>()
        {
            { ContainerSize.Small, new Color(165, 165, 165) }, // StardewValley.Tool class does not have a 'stoneColor' so this is my best guess at a faithful stone-like color
            { ContainerSize.Medium, Tool.copperColor },
            { ContainerSize.Large, Tool.steelColor },
            { ContainerSize.Giant, Tool.goldColor },
            { ContainerSize.Massive, Tool.iridiumColor }
        };

        /// <summary>The source rectangles of the quality sprites within the <see cref="Game1.mouseCursors"/> spritesheet</summary>
        public static readonly Dictionary<ObjectQuality, Rectangle> QualityIconTexturePositions = new Dictionary<ObjectQuality, Rectangle>()
        {
            { ObjectQuality.Regular, new Rectangle(338, 392, 8, 8) },
            { ObjectQuality.Silver, new Rectangle(338, 400, 8, 8) },
            { ObjectQuality.Gold, new Rectangle(346, 400, 8, 8) },
            { ObjectQuality.Iridium, new Rectangle(346, 392, 8, 8) }
        };

        /// <summary>Determines the scale to use when rendering the bag icon in your inventory/toolbar. Using a value of 1.0 means the bag texture will fill the entire 64x64 inventory space, 
        /// but it looks a bit crowded and too big compared to other items in the inventory so I chose to cap the scale at a smaller value</summary>
        protected static readonly Dictionary<ContainerSize, float> BaseUIScale = new Dictionary<ContainerSize, float>()
        {
            { ContainerSize.Small, 0.56f },
            { ContainerSize.Medium, 0.63f },
            { ContainerSize.Large, 0.70f },
            { ContainerSize.Giant, 0.77f },
            { ContainerSize.Massive, 0.84f }
        };

        public enum RoundingMode
        {
            Floor,
            Ceiling,
            Round
        }

        /// <summary>
        /// Rounds the given <paramref name="Value"/> to the 2nd most significant digit's value.<para/>
        /// For example, 874 is rounded to nearest 10 = 870, while 16446 is rounded to nearest 1000 = 16000.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static int RoundIntegerToSecondMostSignificantDigit(int Value, RoundingMode Mode)
        {
            int NumDigits = DrawHelpers.GetNumDigits(Value);
            if (NumDigits == 1)
                return Value;
            else
            {
                int RoundingSignificance = (int)IntPower(10, (short)Math.Max(0, NumDigits - 2));
                if (RoundingSignificance < 1)
                    return Value;
                else
                {
                    //  Divide by the significance, then round, then multiply by the significance
                    //  EX: 16446 / 1000 = 16.446. 16.446 Rounds to 16.0. 16 * 1000 = 16000.
                    double Divided = Value / (RoundingSignificance * 1.0);
                    int Rounded;
                    if (Mode == RoundingMode.Round)
                        Rounded = (int)Math.Round(Divided, 0, MidpointRounding.AwayFromZero) * RoundingSignificance;
                    else if (Mode == RoundingMode.Floor)
                        Rounded = (int)Math.Floor(Divided) * RoundingSignificance;
                    else if (Mode == RoundingMode.Ceiling)
                        Rounded = (int)Math.Ceiling(Divided) * RoundingSignificance;
                    else
                        throw new NotImplementedException(string.Format("Unexpected Rounding Mode: {0}", Mode.ToString()));
                    return Rounded;
                }
            }
        }

        //  Taken from: https://stackoverflow.com/questions/383587/how-do-you-do-integer-exponentiation-in-c
        private static long IntPower(int x, short power)
        {
            if (power == 0) return 1;
            if (power == 1) return x;
            // ----------------------
            int n = 15;
            while ((power <<= 1) >= 0) n--;

            long tmp = x;
            while (--n > 0)
                tmp = tmp * tmp *
                     (((power <<= 1) < 0) ? x : 1);
            return tmp;
        }

        /// <summary>Returns the sell price of a single instance of the given Item (Stack=1)</summary>
        public static int GetSingleItemPrice(Item Item)
        {
            if (Item == null)
                return 0;
            else if (Item is Object Obj)
            {
                try { return Obj.sellToStorePrice(-1); }
                catch (NullReferenceException) { return 0; } // Not sure why but some modded items are throwing exceptions in sellToStorePrice. EX: "Drying Rack" from PPJA Artisan Valley mod.
            }
            else
                return Item.salePrice() / 2;
        }

        /// <summary>
        /// Compares the given Object's by their <see cref="Item.ParentSheetIndex"/>, <see cref="StardewValley.Object.bigCraftable"/>, <see cref="Object.Quality"/>, <see cref="Object.IsRecipe"/>.
        /// Does not compare object references or other variables such as Stack sizes.
        /// </summary>
        public static bool AreItemsEquivalent(Object object1, Object object2, bool ComparePrice)
        {
            //Possible TODO this method was originally intended to find existing stacks of the same item so that that I could combine the stacks.
            //I've recently realized there is an Item.canStackWith(Item) instance method.  The calls to this function should be refactored to use that. 
            //(But should also account for stacking between different Colors of ColoredObject instances that have the same Id/Quality/Price)
            if (object1 == null && object2 == null) // both null
                return true;
            else if (object1 == null || object2 == null) // 1 is null but the other isn't
                return false;
            else // both are non-null
            {
                //  Compare common properties
                if (object1.ParentSheetIndex != object2.ParentSheetIndex ||
                    object1.Quality != object2.Quality ||
                    object1.IsRecipe != object2.IsRecipe ||
                    object1.bigCraftable.Value != object2.bigCraftable.Value ||
                    (ComparePrice && (object1.Price != object2.Price)) ||
                    object1.Name != object2.Name)
                {
                    return false;
                }
                else
                {
                    //  Allow different colors of the same ColoredObject flowers to stack together
                    Type type1 = object1.GetType();
                    Type type2 = object2.GetType();
                    bool AreTypesCompatible = type1 == type2 || (type1 == typeof(Object) && type2 == typeof(ColoredObject)) || (type2 == typeof(Object) && type1 == typeof(ColoredObject));
                    return AreTypesCompatible;
                }
            }
        }

        /// <summary>Creates a copy of the given input Object but with a Stack size of 0.</summary>
        public static Object CreateCopy(Object Item)
        {
            int PreviousStack = Item.Stack;
            Object Copy;
            try { Copy = Item.getOne() as Object; }
            catch (NullReferenceException) { Copy = null; } // Not sure why but some modded items are throwing exceptions in getOne(). EX: "Drying Rack" from PPJA Artisan Valley mod.

            if (Copy != null && Copy.GetType() == Item.GetType() && Copy != Item)
            {
                Copy.Stack = 0;
                if (Item.Stack != PreviousStack)
                    ForceSetQuantity(Item, PreviousStack);
                return Copy;
            }
            else
            {
                //  This should probably never happen unless maybe a subclass of Object didn't properly override the getOne() method
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("ItemBags: Warning - Item.getOne() did not return a valid copy for {0}. A copy of this item might not be properly created.", Item.DisplayName), LogLevel.Warn);
                if (Item.bigCraftable.Value)
                {
                    return new Object(Item.TileLocation, Item.ParentSheetIndex, Item.IsRecipe) { Stack = 0, Price = Item.Price };
                }
                else
                {
                    return new Object(Item.ParentSheetIndex, 0, Item.IsRecipe, Item.Price, Item.Quality);
                }
            }
        }

        /// <summary>Attempts to find all ItemBags in the entire game (in the player inventory, inside chests, fridges, storage furniture etc)</summary>
        /// <param name="IncludeNestedBags">If true, bags inside of <see cref="OmniBag"/>s will be included</param>
        public static List<ItemBag> GetAllBags(bool IncludeNestedBags)
        {
            List<ItemBag> Results = new List<ItemBag>();
            foreach (ItemBag Bag in SaveLoadHelpers.GetAllInstances(x => x is ItemBag).Cast<ItemBag>())
            {
                Results.Add(Bag);
                if (IncludeNestedBags && Bag is OmniBag OB)
                    Results.AddRange(OB.NestedBags);
            }
            return Results;
        }

        #region Textures
        /// <summary>The unmodified LooseSprites/cursors.xnb texture</summary>
        protected static Texture2D CursorsTexture { get { return Game1.mouseCursors; } }
        /// <summary>The 16x16 portion of <see cref="CursorsTexture"/> that contains the bag icon</summary>
        private static Texture2D OriginalTexture { get; set; }
        /// <summary><see cref="OriginalTexture"/>, converted to Grayscale</summary>
        private static Texture2D GrayscaleTexture { get; set; }

        //Slightly modified version of: http://community.monogame.net/t/load-texture-as-16-color-grayscale-image/11641/7
        protected static Texture2D ToGrayScaledPalettable(Texture2D original, Rectangle SourceRect, bool makePaletted, int numberOfPalColors)
        {
            //make an empty bitmap the same size as original
            int PixelCount = SourceRect.Width * SourceRect.Height;
            Color[] PixelData = new Color[PixelCount];
            original.GetData(0, SourceRect, PixelData, 0, PixelCount);

            for (int i = SourceRect.Left; i < SourceRect.Right; i++)
            {
                for (int j = SourceRect.Top; j < SourceRect.Bottom; j++)
                {
                    //get the pixel from the original image
                    int index = i + j * original.Width;
                    Color originalColor = PixelData[index];

                    //create the grayscale version of the pixel
                    //float maxval = .3f + .59f + .11f + .79f;
                    //float grayScale = (((originalColor.R / 255f) * .3f) + ((originalColor.G / 255f) * .59f) + ((originalColor.B / 255f) * .11f) + ((originalColor.A / 255f) * .79f));
                    //grayScale = grayScale / maxval;

                    //https://stackoverflow.com/questions/18560814/dont-tint-but-colorize-sprite
                    float grayScale = (originalColor.R + originalColor.G + originalColor.B) / 3.0f / originalColor.A;

                    if (originalColor == Color.Transparent)
                    {
                        PixelData[index] = Color.Transparent;
                    }
                    else if (makePaletted)
                    {
                        var val = (int)((grayScale - .0001f) * numberOfPalColors);
                        PixelData[index] = new Color(val, val, val, 255);
                    }
                    else
                    {
                        PixelData[index] = new Color(grayScale, grayScale, grayScale, 1f);
                    }
                }
            }

            Texture2D Grayscaled = new Texture2D(Game1.graphics.GraphicsDevice, SourceRect.Width, SourceRect.Height);
            Grayscaled.SetData(PixelData);
            return Grayscaled;
        }

        /// <param name="MaskFactor">Value between 0.0 and 1.0. Just determines how much of an impact the Mask parameter can have on the result. 
        /// Smaller value means the Mask color will make less of a difference.<para/>
        /// Recommended to use a small value like 0.1-0.2 to maintain most of the color defined in <see cref="BaseColorMasks"/></param>
        /// <returns></returns>
        protected virtual Color GetTextureOverlay(float Transparency, Color Mask, float MaskFactor)
        {
            Color BaseColor = BaseColorMasks[Size] * Transparency;
            Vector3 MaskVector = Mask.ToVector3() * MaskFactor * byte.MaxValue + new Vector3(byte.MaxValue - (byte)(MaskFactor * byte.MaxValue));
            MaskVector = new Vector3(MaskVector.X / byte.MaxValue, MaskVector.Y / byte.MaxValue, MaskVector.Z / byte.MaxValue);
            return new Color((byte)(BaseColor.R * MaskVector.X), (byte)(BaseColor.G * MaskVector.Y), (byte)(BaseColor.B * MaskVector.Z), BaseColor.A);
        }

        private void LoadTextures()
        {
            if (OriginalTexture == null || OriginalTexture.IsDisposed)
            {
                //528 1435 16x16 LooseSprites/cursors.xnb
                Rectangle SourceRect = new Rectangle(528, 1435, 16, 16);
                int PixelCount = SourceRect.Width * SourceRect.Height;
                Color[] PixelData = new Color[PixelCount];
                CursorsTexture.GetData(0, SourceRect, PixelData, 0, PixelCount);
                OriginalTexture = new Texture2D(Game1.graphics.GraphicsDevice, SourceRect.Width, SourceRect.Height);
                OriginalTexture.SetData(PixelData);
            }

            if (GrayscaleTexture == null || GrayscaleTexture.IsDisposed)
            {
                GrayscaleTexture = ToGrayScaledPalettable(OriginalTexture, new Rectangle(0, 0, OriginalTexture.Width, OriginalTexture.Height), false, 0);

                //  Brighten the grayscale texture a little bit so it looks better when we apply color masks to it
                Rectangle SourceRect = new Rectangle(0, 0, GrayscaleTexture.Width, GrayscaleTexture.Height);
                int PixelCount = SourceRect.Width * SourceRect.Height;
                Color[] PixelData = new Color[PixelCount];
                GrayscaleTexture.GetData(0, SourceRect, PixelData, 0, PixelCount);
                for (int i = 0; i < PixelData.Length; i++)
                {
                    PixelData[i] = PixelData[i] * 1.8f;
                }
                GrayscaleTexture.SetData(PixelData);
            }
        }

        private static bool AreColorsAlmostEqual(Color a, Color z, int threshold = 50)
        {
            int r = (int)a.R - z.R;
            int g = (int)a.G - z.G;
            int b = (int)a.B - z.B;
            return (r * r + g * g + b * b) <= threshold * threshold;
        }
        #endregion Textures

        /// <summary>Same purpose as <see cref="Tool.Description"/> except this is a read-only property that doesn't call a function</summary>
        [XmlElement("DescriptionAlias")]
        public string DescriptionAlias { get; protected set; }

        [XmlElement("ContainerSize")]
        public ContainerSize Size { get; protected set; }
        /// <summary>Never Add/Remove from this List directly. Use <see cref="MoveToBag(Object, int, out int, bool, IList{Item})"/> and <see cref="MoveFromBag(Object, int, out int, bool, IList{Item}, int)"/></summary>
        [XmlArray("BagContents")]
        [XmlArrayItem("BagItem")]
        public List<Object> Contents { get; }
        /// <summary>Invoked when Items are added to or removed from the bag</summary>
        public EventHandler<EventArgs> OnContentsChanged;

        #region Lookup Anything Compatibility
        /// <summary>
        /// This property is only intended as read-only data, for use with the Lookup Anything mod (See also: https://github.com/Pathoschild/StardewMods/tree/develop/LookupAnything#extensibility-for-modders) 
        /// <para/>If you intend to modify the contents of the chest, use <see cref="MoveToBag(Object, int, out int, bool, IList{Item}, bool, bool)"/> or <see cref="MoveFromBag(Object, int, out int, bool, IList{Item}, int, bool, bool)"/>
        /// </summary>
        [XmlIgnore]
        public virtual Chest heldObject { get { return new Chest(0, Contents.Where(x => x != null).Cast<Item>().ToList(), Vector2.Zero); } }
        #endregion Lookup Anything Compatibility

        internal const int RecentlyModifiedHistorySize = 12;
        /// <summary>Key = An Object that was recently added to this bag's <see cref="Contents"/>, or had it's Quantity increased.<para/>
        /// Value = DateTime of when that event happened. Only stores up to 1 KeyValuePair for each distinct Object, and only the most recent <see cref="RecentlyModifiedHistorySize"/> changes</summary>
        [XmlIgnore]
        internal Dictionary<Object, DateTime> RecentlyModified { get; private set; }

        /// <summary>The sum of the value of all items stored in this bag</summary>
        [XmlIgnore]
        public int ContentsValue { get { return Contents == null ? 0 : Contents.Sum(x => GetSingleItemPrice(x) * x.Stack); } }
        public virtual bool IsEmpty() { return Contents == null || !Contents.Any(x => x != null); }

        [XmlIgnore]
        public Texture2D Icon { get; set; }
        /// <summary>The SourceRectangle portion of the <see cref="Icon"/> Texture</summary>
        [XmlElement("IconTexturePosition")]
        public Rectangle? IconTexturePosition { get; set; }

        /// <summary>An offset to use when rendering <see cref="Icon"/>. If zero/null, then the additional icon would appear in the center of the bag's inventory icon.</summary>
        [XmlElement("IconRenderOffset")]
        public Vector2? IconRenderOffset { get; }
        [XmlElement("IconScale")]
        public float IconScale { get; }
        [XmlElement("IconTransparency")]
        public float IconTransparency { get; }

        /// <summary>The maximum number of the same item that can be stored in a single slot of this bag.</summary>
        [XmlIgnore]
        public abstract int MaxStackSize { get; }
        protected virtual int GetMaxStackSize(Object Item) { return this.MaxStackSize; }

        public abstract string GetTypeId();
        protected abstract void LoadSettings(BagInstance Data);

        internal virtual bool OnJsonAssetsItemIdsFixed(IJsonAssetsAPI API, bool AllowResyncing)
        {
            return ValidateContentsIds(API, AllowResyncing);
        }

        private bool HasValidatedContentsIds { get; set; } = false;

        /// <summary>Intended to be invoked exactly one time per bag instance when a save file is loaded. Checks if an Item Id of a modded item added through Json Assets has changed since the last game session ended.<para/>
        /// This typically happens when mods that add items through Json Assets have been installed/uninstalled between game sessions. When an item's Id changes, it must be re-created with the correct Id or else the item would be transformed into a different Object.</summary>
        /// <param name="AllowResyncing">True to allow the bag data to be resynced across all multiplayer clients if a change was made.</param>
        protected bool ValidateContentsIds(IJsonAssetsAPI API, bool AllowResyncing)
        {
            if (HasValidatedContentsIds)
                return false;

            try
            {
                bool ChangesMade = false;
                for (int i = Contents.Count - 1; i >= 0; i--)
                {
                    Object Item = Contents[i];
                    if (Item != null)
                    {
                        int IdBeforeFixing = Item.ParentSheetIndex;

                        bool IsItemStillValid = !API.FixIdsInItem(Item);
                        if (!IsItemStillValid)
                        {
                            //  The mod that this item belongs to is no longer installed
                            this.Contents.RemoveAt(i);
                            ChangesMade = true;
                        }
                        else
                        {
                            int IdAfterFixing = Item.ParentSheetIndex;
                            if (IdBeforeFixing != IdAfterFixing)
                            {
                                //  Re-create the item with the new Id
                                Object Actual = new BagItem(Item).ToObject();
                                Contents[i] = Actual;
                                ChangesMade = true;

                                string Message = string.Format("Detected a change in a managed item's Id within Bag: {0}. Item Name = {1}, Previous Id = {2}, New Id = {3}", DisplayName, Actual.DisplayName, IdBeforeFixing, IdAfterFixing);
#if DEBUG
                                ItemBagsMod.ModInstance.Monitor.Log(Message, LogLevel.Debug);
#else
                            ItemBagsMod.ModInstance.Monitor.Log(Message, LogLevel.Trace);
#endif
                            }
                        }
                    }
                }

                if (AllowResyncing && ChangesMade && Context.IsMainPlayer)
                {
                    Resync();
                }

                return ChangesMade;
            }
            finally { HasValidatedContentsIds = true; }
        }

        /// <summary>Default parameterless constructor intended for use by XML Serialization. Do not use this constructor to instantiate a bag.</summary>
        private ItemBag() : base("", "", 0, Tool.wateringCanSpriteIndex, Tool.wateringCanMenuIndex)
        {
            BagInstanceString = new NetString(null);
            BagInstanceString.fieldChangeEvent += BagInstanceString_fieldChangeEvent;
            NetFields.AddField(BagInstanceString);

            Stackable = false;
            DisplayName = BaseName;
            InstantUse = true;

            this.Size = ContainerSize.Small;
            this.Contents = new List<Object>();
            this.RecentlyModified = new Dictionary<Object, DateTime>();
            this.DescriptionAlias = "";

            LoadTextures();
        }

        /// <param name="BaseName">The name of the bag without the size prefix. EX: "Gem Bag"</param>
        /// <param name="Icon">An additional texture to render over-top of the bag texture. For example, this texture might be a small emerald icon for a Gem-related bag. Can be null.</param>
        /// <param name="IconTexturePosition">The SourceRectangle portion of the <paramref name="Icon"/> Texture.</param>
        /// <param name="IconRenderOffset">An offset to use when rendering Parameter=<paramref name="Icon"/>. If zero/null, then the additional icon would appear in the center of the bag's inventory icon.</param>
        protected ItemBag(string BaseName, string Description, ContainerSize Size, Texture2D Icon = null, Rectangle? IconTexturePosition = null, 
            Vector2? IconRenderOffset = null, float IconScale = 1.0f, float IconTransparency = 1.0f)
            : base(BaseName, Description, 0, Tool.wateringCanSpriteIndex, Tool.wateringCanMenuIndex)
        {
            BagInstanceString = new NetString(null);
            BagInstanceString.fieldChangeEvent += BagInstanceString_fieldChangeEvent;
            NetFields.AddField(BagInstanceString);

            Stackable = false;
            DisplayName = BaseName;
            InstantUse = true;

            this.Size = Size;
            this.Contents = new List<Object>();
            this.RecentlyModified = new Dictionary<Object, DateTime>();
            this.DescriptionAlias = Description;

            this.Icon = Icon;
            this.IconTexturePosition = IconTexturePosition;
            this.IconRenderOffset = IconRenderOffset;
            this.IconScale = IconScale;
            this.IconTransparency = IconTransparency;

            LoadTextures();
        }

#region Multiplayer Support
        public bool IsBagInUse { get { return IsContentsMenuOpen || CraftingHandler.IsUsingForCrafting(this); } }
        //private bool IgnoreNextDeserialize { get; set; }

        public NetString BagInstanceString { get; }
        private void BagInstanceString_fieldChangeEvent(NetString field, string oldValue, string newValue)
        {
            try
            {
                //  No need to resync if this client was the source of the newest data
                //if (IgnoreNextDeserialize)
                //    return;

                //  If the bag is currently in use, avoid syncing as the sync data is probably not the latest
                //  (Another resync will happen once they're done using the bag)
                if (IsBagInUse)
                    return;

                if (!string.IsNullOrEmpty(newValue))
                {
                    TryDeserializeFromString(newValue, out Exception Error);
                }
            }
            finally
            {
                //IgnoreNextDeserialize = false;
            }
        }

        /// <summary>Attempts to re-synchronize this bag's data across all players in multiplayer, by sending the current client's data to all other clients</summary>
        public void Resync()
        {
            if (!Context.IsMultiplayer)
                return;

            if (!IsBagInUse) // No need to resync while the bag is in use, since it will ge resynced the moment they stop using it
            {
                //  Write this bag's data to an xml string, and store that xml string in a NetString field.
                //  When the other clients detect a change to this NetString, they will deserialize the value to load the data.
                if (TrySerializeToString(out string DataString, out Exception Error))
                {
                    //IgnoreNextDeserialize = true;
                    BagInstanceString.Value = DataString;
                }
            }
        }

        public bool TrySerializeToString(out string Data, out Exception Error)
        {
            bool Result = XMLSerializer.TrySerializeToString(new BagInstance(-1, this), out Data, out Error);
            if (!Result)
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Warning - Error while serializing ItemBag to a string: {0}", Error), LogLevel.Warn);
            return Result;
        }

        public bool TryDeserializeFromString(string Data, out Exception Error)
        {
            if (XMLSerializer.TryDeserializeFromString(Data, out BagInstance BI, out Error))
            {
                LoadSettings(BI);
                return true;
            }
            else
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Warning - Error while deserializing ItemBag from string: {0}\n\nData:\n{1}", Error, Data), LogLevel.Warn);
                return false;
            }
        }
#endregion Multiplayer Support

        public bool IsValidBagItem(Item Item)
        {
            return Item != null && !(Item is ItemBag) && IsValidBagObject(Item as Object);
        }

        public virtual bool IsValidBagObject(Object Item)
        {
            return BaseIsValidBagObject(Item);
        }

        protected bool BaseIsValidBagObject(Object Item)
        {
            return
                Item != null &&
                //!Item.specialItem && // Removed this restriction because apparently Gunther donation rewards are marked as specialItem=true
                (Item.questItem == null || !Item.questItem.Value) &&    // Possible TODO: check if void salmons are questItems. 
                                                                        // I ran into an issue where right after I caught one, it didn't get autofilled. 
                                                                        // But when I reloaded the save it was now getting autofilled so something is different after it got deserialized
                (Item.heldObject == null || Item.heldObject.Value == null) &&
                !IsSecretNote(Item) &&
                !Item.isLostItem && (!Item.GetType().IsSubclassOf(typeof(Object)) || Item is ColoredObject);
        }

        private static bool IsSecretNote(Object Item)
        {
            return Item.ParentSheetIndex == 79 && !Item.bigCraftable.Value && !Item.IsRecipe;
        }

#region Open/Close Menu
        [XmlIgnore]
        public ItemBagMenu ContentsMenu { get; private set; }
        [XmlIgnore]
        public bool IsContentsMenuOpen { get { return ContentsMenu != null; } }
        [XmlIgnore]
        public IClickableMenu PreviousMenu { get; private set; }

        /// <param name="Source">The source items that should appear in the bottom half of the bag's interface. Typically this is <see cref="Game1.player.Items"/> if moving to/from the inventory.</param>
        /// <param name="ActualCapacity">The maximum # of items that can be stored in the Source list. Use <see cref="Game1.player.MaxItems"/> if moving to/from the inventory.</param>
        public virtual void OpenContents(IList<Item> Source, int ActualCapacity)
        {
            OpenContents(Source, ActualCapacity, Game1.activeClickableMenu);
        }

        /// <param name="RestoreableMenu">The menu to re-open when this bag is closed. Typically this should be <see cref="Game1.activeClickableMenu"/></param>
        internal void OpenContents(IList<Item> Source, int ActualCapacity, IClickableMenu RestoreableMenu)
        {
            this.PreviousMenu = RestoreableMenu;
            TryRemoveInvalidItems(Game1.player.Items, Game1.player.MaxItems);
            this.ContentsMenu = CreateMenu(Source, ActualCapacity);
            Game1.activeClickableMenu = ContentsMenu;
            Game1.playSound("bigSelect");
        }

        [XmlIgnore]
        protected bool IsClosingContents { get; set; } = false;

        public virtual void CloseContents()
        {
            CloseContents(true, true);
        }

        internal void CloseContents(bool RestorePreviousMenu, bool PlaySoundEffect)
        {
            try
            {
                IsClosingContents = true;
                if (ContentsMenu != null)
                {
                    ContentsMenu.OnClose();
                    if (RestorePreviousMenu)
                    {
                        if (PreviousMenu is ItemGrabMenu IGM && IGM.context is Chest Chest)
                        {
                            //  Fix chest item indices by removing nulls
                            //  (While the bag contents was open, user may have moved items from chest into bag. But unlike a regular inventory,
                            //  chests are supposed to keep all items adjacent to each other with no empty slots in between, so we must remove the null items from list)
                            IList<Item> Items = IGM.ItemsToGrabMenu.actualInventory;
                            for (int i = Items.Count - 1; i >= 0; i--)
                            {
                                if (Items[i] == null)
                                    Items.RemoveAt(i);
                            }
                        }
                        Game1.activeClickableMenu = PreviousMenu;
                    }
                    this.ContentsMenu = null;
                    PreviousMenu = null;
                    if (PlaySoundEffect)
                        Game1.playSound("bigDeSelect");
                }
            }
            finally
            {
                IsClosingContents = false;
                Resync();
            }
        }
#endregion Open/Close Menu

        /// <summary>Removes anyitems that should no longer be storeable in this bag.<para/>
        /// (Could be useful if, for instance, the BagType is modified and certain items can no longer be placed inside the bag. 
        /// This would prevent old items from being stuck in the bag with no way of removing them from the interface)</summary>
        /// <param name="Target">The target items that this bag's contents are being moved to. Typically this is <see cref="Game1.player.Items"/> if moving to/from the inventory.</param>
        /// <param name="ActualTargetCapacity">The maximum # of items that can be stored in the Target list. Use <see cref="Game1.player.MaxItems"/> if moving to/from the inventory.</param>
        public virtual bool TryRemoveInvalidItems(IList<Item> Target, int ActualTargetCapacity)
        {
            bool ChangesMade = false;
            List<Object> InvalidItems = Contents.Where(x => !IsValidBagObject(x)).ToList();
            foreach (Object InvalidItem in InvalidItems)
            {
                MoveFromBag(InvalidItem, InvalidItem.Stack, out int MovedQty, false, Target, ActualTargetCapacity);
                if (MovedQty > 0)
                    ChangesMade = true;
            }
            return ChangesMade;
        }

        /// <param name="InventorySource">Typically this is <see cref="Game1.player.Items"/> if this menu should display the player's inventory.</param>
        /// <param name="ActualCapacity">The maximum # of items that can be stored in the InventorySource list. Use <see cref="Game1.player.MaxItems"/> if moving to/from the inventory.</param>
        protected abstract ItemBagMenu CreateMenu(IList<Item> InventorySource, int ActualCapacity);

        /// <summary>Reverts this Bag's icon back to default values</summary>
        public abstract void ResetIcon();
        public abstract bool IsUsingDefaultIcon();
        public virtual bool CanCustomizeIcon() { return true; }

        public abstract bool IsFull(Object Item);
        public abstract int GetPurchasePrice();

        private static int Min(params int[] values) { return Enumerable.Min(values); }
        private static int Max(params int[] values) { return Enumerable.Max(values); }

        public const string MoveContentsSuccessSound = "smallSelect";
        public const string MoveContentsFailedSound = "thudStep";

        public enum InputTransferAction
        {
            PrimaryActionButtonPressed,
            SecondaryActionButtonPressed,
            SecondaryActionButtonHeld
        }

        /// <summary>Determines how many of the given Objects Stack should be transferred either to or from an ItemBag when the given inputs are used</summary>
        public static int GetQuantityToTransfer(InputTransferAction Input, Object PressedObject, bool TransferMultipleModifierPressed, bool TransferHalfModifierPressed)
        {
            if (PressedObject == null)
            {
                return 0;
            }
            else if (Input == InputTransferAction.PrimaryActionButtonPressed)
            {
                if (TransferMultipleModifierPressed)
                {
                    if (PressedObject.Stack > 999)
                        return Math.Min(25, PressedObject.Stack);
                    else
                        return Math.Min(5, PressedObject.Stack);
                }
                else if (TransferHalfModifierPressed)
                {
                    return Math.Max(1, PressedObject.Stack / 2);
                }
                else
                {
                    return PressedObject.Stack;
                }
            }
            else if (Input == InputTransferAction.SecondaryActionButtonPressed || Input == InputTransferAction.SecondaryActionButtonHeld)
            {
                if (TransferMultipleModifierPressed)
                {
                    if (PressedObject.Stack > 999)
                        return Math.Min(25, PressedObject.Stack);
                    else
                        return Math.Min(5, PressedObject.Stack);
                }
                else if (TransferHalfModifierPressed)
                {
                    return Math.Max(1, PressedObject.Stack / 2);
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>Determines how many of the given Objects Stack should be transferred either to or from an ItemBag when the given inputs are used</summary>
        /// <param name="IsShiftHeld">True if either of the LeftShift or RightShift keys are held.</param>
        /// <param name="IsControlHeld">True if either of the LeftCtrl or RightCtrl keys are held.</param>
        public static int GetQuantityToTransfer(ButtonPressedEventArgs e, Object PressedObject)
        {
            bool IsShiftHeld = e.IsDown(SButton.LeftShift) || e.IsDown(SButton.RightShift);
            bool IsControlHeld = e.IsDown(SButton.LeftControl) || e.IsDown(SButton.RightControl);

            if (e.Button == SButton.MouseLeft)
                return GetQuantityToTransfer(InputTransferAction.PrimaryActionButtonPressed, PressedObject, IsShiftHeld, IsControlHeld);
            else if (e.Button == SButton.MouseRight)
                return GetQuantityToTransfer(InputTransferAction.SecondaryActionButtonPressed, PressedObject, IsShiftHeld, IsControlHeld);
            else
                return 0;
        }

        public bool MoveToBag(List<Object> Items, List<int> Quantities, out int TotalMovedQty, bool PlaySoundEffect, IList<Item> Source)
        {
            if (Items.Count != Quantities.Count)
                throw new ArgumentException("Parameter Mismatch: Every Item must have a Quantity to transfer");

            TotalMovedQty = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                Object Item = Items[i];
                int Qty = Quantities[i];

                MoveToBag(Item, Qty, out int MovedQty, false, Source, false, false);
                TotalMovedQty += MovedQty;
            }

            if (TotalMovedQty > 0)
            {
                OnContentsChanged?.Invoke(this, EventArgs.Empty);
                Resync();
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsSuccessSound);
                return true;
            }
            else
            {
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsFailedSound);
                return false;
            }
        }

        /// <summary>Attempts to move the given <paramref name="Qty"/> of the given <paramref name="Item"/> from the <paramref name="Source"/> into this bag</summary>
        /// <param name="MovedQty">The Qty that was successfully moved.</param>
        /// <returns>True if any changes were made</returns>
        /// <param name="Source">The source items that are being moved to the bag. Typically this is <see cref="Game1.player.Items"/> if moving to/from the inventory.</param>
        public virtual bool MoveToBag(Object Item, int Qty, out int MovedQty, bool PlaySoundEffect, IList<Item> Source, bool NotifyIfContentsChanged = true, bool ResyncMultiplayerData = true)
        {
            MovedQty = 0;
            if (!IsValidBagItem(Item))
                return false;
            else if (Qty <= 0)
            {
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsFailedSound);
                return false;
            }

            Object BagItem = this.Contents.FirstOrDefault(x => AreItemsEquivalent(x, Item, false));

            //  Determine how many more of this Item the bag can hold
            int MaxCapacity = GetMaxStackSize(Item);
            int CurrentCapacity = BagItem == null ? 0 : BagItem.Stack;
            int RemainingCapacity = Math.Max(0, MaxCapacity - CurrentCapacity);
            if (RemainingCapacity == 0)
            {
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsFailedSound);
                return false;
            }

            if (BagItem == null)
            {
                BagItem = CreateCopy(Item);
                this.Contents.Add(BagItem);
            }

            List<Object> InventoryItems = Source.Where(x => x != null && x is Object).Cast<Object>().Where(x => AreItemsEquivalent(x, Item, false))
                .OrderByDescending(x => x == Item).ToList(); // OrderBy will prioritize moving the clicked item's index first
            if (Source == Game1.player.Items && Game1.player.CursorSlotItem != null && Game1.player.CursorSlotItem is Object CursorSlotObject && AreItemsEquivalent(CursorSlotObject, Item, false))
                InventoryItems.Insert(0, CursorSlotObject);

            foreach (Object InventoryItem in InventoryItems)
            {
                int AmountToMove = Math.Max(0, Min(Qty - MovedQty, InventoryItem.Stack, RemainingCapacity));

                int PreviousStack = BagItem.Stack;
                if (!ForceSetQuantity(BagItem, BagItem.Stack + AmountToMove))
                {
                    AmountToMove = BagItem.Stack - PreviousStack;
                    ItemBagsMod.ModInstance.Monitor.Log("Failed to set quantity to a value over 999. An update to Stardew Valley may have broken this functionality and could result in unexpected behavior!", LogLevel.Warn);
                }
                MovedQty += AmountToMove;
                RemainingCapacity -= AmountToMove;
                InventoryItem.Stack -= AmountToMove;

                if (InventoryItem.Stack <= 0)
                {
                    if (Source == Game1.player.Items && InventoryItem == Game1.player.CursorSlotItem)
                        Game1.player.CursorSlotItem = null;
                    else
                        Source[Source.IndexOf(InventoryItem)] = null;
                }

                if (RemainingCapacity == 0 || MovedQty == Qty)
                    break;
            }

            if (MovedQty > 0)
            {
                if (RecentlyModified.TryGetValue(BagItem, out DateTime TimeStamp))
                {
                    RecentlyModified[BagItem] = DateTime.Now;
                }
                else
                {
                    RecentlyModified.Add(BagItem, DateTime.Now);
                    if (RecentlyModified.Count > RecentlyModifiedHistorySize)
                    {
                        List<Object> OldestItems = RecentlyModified.OrderBy(x => x.Value).Select(x => x.Key).Take(RecentlyModified.Count - RecentlyModifiedHistorySize).ToList();
                        OldestItems.ForEach(x => RecentlyModified.Remove(x));
                    }
                }

                if (NotifyIfContentsChanged)
                    OnContentsChanged?.Invoke(this, EventArgs.Empty);
                if (ResyncMultiplayerData)
                    Resync();
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsSuccessSound);
                return true;
            }
            else
            {
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsFailedSound);
                return false;
            }
        }

        public bool MoveFromBag(List<Object> Items, List<int> Quantities, out int TotalMovedQty, bool PlaySoundEffect, IList<Item> Target, int ActualTargetCapacity)
        {
            if (Items.Count != Quantities.Count)
                throw new ArgumentException("Parameter Mismatch: Every Item must have a Quantity to transfer");

            TotalMovedQty = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                Object Item = Items[i];
                int Qty = Quantities[i];

                MoveFromBag(Item, Qty, out int MovedQty, false, Target, ActualTargetCapacity, false, false);
                TotalMovedQty += MovedQty;
            }

            if (TotalMovedQty > 0)
            {
                OnContentsChanged?.Invoke(this, EventArgs.Empty);
                Resync();
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsSuccessSound);
                return true;
            }
            else
            {
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsFailedSound);
                return false;
            }
        }

        /// <summary>Attempts to move the given <paramref name="Qty"/> of the given <paramref name="Item"/> from this Bag into player1's inventory</summary>
        /// <param name="MovedQty">The Qty that was successfully moved.</param>
        /// <returns>True if any changes were made</returns>
        /// <param name="Target">The target items that this bag's contents are being moved to. Typically this is <see cref="Game1.player.Items"/> if moving to/from the inventory.</param>
        /// <param name="ActualTargetCapacity">The maximum # of items that can be stored in the Target list. Use <see cref="Game1.player.MaxItems"/> if moving to/from the inventory.</param>
        public bool MoveFromBag(Object Item, int Qty, out int MovedQty, bool PlaySoundEffect, IList<Item> Target, int ActualTargetCapacity, bool NotifyIfContentsChanged = true, bool ResyncMultiplayerData = true)
        {
            MovedQty = 0;
            if (Qty <= 0)
                return false;

            int TargetCapacity = Math.Max(ActualTargetCapacity, Target.Count);

            Object BagItem = this.Contents.FirstOrDefault(x => AreItemsEquivalent(x, Item, false));
            if (BagItem == null)
                return false;
            int RemainingQty = Math.Min(BagItem.Stack, Qty);

            //  Add to existing stacks of the item that are already in the target inventory
            List<Object> ExistingStacks = Target.Where(x => x != null && x is Object).Cast<Object>().Where(x => AreItemsEquivalent(x, Item, false)).ToList();
            foreach (Object InventoryItem in ExistingStacks)
            {
                int AmountToMove = Math.Min(RemainingQty, InventoryItem.maximumStackSize() - InventoryItem.Stack);

                int PreviousStack = BagItem.Stack;
                if (!ForceSetQuantity(BagItem, BagItem.Stack - AmountToMove))
                {
                    int LostAmount = PreviousStack - AmountToMove - BagItem.Stack;
                    RemainingQty += LostAmount;
                    AmountToMove = Math.Min(RemainingQty, InventoryItem.maximumStackSize() - InventoryItem.Stack);
                    ItemBagsMod.ModInstance.Monitor.Log("Failed to set quantity to a value over 999. An update to Stardew Valley may have broken this functionality and could result in unexpected behavior!", LogLevel.Warn);
                }

                InventoryItem.Stack += AmountToMove;
                MovedQty += AmountToMove;
                RemainingQty -= AmountToMove;

                if (BagItem.Stack <= 0)
                {
                    Contents.Remove(BagItem);
                    break;
                }
                else if (RemainingQty == 0)
                    break;
            }

            //  Create new stacks of the item
            if (RemainingQty > 0)
            {
                List<int> EmptySlotIndices = new List<int>();
                for (int i = 0; i < TargetCapacity; i++)
                {
                    if (i >= Target.Count || Target[i] == null)
                        EmptySlotIndices.Add(i);
                }
                while (EmptySlotIndices.Any() && RemainingQty > 0)
                {
                    int ItemIndex = EmptySlotIndices[0];
                    EmptySlotIndices.RemoveAt(0);

                    Object NewItem = CreateCopy(Item);
                    int AmountToMove = Math.Min(RemainingQty, NewItem.maximumStackSize() - NewItem.Stack);

                    int PreviousStack = BagItem.Stack;
                    if (!ForceSetQuantity(BagItem, BagItem.Stack - AmountToMove))
                    {
                        int LostAmount = PreviousStack - AmountToMove - BagItem.Stack;
                        RemainingQty += LostAmount;
                        AmountToMove = Math.Min(RemainingQty, NewItem.maximumStackSize() - NewItem.Stack);
                        ItemBagsMod.ModInstance.Monitor.Log("Failed to set quantity to a value over 999. An update to Stardew Valley may have broken this functionality and could result in unexpected behavior!", LogLevel.Warn);
                    }

                    NewItem.Stack += AmountToMove;
                    if (ItemIndex >= Target.Count)
                        Target.Add(NewItem);
                    else
                        Target[ItemIndex] = NewItem;

                    MovedQty += AmountToMove;
                    RemainingQty -= AmountToMove;

                    if (BagItem.Stack <= 0)
                    {
                        Contents.Remove(BagItem);
                        break;
                    }
                    else if (RemainingQty == 0)
                        break;
                }
            }

            if (MovedQty > 0)
            {
                //if (RecentlyModified.TryGetValue(BagItem, out DateTime TimeStamp))
                //{
                //    RecentlyModified[BagItem] = DateTime.Now;
                //}
                //else
                //{
                //    RecentlyModified.Add(BagItem, DateTime.Now);
                //    if (RecentlyModified.Count > RecentlyModifiedHistorySize)
                //    {
                //        List<Object> OldestItems = RecentlyModified.OrderBy(x => x.Value).Select(x => x.Key).Take(RecentlyModified.Count - RecentlyModifiedHistorySize).ToList();
                //        OldestItems.ForEach(x => RecentlyModified.Remove(x));
                //    }
                //}

                if (NotifyIfContentsChanged)
                    OnContentsChanged?.Invoke(this, EventArgs.Empty);
                if (ResyncMultiplayerData)
                    Resync();
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsSuccessSound);
                return true;
            }
            else
            {
                if (PlaySoundEffect)
                    Game1.playSound(MoveContentsFailedSound);
                return true;
            }
        }

        /// <summary>Set's the Stack size of the given <paramref name="Item"/> to the desired <paramref name="Qty"/>. This function should even work with quantities exceeding the default maximum of 999</summary>
        /// <returns>True if value was successfully set to the desired <paramref name="Qty"/></returns>
        public static bool ForceSetQuantity(Object Item, int Qty)
        {
            Item.Stack = Qty;
            if (Item.Stack != Qty)
            {
                // StardewValley.Object.Stack seems to have a hardcoded 999 maximum in its Setter function, 
                // but it's backing field, StardewValley.Object.stack (NetInt, lowercase 's' in 'stack') does not as of v1.4.
                Item.stack.Value = Qty;
            }

            return Item.Stack == Qty;
        }

        public override int salePrice()
        {
            if (IsEmpty())
                return GetPurchasePrice();
            else
                return Contents.Where(x => x != null).Sum(x => GetSingleItemPrice(x) * 2 * x.Stack);
        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            if (!Context.IsMultiplayer || who.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
            {
                OpenContents(who.Items, who.MaxItems);
                this.Update(who.FacingDirection, 0, who);
                who.EndUsingTool();
                who.completelyStopAnimatingOrDoingAction();
            }

            return true;

            //return base.beginUsing(location, x, y, who);
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            //base.DoFunction(location, x, y, power, who);
        }

        public override string DisplayName
        {
            get { return string.Format("{0} {1}", ItemBagsMod.Translate(string.Format("Size{0}Name", Size.GetDescription())), BaseName); }
            set { /*base.DisplayName = value;*/ }
        }

        protected override string loadDisplayName()
        {
            return this.DisplayName;
        }

        public override string Name
        {
            get { return this.DisplayName; }
            set { /*base.BaseName = value;*/ }
        }

        public override Item getOne()
        {
            return this; //(Item)new ItemBag();
        }

#region Stack
        public override int Stack
        {
            get { return 1; }
            set { }
        }

        public override int addToStack(Item stack)
        {
            //return base.addToStack(stack);
            return 0;
        }

        public override bool canStackWith(ISalable other)
        {
            //return base.canStackWith(other);
            return false;
        }

        public override int maximumStackSize()
        {
            //return base.maximumStackSize();
            return 1;
        }
#endregion Stack

        public override Color getCategoryColor()
        {
            //return base.getCategoryColor();
            return Color.DarkGreen;
        }

        public override string getCategoryName()
        {
            //return base.getCategoryName();
            return ItemBagsMod.Translate("BagCategoryName");
        }

        protected override string loadDescription()
        {
            return getDescription();
        }

        public override string getDescription()
        {
            //return base.getDescription();
            return this.DescriptionAlias;
        }

#region Draw Overrides
        public override void draw(SpriteBatch b)
        {
            //base.draw(b);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            //base.drawAttachments(b, x, y);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            DrawInMenu(GrayscaleTexture, null, Vector2.Zero, 1f, spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        protected void DrawInMenu(Texture2D primaryIcon, Rectangle? primaryIconSourceRect, Vector2 primaryIconOffset, float primaryIconScale, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            //base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);

            float BaseScale = 4f * scaleSize; // Default scale of 4.0f because the source textures are 16x16 but being drawn to a 64x64 inventory slot

            //Color Color = color * transparency;
            Color Color = GetTextureOverlay(transparency, Color.White, 0.125f);
            spriteBatch.Draw(primaryIcon, location + new Vector2(32f, 32f) + primaryIconOffset, primaryIconSourceRect, Color, 0f, new Vector2(8f, 8f), BaseScale * BaseUIScale[Size] * primaryIconScale, SpriteEffects.None, layerDepth);

            if (Icon != null && !Icon.IsDisposed)
            {
                Vector2 Position = location + new Vector2(32f, 32f);
                if (IconRenderOffset.HasValue)
                {
                    Position += IconRenderOffset.Value * scaleSize;
                }
                spriteBatch.Draw(Icon, Position, this.IconTexturePosition, Color.White * IconTransparency * Math.Max(0.15f, Math.Min(1f, transparency * 1.2f)),
                    0f, new Vector2(8f, 8f), BaseScale * IconScale, SpriteEffects.None, layerDepth);
            }

            //spriteBatch.Draw(Game1.toolSpriteSheet, location + new Vector2(32f, 32f), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, 16, 16, this.IndexOfMenuItemView)), color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
        }

        public override void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, string overrideText)
        {
            //  Item.cs implementation:
            //if (!string.IsNullOrEmpty(overrideText) && overrideText != " ")
            //{
            //    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
            //    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
            //    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
            //    spriteBatch.DrawString(font, overrideText, new Vector2((float)(x + 16), (float)(y + 16 + 4)), (Game1.textColor * 0.9f) * alpha);
            //    y = y + (int)font.MeasureString(overrideText).Y + 4;
            //}

            base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);
        }

        protected void BaseDrawToolTip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, string overrideText)
        {
            base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);
        }
#endregion Draw Overrides

#region Basic Overrides
        public override int attachmentSlots()
        {
            //return base.attachmentSlots();
            return 0;
        }

        public override bool canBeGivenAsGift()
        {
            //return base.canBeGivenAsGift();
            return false;
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            //return base.canBePlacedHere(l, tile);
            return false;
        }

        public override bool canBePlacedInWater()
        {
            //return base.canBePlacedInWater();
            return false;
        }

        public override bool canBeDropped()
        {
            //return base.canBeDropped();

            if (Context.IsMultiplayer)
            {
                return false;
            }
            else
            {
                //return false;
                return base.canBeDropped() && IsEmpty(); // Note that the game doesn't allow dropping an item where canBeTrashed()==false
            }
        }

        public override bool canBeTrashed()
        {
            //return base.canBeTrashed();

            if (Context.IsMultiplayer)
            {
                return IsEmpty();
                //return false;
            }
            else
            {
                return IsEmpty();
            }
        }

        public override bool canThisBeAttached(StardewValley.Object o)
        {
            //return base.canThisBeAttached(o);
            return false;
        }

        public override bool ShouldSerializeparentSheetIndex()
        {
            //return base.ShouldSerializeparentSheetIndex();
            return false;
        }

        public override int staminaRecoveredOnConsumption()
        {
            //return base.staminaRecoveredOnConsumption();
            return 0;
        }

        public override bool doesShowTileLocationMarker()
        {
            //return base.doesShowTileLocationMarker();
            return false;
        }

        public override int healthRecoveredOnConsumption()
        {
            //return base.healthRecoveredOnConsumption();
            return 0;
        }

        public override bool isPlaceable()
        {
            //return base.isPlaceable();
            return false;
        }
#endregion Basic Overrides
    }
}
