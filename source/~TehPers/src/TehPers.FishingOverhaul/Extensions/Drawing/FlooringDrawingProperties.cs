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
    internal record FlooringDrawingProperties : IDrawingProperties
    {
        public Vector2 SourceSize => new(28f, 26f);

        public Vector2 Offset(float scaleSize)
        {
            return new(32f, 30f);
        }

        public Vector2 Origin(float scaleSize)
        {
            return new(14f, 13f);
        }

        public float RealScale(float scaleSize)
        {
            return 2f * scaleSize;
        }
    }
}