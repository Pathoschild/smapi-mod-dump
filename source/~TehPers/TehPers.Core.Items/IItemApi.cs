using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items {
    public interface IItemApi {
        //SObject RegisterItem<T>(T managedItem) where T : IApiManagedObject;

        // void SetTextureForIndex(int index, Texture2D texture);
        // void SetTextureForIndex(int index, Texture2D texture, Rectangle? sourceRectangle);
        // void SetTextureForIndex(int index, in Func<TextureRegion> textureRegionFactory);
    }
}