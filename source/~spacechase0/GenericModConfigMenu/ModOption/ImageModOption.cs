/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using GenericModConfigMenu.Framework;
using Microsoft.Xna.Framework;

namespace GenericModConfigMenu.ModOption
{
    internal class ImageModOption : BaseModOption
    {
        public string TexturePath { get; }
        public Rectangle? TextureRect { get; }
        public int Scale { get; }

        public override void SyncToMod()
        {
        }

        public override void Save()
        {
        }

        public ImageModOption(string texPath, Rectangle? texRect, int scale, ModConfig mod)
            : base(texPath, "", texPath, mod)
        {
            this.TexturePath = texPath;
            this.TextureRect = texRect;
            this.Scale = scale;
        }
    }
}
