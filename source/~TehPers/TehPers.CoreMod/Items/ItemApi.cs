using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;
using TehPers.CoreMod.Api.Items.Recipes;
using TehPers.CoreMod.Drawing.Sprites;
using TehPers.CoreMod.Items.ItemProviders;

namespace TehPers.CoreMod.Items {
    internal class ItemApi : IItemApi {
        private readonly IApiHelper _coreApiHelper;
        private readonly ItemDelegator _itemDelegator;

        public ICommonItemRegistry CommonRegistry { get; }
        public IItemDelegator Delegator => this._itemDelegator;

        public ItemApi(IApiHelper coreApiHelper, ItemDelegator itemDelegator) {
            this._coreApiHelper = coreApiHelper;
            this._itemDelegator = itemDelegator;
            this.CommonRegistry = new CommonItemRegistry(coreApiHelper, itemDelegator);
        }

        public bool TryCreate(string localKey, out Item item) {
            return this.TryCreate(new ItemKey(this._coreApiHelper.Owner, localKey), out item);
        }

        public bool TryParseKey(string source, out ItemKey key) {
            return this._itemDelegator.TryParseKey(source, out key);
        }

        public bool TryCreate(in ItemKey key, out Item item) {
            foreach (IItemProvider provider in this._itemDelegator.GetItemProviders()) {
                if (provider.TryCreate(key, out item)) {
                    return true;
                }
            }

            item = default;
            return false;
        }

        public bool IsInstanceOf(in ItemKey key, Item item) {
            ItemKey keyCopy = key;
            return this._itemDelegator.GetItemProviders().Any(provider => provider.IsInstanceOf(keyCopy, item));
        }

        public void RegisterProvider(Func<IItemDelegator, IItemProvider> providerFactory) {
            this._itemDelegator.AddProvider(providerFactory);
            this._coreApiHelper.Log("Item provider registered", LogLevel.Trace);
        }

        public string RegisterCraftingRecipe(IRecipe recipe) {
            return this._itemDelegator.RegisterCraftingRecipe(recipe);
        }

        public ISprite CreateSprite(Texture2D texture, Rectangle? sourceRectangle = null) {
            return this._itemDelegator.CustomItemSpriteSheet.Add(this._coreApiHelper, texture, sourceRectangle ?? texture.Bounds);
        }

        public bool TryGetSprite(in ItemKey key, out ISprite sprite) {
            foreach (IItemProvider provider in this._itemDelegator.GetItemProviders()) {
                if (provider.TryGetSprite(key, out sprite)) {
                    return true;
                }
            }

            sprite = default;
            return false;
        }
    }
}