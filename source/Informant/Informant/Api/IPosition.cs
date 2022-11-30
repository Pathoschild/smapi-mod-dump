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
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Slothsoft.Informant.Api; 

public interface IPosition {
    
    /// <summary>
    /// Displays the icon to the left of the text, aligned at the top.
    /// </summary>
    public static readonly IPosition TopLeft = new Position(Position.Leading, Position.Leading);
    /// <summary>
    /// Displays the icon to the left of the text, aligned at the center.
    /// </summary>
    public static readonly IPosition CenterLeft = new Position(Position.Leading, Position.Middle); 
    /// <summary>
    /// Displays the icon to the left of the text, aligned at the top.
    /// </summary>
    public static readonly IPosition BottomLeft = new Position(Position.Leading, Position.Trailing); 
    
    /// <summary>
    /// Displays the icon to the right of the text, aligned at the top.
    /// </summary>
    public static readonly IPosition TopRight = new Position(Position.Trailing, Position.Leading); 
    /// <summary>
    /// Displays the icon to the right of the text, aligned at the center.
    /// </summary>
    public static readonly IPosition CenterRight = new Position(Position.Trailing, Position.Middle); 
    /// <summary>
    /// Displays the icon to the right of the text, aligned at the bottom.
    /// </summary>
    public static readonly IPosition BottomRight = new Position(Position.Trailing, Position.Trailing); 
    
    /// <summary>
    /// Displays the icon at the top of the text, aligned at the center.
    /// </summary>
    public static readonly IPosition TopCenter = new Position(Position.Middle, Position.Leading); 
    /// <summary>
    /// Displays the icon centered below the text.
    /// </summary>
    public static readonly IPosition Center = new Position(Position.Middle, Position.Middle); 
    /// <summary>
    /// Displays the icon at the bottom of the text, aligned at the center.
    /// </summary>
    public static readonly IPosition BottomCenter = new Position(Position.Middle, Position.Trailing); 
    
    /// <summary>
    /// Displays the icon filled below the text.
    /// </summary>
    public static readonly IPosition Fill = new Position(Position.Fill, Position.Fill); 

    Rectangle CalculateIconPosition(Rectangle tooltipBounds, Vector2 iconSize);
    
    Rectangle CalculateTooltipPosition(Rectangle tooltipBounds, Vector2 iconSize);
    
    private class Position : IPosition {

        internal const int Leading = 0;
        internal const int Middle = 1;
        internal const int Trailing = 2;
        internal const int Fill = 3;

        private readonly int _horizontalPosition;
        private readonly int _verticalPosition;

        public Position(int horizontalPosition, int verticalPosition) {
            _horizontalPosition = horizontalPosition;
            _verticalPosition = verticalPosition;
        }

        public Rectangle CalculateIconPosition(Rectangle tooltipBounds, Vector2 iconSize) {
            var usedIconSizeX = _horizontalPosition == Fill ? tooltipBounds.Width : iconSize.X;
            var usedIconSizeY = _verticalPosition == Fill ? tooltipBounds.Height : iconSize.Y;
            return new Rectangle(
                CalculateIconPosition(_horizontalPosition, tooltipBounds.X, tooltipBounds.Width, iconSize.X),
                CalculateIconPosition(_verticalPosition, tooltipBounds.Y, tooltipBounds.Height, iconSize.Y),
                (int) usedIconSizeX,
                (int) usedIconSizeY
            );
        }
        
        private int CalculateIconPosition(int position, int tooltipPosition, int tooltipSize, float iconSize) {
            return position switch {
                Leading => tooltipPosition,
                Middle => tooltipPosition + (int) (tooltipSize - iconSize) / 2,
                Trailing => tooltipPosition + tooltipSize - (int) iconSize,
                _ => tooltipPosition
            };
        }

        public Rectangle CalculateTooltipPosition(Rectangle tooltipBounds, Vector2 iconSize) {
            return new Rectangle(
                CalculateTooltipPosition(_horizontalPosition is Leading, tooltipBounds.X, iconSize.X),
                CalculateTooltipPosition(_horizontalPosition is Middle && _verticalPosition is Leading, tooltipBounds.Y, iconSize.Y),
                CalculateTooltipSize(_horizontalPosition is Leading or Trailing, tooltipBounds.Width, iconSize.X),
                CalculateTooltipSize(_horizontalPosition is Middle && _verticalPosition is Leading or Trailing, tooltipBounds.Height, iconSize.Y)
            );
        }
        
        private static int CalculateTooltipPosition(bool expand, int tooltipPosition, float iconSize) {
            return expand ? tooltipPosition - (int) iconSize : tooltipPosition;
        }
        
        private static int CalculateTooltipSize(bool expand, int tooltipSize, float iconSize) {
            return expand ? tooltipSize + (int) iconSize : tooltipSize;
        }
    }
}