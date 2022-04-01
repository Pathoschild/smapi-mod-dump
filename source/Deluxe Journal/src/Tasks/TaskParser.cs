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
using DeluxeJournal.Framework.Tasks;
using DeluxeJournal.Util;

namespace DeluxeJournal.Tasks
{
    /// <summary>Parses text into tasks.</summary>
    public class TaskParser
    {
        private readonly ITranslationHelper _translation;
        private readonly LocalizedObjects _localizedObjects;
        private readonly IDictionary<string, HashSet<string>> _keywords;
        private string _id;
        private int _count;
        private Item? _item;
        private NPC? _npc;
        private BlueprintInfo? _blueprint;
        private TaskFactory? _tempFactory;

        /// <summary>The ID of the matched task.</summary>
        public string ID => _id;

        /// <summary>The parsed count value.</summary>
        public int Count => _count;

        /// <summary>The parsed Item value.</summary>
        public Item? Item => _item;

        /// <summary>The parsed NPC value.</summary>
        public NPC? NPC => _npc;

        /// <summary>The parsed BlueprintInfo.</summary>
        public BlueprintInfo? Blueprint => _blueprint;

        /// <summary>The smart icon Item to show.</summary>
        public Item? SmartIconItem => _tempFactory?.SmartIconItem();

        /// <summary>The smart icon NPC to show.</summary>
        public NPC? SmartIconNPC => _tempFactory?.SmartIconNPC();

        /// <summary>The smart icon name to show.</summary>
        public string? SmartIconName => _tempFactory?.SmartIconName();

        public TaskParser(ITranslationHelper translation)
        {
            _translation = translation;
            _localizedObjects = new LocalizedObjects(translation);
            _keywords = new Dictionary<string, HashSet<string>>();
            _id = TaskTypes.Basic;

            Init();
            PopulateKeywords(_keywords);
        }

        /// <summary>Reset parser state.</summary>
        public void Init()
        {
            _id = TaskTypes.Basic;
            _count = 1;
            _item = null;
            _npc = null;
            _blueprint = null;
            _tempFactory = null;
        }

        /// <summary>Matched a non-basic task.</summary>
        public bool MatchFound()
        {
            return _id != TaskTypes.Basic;
        }

        /// <summary>Parse text.</summary>
        public bool Parse(string text)
        {
            Init();

            if (text.Length == 0)
            {
                return false;
            }

            HashSet<string> ids = new HashSet<string>();
            HashSet<int> ignored = new HashSet<int>();
            string[] words = text.Trim().Split(' ');
            bool skip;

            for (int group = Math.Min(words.Length, 3); group > 0; group--)
            {
                for (int i = words.Length; i >= group; i--)
                {
                    string word = string.Join(" ", words[(i - group)..i]).ToLowerInvariant()
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace("'", "")
                        .Replace(".", "")
                        .Replace("!", "")
                        .Replace("?", "");
                    skip = false;

                    if (word.Length == 0)
                    {
                        continue;
                    }

                    for (int j = i - group; j < i; j++)
                    {
                        if (ignored.Contains(j))
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip)
                    {
                        continue;
                    }
                    else if (_keywords.ContainsKey(word))
                    {
                        ids.UnionWith(_keywords[word]);
                    }
                    else if (int.TryParse(word, out int count) && count > 0)
                    {
                        _count = count;
                    }
                    else if (_localizedObjects.GetNPC(word) is NPC npc)
                    {
                        _npc = npc;
                    }
                    else if (_localizedObjects.GetBlueprintInfo(word) is BlueprintInfo blueprint)
                    {
                        _blueprint = blueprint;
                    }
                    else if (_localizedObjects.GetItem(word) is Item item)
                    {
                        _item = item;
                    }
                    else
                    {
                        continue;
                    }

                    for (int j = i - group; j < i; j++)
                    {
                        ignored.Add(j);
                    }
                }
            }

            foreach (string id in TaskRegistry.PriorityOrderedKeys)
            {
                if (ids.Contains(id))
                {
                    _id = id;
                    _tempFactory = GenerateFactory();

                    if (_tempFactory.IsReady())
                    {
                        return true;
                    }
                }
            }

            _id = TaskTypes.Basic;
            _tempFactory = null;

            return false;
        }

        /// <summary>Set the value of a TaskParameter based on the parser state.</summary>
        public bool SetParameterValue(TaskParameter parameter)
        {
            Type propertyType = parameter.Type;
            string tag = parameter.Attribute.Tag;

            if (propertyType == typeof(Item))
            {
                parameter.Value = _item;
            }
            else if (propertyType == typeof(NPC))
            {
                parameter.Value = _npc;
            }
            else if (tag == "count" && propertyType == typeof(int))
            {
                parameter.Value = _count;
            }
            else if (tag == "cost" && propertyType == typeof(int))
            {
                parameter.Value = _blueprint?.Cost ?? 0;
            }
            else if ((tag == "building" || tag == "blueprint" || (tag == "animal" && (_blueprint == null || _blueprint.IsAnimal()))) && propertyType == typeof(string))
            {
                parameter.Value = _blueprint?.DisplayName ?? string.Empty;
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>Generate a TaskFactory corresponding to the matched task ID.</summary>
        public TaskFactory GenerateFactory()
        {
            TaskFactory factory = TaskRegistry.CreateFactoryInstance(_id);

            foreach (var parameter in factory.GetParameters())
            {
                SetParameterValue(parameter);
            }

            return factory;
        }

        /// <summary>Generate a task.</summary>
        public ITask GenerateTask(string name)
        {
            return GenerateFactory().Create(name) ?? new BasicTask(name);
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
