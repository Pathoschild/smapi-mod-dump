/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

namespace MapTK.TileActions
{
    internal class ShopItem
    {
        public int Index { get; set; } = -1;

        public string Name { get; set; } = "none";

        public string Type { get; set; } = "Object";

        public int Stock { get; set; } = int.MaxValue;

        public int Price { get; set; } = -1;

        public int ItemCurrency { get; set; } = -1;

        public int ItemAmount { get; set; } = 5;
    }
}
