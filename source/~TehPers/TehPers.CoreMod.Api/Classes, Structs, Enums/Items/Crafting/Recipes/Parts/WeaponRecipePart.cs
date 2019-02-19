using System;
using StardewValley;
using StardewValley.Tools;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Crafting.Recipes.Parts {
    public class WeaponRecipePart : IRecipePart {
        private readonly int _index;

        public int Quantity { get; }
        public ISprite Sprite { get; }

        public WeaponRecipePart(ICoreApi coreApi, int index, int quantity = 1) {
            this._index = index;
            this.Quantity = quantity;
            this.Sprite = coreApi.Drawing.WeaponSpriteSheet.TryGetSprite(index, out ISprite sprite) ? sprite : throw new ArgumentOutOfRangeException(nameof(index));
        }

        public bool Matches(Item item) {
            return item is MeleeWeapon wep && wep.InitialParentTileIndex == this._index;
        }

        public string GetDisplayName() {
            return new MeleeWeapon(this._index).DisplayName;
        }

        public bool TryCreateOne(out Item result) {
            result = new MeleeWeapon(this._index);
            return true;
        }
    }
}