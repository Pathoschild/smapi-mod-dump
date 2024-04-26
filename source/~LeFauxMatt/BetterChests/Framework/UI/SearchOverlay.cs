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

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

/// <summary>Menu for searching for chests which contain specific items.</summary>
internal sealed class SearchOverlay : IClickableMenu
{
    private readonly SearchBar searchBar;

    /// <summary>Initializes a new instance of the <see cref="SearchOverlay" /> class.</summary>
    /// <param name="getMethod">The function that gets the current search text.</param>
    /// <param name="setMethod">The action that sets the search text.</param>
    public SearchOverlay(Func<string> getMethod, Action<string> setMethod)
    {
        var searchBarWidth = Math.Min(12 * Game1.tileSize, Game1.uiViewport.Width);
        var origin = Utility.getTopLeftPositionForCenteringOnScreen(searchBarWidth, 48);

        this.searchBar =
            new SearchBar((int)origin.X, Game1.tileSize, searchBarWidth, getMethod, setMethod)
            {
                Selected = true,
            };
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        this.searchBar.Draw(b);
        this.drawMouse(b);
    }

    /// <inheritdoc />
    public override void performHoverAction(int x, int y) => this.searchBar.Update(x, y);

    /// <inheritdoc />
    public override void receiveKeyPress(Keys key)
    {
        if (key is not (Keys.Enter or Keys.Escape))
        {
            return;
        }

        this.searchBar.Update();
        this.exitThisMenuNoSound();
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        this.searchBar.LeftClick(x, y);
        if (this.searchBar.Selected)
        {
            return;
        }

        this.searchBar.Selected = false;
        this.exitThisMenuNoSound();
    }

    /// <inheritdoc />
    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        this.searchBar.RightClick(x, y);
        if (this.searchBar.Selected)
        {
            return;
        }

        this.searchBar.Selected = false;
        this.exitThisMenuNoSound();
    }
}