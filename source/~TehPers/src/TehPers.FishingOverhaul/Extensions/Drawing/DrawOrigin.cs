/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace TehPers.FishingOverhaul.Extensions.Drawing
{
    internal record DrawOrigin(Vector2 SourceSize, Vector2 OriginInSource) : IDrawOrigin
    {
        public Vector2 GetTranslation(Vector2 size)
        {
            var scale = size / this.SourceSize;
            var scaledOrigin = this.OriginInSource * scale;
            return -scaledOrigin;
        }
    }
}