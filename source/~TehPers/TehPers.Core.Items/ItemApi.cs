using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.Core.Helpers.Static;
using TehPers.Core.Items.Managed;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items {
    internal class ItemApi : IItemApi {
        public static Dictionary<IApiManagedObject, int> ItemToIndex { get; } = new Dictionary<IApiManagedObject, int>();
        public static Dictionary<int, IApiManagedObject> IndexToItem { get; } = new Dictionary<int, IApiManagedObject>();
        private static int _nextIndex = 1000000;

        private readonly ITehCoreApi _core;

        public ItemApi(ITehCoreApi core) {
            this._core = core;
        }

        /// <inheritdoc />
        public SObject RegisterItem<T>(T managedItem) where T : IApiManagedObject {
            if (ItemApi.ItemToIndex.ContainsKey(managedItem)) {
                return null;
            }

            // Create a new SObject
            int index = ItemApi._nextIndex++;
            SObject sobj = new SObject(Vector2.Zero, index, 1);

            // Register that index
            ItemApi.ItemToIndex.Add(managedItem, index);
            ItemApi.IndexToItem[index] = managedItem;
            return sobj;
        }

        public void SetTextureForIndex(int index, Texture2D texture) => this.SetTextureForIndex(index, () => new TextureRegion(texture, null));
        public void SetTextureForIndex(int index, Texture2D texture, Rectangle? sourceRectangle) => this.SetTextureForIndex(index, () => new TextureRegion(texture, sourceRectangle));
        public void SetTextureForIndex(int index, in Func<TextureRegion> textureRegionFactory) {
            
        }
    }

    public readonly struct TextureRegion {
        public Texture2D Texture { get; }
        public Rectangle? SourceRectangle { get; }

        public TextureRegion(Texture2D texture, Rectangle? sourceRectangle) {
            this.Texture = texture;
            this.SourceRectangle = sourceRectangle;
        }
    }
}
