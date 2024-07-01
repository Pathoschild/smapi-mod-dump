/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications
{
    public class CharacterCustomizationArchipelago : CharacterCustomization
    {
        private const int ARCHIPELAGO_Y_OFFSET = 488;

        public TextBox IpAddressTextBox;
        public TextBox SlotNameTextBox;
        public TextBox PasswordTextBox;
        private ClickableComponent ipAddressCC;
        private ClickableComponent slotNameCC;
        private ClickableComponent passwordCC;
        private ClickableComponent ipAddressLabel;
        private ClickableComponent slotNameLabel;
        private ClickableComponent passwordLabel;

        public CharacterCustomizationArchipelago(CharacterCustomization parent, IModHelper modHelper)
            : base(parent.source)
        {
            height += 48;
            CreateArchipelagoFields();
            SetupArchipelagoFieldsPositions();
            var bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
            gameWindowSizeChanged(bounds, bounds);
            SetDefaultValues(modHelper);
        }

        public sealed override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            SetupArchipelagoFieldsPositions();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            IpAddressTextBox.Draw(b);
            SlotNameTextBox.Draw(b);
            PasswordTextBox.Draw(b);

            var ipIsValid = IpIsFormattedCorrectly();

            var ipLabelColor = ipIsValid ? Game1.textColor : Color.Red;
            Utility.drawTextWithShadow(b, ipAddressLabel.name, Game1.smallFont, new Vector2(ipAddressLabel.bounds.X, ipAddressLabel.bounds.Y), ipLabelColor);

            var slotNameLabelColor = SlotNameTextBox.Text != null && SlotNameTextBox.Text.Length >= 1 ? Game1.textColor : Color.Red;
            Utility.drawTextWithShadow(b, slotNameLabel.name, Game1.smallFont, new Vector2(slotNameLabel.bounds.X, slotNameLabel.bounds.Y), slotNameLabelColor);
        }

        public bool IpIsFormattedCorrectly()
        {
            return TryParseIpAddress(out _, out _);
        }

        public bool TryParseIpAddress(out string url, out int port)
        {
            url = "";
            port = 0;
            if (IpAddressTextBox?.Text == null)
            {
                return false;
            }

            var ipParts = IpAddressTextBox.Text.Split(":");
            var numberParts = ipParts.Length;
            if (numberParts < 2 || numberParts > 3)
            {
                return false;
            }

            var allPartsHaveContent = ipParts.All(x => x.Length > 0);
            if (numberParts == 2)
            {
                if (int.TryParse(ipParts[1], out port))
                {
                    url = ipParts[0];
                    return allPartsHaveContent;
                }
            }

            if (numberParts == 3)
            {
                if (int.TryParse(ipParts[2], out port) && allPartsHaveContent && ipParts[1].Length > 2 && ipParts[1].StartsWith("//"))
                {
                    url = $"{ipParts[0]}:{ipParts[1]}";
                    return true;
                }
            }

            return false;
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            IpAddressTextBox.Hover(x, y);
            SlotNameTextBox.Hover(x, y);
            PasswordTextBox.Hover(x, y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            IpAddressTextBox.Update();
            SlotNameTextBox.Update();
            PasswordTextBox.Update();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Tab)
            {
                if (IpAddressTextBox.Selected)
                {
                    IpAddressTextBox.Selected = false;
                    SlotNameTextBox.SelectMe();
                    return;
                }
                else if (SlotNameTextBox.Selected)
                {
                    SlotNameTextBox.Selected = false;
                    PasswordTextBox.SelectMe();
                    return;
                }
                else if (PasswordTextBox.Selected)
                {
                    PasswordTextBox.Selected = false;
                    IpAddressTextBox.SelectMe();
                    return;
                }
            }
            base.receiveKeyPress(key);
        }

        public override void RefreshFarmTypeButtons()
        {
            farmTypeButtons.Clear();
        }

        private void CreateArchipelagoFields()
        {
            var texture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            CreateIpField(texture);
            CreateSlotNameField(texture);
            CreatePasswordField(texture);
        }

        private void SetupArchipelagoFieldsPositions()
        {
            var xOffset = xPositionOnScreen + spaceToClearSideBorder + borderWidth;
            var yOffset = yPositionOnScreen + spaceToClearTopBorder + borderWidth + ARCHIPELAGO_Y_OFFSET;

            SetupIpFieldPosition(xOffset, yOffset);
            SetupSlotNameFieldPosition(xOffset, yOffset + 64);
            SetupPasswordFieldPosition(xOffset, yOffset + 128);

            skipIntroButton.setPosition(skipIntroButton.bounds.X - 240, skipIntroButton.bounds.Y + 136);
        }

        private void CreateIpField(Texture2D texture)
        {
            IpAddressTextBox = new TextBox(texture, null, Game1.smallFont, Game1.textColor);
            ipAddressCC = new ClickableComponent(Rectangle.Empty, "")
            {
                myID = 536536,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998,
            };
            ipAddressLabel = new ClickableComponent(Rectangle.Empty, "Server");
        }

        private void CreateSlotNameField(Texture2D texture)
        {
            SlotNameTextBox = new TextBox(texture, null, Game1.smallFont, Game1.textColor);
            slotNameCC = new ClickableComponent(Rectangle.Empty, "")
            {
                myID = 537537,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998,
            };
            slotNameLabel = new ClickableComponent(Rectangle.Empty, "Slot");
        }

        private void CreatePasswordField(Texture2D texture)
        {
            PasswordTextBox = new TextBox(texture, null, Game1.smallFont, Game1.textColor);
            passwordCC = new ClickableComponent(Rectangle.Empty, "")
            {
                myID = 538538,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998,
            };
            passwordLabel = new ClickableComponent(Rectangle.Empty, "Password");
        }

        private void SetupIpFieldPosition(int xOffset, int yOffset)
        {
            var xPosition = xOffset + 256 + 64 + 16;
            var yPosition = yOffset - 16;
            IpAddressTextBox.X = xPosition;
            IpAddressTextBox.Y = yPosition;
            IpAddressTextBox.limitWidth = false;

            var ipAddressRectangle = new Rectangle(xPosition, yPosition, 192, 48);
            ipAddressCC.bounds = ipAddressRectangle;

            var languageOffset = LocalizedContentManager.CurrentLanguageCode is LocalizedContentManager.LanguageCode.ru or LocalizedContentManager.LanguageCode.es or LocalizedContentManager.LanguageCode.pt ? -4 : 0;
            var ipAddressLabelPosition = new Rectangle(xOffset + languageOffset + 16 + 192 + 4, yOffset - 8, 1, 1);
            ipAddressLabel.bounds = ipAddressLabelPosition;

            labels.Add(ipAddressLabel);
        }

        private void SetupSlotNameFieldPosition(int xOffset, int yOffset)
        {
            var xPosition = xOffset + 256 + 64 + 16;
            var yPosition = yOffset - 16;
            SlotNameTextBox.X = xPosition;
            SlotNameTextBox.Y = yPosition;
            SlotNameTextBox.limitWidth = false;
            
            var slotNameRectangle = new Rectangle(xPosition, yPosition, 192, 48);
            slotNameCC.bounds = slotNameRectangle;

            var languageOffset = LocalizedContentManager.CurrentLanguageCode is LocalizedContentManager.LanguageCode.ru or LocalizedContentManager.LanguageCode.es or LocalizedContentManager.LanguageCode.pt ? -4 : 0;
            var slotNameLabelPosition = new Rectangle(xOffset + languageOffset + 16 + 192 + 4, yOffset - 8, 1, 1);
            slotNameLabel.bounds = slotNameLabelPosition;

            labels.Add(slotNameLabel);
        }

        private void SetupPasswordFieldPosition(int xOffset, int yOffset)
        {
            var xPosition = xOffset + 256 + 64 + 16;
            var yPosition = yOffset - 16;
            PasswordTextBox.X = xPosition;
            PasswordTextBox.Y = yPosition;
            PasswordTextBox.limitWidth = false;

            var ipAddressRectangle = new Rectangle(xPosition, yPosition, 192, 48);
            passwordCC.bounds = ipAddressRectangle;

            var languageOffset = LocalizedContentManager.CurrentLanguageCode is LocalizedContentManager.LanguageCode.ru or LocalizedContentManager.LanguageCode.es or LocalizedContentManager.LanguageCode.pt ? -4 : 0;
            var passwordLabelPosition = new Rectangle(xOffset + languageOffset + 16 + 192 + 4, yOffset - 8, 1, 1);
            passwordLabel.bounds = passwordLabelPosition;
            
            labels.Add(passwordLabel);
        }

        private void SetDefaultValues(IModHelper modHelper)
        {
            var defaultIp = "archipelago.gg:";
            var defaultSlotName = "";
#if DEBUG
            defaultIp = "localhost:38281";
            defaultSlotName = "Tester";
            SetDebugDefaultValues(modHelper);
#endif
            IpAddressTextBox.Text = defaultIp;
            SlotNameTextBox.Text = defaultSlotName;
            PasswordTextBox.Text = "";
        }

        private void SetDebugDefaultValues(IModHelper modHelper)
        {
            // private TextBox nameBox;
            var nameBoxField = modHelper.Reflection.GetField<TextBox>(this, "nameBox");
            var nameBox = nameBoxField.GetValue();

            // private TextBox farmnameBox;
            var farmnameBoxField = modHelper.Reflection.GetField<TextBox>(this, "farmnameBox");
            var farmnameBox = farmnameBoxField.GetValue();

            // private TextBox favThingBox;
            var favThingBoxField = modHelper.Reflection.GetField<TextBox>(this, "favThingBox");
            var favThingBox = favThingBoxField.GetValue();

            // private bool skipIntro;
            var skipIntroField = modHelper.Reflection.GetField<bool>(this, "skipIntro");

            nameBox.Text = "Tester";
            farmnameBox.Text = "Testing Farm";
            favThingBox.Text = "No Bugs";
            skipIntroField.SetValue(true);
            skipIntroButton.sourceRect.X = 236;
        }
    }
}
