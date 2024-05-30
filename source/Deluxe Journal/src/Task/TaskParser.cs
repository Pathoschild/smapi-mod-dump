/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Tools;
using StardewValley.TokenizableStrings;
using DeluxeJournal.Task.Tasks;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task
{
    /// <summary>Parses text into tasks.</summary>
    public class TaskParser
    {
        /// <summary>Mode of operation for parsing.</summary>
        public enum ParseMode
        {
            /// <summary>Default mode. Parse text and create a new factory.</summary>
            CreateFactory,

            /// <summary>Keep the current factory and update its parameters with the parsed values.</summary>
            UpdateFactory,

            /// <summary>Only update the parsed values. Ignore the factory entirely.</summary>
            UpdateValues
        }

        private readonly ITranslationHelper _translation;
        private readonly TaskParserSettings _settings;
        private readonly LocalizedGameDataMaps _localizedGameData;
        private readonly IDictionary<string, HashSet<string>> _keywords;

        private string _id;
        private string _npcName;
        private string _buildingType;
        private int? _count;
        private IEnumerable<string>? _itemIds;
        private IEnumerable<string>? _farmAnimals;
        private TaskFactory? _factory;

        private Item? _cachedItem;
        private string? _cachedNpcDisplayName;
        private string? _cachedBuildingDisplayName;
        private string? _cachedFarmAnimalDisplayName;

        /// <summary>The ID of the matched task.</summary>
        public string ID => _id;

        /// <summary>The parsed count value.</summary>
        public int Count => _count ?? 1;

        /// <summary>Parsed item IDs.</summary>
        public IEnumerable<string>? ItemIds
        {
            get => _itemIds;

            private set
            {
                _itemIds = value;
                _cachedItem = null;
            }
        }

        /// <summary>Parsed item instance that represents the group of item IDs (if there is more than one).</summary>
        public Item? ProxyItem
        {
            get
            {
                if (_cachedItem == null && ItemIds != null && ItemIds.Any())
                {
                    string itemId = ItemIds.First();

                    if (itemId.StartsWith('-'))
                    {
                        if (!int.TryParse(itemId, out int category))
                        {
#if DEBUG
                            if (DeluxeJournalMod.Instance is DeluxeJournalMod instance)
                            {
                                instance.Monitor.Log($"{nameof(TaskParser)}.{nameof(ProxyItem)}: invalid item ID for category group, id='{itemId}'", LogLevel.Debug);
                            }
#endif
                            ItemIds = null;
                            return _cachedItem = null;
                        }

                        return _cachedItem = ItemRegistry.Create(category switch
                        {
                            SObject.GreensCategory => "(O)20",
                            SObject.GemCategory => "(O)80",
                            SObject.VegetableCategory => "(O)24",
                            SObject.FishCategory => "(O)145",
                            SObject.EggCategory => "(O)176",
                            SObject.MilkCategory => "(O)184",
                            _ => null
                        });
                    }

                    if (FlavoredItemHelper.CreateFlavoredItem(itemId) is Item flavoredItem)
                    {
                        return _cachedItem = flavoredItem;
                    }

                    if (ItemRegistry.GetData(itemId)?.RawData is ToolData toolData
                        && ToolHelper.IsToolUpgradable(toolData)
                        && ToolHelper.IsToolBaseUpgradeLevel(toolData))
                    {
                        _cachedItem = ToolHelper.GetToolUpgradeForPlayer(toolData, Game1.player);
                    }

                    if (_cachedItem == null)
                    {
                        _cachedItem = ItemRegistry.Create(itemId, allowNull: true);
                    }
                }

                return _cachedItem;
            }
        }

        /// <summary>Localized display name for the parsed item group.</summary>
        public string ProxyItemDisplayName
        {
            get
            {
                if (ItemIds != null && ProxyItem is Item item)
                {
                    if (ItemIds.First().StartsWith('-'))
                    {
                        return item.Category switch
                        {
                            SObject.GreensCategory => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568"),
                            SObject.GemCategory => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569"),
                            SObject.VegetableCategory => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570"),
                            SObject.FishCategory => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571"),
                            SObject.EggCategory => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572"),
                            SObject.MilkCategory => Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573"),
                            _ => "???"
                        };
                    }

                    return item.DisplayName;
                }

                return string.Empty;
            }
        }

        /// <summary>The parsed NPC's internal name.</summary>
        public string NpcName
        {
            get => _npcName;

            private set
            {
                _npcName = value;
                _cachedNpcDisplayName = null;
            }
        }

        /// <summary>Localized NPC display name.</summary>
        public string NpcDisplayName
        {
            get
            {
                if (_cachedNpcDisplayName == null && Game1.characterData.TryGetValue(_npcName, out var data))
                {
                    _cachedNpcDisplayName = TokenParser.ParseText(data.DisplayName);
                }

                return _cachedNpcDisplayName ?? _npcName;
            }
        }

        /// <summary>The parsed building type.</summary>
        public string BuildingType
        {
            get => _buildingType;

            private set
            {
                _buildingType = value;
                _cachedBuildingDisplayName = null;
            }
        }

        /// <summary>Localized building display name.</summary>
        public string BuildingDisplayName
        {
            get
            {
                if (_cachedBuildingDisplayName == null && Game1.buildingData.TryGetValue(_buildingType, out var data))
                {
                    _cachedBuildingDisplayName = TokenParser.ParseText(data.Name);
                }

                return _cachedBuildingDisplayName ?? _buildingType;
            }
        }

        /// <summary>The parsed farm animal name.</summary>
        public IEnumerable<string>? FarmAnimals
        {
            get => _farmAnimals;

            private set
            {
                _farmAnimals = value;
                _cachedFarmAnimalDisplayName = null;
            }
        }

        /// <summary>Localized farm animal display name.</summary>
        public string FarmAnimalDisplayName
        {
            get
            {
                string farmAnimalName = string.Empty;

                if (_cachedFarmAnimalDisplayName == null
                    && FarmAnimals != null
                    && FarmAnimals.Any()
                    && Game1.farmAnimalData.TryGetValue(farmAnimalName = FarmAnimals.First(), out var data))
                {
                    _cachedFarmAnimalDisplayName = TokenParser.ParseText(data.ShopDisplayName ?? data.DisplayName);
                }

                return _cachedFarmAnimalDisplayName ?? farmAnimalName;
            }
        }

        /// <summary><see cref="TaskFactory"/> corresponding to the matched task ID.</summary>
        public TaskFactory Factory
        {
            get => _factory ??= GenerateFactory();

            set
            {
                Type factoryType = value.GetType();

                if (factoryType != TaskRegistry.GetFactoryType(_id))
                {
                    foreach (string id in TaskRegistry.Keys)
                    {
                        if (TaskRegistry.GetFactoryType(id) == factoryType)
                        {
                            _id = id;
                            break;
                        }
                    }
                }

                _factory = value;
            }
        }

        public TaskParser(ITranslationHelper translation) : this(translation, new())
        {
        }

        public TaskParser(ITranslationHelper translation, TaskParserSettings settings)
        {
            _translation = translation;
            _settings = settings;
            _localizedGameData = new LocalizedGameDataMaps(translation, settings, DeluxeJournalMod.Instance?.Monitor);
            _keywords = new Dictionary<string, HashSet<string>>();
            _id = TaskTypes.Basic;
            _npcName = string.Empty;
            _buildingType = string.Empty;
            _count = null;

            PopulateKeywords(_keywords);
        }

        /// <summary>Clear cached parser state.</summary>
        /// <param name="excludeType">Exclude type information: ID and <see cref="TaskFactory"/>.</param>
        public void Clear(bool excludeType = false)
        {
            if (!excludeType)
            {
                _id = TaskTypes.Basic;
                _factory = null;
            }

            _count = null;
            ItemIds = null;
            FarmAnimals = null;
            NpcName = string.Empty;
            BuildingType = string.Empty;
        }

        /// <summary>Matched a non-basic task.</summary>
        public bool MatchFound()
        {
            return _id != TaskTypes.Basic;
        }

        /// <summary>Parse text and update the parser state with the results.</summary>
        /// <param name="text">Raw text to be parsed.</param>
        /// <param name="mode">Parser mode.</param>
        /// <returns>
        /// <c>true</c> if the parsed text produced a factory in a ready state, or the <paramref name="mode"/>
        /// is <see cref="ParseMode.UpdateValues"/>; <c>false</c> if the text did not match any
        /// task (excluding "basic" tasks).
        /// </returns>
        public bool Parse(string text, ParseMode mode = ParseMode.CreateFactory)
        {
            if (mode == ParseMode.CreateFactory)
            {
                Clear();
            }

            if (text.Length == 0)
            {
                return false;
            }

            HashSet<string> ids = new HashSet<string>();
            HashSet<int> ignored = new HashSet<int>();
            List<string> keywords = new List<string>();
            string[] words = text.Trim().Split(' ');

            for (int group = Math.Min(words.Length, 3); group > 0; group--)
            {
                for (int i = words.Length; i >= group; i--)
                {
                    string word = string.Join(" ", words[(i - group)..i]).ToLower()
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace("'", "")
                        .Replace(".", "")
                        .Replace(",", "")
                        .Replace("!", "")
                        .Replace("?", "");

                    if (word.Length == 0)
                    {
                        continue;
                    }

                    for (int j = i - group; j < i; j++)
                    {
                        if (ignored.Contains(j))
                        {
                            goto skip;
                        }
                    }

                    if (mode == ParseMode.CreateFactory && _keywords.ContainsKey(word))
                    {
                        keywords.Add(word);
                    }
                    else if (!HandleWord(word))
                    {
                        continue;
                    }

                    for (int j = i - group; j < i; j++)
                    {
                        ignored.Add(j);
                    }
                skip:
                    ;
                }
            }

            switch (mode)
            {
                case ParseMode.CreateFactory:
                    if (keywords.Count > 0)
                    {
                        ids.UnionWith(_keywords[keywords.Last()]);

                        for (int i = 0; i < keywords.Count - 1; i++)
                        {
                            HandleWord(keywords[i]);
                        }
                    }

                    foreach (string id in TaskRegistry.PriorityOrderedKeys)
                    {
                        if (ids.Contains(id))
                        {
                            _id = id;
                            Factory = GenerateFactory();

                            if (Factory.IsReady())
                            {
                                return true;
                            }
                        }
                    }

                    _id = TaskTypes.Basic;
                    Factory = TaskRegistry.BasicFactory;
                    break;
                case ParseMode.UpdateFactory:
                    foreach (var parameter in Factory.GetParameters())
                    {
                        if (!SetParameterValue(parameter))
                        {
                            ApplyParameterValue(parameter);
                        }
                    }

                    return Factory.IsReady();
                case ParseMode.UpdateValues:
                    return true;
            }

            return false;

            bool HandleWord(string word)
            {
                if (int.TryParse(word.Trim('x'), out int count) && count > 0)
                {
                    _count = count;
                }
                else if (!_settings.IgnoreNpcs && _localizedGameData.LocalizedNpcs.TryGetValue(word, out var name))
                {
                    NpcName = name;
                }
                else if (!_settings.IgnoreItems && _localizedGameData.LocalizedItems.TryGetValues(word, out var itemIds))
                {
                    ItemIds = itemIds;
                }
                else if (!_settings.IgnoreBuildings && _localizedGameData.LocalizedBuildings.TryGetValue(word, out var buildingType))
                {
                    BuildingType = buildingType;
                }
                else if (!_settings.IgnoreFarmAnimals && _localizedGameData.LocalizedFarmAnimals.TryGetValues(word, out var farmAnimals))
                {
                    FarmAnimals = farmAnimals;
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>Set the value of a <see cref="TaskParameter"/> based on the parser state.</summary>
        /// <param name="parameter">The parameter to update.</param>
        /// <returns>Whether the parameter value was set.</returns>
        public bool SetParameterValue(TaskParameter parameter)
        {
            Type propertyType = parameter.Type;
            TaskParameterTag tag = parameter.Attribute.Tag;

            if (tag.Equals(TaskParameterTag.NpcName) && propertyType == typeof(string))
            {
                return parameter.TrySetValue(NpcName);
            }
            else if (tag.Equals(TaskParameterTag.Building) && propertyType == typeof(string))
            {
                return parameter.TrySetValue(BuildingType);
            }
            else if (tag.Equals(TaskParameterTag.ItemList) && propertyType == typeof(IList<string>))
            {
                return ItemIds != null && parameter.TrySetValue(ItemIds.ToList());
            }
            else if (tag.Equals(TaskParameterTag.FarmAnimalList) && propertyType == typeof(IList<string>))
            {
                return FarmAnimals != null && parameter.TrySetValue(FarmAnimals.ToList());
            }
            else if (tag.Equals(TaskParameterTag.Count) && propertyType == typeof(int))
            {
                return parameter.TrySetValue(_count);
            }
            else
            {
                return false;
            }
        }

        /// <summary>Update the parser state with the value of a <see cref="TaskParameter"/>.</summary>
        /// <param name="parameter">The task parameter to use.</param>
        /// <returns>Whether the parameter value was applied.</returns>
        public bool ApplyParameterValue(TaskParameter parameter)
        {
            TaskParameterTag tag = parameter.Attribute.Tag;

            if (tag.Equals(TaskParameterTag.NpcName))
            {
                NpcName = parameter.Value is string npcName ? npcName : string.Empty;
            }
            else if (tag.Equals(TaskParameterTag.Building))
            {
                BuildingType = parameter.Value is string buildingType ? buildingType : string.Empty;
            }
            else if (tag.Equals(TaskParameterTag.ItemList))
            {
                ItemIds = parameter.Value is IList<string> itemIds ? itemIds.ToList() : null;
            }
            else if (tag.Equals(TaskParameterTag.FarmAnimalList))
            {
                FarmAnimals = parameter.Value is IList<string> farmAnimals ? farmAnimals.ToList() : null;
            }
            else if (tag.Equals(TaskParameterTag.Count))
            {
                _count = parameter.Value is int count ? count : 1;
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>Whether the smart icons for the given flags be shown.</summary>
        public bool ShouldShowSmartIcon(SmartIconFlags flags)
        {
            SmartIconFlags enabled = Factory.EnabledSmartIcons & flags;

            if (enabled.HasFlag(SmartIconFlags.Npc))
            {
                return !string.IsNullOrEmpty(NpcName);
            }
            else if (enabled.HasFlag(SmartIconFlags.Building))
            {
                return !string.IsNullOrEmpty(BuildingType);
            }
            else if (enabled.HasFlag(SmartIconFlags.Item))
            {
                return ItemIds != null;
            }
            else if (enabled.HasFlag(SmartIconFlags.Animal))
            {
                return FarmAnimals != null;
            }
            else
            {
                return false;
            }
        }

        public bool ShouldShowCount()
        {
            return Count > 1 && Factory.EnableSmartIconCount;
        }

        /// <summary>Generate a task.</summary>
        public ITask GenerateTask(string name)
        {
            return Factory.Create(name) ?? new BasicTask(name);
        }

        /// <summary>Generate a TaskFactory corresponding to the matched task ID.</summary>
        private TaskFactory GenerateFactory()
        {
            TaskFactory factory = TaskRegistry.CreateFactoryInstance(_id);

            foreach (var parameter in factory.GetParameters())
            {
                SetParameterValue(parameter);
            }

            return factory;
        }

        private void PopulateKeywords(IDictionary<string, HashSet<string>> keywords)
        {
            foreach (string id in TaskRegistry.Keys)
            {
                Translation keywordList = _translation.Get("task." + id + ".keywords").UsePlaceholder(false);

                if (keywordList.HasValue())
                {
                    foreach (string keyword in keywordList.ToString().Split(','))
                    {
                        if (!keywords.ContainsKey(keyword))
                        {
                            keywords[keyword] = new HashSet<string>();
                        }

                        keywords[keyword].Add(id);
                    }
                }
            }
        }
    }
}
