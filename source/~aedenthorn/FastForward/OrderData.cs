/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

namespace FastForward
{
    internal class OrderData
    {
        public int dish;
        public string dishName;
        public bool loved;

        public OrderData(int dish, string dishName, bool loved)
        {
            this.dish = dish;
            this.dishName = dishName;
            this.loved = loved;
        }
    }
}