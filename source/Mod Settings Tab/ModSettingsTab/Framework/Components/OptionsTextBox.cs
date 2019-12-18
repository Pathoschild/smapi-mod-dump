using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsTextBox : OptionsElement
    {
        private bool NumAsString { get; }
        private static readonly Texture2D TextBoxTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
        private readonly TextBox _textBox;

        public OptionsTextBox(
            string name,
            string modId,
            string label,
            StaticConfig config,
            Point slotSize,
            bool floatOnly = false,
            bool numbersOnly = false,
            bool numAsString = false)
            : base(name, modId, label, config, 32, slotSize.Y / 2 - 4, slotSize.X / 2 - 64, 48)
        {
            NumAsString = numAsString;
            // text field value
            var str = config[name].ToString();
            // if the string is short
            if (str.Length < 12)
            {
                Bounds.Width /= 2;
                InfoIconBounds = new Rectangle();
            }
            Offset.Y = 8;

            _textBox = new TextBox(SaveState)
            {
                TitleText = name,
                Width = Bounds.Width,
                Height = Bounds.Height,
                numbersOnly = numbersOnly,
                FloatOnly = floatOnly,
                Text = !floatOnly ? str : config[name].ToString(Formatting.Indented)
            };
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            _textBox.Update();
        }
        
        private void SaveState(string s)
        {
            s = s.Trim();
            if (_textBox.numbersOnly)
            {
                Config[Name] =
                    NumAsString || !int.TryParse(s, out var result) ? (JToken) s : (JToken) result;
            }
            else if (_textBox.FloatOnly)
            {
                Config[Name] =
                    NumAsString || !float.TryParse(s, NumberStyles.Any,
                        CultureInfo.CreateSpecificCulture("en-US"), out var result)
                        ? (JToken) s
                        : (JToken) result;
            }
            else
                Config[Name]= s;
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            _textBox.X = slotX + Bounds.X;
            _textBox.Y = slotY + Bounds.Y;
            base.Draw(b, slotX, slotY);
            b.Draw(TextBoxTexture, new Rectangle(_textBox.X, _textBox.Y, 16, _textBox.Height),
                new Rectangle(0, 0, 16, _textBox.Height), Color.White);
            b.Draw(TextBoxTexture, new Rectangle(_textBox.X + 16, _textBox.Y, _textBox.Width - 32, _textBox.Height),
                new Rectangle(16, 0, 4, _textBox.Height), Color.White);
            b.Draw(TextBoxTexture, new Rectangle(_textBox.X + _textBox.Width - 16, _textBox.Y, 16, _textBox.Height),
                new Rectangle(TextBoxTexture.Bounds.Width - 16, 0, 16, _textBox.Height),
                Color.White);
            _textBox.Draw(b);
        }
    }
}