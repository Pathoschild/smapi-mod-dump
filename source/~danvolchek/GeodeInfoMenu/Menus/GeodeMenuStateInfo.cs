/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace GeodeInfoMenu.Menus
{
    /// <summary>Represents the state of the geode menu.</summary>
    internal class GeodeMenuStateInfo
    {
        /***
         * Public Fields
         ***/

        /// <summary>The search text in the search box.</summary>
        public string searchText;

        /// <summary>The tab that is currently open.</summary>
        public int currentTab;

        /// <summary>The index of each scroll bar for each tab.</summary>
        public int[] scrollBarIndicies;

        /***
         * Public Methods
         ***/

        /// <summary>Construct an instance.</summary>
        /// <param name="text">Search box text</param>
        /// <param name="currentTab">Current open tab</param>
        /// <param name="indicies">Scroll bar locations</param>
        public GeodeMenuStateInfo(string text, int currentTab, int[] indicies)
        {
            this.currentTab = currentTab;
            this.searchText = text;
            this.scrollBarIndicies = indicies;
        }
    }
}
