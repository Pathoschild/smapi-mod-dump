using Netcode;
using PyTK.Extensions;
using PyTK.Types;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using System.IO;
using SObject = StardewValley.Object;

namespace CustomCropsDecay
{
    class ModEntry : Mod
    {
        internal static ModEntry _instance { get; set; }
        internal static IMonitor _monitor
        {
            get
            {
                return _instance.Monitor;
            }
        }
        internal static IReflectionHelper _reflection
        {
            get
            {
                return _instance.Helper.Reflection;
            }
        }

        public ModConfig ModConfig { get; private set; }

        private Api api;
        public static Dictionary<int, CustomCropsDecayData> cropsById = new Dictionary<int, CustomCropsDecayData>();
        public static Dictionary<string, CustomCropsDecayData> cropsByName = new Dictionary<string, CustomCropsDecayData>();
        public static Dictionary<int, CustomCropsDecayData> cropsByCategory = new Dictionary<int, CustomCropsDecayData>();
        public override void Entry(IModHelper helper)
        {
            ModConfig = helper.ReadConfig<ModConfig>();
            if (ModConfig.FridgeEffect >= 1)
            {
                ModConfig.FridgeEffect = 1;
            }
            helper.WriteConfig(ModConfig);
            _instance = this;
            loadPacks();
            addInventoryListener();
            helper.Events.World.ChestInventoryChanged += World_ChestInventoryChanged;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            FarmHouse farmHouse = Game1.getLocationFromName("farmHouse") as FarmHouse;
            foreach(Item item in farmHouse.fridge.Value.items)
            {
                if(item is ICropWithDecay crop)
                {
                    crop.decayDays += ModConfig.FridgeEffect;
                }
            }
        }

        private void World_ChestInventoryChanged(object sender, StardewModdingAPI.Events.ChestInventoryChangedEventArgs e)
        {
            foreach(Item item in e.Added)
            {
                if(!(item is ICropWithDecay))
                {
                    chestItemChanged(e.Chest.items, (SObject)item);
                }
            }
            foreach (ItemStackSizeChange changed in e.QuantityChanged)
            {
                Item item = changed.Item;
                if (!(item is ICropWithDecay))
                {
                    chestItemChanged(e.Chest.items, (SObject)item);
                }
            }
        }

        public override object GetApi()
        {
            return api ?? (api = new Api());
        }
        private void addInventoryListener()
        {
            new ItemSelector<SObject>(o => !(o is CropWithDecay) && !(o is ColoredCropWithDecay)).whenAddedToInventory((list) =>
            {
                list.ForEach(item =>
                {
                    chestItemChanged(Game1.player.items, item);
                });
            });
        }

        private static void chestItemChanged(NetObjectList<Item> items, SObject item)
        {
            if (cropsById.TryGetValue(item.ParentSheetIndex, out CustomCropsDecayData data))
            {
                transformObject(items, item, data);
            }
            else if (cropsByName.TryGetValue(item.name, out data))
            {
                transformObject(items, item, data);
            }
            else if (cropsByCategory.TryGetValue(item.Category, out data))
            {
                transformObject(items, item, data);
            }
        }

        private static void transformObject(NetObjectList<Item> items, SObject item, CustomCropsDecayData data)
        {
            if (data.decayDays.TryGetValue(item.Quality, out int decayDays))
            {
                if (item is ColoredObject coloredObject)
                {
                    ColoredCropWithDecay crop = ColoredCropWithDecay.copyFrom(coloredObject);
                    crop.decayDays = decayDays;
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] == item)
                        {
                            items[i] = crop;
                        }
                    }
                }
                else
                {
                    CropWithDecay crop = CropWithDecay.copyFrom(item);
                    crop.decayDays = decayDays;
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] == item)
                        {
                            items[i] = crop;
                        }
                    }
                }
            }
        }

        private List<CustomCropsDecayPack> loadContentPacks()
        {
            List<CustomCropsDecayPack> packs = new List<CustomCropsDecayPack>();

            foreach (StardewModdingAPI.IContentPack pack in Helper.ContentPacks.GetOwned())
            {
                List<CustomCropsDecayPack> cpPacks = loadCP(pack);
                packs.AddRange(cpPacks);
            }
            return packs;
        }


        public List<CustomCropsDecayPack> loadCP(StardewModdingAPI.IContentPack contentPack, SearchOption option = SearchOption.TopDirectoryOnly, string filesearch = "*.json")
        {
            List<CustomCropsDecayPack> packs = new List<CustomCropsDecayPack>();
            string[] files = Directory.GetFiles(contentPack.DirectoryPath, filesearch, option);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                DirectoryInfo directoryInfo = fileInfo.Directory;
                string filename = fileInfo.Name;
                if (filename == "manifest.json")
                    continue;

                CustomCropsDecayPack pack = contentPack.ReadJsonFile<CustomCropsDecayPack>(filename);

                pack.fileName = filename;
                pack.folderName = directoryInfo.Name;
                pack.author = contentPack.Manifest.Author;
                pack.version = contentPack.Manifest.Version.ToString();
                pack.baseFolder = "ContentPack";
                pack.contentPack = contentPack;
                packs.Add(pack);
            }

            return packs;
        }
        private void loadPacks()
        {
            List<CustomCropsDecayPack> packs = loadContentPacks();
            int ruleCount = 0;
            foreach (CustomCropsDecayPack pack in packs)
            {
                foreach (CustomCropsDecayData data in pack.crops)
                {
                    foreach (int id in data.id)
                    {
                        cropsById.AddOrReplace(id, data);
                        ruleCount++;
                    }
                    foreach (string name in data.name)
                    {
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            cropsByName.AddOrReplace(name, data);
                            ruleCount++;
                        }
                    }
                    foreach (Category_ category in data.category)
                    {
                        cropsByCategory.AddOrReplace((int)category, data);
                        ruleCount++;
                    }
                }
            }
            Monitor.Log(packs.Count + " Content Packs with " + ruleCount + " rules found.", LogLevel.Trace);
        }
    }

}
