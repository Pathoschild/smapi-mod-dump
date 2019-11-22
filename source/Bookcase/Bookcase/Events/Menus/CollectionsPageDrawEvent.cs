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
