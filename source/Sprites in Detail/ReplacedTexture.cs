/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BleakCodex/SpritesInDetail
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace SpritesInDetail
{
    internal class ReplacedTexture : Texture2D
    {
        public Texture2D OriginalTexture {get; set;}
        public Texture2D? NewTexture { get; set; }

        public HDTextureInfo HDTextureInfo {get; set;}
        
        public ReplacedTexture(Texture2D originalTexture, Texture2D? newTexture, HDTextureInfo hdTextureInfo, int? width = null, int? height = null) 
            : base(originalTexture.GraphicsDevice, width ?? originalTexture.Width, height ?? originalTexture.Height)
        {
            this.OriginalTexture = originalTexture;
            this.NewTexture = newTexture;
            this.HDTextureInfo = hdTextureInfo;
        }
    }
}