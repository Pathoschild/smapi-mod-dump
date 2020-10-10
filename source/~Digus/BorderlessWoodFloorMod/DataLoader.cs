/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace BorderlessWoodFloorMod
{
    class DataLoader : IAssetEditor
    {
        private IModHelper helper;

        public DataLoader(IModHelper helper)
        {
            this.helper = helper;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"TerrainFeatures/Flooring");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals(@"TerrainFeatures/Flooring"))
            {
                Texture2D image = helper.Content.Load<Texture2D>(@"Floors/WoodFloor.png", ContentSource.ModFolder);
                Rectangle targetArea = Game1.getSourceRectForStandardTileSheet(asset.AsImage().Data, 0, 64, 64);
                asset.AsImage().PatchImage(image, null, targetArea, PatchMode.Replace);
            }
        }

    }
}
