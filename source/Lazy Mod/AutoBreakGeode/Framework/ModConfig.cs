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

namespace AutoBreakGeode.Framework;

public class ModConfig
{
    public KeybindList AutoBreakGeodeKey { get; set; } = new(SButton.F);
    public int BreakGeodeSpeed { get; set; } = 20;
}