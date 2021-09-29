/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Community_Center;
using ItemBags.Helpers;
using ItemBags.Menus;
using ItemBags.Persistence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Object = StardewValley.Object;
#if !ANDROID
using PyTK.CustomElementHandler;
#endif

namespace ItemBags.Bags
{
    /// <summary>A bag used for storing items required by incomplete Community Center Bundles</summary>
    [XmlType("Mods_BundleBag")]
    [XmlRoot(ElementName = "BundleBag", Namespace = "")]
#if ANDROID
    public class BundleBag : BoundedBag
#else
    public class BundleBag : BoundedBag//, ISaveElement
#endif
    {
        public const string BundleBagTypeId = "c3f69b2c-6b21-477c-ad43-ee3b996a96bd";

        public static readonly ReadOnlyCollection<ContainerSize> ValidSizes = new List<ContainerSize>() {
            ContainerSize.Large,
            ContainerSize.Massive
        }.AsReadOnly();

        /// <summary>Value = The names of Community Center rooms that the given size of <see cref="BundleBag"/> is NOT capable of storing.</summary>
        public static readonly Dictionary<ContainerSize, HashSet<string>> InvalidRooms = new Dictionary<ContainerSize, HashSet<string>>()
        {
            { ContainerSize.Large, new HashSet<string>(ItemBagsMod.Translate("LargeBundleBagUnstoreableRooms").Split(',').Select(x => x.Trim())) }, // { "Bulletin Board", "Abandoned Joja Mart" }
            { ContainerSize.Massive, new HashSet<string>() { } }
        };

        [XmlIgnore]
        public override int MaxStackSize { get { return int.MaxValue; } }

        /// <summary>If true, then placing items inside the BundleBag will allow  downgrading the placed item's <see cref="StardewValley.Object.Quality"/> to the highest quality still needed of that item for an incomplete bundle.<para/>
        /// For example, suppose you picked up a Gold-quality Parsnip. Gold parsnips are needed for the Quality crops bundle and Regular-quality are needed for Spring crops bundle.<para/>
        /// If Quality crops is already complete, then the picked-up Parsnip will be downgraded to regular quality, to fulfill the Spring crops bundle instead.</summary>
        [XmlIgnore]
        public bool AllowDowngradeItemQuality { get { return ItemBagsMod.UserConfig.AllowDowngradeBundleItemQuality(this.Size); } }

        /// <summary>Default parameterless constructor intended for use by XML Serialization. Do not use this constructor to instantiate a bag.</summary>
        public BundleBag() : base()
        {
            this.Size = ValidSizes.Min();
            this.Autofill = true;
        }

        /// <param name="Size">Must be a Size within <see cref="ValidSizes"/></param>
        public BundleBag(ContainerSize Size, bool Autofill)
            : base(ItemBagsMod.Translate("BundleBagName"), ItemBagsMod.Translate("BundleBagDescription"), Size, true)
        {
            if (!ValidSizes.Contains(Size))
                throw new InvalidOperationException(string.Format("Size '{0}' is not valid for BundleBag types", Size.ToString()));

            this.Autofill = Autofill;
        }

        public BundleBag(BagInstance SavedData)
            : this(SavedData.Size, SavedData.Autofill)
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
        }

#region PyTK CustomElementHandler
        public override object getReplacement()
        {
            return new Object(172, 1);
        }

        protected override void LoadSettings(BagInstance Data)
        {
            if (Data != null)
            {
                this.Size = Data.Size;
                this.Autofill = Data.Autofill;

                this.BaseName = ItemBagsMod.Translate("BundleBagName");
                this.DescriptionAlias = ItemBagsMod.Translate("BundleBagDescription");

                Contents.Clear();
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
#endregion PyTK CustomElementHandler

        internal override bool OnJsonAssetsItemIdsFixed(IJsonAssetsAPI API, bool AllowResyncing)
        {
            return ValidateContentsIds(API, AllowResyncing);
        }

        public override void ResetIcon()
        {
            this.DefaultIconTexture = TextureHelpers.JunimoNoteTexture;
            this.DefaultIconTexturePosition = new Rectangle(0, 244, 16, 16);
            this.CustomIconSourceTexture = null;
            this.CustomIconTexturePosition = null;
        }

        public override bool CanCustomizeIcon() { return false; }

        public override int GetPurchasePrice() { return ItemBagsMod.UserConfig.GetBundleBagPrice(Size); }
        public override string GetTypeId() { return BundleBagTypeId; }

        /// <param name="InventorySource">Typically this is <see cref="Game1.player.Items"/> if this menu should display the player's inventory.</param>
        /// <param name="ActualCapacity">The maximum # of items that can be stored in the InventorySource list. Use <see cref="Game1.player.MaxItems"/> if moving to/from the inventory.</param>
        protected override ItemBagMenu CreateMenu(IList<Item> InventorySource, int ActualCapacity)
        {
            try
            {
                ItemBagMenu Menu = new ItemBagMenu(this, InventorySource, ActualCapacity, 12, BagInventoryMenu.DefaultInventoryIconSize);
                Menu.Content = new BundleBagMenu(Menu, this, 20, 48, true, 12);
                return Menu;
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Unhandled error while creating BundleBagMenu: {0}", ex.Message), LogLevel.Error);
                return null;
            }
        }

        public override bool IsValidBagObject(Object item)
        {
            if (!BaseIsValidBagObject(item) || item.bigCraftable.Value)
            {
                return false;
            }
            else
            {
                if (CommunityCenterBundles.Instance.IsJojaMember ||
                    !CommunityCenterBundles.Instance.IncompleteBundleItemIds[this.Size].TryGetValue(item.ParentSheetIndex, out HashSet<ObjectQuality> AcceptedQualities))
                {
                    return false;
                }
                else
                {
                    if (!Enum.IsDefined(typeof(ObjectQuality), item.Quality))
                        return false;
                    else
                    {
                        ObjectQuality ItemQuality = (ObjectQuality)item.Quality;
                        if (AcceptedQualities.Contains(ItemQuality))
                            return true;
                        else if (AllowDowngradeItemQuality)
                            return AcceptedQualities.Any(x => x <= ItemQuality);
                        else
                            return false;
                    }
                }
            }
        }

        private Dictionary<ObjectQuality, int> GetRequiredQuantities(Object Item)
        {
            //  Get all incomplete bundle items referring to the given item, and index the required quantity of each quality
            Dictionary<ObjectQuality, int> RequiredAmounts = new Dictionary<ObjectQuality, int>();
            CommunityCenterBundles.Instance.IterateAllBundleItems(x =>
            {
                string RoomName = x.Task.Room.Name;
                bool IsValidRoomForCurrentSize = !InvalidRooms[this.Size].Contains(RoomName);
                if (IsValidRoomForCurrentSize && !x.IsCompleted && x.Id == Item.ParentSheetIndex)
                {
                    if (!RequiredAmounts.ContainsKey(x.MinQuality))
                    {
                        RequiredAmounts.Add(x.MinQuality, x.Quantity);
                    }
                    else
                    {
                        RequiredAmounts[x.MinQuality] += x.Quantity;
                    }
                }
            });
            return RequiredAmounts;
        }

        /// <param name="RequiredQuantities">Optional. Use null to have this automatically computed, or use <see cref="GetRequiredQuantities(Object)"/> and cache the value if making several successive calls to this method for the same Object.</param>
        private int GetMaxStackSize(Object Item, Dictionary<ObjectQuality, int> RequiredQuantities)
        {
            if (!BaseIsValidBagObject(Item) || Item.bigCraftable.Value)
                return 0;

            if (RequiredQuantities == null)
                RequiredQuantities = GetRequiredQuantities(Item);

            ObjectQuality ItemQuality = (ObjectQuality)Item.Quality;

            /*if (AllowDowngradeItemQuality)
            {
                return RequiredAmounts.Where(x => x.Key <= ItemQuality).Sum(x => x.Value);
            }
            else
            {
                if (!RequiredAmounts.ContainsKey(ItemQuality))
                    return 0;
                else
                    return RequiredAmounts[ItemQuality];
            }*/

            if (!RequiredQuantities.ContainsKey(ItemQuality))
                return 0;
            else
                return RequiredQuantities[ItemQuality];
        }

        protected override int GetMaxStackSize(Object Item)
        {
            return GetMaxStackSize(Item, null);
        }

        public override bool MoveToBag(Object Item, int Qty, out int MovedQty, bool PlaySoundEffect, IList<Item> Source, bool NotifyIfContentsChanged = true, bool ResyncMultiplayerData = true)
        {
            ObjectQuality OriginalQuality = (ObjectQuality)Item.Quality;
            bool CanDowngradeQuality = OriginalQuality > ObjectQuality.Regular && this.AllowDowngradeItemQuality && Source != null && Item != null && Source.Contains(Item);
            if (!CanDowngradeQuality)
                return base.MoveToBag(Item, Qty, out MovedQty, PlaySoundEffect, Source, NotifyIfContentsChanged, ResyncMultiplayerData);
            else
            {
                //Dictionary<ObjectQuality, int> RequiredQuantities = GetRequiredQuantities(Item);

                //  Keep trying to transfer the next worse quality until all have been transferred, or we reach the minimum quality
                int RemainingQty = Qty;
                int TotalMovedQty = 0;
                List<ObjectQuality> ValidQualities = Enum.GetValues(typeof(ObjectQuality)).Cast<ObjectQuality>().Where(x => x <= OriginalQuality).OrderByDescending(x => x).ToList();
                foreach (ObjectQuality CurrentQuality in ValidQualities)
                {
                    Item.Quality = (int)CurrentQuality;
                    if (base.MoveToBag(Item, RemainingQty, out int CurrentMovedQty, false, Source, false, false))
                    {
                        TotalMovedQty += CurrentMovedQty;
                        RemainingQty -= CurrentMovedQty;
                    }
                    if (TotalMovedQty >= Qty)
                        break;
                }

                //  Put any leftover stack back to the original quality
                Item.Quality = (int)OriginalQuality;

                MovedQty = TotalMovedQty;
                if (MovedQty > 0)
                {
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
        }

        public override void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
        {
            BaseDrawToolTip(spriteBatch, ref x, ref y, font, alpha, overrideText);
        }
    }
}
