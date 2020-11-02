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
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static ItemBags.Persistence.BagSizeConfig;
using Object = StardewValley.Object;

namespace ItemBags.Persistence
{
    /// <summary>Represents a <see cref="BoundedBag"/> that can store custom items belonging to other mods.</summary>
    [JsonObject(Title = "ModdedBag")]
    [DataContract(Name = "ModdedBag", Namespace = "")]
    public class ModdedBag
    {
        public static readonly ReadOnlyCollection<ContainerSize> AllSizes = Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>().ToList().AsReadOnly();

        /// <summary>If false, this data file will not be loaded on startup</summary>
        [JsonProperty("IsEnabled")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>The UniqueID property of the mod manifest that this modded bag holds items for</summary>
        [JsonProperty("ModUniqueId")]
        public string ModUniqueId { get; set; } = "";

        /// <summary>A unique identifier for this modded bag. Typically this Guid is computed using <see cref="StringToGUID(string)"/> with parameter = "<see cref="ModUniqueId"/>+<see cref="BagName"/>"</summary>
        [JsonProperty("BagId")]
        public string Guid { get; set; } = "";

        [JsonProperty("BagName")]
        public string BagName { get; set; } = "Unnamed";
        [JsonProperty("BagDescription")]
        public string BagDescription { get; set; } = "";

        [JsonProperty("IconTexture")]
        public BagType.SourceTexture IconTexture { get; set; } = BagType.SourceTexture.SpringObjects;
        [JsonProperty("IconPosition")]
        public Rectangle IconPosition { get; set; } = new Rectangle();

        [JsonProperty("Prices")]
        public Dictionary<ContainerSize, int> Prices { get; set; } = AllSizes.ToDictionary(x => x, x => BagTypeFactory.DefaultPrices[x]);
        [JsonProperty("Capacities")]
        public Dictionary<ContainerSize, int> Capacities { get; set; } = AllSizes.ToDictionary(x => x, x => BagTypeFactory.DefaultCapacities[x]);
        [JsonProperty("SizeSellers")]
        public Dictionary<ContainerSize, List<BagShop>> Sellers { get; set; } = AllSizes.ToDictionary(x => x, x => new List<BagShop>() { BagShop.Pierre });

        private static readonly BagMenuOptions DefaultMenuOptions = new BagMenuOptions() {
            GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() {
                GroupsPerRow = 5
            }
        };
        [JsonProperty("SizeMenuOptions")]
        public Dictionary<ContainerSize, BagMenuOptions> MenuOptions { get; set; } = AllSizes.ToDictionary(x => x, x => DefaultMenuOptions.GetCopy());

        /// <summary>Metadata about each modded item that should be storeable inside this bag.</summary>
        [JsonProperty("Items")]
        public List<ModdedItem> Items { get; set; } = new List<ModdedItem>();

        //Taken from: https://weblogs.asp.net/haithamkhedre/generate-guid-from-any-string-using-c
        public static Guid StringToGUID(string value)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            return new Guid(data);
        }

        //  Categories can be found here: https://stardewvalleywiki.com/Modding:Object_data#Categories
        /// <summary>Object instances belonging to these categories can have different values in <see cref="Object.Quality"/>. This list might not be accurate.</summary>
        private static readonly ReadOnlyCollection<int> CategoriesWithQualities = new List<int>() {
            Object.FishCategory, Object.EggCategory, Object.MilkCategory, Object.meatCategory,
            Object.sellAtPierresAndMarnies, Object.artisanGoodsCategory, /*Object.syrupCategory,*/
            Object.VegetableCategory, Object.FruitsCategory, Object.flowersCategory, Object.GreensCategory
        }.AsReadOnly();

        /// <summary>Must be executed after <see cref="IJsonAssetsAPI.IdsFixed"/> event has fired.<para/>
        /// Returns all BigCraftable and all Objects that belong to the given mod manifest UniqueID. 
        /// Does not include other types of items such as Hats or Weapons.</summary>
        internal static List<ModdedItem> GetModdedItems(string ModUniqueId, ContainerSize RequiredSize = ContainerSize.Small)
        {
            List<ModdedItem> Items = new List<ModdedItem>();

            IModHelper Helper = ItemBagsMod.ModInstance.Helper;
            if (Helper.ModRegistry.IsLoaded(ItemBagsMod.JAUniqueId))
            {
                IJsonAssetsAPI API = Helper.ModRegistry.GetApi<IJsonAssetsAPI>(ItemBagsMod.JAUniqueId);
                if (API != null)
                {
                    List<string> Objects = API.GetAllObjectsFromContentPack(ModUniqueId);
                    if (Objects != null)
                    {
                        //  Index all regular Objects by their names
                        Dictionary<string, int> AllObjectIds = new Dictionary<string, int>();
                        foreach (System.Collections.Generic.KeyValuePair<int, string> KVP in Game1.objectInformation)
                        {
                            string ObjectName = KVP.Value.Split('/').First();
                            if (!AllObjectIds.ContainsKey(ObjectName))
                                AllObjectIds.Add(ObjectName, KVP.Key);
                        }

                        foreach (string ModdedItemName in Objects)
                        {
                            //  Try to guess if the item has multiple different valid qualities, based on the it's category
                            bool HasQualities = true;
                            if (AllObjectIds.TryGetValue(ModdedItemName, out int ItemId))
                            {
                                Object SampleItem = new Object(ItemId, 1, false, -1, 0);
                                if (SampleItem != null)
                                    HasQualities = CategoriesWithQualities.Contains(SampleItem.Category);
                            }

                            Items.Add(new ModdedItem(ModdedItemName, false, HasQualities, RequiredSize));
                        }
                    }
                    //List<string> Crops = API.GetAllCropsFromContentPack(ModUniqueId);
                    //if (Crops != null)
                    //    Items.AddRange(Crops.Select(x => new ModdedItem(x, false, false, RequiredSize)));
                    List<string> BigCraftables = API.GetAllBigCraftablesFromContentPack(ModUniqueId);
                    if (BigCraftables != null)
                        Items.AddRange(BigCraftables.Select(x => new ModdedItem(x, true, false, RequiredSize)));
                }
            }

            //Possible TODO
            //If ContentPatcher mod is loaded, try to parse the mod's content.json file and read the modded item's added by "Action"="EditData" and "Target"="Data/ObjectInformation"
            //For example, the content.json file might have data like this (this is a brief sample from the 'New Fish' mod https://www.nexusmods.com/stardewvalley/mods/3578):
            //{
            //      "Action": "EditData",
            //      "Target": "Data/ObjectInformation",
            //      "Entries": {
            //          "1120": "Ladyfish/80/8/Fish -4/Ladyfish/This saltwater fish prefers temperate waters and feeds in large schools.",
            //          "1121": "Tancho Koi/150/5/Fish -4/Tancho Koi/A white carp with a red spot on its head. People appreciate their beauty and usually keep them in aquariums. It would be such a waste to eat or turn into fertilizer./Day Night^Fall",
            //          ...
            //          ...
            //          ...
            //      }
            //}

            return Items;
        }

        internal static void OnGameLaunched()
        {
            IModHelper Helper = ItemBagsMod.ModInstance.Helper;

            //  Load modded items from JsonAssets the moment it finishes registering items
            if (Helper.ModRegistry.IsLoaded(ItemBagsMod.JAUniqueId))
            {
                IJsonAssetsAPI API = Helper.ModRegistry.GetApi<IJsonAssetsAPI>(ItemBagsMod.JAUniqueId);
                if (API != null)
                {
                    API.IdsFixed += (sender, e) => { OnJsonAssetsIdsFixed(API, ItemBagsMod.BagConfig); };
                }
            }
        }

        private static void OnJsonAssetsIdsFixed(IJsonAssetsAPI API, BagConfig Target)
        {
            try
            {
                ItemBagsMod.ModdedItems.ImportModdedItems(API, ItemBagsMod.BagConfig);

                if (ItemBagsMod.TemporaryModdedBagTypes.Any())
                {
                    Dictionary<string, int> AllBigCraftableIds = new Dictionary<string, int>();
                    foreach (System.Collections.Generic.KeyValuePair<int, string> KVP in Game1.bigCraftablesInformation)
                    {
                        string ObjectName = KVP.Value.Split('/').First();
                        if (!AllBigCraftableIds.ContainsKey(ObjectName))
                            AllBigCraftableIds.Add(ObjectName, KVP.Key);
                    }

                    Dictionary<string, int> AllObjectIds = new Dictionary<string, int>();
                    foreach (System.Collections.Generic.KeyValuePair<int, string> KVP in Game1.objectInformation)
                    {
                        string ObjectName = KVP.Value.Split('/').First();
                        if (!AllObjectIds.ContainsKey(ObjectName))
                            AllObjectIds.Add(ObjectName, KVP.Key);
                    }

                    IDictionary<string, int> JABigCraftableIds = API.GetAllBigCraftableIds();
                    IDictionary<string, int> JAObjectIds = API.GetAllObjectIds();

                    //  Now that JsonAssets has finished loading the modded items, go through each one, and convert the items into StoreableBagItems (which requires an Id instead of just a Name)
                    foreach (System.Collections.Generic.KeyValuePair<ModdedBag, BagType> KVP in ItemBagsMod.TemporaryModdedBagTypes)
                    {
                        foreach (BagSizeConfig SizeCfg in KVP.Value.SizeSettings)
                        {
                            HashSet<string> FailedItemNames = new HashSet<string>();
                            List<StoreableBagItem> Items = new List<StoreableBagItem>();

                            foreach (ModdedItem DesiredItem in KVP.Key.Items.Where(x => x.Size <= SizeCfg.Size))
                            {
                                StoreableBagItem Item = DesiredItem.ToStoreableBagItem(JABigCraftableIds, JAObjectIds, AllBigCraftableIds, AllObjectIds);
                                if (Item == null)
                                    FailedItemNames.Add(DesiredItem.Name);
                                else
                                    Items.Add(Item);
                            }

                            SizeCfg.Items = Items;

                            if (FailedItemNames.Any())
                            {
                                int MaxNamesShown = 5;
                                string MissingItems = string.Format("{0}{1}", string.Join(", ", FailedItemNames.Take(MaxNamesShown)),
                                    FailedItemNames.Count <= MaxNamesShown ? "" : string.Format(" + {0} more", (FailedItemNames.Count - MaxNamesShown)));
                                string WarningMsg = string.Format("Warning - {0} items could not be found for modded bag '{1} {2}'. Missing Items: {3}",
                                    FailedItemNames.Count, SizeCfg.Size.ToString(), KVP.Key.BagName, MissingItems);
                                ItemBagsMod.ModInstance.Monitor.Log(WarningMsg, LogLevel.Warn);
                            }
                        }
                    }

                    ItemBag.GetAllBags(true).ForEach(x => x.OnJsonAssetsItemIdsFixed(API, true));
                }
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Failed to import modded bags. Error: {0}", ex.Message), LogLevel.Error);
            }
        }

        internal BagType GetBagTypePlaceholder()
        {
            return new BagType()
            {
                  Id = Guid,
                  Name = BagName,
                  Description = BagDescription,
                  IconSourceTexture = IconTexture,
                  IconSourceRect = IconPosition,
                  SizeSettings = AllSizes.Select(x => new BagSizeConfig()
                  {
                      Size = x,
                      MenuOptions = MenuOptions[x],
                      Price = Prices[x],
                      Sellers = Sellers[x],
                      CapacityMultiplier = BagTypeFactory.GetCapacityMultiplier(x, Capacities[x]),
                      Items = new List<StoreableBagItem>()
                  }).ToArray()
            };
        }

        #region Backwards Compatibility
        /// <summary>Deprecated. Use <see cref="Prices"/> instead.</summary>
        [JsonProperty("Price")]
        private int DeprecatedPrice { set { Prices = AllSizes.ToDictionary(x => x, x => value); } }
        /// <summary>Deprecated. Use <see cref="Capacities"/> instead. The maximum quantity of each item that this bag is capable of storing.</summary>
        [JsonProperty("Capacity")]
        private int DeprecatedCapacity { set { Capacities = AllSizes.ToDictionary(x => x, x => value); } }
        /// <summary>Deprecated. Use <see cref="Sellers"/> instead. The shops that will sell this bag</summary>
        [JsonProperty("Sellers")]
        private List<BagShop> DeprecatedSellers { set { Sellers = AllSizes.ToDictionary(x => x, x => new List<BagShop>(value)); } }
        /// <summary>Deprecated. Use <see cref="MenuOptions"/> instead.</summary>
        [JsonProperty("MenuOptions")]
        private BagMenuOptions DeprecatedMenuOptions { set { MenuOptions = AllSizes.ToDictionary(x => x, x => value.GetCopy()); } }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc)
        {
            if (string.IsNullOrEmpty(Guid))
                Guid = GetLegacyGuid(ModUniqueId);
        }

        public static string GetLegacyGuid(string ModUniqueId) { return StringToGUID(ModUniqueId).ToString(); }
        #endregion Backwards Compatibility
    }

    /// <summary>Represents modded items that should be merged into non-modded bags, such as storing a modded seed item in the built-in "Seed Bag"</summary>
    [JsonObject(Title = "ModdedItems")]
    [DataContract(Name = "ModdedItems", Namespace = "")]
    public class ModdedItems
    {
        /// <summary>This property is only public for serialization purposes. Use <see cref="CreatedByVersion"/> instead.</summary>
        [JsonProperty("CreatedByVersion")]
        public string CreatedByVersionString { get; set; }
        /// <summary>Warning - in old versions of the mod, this value may be null. This feature was added with v1.3.4</summary>
        [JsonIgnore]
        public Version CreatedByVersion {
            get { return string.IsNullOrEmpty(CreatedByVersionString) ? null : Version.Parse(CreatedByVersionString); }
            set { CreatedByVersionString = value?.ToString(); }
        }

        [JsonProperty("Mods")]
        public List<ModAddon> ModAddons { get; set; } = new List<ModAddon>();

        private bool HasImportedItems { get; set; } = false;
        internal void ImportModdedItems(IJsonAssetsAPI API, BagConfig Target)
        {
            try
            {
                if (HasImportedItems)
                    return;

                IModHelper Helper = ItemBagsMod.ModInstance.Helper;

                //  Index all BagTypes by their names
                Dictionary<string, BagType> IndexedTypes = new Dictionary<string, BagType>();
                foreach (BagType Type in Target.BagTypes)
                {
                    if (!IndexedTypes.ContainsKey(Type.Name))
                    {
                        IndexedTypes.Add(Type.Name, Type);
                    }
                    else
                    {
                        ItemBagsMod.ModInstance.Monitor.Log(string.Format("Warning - multiple BagTypes were found with the name: '{0}'\nDid you manually edit your bagconfig.json file or do you have multiple Modded Bags with the same name?", Type.Name), LogLevel.Warn);
                    }
                }

                Dictionary<string, int> AllBigCraftableIds = new Dictionary<string, int>();
                foreach (System.Collections.Generic.KeyValuePair<int, string> KVP in Game1.bigCraftablesInformation)
                {
                    string ObjectName = KVP.Value.Split('/').First();
                    if (!AllBigCraftableIds.ContainsKey(ObjectName))
                        AllBigCraftableIds.Add(ObjectName, KVP.Key);
                }

                Dictionary<string, int> AllObjectIds = new Dictionary<string, int>();
                foreach (System.Collections.Generic.KeyValuePair<int, string> KVP in Game1.objectInformation)
                {
                    string ObjectName = KVP.Value.Split('/').First();
                    if (!AllObjectIds.ContainsKey(ObjectName))
                        AllObjectIds.Add(ObjectName, KVP.Key);
                }

                IDictionary<string, int> JABigCraftableIds = API.GetAllBigCraftableIds();
                IDictionary<string, int> JAObjectIds = API.GetAllObjectIds();

                //  Import items from each ModAddon
                foreach (ModAddon ModAddon in ModAddons)
                {
                    if (Helper.ModRegistry.IsLoaded(ModAddon.UniqueId))
                    {
                        foreach (BagAddon BagAddon in ModAddon.BagAddons)
                        {
                            if (!IndexedTypes.TryGetValue(BagAddon.Name, out BagType TargetType))
                            {
                                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Warning - no BagType found with Name = '{0}'. Modded items for this bag will not be imported.", BagAddon.Name), LogLevel.Warn);
                            }
                            else
                            {
                                foreach (ModdedItem Item in BagAddon.Items)
                                {
                                    int Id = -1;
                                    if (Item.ObjectId.HasValue)
                                        Id = Item.ObjectId.Value;
                                    else
                                    {
                                        if ((Item.IsBigCraftable && !JABigCraftableIds.TryGetValue(Item.Name, out Id) && !AllBigCraftableIds.TryGetValue(Item.Name, out Id)) ||
                                            (!Item.IsBigCraftable && !JAObjectIds.TryGetValue(Item.Name, out Id) && !AllObjectIds.TryGetValue(Item.Name, out Id)))
                                        {
                                            string Message = string.Format("Warning - no item with Name = '{0}' was found. This item will not be imported to Bag '{1}'.", Item.Name, BagAddon.Name);
                                            ItemBagsMod.ModInstance.Monitor.Log(Message, LogLevel.Warn);
                                            continue;
                                        }
                                    }

                                    foreach (BagSizeConfig SizeConfig in TargetType.SizeSettings.Where(x => x.Size >= Item.Size))
                                    {
                                        SizeConfig.Items.Add(new StoreableBagItem(Id, Item.HasQualities, null, Item.IsBigCraftable));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Failed to import modded items. Error: {0}", ex.Message), LogLevel.Error);
            }
            finally { HasImportedItems = true; }
        }
    }

    [JsonObject(Title = "ModAddon")]
    [DataContract(Name = "ModAddon", Namespace = "")]
    public class ModAddon
    {
        [JsonProperty("ModUniqueId")]
        public string UniqueId { get; set; } = "";
        [JsonProperty("Bags")]
        public List<BagAddon> BagAddons { get; set; } = new List<BagAddon>();
    }

    [JsonObject(Title = "BagAddon")]
    [DataContract(Name = "BagAddon", Namespace = "")]
    public class BagAddon
    {
        /// <summary>The un-translated name of the standard bag type that is being modified, without a Size prefix. Case sensitive. EX: "Crop Bag".</summary>
        [JsonProperty("Name")]
        public string Name { get; set; } = "";
        /// <summary>Metadata about each modded item that should be storeable inside this bag.</summary>
        [JsonProperty("Items")]
        public List<ModdedItem> Items { get; set; } = new List<ModdedItem>();
    }

    [JsonObject(Title = "Item")]
    [DataContract(Name = "Item", Namespace = "")]
    public class ModdedItem
    {
        /// <summary>The un-translated name of the modded Object.</summary>
        [JsonProperty("Name")]
        public string Name { get; set; } = "";
        /// <summary>True if this Object is a placeable Object such as a Furnace.</summary>
        [JsonProperty("IsBigCraftable")]
        public bool IsBigCraftable { get; set; } = false;
        /// <summary>True if this Object is available in multiple different Qualities (Regular/Silver/Gold/Iridium)</summary>
        [JsonProperty("HasQualities")]
        public bool HasQualities { get; set; } = false;
        /// <summary>The minimum size of the bag that is required to store this Object.</summary>
        [JsonProperty("RequiredSize")]
        public string SizeString { get; set; } = ContainerSize.Small.ToString();
        /// <summary>Optional. You only need to specify either <see cref="Name"/> or <see cref="ObjectId"/>, not both.<para/>
        /// Do not use <see cref="ObjectId"/> if the item does not have a static item id (such as a JsonAssets modded item).</summary>
        [JsonProperty("ObjectId")]
        public int? ObjectId { get; set; } = null;

        public ModdedItem()
        {
            this.Name = "";
            this.IsBigCraftable = false;
            this.HasQualities = false;
            this.SizeString = ContainerSize.Small.ToString();
            this.ObjectId = null;
        }

        public ModdedItem(string Name, bool IsBigCraftable, bool HasQualities, ContainerSize Size)
        {
            this.Name = Name;
            this.IsBigCraftable = IsBigCraftable;
            this.HasQualities = HasQualities;
            this.SizeString = Size.ToString();
            this.ObjectId = null;
        }

        [JsonIgnore]
        public ContainerSize Size { get { return string.IsNullOrEmpty(SizeString) ? ContainerSize.Small : (ContainerSize)Enum.Parse(typeof(ContainerSize), SizeString); } }

        /// <param name="JABigCraftableIds">Ids of BigCraftable items added through JsonAssets. See also: <see cref="IJsonAssetsAPI.GetAllBigCraftableIds"/></param>
        /// <param name="JAObjectIds">Ids of Objects added through JsonAssets. See also: <see cref="IJsonAssetsAPI.GetAllObjectIds"/></param>
        public StoreableBagItem ToStoreableBagItem(IDictionary<string, int> JABigCraftableIds, IDictionary<string, int> JAObjectIds, IDictionary<string, int> AllBigCraftableIds, IDictionary<string, int> AllObjectIds)
        {
            if (ObjectId.HasValue)
            {
                return new StoreableBagItem(ObjectId.Value, HasQualities, null, IsBigCraftable);
            }
            else if (IsBigCraftable)
            {
                if (JABigCraftableIds.TryGetValue(Name, out int JAId))
                    return new StoreableBagItem(JAId, HasQualities, null, IsBigCraftable);
                else if (AllBigCraftableIds.TryGetValue(Name, out int Id))
                    return new StoreableBagItem(Id, HasQualities, null, IsBigCraftable);
                else
                    return null;
            }
            else
            {
                if (JAObjectIds.TryGetValue(Name, out int JAId))
                    return new StoreableBagItem(JAId, HasQualities, null, IsBigCraftable);
                else if (AllObjectIds.TryGetValue(Name, out int Id))
                    return new StoreableBagItem(Id, HasQualities, null, IsBigCraftable);
                else
                    return null;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext sc)
        {
            //  Attempt to fix issues with the SizeString value
            if (string.IsNullOrEmpty(SizeString))
                SizeString = ContainerSize.Small.ToString();
            else
            {
                //  Fix character casing
                if (char.IsLower(SizeString[0]) || SizeString.Skip(1).Any(x => char.IsUpper(x)))
                {
                    SizeString = char.ToUpper(SizeString[0]) + string.Join("", SizeString.Skip(1).Select(x => char.ToLower(x)));
                }

                if (!Enum.TryParse(SizeString, out ContainerSize Result))
                {
                    SizeString = ContainerSize.Small.ToString();
                }
            }
        }
    }
}
