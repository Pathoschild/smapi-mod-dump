/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace CustomWallpaperFramework
{
    public class WallpaperData
    {
        public string id;
        public string texturePath;
        public Texture2D texture;
        public int width = 1;
        public int height = 3;
        public float scale = 4;
        public bool isFloor;
    }
}