/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace TMXLoader
{
    public class TileShopItem
    {
        public int index { get; set; } = -1;
        public string type { get; set; } = "Object";
        public string name { get; set; } = "none";
        public int stock { get; set; } = int.MaxValue;

        public int stack { get => stock; set => stock = value; }
        public int price { get; set; } = -1;
        public string conditions { get; set; } = "";
    }
}
