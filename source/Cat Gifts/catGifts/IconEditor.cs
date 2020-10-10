/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/skuldomg/catGifts
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace catGifts
{
    // Replaces achievement stars with cat/dog head temporarily
    internal class IconEditor : IAssetEditor
    {
        private IModHelper Helper;

        public IconEditor(IModHelper helper)
        {
            this.Helper = helper;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"/LooseSprites/Cursors");
        }

        public void Edit<T>(IAssetData asset)
        {
            if(ModEntry.msgDisplayed) { 
                Texture2D icons = this.Helper.Content.Load<Texture2D>(@"assets/catDogIcons.xnb", ContentSource.ModFolder);                        

                if(ModEntry.isCat)
                    asset.AsImage().PatchImage(icons, new Rectangle(0, 0, 16, 16), new Rectangle(294, 392, 16, 16));                
                else
                    asset.AsImage().PatchImage(icons, new Rectangle(16, 0, 16, 16), new Rectangle(294, 392, 16, 16));
            }
            else
            {
                Texture2D icons = this.Helper.Content.Load<Texture2D>(@"assets/stars.xnb", ContentSource.ModFolder);
                asset.AsImage().PatchImage(icons, targetArea: new Rectangle(294, 392, 32, 16));
            }
        }
    }
}