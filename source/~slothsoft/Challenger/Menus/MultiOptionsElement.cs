/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace Slothsoft.Challenger.Menus;

public class MultiOptionsElement : OptionsElement {
    private readonly OptionsElement[] _columns;
    private readonly int _columnWidth;
    public MultiOptionsElement(Rectangle entireBounds, params OptionsElement[] columns)
        : base("", entireBounds.X, entireBounds.Y, entireBounds.Width, entireBounds.Height) {
        _columns = columns;

        _columnWidth = bounds.Width / columns.Length;
        for (var i = 0; i < _columns.Length; i++) {
            _columns[i].bounds.X = entireBounds.X + i * _columnWidth;
            _columns[i].bounds.Width = _columnWidth;
        }
    }

    public override void leftClickHeld(int x, int y) {
        GetElement(x)?.leftClickHeld(x, y);
    }

    private OptionsElement? GetElement(int x) {
        var index = x / _columnWidth;
        if (index < 0 || index >= _columns.Length) {
            return null;
        }
        return _columns[index];
    }

    public override void receiveLeftClick(int x, int y) {
        GetElement(x)?.receiveLeftClick(x, y);
    }

    public override void leftClickReleased(int x, int y) {
        GetElement(x)?.leftClickReleased(x, y);
    }

    public override void receiveKeyPress(Keys key) {
        foreach (var column in _columns) {
            column.receiveKeyPress(key);
        }
    }

    public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu? context = null) {
        foreach (var column in _columns) {
            column.draw(b, slotX, slotY, context);
        }
    }
}