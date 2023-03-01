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
using StardewModdingAPI.Events;
using StardewValley;

namespace BorderlessWoodFloorMod
{
    class DataLoader
    {
        private IModHelper helper;

        public DataLoader(IModHelper helper)
        {
            this.helper = helper;

            this.helper.Events.Content.AssetRequested += this.Edit;
        }

        public void Edit(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("TerrainFeatures/Flooring"))
            {
                e.Edit(asset =>
                {
                    Texture2D image = helper.ModContent.Load<Texture2D>("Floors/WoodFloor.png");
                    asset.AsImage().PatchImage(image);
                });
            }
        }

    }
}
