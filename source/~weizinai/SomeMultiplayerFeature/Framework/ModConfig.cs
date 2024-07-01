/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

public class ModConfig
{
    // 访问商店信息
    public bool ShowAccessShopInfo { get; set; } = true;

    // 显示延迟玩家
    public bool ShowDelayedPlayer { get; set; } = true;
    public int ShowInterval { get; set; } = 30;

    // 自动设置IP连接
    public bool AutoSetIpConnection { get; set; } = true;
    public int EnableTime { get; set; } = 6;
    public int DisableTime { get; set; } = 20;
    public bool DisableWhenFestival { get; set; } = true;

    // 显示玩家数量
    public bool ShowPlayerCount { get; set; } = true;

    // 显示提示
    public bool ShowTip { get; set; } = true;
    public string TipText { get; set; } = "粉丝联机档QQ群：232127142";

    // 踢出未准备玩家
    public bool KickUnreadyPlayer { get; set; } = true;
    public KeybindList KickUnreadyPlayerKey { get; set; } = new(SButton.F3);

    // 版本限制
    public bool VersionLimit { get; set; } = true;
}