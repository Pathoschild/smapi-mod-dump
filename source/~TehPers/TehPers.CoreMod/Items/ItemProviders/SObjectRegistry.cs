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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using SObject = StardewValley.Object;

namespace TehPers.CoreMod.Items.ItemProviders {
    internal class SObjectRegistry : ItemRegistry<IModObject>, IAssetEditor {
        public SObjectRegistry(IApiHelper apiHelper, IItemDelegator itemDelegator) : base(apiHelper, itemDelegator) { }

        public override bool IsInstanceOf(in ItemKey key, Item item) {
            return item is SObject obj && !obj.bigCraftable.Value && this.ItemDelegator.TryGetIndex(key, out int index) && item.ParentSheetIndex == index;
        }

        public override void InvalidateAssets() {
            if (this.Managers.Any()) {
                this.ApiHelper.Owner.Helper.Content.InvalidateCache("Data/ObjectInformation");
            }
        }

        protected override ISpriteSheet GetSpriteSheet(ItemKey key, IModObject manager) {
            return this.ApiHelper.CoreApi.Drawing.ObjectSpriteSheet;
        }

        protected override Item CreateSingleItem(ItemKey key, int index) {
            return new SObject(Vector2.Zero, index, 1);
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            return asset.AssetNameEquals("Data/ObjectInformation");
        }

        public void Edit<T>(IAssetData asset) {
            if (!asset.AssetNameEquals("Data/ObjectInformation")) {
                return;
            }

            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            foreach ((ItemKey key, IModObject manager) in this.Managers) {
                if (this.ItemDelegator.TryGetIndex(key, out int index)) {
                    data[index] = manager.GetRawObjectInformation();
                }
            }
        }
    }
}