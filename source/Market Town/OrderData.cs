/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

namespace MarketTown
{
    internal class OrderData
    {
        public int dish;
        public string dishName;
        public int dishPrice;
        public string loved;

        public OrderData(int dish, string dishName, int dishPrice, string loved)
        {
            this.dish = dish;
            this.dishName = dishName;
            this.dishPrice = dishPrice;
            this.loved = loved;
        }
    }
}