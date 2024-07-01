/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Task
{
    /// <summary>Settings for a <see cref="TaskParser"/> object.</summary>
    public class TaskParserSettings
    {
        private bool? _enableFuzzySearch;
        private bool? _ignoreItems;
        private bool? _ignoreNpcs;
        private bool? _ignoreBuildings;
        private bool? _ignoreFarmAnimals;
        private bool? _setItemCategoryObject;
        private bool? _setItemCategoryBigCraftable;
        private bool? _setItemCategoryCraftable;
        private bool? _setItemCategoryTool;

        /// <summary>
        /// Enable fuzzy search when matching game data. A fuzzy search allows matching
        /// substrings to the nearest display name.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool EnableFuzzySearch
        {
            get => _enableFuzzySearch ?? false;
            set => _enableFuzzySearch = value;
        }

        /// <summary>
        /// Ignore items when parsing.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreItems
        {
            get => _ignoreItems ?? false;
            set => _ignoreItems = value;
        }

        /// <summary>
        /// Ignore NPC names when parsing.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreNpcs
        {
            get => _ignoreNpcs ?? false;
            set => _ignoreNpcs = value;
        }

        /// <summary>
        /// Ignore building names when parsing.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreBuildings
        {
            get => _ignoreBuildings ?? false;
            set => _ignoreBuildings = value;
        }

        /// <summary>
        /// Ignore farm animal names when parsing.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreFarmAnimals
        {
            get => _ignoreFarmAnimals ?? false;
            set => _ignoreFarmAnimals = value;
        }

        /// <summary>
        /// Allow items of type <c>(O)</c> to be matched when parsing. When this flag is set,
        /// only items of this category, and those included in any additional <c>SetItemCategoryX</c>
        /// flags, are able to be matched.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool SetItemCategoryObject
        {
            get => _setItemCategoryObject ?? false;
            set => _setItemCategoryObject = value;
        }

        /// <summary>
        /// Allow items of type <c>(BC)</c> to be matched when parsing. When this flag is set,
        /// only items of this category, and those included in any additional <c>SetItemCategoryX</c>
        /// flags, are able to be matched.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool SetItemCategoryBigCraftable
        {
            get => (_setItemCategoryBigCraftable ?? false) || SetItemCategoryCraftable;
            set => _setItemCategoryBigCraftable = value;
        }

        /// <summary>
        /// Allow items of type <c>(BC)</c> and <c>(O)</c> that are craftable to be matched when parsing.
        /// When this flag is set, only items of this category, and those included in any additional
        /// <c>SetItemCategoryX</c> flags, are able to be matched.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool SetItemCategoryCraftable
        {
            get => _setItemCategoryCraftable ?? false;
            set => _setItemCategoryCraftable = value;
        }

        /// <summary>
        /// Allow items of type <c>(T)</c> to be matched when parsing. When this flag is set,
        /// only items of this category, and those included in any additional <c>SetItemCategoryX</c>
        /// flags, are able to be matched.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool SetItemCategoryTool
        {
            get => _setItemCategoryTool ?? false;
            set => _setItemCategoryTool = value;
        }

        /// <summary>Returns true if none of the <c>SetItemCategoryX</c> flags are set.</summary>
        public bool AllItemsEnabled
        {
            get => !(SetItemCategoryObject || SetItemCategoryBigCraftable || SetItemCategoryTool);
        }
    }
}
