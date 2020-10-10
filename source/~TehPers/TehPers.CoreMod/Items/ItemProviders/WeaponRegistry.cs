/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.Items.ItemProviders {
    internal class WeaponRegistry : ItemRegistry<IModWeapon>, IAssetEditor {
        public WeaponRegistry(IApiHelper apiHelper, IItemDelegator itemDelegator) : base(apiHelper, itemDelegator) { }

        public override bool IsInstanceOf(in ItemKey key, Item item) {
            return item is MeleeWeapon && this.ItemDelegator.TryGetIndex(key, out int index) && item.ParentSheetIndex == index;
        }

        public override void InvalidateAssets() {
            if (this.Managers.Any()) {
                this.ApiHelper.Owner.Helper.Content.InvalidateCache("Data/weapons");
            }
        }

        protected override ISpriteSheet GetSpriteSheet(ItemKey key, IModWeapon manager) {
            return this.ApiHelper.CoreApi.Drawing.WeaponSpriteSheet;
        }

        protected override Item CreateSingleItem(ItemKey key, int index) {
            return new MeleeWeapon(index);
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals("Data/weapons");
        }

        public void Edit<T>(IAssetData asset) {
            if (!asset.AssetNameEquals("Data/weapons")) {
                return;
            }

            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            foreach ((ItemKey key, IModWeapon manager) in this.Managers) {
                if (this.ItemDelegator.TryGetIndex(key, out int index)) {
                    data[index] = manager.GetRawWeaponInformation();
                }
            }
        }
    }
}