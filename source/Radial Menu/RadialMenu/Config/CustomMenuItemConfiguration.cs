/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using StardewModdingAPI.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace RadialMenu.Config;

/// <summary>
/// Configuration for an item in the custom radial menu.
/// </summary>
public class CustomMenuItemConfiguration
{
    /// <summary>
    /// Short name for the item; displayed as the title when selected.
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// Item description, to display under the title when selected.
    /// </summary>
    public string Description { get; set; } = "";
    /// <summary>
    /// Key binding specifying the keys to simulate pressing when activated.
    /// </summary>
    public Keybind Keybind { get; set; } = new();
    /// <summary>
    /// Specifies the format of the <see cref="SpriteSourcePath"/>.
    /// </summary>
    public SpriteSourceFormat SpriteSourceFormat { get; set; }
    /// <summary>
    /// Path identifying which sprite to display for this item in the menu.
    /// </summary>
    /// <remarks>
    /// The format depends on the <see cref="SpriteSourceFormat"/> setting:
    /// <list type="bullet">
    /// <item>If the format is <see cref="SpriteSourceFormat.ItemIcon"/>, then this is the
    /// item's <see cref="StardewValley.Item.QualifiedItemId"/>, such as <c>O(128)</c>. If an
    /// unqualified ID is used, the menu will make an attempt anyway, but ID conflicts are
    /// possible and the wrong icon may get displayed.</item>
    /// <item>If the format is <see cref="SpriteSourceFormat.TextureSegment"/>, then this is a
    /// string in the form of <c>{assetPath}:(left,top,width,height)</c>. For example, to
    /// display the sprite for the Lucky Purple Shorts, use the string
    /// <c>maps/springobjects:(368,32,16,16)</c>.</item>
    /// </list>
    /// </remarks>
    public string SpriteSourcePath { get; set; } = "";
    /// <summary>
    /// Associates this item with a Generic Mod Config Menu key binding, which will cause it to
    /// automatically update the <see cref="Name"/>, <see cref="Description"/> and
    /// <see cref="Keybind"/>.
    /// </summary>
    public GmcmAssociation? Gmcm { get; set; }

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <remarks>
    /// Cloning is useful when loading configuration items into the GMCM UI, to avoid accidental
    /// edits to the real configuration due to hacky instant UI updates.
    /// </remarks>
    public CustomMenuItemConfiguration Clone()
    {
        return new()
        {
            Name = Name,
            Description = Description,
            Keybind = Keybind,
            SpriteSourceFormat = SpriteSourceFormat,
            SpriteSourcePath = SpriteSourcePath,
            Gmcm = Gmcm?.Clone(),
        };
    }

    /// <summary>
    /// Checks whether or not this item has any useful settings.
    /// </summary>
    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Name)
            && string.IsNullOrEmpty(Description)
            && !Keybind.IsBound
            && string.IsNullOrEmpty(SpriteSourcePath)
            && Gmcm is null;
    }

    /// <summary>
    /// Attempts to infer the custom asset path and bounds for the sprite, if one is configured.
    /// </summary>
    /// <param name="assetPath">Receives the parsed asset path, if successful.</param>
    /// <param name="sourceRect">Receives the parsed source rectangle, if successful.</param>
    /// <returns>
    /// True if <see cref="SpriteSourceFormat"/> is <see cref="SpriteSourceFormat.TextureSegment"/> and
    /// the <see cref="SpriteSourcePath"/> is a valid path:rect format. Otherwise, `null`.
    /// </returns>
    public bool TryGetTextureSegment([MaybeNullWhen(false)] out TextureSegmentPath parsedPath)
    {
        if (SpriteSourceFormat == SpriteSourceFormat.TextureSegment)
        {
            return TextureSegmentPath.TryParse(SpriteSourcePath, out parsedPath);
        }
        parsedPath = null;
        return false;
    }
}
