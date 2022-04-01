/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace ModularBuildings
{
    public class ModularBuildingPart
    {
        public string partType;
        public int angle;
        public string repeatX;
        public string repeatY;
        public int tileSize;
        public int overhangTiles;
        public int offsetX;
        public int offsetY;
        public string texturePath;
        public Texture2D texture;
    }
}