using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewValley;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsCheckbox : OptionsElement
    {
        private static readonly Rectangle SourceRectUnchecked = new Rectangle(227, 425, 9, 9);
        private static readonly Rectangle SourceRectChecked = new Rectangle(236, 425, 9, 9);
        public bool AsString { get; set; }
        private bool _isChecked;

        public OptionsCheckbox(
            string name,
            string modId,
            string label,
            StaticConfig config,
            Point slotSize)
            : base(name, modId, label, config, 32, slotSize.Y / 2)
        {
            _isChecked = config[name].ToString().ToLower() == "true";
            InfoIconBounds = new Rectangle(0,-8,0,0);
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            Game1.playSound("drumkit6");
            base.ReceiveLeftClick(x, y);
            _isChecked = !_isChecked;
            Config[Name] = AsString ? (JToken) _isChecked.ToString().ToLower() : (JToken) _isChecked;
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            b.Draw(Game1.mouseCursors, new Vector2(slotX + Bounds.X, slotY + Bounds.Y), _isChecked ? SourceRectChecked : SourceRectUnchecked, Color.White * (GreyedOut ? 0.33f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
            base.Draw(b, slotX, slotY);
        }
    }
}