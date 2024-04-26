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

public class Slider : Element
{
    /*********
     ** Fields
     *********/
    protected bool Dragging;


    /*********
     ** Accessors
     *********/
    public int RequestWidth { get; set; }

    public Action<Element> Callback { get; set; }

    /// <inheritdoc />
    public override int Width => RequestWidth;

    /// <inheritdoc />
    public override int Height => 24;


    /*********
     ** Public methods
     *********/
    /// <inheritdoc />
    public override void Draw(SpriteBatch b) { }
}

internal class Slider<T> : Slider
{
    /*********
     ** Accessors
     *********/
    public T Minimum { get; set; }
    public T Maximum { get; set; }
    public T Value { get; set; }

    public T Interval { get; set; }


    /*********
     ** Public methods
     *********/
    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        if (Clicked)
            Dragging = true;
        if (Constants.TargetPlatform != GamePlatform.Android)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Released && Game1.input.GetGamePadState().Buttons.A == ButtonState.Released)
                Dragging = false;
        }
        else
        {
            if (Game1.input.GetMouseState().LeftButton == ButtonState.Released && Game1.input.GetGamePadState().Buttons.A == ButtonState.Released)
                Dragging = false;
        }


        if (Dragging)
        {
            float perc = (Game1.getOldMouseX() - Position.X) / Width;
            Value = CommonHelper.Adjust(Value, Interval);
            Value = Value switch
            {
                int => CommonHelper.Clamp<T>(Minimum, (T)(object)(int)(perc * ((int)(object)Maximum - (int)(object)Minimum) + (int)(object)Minimum), Maximum),
                float => CommonHelper.Clamp<T>(Minimum, (T)(object)(perc * ((float)(object)Maximum - (float)(object)Minimum) + (float)(object)Minimum), Maximum),
                _ => Value
            };

            Callback?.Invoke(this);
        }
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        float perc = Value switch
        {
            int => ((int)(object)Value - (int)(object)Minimum) / (float)((int)(object)Maximum - (int)(object)Minimum),
            float => ((float)(object)Value - (float)(object)Minimum) / ((float)(object)Maximum - (float)(object)Minimum),
            _ => 0
        };

        Rectangle back = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        Rectangle front = new Rectangle((int)(Position.X + perc * (Width - 40)), (int)Position.Y, 40, Height);

        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), back.X, back.Y, back.Width, back.Height, Color.White, Game1.pixelZoom, false);
        b.Draw(Game1.mouseCursors, new Vector2(front.X, front.Y), new Rectangle(420, 441, 10, 6), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    }
}