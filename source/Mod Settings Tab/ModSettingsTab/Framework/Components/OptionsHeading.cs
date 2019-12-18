using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsHeading : OptionsElement
    {
        public bool Favorite;
        private static readonly Rectangle Hl = new Rectangle(325, 318, 11, 18);
        private static readonly Rectangle Hc = new Rectangle(337, 318, 1, 18);
        private static readonly Rectangle Hr = new Rectangle(338, 318, 12, 18);
        private static readonly Rectangle Star0 = new Rectangle(310, 392, 16, 16);
        private static readonly Rectangle Star1 = new Rectangle(294, 392, 16, 16);
        private static Rectangle _boundsStar = new Rectangle(92, 36, 32, 32);


        public OptionsHeading(string modId, IManifest manifest, Point slotSize)
            : base("", modId, "", null, 32, 16, slotSize.X, slotSize.Y + 4)
        {
            var length = manifest.Name.Length;
            Label = length * 2 <= 32 || length <= 25
                ? $"{manifest.Name} v.{manifest.Version}"
                : $"{manifest.Name.Substring(0, 25)}... v{manifest.Version}";
            HoverTitle = manifest.Author;
            HoverText = manifest.Description;
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (!_boundsStar.Contains(x,y)) return;
            Game1.playSound("drumkit6");
            base.ReceiveLeftClick(x, y);
            Favorite = !Favorite;
            FavoriteData.ChangeStatus(ModId);
        }

        public override bool PerformHoverAction(int x, int y)
        {
            return _boundsStar.Contains(x, y);
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
            b.Draw(Game1.mouseCursors,new Vector2(slotX + _boundsStar.X,slotY + _boundsStar.Y),Favorite ? Star1 : Star0,Color.White,0f,Vector2.Zero, 2f,SpriteEffects.None,0.1f);
            
        }
    }
}