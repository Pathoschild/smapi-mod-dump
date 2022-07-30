/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;

namespace Chest_Displays.Utility
{
    public class SaveData
    {
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public int ItemQuality { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Location { get; set; }

        public SaveData() { }

        public SaveData(string item, string descript, int quality, int x, int y, string location)
        {
            Item = item;
            ItemDescription = descript;
            ItemQuality = quality;
            X = x;
            Y = y;
            Location = location;
        }
    }
}
