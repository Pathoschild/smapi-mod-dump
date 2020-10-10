/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/skuldomg/freeDusty
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace freeDusty
{
    // Removes the eyes from Dusty's box
    internal class BoxEditor : IAssetEditor
    {
        private IModHelper Helper;
        private string prefix = "";
        private bool eyes = false;

        public BoxEditor(IModHelper helper, string pre = "", bool eyes = false)
        {
            this.Helper = helper;
            prefix = pre;
            this.eyes = eyes;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(prefix + "_town"))
                return true;
            else if (asset.AssetNameEquals(@"/Maps/" + prefix + "_town"))
                return true;

            return false;           
        }
        
        public void Edit<T>(IAssetData asset)
        {
            Texture2D emptyBox = this.Helper.Content.Load<Texture2D>("assets/"+prefix+"Box.png", ContentSource.ModFolder);
            Texture2D eyesBox = this.Helper.Content.Load<Texture2D>("assets/" + prefix + "BoxEyes.png", ContentSource.ModFolder);

            if(!eyes)
                asset.AsImage().PatchImage(emptyBox, targetArea: new Rectangle(192, 0, 16, 16));
            else
                asset.AsImage().PatchImage(eyesBox, targetArea: new Rectangle(192, 0, 16, 16));
        }
    }
}