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

namespace SomeMultiplayerFeature.Framework;

public class ModMessage
{
    public readonly string PlayerName;
    public readonly string ShopId;
    public readonly bool IsExit;

    public ModMessage(string playerName, string shopId, bool isExit = false)
    {
        PlayerName = playerName;
        ShopId = shopId;
        IsExit = isExit;
    }

    public override string ToString()
    {
        return IsExit ? $"{PlayerName}离开了{ShopId}" : $"{PlayerName}访问了{ShopId}";
    }
}