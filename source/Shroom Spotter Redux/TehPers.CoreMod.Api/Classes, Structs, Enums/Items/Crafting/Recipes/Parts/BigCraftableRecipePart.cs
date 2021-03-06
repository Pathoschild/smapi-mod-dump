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
using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Recipes;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Api.Items.Crafting.Recipes.Parts {
    public class BigCraftableRecipePart : IRecipePart {
        private readonly int _index;

        public int Quantity { get; }
        public ISprite Sprite { get; }

        public BigCraftableRecipePart(ICoreApi coreApi, int index, int quantity = 1) {
            this._index = index;
            this.Quantity = quantity;
            this.Sprite = coreApi.Drawing.CraftableSpriteSheet.TryGetSprite(index, out ISprite sprite) ? sprite : throw new ArgumentOutOfRangeException(nameof(sprite));
        }

        public bool Matches(Item item) {
            return item is SObject obj && obj.bigCraftable.Value && obj.ParentSheetIndex == this._index;
        }

        public string GetDisplayName() {
            return new SObject(Vector2.Zero, this._index).DisplayName;
        }

        public bool TryCreateOne(out Item result) {
            result = new SObject(Vector2.Zero, this._index);
            return true;
        }
    }
}