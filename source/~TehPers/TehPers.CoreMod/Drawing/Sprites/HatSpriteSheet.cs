using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal class HatSpriteSheet : SpriteSheet, IHatSpriteSheet {
        public HatSpriteSheet(ITrackedTexture trackedTexture) : base(trackedTexture, 20, 20) { }

        public bool TryGetSprite(int hatId, FacingDirection direction, out ISprite sprite) {
            (bool success, int spriteIndex) = direction.Match<FacingDirection, (bool, int)>()
                .When(FacingDirection.DOWN, () => (true, GetIndexFromYOffset(0)))
                .When(FacingDirection.RIGHT, () => (true, GetIndexFromYOffset(1)))
                .When(FacingDirection.LEFT, () => (true, GetIndexFromYOffset(2)))
                .When(FacingDirection.UP, () => (true, GetIndexFromYOffset(3)))
                .Else((false, 0));

            sprite = default;
            return success && this.TryGetSprite(spriteIndex, out sprite);

            int GetIndexFromYOffset(int yOffset) {
                int spritesPerRow = this.TrackedTexture.CurrentTexture.Width / 20;
                int x = hatId % spritesPerRow;
                int y = hatId / spritesPerRow * 4 + yOffset;
                return y * spritesPerRow + x;
            }
        }
    }
}