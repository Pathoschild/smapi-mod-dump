using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MegaStorage.UI
{
    public class AllCategory : ChestCategory
    {
        private readonly Texture2D _sprite;
        
        public AllCategory(int index, string name, int x, int y) : base(index, name, new Vector2(), null, x, y)
        {
            _sprite = MegaStorageMod.Instance.Helper.Content.Load<Texture2D>("Sprites/AllTab.png");
        }

        public override void Draw(SpriteBatch b, int x, int y)
        {
            b.Draw(_sprite, new Vector2(x - 72, y + StartY + Index * Height), new Rectangle(0, 0, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }

        protected override bool BelongsToCategory(Item i)
        {
            return true;
        }
    }
}
