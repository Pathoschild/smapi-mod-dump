/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Objects;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Crafting.Recipes.Parts {
    public class HatRecipePart : IRecipePart {
        private readonly int _index;

        public int Quantity { get; }
        public ISprite Sprite { get; }

        public HatRecipePart(ICoreApi coreApi, int index, int quantity = 1) {
            this._index = index;
            this.Quantity = quantity;
            this.Sprite = coreApi.Drawing.HatSpriteSheet.TryGetSprite(index, FacingDirection.DOWN, out ISprite sprite) ? sprite : throw new ArgumentOutOfRangeException(nameof(index));
        }

        public bool Matches(Item item) {
            return item is Hat hat && hat.ParentSheetIndex == this._index;
        }

        public string GetDisplayName() {
            return new Hat(this._index).DisplayName;
        }

        public bool TryCreateOne(out Item result) {
            result = new Hat(this._index);
            return true;
        }
    }
}