using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Menu;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace ModSettingsTab.Framework.Components
{
    public class ModManagerHeading : OptionsElement
    {
        private static readonly Rectangle Hl = new Rectangle(325, 318, 11, 18);
        private static readonly Rectangle Hc = new Rectangle(337, 318, 1, 18);
        private static readonly Rectangle Hr = new Rectangle(338, 318, 12, 18);
        private static readonly Rectangle SetButtonSource = new Rectangle(294, 428, 21, 11);
        private static readonly Rectangle DelButtonSource = new Rectangle(322, 498, 12, 12);
        private Rectangle _setButtonBounds;
        private Rectangle _delButtonBounds;

        public ModManagerHeading(string packName) : base(
            packName, "", "", null, 
            32, 16, BaseOptionsModPage.SlotSize.X, BaseOptionsModPage.SlotSize.Y + 4)
        {
            var length = packName.Length;
            Label = length * 2 <= 32 || length <= 25
                ? $"{packName}"
                : $"{packName.Substring(0, 25)}...";
            _setButtonBounds = new Rectangle(Bounds.Width - 256, Bounds.Height/2 - 11, 84, 44);
            _delButtonBounds = new Rectangle(Bounds.Width - 140, Bounds.Height/2 - 12, 48, 48);
            HoverTitle = Helper.I18N.Get("ModManager.HoverSetUp.Title");
            HoverText = Helper.I18N.Get("ModManager.HoverSetUp.Description");
        }
        public override void ReceiveLeftClick(int x, int y)
        {
            if (_setButtonBounds.Contains(x, y))
            {
                Game1.playSound("drumkit6");
                ModManager.ActivePack = Name;
            }
            else if (_delButtonBounds.Contains(x, y))
            {
                ModManager.RemoveModPack(Name);
            }
        }

        public override bool PerformHoverAction(int x, int y)
        {
            return _setButtonBounds.Contains(x, y);
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            b.Draw(Game1.mouseCursors, new Rectangle(slotX + 32, slotY + Bounds.Y, 44, 72), Hl, Color.White);
            b.Draw(Game1.mouseCursors,
                new Rectangle(slotX + 32 + 44, slotY + Bounds.Y, Bounds.Width - 64 - 48 - 44, 72), Hc, Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(slotX + 32 + Bounds.Width - 64 - 48, slotY + Bounds.Y, 48, 72), Hr,
                Color.White);
            SpriteText.drawString(b, Label, slotX + 32 + 44 + 64, slotY + Bounds.Y + 12, 999, Bounds.Width - 64 - 48,
                72, 1f, 0.1f);
            Utility.drawWithShadow(b, Game1.mouseCursors,
                new Vector2(_setButtonBounds.X + slotX, _setButtonBounds.Y + slotY),
                SetButtonSource, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.15f);
            Utility.drawWithShadow(b, Game1.mouseCursors,
                new Vector2(_delButtonBounds.X + slotX, _delButtonBounds.Y + slotY),
                DelButtonSource, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.15f);
        }
    }
}