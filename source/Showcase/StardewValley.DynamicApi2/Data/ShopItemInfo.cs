/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

namespace Igorious.StardewValley.DynamicApi2.Data
{
    public sealed class ShopItemInfo
    {
        public ShopItemInfo() { }

        public ShopItemInfo(int id)
        {
            ID = id;
        }

        public int ID { get; set; }
    }
}