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
/// The types of actions that can be delayed when selecting from the radial menu.
/// </summary>
public enum DelayedActions
{
    /// <summary>
    /// Delay for all items activated via the menu.
    /// </summary>
    All,
    /// <summary>
    /// Only delay when switching tools, which has no in-game animation.
    /// </summary>
    ToolSwitch,
}
