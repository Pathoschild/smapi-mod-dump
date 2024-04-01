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
using ItemBags.Persistence;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ItemBags.Persistence.BagSizeConfig;
using SObject = StardewValley.Object;

namespace ItemBags
{
    public static class CommandHandler
    {
        private static IModHelper Helper { get; set; }
        private static IMonitor Monitor { get { return ItemBagsMod.ModInstance.Monitor; } }
        private static BagConfig BagConfig { get { return ItemBagsMod.BagConfig; } }

        /// <summary>Adds several SMAPI Console commands for adding bags to the player's inventory</summary>
        internal static void OnModEntry(IModHelper Helper)
        {
            CommandHandler.Helper = Helper;

            RegisterAddItemBagCommand();
            RegisterAddBundleBagCommand();
            RegisterAddRucksackCommand();
            RegisterAddOmniBagCommand();
            RegisterGenerateModdedBagCommand();
            RegisterReloadConfigCommand();
        }

        private static void RegisterAddItemBagCommand()
        {
            List<string> ValidSizes = Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>().Select(x => x.ToString()).ToList();
            List<string> ValidTypes = BagConfig.BagTypes.Select(x => x.Name).ToList();

            //Possible TODO: Add translation support for this command
            string CommandName = Constants.TargetPlatform == GamePlatform.Android ? "addbag" : "player_additembag";
            string CommandHelp = string.Format("Adds an empty Bag of the desired size and type to your inventory.\n"
                + "Arguments: <BagSize> <BagType>\n"
                + "Example: {0} Massive River Fish Bag\n\n"
                + "Valid values for <BagSize>: {1}\n\n"
                + "Valid values for <BagType>: {2}",
                CommandName, string.Join(", ", ValidSizes), string.Join(", ", ValidTypes));
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                if (Game1.player.isInventoryFull())
                {
                    Monitor.Log("Unable to execute command: Inventory is full!", LogLevel.Alert);
                }
                else if (Args.Length < 1)
                {
                    Monitor.Log("Unable to execute command: Required arguments missing!", LogLevel.Alert);
                }
                else
                {
                    //  If user didn't specify a size, give them 1 of every available size for thes BagType
                    bool AllSizes = Args.Length < 2;
                    if (AllSizes)
                    {
                        string TypeName = string.Join(" ", Args[0]);
                        BagType BagType = BagConfig.BagTypes.FirstOrDefault(x => x.Name.Equals(TypeName, StringComparison.CurrentCultureIgnoreCase));
                        if (BagType == null)
                        {
                            Monitor.Log(string.Format("Unable to execute command: <BagType> \"{0}\" is not valid. Expected valid values: {1}", TypeName, string.Join(", ", ValidTypes)), LogLevel.Alert);
                        }
                        else
                        {
                            foreach (ContainerSize Size in Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>())
                            {
                                if (BagType.SizeSettings.Any(x => x.Size == Size))
                                {
                                    try
                                    {
                                        if (!Game1.player.isInventoryFull())
                                        {
                                            BoundedBag NewBag = new BoundedBag(BagType, Size, false);
                                            Game1.player.addItemToInventory(NewBag);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Monitor.Log(string.Format("ItemBags: Unhandled error while executing command: {0}", ex.Message), LogLevel.Error);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string SizeName = Args[0];
                        if (!Enum.TryParse(SizeName, out ContainerSize Size))
                        {
                            Monitor.Log(string.Format("Unable to execute command: <BagSize> \"{0}\" is not valid. Expected valid values: {1}", SizeName, string.Join(", ", ValidSizes)), LogLevel.Alert);
                        }
                        else
                        {
                            string TypeName = string.Join(" ", Args.Skip(1));
                            //Possible TODO: If you add translation support to this command, then find the BagType where BagType.GetTranslatedName().Equals(TypeName, StringComparison.CurrentCultureIgnoreCase));
                            BagType BagType = BagConfig.BagTypes.FirstOrDefault(x => x.Name.Equals(TypeName, StringComparison.CurrentCultureIgnoreCase));
                            if (BagType == null)
                            {
                                Monitor.Log(string.Format("Unable to execute command: <BagType> \"{0}\" is not valid. Expected valid values: {1}", TypeName, string.Join(", ", ValidTypes)), LogLevel.Alert);
                            }
                            else
                            {
                                if (!BagType.SizeSettings.Any(x => x.Size == Size))
                                {
                                    Monitor.Log(string.Format("Unable to execute command: Type='{0}' does not contain a configuration for Size='{1}'", TypeName, SizeName), LogLevel.Alert);
                                }
                                else
                                {
                                    try
                                    {
                                        BoundedBag NewBag = new BoundedBag(BagType, Size, false);
                                        Game1.player.addItemToInventory(NewBag);
                                    }
                                    catch (Exception ex)
                                    {
                                        Monitor.Log(string.Format("ItemBags: Unhandled error while executing command: {0}", ex.Message), LogLevel.Error);
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        private static void RegisterAddBundleBagCommand()
        {
            List<string> ValidSizes = BundleBag.ValidSizes.Select(x => x.ToString()).ToList();

            //Possible TODO: Add translation support for this command
            string CommandName = Constants.TargetPlatform == GamePlatform.Android ? "addbundlebag" : "player_addbundlebag";
            string CommandHelp = string.Format("Adds an empty Bundle Bag of the desired size to your inventory.\n"
                + "Arguments: <BagSize>\n"
                + "Example: {0} Large\n\n"
                + "Valid values for <BagSize>: {1}\n\n",
                CommandName, string.Join(", ", ValidSizes));
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                if (Game1.player.isInventoryFull())
                {
                    Monitor.Log("Unable to execute command: Inventory is full!", LogLevel.Alert);
                }
                else if (Args.Length < 1)
                {
                    Monitor.Log("Unable to execute command: Required arguments missing!", LogLevel.Alert);
                }
                else
                {
                    string SizeName = Args[0];
                    if (!Enum.TryParse(SizeName, out ContainerSize Size) || !ValidSizes.Contains(SizeName))
                    {
                        Monitor.Log(string.Format("Unable to execute command: <BagSize> \"{0}\" is not valid. Expected valid values: {1}", SizeName, string.Join(", ", ValidSizes)), LogLevel.Alert);
                    }
                    else
                    {
                        try
                        {
                            BundleBag NewBag = new BundleBag(Size, true);
                            Game1.player.addItemToInventory(NewBag);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log(string.Format("ItemBags: Unhandled error while executing command: {0}", ex.Message), LogLevel.Error);
                        }
                    }
                }
            });
        }

        private static void RegisterAddRucksackCommand()
        {
            List<string> ValidSizes = Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>().Select(x => x.ToString()).ToList();

            //Possible TODO: Add translation support for this command
            string CommandName = Constants.TargetPlatform == GamePlatform.Android ? "addrucksack" : "player_addrucksack";
            string CommandHelp = string.Format("Adds an empty Rucksack of the desired size to your inventory.\n"
                + "Arguments: <BagSize>\n"
                + "Example: {0} Large\n\n"
                + "Valid values for <BagSize>: {1}\n\n",
                CommandName, string.Join(", ", ValidSizes));
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                if (Game1.player.isInventoryFull())
                {
                    Monitor.Log("Unable to execute command: Inventory is full!", LogLevel.Alert);
                }
                else if (Args.Length < 1)
                {
                    Monitor.Log("Unable to execute command: Required arguments missing!", LogLevel.Alert);
                }
                else
                {
                    string SizeName = Args[0];
                    if (!Enum.TryParse(SizeName, out ContainerSize Size) || !ValidSizes.Contains(SizeName))
                    {
                        Monitor.Log(string.Format("Unable to execute command: <BagSize> \"{0}\" is not valid. Expected valid values: {1}", SizeName, string.Join(", ", ValidSizes)), LogLevel.Alert);
                    }
                    else
                    {
                        try
                        {
                            Rucksack NewBag = new Rucksack(Size, true);
                            Game1.player.addItemToInventory(NewBag);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log(string.Format("ItemBags: Unhandled error while executing command: {0}", ex.Message), LogLevel.Error);
                        }
                    }
                }
            });
        }

        private static void RegisterAddOmniBagCommand()
        {
            List<string> ValidSizes = Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>().Select(x => x.ToString()).ToList();

            //Possible TODO: Add translation support for this command
            string CommandName = Constants.TargetPlatform == GamePlatform.Android ? "addomnibag" : "player_addomnibag";
            string CommandHelp = string.Format("Adds an empty Omni Bag of the desired size to your inventory.\n"
                + "Arguments: <BagSize>\n"
                + "Example: {0} Large\n\n"
                + "Valid values for <BagSize>: {1}\n\n",
                CommandName, string.Join(", ", ValidSizes));
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                if (Game1.player.isInventoryFull())
                {
                    Monitor.Log("Unable to execute command: Inventory is full!", LogLevel.Alert);
                }
                else if (Args.Length < 1)
                {
                    Monitor.Log("Unable to execute command: Required arguments missing!", LogLevel.Alert);
                }
                else
                {
                    string SizeName = Args[0];
                    if (!Enum.TryParse(SizeName, out ContainerSize Size) || !ValidSizes.Contains(SizeName))
                    {
                        Monitor.Log(string.Format("Unable to execute command: <BagSize> \"{0}\" is not valid. Expected valid values: {1}", SizeName, string.Join(", ", ValidSizes)), LogLevel.Alert);
                    }
                    else
                    {
                        try
                        {
                            OmniBag NewBag = new OmniBag(Size);
                            Game1.player.addItemToInventory(NewBag);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log(string.Format("ItemBags: Unhandled error while executing command: {0}", ex.Message), LogLevel.Error);
                        }
                    }
                }
            });
        }

        private static void RegisterGenerateModdedBagCommand()
        {
            string CommandName = "generate_modded_bag";
            string CommandHelp = string.Format("Creates a json file that defines a modded Item Bag for a particular mod.\n"
                + "Arguments: <BagName> <ModUniqueID> (This is the 'ModUniqueID' value of the mod's manifest.json that you want to generate the file for. All modded items belonging to this mod will be included in the generated modded bag)\n"
                + "If the BagName is multiple words, wrap it in double quotes."
                + "Example: {0} \"Artisan Valley Bag\" ppja.artisanvalleymachinegoods\n\n",
                CommandName);
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                try
                {
                    if (!Context.IsWorldReady)
                    {
                        Monitor.Log("Unable to execute command: JsonAssets has not finished loading modded items. You must load a save file before using this command.", LogLevel.Alert);
                    }
                    else if (Args.Length < 2)
                    {
                        Monitor.Log("Unable to execute command: Required arguments missing. You must specify a Bag Name and a ModUniqueID. All modded items from the given ModUniqueID will be included in the generated bag.", LogLevel.Alert);
                    }
                    else
                    {
                        string BagName = Args[0];
                        string ModUniqueId = string.Join(" ", Args.Skip(1));
                        if (!Helper.ModRegistry.IsLoaded(ModUniqueId))
                        {
                            string Message = string.Format("Unable to execute command: ModUniqueID = '{0}' is not installed. "
                                + "Either install this mod first, or double check that you used the correct value for ModUniqueID. "
                                + "The ModUniqueID can be found in the mod's manifest.json file.", ModUniqueId);
                            Monitor.Log(Message, LogLevel.Alert);
                        }
                        else
                        {
                            List<ContainerSize> AllSizes = Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>().ToList();

                            ModdedBag ModdedBag = new ModdedBag()
                            {
                                IsEnabled = true,
                                ModUniqueId = ModUniqueId,
                                Guid = ModdedBag.StringToGUID(ModUniqueId + BagName).ToString(),
                                BagName = BagName,
                                BagDescription = string.Format("A bag for storing items belonging to {0} mod", ModUniqueId),
                                IconTexture = BagType.SourceTexture.SpringObjects,
                                IconPosition = new Rectangle(),
                                Prices = AllSizes.ToDictionary(x => x, x => BagTypeFactory.DefaultPrices[x]),
                                Capacities = AllSizes.ToDictionary(x => x, x => BagTypeFactory.DefaultCapacities[x]),
                                Sellers = AllSizes.ToDictionary(x => x, x => new List<BagShop>() { BagShop.Pierre }),
                                MenuOptions = AllSizes.ToDictionary(x => x, x => new BagMenuOptions() {
                                    GroupedLayoutOptions = new BagMenuOptions.GroupedLayout() {
                                        GroupsPerRow = 5
                                    }
                                }),
                                Items = ModdedBag.GetModdedItems(ModUniqueId)
                            };

                            string OutputDirectory = Path.Combine(Helper.DirectoryPath, "assets", "Modded Bags");
                            string DesiredFilename = ModdedBag.ModUniqueId;
                            string CurrentFilename = DesiredFilename;
                            int CurrentIndex = 0;
                            while (File.Exists(Path.Combine(OutputDirectory, CurrentFilename + ".json")))
                            {
                                CurrentIndex++;
                                CurrentFilename = string.Format("{0} ({1})", DesiredFilename, CurrentIndex);
                            }

                            string RelativePath = Path.Combine("assets", "Modded Bags", CurrentFilename + ".json");
                            Helper.Data.WriteJsonFile(RelativePath, ModdedBag);

                            Monitor.Log($"File exported to: {Path.Combine(Helper.DirectoryPath, RelativePath)}\nYou will need to re-launch the game for this file to be loaded.", LogLevel.Alert);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"ItemBags: Unhandled error while executing command: {ex.Message}", LogLevel.Error);
                }
            });

            CommandName = "generate_category_modded_bag";
            CommandHelp = string.Format("Creates a json file that defines a modded Item Bag for a particular category of items.\n"
                + "Arguments: <IncludeVanillaItems> <IncludeModdedItems> <SortingOrder> <BagName> <CategoryIdOrName>\n"
                + "IncludeVanillaItems: Possible values are 'true' or 'false' (without the quotes).\n\tIf true, the generated bag will be able to store vanilla (non-modded) items of the desired category(s).\n"
                + "IncludeModdedItems: Possible values are 'true' or 'false' (without the quotes).\n\tIf true, the generated bag will be able to store modded items of the desired category(s).\n"
                + "SortingOrder: A comma-separated list of what properties to sort the items by before exporting them.\n\tValid properties are {0}/{1}/{2}/{3}.\n\tYou can also choose {4} or {5} order for each property.\n\t"
                + "For example, '{6}-{7},{8}-{9}' would first sort by the {10} name in {11} order,\n\tand then sort by the item {12} in {13} order.\n"
                + "BagName: The name of the exported bag.\n\tIf the name is multiple words, enclose it in double quotes.\n"
                + "CategoryId: The Id of the category.\n\tMost categories can be found here: https://stardewcommunitywiki.com/Modding:Object_data#Categories \n\tYou can specify as many categories as you want.\n"
                + "Example: {14} true true \"Dairy Bag\" -5 -6\n\t"
                + "This would generate a bag that can store any items belonging to the Egg or Milk categories.",
                SortingProperty.Name, SortingProperty.Id, SortingProperty.Category, SortingProperty.SingleValue, 
                SortingOrder.Ascending, SortingOrder.Descending, 
                SortingProperty.Category, SortingOrder.Ascending, SortingProperty.Id, SortingOrder.Descending, SortingProperty.Category, SortingOrder.Ascending, SortingProperty.Id, SortingOrder.Descending,
                CommandName);
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                try
                {
                    if (!Context.IsWorldReady)
                    {
                        Monitor.Log("Unable to execute command: You must load a save file before using this command.", LogLevel.Alert);
                    }
                    else if (Args.Length < 5)
                    {
                        Monitor.Log(string.Format("Unable to execute command: Required arguments missing. Type 'help {0}' for more details on using this command.", CommandName), LogLevel.Alert);
                    }
                    else if (!bool.TryParse(Args[0], out bool IncludeVanillaItems))
                    {
                        Monitor.Log(string.Format("Unable to execute command: '{0}' is not a valid value for the IncludeVanillaItems parameter. Expected values are 'true' or 'false' (without the quotes)", Args[0]), LogLevel.Alert);
                    }
                    else if (!bool.TryParse(Args[1], out bool IncludeModdedItems))
                    {
                        Monitor.Log(string.Format("Unable to execute command: '{0}' is not a valid value for the IncludeModdedItems parameter. Expected values are 'true' or 'false' (without the quotes)", Args[1]), LogLevel.Alert);
                    }
                    else
                    {
                        string[] SortingArgs = Args[2].Split(',');
                        if (SortingArgs.Length == 0)
                        {
                            Monitor.Log(string.Format("Unable to execute command: '{0}' is not a valid value for the SortingOrder parameter. Type 'help {1}' for more details on using this command.", Args[2], CommandName), LogLevel.Alert);
                        }
                        else
                        {
                            bool IsOrderValid = true;
                            List<Persistence.KeyValuePair<SortingProperty, SortingOrder>> OrderBy = new List<Persistence.KeyValuePair<SortingProperty, SortingOrder>>();
                            foreach (string Value in SortingArgs)
                            {
                                string PropertyName;
                                string OrderName;
                                if (Value.Contains('-'))
                                {
                                    PropertyName = Value.Split('-')[0];
                                    OrderName = Value.Split('-')[1];
                                }
                                else
                                {
                                    PropertyName = Value;
                                    OrderName = SortingOrder.Ascending.ToString();
                                }

                                if (!Enum.TryParse(PropertyName, out SortingProperty Property))
                                {
                                    Monitor.Log(string.Format("Unable to execute command: '{0}' is not a valid sorting property. Expected values are: {1} {2} {3} {4}. " +
                                        "Type 'help {5}' for more details on using this command.", 
                                        PropertyName, SortingProperty.Name, SortingProperty.Id, SortingProperty.Category, SortingProperty.SingleValue, CommandName), LogLevel.Alert);
                                    IsOrderValid = false;
                                    break;
                                }

                                if (!Enum.TryParse(OrderName, out SortingOrder Direction))
                                {
                                    Monitor.Log(string.Format("Unable to execute command: '{0}' is not a valid sorting direction. Expected values are: {1} or {2}. " +
                                        "Type 'help {3}' for more details on using this command.",
                                        OrderName, SortingOrder.Ascending, SortingOrder.Descending, CommandName), LogLevel.Alert);
                                    IsOrderValid = false;
                                    break;
                                }

                                OrderBy.Add(new Persistence.KeyValuePair<SortingProperty, SortingOrder>(Property, Direction));
                            }

                            if (IsOrderValid)
                            {
                                string BagName = Args[3];

                                HashSet<int> CategoryValues = new HashSet<int>(Args.Skip(4).Select(x => int.Parse(x)));

                                List<ItemData> TargetItems = new List<ItemData>();

                                //  Get the items that match the desired conditions
                                foreach (var KVP in Game1.objectData)
                                {
                                    string Id = KVP.Key;
                                    ObjectData Data = KVP.Value;
                                    bool IsModdedItem = !int.TryParse(Id, out _);

                                    if ((IsModdedItem && IncludeModdedItems) || (!IsModdedItem && IncludeVanillaItems))
                                    {
                                        if (CategoryValues.Contains(Data.Category))
                                        {
                                            TargetItems.Add(ItemData.FromObjectData(Id, Data, IsModdedItem));
                                        }
                                    }
                                }
                                foreach (var KVP in Game1.bigCraftableData)
                                {
                                    string Id = KVP.Key;
                                    BigCraftableData Data = KVP.Value;
                                    bool IsModdedItem = !int.TryParse(Id, out _);

                                    if ((IsModdedItem && IncludeModdedItems) || (!IsModdedItem && IncludeVanillaItems))
                                    {
                                        ItemData ItemData = ItemData.FromBigCraftableData(Id, Data, IsModdedItem);
                                        if (CategoryValues.Contains(ItemData.Instance.Category))
                                        {
                                            TargetItems.Add(ItemData);
                                        }
                                    }
                                }

                                //  Sort the items
                                IOrderedEnumerable<ItemData> SortedItems = ApplySorting(TargetItems, OrderBy[0].Key, OrderBy[0].Value);
                                foreach (var KVP in OrderBy.Skip(1))
                                    SortedItems = ApplySorting(SortedItems, KVP.Key, KVP.Value);

                                //  Generate the bag
                                List<ContainerSize> AllSizes = Enum.GetValues(typeof(ContainerSize)).Cast<ContainerSize>().ToList();
                                ModdedBag ModdedBag = new ModdedBag()
                                {
                                    IsEnabled = true,
                                    ModUniqueId = ItemBagsMod.ModUniqueId,
                                    Guid = ModdedBag.StringToGUID(ItemBagsMod.ModUniqueId + BagName).ToString(),
                                    BagName = BagName,
                                    BagDescription = "A bag for storing items of specific category(s)",
                                    IconTexture = BagType.SourceTexture.SpringObjects,
                                    IconPosition = new Rectangle(),
                                    Prices = AllSizes.ToDictionary(x => x, x => BagTypeFactory.DefaultPrices[x]),
                                    Capacities = AllSizes.ToDictionary(x => x, x => BagTypeFactory.DefaultCapacities[x]),
                                    Sellers = AllSizes.ToDictionary(x => x, x => new List<BagShop>() { BagShop.Pierre }),
                                    MenuOptions = AllSizes.ToDictionary(x => x, x => new BagMenuOptions()
                                    {
                                        GroupedLayoutOptions = new BagMenuOptions.GroupedLayout()
                                        {
                                            GroupsPerRow = 5
                                        },
                                        UngroupedLayoutOptions = new BagMenuOptions.UngroupedLayout()
                                        {
                                            Columns = 18
                                        }
                                    }),
                                    Items = SortedItems.Select(x => new ModdedItem(x.Instance, true)).ToList()
                                };

                                string OutputDirectory = Path.Combine(Helper.DirectoryPath, "assets", "Modded Bags");
                                string DesiredFilename = string.Format("Category-{0}", BagName);
                                string CurrentFilename = DesiredFilename;
                                int CurrentIndex = 0;
                                while (File.Exists(Path.Combine(OutputDirectory, CurrentFilename + ".json")))
                                {
                                    CurrentIndex++;
                                    CurrentFilename = string.Format("{0} ({1})", DesiredFilename, CurrentIndex);
                                }

                                string RelativePath = Path.Combine("assets", "Modded Bags", CurrentFilename + ".json");
                                Helper.Data.WriteJsonFile(RelativePath, ModdedBag);

                                Monitor.Log($"File exported to: {Path.Combine(Helper.DirectoryPath, RelativePath)}\nYou will need to re-launch the game for this file to be loaded.", LogLevel.Alert);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log($"ItemBags: Unhandled error while executing command: {ex.Message}", LogLevel.Error);
                }
            });
        }

        public readonly record struct ItemData(string ItemId, int Category, bool IsModdedItem, SObject Instance = null)
        {
            public SObject Instance { get; init; } = Instance ?? ItemRegistry.Create<SObject>(ItemId);

            public static ItemData FromObjectData(string Id, ObjectData Data, bool IsModdedItem)
            {
                if (!IsModdedItem && !string.IsNullOrEmpty(Id) && int.TryParse(Id, out int UnqualifiedId))
                    Id = $"(O){UnqualifiedId}";
                return new(Id ?? Data.Name, Data.Category, IsModdedItem);
            }

            public static ItemData FromBigCraftableData(string Id, BigCraftableData Data, bool IsModdedItem)
            {
                if (!IsModdedItem && !string.IsNullOrEmpty(Id) && int.TryParse(Id, out int UnqualifiedId))
                    Id = $"(BC){UnqualifiedId}";
                SObject Instance = ItemRegistry.Create<SObject>(Id ?? Data.Name);
                return new(Id ?? Data.Name, Instance.Category, IsModdedItem, Instance);
            }
        }

        private static IOrderedEnumerable<ItemData> ApplySorting(IEnumerable<ItemData> Source, SortingProperty Property, SortingOrder Direction)
        {
            if (Source is IOrderedEnumerable<ItemData> OrderedSource)
            {
                if (Direction == SortingOrder.Ascending)
                    return OrderedSource.ThenBy(x => GetSortValue(x.Instance, Property));
                else
                    return OrderedSource.ThenByDescending(x => GetSortValue(x.Instance, Property));
            }
            else
            {
                if (Direction == SortingOrder.Ascending)
                    return Source.OrderBy(x => GetSortValue(x.Instance, Property));
                else
                    return Source.OrderByDescending(x => GetSortValue(x.Instance, Property));
            }
        }

        private static string GetSortValue(SObject Instance, SortingProperty Property)
        {
            return Property switch
            {
                SortingProperty.Name => Instance.DisplayName,
                SortingProperty.Id => Instance.ParentSheetIndex.ToString("D4"),
                SortingProperty.Category => $"{Instance.getCategoryName()} {Instance.getCategorySortValue().ToString("D3")}",
                SortingProperty.SingleValue => ItemBag.GetSingleItemPrice(Instance).ToString("D6"),
                _ => throw new NotImplementedException($"Unimplemented {nameof(SortingProperty)} '{Property}' in {nameof(CommandHandler)}.{nameof(GetSortValue)}"),
            };
        }

        private static void RegisterReloadConfigCommand()
        {
            //Possible TODO: Add translation support for this command
            string CommandName = "itembags_reload_config";
            string CommandHelp = "Reloads configuration settings from this mod's config.json file. Normally this file's settings are only loaded once when the game is started."
                + " Use this command if you've made changes to the config during this game session.";
            Helper.ConsoleCommands.Add(CommandName, CommandHelp, (string Name, string[] Args) =>
            {
                try
                {
                    ItemBagsMod.LoadUserConfig();
                    Monitor.Log("config.json settings were successfully reloaded.", LogLevel.Alert);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"ItemBags: Unhandled error while executing command: {ex.Message}", LogLevel.Error);
                }
            });
        }
    }
}
