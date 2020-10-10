/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using StardewValley;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Crafting.Recipes.Parts {
    public class ModRecipePart : IRecipePart {
        private readonly ICoreApi _coreApi;
        private readonly ItemKey _key;

        public int Quantity { get; }
        public ISprite Sprite => this._coreApi.Items.TryGetSprite(this._key, out ISprite s) ? s : this._coreApi.Drawing.ObjectSpriteSheet.TryGetSprite(0, out s) ? s : throw new Exception("Unable to retrieve item sprites");

        public ModRecipePart(ICoreApi coreApi, ItemKey key, int quantity = 1) {
            this.Quantity = quantity;
            this._coreApi = coreApi;
            this._key = key;
        }

        public bool Matches(Item item) {
            return this._coreApi.Items.IsInstanceOf(this._key, item);
        }

        public string GetDisplayName() {
            return this._coreApi.Items.TryCreate(this._key, out Item item) ? item.DisplayName : "Invalid item";
        }

        public bool TryCreateOne(out Item result) {
            if (!this._coreApi.Items.TryCreate(this._key, out result)) {
                return false;
            }

            return true;
        }
    }
}
