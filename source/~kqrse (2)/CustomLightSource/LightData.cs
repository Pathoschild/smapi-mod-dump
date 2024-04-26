/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;

namespace CustomLightSource
{
    public class LightData
    {
        public Color color;
        public int textureIndex;
        public string texturePath;
        public float radius;
        public XY offset;
        public bool isLamp;
    }

    public class XY
    {
        public float X;
        public float Y;
    }
}