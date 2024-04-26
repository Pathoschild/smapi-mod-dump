/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

/// <summary>Represents a search overlay control that allows the user to input text.</summary>
internal sealed class SearchBar : ClickableComponent
{
    private const int CountdownTimer = 20;

    private readonly Func<string> getMethod;
    private readonly ClickableTextureComponent icon;
    private readonly Action<string> setMethod;
    private readonly TextBox textBox;
    private string previousText;
    private int timeout;

    /// <summary>Initializes a new instance of the <see cref="SearchBar" /> class.</summary>
    /// <param name="x">The x-coordinate of the search bar.</param>
    /// <param name="y">The y-coordinate of the search bar.</param>
    /// <param name="width">The width of the search bar.</param>
    /// <param name="getMethod">The function that gets the current search text.</param>
    /// <param name="setMethod">The action that sets the search text.</param>
    public SearchBar(int x, int y, int width, Func<string> getMethod, Action<string> setMethod)
        : base(new Rectangle(x, y, width, 48), "SearchBar")
    {
        this.previousText = getMethod();
        this.getMethod = getMethod;
        this.setMethod = setMethod;
        var texture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
        this.textBox = new TextBox(texture, null, Game1.smallFont, Game1.textColor)
        {
            X = this.bounds.X,
            Y = this.bounds.Y,
            Width = this.bounds.Width,
            Text = this.previousText,
        };

        this.icon = new ClickableTextureComponent(
            new Rectangle(this.bounds.X + this.textBox.Width - 38, this.bounds.Y + 6, 32, 32),
            Game1.mouseCursors,
            new Rectangle(80, 0, 13, 13),
            2.5f);
    }

    /// <summary>Gets or sets a value indicating whether the search bar is currently selected.</summary>
    public bool Selected
    {
        get => this.textBox.Selected;
        set => this.textBox.Selected = value;
    }

    /// <summary>Gets or sets the width of the search bar.</summary>
    public int Width
    {
        get => this.bounds.Width;
        set
        {
            this.bounds.Width = value;
            this.textBox.Width = value;
            this.icon.bounds.X = this.bounds.X + this.textBox.Width - 38;
        }
    }

    /// <summary>Gets or sets the x-coordinate of the search bar.</summary>
    public int X
    {
        get => this.bounds.X;
        set
        {
            this.bounds.X = value;
            this.textBox.X = value;
            this.icon.bounds.X = value + this.textBox.Width - 38;
        }
    }

    /// <summary>Gets or sets the y-coordinate of the search bar.</summary>
    public int Y
    {
        get => this.bounds.Y;
        set
        {
            this.bounds.Y = value;
            this.textBox.Y = value;
            this.icon.bounds.Y = value + 6;
        }
    }

    private string Text
    {
        get => this.getMethod();
        set => this.setMethod(value);
    }

    /// <summary>Draws the search overlay to the screen.</summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        this.textBox.Draw(spriteBatch);
        this.icon.draw(spriteBatch);
    }

    /// <summary>Performs a left click at the specified coordinates on the screen.</summary>
    /// <param name="mouseX">The X-coordinate of the mouse click.</param>
    /// <param name="mouseY">The Y-coordinate of the mouse click.</param>
    /// <returns>true if the search bar was clicked; otherwise, false.</returns>
    public bool LeftClick(int mouseX, int mouseY)
    {
        this.Selected = this.bounds.Contains(mouseX, mouseY);
        return this.Selected;
    }

    /// <summary>Performs a right click at the specified coordinates on the screen.</summary>
    /// <param name="mouseX">The X-coordinate of the mouse click.</param>
    /// <param name="mouseY">The Y-coordinate of the mouse click.</param>
    /// <returns>true if the search bar was clicked; otherwise, false.</returns>
    public bool RightClick(int mouseX, int mouseY)
    {
        if (!this.bounds.Contains(mouseX, mouseY))
        {
            this.Selected = false;
            return false;
        }

        this.Selected = true;
        this.textBox.Text = string.Empty;
        return this.Selected;
    }

    /// <summary>Resets the textbox text to match the current search text.</summary>
    public void Reset() => this.textBox.Text = this.Text;

    /// <summary>Updates the current search text with the textbox text.</summary>
    public void Update()
    {
        if (this.Text != this.textBox.Text)
        {
            this.Text = this.textBox.Text;
        }
    }

    /// <summary>Updates the search bar based on the mouse position.</summary>
    /// <param name="mouseX">The x-coordinate of the mouse position.</param>
    /// <param name="mouseY">The y-coordinate of the mouse position.</param>
    public void Update(int mouseX, int mouseY)
    {
        this.textBox.Hover(mouseX, mouseY);
        if (this.timeout > 0 && --this.timeout == 0 && this.Text != this.textBox.Text)
        {
            this.Update();
        }

        if (this.textBox.Text.Equals(this.previousText, StringComparison.Ordinal))
        {
            return;
        }

        this.timeout = SearchBar.CountdownTimer;
        this.previousText = this.textBox.Text;
    }
}