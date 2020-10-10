/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using StardewModdingAPI;

using Microsoft.Xna.Framework.Graphics;

namespace Entoarox.Utilities.Internals.AssetHandlers
{
    class HelperSheetLoader : IAssetLoader
    {
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("__EMU_HELPER_TILESHEET.png") || asset.AssetNameEquals("Maps/__EMU_HELPER_TILESHEET.png") || asset.AssetNameEquals("__EMU_HELPER_TILESHEET") || asset.AssetNameEquals("Maps/__EMU_HELPER_TILESHEET");
        }
        
        public T Load<T>(IAssetInfo asset)
        {
            return (T)(object)new Texture2D(StardewValley.Game1.graphics.GraphicsDevice, 16, 16);
        }
    }
}
