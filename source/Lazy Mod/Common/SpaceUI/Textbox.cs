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
using StardewValley;

namespace Common.SpaceUI;

public class Textbox : Element, IKeyboardSubscriber
{
    /*********
     ** Fields
     *********/
    private readonly Texture2D Tex;
    private readonly SpriteFont Font;
    private bool SelectedImpl;


    /*********
     ** Accessors
     *********/
    public virtual string String { get; set; }

    public bool Selected
    {
        get => SelectedImpl;
        set
        {
            if (SelectedImpl == value)
                return;

            SelectedImpl = value;
            if (SelectedImpl)
                Game1.keyboardDispatcher.Subscriber = this;
            else
            {
                if (Game1.keyboardDispatcher.Subscriber == this)
                    Game1.keyboardDispatcher.Subscriber = null;
            }
        }
    }

    public Action<Element> Callback { get; set; }

    /// <inheritdoc />
    public override int Width => 192;

    /// <inheritdoc />
    public override int Height => 48;


    /*********
     ** Public methods
     *********/
    public Textbox()
    {
        Tex = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
        Font = Game1.smallFont;
    }

    /// <inheritdoc />
    public override void Update(bool isOffScreen = false)
    {
        base.Update(isOffScreen);

        if (ClickGestured && Callback != null)
        {
            Selected = Hover;
        }
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch b)
    {
        if (IsHidden())
            return;

        b.Draw(Tex, Position, Color.White);

        // Copied from game code - caret
        string text = String;
        Vector2 vector2;
        for (vector2 = Font.MeasureString(text); vector2.X > 192f; vector2 = Font.MeasureString(text))
            text = text.Substring(1);
        if (DateTime.UtcNow.Millisecond % 1000 >= 500 && Selected)
            b.Draw(Game1.staminaRect, new Rectangle((int)Position.X + 16 + (int)vector2.X + 2, (int)Position.Y + 8, 4, 32), Game1.textColor);

        b.DrawString(Font, text, Position + new Vector2(16, 12), Game1.textColor);
    }

    /// <inheritdoc />
    public void RecieveTextInput(char inputChar)
    {
        ReceiveInput(inputChar.ToString());

        // Copied from game code
        switch (inputChar)
        {
            case '"':
                return;
            case '$':
                Game1.playSound("money");
                break;
            case '*':
                Game1.playSound("hammer");
                break;
            case '+':
                Game1.playSound("slimeHit");
                break;
            case '<':
                Game1.playSound("crystal");
                break;
            case '=':
                Game1.playSound("coin");
                break;
            default:
                Game1.playSound("cowboy_monsterhit");
                break;
        }
    }

    /// <inheritdoc />
    public void RecieveTextInput(string text)
    {
        ReceiveInput(text);
    }

    /// <inheritdoc />
    public void RecieveCommandInput(char command)
    {
        if (command == '\b' && String.Length > 0)
        {
            Game1.playSound("tinyWhip");
            String = String.Substring(0, String.Length - 1);
            Callback?.Invoke(this);
        }
    }

    /// <inheritdoc />
    public void RecieveSpecialInput(Keys key)
    {
    }


    /*********
     ** Protected methods
     *********/
    protected virtual void ReceiveInput(string str)
    {
        String += str;
        Callback?.Invoke(this);
    }
}
