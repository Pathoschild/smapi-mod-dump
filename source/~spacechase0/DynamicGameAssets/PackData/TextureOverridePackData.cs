/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace DynamicGameAssets.PackData
{
    public class TextureOverridePackData : BasePackData
    {
        public string TargetTexture { get; set; }
        public Rectangle TargetRect { get; set; }
        public string SourceTexture { get; set; }

        public TexturedRect GetCurrentTexture()
        {
            return this.pack.GetTexture(this.SourceTexture, this.TargetRect.Width, this.TargetRect.Height);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext ctx)
        {
            // This is important because the paths need to match exactly.
            // Starting in SDV 1.5.5, these are always '/', not OS-dependent.
            this.TargetTexture = this.TargetTexture.Replace('\\', '/');
        }
    }
}
