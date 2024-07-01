/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadialMenu.Config;
using StardewValley;
using TileRectangle = xTile.Dimensions.Rectangle;

namespace RadialMenu;

internal class Painter
{
    private record SelectionState(int ItemCount, int PreviousIndex, int SelectedIndex);

    private const float CIRCLE_MAX_ERROR = 0.1f;
    private const float EQUILATERAL_ANGLE = MathF.PI * 2 / 3;
    private const float MENU_SPRITE_MAX_WIDTH_RATIO = 0.8f;
    private const float TWO_PI = MathF.PI * 2;

    private static readonly float ROOT_3 = MathF.Sqrt(3);

    private static readonly TextureSegment UnknownSprite =
        new(Game1.mouseCursors, /* Question Mark */ new(176, 425, 9, 12));

    public IReadOnlyList<MenuItem> Items { get; set; } = [];
    public Styles Styles => getStyles();

    private readonly GraphicsDevice graphicsDevice;
    private readonly BasicEffect effect;
    private readonly Func<Styles> getStyles;

    private VertexPositionColor[] innerVertices = [];
    private VertexPositionColor[] outerVertices = [];
    private float selectionBlend = 1.0f;
    private SelectionState selectionState =
        new(/* ItemCount= */ 0, /* FocusedIndex= */ 0, /* SelectedIndex= */ 0);

    public Painter(GraphicsDevice graphicsDevice, Func<Styles> getStyles)
    {
        this.graphicsDevice = graphicsDevice;
        effect = new BasicEffect(graphicsDevice)
        {
            World = Matrix.Identity,
            View = Matrix.CreateLookAt(Vector3.Forward, Vector3.Zero, Vector3.Down),
            VertexColorEnabled = true,
        };
        this.getStyles = getStyles;
    }

    public void Paint(
        SpriteBatch spriteBatch,
        TileRectangle viewport,
        int previousIndex,
        int selectedIndex,
        float? selectionAngle,
        float selectionBlend = 1.0f)
    {
        GenerateVertices();
        var selectionState = new SelectionState(Items.Count, previousIndex, selectedIndex);
        if (selectionState != this.selectionState || selectionBlend != this.selectionBlend)
        {
            this.selectionState = selectionState;
            this.selectionBlend = selectionBlend;
            UpdateVertexColors();
        }
        PaintBackgrounds(viewport, selectionAngle);
        PaintItems(spriteBatch, viewport);
        PaintSelectionDetails(spriteBatch, viewport);
    }

    private void PaintBackgrounds(TileRectangle viewport, float? selectionAngle)
    {
        effect.Projection = Matrix.CreateOrthographic(viewport.Width, viewport.Height, 0, 1);
        var oldRasterizerState = graphicsDevice.RasterizerState;
        // Unsure why this doesn't seem to have any effect. Keeping it here in case we figure it
        // out, because the circle looks jagged without it.
        graphicsDevice.RasterizerState = new RasterizerState
        {
            MultiSampleAntiAlias = true
        };
        try
        {
            // Cursor is just 1 triangle, so we can compute this on every frame.
            var cursorVertices = selectionAngle != null
                ? GenerateCursorVertices(
                    Styles.InnerRadius - Styles.CursorDistance,
                    selectionAngle.Value)
                : [];
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList, innerVertices, 0, innerVertices.Length / 3);
                graphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList, outerVertices, 0, outerVertices.Length / 3);
                if (cursorVertices.Length > 0)
                {
                    graphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleList,
                        cursorVertices,
                        0,
                        cursorVertices.Length / 3);
                }
            }
        }
        finally
        {
            graphicsDevice.RasterizerState = oldRasterizerState;
        }
    }

    private void PaintItems(SpriteBatch spriteBatch, TileRectangle viewport)
    {
        var centerX = viewport.Width / 2.0f;
        var centerY = viewport.Height / 2.0f;
        var itemRadius = Styles.InnerRadius + Styles.GapWidth + Styles.OuterRadius / 2.0f;
        var angleBetweenItems = TWO_PI / Items.Count;
        var t = 0.0f;
        foreach (var item in Items)
        {
            var itemPoint = GetCirclePoint(itemRadius, t);
            var displaySize = GetScaledSize(item, Styles.MenuSpriteHeight);
            // Aspect ratio is usually almost square, or has extra height (e.g. big craftables).
            // In case of a horizontal aspect ratio, shrink the size so that it still fits.
            var maxWidth = Styles.OuterRadius * MENU_SPRITE_MAX_WIDTH_RATIO;
            if (displaySize.X > maxWidth)
            {
                var scale = maxWidth / displaySize.X;
                displaySize = new(
                    (int)MathF.Round(displaySize.X * scale),
                    (int)MathF.Round(displaySize.Y * scale));
            }
            var sourceSize = GetSpriteSize(item, out var isMonogram);
            var aspectRatio = (float)sourceSize.X / sourceSize.Y;
            // Sprites draw from top left rather than center; we have to adjust for it.
            var itemPoint2d = new Vector2(
                centerX + itemPoint.X - displaySize.X / 2.0f,
                centerY + itemPoint.Y - displaySize.Y / 2.0f);
            var destinationRect = new Rectangle(itemPoint2d.ToPoint(), displaySize);
            if (isMonogram)
            {
                Monogram.Draw(
                    spriteBatch,
                    destinationRect,
                    item.Title,
                    Color.White);
            }
            else
            {
                var shadowTexture = Game1.shadowTexture;
                spriteBatch.Draw(
                    shadowTexture,
                    destinationRect.Location.ToVector2() + new Vector2(32f, 52f),
                    shadowTexture.Bounds,
                    new Color(Color.Gray, 0.5f),
                    0.0f,
                    new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                    3f,
                    SpriteEffects.None,
                    -0.0001f);
                // Tinting may be an overlay sprite, or a tint of the original sprite. In here, we
                // determine that by the nullness of both the rectangle and color. If a tint color
                // is specified, but no separate rectangle, then it means we need to tint the base
                // sprite since no overlay will be drawn; otherwise, draw the base normally.
                var baseColor = item.TintRectangle is null
                    ? (item.TintColor ?? Color.White)
                    : Color.White;
                spriteBatch.Draw(item.Texture, destinationRect, item.SourceRectangle, baseColor);
                if (item.TintRectangle is Rectangle tintRect && item.TintColor is Color tintColor)
                {
                    spriteBatch.Draw(item.Texture, destinationRect, tintRect, tintColor);
                }
            }
            if (item.Quality is int quality && quality > 0)
            {
                // From StardewValley:Object.cs
                var qualitySourceRect = quality < 4
                    ? new Rectangle(338 + (quality - 1) * 8, 400, 8, 8)
                    : new Rectangle(346, 392, 8, 8);
                var qualityIconPos = new Vector2(
                    destinationRect.Left,
                    destinationRect.Bottom - 16);
                spriteBatch.Draw(
                    Game1.mouseCursors,
                    qualityIconPos,
                    qualitySourceRect,
                    Color.White,
                    /* rotation= */ 0,
                    Vector2.Zero,
                    /* scale= */ 3.0f,
                    SpriteEffects.None,
                    /* layerDepth= */ 0.1f);
            }
            if (item.StackSize is int stackSize)
            {
                var stackTextScale = 3.0f;
                var stackTextWidth =
                    Utility.getWidthOfTinyDigitString(stackSize, stackTextScale);
                var stackLabelPos = new Vector2(
                    destinationRect.Right - stackTextWidth,
                    destinationRect.Bottom - 8);
                Utility.drawTinyDigits(
                    stackSize,
                    spriteBatch,
                    stackLabelPos,
                    stackTextScale,
                    /* layerDepth= */ 0.1f,
                    Styles.StackSizeColor);
            }
            t += angleBetweenItems;
        }
    }

    private void PaintSelectionDetails(
        SpriteBatch spriteBatch, TileRectangle viewport)
    {
        if (selectionState.SelectedIndex < 0)
        {
            return;
        }
        var item = Items.Count > selectionState.SelectedIndex
            ? Items[selectionState.SelectedIndex]
            : null;
        if (item is null)
        {
            return;
        }

        var centerX = viewport.Width / 2.0f;
        var centerY = viewport.Height / 2.0f;
        var itemDrawSize = GetScaledSize(item, Styles.SelectionSpriteHeight);
        var itemPos =
            new Vector2(centerX - itemDrawSize.X / 2, centerY - itemDrawSize.Y - 24);
        var itemRect = new Rectangle(itemPos.ToPoint(), itemDrawSize);
        var baseColor = item.TintRectangle is null
                    ? (item.TintColor ?? Color.White)
                    : Color.White;
        spriteBatch.Draw(item.Texture, itemRect, item.SourceRectangle, baseColor);
        if (item.TintRectangle is Rectangle tintRect && item.TintColor is Color tintColor)
        {
            spriteBatch.Draw(item.Texture, itemRect, tintRect, tintColor);
        }

        var labelFont = Game1.dialogueFont;
        var labelSize = labelFont.MeasureString(item.Title);
        var labelPos = new Vector2(centerX - labelSize.X / 2.0f, centerY);
        spriteBatch.DrawString(labelFont, item.Title, labelPos, Styles.SelectionTitleColor);

        var descriptionFont = Game1.smallFont;
        var descriptionText = item.Description;
        var descriptionY = labelPos.Y + labelFont.LineSpacing + 16.0f;
        var descriptionLines = Game1.parseText(descriptionText, descriptionFont, 400)
            .Split(Environment.NewLine);
        foreach (var descriptionLine in descriptionLines)
        {
            var descriptionSize = descriptionFont.MeasureString(descriptionLine);
            var descriptionPos = new Vector2(centerX - descriptionSize.X / 2.0f, descriptionY);
            descriptionY += descriptionFont.LineSpacing;
            spriteBatch.DrawString(
                descriptionFont,
                descriptionLine,
                descriptionPos,
                Styles.SelectionDescriptionColor);
        }
    }

    private void GenerateVertices()
    {
        if (innerVertices.Length == 0)
        {
            innerVertices =
                GenerateCircleVertices(Styles.InnerRadius, Styles.InnerBackgroundColor);
        }
        if (outerVertices.Length == 0)
        {
            outerVertices = GenerateDonutVertices(
                Styles.InnerRadius + Styles.GapWidth,
                Styles.OuterRadius,
                Styles.OuterBackgroundColor);
        }
    }

    private static (float start, float end) GetSegmentRange(
        int selectedIndex,
        int itemCount,
        int segmentCount)
    {
        if (selectedIndex < 0)
        {
            return (-1.0f, -0.5f);
        }
        var sliceSize = (float)segmentCount / itemCount;
        var relativePosition = (float)selectedIndex / itemCount;
        var end = (relativePosition * segmentCount + sliceSize / 2) % segmentCount;
        var start = (end - sliceSize + segmentCount) % segmentCount;
        return (start, end);
    }

    private void UpdateVertexColors()
    {
        var (itemCount, previousIndex, selectedIndex) = selectionState;
        const int outerChordSize = 6;
        var segmentCount = outerVertices.Length / outerChordSize;
        var (previousHighlightStartSegment, previousHighlightEndSegment) =
            GetSegmentRange(previousIndex, itemCount, segmentCount);
        var (currentHighlightStartSegment, currentHighlightEndSegment) =
            GetSegmentRange(selectedIndex, itemCount, segmentCount);
        for (var i = 0; i < segmentCount; i++)
        {
            var isCurrentHighlight = currentHighlightStartSegment < currentHighlightEndSegment
                ? (i >= currentHighlightStartSegment && i < currentHighlightEndSegment)
                : (i >= currentHighlightStartSegment || i < currentHighlightEndSegment);
            var isPreviousHighlight = isCurrentHighlight
                ? false
                : previousHighlightStartSegment < previousHighlightEndSegment
                    ? (i >= previousHighlightStartSegment && i < previousHighlightEndSegment)
                    : (i >= previousHighlightStartSegment || i < previousHighlightEndSegment);
            var outerIndex = i * outerChordSize;
            var outerColor = isCurrentHighlight
                ? Color.Lerp(Styles.OuterBackgroundColor, Styles.HighlightColor, selectionBlend)
                : isPreviousHighlight
                    ? Styles.SelectionColor
                    : Styles.OuterBackgroundColor;
            for (var j = 0; j < outerChordSize; j++)
            {
                outerVertices[outerIndex + j].Color = outerColor;
            }
        }
    }

    private static VertexPositionColor[] GenerateCircleVertices(float radius, Color color)
    {
        var vertexCount = GetOptimalVertexCount(radius);
        var step = TWO_PI / vertexCount;
        var t = 0.0f;
        var vertices = new VertexPositionColor[vertexCount * 3];
        var vertexIndex = 0;
        var prevPoint = GetCirclePoint(radius, 0);
        // Note: We loop using a fixed number of vertices, instead of a max angle, in case of
        // floating point rounding error.
        for (var i = 0; i < vertexCount; i++)
        {
            t += step;
            var nextPoint = GetCirclePoint(radius, t);
            vertices[vertexIndex++] = new VertexPositionColor(prevPoint, color);
            vertices[vertexIndex++] = new VertexPositionColor(nextPoint, color);
            vertices[vertexIndex++] = new VertexPositionColor(Vector3.Zero, color);
            prevPoint = nextPoint;
        }
        return vertices;
    }

    private static VertexPositionColor[] GenerateDonutVertices(
        float innerRadius, float thickness, Color color)
    {
        var outerRadius = innerRadius + thickness;
        var vertexCount = GetOptimalVertexCount(outerRadius);
        var step = TWO_PI / vertexCount;
        var t = 0.0f;
        var vertices = new VertexPositionColor[vertexCount * 6];
        var vertexIndex = 0;
        var prevInnerPoint = GetCirclePoint(innerRadius, 0);
        var prevOuterPoint = GetCirclePoint(outerRadius, 0);
        // Note: We loop using a fixed number of vertices, instead of a max angle, in case of
        // floating point rounding error.
        for (var i = 0; i < vertexCount; i++)
        {
            t += step;
            var nextInnerPoint = GetCirclePoint(innerRadius, t);
            var nextOuterPoint = GetCirclePoint(outerRadius, t);
            vertices[vertexIndex++] = new VertexPositionColor(prevOuterPoint, color);
            vertices[vertexIndex++] = new VertexPositionColor(nextOuterPoint, color);
            vertices[vertexIndex++] = new VertexPositionColor(nextInnerPoint, color);
            vertices[vertexIndex++] = new VertexPositionColor(nextInnerPoint, color);
            vertices[vertexIndex++] = new VertexPositionColor(prevInnerPoint, color);
            vertices[vertexIndex++] = new VertexPositionColor(prevOuterPoint, color);
            prevInnerPoint = nextInnerPoint;
            prevOuterPoint = nextOuterPoint;
        }
        return vertices;
    }

    private VertexPositionColor[] GenerateCursorVertices(float tipRadius, float angle)
    {
        var center = GetCirclePoint(tipRadius - Styles.CursorSize / 2, angle);
        // Compute the points for an origin-centered triangle, then offset.
        var radius = Styles.CursorSize / ROOT_3;
        var p1 = center + radius * new Vector3(MathF.Sin(angle), -MathF.Cos(angle), 0);
        var angle2 = angle + EQUILATERAL_ANGLE;
        var p2 = center + radius * new Vector3(MathF.Sin(angle2), -MathF.Cos(angle2), 0);
        var angle3 = angle2 + EQUILATERAL_ANGLE;
        var p3 = center + radius * new Vector3(MathF.Sin(angle3), -MathF.Cos(angle3), 0);
        return
        [
            new VertexPositionColor(p1, Styles.CursorColor),
            new VertexPositionColor(p2, Styles.CursorColor),
            new VertexPositionColor(p3, Styles.CursorColor),
        ];
    }

    private static Vector3 GetCirclePoint(float radius, float angle)
    {
        var x = radius * MathF.Sin(angle);
        var y = radius * -MathF.Cos(angle);
        return new Vector3(x, y, 0);
    }

    private static int GetOptimalVertexCount(float radius)
    {
        var optimalAngle = Math.Acos(1 - CIRCLE_MAX_ERROR / radius);
        return (int) Math.Ceiling(TWO_PI / optimalAngle);
    }

    private static Point GetScaledSize(MenuItem item, int height)
    {
        var sourceSize = GetSpriteSize(item, out _);
        var aspectRatio = (float)sourceSize.X / sourceSize.Y;
        var width = (int)MathF.Round(height * aspectRatio);
        return new(width, height);
    }

    private static Point GetSpriteSize(MenuItem item, out bool isMonogram)
    {
        if (item.Texture is null)
        {
            var monogramSize = Monogram.Measure(item.Title)?.ToPoint();
            isMonogram = monogramSize.HasValue;
            return monogramSize ?? UnknownSprite.SourceRect!.Value.Size;
        }
        isMonogram = false;
        return item.SourceRectangle?.Size ?? new Point(item.Texture.Width, item.Texture.Height);
    }
}
