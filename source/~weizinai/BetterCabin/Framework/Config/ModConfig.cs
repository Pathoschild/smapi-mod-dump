/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Config;

internal class ModConfig
{
    // 拜访小屋信息
    public bool VisitCabinInfo { get; set; } = true;

    // 小屋主人名字标签
    public bool CabinOwnerNameTag { get; set; } = true;
    public int NameTagXOffset { get; set; }
    public int NameTagYOffset { get; set; }
    public Color OnlineFarmerColor { get; set; } = Color.Black;
    public Color OfflineFarmerColor { get; set; } = Color.White;
    public Color OwnerColor { get; set; } = Color.Red;

    // 总在线时间
    public OnlineTimeConfig TotalOnlineTime { get; set; } = new(false, 0, -64, Color.Black);

    // 上次在线时间
    public OnlineTimeConfig LastOnlineTime { get; set; } = new(true, 0, 64, Color.Black);

    // 小屋面板
    public bool CabinMenu { get; set; } = true;
    public KeybindList CabinMenuKeybind { get; set; } = new(SButton.O);
    public bool BuildCabinContinually { get; set; } = true;
    
    // 上锁小屋
    public bool LockCabin { get; set; } = true;
    public KeybindList LockCabinKeybind { get; set; } = new(SButton.L);

    // 重置小屋
    public bool ResetCabinPlayer { get; set; } = true;
    public KeybindList ResetCabinPlayerKeybind { get; set; } = new(SButton.Delete);
}