/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System.Collections.Generic;

namespace ChestFullnessTextures
{
    public class ChestTextureDataShell
    {
        public List<ChestTextureData> Entries;
    }
    public class ChestTextureData
    {
        public string texturePath;
        public int items;
        public int frames;
        public int ticksPerFrame;
        public int xOffset;
        public int yOffset;
        public int tileWidth;
        public int tileHeight;
    }
}