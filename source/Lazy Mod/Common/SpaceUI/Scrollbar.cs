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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Common.SpaceUI;

public class Scrollbar : Element
{
    /*********
     ** Fields
     *********/
    private bool DragScroll;


    /*********
     ** Accessors
     *********/
    public int RequestHeight { get; set; }

    public int Rows { get; set; }
    public int FrameSize { get; set; }

    public int TopRow { get; private set; }
    public int MaxTopRow => Math.Max(0, Rows - FrameSize);

    public float ScrollPercent => MaxTopRow > 0 ? TopRow / (float)MaxTopRow : 0f;

    /// <inheritdoc />
    public override int Width => 24;

    /// <inheritdoc />
    public override int Height => RequestHeight;


    /*********
     ** Public methods
     *********/
    public void ScrollBy(int amount)
    {
        int row = CommonHelper.Clamp(0, TopRow + amount, MaxTopRow);
        if (row != TopRow)
        {
            Game1.playSound("shwip");
            TopRow = row;
        }
    }

    public void ScrollTo(int row)
    {
        if (TopRow != row)
        {
            Game1.playSound("shiny4");
            TopRow = CommonHelper.Clamp(0, row, MaxTopRow);
        }
    }

    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        if (Clicked)
            DragScroll = true;
        if (Constants.TargetPlatform != GamePlatform.Android)
        {
            if (DragScroll && Mouse.GetState().LeftButton == ButtonState.Released)
                DragScroll = false;
        }
        else
        {
            if (DragScroll && Game1.input.GetMouseState().LeftButton == ButtonState.Released)
                DragScroll = false;
        }


        if (DragScroll)
        {
            int my = Game1.getMouseY();
            int relY = (int)(my - Position.Y - 40 / 2);
            ScrollTo((int)Math.Round(relY / (float)(Height - 40) * MaxTopRow));
        }
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        // Don't draw a scrollbar if scrolling is (currently) not possible.
        if (MaxTopRow == 0)
            return;

        Rectangle back = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        Vector2 front = new Vector2(back.X, back.Y + (Height - 40) * ScrollPercent);

        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), back.X, back.Y, back.Width, back.Height, Color.White, Game1.pixelZoom, false);
        b.Draw(Game1.mouseCursors, front, new Rectangle(435, 463, 6, 12), Color.White, 0f, new Vector2(), Game1.pixelZoom, SpriteEffects.None, 0.77f);
    }
}