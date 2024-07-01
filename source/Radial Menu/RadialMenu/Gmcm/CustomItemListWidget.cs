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

namespace RadialMenu.Gmcm;

internal class CustomItemListWidget(TextureHelper textureHelper)
{
    public event EventHandler<EventArgs>? SelectedIndexChanged;
    public event EventHandler<EventArgs>? SelectedIndexChanging;

    private record ItemLayout(Texture2D Texture, Rectangle? SourceRect, Rectangle DestinationRect);

    private const int ITEM_HEIGHT = 64;
    private const int ITEM_VERTICAL_SPACING = 32;
    private const int MARGIN_BOTTOM = 16;
    private const float MAX_ANIMATION_SCALE = 1.1f;
    private const int MAX_COLUMNS = 6;
    private const int SELECTION_PADDING = 12;
    private const int VERTICAL_OFFSET = 16;

    public IReadOnlyList<CustomMenuItemConfiguration> Items => items;
    public int SelectedIndex { get; private set; } = 0;
    public CustomMenuItemConfiguration SelectedItem => items[SelectedIndex];
    public IEnumerable<CustomMenuItemConfiguration> VisibleItems => items.Take(itemCount);

    private readonly ClickDetector clickDetector = new();
    private readonly List<CustomMenuItemConfiguration> items = [];
    // Item count is tracked separately from customItems.Count, so we can "remove" items without
    // losing their data. This way, if the player lowers the count and raises it again, the old
    // items are still there and don't have to be tediously set up all over again.
    private int itemCount;

    private (int, double) animatedItemIndexAndStartTime = (-1, 0);

    public void Draw(SpriteBatch spriteBatch, Vector2 startPosition)
    {
        var mousePos = Game1.getMousePosition();
        var labelHeight = (int)Game1.dialogueFont.MeasureString("A").Y;
        startPosition.Y += labelHeight + VERTICAL_OFFSET;
        // From SpecificModConfigMenu.cs
        var tableWidth = Math.Min(1200, Game1.uiViewport.Width - 200);
        startPosition.X = (Game1.uiViewport.Width - tableWidth) / 2;
        var maxItemWidth = tableWidth / MAX_COLUMNS;
        var position = startPosition;
        int col = 0;
        bool hadMouseOver = false;
        for (int i = 0; i < itemCount; i++)
        {
            if (col == MAX_COLUMNS)
            {
                col = 0;
                position.X = startPosition.X;
                position.Y += ITEM_HEIGHT + ITEM_VERTICAL_SPACING;
            }
            var item = items[i];
            var centerX = (int)position.X + maxItemWidth / 2;
            var (texture, sourceRect, destinationRect) =
                LayoutItem(item, centerX, position.Y, maxItemWidth);
            var imageDestinationRect = destinationRect;
            var hoverTestRect = destinationRect;
            if (hoverTestRect.Width < ITEM_HEIGHT)
            {
                hoverTestRect.Inflate(ITEM_HEIGHT - hoverTestRect.Width, 0);
            }
            if (hoverTestRect.Contains(mousePos))
            {
                var animationProgress = GetAnimationProgress(i);
                var inflationScale = animationProgress * (MAX_ANIMATION_SCALE - 1.0f);
                imageDestinationRect.Inflate(
                    imageDestinationRect.Width * inflationScale,
                    imageDestinationRect.Height * inflationScale);
                hadMouseOver = true;
                if (clickDetector.HasLeftClick() && i != SelectedIndex)
                {
                    SelectedIndexChanging?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = i;
                    // TODO: Play selection sound
                    SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            spriteBatch.Draw(texture, imageDestinationRect, sourceRect, Color.White);
            if (i == SelectedIndex)
            {
                var selectionHeight =
                    (int)(ITEM_HEIGHT * MAX_ANIMATION_SCALE + SELECTION_PADDING * 2);
                var selectionWidth = (int)(Math.Max(destinationRect.Width, ITEM_HEIGHT)
                    * MAX_ANIMATION_SCALE
                    + SELECTION_PADDING * 2);
                var centerY = (int)position.Y + ITEM_HEIGHT / 2;
                var borderDestinationRect = new Rectangle(
                    centerX - selectionWidth / 2,
                    centerY - selectionHeight / 2,
                    selectionWidth,
                    selectionHeight);
                var borderSourceRect = new Rectangle(64, 192, 64, 64); // Orange-red border
                spriteBatch.Draw(
                    Game1.mouseCursors, borderDestinationRect, borderSourceRect, Color.White);
            }
            col++;
            position.X += maxItemWidth;
        }
        // If the mouse wasn't over any item in this frame, then it means whatever was
        // previously being animated should be reset, otherwise it may "pop" to a random scale
        // if the player returns to that previous item.
        if (!hadMouseOver)
        {
            animatedItemIndexAndStartTime = (-1, 0);
        }
    }

    public int GetHeight()
    {
        var labelHeight = (int)Game1.dialogueFont.MeasureString("A").Y;
        var rowCount = (int)MathF.Ceiling((float)itemCount / MAX_COLUMNS);
        return labelHeight
            + VERTICAL_OFFSET
            + ITEM_HEIGHT * rowCount + ITEM_VERTICAL_SPACING * (rowCount - 1)
            + MARGIN_BOTTOM;
    }

    public void Load(IEnumerable<CustomMenuItemConfiguration> items)
    {
        this.items.Clear();
        this.items.AddRange(items);
        itemCount = this.items.Count;
        if (itemCount == 0)
        {
            SetCount(1);
        }
        SelectedIndex = 0;
    }

    // We don't have a Save method because it's not a very useful API compared to having callers
    // reference the actual Items and copy it.
    //
    // Since the widget is part of a "real-time" editor, rather than trying to sync with a master
    // configuration list (which isn't supposed to happen until the user saves anyway), the owner
    // of this widget can simply use its Items/SelectedItem as the main data source for subsequent
    // editing UI.

    public void SetCount(int count)
    {
        itemCount = count;
        while (items.Count < itemCount)
        {
            items.Add(new());
        }
        if (SelectedIndex >= count)
        {
            SelectedIndex = count - 1;
        }
    }

    /* ----- Layout and Drawing ----- */

    private float GetAnimationProgress(int index)
    {
        var gameTime = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
        var (previousIndex, startTime) = animatedItemIndexAndStartTime;
        if (index != previousIndex)
        {
            startTime = gameTime;
            animatedItemIndexAndStartTime = (index, startTime);
        }
        return (float)(Math.Sin((gameTime - startTime) * Math.PI / 512) + 1.0f) / 2.0f;
    }

    private ItemLayout LayoutItem(
        CustomMenuItemConfiguration item,
        float centerX,
        float topY,
        float maxItemWidth)
    {
        var sprite = textureHelper.GetSprite(item.SpriteSourceFormat, item.SpriteSourcePath)
            ?? new(Game1.mouseCursors, /* Question Mark */ new(176, 425, 9, 12));
        var sourceSize = sprite.SourceRect?.Size ?? sprite.Texture.Bounds.Size;
        var aspectRatio = sourceSize.X / (float)sourceSize.Y;
        var itemWidth = Math.Min(aspectRatio * ITEM_HEIGHT, maxItemWidth);
        var destinationRect = new Rectangle(
            (int)MathF.Round(centerX - itemWidth / 2), (int)topY, (int)itemWidth, ITEM_HEIGHT);
        return new(sprite.Texture, sprite.SourceRect, destinationRect);
    }
}
