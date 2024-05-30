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

namespace SomeMultiplayerFeature.Framework;

public class ModConfig
{
    public bool ShowShopInfo { get; set; } = true;

    // public bool ShowModInfo { get; set; } = true;
    public KeybindList ShowModInfoKeybind { get; set; } = new(SButton.L);
    public KeybindList SetAllPlayerReadyKeybind { get; set; } = new(SButton.K);
}