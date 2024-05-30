/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickShop
**
*************************************************/

namespace QuickShop.Framework;

public record Shop(string Title, string ShopId, string OwnerName, Shop.ShopType Type)
{
    public enum ShopType
    {
        Normal,
        Festival,
    }
}