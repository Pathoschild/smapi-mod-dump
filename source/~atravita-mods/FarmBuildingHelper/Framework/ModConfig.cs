/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace FarmBuildingHelper.Framework;

/// <summary>
/// The config class for this mod.
/// </summary>
public sealed class ModConfig
{
    public KeybindList DownButton { get; set; } = KeybindList.ForSingle(SButton.S);

    public KeybindList UpButton { get; set; } = KeybindList.ForSingle(SButton.D);

    public KeybindList LeftButton { get; set; } = KeybindList.ForSingle(SButton.A);

    public KeybindList RightButton { get; set; } = KeybindList.ForSingle(SButton.W);

    public KeybindList PanLeftButton { get; set; } = KeybindList.ForSingle(SButton.PageUp);

    public KeybindList PanRightButton { get; set; } = KeybindList.ForSingle(SButton.PageDown);
}