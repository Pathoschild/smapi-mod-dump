/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System.Collections.Generic;

namespace LevelExtender {
    public class ModData
    {

        public bool WorldMonsters { get; set; }

        public bool drawBars { get; set; }

        public bool drawExtraItemNotifications { get; set; }

        public int minItemPriceForNotifications { get; set; }

        public List<string> skills { get; set; }

        public ModData()
        {

            this.WorldMonsters = false;

            this.drawBars = true;

            this.drawExtraItemNotifications = true;

            this.minItemPriceForNotifications = 50;

            this.skills = new List<string>();
        }
    }
}