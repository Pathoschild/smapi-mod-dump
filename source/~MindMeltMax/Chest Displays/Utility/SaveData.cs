/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace ChestDisplays.Utility
{
    [Obsolete("Replacing with ModData")]
    public class SaveData
    {
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQuality { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Location { get; set; }
        public int ItemType { get; set; }

        public SaveData() { }

        public SaveData(string item, string descript, int quality, int x, int y, string location, int itemType = -1)
        {
            Item = item;
            ItemDescription = descript;
            ItemQuality = quality;
            X = x;
            Y = y;
            Location = location;
            ItemType = itemType;
        }
    }

    public class ModData
    {
        public string ItemId { get; set; }

        [Obsolete("Convert to using QualifiedId's for items")]
        public string Item { get; set; }

        public int ItemQuality { get; set; }

        [Obsolete("Convert to using QualifiedId's for items")]
        public int ItemType { get; set; }

        public int UpgradeLevel { get; set; }

        public Color? Color { get; set; }
    }
}
