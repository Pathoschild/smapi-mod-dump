using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Menu;

namespace ModSettingsTab.Framework.Components
{
    public class ModManagerAddNameTextBox : OptionsElement
    {
        private readonly TextBox _textBox;
        public string Text;
        
        public ModManagerAddNameTextBox() : base("", "", "", null,
            32, BaseOptionsModPage.SlotSize.Y / 2 - 4, BaseOptionsModPage.SlotSize.X / 2 - 64, 48)
        {
            Label = Helper.I18N.Get("ModManager.Add.NameTextBox");
            Offset.Y = 8;
            _textBox = new TextBox(SaveState)
            {
                TitleText = "",
                Width = Bounds.Width,
                Height = Bounds.Height,
                numbersOnly = false,
                FloatOnly = false,
                Text = ""
            };
        }
        
        public override void ReceiveLeftClick(int x, int y)
        {
            _textBox.Update();
        }
        
        private void SaveState(string s)
        {
            Text = s;
        }
        
        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            _textBox.X = slotX + Bounds.X;
            _textBox.Y = slotY + Bounds.Y;
            base.Draw(b, slotX, slotY);
            b.Draw(OptionsTextBox.TextBoxTexture, new Rectangle(_textBox.X, _textBox.Y, 16, _textBox.Height),
                new Rectangle(0, 0, 16, 48), Color.White);
            b.Draw(OptionsTextBox.TextBoxTexture, new Rectangle(_textBox.X + 16, _textBox.Y, _textBox.Width - 32, _textBox.Height),
                new Rectangle(16, 0, 4, 48), Color.White);
            b.Draw(OptionsTextBox.TextBoxTexture, new Rectangle(_textBox.X + _textBox.Width - 16, _textBox.Y, 16, _textBox.Height),
                new Rectangle(OptionsTextBox.TextBoxTexture.Bounds.Width - 16, 0, 16, 48),
                Color.White);
            _textBox.Draw(b);
        }
    }
}