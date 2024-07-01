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
/// Sources from which the sprite for a custom menu item can be taken.
/// </summary>
public enum SpriteSourceFormat
{
    /// <summary>
    /// Use the icon of an existing in-game item.
    /// </summary>
    ItemIcon,
    /// <summary>
    /// Draw an arbitrary area from any texture, tile sheet, etc.
    /// </summary>
    TextureSegment,
}
