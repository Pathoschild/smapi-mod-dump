/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using BankOfFerngill.Framework.Configs;
using BankOfFerngill.Framework.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace BankOfFerngill.Framework.Menu
{
    public class BankMenu : IClickableMenu
    {
        private readonly IMonitor _monitor;
        private readonly ITranslationHelper _i18N;
        private readonly BoFConfig _config;
        private readonly BankData _bankData;

        public delegate void DoneBehaviour(string s);

        private const int UiWidth = 1280;
        private const int UiHeight = 760;

        private readonly int _xPos = (int)(Game1.viewport.Width * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 - 
                                     (UiWidth / 2);

        private readonly int _yPos = (int)(Game1.viewport.Height * Game1.options.zoomLevel * (1 / Game1.options.uiScale)) / 2 -
                                     (UiHeight / 2);
        
        //Clickable Components
        private readonly ClickableComponent _title;
        private readonly ClickableTextureComponent _okButtonCc;
        //private ClickableComponent _textBoxCc;
        private readonly TextBox _textBox;
        private readonly DoneBehaviour _doneBehaviour;
       
        private readonly string _description;

        public BankMenu(IMonitor monitor, 
            ITranslationHelper i18N, 
            BoFConfig config, 
            BankData bankData,
            DoneBehaviour doneBehaviour,
            string title,
            string description)
        {
            initialize(_xPos, _yPos, UiWidth, UiHeight);
            _monitor = monitor;
            _i18N = i18N;
            _config = config;
            _bankData = bankData;
            _doneBehaviour = doneBehaviour;
            _title = new ClickableComponent(new Rectangle(_xPos + 500, _yPos + 96, UiWidth - 400, 128), title);
            _description = description;

            
            //Lets set up the text box
            _textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor)
            {
                X = _xPos + (UiWidth - 512) / 2,
                Y = _title.bounds.Y + (64 * 6),
                Width = 512,
                Height = 192
            };
            Game1.keyboardDispatcher.Subscriber = _textBox;
            _textBox.OnEnterPressed += TextBoxEnter;
            _textBox.Selected = true;
            
            
            _okButtonCc =
                new ClickableTextureComponent(
                    new Rectangle(_textBox.X + _textBox.Width + 32 + 4, _textBox.Y, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

        }
        
        //Overrides
        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(_xPos, _yPos, UiWidth, UiHeight, false, true);
            //Draw Title Label
            
            Utility.drawTextWithShadow(b, _title.name, Game1.dialogueFont, new Vector2(_title.bounds.X, _title.bounds.Y), Color.Black);
            SpriteText.drawString(b, _description, _xPos + 45, _title.bounds.Y + 64, 999999, 999, 9999, 0.75f, 0.865f, junimoText: false);
            
            _textBox.Draw(b);
            _okButtonCc.draw(b);
            
            //Draw the mouse
            drawMouse(b);
        }
        
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            
            _textBox.Update();

            if (_okButtonCc.containsPoint(x, y))
            {
                TextBoxEnter(_textBox);
            }
        }

        private void TextBoxEnter(TextBox sender)
        {
            if (sender.Text.Length >= 1)
            {
                if (_doneBehaviour is not null)
                {
                    _doneBehaviour(sender.Text);
                    _textBox.Selected = false;
                }
                else
                {
                    _monitor.Log($"The Ok button was null.");
                }
            }
        }
    }
}