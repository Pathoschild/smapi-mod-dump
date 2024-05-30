/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.UI.Overlays;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewMods.Common.UI.Components;
using StardewMods.Common.UI.Menus;

/// <summary>Menu for searching for chests which contain specific items.</summary>
internal sealed class SearchOverlay : BaseMenu
{
    private readonly TextField textField;

    /// <summary>Initializes a new instance of the <see cref="SearchOverlay" /> class.</summary>
    /// <param name="getMethod">The function that gets the current search text.</param>
    /// <param name="setMethod">The action that sets the search text.</param>
    public SearchOverlay(Func<string> getMethod, Action<string> setMethod)
    {
        var searchBarWidth = Math.Min(12 * Game1.tileSize, Game1.uiViewport.Width);
        var origin = Utility.getTopLeftPositionForCenteringOnScreen(searchBarWidth, 48);

        this.textField =
            new TextField(this, (int)origin.X, Game1.tileSize, searchBarWidth, getMethod, setMethod)
            {
                Selected = true,
            };

        this.allClickableComponents.Add(this.textField);
    }

    /// <inheritdoc />
    public override void receiveKeyPress(Keys key)
    {
        if (key is Keys.Enter or Keys.Escape)
        {
            this.exitThisMenuNoSound();
        }
    }

    /// <inheritdoc />
    public override bool TryLeftClick(Point cursor)
    {
        this.textField.TryLeftClick(cursor);
        if (this.textField.Selected)
        {
            return false;
        }

        this.textField.Selected = false;
        this.exitThisMenuNoSound();
        return true;
    }

    /// <inheritdoc />
    public override bool TryRightClick(Point cursor)
    {
        this.textField.TryRightClick(cursor);
        if (this.textField.Selected)
        {
            return false;
        }

        this.textField.Selected = false;
        this.exitThisMenuNoSound();
        return true;
    }
}