/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.UI.Menus;

using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.FauxCore.Common.Helpers;
using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;
using StardewValley.Menus;

#else
namespace StardewMods.Common.UI.Menus;

using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewMods.Common.Helpers;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewValley.Menus;
#endif

/// <summary>Menu for selecting an icon.</summary>
internal class SelectIcon : FramedMenu
{
    private readonly IEnumerable<IIcon> allIcons;
    private readonly int columns;
    private readonly GetHoverText getHoverText;
    private readonly List<Highlight> highlights = [];
    private readonly int length;
    private readonly List<Operation> operations = [];
    private readonly int spacing;

    private List<ClickableTextureComponent>? components;
    private int currentIndex = -1;
    private List<IIcon>? icons;
    private EventHandler<IIcon?>? selectionChanged;

    /// <summary>Initializes a new instance of the <see cref="SelectIcon" /> class.</summary>
    /// <param name="allIcons">The icons to pick from.</param>
    /// <param name="rows">This rows of icons to display.</param>
    /// <param name="columns">The columns of icons to display.</param>
    /// <param name="getHoverText">A function which returns the hover text for an icon.</param>
    /// <param name="scale">The icon scale.</param>
    /// <param name="spacing">The spacing between icons.</param>
    /// <param name="x">The x-position.</param>
    /// <param name="y">The y-position.</param>
    public SelectIcon(
        IEnumerable<IIcon> allIcons,
        int rows,
        int columns,
        GetHoverText? getHoverText = null,
        float scale = Game1.pixelZoom,
        int spacing = 16,
        int? x = null,
        int? y = null)
        : base(
            x,
            y,
            (columns * ((int)(scale * 16) + spacing)) + spacing,
            (rows * ((int)(scale * 16) + spacing)) + spacing)
    {
        this.allIcons = allIcons;
        this.columns = columns;
        this.getHoverText = getHoverText ?? SelectIcon.GetUniqueId;
        this.spacing = spacing;
        this.length = (int)Math.Floor(scale * 16);
    }

    /// <summary>Get the hover text for an icon.</summary>
    /// <param name="icon">The icon.</param>
    /// <returns>The hover text.</returns>
    public delegate string GetHoverText(IIcon icon);

    /// <summary>Highlight an icon.</summary>
    /// <param name="icon">The icon to highlight.</param>
    /// <returns><c>true</c> if the icon should be highlighted; otherwise, <c>false</c>.</returns>
    public delegate bool Highlight(IIcon icon);

    /// <summary>An operation to perform on the icons.</summary>
    /// <param name="icons">The original icons.</param>
    /// <returns>Returns a modified list of icons.</returns>
    public delegate IEnumerable<IIcon> Operation(IEnumerable<IIcon> icons);

    /// <summary>Event raised when the selection changes.</summary>
    public event EventHandler<IIcon?> SelectionChanged
    {
        add => this.selectionChanged += value;
        remove => this.selectionChanged -= value;
    }

    /// <summary>Gets the currently selected icon.</summary>
    public IIcon? CurrentSelection => this.Icons.ElementAtOrDefault(this.CurrentIndex);

    /// <inheritdoc />
    public override Rectangle Frame =>
        new(this.xPositionOnScreen + 4, this.yPositionOnScreen + 4, this.width - 8, this.height - 8);

    /// <summary>Gets the icons.</summary>
    public virtual List<IIcon> Icons =>
        this.icons ??= this.operations.Aggregate(this.allIcons, (current, operation) => operation(current)).ToList();

    /// <inheritdoc />
    public override int StepSize => 32;

    /// <summary>Gets or sets the current index.</summary>
    public int CurrentIndex
    {
        get => this.currentIndex;
        protected set
        {
            this.currentIndex = value;
            this.selectionChanged?.InvokeAll(this, this.CurrentSelection);
        }
    }

    private IEnumerable<ClickableTextureComponent> Components
    {
        get
        {
            if (this.components is not null)
            {
                return this.components;
            }

            this.components = this
                .Icons.Select(
                    (icon, index) =>
                    {
                        var component = icon.Component(IconStyle.Transparent);

                        component.baseScale = component.scale = (float)Math.Floor(
                            (float)this.length / Math.Max(component.sourceRect.Width, component.sourceRect.Height));

                        component.bounds = new Rectangle(
                            this.xPositionOnScreen
                            + (index % this.columns * (this.length + this.spacing))
                            + (int)((this.length - (component.sourceRect.Width * component.scale)) / 2f)
                            + this.spacing,
                            this.yPositionOnScreen
                            + (index / this.columns * (this.length + this.spacing))
                            + (int)((this.length - (component.sourceRect.Height * component.scale)) / 2f)
                            + this.spacing,
                            this.length,
                            this.length);

                        component.hoverText = this.getHoverText(icon);
                        component.name = index.ToString(CultureInfo.InvariantCulture);

                        return component;
                    })
                .ToList();

            var yOffset = this.components.Last().bounds.Bottom - this.yPositionOnScreen - this.height + this.spacing;
            this.SetMaxOffset(new Point(-1, yOffset <= 0 ? -1 : yOffset));
            return this.components;
        }
    }

    /// <summary>Add a highlight operation that will be applied to the items.</summary>
    /// <param name="highlight">The highlight operation.</param>
    /// <returns>Returns the menu.</returns>
    public SelectIcon AddHighlight(Highlight highlight)
    {
        this.highlights.Add(highlight);
        return this;
    }

    /// <summary>Add an operation that will be applied to the icons.</summary>
    /// <param name="operation">The operation to perform.</param>
    /// <returns>Returns the menu.</returns>
    public SelectIcon AddOperation(Operation operation)
    {
        this.operations.Add(operation);
        return this;
    }

    /// <inheritdoc />
    public override void DrawInFrame(SpriteBatch spriteBatch, Point cursor)
    {
        // Draw items
        foreach (var component in this.Components)
        {
            var index = int.Parse(component.name, CultureInfo.InvariantCulture);
            var icon = this.Icons[index];
            component.tryHover(cursor.X + this.CurrentOffset.X, cursor.Y + this.CurrentOffset.Y, 0.2f);

            if (index == this.CurrentIndex)
            {
                spriteBatch.Draw(
                    Game1.mouseCursors,
                    new Rectangle(
                        this.xPositionOnScreen
                        + (index % this.columns * (this.length + this.spacing))
                        + this.spacing
                        - this.CurrentOffset.X,
                        this.yPositionOnScreen
                        + (index / this.columns * (this.length + this.spacing))
                        + this.spacing
                        - this.CurrentOffset.Y,
                        this.length,
                        this.length),
                    new Rectangle(194, 388, 16, 16),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.975f);
            }

            component.draw(
                spriteBatch,
                this.HighlightIcon(icon) ? Color.White : Color.White * 0.25f,
                1f,
                0,
                -this.CurrentOffset.X,
                -this.CurrentOffset.Y);

            if (component.bounds.Contains(cursor + this.CurrentOffset))
            {
                this.SetHoverText(component.hoverText);
            }
        }
    }

    /// <inheritdoc />
    public override void DrawUnder(SpriteBatch b, Point cursor) =>
        IClickableMenu.drawTextureBox(
            b,
            Game1.mouseCursors,
            OptionsDropDown.dropDownBGSource,
            this.xPositionOnScreen,
            this.yPositionOnScreen,
            this.width,
            this.height,
            Color.White,
            Game1.pixelZoom,
            false,
            0.97f);

    /// <inheritdoc />
    public override ICustomMenu MoveTo(Point dimensions)
    {
        base.MoveTo(dimensions);
        this.RefreshIcons();
        return this;
    }

    /// <summary>Refreshes the icons by applying the operations to them.</summary>
    /// <returns>Returns the menu.</returns>
    public SelectIcon RefreshIcons()
    {
        this.components = null;
        this.icons = null;
        return this;
    }

    /// <inheritdoc />
    public override bool TryLeftClick(Point cursor)
    {
        if (base.TryLeftClick(cursor))
        {
            return true;
        }

        var component = this.Components.FirstOrDefault(i => i.bounds.Contains(cursor + this.CurrentOffset));
        if (component is null)
        {
            return false;
        }

        this.CurrentIndex = int.Parse(component.name, CultureInfo.InvariantCulture);
        return true;
    }

    private static string GetUniqueId(IIcon icon) => icon.UniqueId;

    private bool HighlightIcon(IIcon icon) =>
        !this.highlights.Any() || this.highlights.All(highlight => highlight(icon));
}