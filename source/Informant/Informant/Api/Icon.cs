/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Slothsoft.Informant.Api; 

/// <summary>
/// All information needed to display an icon somewhere.
/// </summary>
/// <param name="Texture">the texture to display.</param>
public record Icon(Texture2D Texture) {
    /// <summary>
    /// Create an icon for an Stardew Valley <see cref="SObject"/>. 
    /// </summary>
    public static Icon? ForObject(SObject obj, IPosition? position = null, Vector2? iconSize = null) {
        return ForParentSheetIndex(obj.ParentSheetIndex, obj.bigCraftable.Value, position, iconSize);
    }
    
    /// <summary>
    /// Create an icon for a parentSheetIndex and bigCraftable.
    /// </summary>
    public static Icon? ForParentSheetIndex(int parentSheetIndex, bool bigCraftable, IPosition? position = null, Vector2? iconSize = null) {
        position ??= IPosition.TopRight;
        iconSize ??= new Vector2(Game1.tileSize, Game1.tileSize);
        
        if (bigCraftable) {
            if (Game1.bigCraftableSpriteSheet == null) {
                return null;
            }
            return new Icon(Game1.bigCraftableSpriteSheet) {
                SourceRectangle = SObject.getSourceRectForBigCraftable(parentSheetIndex),
                Position = position,
                IconSize = iconSize,
            };
        }
        if (Game1.objectSpriteSheet == null) {
            return null;
        }
        return new Icon(Game1.objectSpriteSheet) {
            SourceRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, parentSheetIndex, 16, 16),
            Position = position,
            IconSize = iconSize,
        };
    }
    
    /// <summary>
    /// Optionally defines the source rectangle of the texture. Will be the entire <see cref="Texture"/> if not set. 
    /// </summary>
    public Rectangle? SourceRectangle { get; init; }
    /// <summary>
    /// Optionally defines the position of the icon. Will be <see cref="IPosition.TopLeft"/> if not set. 
    /// </summary>
    public IPosition? Position { get; init; }
    /// <summary>
    /// Optionally defines the size of the icon. Will be the <see cref="Texture"/>'s size if not set. 
    /// </summary>
    public Vector2? IconSize { get; init; }

    internal Rectangle NullSafeSourceRectangle => SourceRectangle ?? new Rectangle(0, 0, Texture.Width, Texture.Height);
    private IPosition NullSafePosition => Position ?? IPosition.TopLeft;
    private Vector2 NullSafeIconSize =>  IconSize ?? new Vector2(NullSafeSourceRectangle.Width, NullSafeSourceRectangle.Height);

    internal Rectangle CalculateIconPosition(Rectangle tooltipBounds) {
        return NullSafePosition.CalculateIconPosition(tooltipBounds, NullSafeIconSize);
    }
    
    internal Rectangle CalculateTooltipPosition(Rectangle tooltipBounds) {
        return NullSafePosition.CalculateTooltipPosition(tooltipBounds, NullSafeIconSize);
    }
}