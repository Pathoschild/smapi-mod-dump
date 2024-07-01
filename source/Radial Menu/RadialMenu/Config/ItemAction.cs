/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

namespace RadialMenu.Config;

/// <summary>
/// Types of actions that can be performed when activating an item from the menu.
/// </summary>
public enum ItemAction
{
    /// <summary>
    /// Always select the item, regardless of whether it can be consumed or used. Used for gifting
    /// items, placing them into machines, etc.
    /// </summary>
    Select,
    /// <summary>
    /// Try to consume or use the item, if it has that ability; otherwise, <see cref="Select"/> it.
    /// </summary>
    Use,
}
