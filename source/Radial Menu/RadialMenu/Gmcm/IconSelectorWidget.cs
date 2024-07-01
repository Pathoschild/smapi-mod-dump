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
using StardewModdingAPI;
using StardewValley;

namespace RadialMenu.Gmcm;

internal class IconSelectorWidget(ITranslationHelper translations)
{
    public event EventHandler<EventArgs>? SelectedItemChanged;

    private enum Control { PrevCategory, NextCategory, PrevImage, NextImage }

    private const double CURSOR_FOCUS_DURATION_MS = 120;
    private const float CURSOR_FOCUS_SCALE = 1.08f;
    private const int HORIZONTAL_PADDING = 16; // Between buttons, text, etc.
    private const int ROW_SPACING = 16;

    private static readonly Rectangle LeftArrowCursorRect = new(0, 256, 64, 64);
    private static readonly Rectangle RightArrowCursorRect = new(0, 192, 64, 64);

    public string SelectedItemId
    {
        get { return selectedItemId; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                selectedItemId = GetFirstItemForCategory(0);
            }
            else if (value != selectedItemId)
            {
                selectedItemId = value;
            }
            else
            {
                return;
            }
            SelectedCategoryId =
                ItemRegistry.GetMetadata(value)?.TypeIdentifier ?? CategoryIds.First();
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected static IEnumerable<string> CategoryIds =>
        ItemRegistry.ItemTypes.Select(t => t.Identifier);
    protected static SpriteFont Font => Game1.dialogueFont;

    protected string SelectedCategoryId
    {
        get { return selectedCategoryId; }
        set
        {
            if (value != selectedCategoryId)
            {
                selectedCategoryId = value;
                selectedCategoryName = GetCategoryName(value);
            }
        }
    }

    private readonly ITranslationHelper translations = translations;
    private readonly SpritePreviewWidget preview = new();
    private readonly ClickDetector clickDetector = new(new ClickRepeat(250, 50));

    private Control? clickedControl;
    private Control? hoveredControl;
    private Control? previousHoveredControl;
    private double? hoverAnimationStartTime;
    private int maxCategoryTextWidth = 0;
    private string selectedCategoryId = "";
    private string selectedCategoryName = "";
    private string selectedItemId = "";

    // Main reason to abstract this into a utility function is to handle the hover/click interaction
    // automatically instead of having to repeat it for every button.
    private void DrawCursorControl(
        SpriteBatch spriteBatch, Control control, Vector2 position, Rectangle sourceRect)
    {
        var inflation = 0.0f;

        var hoverRect = new Rectangle(position.ToPoint(), new(sourceRect.Width, sourceRect.Height));
        if (hoverRect.Contains(Game1.getMousePosition()))
        {
            if (previousHoveredControl != control)
            {
                hoverAnimationStartTime = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
            }
            hoveredControl = control;
            var animationProgress = GetAnimationProgress();
            inflation = animationProgress * (CURSOR_FOCUS_SCALE - 1);
            if (clickDetector.HasLeftClick())
            {
                clickedControl = control;
            }
        }

        var destinationRect = hoverRect;
        if (inflation > 0)
        {
            destinationRect.Inflate(
                inflation * destinationRect.Width,
                inflation * destinationRect.Height);
        }
        spriteBatch.Draw(Game1.mouseCursors, destinationRect, sourceRect, Color.White);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 startPosition)
    {
        // Reset this every frame; DrawCursorControl will assign if events are detected.
        previousHoveredControl = hoveredControl;
        hoveredControl = null;
        clickedControl = null;

        var currentPosition = startPosition;

        // Category selector
        var maxCategoryWidth = Math.Max(GetMaxCategoryTextWidth(), SpritePreviewWidget.Width);
        DrawCursorControl(spriteBatch, Control.PrevCategory, currentPosition, LeftArrowCursorRect);
        currentPosition.X += LeftArrowCursorRect.Width + HORIZONTAL_PADDING;
        var textHeight = Font.MeasureString("A").Y;
        var textTop = currentPosition.Y + LeftArrowCursorRect.Height / 2 - textHeight / 2;
        Utility.drawTextWithShadow(
            spriteBatch,
            selectedCategoryName,
            Font,
            new(currentPosition.X, textTop),
            Game1.textColor);
        currentPosition.X += maxCategoryWidth + HORIZONTAL_PADDING;
        DrawCursorControl(spriteBatch, Control.NextCategory, currentPosition, RightArrowCursorRect);

        // Icon selector/preview
        currentPosition.X = startPosition.X;
        currentPosition.Y += LeftArrowCursorRect.Height + ROW_SPACING;
        var previewArrowTop =
            currentPosition.Y + SpritePreviewWidget.Height / 2 - LeftArrowCursorRect.Height / 2;
        DrawCursorControl(
            spriteBatch,
            Control.PrevImage,
            new(currentPosition.X, previewArrowTop),
            LeftArrowCursorRect);
        currentPosition.X += LeftArrowCursorRect.Width + HORIZONTAL_PADDING;
        
        var itemImage = ItemRegistry.GetData(selectedItemId);
        if (itemImage is not null)
        {
            preview.Texture = itemImage.GetTexture();
            preview.SourceRect = itemImage.GetSourceRect();
            preview.Draw(spriteBatch, currentPosition.ToPoint());
        }
        currentPosition.X += SpritePreviewWidget.Width + HORIZONTAL_PADDING;
        DrawCursorControl(
            spriteBatch,
            Control.NextImage,
            new(currentPosition.X, previewArrowTop),
            RightArrowCursorRect);

        HandleClick();
    }

    public int GetHeight()
    {
        return LeftArrowCursorRect.Height + ROW_SPACING + SpritePreviewWidget.Height;
    }

    private float GetAnimationProgress()
    {
        var gameTime = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
        var elapsedTime = gameTime - hoverAnimationStartTime!.Value;
        return elapsedTime < CURSOR_FOCUS_DURATION_MS
            ? (float)(elapsedTime / CURSOR_FOCUS_DURATION_MS)
            : 1.0f;
    }

    private static int GetAvailableCategoryTextWidth()
    {
        // From SpecificModConfigMenu.cs
        var tableWidth = Math.Min(1200, Game1.uiViewport.Width - 200);
        return tableWidth / 2
            - LeftArrowCursorRect.Width
            - RightArrowCursorRect.Width
            - 2 * HORIZONTAL_PADDING;
    }

    private string GetCategoryName(string qualifier)
    {
        // Category names don't seem to be in the API anywhere and maybe not even in the game at
        // all, so we store our own in the translations.
        var id = qualifier.TrimStart('(').TrimEnd(')');
        return translations.Get($"itemcategory.{id}")
            .Default(translations.Get("itemcategory.unknown", new { id }));
    }

    private static string GetFirstItemForCategory(string categoryId)
    {
        int categoryIndex = CategoryIds
            .Select((id, index) => (id, index))
            .Where(x => x.id == categoryId)
            .Select(x => x.index)
            .FirstOrDefault();
        return GetFirstItemForCategory(categoryIndex);
    }

    private static string GetFirstItemForCategory(int categoryIndex)
    {
        var category = ItemRegistry.ItemTypes[categoryIndex];
        var nextItemId = category.GetAllIds().FirstOrDefault() ?? "";
        return ItemRegistry.ManuallyQualifyItemId(nextItemId, category.Identifier);
    }

    private int GetMaxCategoryTextWidth()
    {
        if (maxCategoryTextWidth == 0)
        {
            maxCategoryTextWidth = CategoryIds
                .Select(GetCategoryName)
                .Select(name => (int)MathF.Ceiling(Font.MeasureString(name).X))
                .Max();
        }
        return Math.Min(maxCategoryTextWidth, GetAvailableCategoryTextWidth());
    }

    private void HandleClick()
    {
        switch (clickedControl)
        {
            case Control.PrevCategory:
                MoveSelectedCategory(-1);
                break;
            case Control.NextCategory:
                MoveSelectedCategory(1);
                break;
            case Control.PrevImage:
                MoveSelectedImage(-1);
                break;
            case Control.NextImage:
                MoveSelectedImage(1);
                break;
        }
    }

    private void MoveSelectedCategory(int offset)
    {
        var categoryIds = CategoryIds.ToList();
        var categoryIndex = categoryIds.IndexOf(selectedCategoryId);
        var nextIndex = (categoryIndex + offset + categoryIds.Count) % categoryIds.Count;
        SelectedCategoryId = categoryIds[nextIndex];
        SelectedItemId = GetFirstItemForCategory(nextIndex);
    }

    private void MoveSelectedImage(int offset)
    {
        // Doing this by item could be way more inefficient than the same code for category, since
        // there can be a thousand items in a category. Whether or not it's really worth the extra
        // complexity to optimize is another question, since we only run this on user input.
        var selectedCategory = ItemRegistry.ItemTypes
            .Where(t => t.Identifier == SelectedCategoryId)
            .FirstOrDefault();
        if (selectedCategory is null)
        {
            return;
        }
        var allItemIds = selectedCategory
            .GetAllIds()
            .Select(id => ItemRegistry.ManuallyQualifyItemId(id, selectedCategory.Identifier))
            .ToList();
        var itemIndex = allItemIds.IndexOf(SelectedItemId);
        var nextIndex = (itemIndex + offset + allItemIds.Count) % allItemIds.Count;
        SelectedItemId = allItemIds[nextIndex];
    }
}
