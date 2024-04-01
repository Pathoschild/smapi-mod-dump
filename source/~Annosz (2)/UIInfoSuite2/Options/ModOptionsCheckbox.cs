/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace UIInfoSuite2.Options;

internal class ModOptionsCheckbox : ModOptionsElement
{
  private readonly Action<bool> _setOption;
  private readonly Action<bool> _toggleOptionsDelegate;
  private bool _isChecked;

  public ModOptionsCheckbox(
    string label,
    int whichOption,
    Action<bool> toggleOptionDelegate,
    Func<bool> getOption,
    Action<bool> setOption,
    ModOptionsCheckbox parent = null
  ) : base(label, whichOption, parent)
  {
    _toggleOptionsDelegate = toggleOptionDelegate;
    _setOption = setOption;

    _isChecked = getOption();
    _toggleOptionsDelegate(_isChecked);
  }

  private bool _canClick => !(_parent is ModOptionsCheckbox) || (_parent as ModOptionsCheckbox)._isChecked;

  public override void ReceiveLeftClick(int x, int y)
  {
    if (_canClick)
    {
      Game1.playSound("drumkit6");
      base.ReceiveLeftClick(x, y);
      _isChecked = !_isChecked;
      _setOption(_isChecked);
      _toggleOptionsDelegate(_isChecked);
    }
  }

  public override void Draw(SpriteBatch batch, int slotX, int slotY)
  {
    batch.Draw(
      Game1.mouseCursors,
      new Vector2(slotX + Bounds.X, slotY + Bounds.Y),
      _isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked,
      Color.White * (_canClick ? 1f : 0.33f),
      0.0f,
      Vector2.Zero,
      Game1.pixelZoom,
      SpriteEffects.None,
      0.4f
    );
    base.Draw(batch, slotX, slotY);
  }

  public override Point? GetRelativeSnapPoint(Rectangle slotBounds)
  {
    // Based on the value calculated in OptionsPage.snapCursorToCurrentSnappedComponent
    return new Point(Bounds.X + 16, Bounds.Y + 13);
  }
}
