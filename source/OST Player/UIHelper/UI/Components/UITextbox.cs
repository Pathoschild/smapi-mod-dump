/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;

namespace UIHelper;

internal class UITextbox : UIComponent, IKeyboardSubscriber
{
    private static readonly HashSet<SButton> allowedKeys = new(){
        SButton.A,
        SButton.B,
        SButton.C,
        SButton.D,
        SButton.E,
        SButton.F,
        SButton.G,
        SButton.H,
        SButton.I,
        SButton.J,
        SButton.K,
        SButton.L,
        SButton.M,
        SButton.N,
        SButton.O,
        SButton.P,
        SButton.Q,
        SButton.R,
        SButton.S,
        SButton.T,
        SButton.U,
        SButton.V,
        SButton.W,
        SButton.X,
        SButton.Y,
        SButton.Z,
        SButton.NumPad0,
        SButton.NumPad1,
        SButton.NumPad2,
        SButton.NumPad3,
        SButton.NumPad4,
        SButton.NumPad5,
        SButton.NumPad6,
        SButton.NumPad7,
        SButton.NumPad8,
        SButton.NumPad9,
        SButton.Space
    };
    System.Timers.Timer focusBarTimer;
    private string focusBar = "";
    private string Text{
        get{return (string)Value;}
        set{Value = value;}
    }
    internal override bool IsFocus { 
        get{
            return base.IsFocus;
        } 
        set{
            if(value != focused){
                focusBarTimer.Enabled = value;
                base.IsFocus = value;
                if(value)
                    Game1.keyboardDispatcher.Subscriber = this;
                else if(this == Game1.keyboardDispatcher.Subscriber){
                    Game1.keyboardDispatcher.Subscriber = null;
                    InvokeValueChange();
                }
            }
        } 
    }

    public bool Selected { get => IsFocus; set => IsFocus = value; }

    internal UITextbox(Rectangle initialBounds, object initText, EAlign align, Color? textColor) : base(initialBounds, initText, align, textColor)
    {
        focusBarTimer = new(800){
            AutoReset = true
        };
        focusBarTimer.Elapsed += OnElapsed;
    }

    private void OnElapsed(object? sender, ElapsedEventArgs e)
    {
        focusBar = (focusBar == "")? "|": "";
    }

    public override void draw(SpriteBatch b)
    {
        b.Draw(mouseCursors, new Rectangle(Bounds.X, Bounds.Y, 12, Bounds.Height), new(293, 360, 6, 24), configColor(Color.Wheat));
        b.Draw(mouseCursors, new Rectangle(Bounds.X+12, Bounds.Y, Bounds.Width - 24, Bounds.Height), new(299, 360, 12, 24), configColor(Color.Wheat));
        b.Draw(mouseCursors, new Rectangle(Bounds.Right-12, Bounds.Y, 12, Bounds.Height), new(311, 360, 6, 24), configColor(Color.Wheat));

        string str = Text + (IsFocus? focusBar: "");
        Vector2 strCentered = UIUtils.getCenteredText(new(Bounds.X+5+12, Bounds.Y, Bounds.Width-10 - 24, Bounds.Height), str, Game1.smallFont);
        strCentered.X = Bounds.X + 16 + 5;
        b.DrawString(Game1.smallFont, str, strCentered, configColor(textColor));
    }

    public override bool containsPoint(int x, int y)
    {
        return Bounds.Contains(x, y);
    }

    protected override Color configColor(Color color, float hlFactor = 0.8F, float clckFactor = 1.3F)
    {
        return base.configColor(color, hlFactor, clckFactor);
    }

    internal override void receiveLeftClick(int x, int y)
    {
        IsFocus = true;
        IsClicked = true;
    }

    private void DeleteLastChar(){
        Game1.playSound("woodyStep");
        if(Text.Length == 0)
            return;
        
        Text = Text.Remove(Text.Length - 1);
    }

    private void AddChar(char c){
        Game1.playSound("woodyStep");
        if(CanAddChar()){
            Text += ModEntry.context.Helper.Input.IsDown(SButton.LeftShift)? c: char.ToLower(c);
        }
    }

    private bool CanAddChar(){
        return (int)Game1.smallFont.MeasureString(Text + "M|").X < width-10-24;
    }

    public void RecieveTextInput(char inputChar)
    {
        if(IsFocus){
            AddChar(inputChar);
        }
    }

    public void RecieveTextInput(string text)
    {
        if(IsFocus){
            foreach(char c in text){
                AddChar(c);
            }
        }
    }

    public void RecieveCommandInput(char command)
    {
        if(!IsFocus)
            return;

        if(command == '\b')
            DeleteLastChar();
        else if(command == '\r'){
            IsFocus = false;
        }
    }

    public void RecieveSpecialInput(Keys key){}
}
