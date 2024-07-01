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

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class ModConfig
{
    // 旁观者模式
    public KeybindList SpectateLocationKey { get; set; } = new(SButton.F6);
    public KeybindList SpectatePlayerKey { get; set; } = new(SButton.F7);
    public KeybindList ToggleStateKey { get; set; } = new(SButton.K);
    public int MoveSpeed { get; set; } = 32;
    public int MoveThreshold { get; set; } = 64;

    // 轮播玩家
    public KeybindList RotatePlayerKey { get; set; } = new(SButton.F8);
    public int RotationInterval { get; set; } = 30;
}