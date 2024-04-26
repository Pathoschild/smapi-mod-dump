/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Common.SpaceUI;

public class Checkbox : Element, ISingleTexture
{
    /*********
     ** Accessors
     *********/
    public Texture2D Texture { get; set; }
    public Rectangle CheckedTextureRect { get; set; }
    public Rectangle UncheckedTextureRect { get; set; }

    public Action<Element> Callback { get; set; }

    public bool Checked { get; set; } = true;

    /// <inheritdoc />
    public override int Width => CheckedTextureRect.Width * 4;

    /// <inheritdoc />
    public override int Height => CheckedTextureRect.Height * 4;

    /// <inheritdoc />
    public override string ClickedSound => "drumkit6";


    /*********
     ** Public methods
     *********/
    public Checkbox()
    {
        Texture = Game1.mouseCursors;
        CheckedTextureRect = OptionsCheckbox.sourceRectChecked;
        UncheckedTextureRect = OptionsCheckbox.sourceRectUnchecked;
    }

    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        if (Clicked && Callback != null)
        {
            Checked = !Checked;
            Callback.Invoke(this);
        }
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        b.Draw(Texture, Position, Checked ? CheckedTextureRect : UncheckedTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
        Game1.activeClickableMenu?.drawMouse(b);
    }
}