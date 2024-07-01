/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

// ReSharper disable MemberCanBePrivate.Global

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework.Message;

public class AccessShopInfoMessage
{
    public readonly string PlayerName;
    public readonly string ShopId;
    public readonly bool IsExit;

    public AccessShopInfoMessage(string playerName, string shopId, bool isExit = false)
    {
        this.PlayerName = playerName;
        this.ShopId = shopId;
        this.IsExit = isExit;
    }

    public override string ToString()
    {
        return this.IsExit ? I18n.UI_ExitShop(this.PlayerName, this.ShopId) : I18n.UI_AccessShop(this.PlayerName, this.ShopId);
    }
}