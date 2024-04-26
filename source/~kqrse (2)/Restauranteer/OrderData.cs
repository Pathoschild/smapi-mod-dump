/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

namespace Restauranteer
{
    internal class OrderData
    {
        public int dish;
        public string dishName;
        public int dishPrice;
        public bool loved;

        public OrderData(int dish, string dishName, int dishPrice, bool loved)
        {
            this.dish = dish;
            this.dishName = dishName;
            this.dishPrice = dishPrice;
            this.loved = loved;
        }
    }
}