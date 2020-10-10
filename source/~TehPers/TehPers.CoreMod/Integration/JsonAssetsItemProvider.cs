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
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Integration {
    internal class JsonAssetsItemProvider : IItemProvider {
        private const string JSON_ASSETS_ID = "spacechase0.JsonAssets";

        private readonly ICoreApi _coreApi;
        private readonly IJsonAssetsApi _jsonAssetsApi;

        public JsonAssetsItemProvider(ICoreApi coreApi, IJsonAssetsApi jsonAssetsApi) {
            this._coreApi = coreApi;
            this._jsonAssetsApi = jsonAssetsApi;
        }

        public bool TryCreate(in ItemKey key, out Item item) {
            bool success;
            (success, item) = this.SwitchOnKey<(bool success, Item item)>(key,
                id => (true, new SObject(Vector2.Zero, id, 1)),
                id => (true, new SObject(Vector2.Zero, id)),
                id => (true, new MeleeWeapon(id)),
                id => (true, new Hat(id)),
                () => (false, null)
            );

            return success;
        }

        public bool IsInstanceOf(in ItemKey key, Item item) {
            return this.SwitchOnKey(key,
                id => item is SObject obj && !obj.bigCraftable.Value && obj.ParentSheetIndex == id,
                id => item is SObject obj && obj.bigCraftable.Value && obj.ParentSheetIndex == id,
                id => item is MeleeWeapon wep && wep.InitialParentTileIndex == id,
                id => item is Hat hat && hat.ParentSheetIndex == id,
                () => false
            );
        }

        public bool TryGetSprite(in ItemKey key, out ISprite sprite) {
            bool success;
            (success, sprite) = this.SwitchOnKey(key,
                id => this._coreApi.Drawing.ObjectSpriteSheet.TryGetSprite(id, out ISprite s) ? (true, s) : (false, null),
                id => this._coreApi.Drawing.CraftableSpriteSheet.TryGetSprite(id, out ISprite s) ? (true, s) : (false, null),
                id => this._coreApi.Drawing.WeaponSpriteSheet.TryGetSprite(id, out ISprite s) ? (true, s) : (false, null),
                id => this._coreApi.Drawing.HatSpriteSheet.TryGetSprite(id, FacingDirection.DOWN, out ISprite s) ? (true, s) : (false, null),
                () => (false, null)
            );

            return success;
        }

        public void InvalidateAssets() { }

        private T SwitchOnKey<T>(in ItemKey key, Func<int, T> objectSelector, Func<int, T> bigCraftableSelector, Func<int, T> weaponSelector, Func<int, T> hatSelector, Func<T> @else) {
            // Make sure the key is for JA
            if (!this.IsJsonAssetsKey(key)) {
                return @else();
            }

            // Try to get it as an object
            int index = this._jsonAssetsApi.GetObjectId(key.LocalKey);
            if (index >= 0) {
                return objectSelector(index);
            }

            // Try to get it as a big craftable
            index = this._jsonAssetsApi.GetBigCraftableId(key.LocalKey);
            if (index >= 0) {
                return bigCraftableSelector(index);
            }

            // Try to get it as a weapon
            index = this._jsonAssetsApi.GetWeaponId(key.LocalKey);
            if (index >= 0) {
                return weaponSelector(index);
            }

            // Try to get it as a hat
            index = this._jsonAssetsApi.GetHatId(key.LocalKey);
            if (index >= 0) {
                return hatSelector(index);
            }

            return @else();
        }

        private bool IsJsonAssetsKey(in ItemKey key) {
            // Check if the key is for JA directly
            if (key.OwnerId == JsonAssetsItemProvider.JSON_ASSETS_ID) {
                return true;
            }

            // Check if the key is for a JA content pack
            return this._coreApi.Owner.Helper.ModRegistry.Get(key.OwnerId) is IModInfo ownerInfo && ownerInfo.Manifest.ContentPackFor?.UniqueID == JsonAssetsItemProvider.JSON_ASSETS_ID;
        }
    }
}
