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

namespace weizinai.StardewValleyMod.AutoBreakGeode.Framework;

internal class ModConfig
{
    public KeybindList OpenConfigMenuKeybind { get; set; } = new(SButton.None);
    public KeybindList AutoBreakGeodeKeybind { get; set; } = new(SButton.F);
    public bool DrawBeginButton { get; set; } = true;
    public int BreakGeodeSpeed { get; set; } = 20;
}