using Microsoft.Xna.Framework.Graphics;

namespace ConvenientChests.CategorizeChests.Interface.Widgets {
    /// <summary>
    /// A button that uses a single TextureRegion to display itself.
    /// </summary>
    class SpriteButton : Button {
        private readonly TextureRegion TextureRegion;

        public bool Visible { get; set; } = true;

        public SpriteButton(TextureRegion textureRegion) {
            TextureRegion = textureRegion;
            Width         = TextureRegion.Width;
            Height        = TextureRegion.Height;
        }

        public override void Draw(SpriteBatch batch) {
            if (!Visible)
                return;

            batch.Draw(TextureRegion.Texture, TextureRegion.Region, GlobalPosition.X, GlobalPosition.Y,
                       TextureRegion.Width, TextureRegion.Height);
        }
    }
}