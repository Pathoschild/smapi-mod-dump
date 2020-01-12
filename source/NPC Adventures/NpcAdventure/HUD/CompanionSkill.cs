using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace NpcAdventure.HUD
{
    class CompanionSkill : Internal.IDrawable, Internal.IUpdateable
    {
        private Vector2 framePosition;
        private Vector2 iconPosition;
        private int ticks;
        private Color glowColor;

        public CompanionSkill(string type, string description)
        {
            this.Type = type;
            this.HoverText = description;
        }

        public string Type { get; }
        public int Index { get; set; }
        public bool ShowTooltip { get; private set; }
        public string HoverText { get; set; }
        public bool Glowing { get => this.ticks > 0; }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color glowing = this.ticks > 0 ? this.glowColor : Color.White;
            Rectangle icon;

            switch (this.Type)
            {
                case "doctor":
                    icon = new Rectangle(0, 428, 10, 10);
                    break;
                case "warrior":
                    icon = new Rectangle(120, 428, 10, 10);
                    break;
                case "fighter":
                    icon = new Rectangle(40, 428, 10, 10);
                    break;
                default:
                    return;
            }

            spriteBatch.Draw(Game1.mouseCursors, this.framePosition, new Rectangle(384, 373, 18, 18), glowing * 1f, 0f, Vector2.Zero, 3.4f, SpriteEffects.None, 1f);
            spriteBatch.Draw(Game1.mouseCursors, this.iconPosition, icon, Color.White * 1f, 0f, Vector2.Zero, 2.8f, SpriteEffects.None, 1f);
        }

        public void PerformHoverAction(int x, int y)
        {
            Rectangle frameBounding = new Rectangle((int)this.framePosition.X, (int)this.framePosition.Y, 18 * 4, 18 * 4);

            if (frameBounding.Contains(x, y))
            {
                this.ShowTooltip = true;
                return;
            }

            this.ShowTooltip = false;
        }

        public void Glow(Color color, int duration)
        {
            this.glowColor = color;
            this.ticks = duration * 60;
        }

        internal void UpdatePosition(Vector2 framePosition, Vector2 iconPosition)
        {
            this.framePosition = framePosition;
            this.iconPosition = iconPosition;
        }

        public void Update(UpdateTickedEventArgs e)
        {
            this.ticks = this.ticks > 0 ? --this.ticks : this.ticks;
        }
    }
}
