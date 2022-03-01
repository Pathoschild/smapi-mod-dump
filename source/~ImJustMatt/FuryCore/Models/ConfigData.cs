/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Models;

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewMods.FuryCore.Enums;

/// <summary>
///     Mod config data.
/// </summary>
internal class ConfigData
{
    /// <summary>
    ///     Gets or sets controls to configure an item you're carrying.
    /// </summary>
    public KeybindList Configure { get; set; } = new(SButton.End);

    /// <summary>
    ///     Gets or sets which custom tags can be added to items.
    /// </summary>
    public HashSet<CustomTag> CustomTags { get; set; } = new()
    {
        CustomTag.CategoryArtifact,
        CustomTag.CategoryFurniture,
        CustomTag.DonateBundle,
        CustomTag.DonateMuseum,
    };

    /// <summary>
    ///     Gets or sets a value indicating whether scrolling will be added to menus where items overflow the menu capacity.
    /// </summary>
    public bool ScrollMenuOverflow { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to enable icons next to the toolbar.
    /// </summary>
    public bool ToolbarIcons { get; set; } = true;

    /// <summary>
    ///     Copies data from one <see cref="ConfigData" /> to another.
    /// </summary>
    /// <param name="other">The <see cref="ConfigData" /> to copy values to.</param>
    public void CopyTo(ConfigData other)
    {
        other.Configure = this.Configure;
        other.CustomTags = this.CustomTags;
        other.ScrollMenuOverflow = this.ScrollMenuOverflow;
        other.ToolbarIcons = this.ToolbarIcons;
    }
}