/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookcase.Events {
    public class CollectionsPageDrawEvent : Event {
        public CollectionsPage instance;
        public int currentTab;
        public int currentPage;
        public string hoverText;
        public Dictionary<int, List<List<ClickableTextureComponent>>> collections;

        public CollectionsPageDrawEvent(CollectionsPage instance, int currentTab, int currentPage, string hoverText, Dictionary<int, List<List<ClickableTextureComponent>>> collections) {
            this.instance = instance;
            this.currentTab = currentTab;
            this.currentPage = currentPage;
            this.hoverText = hoverText;
            this.collections = collections;
        }

        public override string ToString() {
            return $"Tab: {currentTab}\nPage: {currentPage}\nHoverText: {hoverText}";
        }
    }
}
