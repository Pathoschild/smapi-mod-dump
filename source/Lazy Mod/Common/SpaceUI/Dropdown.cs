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

public class Dropdown : Element, ISingleTexture
{
    /*********
     ** Accessors
     *********/
    public int RequestWidth { get; set; }
    public int MaxValuesAtOnce { get; set; }
    public Texture2D Texture { get; set; } = Game1.mouseCursors;
    public Rectangle BackgroundTextureRect { get; set; } = OptionsDropDown.dropDownBGSource;
    public Rectangle ButtonTextureRect { get; set; } = OptionsDropDown.dropDownButtonSource;

    public string Value
    {
        get => Choices[ActiveChoice];
        set
        {
            if (Choices.Contains(value)) ActiveChoice = Array.IndexOf(Choices, value);
        }
    }

    public string Label => Labels[ActiveChoice];

    public int ActiveChoice { get; set; }

    public int ActivePosition { get; set; }
    public string[] Choices { get; set; } = new[] { "null" };

    public string[] Labels { get; set; } = new[] { "null" };

    public bool Dropped;

    public Action<Element> Callback;

    public static Dropdown? ActiveDropdown;
    public static int SinceDropdownWasActive = 0;

    /// <inheritdoc />
    public override int Width => Math.Max(300, Math.Min(500, RequestWidth));

    /// <inheritdoc />
    public override int Height => 44;

    /// <inheritdoc />
    public override string ClickedSound => "shwip";


    /*********
     ** Public methods
     *********/
    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        bool justClicked = false;
        if (Clicked && ActiveDropdown == null)
        {
            justClicked = true;
            Dropped = true;
            Parent.RenderLast = this;
        }

        if (Dropped)
        {
            //if (Mouse.GetState().LeftButton == ButtonState.Released)
            if (Constants.TargetPlatform != GamePlatform.Android)
            {
                if ((Mouse.GetState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released ||
                     Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && Game1.oldPadState.Buttons.A == ButtonState.Released)
                    && !justClicked)
                {
                    Game1.playSound("drumkit6");
                    Dropped = false;
                    if (Parent.RenderLast == this)
                        Parent.RenderLast = null;
                }
            }
            else
            {
                if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released ||
                     Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && Game1.oldPadState.Buttons.A == ButtonState.Released)
                    && !justClicked)
                {
                    Game1.playSound("drumkit6");
                    Dropped = false;
                    if (Parent.RenderLast == this)
                        Parent.RenderLast = null;
                }
            }

            int tall = Math.Min(MaxValuesAtOnce, Choices.Length - ActivePosition) * Height;
            int drawY = Math.Min((int)Position.Y, Game1.uiViewport.Height - tall);
            var bounds2 = new Rectangle((int)Position.X, drawY, Width, Height * MaxValuesAtOnce);
            if (bounds2.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                int choice = (Game1.getOldMouseY() - drawY) / Height;
                ActiveChoice = choice + ActivePosition;

                Callback?.Invoke(this);
            }
        }

        if (Dropped)
        {
            ActiveDropdown = this;
            SinceDropdownWasActive = 3;
        }
        else
        {
            if (ActiveDropdown == this)
                ActiveDropdown = null;
            ActivePosition = Math.Min(ActiveChoice, Choices.Length - MaxValuesAtOnce);
        }
    }

    public void ReceiveScrollWheelAction(int direction)
    {
        if (Dropped)
            ActivePosition = Math.Min(Math.Max(ActivePosition - direction / 120, 0), Choices.Length - MaxValuesAtOnce);
        else
            ActiveDropdown = null;
    }

    public void DrawOld(SpriteBatch b)
    {
        IClickableMenu.drawTextureBox(b, Texture, BackgroundTextureRect, (int)Position.X, (int)Position.Y, Width - 48, Height, Color.White,
            4, false);
        b.DrawString(Game1.smallFont, Value, new Vector2(Position.X + 4, Position.Y + 8), Game1.textColor);
        b.Draw(Texture, new Vector2(Position.X + Width - 48, Position.Y), ButtonTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None,
            0);

        if (Dropped)
        {
            int tall = Choices.Length * Height;
            IClickableMenu.drawTextureBox(b, Texture, BackgroundTextureRect, (int)Position.X, (int)Position.Y, Width - 48, tall, Color.White, 4,
                false);
            for (int i = 0; i < Choices.Length; ++i)
            {
                if (i == ActiveChoice)
                    b.Draw(Game1.staminaRect, new Rectangle((int)Position.X + 4, (int)Position.Y + i * Height, Width - 48 - 8, Height), null,
                        Color.Wheat, 0, Vector2.Zero, SpriteEffects.None, 0.98f);
                b.DrawString(Game1.smallFont, Choices[i], new Vector2(Position.X + 4, Position.Y + i * Height + 8), Game1.textColor, 0, Vector2.Zero,
                    1, SpriteEffects.None, 1);
            }
        }
    }

    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        IClickableMenu.drawTextureBox(b, Texture, BackgroundTextureRect, (int)Position.X, (int)Position.Y, Width - 48, Height, Color.White,
            4, false);
        b.DrawString(Game1.smallFont, Label, new Vector2(Position.X + 4, Position.Y + 8), Game1.textColor);
        b.Draw(Texture, new Vector2(Position.X + Width - 48, Position.Y), ButtonTextureRect, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None,
            0);

        if (Dropped)
        {
            int maxValues = MaxValuesAtOnce;
            int start = ActivePosition;
            int end = Math.Min(Choices.Length, start + maxValues);
            int tall = Math.Min(maxValues, Choices.Length - ActivePosition) * Height;
            int drawY = Math.Min((int)Position.Y, Game1.uiViewport.Height - tall);
            IClickableMenu.drawTextureBox(b, Texture, BackgroundTextureRect, (int)Position.X, drawY, Width - 48, tall, Color.White, 4, false);
            for (int i = start; i < end; ++i)
            {
                if (i == ActiveChoice)
                    b.Draw(Game1.staminaRect, new Rectangle((int)Position.X + 4, drawY + (i - ActivePosition) * Height, Width - 48 - 8, Height),
                        null, Color.Wheat, 0, Vector2.Zero, SpriteEffects.None, 0.98f);
                b.DrawString(Game1.smallFont, Labels[i], new Vector2(Position.X + 4, drawY + (i - ActivePosition) * Height + 8), Game1.textColor, 0,
                    Vector2.Zero, 1, SpriteEffects.None, 1);
            }
        }
    }
}