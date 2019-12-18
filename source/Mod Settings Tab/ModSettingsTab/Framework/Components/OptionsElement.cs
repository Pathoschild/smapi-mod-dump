using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModSettingsTab.Framework.Interfaces;
using StardewValley;

namespace ModSettingsTab.Framework.Components
{
    /// <summary>
    /// base options element
    /// </summary>
    public abstract class OptionsElement : IModOption
    {
        /// <summary>
        /// active zone
        /// </summary>
        public Rectangle Bounds;

        public static Rectangle InfoIcon = new Rectangle(240, 192, 16, 16);
        private Rectangle _infoIconBounds;


        private string _hoverText;
        private string _hoverTitle;

        /// <summary>
        /// element name (JToken path)
        /// </summary>
        public string Name { get; set; }

        public Point Offset = new Point();

        /// <summary>
        /// mod UniqueId
        /// </summary>
        public string ModId { get; set; }

        /// <summary>
        /// the setting header is displayed in the list
        /// </summary>
        /// <remarks>
        /// Name by default
        /// </remarks>
        public string Label { get; set; }

        public bool ShowTooltip { get; set; }


        public string HoverTitle
        {
            get => _hoverTitle;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _hoverTitle = value.Length > 50 ? value.Substring(0, 50) : value;
                else _hoverTitle = "";
            }
        }

        public string HoverText
        {
            get => _hoverText;
            set => _hoverText = !string.IsNullOrEmpty(value)
                ? Regex.Replace(value.Replace('\n', ' '), @"(.{0,50}[, .!:;，])", "$1\n")
                : "";
        }

        /// <summary>
        /// settings for saving
        /// </summary>
        public StaticConfig Config { get; set; }

        /// <summary>
        /// inactive state
        /// </summary>
        public bool GreyedOut { get; set; }

        public Rectangle InfoIconBounds
        {
            get => _infoIconBounds;
            set
            {
                if (value.IsEmpty)
                {
                    _infoIconBounds = new Rectangle(
                        Bounds.X + Bounds.Width + (int) Math.Ceiling(Game1.smallFont.MeasureString(Label).X) + 12,
                        Bounds.Y + 8, 32, 32);
                    return;
                }

                _infoIconBounds.X += value.X;
                _infoIconBounds.Y += value.Y;
            }
        }

        protected OptionsElement(
            string name,
            string modId,
            string label,
            StaticConfig config,
            int x = 32,
            int y = 16,
            int width = 36,
            int height = 36)
        {
            Bounds = new Rectangle(x, y, width, height);
            Name = name;
            ModId = modId;
            Config = config;
            Label = !string.IsNullOrEmpty(label) ? label.Replace(".", " > ") : "";
            _hoverText = "";
            _hoverTitle = "";
            InfoIconBounds = new Rectangle();
        }

        public virtual void ReceiveLeftClick(int x, int y)
        {
        }

        public virtual void LeftClickHeld(int x, int y)
        {
        }

        public virtual bool PerformHoverAction(int x, int y)
        {
            return ShowTooltip && InfoIconBounds.Contains(x, y);
        }

        public virtual void LeftClickReleased(int x, int y)
        {
        }

        public virtual void ReceiveKeyPress(Keys key)
        {
        }

        public virtual void Draw(SpriteBatch b, int slotX, int slotY)
        {
            Utility.drawTextWithShadow(b, Label, Game1.smallFont,
                new Vector2(slotX + Bounds.X + Bounds.Width + 8 + Offset.X, slotY + Bounds.Y + Offset.Y),
                GreyedOut ? Game1.textColor * 0.33f : Game1.textColor, 1f, 0.1f);
            if (ShowTooltip)
                b.Draw(Game1.mouseCursors,
                    new Rectangle(slotX + InfoIconBounds.X, slotY + InfoIconBounds.Y, InfoIconBounds.Width,
                        InfoIconBounds.Height), InfoIcon,
                    Color.White);
        }
    }
}