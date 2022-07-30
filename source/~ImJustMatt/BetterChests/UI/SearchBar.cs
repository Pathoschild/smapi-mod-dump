/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.UI;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewMods.Common.Helpers;
using StardewMods.Common.Integrations.BetterChests;
using StardewValley;
using StardewValley.Menus;

/// <summary>
///     Menu for searching for chests which contain specific items.
/// </summary>
internal class SearchBar : IClickableMenu
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SearchBar" /> class.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <param name="matcher">ItemMatcher for holding the selected item tags.</param>
    public SearchBar(IModHelper helper, IItemMatcher matcher)
    {
        this.Helper = helper;
        this.ItemMatcher = matcher;

        var texture = this.Helper.GameContent.Load<Texture2D>("LooseSprites\\textBox");
        this.width = Math.Min(12 * Game1.tileSize, Game1.uiViewport.Width);
        this.height = texture.Height;

        var origin = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
        this.xPositionOnScreen = (int)origin.X;
        this.yPositionOnScreen = Game1.tileSize;

        this.SearchField = new(texture, null, Game1.smallFont, Game1.textColor)
        {
            X = this.xPositionOnScreen,
            Y = this.yPositionOnScreen,
            Width = this.width,
            Selected = true,
            Text = this.ItemMatcher.StringValue,
        };

        this.SearchArea = new(Rectangle.Empty, string.Empty)
        {
            visible = true,
            bounds = new(this.SearchField.X, this.SearchField.Y, this.SearchField.Width, this.SearchField.Height),
        };

        this.SearchIcon = new(Rectangle.Empty, Game1.mouseCursors, new(80, 0, 13, 13), 2.5f)
        {
            bounds = new(this.SearchField.X + this.SearchField.Width - 38, this.SearchField.Y + 6, 32, 32),
        };
    }

    private IModHelper Helper { get; }

    private IItemMatcher ItemMatcher { get; }

    private ClickableComponent SearchArea { get; }

    private TextBox SearchField { get; }

    private ClickableTextureComponent SearchIcon { get; }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        this.SearchField.Draw(b);
        this.SearchIcon.draw(b);
        this.drawMouse(b);
    }

    /// <inheritdoc />
    public override void receiveKeyPress(Keys key)
    {
        switch (key)
        {
            case Keys.Enter:
                this.exitThisMenuNoSound();
                return;
            case Keys.Escape:
                this.SearchField.Text = string.Empty;
                this.exitThisMenuNoSound();
                return;
            default:
                return;
        }
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.SearchArea.containsPoint(x, y))
        {
            this.SearchField.Selected = true;
            return;
        }

        this.SearchField.Selected = false;
        this.exitThisMenuNoSound();
    }

    /// <inheritdoc />
    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        if (this.SearchArea.containsPoint(x, y))
        {
            this.SearchField.Selected = true;
            this.SearchField.Text = string.Empty;
            return;
        }

        this.SearchField.Selected = false;
        this.exitThisMenuNoSound();
    }

    /// <inheritdoc />
    protected override void cleanupBeforeExit()
    {
        if (!string.IsNullOrWhiteSpace(this.SearchField.Text))
        {
            Log.Trace($"ChestFinder: {this.SearchField.Text}");
            this.ItemMatcher.StringValue = this.SearchField.Text;
        }
        else
        {
            this.ItemMatcher.Clear();
        }
    }
}