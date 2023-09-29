/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Umbrellas
{
    public class UmbrellaData
    {
        public string UmbrellaTexturePath;
        public Texture2D UmbrellaTexture;
        public Vector2 UmbrellaOffset = Vector2.Zero;
        public Dictionary<int, Vector2> FrameOffsets = new();
        public bool LeftHanded = false;

        // Frames 2, 6, 10, and 14 are duplicate frames. All other frames are unique and if not provided, will default to an offset of (0, 0).
        public void ExtrapolateOffsetData()
        {
            for (int i = 0; i < 16; i++)
            {
                if (FrameOffsets.ContainsKey(i))
                    continue;

                switch (i)
                {
                    case not 2 and not 6 and not 10 and not 14:
                    {
                        FrameOffsets[i] = Vector2.Zero;
                        break;
                    }

                    case 2 or 6 or 10 or 14:
                    {
                        FrameOffsets[i] = FrameOffsets[i - 2];
                        break;
                    }
                }
            }
        }
    }
}
