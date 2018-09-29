using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreMultiplayerInfo.Helpers;

namespace MoreMultiplayerInfo
{
    public class OptionDisplayAttribute : Attribute
    {
        public string Name { get; }

        public OptionDisplayAttribute(string name)
        {
            Name = name;
        }
    }

    public class OptionsMenu<TOptions> : IClickableMenu
    where TOptions : class, new()
    {
        private readonly IModHelper _helper;
        private readonly long _originPlayerId;
        private TOptions _config;

        private static int OptionHeight => 75;

        private static int BorderWidth = 15;

        private static List<ClickableTextureComponent> Checkboxes { get; set; }
        
        public OptionsMenu(IModHelper helper, int w, int h, long originPlayerId, TOptions options) : base((Game1.viewport.Width / 2) - (w / 2), (Game1.viewport.Height / 2) - (h / 2), w, h, true)
        {
            _helper = helper;
            _originPlayerId = originPlayerId;
            _config = options;
        }

        public override void draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, Game1.pixelZoom);

            var options = _config.GetType().GetProperties();

            Checkboxes = new List<ClickableTextureComponent>();

            for (var idx = 0; idx < options.Length; idx++)
            {
                var option = options[idx];
                var value = option.GetValue(_config, null);

                var optionType = option.PropertyType;

                var displayName = option.Name;

                var optionDisplayAttribute = option.GetCustomAttributes(true).FirstOrDefault(a => (a as OptionDisplayAttribute) != null);

                if (optionDisplayAttribute != null)
                {
                    displayName = ((OptionDisplayAttribute) optionDisplayAttribute).Name;
                }

                DrawOption(idx, displayName, b);

                if (optionType == typeof(bool))
                {
                    DrawBoolOption(idx, (bool) value, option.Name, b);
                }
                else
                {
                    DrawTextOption(idx, value, b);
                }
            }

            base.draw(b);

            drawMouse(b);
        }

        private void DrawOption(int idx, string optionName, SpriteBatch b)
        {
            var xPos = this.xPositionOnScreen + BorderWidth;
            var yPos = this.yPositionOnScreen + 15 + (idx * OptionHeight);

            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), xPos, yPos, width - ( 2* BorderWidth), OptionHeight, Color.White, Game1.pixelZoom, false);

            var textHeight = Game1.smallFont.MeasureString(optionName).Y;

            var textY = yPos +(OptionHeight / 2f) - (textHeight / 2);
            
            b.DrawString(Game1.smallFont, optionName, new Vector2(xPos + BorderWidth, textY), Color.Black);
        }

        private void DrawTextOption(int idx, object value, SpriteBatch b)
        {


        }

        private void DrawBoolOption(int idx, bool value, string propertyName, SpriteBatch b)
        {
            var checkboxSize = 10 * Game1.pixelZoom;

            var xPos = this.xPositionOnScreen + this.width - (2 * BorderWidth) - checkboxSize;
            var yPos = this.yPositionOnScreen + BorderWidth + (idx * OptionHeight) + (OptionHeight / 2) - (checkboxSize / 2);

            var uncheckedSrc = new Rectangle(267, 256, 10, 10);

            var checkedSrc = new Rectangle(338, 494, 12, 12);

            var src = value ? checkedSrc : uncheckedSrc;

            var checkbox = new ClickableTextureComponent(propertyName, new Rectangle(xPos, yPos, checkboxSize, checkboxSize), "", "", Game1.mouseCursors, src, Game1.pixelZoom * 0.75f, false);

            Checkboxes.Add(checkbox);

            checkbox.draw(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (var checkbox in Checkboxes)
            {
                if (checkbox.containsPoint(x, y))
                {
                    ToggleOption(checkbox.name);
                }
            }

            if (upperRightCloseButton.containsPoint(x, y))
            {
                this.exitThisMenu(playSound);

                Game1.onScreenMenus.Remove(this);

                var menu = new PlayerInformationMenu(_originPlayerId, _helper);

                Game1.activeClickableMenu = menu;
            }

            base.receiveLeftClick(x, y, playSound);
        }

        private void ToggleOption(string checkboxName)
        {
            var property = _config.GetType().GetProperties().FirstOrDefault(p => p.Name == checkboxName);

            if (property == null)
            {
                // oops
            }
            else
            {
                property.SetValue(_config, !(bool) property.GetValue(_config, null));
            }


            ConfigHelper.SaveOptions(_config);
        }
    }
}